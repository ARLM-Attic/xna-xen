﻿using System;
using System.Collections.Generic;
using System.Text;
using Xen.Graphics.State;
using Xen.Graphics;
using Xen.Camera;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Xen.Ex.Graphics.Display;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Xen.Ex.Graphics.Content;

namespace Xen.Ex.Graphics.Processor
{
	//list of particles to be added to the gpu processor
	sealed class ProcessorAddList
	{
		public GpuParticleProcessor processor;
		public Vector4[] AddData;
		public ParticleSpawnValues[] AddDetails;
		public int AddCount;

		public ProcessorAddList(IParticleProcessor processor)
		{
			this.processor = processor as GpuParticleProcessor;
		}
	}

	//lists all the data to be processed in a single frame (render pass)
	sealed class GpuParticleRenderPass
	{
		public int ParticleCount;
		public float ParticleCountF;

		//particles to be added (one for each particle type)
		public ProcessorAddList[] AddLists;

		//particles to be copied
		public Vector4[] CopyData;
		public int CopyCount;

		//data caching the life/age of each particle
		public Vector3[] LifeData;
		public int LifeCount;

		public readonly float[] globals;

		public GpuParticleRenderPass(IParticleProcessor[] processors)
		{
			this.globals = new float[16];
			this.AddLists = new ProcessorAddList[processors.Length+1];
			this.AddLists[0] = new ProcessorAddList(null);
			for (int i = 0; i < processors.Length; i++)
				this.AddLists[i+1] = new ProcessorAddList(processors[i]);
		}
	}

	//the random methods need to lookup a texture, in this case a 128x128 RG32 texture with rand values in Red, rand_smooth in Green
	sealed class RandomValueTexture
	{
		private Texture2D texture;
		public static string LookupName = typeof(RandomValueTexture).FullName + ".Texture";
		public const int Resolution = 128;

		public Texture2D GetTexture(DrawState state)
		{
			if (texture == null)
			{
				texture = state.Application.UserValues[LookupName] as Texture2D;
				if (texture == null)
				{
					texture = CreateTexture(state);
					state.Application.UserValues[LookupName] = texture;
				}
			}
			return texture;
		}

		private Texture2D CreateTexture(DrawState state)
		{
			Texture2D texture;
			ushort[] pixels = new ushort[Resolution * Resolution * 2];

			Random rand = new Random();

			for (int i = 0; i < pixels.Length; )
			{
				//gpu shaders will vary the random numbers by +/- 1/256

				//first values are linear rand
				ushort linearRand = (ushort)(rand.Next(ushort.MaxValue - 512) + 256);

				//second are non linear

				int randBase = rand.Next(ushort.MaxValue - 512) + 256;
				//get a non-linear squared version..
				int rand0 = randBase - ushort.MaxValue / 2;
				rand0 = (rand0 * Math.Abs(rand0)) / (ushort.MaxValue / 2) + ushort.MaxValue / 2;
				//then average it with the original linear
				//so it's half linear, half squared non-linear
				rand0 = (rand0 + randBase) >> 1;

				ushort nonLinearRand = (ushort)rand0;

#if !XBOX360
				pixels[i++] = linearRand;
				pixels[i++] = nonLinearRand;
#else
				//on the xbox, storage order is reversed. Thanks to Arc for spotting this.
				pixels[i++] = nonLinearRand;
				pixels[i++] = linearRand;
#endif
			}

			GraphicsDevice device = state.BeginGetGraphicsDevice(StateFlag.None);
			texture = new Texture2D(device, Resolution, Resolution, 1, TextureUsage.None, SurfaceFormat.Rg32);
			texture.SetData(pixels);

			state.EndGetGraphicsDevice();

			return texture;
		}
	}

	/// <summary>
	/// Particle processor that peforms all particle logic on the GPU, using 16bit floating point buffers
	/// </summary>
	public sealed class GpuParticleProcessor : IParticleProcessor, IDraw
	{
		private ParticleSystemTypeData particleTypeData;

		private static Camera2D camera = new Camera2D();
		//render targets must be double buffered on the PC
		private DrawTargetTexture2D positionSizeBufferA, velocityRotationBufferA, userBufferA, colourBufferA;
		private DrawTargetTexture2D positionSizeBufferB, velocityRotationBufferB, userBufferB, colourBufferB;
		private DrawTargetTexture2D lifeStoreBufferA, lifeStoreBufferB;

		//group all the targets
		private DrawTargetTexture2DGroup mrtGroupA;
		private DrawTargetTexture2DGroup mrtGroupB;

		private bool frameBufferToggle;
		private IParticleProcessor[] allProcessors;

		//vertex buffer that stores indices.. 0,1,2,3... etc
		private IVertices verticesRenderIndex;
		static string vertexObjectNameRenderIndex = typeof(GpuParticleProcessor).FullName + ".VertexBufferRenderIndex";

		private GpuParticleRenderPass[] renderPasses;
		private int renderPassCount;
		private GpuParticleRenderPass activeRenderPass;

		//resolution of the render targets
		private int resolutionX;
		private float resolutionXF;
		private int resolutionY;
		private float resolutionYF;

		/// <summary></summary>
		public float ResolutionX { get { return resolutionXF; } }
		/// <summary></summary>
		public float ResolutionY { get { return resolutionYF; } }
		/// <summary></summary>
		public bool RequiresDrawPass { get { return true; } }

		private Xen.Graphics.Modifier.ScissorModifier scissorTest;
		private ShaderElement backgroundFillPass;
		private RandomValueTexture randomTexture = new RandomValueTexture();

		private GpuParticleProcessorData processorData;
		private Random shaderRandomValues;

		//for storing life data
		private bool renderLifeStoreMode, usesLifeStorage, forceLifeStoreRender;
		private uint maxParticleTimeStep;
		private float systemStepDeltaTime;
		private Vector2[] lifeStepParticleData;

		void IParticleProcessor.Initalise(ParticleSystemTypeData typeData, IParticleProcessor[] allProcessors, bool useColourValues, uint maxLifeTimeSteps, uint timeStepHz, uint maxExpectedCount)
		{
			this.processorData = typeData.RuntimeLogicData.GpuParticleProcessorData;

			//compute resolution to fit the particle count
			this.resolutionX = 2;
			this.resolutionY = 2;

			while (true)
			{
				int count = resolutionX * resolutionY;
				if (count >= maxExpectedCount)
					break;
				if (resolutionY >= resolutionX)
					resolutionX *= 2;
				else
					resolutionY *= 2;
			}

			resolutionXF = (float)resolutionX;
			resolutionYF = (float)resolutionY;

			this.particleTypeData = typeData;

			this.allProcessors = allProcessors;
			this.renderPasses = new GpuParticleRenderPass[4];
			this.shaderRandomValues = new Random();

			this.maxParticleTimeStep = maxLifeTimeSteps;
			this.systemStepDeltaTime = 1.0f / ((float)timeStepHz);

			bool usesUserValues = typeData.RuntimeLogicData.SystemUsesUserValues;
			bool usesLifeOrAge = typeData.RuntimeLogicData.SystemUsesLifeOrAgeValues;

			this.usesLifeStorage = usesLifeOrAge;

			SurfaceFormat vec4Format = SurfaceFormat.HalfVector4;
			if (!ParticleSystem.gpuVTexHalfVec4supported)
				vec4Format = SurfaceFormat.Vector4;

			this.positionSizeBufferA		= new DrawTargetTexture2D(camera, resolutionX, resolutionY, vec4Format);
			this.velocityRotationBufferA	= new DrawTargetTexture2D(camera, resolutionX, resolutionY, vec4Format);

			if (useColourValues)
				this.colourBufferA			= new DrawTargetTexture2D(camera, resolutionX, resolutionY, vec4Format);
			if (usesUserValues)
				this.userBufferA			= new DrawTargetTexture2D(camera, resolutionX, resolutionY, vec4Format);

#if XBOX360
			//technically doing this can produce bugs when the buffers combine to be greater than 10MiB
			//(approx ~320,000 particles)
			//This can happen as the textures are updated in tiles, and it's possible for a particle
			//to be shifted across a tile and recieve an invalid update. However the majority of the time,
			//this would only cause a particle to be a frame behind or ahead. Worst case is the particle
			//either dissapears or is duplicated.

			//it is considered a worthwhile tradeoff for half the render target memory usage

			this.positionSizeBufferB		= positionSizeBufferA;
			this.velocityRotationBufferB	= velocityRotationBufferA;
			this.colourBufferB				= colourBufferA;
			this.userBufferB				= userBufferA;
#else

			this.positionSizeBufferB		= new DrawTargetTexture2D(camera, resolutionX, resolutionY, vec4Format);
			this.velocityRotationBufferB	= new DrawTargetTexture2D(camera, resolutionX, resolutionY, vec4Format);
			
			if (useColourValues)
				this.colourBufferB			= new DrawTargetTexture2D(camera, resolutionX, resolutionY, vec4Format);
			if (usesUserValues)
				this.userBufferB			= new DrawTargetTexture2D(camera, resolutionX, resolutionY, vec4Format);

#endif

			//setup the render target groups (MRT)
			DrawTargetTexture2D[] targets = new DrawTargetTexture2D[2 + (useColourValues ? 1 : 0) + (usesUserValues ? 1 : 0)];

			targets[0] = positionSizeBufferA;
			targets[1] = velocityRotationBufferA;
			if (useColourValues) targets[2] = colourBufferA;
			if (usesUserValues) targets[targets.Length-1] = userBufferA;

			this.mrtGroupA = new DrawTargetTexture2DGroup(camera, targets);

			targets[0] = positionSizeBufferB;
			targets[1] = velocityRotationBufferB;
			if (useColourValues) targets[2] = colourBufferB;
			if (usesUserValues) targets[targets.Length - 1] = userBufferB;

			this.mrtGroupB = new DrawTargetTexture2DGroup(camera, targets);


			this.mrtGroupA.ClearBuffer.Enabled = false;
			this.mrtGroupB.ClearBuffer.Enabled = false;


			if (usesLifeOrAge)
			{
				SurfaceFormat ageFormat = SurfaceFormat.HalfVector2;

				if (!ParticleSystem.gpuVTexHalfVec2supported)
				{
					ageFormat = SurfaceFormat.HalfVector4;

					if (!ParticleSystem.gpuVTexHalfVec4supported)
						ageFormat = SurfaceFormat.Vector4;
				}

#if XBOX360
				lifeStoreBufferA = new DrawTargetTexture2D(camera, resolutionX, resolutionY, ageFormat);
				lifeStoreBufferB = lifeStoreBufferA;
				lifeStoreBufferA.ClearBuffer.Enabled = false;
				
				lifeStoreBufferA.Add(new TexturedElement(lifeStoreBufferA, Vector2.One, true));
#else
				lifeStoreBufferA = new DrawTargetTexture2D(camera, resolutionX, resolutionY, ageFormat);
				lifeStoreBufferB = new DrawTargetTexture2D(camera, resolutionX, resolutionY, ageFormat);
				lifeStoreBufferA.ClearBuffer.Enabled = false;
				lifeStoreBufferB.ClearBuffer.Enabled = false;

				//setup so that when a store buffer is drawn, it copies in the other buffer.

				lifeStoreBufferA.Add(new TexturedElement(lifeStoreBufferB, Vector2.One, true));
				lifeStoreBufferB.Add(new TexturedElement(lifeStoreBufferA, Vector2.One, true));
#endif
			}


			this.scissorTest = new Xen.Graphics.Modifier.ScissorModifier(0, 0, 1, 1);

			//scissor off the MRT, as it uses a fullsize quad to run the particle logic for every particle
			this.mrtGroupA.AddModifier(scissorTest);
			this.mrtGroupB.AddModifier(scissorTest);

			//this is the pass that runs per-frame logic on the particles
			this.backgroundFillPass = new ShaderElement(this.processorData.FrameShader, new Vector2(1, 1), true);

			this.mrtGroupA.Add(backgroundFillPass);
			this.mrtGroupB.Add(backgroundFillPass);

			this.mrtGroupA.Add(this);
			this.mrtGroupB.Add(this);

			if (usesLifeOrAge)
			{
				this.lifeStoreBufferA.Add(this);
#if !XBOX360
				this.lifeStoreBufferB.Add(this);
#endif
			}
		}

		void GetLogicTextures(DrawState state,
			out Texture2D positionSize,
			out Texture2D velocityRotation,
			out Texture2D colour,
			out Texture2D user,
			out Texture2D life)
		{
			colour = null;
			user = null;
			life = null;

			if (frameBufferToggle)
			{
				positionSize = this.positionSizeBufferB.GetTexture(state);
				velocityRotation = this.velocityRotationBufferB.GetTexture(state);
				if (this.colourBufferB != null) colour = this.colourBufferB.GetTexture(state);
				if (this.userBufferB != null) user = this.userBufferB.GetTexture(state);
				if (this.lifeStoreBufferB != null) life = this.lifeStoreBufferB.GetTexture(state);
			}
			else
			{
				positionSize = this.positionSizeBufferA.GetTexture(state);
				velocityRotation = this.velocityRotationBufferA.GetTexture(state);
				if (this.colourBufferA != null) colour = this.colourBufferA.GetTexture(state);
				if (this.userBufferA != null) user = this.userBufferA.GetTexture(state);
				if (this.lifeStoreBufferA != null) life = this.lifeStoreBufferA.GetTexture(state);
			}
		}


		void GetDisplayTextures(
			out Texture2D positionSize,
			out Texture2D velocityRotation,
			out Texture2D colour,
			out Texture2D user)
		{
			colour = null;
			user = null;

			if (!frameBufferToggle)
			{
				positionSize = this.positionSizeBufferB.GetTexture();
				velocityRotation = this.velocityRotationBufferB.GetTexture();
				if (this.colourBufferB != null) colour = this.colourBufferB.GetTexture();
				if (this.userBufferB != null) user = this.userBufferB.GetTexture();
			}
			else
			{
				positionSize = this.positionSizeBufferA.GetTexture();
				velocityRotation = this.velocityRotationBufferA.GetTexture();
				if (this.colourBufferA != null) colour = this.colourBufferA.GetTexture();
				if (this.userBufferA != null) user = this.userBufferA.GetTexture();
			}
		}

		void IParticleProcessor.Update(int particleCount, float particleCountF, float delatTime, float[] globals, CopyAction[] copyActions, int copyActionCount, AddAction[] addActions, int addActionCount, int step)
		{
			//update may be called multiple times for a single frame
			//so store the update data in GpuParticleRenderPass instances

			//instances are reused

			//get a free pass to copy in action data
			if (renderPassCount == renderPasses.Length)
				Array.Resize(ref renderPasses, renderPasses.Length * 2);

			GpuParticleRenderPass pass = renderPasses[renderPassCount];
			if (pass == null)
			{
				//allocate a pass
				pass = new GpuParticleRenderPass(this.allProcessors);
				renderPasses[renderPassCount] = pass;
			}

			//copy in the action data...
			pass.ParticleCount = particleCount;
			pass.ParticleCountF = particleCountF;

			for (int i = 0; i < globals.Length; i++)
				pass.globals[i] = globals[i];

			if (copyActionCount > 0)
			{
				//allocate
				int count = 2;
				while (copyActionCount > count)
					count *= 2; // keep allocations as powers of two

				if (pass.CopyData == null ||
					pass.CopyData.Length < copyActionCount)
					Array.Resize(ref pass.CopyData, count);

				//reset the count
				pass.CopyCount = copyActionCount;

				//write the data
				int i = 0;
				while (i != copyActionCount)
				{
					pass.CopyData[i].X = copyActions[i].indexToF;
					pass.CopyData[i].Y = copyActions[i].indexFromF;

					i++;
					//ZW store the life / age data
				}
			}


			if (addActionCount > 0)
			{
				//count the number of actions for each type
				for (int i = 0; i < addActionCount; i++)
					pass.AddLists[addActions[i].cloneTypeIndex + 1].AddCount++;

				//allocate the storage for each action by type
				for (int i = 0; i < pass.AddLists.Length; i++)
				{
					int listCount = pass.AddLists[i].AddCount;
					pass.AddLists[i].AddCount = 0;

					if (listCount == 0)
						continue;

					Vector4[] list = pass.AddLists[i].AddData;

					if (list != null &&
						list.Length >= listCount)
						continue;

					int count = 4;
					while (listCount > count)
						count *= 2; // keep allocations as powers of two

					if (list == null || list.Length < listCount)
					{
						Array.Resize(ref pass.AddLists[i].AddData, count);

						if (pass.AddLists[i].processor == null)
						{
							//this processor also sets positions
							Array.Resize(ref pass.AddLists[i].AddDetails, count);
						}
					}
				}

				//fill up the data
				for (int i = 0; i < addActionCount; i++)
				{
					ProcessorAddList list = pass.AddLists[addActions[i].cloneTypeIndex + 1];
					int index = list.AddCount;

					list.AddData[index].X = addActions[i].indexF;

					//when cloneFromIndex is -1, cloneFromIndexF stores the starting x coord
					list.AddData[index].Y = addActions[i].cloneFromIndexF;

					//ZW store the life / age data (set in next block, if needed)

					if (addActions[i].cloneFromIndex == -1)
					{
						//store the position of the particle (it's not being cloned)
						list.AddDetails[index] = addActions[i].spawnDetails;
					}

					list.AddCount++;
				}

			}
			//done...

			//except...

			//copy in the data to the 'Life storage' list.
			//this is a separate texture that stores life and age of each particle
			//this is rather inefficient unfortunately

			if (usesLifeStorage && (copyActionCount > 0 || addActionCount > 0))
			{
				//unfortunately, there is a lot of int-to-float conversion here

				//stored step is wrapped to help precision
				float startStep = ((float)(step & (this.maxParticleTimeStep - 1))) * this.systemStepDeltaTime;
				
				//allocate
				int count = 2;
				while (copyActionCount + addActionCount > count)
					count *= 2; // keep allocations as powers of two

				if (pass.LifeData == null ||
					pass.LifeData.Length < copyActionCount + addActionCount)
					Array.Resize(ref pass.LifeData, count);

				count = 2;
				while (pass.ParticleCount > count)
					count *= 2;
				if (lifeStepParticleData == null ||
					lifeStepParticleData.Length < pass.ParticleCount)
					Array.Resize(ref lifeStepParticleData, count);

				count = 0;
				Vector3[] data = pass.LifeData;
				Vector3 v = new Vector3();
				Vector2 store = new Vector2();

				for (int i = 0; i < pass.AddLists.Length; i++)
					pass.AddLists[i].AddCount = 0;


				//move a particle
				for (int i = 0; i < copyActionCount; i++)
				{
					v.X = copyActions[i].indexToF;
					
					//get it's life data
					store = lifeStepParticleData[copyActions[i].indexFrom];
					//store in case it moves again
					lifeStepParticleData[copyActions[i].indexTo] = store;

					v.Y = store.X;
					v.Z = store.Y;
#if DEBUG
					if (v.Y == 0 &&
						v.Z == 0)
						throw new ArgumentException();
#endif

					//a copy will read the life / age from the texture

					//update the CopyData data too
					//as the ZW values need to store this data
					pass.CopyData[count].Z = store.X;
					pass.CopyData[count].W = store.Y;

					data[count++] = v;
				}

				//data is stored as 'index, age, start time'
				for (int i = 0; i < addActionCount; i++)
				{
					v.X = addActions[i].indexF;
					v.Y = ((float)addActions[i].lifeSteps) * this.systemStepDeltaTime;
					v.Z = startStep;

					//update the AddList data too
					//as the ZW values need to store this data

					ProcessorAddList list = pass.AddLists[addActions[i].cloneTypeIndex + 1];

					list.AddData[list.AddCount].Z = v.Y;
					list.AddData[list.AddCount].W = v.Z;
					list.AddCount++;



					store.X = v.Y;
					store.Y = v.Z;

					//need to store this data for particles that are moved (see above)
					lifeStepParticleData[addActions[i].index] = store;

					data[count++] = v;
				}

				pass.LifeCount = count;
			}

			renderPassCount++;
		}

		//a display class wants to draw the particle system
		void IParticleProcessor.DrawCallback(DrawState state, IParticleSystemDrawer drawer, uint particleCount)
		{
			if (particleCount == 0)
				return;

			Texture2D positionTex, velocityTex, colourTex, userValues;
			//get the most recent textures this time

			GetDisplayTextures(out positionTex, out velocityTex, out colourTex, out userValues);

			if (positionTex != null)
				drawer.DrawGpuParticles(state, this.particleTypeData, particleCount, positionTex, velocityTex, colourTex, userValues);
		}

		//prepare to perform the particle logic
		void IParticleProcessor.BeginDrawPass(int pass, float deltaTime, int step)
		{
			float maxTimeStep = ((float)this.maxParticleTimeStep) * this.systemStepDeltaTime;
			float time = ((float)(step & (this.maxParticleTimeStep - 1))) * this.systemStepDeltaTime;
			deltaTime = systemStepDeltaTime;

			activeRenderPass = this.renderPasses[pass];
			frameBufferToggle = !frameBufferToggle;

			if (activeRenderPass == null)
				return; // this can happen when a particle system is disposed

			if (this.processorData.FrameShader != null)
				(this.processorData.FrameShader as GpuParticleShader).SetConstants(activeRenderPass.globals, deltaTime, time, maxTimeStep);
			if (this.processorData.FrameMoveShader != null)
				(this.processorData.FrameMoveShader as GpuParticleShader).SetConstants(activeRenderPass.globals, deltaTime, time, maxTimeStep);
			if (this.processorData.OnceShader != null)
				(this.processorData.OnceShader as GpuParticleShader).SetConstants(activeRenderPass.globals, deltaTime, time, maxTimeStep);
			if (this.processorData.OnceCloneShader != null)
				(this.processorData.OnceCloneShader as GpuParticleShader).SetConstants(activeRenderPass.globals, deltaTime, time, maxTimeStep);

		}

		void IParticleProcessor.Warm(Application application)
		{
			mrtGroupA.Warm(application);
			mrtGroupB.Warm(application);
		}

		//perform the particle logic
		void IParticleProcessor.DrawProcess(DrawState state, int step)
		{
			if (activeRenderPass == null)
				return;

			if (activeRenderPass.ParticleCount > 0)
			{
				//setup scissor rect
				float countF = activeRenderPass.ParticleCountF;
				float lines = (float)Math.Ceiling(countF / resolutionXF);

				scissorTest.Top = 1 - Math.Min(1, lines / resolutionYF);

				//set the background fill textures
				Texture2D posSize, velRot, colourTex, userTex, lifeTex;
				GetLogicTextures(state, out posSize, out velRot, out colourTex, out userTex, out lifeTex);

				(this.processorData.FrameShader as GpuParticleShader).SetTextures(state, posSize, velRot, colourTex, userTex, randomTexture.GetTexture(state), lifeTex);
				(this.processorData.FrameMoveShader as GpuParticleShader).SetTextures(state, posSize, velRot, colourTex, userTex, randomTexture.GetTexture(state), null);

				//draw it
				(frameBufferToggle ? mrtGroupA : mrtGroupB).Draw(state);

				if (usesLifeStorage)
				{
					bool renderStore = activeRenderPass.LifeCount > 0;

					if (renderStore || forceLifeStoreRender)
					{
						this.renderLifeStoreMode = true;
						//draw the life data
						(frameBufferToggle ? lifeStoreBufferA : lifeStoreBufferB).Draw(state);
						this.renderLifeStoreMode = false;

#if !XBOX360
						//force the alternate buffer to update too on the next frame (PC only due to no EDRAM, texture is buffered)
						forceLifeStoreRender = renderStore;
#else
						forceLifeStoreRender = false;
#endif
					}
				}

				activeRenderPass.LifeCount = 0;
				activeRenderPass.CopyCount = 0;
				for (int i = 0; i < activeRenderPass.AddLists.Length; i++)
					activeRenderPass.AddLists[i].AddCount = 0;
			}

			renderPassCount = 0;
		}

		//draw the particle update data
		void IDraw.Draw(DrawState state)
		{
			int copyIndex = 0;

			//keep an application wide cache of the vertices used
			AllocateVertices(state);


			//special mode, renderig the life / age of particles

			if (renderLifeStoreMode)
			{
				DrawLifeAgeToStore(state);
				return;
			}
			//otherwise, render normal

			//move particles
			if (activeRenderPass.CopyCount > 0)
			{
				while (activeRenderPass.CopyCount > 0)
				{
					int count = (this.processorData.FrameMoveShader as GpuParticleShader).SetMoveShaderEnabled(state, this, null, activeRenderPass.CopyData, null, activeRenderPass.CopyCount, copyIndex);

					this.processorData.FrameMoveShader.Bind(state);

					verticesRenderIndex.Draw(state, null, PrimitiveType.PointList, Math.Min(count, activeRenderPass.CopyCount), 0, 0);

					activeRenderPass.CopyCount -= count;
					copyIndex += count;
				}
			}

			//particle are being added?
			for (int i = 0; i < activeRenderPass.AddLists.Length; i++)
			{
				ProcessorAddList list = activeRenderPass.AddLists[i];

				if (list.AddCount > 0)
				{
					copyIndex = 0;

					GpuParticleShader shader = this.processorData.OnceShader as GpuParticleShader;

					//added from a different processor?
					if (list.processor != null)
					{
						shader = this.processorData.OnceCloneShader as GpuParticleShader;

						Texture2D t0, t1, t2, t3, t4;
						list.processor.GetLogicTextures(state, out t0, out t1, out t2, out t3, out t4);
						shader.SetTextures(state, t0, t1, t2, t3, randomTexture.GetTexture(state),t4);
					}
					else
						shader.SetTextures(state, null, null, null, null, randomTexture.GetTexture(state), null);

					while (list.AddCount > 0)
					{
						//draw them
						int count = shader.SetMoveShaderEnabled(state, this, list.processor, list.AddData, list.AddDetails, list.AddCount, copyIndex);

						shader.Bind(state);

						verticesRenderIndex.Draw(state, null, PrimitiveType.PointList, Math.Min(count, list.AddCount), 0, 0);

						list.AddCount -= count;
						copyIndex += count;
					}
				}
			}
		}

		private void DrawLifeAgeToStore(DrawState state)
		{

			//both copying and adding are treated the same way
			//just draw the life / age values directly

			int copyIndex = 0;
			while (activeRenderPass.LifeCount - copyIndex > 0)
			{
				ParticleStoreLife128 shader = state.GetShader<ParticleStoreLife128>();
				shader.InvTargetSize = new Vector2(1.0f / resolutionXF, 1.0f / resolutionYF);
				shader.Indices128.SetArray(activeRenderPass.LifeData, copyIndex);

				shader.Bind(state);
				verticesRenderIndex.Draw(state, null, PrimitiveType.PointList, Math.Min(128, activeRenderPass.LifeCount - copyIndex), 0, 0);

				copyIndex += 128;
			}
		}

		private void AllocateVertices(DrawState state)
		{
			AllocateVertices(state, ref this.verticesRenderIndex);
		}
		internal static void AllocateVertices(DrawState state, ref IVertices vertices)
		{
			if (vertices == null)
			{
				vertices = state.Application.UserValues[vertexObjectNameRenderIndex] as IVertices;

				if (vertices == null)
				{
					//this buffer may also be used for rendering the particles
					Vector4[] vertexData = new Vector4[512];
					for (int i = 0; i < vertexData.Length; i++)
						vertexData[i] = new Vector4((float)i, (float)(i / 4), (float)(i % 4), 0);

					vertices = new Vertices<Vector4>(vertexData);
					vertices.ResourceUsage = ResourceUsage.Points;
					state.Application.UserValues[vertexObjectNameRenderIndex] = vertices;
				}
			}
		}

		bool ICullable.CullTest(ICuller culler)
		{
			return true;
		}

		/// <summary></summary>
		public void Dispose()
		{
			DisposeMember(ref positionSizeBufferA);
			DisposeMember(ref velocityRotationBufferA);
			DisposeMember(ref userBufferA);
			DisposeMember(ref colourBufferA);

			DisposeMember(ref positionSizeBufferB);
			DisposeMember(ref velocityRotationBufferB);
			DisposeMember(ref userBufferB);
			DisposeMember(ref colourBufferB);

			DisposeMember(ref lifeStoreBufferA);
			DisposeMember(ref lifeStoreBufferB);

			DisposeMember(ref mrtGroupA);
			DisposeMember(ref mrtGroupB);

			lifeStepParticleData = null;
			allProcessors = null;
			activeRenderPass = null;
			verticesRenderIndex = null;
			scissorTest = null;
			backgroundFillPass = null;
			randomTexture = null;

			shaderRandomValues = null;

		}

		private void DisposeMember<T>(ref T obj) where T : class, IDisposable
		{
			if (obj != null) 
				obj.Dispose();
			obj = null;
		}
	}
}
