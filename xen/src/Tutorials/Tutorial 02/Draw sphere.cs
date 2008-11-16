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
 * using an prebuilt geometry class to draw spherical geometry
 * 
 */
namespace Tutorials.Tutorial_02
{
	//This is a class that draws a sphere. (It uses a prebuilt Sphere geometry class)
	//It stores a world matrix for the sphere it will draw, a shader used to draw it and the 
	//geometry for the sphere.
	//
	//This class implements IDraw.
	//Anything in xen that can be drawn implements the IDraw interface
	//
	//Almost every drawing class in xen implements IDraw. This includes the DrawTarget objects,
	//the only exception are vertex buffers. (More on that in tutorial 4)
	//
	//Each DrawTarget stores a list of other IDraw objects. (such as game entities, effects, etc).
	//These objects get drawn to the target during the DrawTarget.Draw() call.
	//If you wish to have an object drawn to the screen, Add() it to the DrawTargetScreen object.
	//This is shown later in this tutorial...

	class SphereDrawer : IDraw
	{
		//This object stores the geometry of a sphere, using a pre-built class in Xen.Ex.
		//Xen.Ex.Geometry.Sphere implements IDraw too.
		//Note however that the Sphere class *only* draws the geometry. (It doesn't bind a shader, for example)
		private Xen.Ex.Geometry.Sphere sphereGeometry;

		//world matrix (position, scale, rotation, etc) of the sphere
		private Matrix worldMatrix;

		//shader instance used to display the sphere.
		//all geometry must be drawn with a shader bound.
		private IShader shader;


		//constructor
		public SphereDrawer(Vector3 position)
		{
			//setup the sphere geometry
			Vector3 size = new Vector3(1,1,1);
			//Use the prebuilt sphere geometry class
			this.sphereGeometry = new Sphere(size, 32);

			//Setup the world matrix
			this.worldMatrix = Matrix.CreateTranslation(position);

			//Create a lighting shader with some nice looking lighting
			//This is a prebuilt class in Xen.Ex. It is similar to the XNA BasicEffect
			//This class implements IShader. All shaders implement IShader.
			MaterialShader material = new MaterialShader();

			Vector3 lightDirection = new Vector3(0.5f,1,-0.5f); //a dramatic direction

			//create a light collection and add a couple of lights to it
			material.Lights = new MaterialLightCollection();
			material.UsePerPixelSpecular = true;
			material.Lights.AmbientLightColour = Color.CornflowerBlue.ToVector3() * 0.5f;	//set the ambient
			material.Lights.AddDirectionalLight(true, lightDirection, Color.Gray);			//add the first of two light sources
			material.Lights.AddDirectionalLight(true, -lightDirection, Color.DarkSlateBlue);
			material.SpecularColour = Color.LightYellow.ToVector3();						//give the material a nice sheen

			//store the shader
			this.shader = material;
		}

		//draw the sphere (This is the method declared in the IDraw interface)
		public void Draw(DrawState state)
		{
			//the DrawState object controls current drawing state for the application.

			//The DrawState uses a number of stacks, it is important to understand how pushing/popping a stack works.

			//First, push the world matrix, multiplying by the current matrix (if there is one).
			//This is very similar to using openGL glPushMatrix() and then glMultMatrix().
			//The DrawState object maintains the world matrix stack, pushing and popping this stack is very fast.
			state.PushWorldMatrixMultiply(ref this.worldMatrix);

			//The next line frustum cull tests the sphere
			//Culltest will return false if the test fails (in this case false would mean the sphere is off screen)
			//The CullTest method requirs an ICuller to be passed in. Here the state object is used because the 
			//DrawState object implements the ICuller interface (DrawState's culler performs screen culling)
			//The cull test uses the current world matrix, so make sure you perform the CullTest after applying any
			//transformations.
			//The CullTest method is defined by the ICullable interface. Any IDraw object also implements ICullable.
			if (sphereGeometry.CullTest(state))
			{
				//the sphere is on screen...

				//bind the shader.
				//Note that if the sphere was off screen, the shader would never be bound,
				//which would save valuable CPU time. (The sphere geometry class assumes a shader is bound)
				//This is why the CullTest() method is separate from the Draw() method.
				shader.Bind(state);

				//Once the call to Bind() has been made, the shaders will be active ('bound')
				//on the graphics card. There is no way to 'unbind' or 'end' the shader.
				//Once bound, that shader is in use - until the point a different shader is bound.

				//draw the sphere geometry
				sphereGeometry.Draw(state);
			}

			//always pop the world matrix afterwards
			state.PopWorldMatrix();
		}

		//Anything that implements IDraw (such as this class) also implements ICullable.
		//this requires an object to implement the CullTest method declared below.
		//CullTest returns true or false.
		//For the majority of cases, CullTest will perform frustum culling ('on-screen culling').
		//A return value of true means on-screen, false means off screen.
		//
		//The 'culler' object passed in is smart. It knows the current draw context,
		//for example it knows the world matrix.
		//
		//In the example above, the Sphere geometry class ('this.sphereGeometry') internally
		//implements CullTest as:
		//
		//return culler.TestSphere(this.radius);
		//
		//The sphere need not know where it is being drawn (ie, it's current world matrix)
		//because the ICuller already knows this.
		public bool CullTest(ICuller culler)
		{
			//in this case, however, simply return true.
			//this means 'always draw'.

			//This is because in this case, a world-matrix will be applied during the
			//draw() method, which will potentially change the result of the sphereGeometry
			//cull test.
			
			//What this means is that:

			//return sphereGeometry.CullTest(culler);

			//would be invalid here.

			//However,
			//culler.TestSphere(this.sphereGeometry.Radius, this.worldMatrix.Translation);
			//would be correct, assuming the world matrix was not scaling/rotating
			
			return true;
		}
	}


	//This class is an application that draws the sphere in the middle of the screen
	[DisplayName(Name = "Tutorial 02: Draw a Sphere")]
	public class Tutorial : Application
	{
		//a DrawTargetScreen is a draw target that draws items directly to the screen.
		//in this case it will only draw a SphereDrawer (the class defined above)
		DrawTargetScreen drawToScreen;

		protected override void Initialise()
		{
			//draw targets usually need a camera.
			Camera3D camera = new Camera3D();

			//look at the sphere, which will be at 0,0,0. Look from 0,0,4
			camera.LookAt(Vector3.Zero, new Vector3(0, 0, 4), Vector3.UnitY);

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);
			drawToScreen.ClearBuffer.ClearColour = Color.CornflowerBlue;

			//create the sphere at 0,0,0
			SphereDrawer sphere = new SphereDrawer(Vector3.Zero);

			//add it to be drawn to the screen
			drawToScreen.Add(sphere);
		}

		//main application draw method
		protected override void Draw(DrawState state)
		{
			//draw to the screen.
			//This causes the sphere to be drawn (because it was added to the screen)
			drawToScreen.Draw(state);
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}
	}
}
