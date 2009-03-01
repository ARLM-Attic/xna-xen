using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Xen.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Xen.Graphics.State;
using Xen.Camera;

namespace Xen.Ex.Scene
{
	/// <summary>
	/// <para>This is a very simple class that will draw wireframe outlines of cull tests that have been performed</para>
	/// <para>This class is intended to be used as a DrawTarget modifier (<see cref="IBeginEndDraw"/>). Add it to a draw target with <see cref="DrawTarget.AddModifier"/></para>
	/// </summary>
	/// <remarks>
	/// <para>This class works by acting as both a DrawTarget Modifier and a PostCuller cull test primitive</para>
	/// <para>A DrawTarget Modifier is setup/shutdown when the DrawTarget begins/finishes rendering. (see <see cref="IBeginEndDraw"/>)</para>
	/// <para>During the setup phase, this class adds itself as a PostCuller to the DrawTarget (see <see cref="DrawTargetPushPostCuller"/>, this allows the class to record the bounding boxes and spheres used for successful CullTests performed</para>
	/// <para>During the shutdown phase, this class draws the recorded sphers/cubes.</para>
	/// <para>Note: The Camera is not avaliable during cull testing, so the camera must be provided in the constructor of this class. Any CullTests performed with a custom camera will not be visualised correctly by this class</para>
	/// </remarks>
	public sealed class CullTestVisualizer : IBeginEndDraw, ICullPrimitive
	{
		private readonly List<Vector4> spheres;
		private readonly List<Matrix> cubes;
		private ICamera camera;
		private IVertices cubeVS, sphereVS;
		private bool enabled, enabledBuffer;

		private static string cubeVSid = typeof(CullTestVisualizer).FullName + ".cubeVS";
		private static string sphereVSid = typeof(CullTestVisualizer).FullName + ".sphereVS";

		/// <summary>
		/// <para>Construct the Visualizer. A camera must be provided, as CullTest primitives do not have access to the camera.</para>
		/// <para>Add the constructed instance of this class to a DrawTarget as a Modifier using <see cref="DrawTarget.AddModifier"/></para>
		/// <para>The camera should match the camera used by the DrawTarget</para>
		/// </summary>
		/// <param name="camera"></param>
		public CullTestVisualizer(ICamera camera)
		{
			if (camera == null)
				throw new ArgumentNullException();

			this.camera = camera;
			this.enabled = true;
			this.spheres = new List<Vector4>();
			this.cubes = new List<Matrix>();
		}

		/// <summary>
		/// <para>Gets/Sets the camera being used to display the recorded CullTests</para>
		/// <para>Note: This class only tracks CullTest shapes during rendering, it does not track the active camera during a cull test</para>
		/// <para>This camera should match the camera used by the DrawTarget</para>
		/// </summary>
		public ICamera Camera { get { return camera; } set { if (value == null) throw new ArgumentNullException(); camera = value; } }

		/// <summary>
		/// <para>Gets/Set if the visualization is enabled</para>
		/// </summary>
		public bool Enabled { get { return enabled; } set { enabled = value; } }

		/// <summary>
		/// Begin the modifier (This method is called by the DrawTarget)
		/// </summary>
		/// <param name="state"></param>
		public void Begin(DrawState state)
		{
			enabledBuffer = enabled;

			spheres.Clear();
			cubes.Clear();

			if (enabledBuffer)
				state.PushPostCuller(this);
		}

		/// <summary>
		/// End the modifier (This method is called by the DrawTarget)
		/// </summary>
		/// <param name="state"></param>
		public void End(DrawState state)
		{
			if (enabledBuffer)
				state.PopPostCuller();

			if (cubes.Count > 0 ||
				spheres.Count > 0)
			{
				state.PushCamera(camera);

				state.PushRenderState();
				state.RenderState.DepthColourCull.DepthWriteEnabled = false;
				state.RenderState.AlphaBlend = AlphaBlendState.Alpha;

				Xen.Ex.Shaders.FillSolidColour shader = state.GetShader<Xen.Ex.Shaders.FillSolidColour>();
				shader.FillColour = new Vector4(1,1,1,0.25f);
				shader.Bind(state);

				GenCubeVS(state);
				GenSphereVS(state);

				Matrix mat;
				for (int i = 0; i < cubes.Count; i++)
				{
					mat = cubes[i];
					state.PushWorldMatrix(ref mat);

					cubeVS.Draw(state, null, PrimitiveType.LineList);

					state.PopWorldMatrix();
				}

				mat = Matrix.Identity;
				Vector4 v;
				for (int i = 0; i < spheres.Count; i++)
				{
					v = spheres[i];

					mat.M11 = v.W;
					mat.M22 = v.W;
					mat.M33 = v.W;
					mat.M41 = v.X;
					mat.M42 = v.Y;
					mat.M43 = v.Z;

					state.PushWorldMatrix(ref mat);

					sphereVS.Draw(state, null, PrimitiveType.LineList);

					state.PopWorldMatrix();
				}

				state.PopRenderState();
				state.PopCamera();
			}
		}

		private void GenCubeVS(DrawState state)
		{
			if (cubeVS != null) return;
			cubeVS = state.UserValues[cubeVSid] as IVertices;
			if (cubeVS != null) return;

			//cube outlines, between 0,0,0 and 1,1,1
			cubeVS = new Vertices<Vector3>(
				new Vector3(0,0,0),new Vector3(1,0,0),new Vector3(0,1,0),new Vector3(1,1,0),
				new Vector3(0,0,1),new Vector3(1,0,1),new Vector3(0,1,1),new Vector3(1,1,1),

				new Vector3(0,0,0),new Vector3(0,1,0),new Vector3(1,0,0),new Vector3(1,1,0),
				new Vector3(0,0,1),new Vector3(0,1,1),new Vector3(1,0,1),new Vector3(1,1,1),
				
				new Vector3(0,0,0),new Vector3(0,0,1),new Vector3(1,0,0),new Vector3(1,0,1),				
				new Vector3(0,1,0),new Vector3(0,1,1),new Vector3(1,1,0),new Vector3(1,1,1)
			);
			state.UserValues[cubeVSid] = cubeVS;
		}

		private void GenSphereVS(DrawState state)
		{
			if (sphereVS != null) return;
			sphereVS = state.UserValues[sphereVSid] as Vertices<Vector3>;
			if (sphereVS != null) return;

			const int subdiv = 64;

			int index = 0;
			Vector3[] verts = new Vector3[(subdiv + 1) * 6];

			for (int axis = 0; axis < 3; axis++)
			{
				for (int i = 0; i <= subdiv; i++)
				{
					float f = (((float)i) / ((float)subdiv)) * MathHelper.TwoPi;
					float fp = (((float)i-1) / ((float)subdiv)) * MathHelper.TwoPi;

					float x = (float)Math.Cos(f);
					float y = (float)Math.Sin(f);
					float xp = (float)Math.Cos(fp);
					float yp = (float)Math.Sin(fp);

					switch (axis)
					{
						case 0:
							verts[index++] = new Vector3(xp, yp, 0);
							verts[index++] = new Vector3(x, y, 0);
							break;
						case 1:
							verts[index++] = new Vector3(xp, 0, yp);
							verts[index++] = new Vector3(x, 0, y);
							break;
						case 2:
							verts[index++] = new Vector3(0, xp, yp);
							verts[index++] = new Vector3(0, x, y);
							break;
					}
				}
			}


			//sphere outlines, between -1,-1,-1 and 1,1,1
			sphereVS = new Vertices<Vector3>(verts);
			state.UserValues[sphereVSid] = sphereVS;
		}


		#region ICullPrimitive Members

		bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
			Matrix mat = Matrix.Identity;
			mat.M41 = min.X;
			mat.M42 = min.Y;
			mat.M43 = min.Z;

			mat.M11 = max.X - min.X;
			mat.M22 = max.Y - min.Y;
			mat.M33 = max.Z - min.Z;

			Matrix.Multiply(ref mat, ref world, out mat);

			cubes.Add(mat);
			return true;
		}

		bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
			Matrix mat = Matrix.Identity;
			mat.M41 = min.X;
			mat.M42 = min.Y;
			mat.M43 = min.Z;

			mat.M11 = max.X - min.X;
			mat.M22 = max.Y - min.Y;
			mat.M33 = max.Z - min.Z;

			Matrix.Multiply(ref mat, ref world, out mat);

			cubes.Add(mat);
			return true;
		}

		bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max)
		{
			Matrix mat = Matrix.Identity;
			mat.M41 = min.X;
			mat.M42 = min.Y;
			mat.M43 = min.Z;

			mat.M11 = max.X - min.X;
			mat.M22 = max.Y - min.Y;
			mat.M33 = max.Z - min.Z;

			cubes.Add(mat);
			return true;
		}

		bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max)
		{
			Matrix mat = Matrix.Identity;
			mat.M41 = min.X;
			mat.M42 = min.Y;
			mat.M43 = min.Z;

			mat.M11 = max.X - min.X;
			mat.M22 = max.Y - min.Y;
			mat.M33 = max.Z - min.Z;

			cubes.Add(mat);
			return true;
		}

		bool ICullPrimitive.TestWorldSphere(float radius, ref Vector3 position)
		{
			spheres.Add(new Vector4(position.X,position.Y,position.Z,radius));
			return true;
		}

		bool ICullPrimitive.TestWorldSphere(float radius, Vector3 position)
		{
			spheres.Add(new Vector4(position.X, position.Y, position.Z, radius));
			return true;
		}

		ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
			Matrix mat = Matrix.Identity;
			mat.M41 = min.X;
			mat.M42 = min.Y;
			mat.M43 = min.Z;

			mat.M11 = max.X - min.X;
			mat.M22 = max.Y - min.Y;
			mat.M33 = max.Z - min.Z;

			Matrix.Multiply(ref mat, ref world, out mat);

			cubes.Add(mat);
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
			Matrix mat = Matrix.Identity;
			mat.M41 = min.X;
			mat.M42 = min.Y;
			mat.M43 = min.Z;

			mat.M11 = max.X - min.X;
			mat.M22 = max.Y - min.Y;
			mat.M33 = max.Z - min.Z;

			Matrix.Multiply(ref mat, ref world, out mat);

			cubes.Add(mat);
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max)
		{
			Matrix mat = Matrix.Identity;
			mat.M41 = min.X;
			mat.M42 = min.Y;
			mat.M43 = min.Z;

			mat.M11 = max.X - min.X;
			mat.M22 = max.Y - min.Y;
			mat.M33 = max.Z - min.Z;

			cubes.Add(mat);
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max)
		{
			Matrix mat = Matrix.Identity;
			mat.M41 = min.X;
			mat.M42 = min.Y;
			mat.M43 = min.Z;

			mat.M11 = max.X - min.X;
			mat.M22 = max.Y - min.Y;
			mat.M33 = max.Z - min.Z;

			cubes.Add(mat);
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, ref Vector3 position)
		{
			spheres.Add(new Vector4(position.X, position.Y, position.Z, radius));
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, Vector3 position)
		{
			spheres.Add(new Vector4(position.X, position.Y, position.Z, radius));
			return ContainmentType.Contains;
		}

		#endregion
	}
}
