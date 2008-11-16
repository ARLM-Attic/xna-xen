using System;
using System.Collections.Generic;
using System.Text;



using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Geometry;
using Xen.Ex.Material;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/*
 * This sample modifies Tutorial_02 (Draw Sphere)
 * This sample demonstrates:
 * 
 * creating and using a custom shader
 * 
 * ---------------------------------------------------------------
 * Before reading further, please read the comments in 'shader.fx'
 * ---------------------------------------------------------------
 * 
 * 
 * see the 'NEW CODE' comments for code that has changed in this tutorial
 * 
 */
namespace Tutorials.Tutorial_03
{
	//this class is mostly the same as the Draw Sphere tutorial,
	//except the shader is hard coded to the custom shader
	class SphereDrawer : IDraw
	{
		private Xen.Ex.Geometry.Sphere sphereGeometry;
		private Matrix worldMatrix;

		public SphereDrawer(Vector3 position)
		{
			//setup the sphere
			Vector3 size = new Vector3(1,1,1);
			sphereGeometry = new Sphere(size, 32);

			//setup the world matrix
			worldMatrix = Matrix.CreateTranslation(position);
		}

		public void Draw(DrawState state)
		{
			state.PushWorldMatrixMultiply(ref worldMatrix);

			//cull test the sphere
			if (sphereGeometry.CullTest(state))
			{
				//NEW CODE
				//compute a scale value that follows a sin wave
				float scaleValue = (float)Math.Sin(state.TotalTimeSeconds) * 0.5f + 1.0f;

				//the shader class has been generated in the namespace 'Shader', because the filename is 'shader.fx'.
				//The only technique in the file is named 'Tutorial03Technique'.
				//The class that was generated is Shader.Tutorial03Technique:
				Shader.Tutorial03Technique shader = null;

				//It is recommended to use the draw state to get a shared static instance of the shader. 
				//Getting shader instances in this way is highly recommended for most shaders, as it reduces
				//memory usage and live object count. This will boost performance in large projects.
				shader = state.GetShader<Shader.Tutorial03Technique>();
				
				//Set the scale value (scale is declared in the shader source)
				shader.Scale = scaleValue;
				
				//Bind the custom shader instance
				//After the call to Bind(), the shader is in use. There is no Begin/End logic required for shaders
				shader.Bind(state);

				//draw the sphere geometry
				sphereGeometry.Draw(state);
			}

			state.PopWorldMatrix();
		}

		//always draw.. don't cull yet
		public bool CullTest(ICuller culler)
		{
			return true;
		}
	}

	//an application that draws a sphere in the middle of the screen
	[DisplayName(Name = "Tutorial 03: Custom Shader")]
	public class Tutorial : Application
	{
		//a DrawTargetScreen is a draw target that draws items directly to the screen.
		//in this case it will only draw a SphereDrawer
		private DrawTargetScreen drawToScreen;

		protected override void Initialise()
		{
			Camera3D camera = new Camera3D();
			camera.LookAt(Vector3.Zero, new Vector3(0, 0, 4), Vector3.UnitY);

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);
			drawToScreen.ClearBuffer.ClearColour = Color.CornflowerBlue;

			//create the sphere
			SphereDrawer sphere = new SphereDrawer(Vector3.Zero);

			//add it to be drawn to the screen
			drawToScreen.Add(sphere);
		}

		//main application draw method
		protected override void Draw(DrawState state)
		{
			//NEW CODE
			//set the global colour float4 value to 1,0,0,1, which is bright red with an alpha of 1.
			state.SetShaderGlobal("colour", new Vector4(1, 0, 0, 1));

			//draw to the screen.
			drawToScreen.Draw(state);
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}
	}
}
