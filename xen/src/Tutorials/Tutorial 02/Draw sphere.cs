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
 * This sample extends from Tutorial_01 (Application class)
 * This sample demonstrates:
 * 
 * drawing objects using the IDraw interface,
 * using a world matrix
 * creating and binding a prebuilt shader
 * using a culler to do on-screen culling
 * using an prebuilt geometry class to draw sphecial geometry
 * 
 */
namespace Tutorials.Tutorial_02
{
	//This is a class that draws a sphere. (It uses a prebuilt Sphere geometry class)
	//This class implements IDraw.
	//It stores a world matrix for the sphere it will draw, a shader used to draw it, and the geometry for the sphere
	class SphereDrawer : IDraw
	{
		//This object stores the geometry of the sphere.
		//Xen.Ex.Geometry.Sphere implements IDraw too.
		//Note however that Sphere *only* draws the geometry. (It doesn't bind a shader, for example)
		private Xen.Ex.Geometry.Sphere sphereGeometry;

		//world matrix (position, rotation, etc) of the sphere
		private Matrix worldMatrix;
		//shader instance used to display the sphere
		private IShader shader;

		//constructor
		public SphereDrawer(Vector3 position)
		{
			//setup the sphere geometry
			Vector3 size = new Vector3(1,1,1);
			//Use the prebuilt sphere geometry class
			sphereGeometry = new Sphere(size, 32);

			//Setup the world matrix
			worldMatrix = Matrix.CreateTranslation(position);

			//Create a lighting shader with some nice looking lighting
			//This is a prebuilt class in Xen.Ex. It is similar to the XNA BasicEffect
			//All shaders, including this one, implement IShader
			MaterialShader material = new MaterialShader();

			Vector3 lightDirection = new Vector3(0.5f,1,-0.5f); //a dramatic direction

			//create a light collection and add a couple of lights to it
			material.Lights = new MaterialLightCollection();
			material.UsePerPixelSpecular = true;
			material.Lights.AddDirectionalLight(true, lightDirection, Color.Gray);//add the first of two light sources
			material.Lights.AddDirectionalLight(true, -lightDirection, Color.DarkSlateBlue);
			material.SpecularColour = Color.LightYellow.ToVector3();//give the material a with a nice sheen

			//store the shader
			this.shader = material;
		}

		//draw the sphere
		public void Draw(DrawState state)
		{
			//push the world matrix, multiplying by the current matrix if there is one
			//this is very similar to openGL glPushMatrix() and glMultMatrix().
			//The DrawState object maintains a world matrix stack. Pushing and Popping this stack is very fast.
			state.PushWorldMatrixMultiply(ref this.worldMatrix);

			//FrustumCull test the sphere (the cull test uses the current world matrix)
			//Culltest will return false if the test fails (in this tutorial false would mean the sphere is off screen)
			//The CullTest method requirs an ICuller to be passed in. Here the state object is used because the 
			//DrawState object implements the ICuller interface (DrawState's culler performs screen culling)
			if (sphereGeometry.CullTest(state))
			{
				//bind the shader. From now on all drawing will use this shader
				shader.Bind(state);

				//draw the sphere geometry
				sphereGeometry.Draw(state);
			}

			//always pop the matrix afterwards
			state.PopWorldMatrix();
		}

		//Anything that implements IDraw (such as this class) also implements ICullable.
		//Sometimes, however, like this case, you want to always return true..
		//It's best not to cull yet, and to cull within the draw method.
		//This is only a good idea for simple classes like this one..
		//Using ICullable allows culling to occur before the Draw method is called,
		//such as in the Draw() method above, in that tutorial the CullTest also 
		//prevents the shader being bound, as well as the geometry being drawn.
		public bool CullTest(ICuller culler)
		{
			//The culler object here is smart. It knows the current draw context,
			//for example it knows the world matrix.

			//Calling culler.TestSphere(1.0f) would cull test a sphere located at the current world position.
			//culler.TestSphere(1, new Vector3(10,0,0)) would test the sphere at a location 10 units to the left of the world position
			//culler.TestWorldSphere(1, Vector3.Zero) would test at the origin (the world matrix would be ignored)

			/*
			//a perfectly valid CullTest here would be:
			
			return culler.TestSphere(this.sphereGeometry.Radius, this.worldMatrix.Translation);
			
			 */

			return true;
		}
	}


	//This class is an application that draws the sphere in the middle of the screen
	[DisplayName(Name = "Tutorial 02: Draw a Sphere")]
	public class Tutorial : Application
	{
		//a DrawTargetScreen is a draw target that draws items directly to the screen.
		//in this case it will only draw a SphereDrawer
		DrawTargetScreen drawToScreen;

		protected override void Initialise()
		{
			//draw targets usually need a camera.
			Camera3D camera = new Camera3D();
			//look at the sphere, which will be at 0,0,0. Look from 0,0,4
			camera.LookAt(Vector3.Zero, new Vector3(0, 0, 4), Vector3.UnitY);

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);

			//create the sphere
			SphereDrawer sphere = new SphereDrawer(Vector3.Zero);

			//add it to be drawn to the screen
			drawToScreen.Add(sphere);
		}

		//main application draw method
		protected override void Draw(DrawState state)
		{
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
