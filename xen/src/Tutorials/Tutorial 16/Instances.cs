using System;
using System.Collections.Generic;
using System.Text;



using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xen.Ex.Material;
using Xen.Graphics.State;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics.Content;


/*
 * This sample demonstrates:
 * 
 * Drawing a mesh with instancing (if supported)
 * 
 * using a Draw proxy to draw instances and perform state batching
 * 
 * Using a Space Partitioning class (StaticBinaryTreePartition)
 * 
 * Visually displaying culling
 * 
 * 
 * This example will run quite slow on systems without hardware instancing support
 */
namespace Tutorials.Tutorial_16
{

	//in this example, many instances of the same mesh will be drawn.
	//however, to make things more flexible, a proxy will be used for each instance.

	//There will be one 'instance mesh drawer', and many instances.

	//the instances will store the world matrix - and will then notify the mesh drawer
	//if it's visible.

	//After all the instances have been culled or 'drawn', the instance mesh drawer
	//will draw all the visible instances in one batch.

	//This way, even if the mesh instances are drawn in an unexpected order,
	//they will still be drawn as a batch.
	//(provided the parent is drawn *after* all instances)

	//Note that using hardware instancing requires special shader instructions.
	//MaterialShader supports hardware instancing, however this sample uses a custom
	//shader to demonstrate the requirements for instancing in the shader.

	//Hardware instancing can be enabled in a MaterialShader by setting it's
	//'UseHardwareInstancing' property to true

	//this class builds a list of visible instances.
	class InstancedMeshGeometry : IDraw
	{
		private Matrix[] instanceMatrices;
		private int instanceCount;
		private IDrawBatch geometry;

		public InstancedMeshGeometry(int maxInstances)
		{
			instanceMatrices = new Matrix[maxInstances];

			//create a sphere for the geometry, as it implements IDrawBatch already
			geometry = new Xen.Ex.Geometry.Sphere(Vector3.One, 2, true, false, false);
		}

		//each instance will call this culling method
		public bool CullTestInstance(ICuller culler, ref Vector3 translation)
		{
			return culler.TestSphere(1,ref translation);
		}

		//when drawing, the instances will call this method
		public void AddInstance(DrawState state)
		{
			//store the instance matrix
			state.GetWorldMatrix(out instanceMatrices[instanceCount]);

			instanceCount++;
		}

		//draw all the instances
		//Note this class must be added after all the instances, to keep drawing in
		//the correct order. Otherwise instance culling may appear a frame delayed.
		public void Draw(DrawState state)
		{
			if (state.SupportsHardwareInstancing)
			{
				//get the instancing shader and bind it
				Shader.Tutorial16_Instance shader = state.GetShader<Shader.Tutorial16_Instance>();
				shader.Bind(state);

				//in this case, Xen.Ex.Geometry.Sphere implements IDrawBatch - allowing drawing a batch easily
				//otherwise, a call to:
				//state.DrawBatch(Vertices, Indices, PrimitiveType, DrawCallback, instanceMatrices, instanceCount);
				//can be made (internally, the Sphere does exactly this)

				//the 'DrawCallback' parametre of the DrawBatch method is an optional delegate callback to cull
				//instances. In this case culling has already been done so it's not needed (it's null).


				//Note that MaterialShader also supports hardware instancing, and could be used
				//in place of the custom shader

				geometry.DrawBatch(state, null, instanceMatrices, instanceCount);
			}
			else
			{
				//bind the non-instancing version of the shader
				Shader.Tutorial16 shader = state.GetShader<Shader.Tutorial16>();
				shader.Bind(state);

				//just draw the instances one by one (much slower)
				for (int i = 0; i < instanceCount; i++)
				{
					state.PushWorldMatrix(ref instanceMatrices[i]);

					geometry.Draw(state);

					state.PopWorldMatrix();
				}
			}

			//reset the counter for the next frame
			instanceCount = 0;
		}

		public bool CullTest(ICuller culler)
		{
			return instanceCount > 0;
		}
	}

	//this is the proxy that adds an instance of the mesh to the drawer
	class MeshInstance : IDraw
	{
		//the parent..
		private readonly InstancedMeshGeometry parent;
		//the position of the instance (a matrix could also be stored)
		private Vector3 translation;


		public MeshInstance(InstancedMeshGeometry parent, Vector3 translation)
		{
			if (parent == null)
				throw new ArgumentNullException();

			this.parent = parent;
			this.translation = translation;
		}


		public void Draw(DrawState state)
		{
			state.PushWorldTranslateMultiply(ref translation);

			//add the instance
			parent.AddInstance(state);

			state.PopWorldMatrix();
		}

		public bool CullTest(ICuller culler)
		{
			return parent.CullTestInstance(culler,ref translation);
		}
	}


	[DisplayName(Name = "Tutorial 16: Instancing and Partitioning")]
	public class Tutorial : Application
	{
		private Camera3D camera;
		private DrawTargetScreen drawToScreen;
		private TextElement statusText;

		protected override void Initialise()
		{
			//create the camera
			Xen.Ex.Camera.FirstPersonControlledCamera3D camera = 
				new Xen.Ex.Camera.FirstPersonControlledCamera3D(this.UpdateManager,Vector3.Zero);
			camera.Projection.FarClip *= 10;

			this.camera = camera;


			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);

			//25,000 instances
			const int instanceCount = 25000;
			const float areaRadius = 500;


			//create the mesh instance drawer, (but add it to the screen later)
			InstancedMeshGeometry meshDrawer = new InstancedMeshGeometry(instanceCount);

			//the instances are added to a StaticBinaryTreePartition, which sorts the items
			//into a binary tree, for more efficient culling.
			//This class assumes it's children do not move (ie they are static)

			Xen.Ex.Scene.StaticBinaryTreePartition sceneTree = new Xen.Ex.Scene.StaticBinaryTreePartition();
			
			//add it to the screen
			drawToScreen.Add(sceneTree);


			//create the instances
			Random random = new Random();

			for (int i = 0; i < instanceCount; i++)
			{
				//create a random position in a sphere
				Vector3 position = new Vector3(	(float)(random.NextDouble()-.5),
												(float)(random.NextDouble()-.5),
												(float)(random.NextDouble()-.5));
				position.Normalize();
				position *= (float)Math.Sqrt(random.NextDouble()) * areaRadius;


				//create the instance
				MeshInstance instance = new MeshInstance(meshDrawer, position);

				//add the instance to the StaticBinaryTreePartition
				sceneTree.Add(instance);
			}

			//now add the drawer (instances will be drawn by the StaticBinaryPartition, before the drawer)
			drawToScreen.Add(meshDrawer);

			//Note that if the StaticBinaryTreePartition was not used, then 
			//in each frame, every single instance would perform a CullTest to the screen
			//CullTests, despite their simplicity can be very costly in large numbers.
			//The StaticBinaryTreePartition will usually perform a maximum number of CullTests
			//that is approximately ~30% the number of children. (in this case, ~8000 tests)
			//At it's best, when it's entirely off or on screen, it will perform only 1 or 2 CullTests.
			
			//The number of cull tests performed will be displayed in debug builds of this tutorial:

			//add some statusText to display on screen to show the stats
			statusText = new TextElement();
			statusText.Position = new Vector2(50, -50);
			drawToScreen.Add(statusText);
		}


		private string cullButtonText;

		protected override void InitialisePlayerInput(Xen.Input.PlayerInputCollection playerInput)
		{
			//generate a string to indicate the button to hold to pause culling
			if (playerInput[PlayerIndex.One].ControlInput == Xen.Input.ControlInput.KeyboardMouse)
			{
				cullButtonText = playerInput[PlayerIndex.One].KeyboardMouseControlMapping.A.ToString();
				playerInput[PlayerIndex.One].InputMapper.CentreMouseToWindow = true;
			}
			else
				cullButtonText = "A";

			cullButtonText = string.Format("Hold '{0}' to pause culling",cullButtonText);
		}

		//load the font used by the status text
		protected override void LoadContent(DrawState state, ContentManager manager)
		{
			statusText.Font = manager.Load<SpriteFont>("Arial");
		}

		protected override void Draw(DrawState state)
		{
			//store the global colour
			state.SetShaderGlobal("colour", Color.Red.ToVector4());

			//set the on screen text
			statusText.Text.Clear();
			//framerate
			statusText.Text += (int)state.ApproximateFrameRate;
			statusText.Text += " fps";

#if DEBUG
			//display some statistics about the render
			DrawStatistics stats;
			state.GetPreviousFrameStatistics(out stats);
			
			statusText.Text += ", ";
			statusText.Text += stats.DefaultCullerTestBoxCount + stats.DefaultCullerTestSphereCount;
			statusText.Text += " cull tests performed, ";
			if (state.SupportsHardwareInstancing)
			{
				statusText.Text += stats.InstancesDrawn;
				statusText.Text += stats.InstancesDrawn == 1 ? " instance" : " instances";
				statusText.Text += " drawn (hardware instancing)";
			}
			else
			{
				statusText.Text += stats.DrawIndexedPrimitiveCallCount;
				statusText.Text += stats.InstancesDrawn == 1 ? " instance" : " instances";
				statusText.Text += " drawn";
			}
#endif
			statusText.Text.AppendLine();
			statusText.Text += cullButtonText;

			//draw everything
			drawToScreen.Draw(state);
		}


		protected override void SetupGraphicsDeviceManager(GraphicsDeviceManager graphics, ref RenderTargetUsage presentation)
		{
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 768;
		}

		protected override void Update(UpdateState state)
		{
			//this code is a debugging 'hack' that is very useful.
			//when 'PauseFrustumCullPlaneUpdates' is true, the Projection class will not update it's
			//cull planes.
			//However you can still move the camera, and the matrices will update.
			//This allows you to visually debug the culling of the sceneTree.
			//While holding the button down, the culling will not change. Moving the camera around
			//will show any objects 'off screen' that are not being correctly culled.

			camera.Projection.PauseFrustumCullPlaneUpdates = (state.PlayerInput[PlayerIndex.One].InputState.Buttons.A);

			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}
	}
}
