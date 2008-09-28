using System;
using System.Collections.Generic;
using System.Text;



using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Graphics.State;
using Xen.Ex.Geometry;
using Xen.Ex.Graphics2D;
using Xen.Ex.Material;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/*
 * This sample extends from Tutorial_02 (Draw Sphere)
 * This sample demonstrates:
 * 
 * Changing the render state
 * 
 */
namespace Tutorials.Tutorial_06
{
	//this class is identical to the SphereDrawer in Tutorial_02,
	//except the draw method now implements different ways to change the alpha blending render state
	class SphereDrawer : IDraw
	{
		private int renderStateMode = 0;


		//draw the sphere with alpha blending
		public void Draw(DrawState state)
		{
			//below are 4 variations on how to set the alpha blending render state
			switch (renderStateMode % 4)
			{
				case 0:
					DrawManual(state);
					break;
				case 1:
					DrawPushPopManual(state);
					break;
				case 2:
					DrawPushPopStatic(state);
					break;
				case 3:
					DrawPushPopStored(state);
					break;
			}

			//cycle the various methods
			renderStateMode++;
		}

		//each of these 4 methods is more efficient than the previous...

		//this method is the manual method where each state is setup peice by peice
		private void DrawManual(DrawState state)
		{
			//manual render state

			//First, take a copy of current render state..
			DeviceRenderState currentState = state.RenderState;

			//The DeviceRenderState structure stores common render state, it's bit-packed so it's small.
			//The majority of render state is stored, with most commonly used xbox-supported state included.
			//The entire structure is 16 bytes (the size of a Vector4)
			//
			//The DeviceRenderState structure stores four smaller structures:
			//
			//	StencilTestState		StencilTest;      (8 bytes)
			//	AlphaBlendState			AlphaBlend;	      (4 bytes)	
			//	AlphaTestState			AlphaTest;        (2 bytes)
			//	DepthColourCullState	DepthColourCull;  (2 bytes)
			//
			//DepthColourCullState combines the three smaller states, Depth testing, Colour masking and FrustumCull mode
			//
			//When geometry is rendered, state.RenderState is compared to the current known device state
			//This comparison is very fast. Any changes detected will be applied at render time.
			//
			//Because of this, chaning state.RenderState is very fast and efficient, as actual render state
			//on the GraphicsDevice doesn't change until the geometry is drawn. No logic is run when chaning
			//the value.
			//
			//In terms of efficiency, think of setting the DeviceRenderState as equivalent assigning integers.


			//Here the alpha blend state is changed manually...

			//reset the state to default (no blending)
			state.RenderState.AlphaBlend = new AlphaBlendState();						//4 bytes assigned to zero, no heap allocations
			//set blending...
			state.RenderState.AlphaBlend.Enabled = true;								//one bit is changed
			state.RenderState.AlphaBlend.SourceBlend = Blend.SourceAlpha;				//4 bits are changed
			state.RenderState.AlphaBlend.DestinationBlend = Blend.InverseSourceAlpha;


			//draw the sphere
			DrawGeometry(state);


			//set the previous state back
			state.SetRenderState(ref currentState);
			//
			//Because state.RenderState is a Property, state.RenderState actually returns a wrapper class (so the 
			//fields can be modified directly). However because of this, you cannot directly set the entire state
			//eg,
			//state.RenderState = currentState;
			//is invalid.
		}


		private void DrawPushPopManual(DrawState state)
		{
			//manual render state, using Push/Pop render state

			//push the render state
			//pusing/popping the render state is very fast, no memory is allocated. Internally just 4 ints are assigned.
			state.PushRenderState();


			//change the alpha blend state (and only the alpha blend state) manually...

			state.RenderState.AlphaBlend = new AlphaBlendState();
			//set blending...
			state.RenderState.AlphaBlend.Enabled = true;
			state.RenderState.AlphaBlend.SourceBlend = Blend.SourceAlpha;
			state.RenderState.AlphaBlend.DestinationBlend = Blend.InverseSourceAlpha;


			//draw the sphere
			DrawGeometry(state);

			
			//pop the render state (sets the render stack back to the pushed state)
			state.PopRenderState();
		}



		private void DrawPushPopStatic(DrawState state)
		{
			//use a static alpha blend render state, using Push/Pop render state

			//push the render state
			state.PushRenderState();

			//change the alpha blend state (and only the alpha blend state) using a static blend state...
			//AlphaBlendState.Alpha is a static already setup with alpha blending
			state.RenderState.AlphaBlend = AlphaBlendState.Alpha;

			//draw the sphere
			DrawGeometry(state);


			//pop the render state
			state.PopRenderState();
		}


		//pre-construct a render state that only changes the alpha blend mode from the default...
		private static DeviceRenderState alphaRenderState = new DeviceRenderState(
																AlphaBlendState.Alpha, 
																new AlphaTestState(), 
																new DepthColourCullState(), 
																new StencilTestState());

		private void DrawPushPopStored(DrawState state)
		{
			//push render state and replace it with a precalculateed render state.
			//Note this sets every render state, not just alpha blending
			//this is the most efficient way to set the entire render state
			state.PushRenderState(ref alphaRenderState);
			
			//draw the sphere
			DrawGeometry(state);

			//pop the render state
			state.PopRenderState();
		}


		//advanced use of render state:
		//
		//Sometimes code is called that changes the render state directly, through the GraphicsDevice.
		//While this type of code is best avoided for performance and consistency reasons, there are
		//going to be cases where this is not possible.
		//
		//In such a case, there are two methods for making the internally tracked render state 'dirty'.
		//
		//state.DirtyInternalRenderState(...)
		//
		//When called, flags are passed in specifying what parts of the internally tracked render state
		//should be considered dirty - or in an unknown state.
		//
		//For example, if some eternal code manually sets the alpha blend mode through the GraphicsDevice,
		//then call
		//state.DirtyInternalRenderState(StateFlag.AlphaBlend);
		//directly after that code has completed. This will tell the state tracker that alpha blending is 
		//dirty (in a state that is no longer known). The next time the device state is updated, all
		//alpha blending state will be updated as well.
		//Calling this method will add overhead, so should be used with caution.
		//
		//The dirty state must also be specified when calling:
		//state.BeginGetGraphicsDevice(StateFlag...);
		//
		//This is the only intended way to safely get the graphics device, and manipulate state.
		//A call to EndGetGraphicsDevice() is required afterwards, although it is not enforced.
		//(Currently, EndGetGraphicsDevice() does nothing, but would be required by a multithreaded DrawState)
		//
		//

		//
		//When drawing geometry manually (using GraphicsDevice.DrawIndexedPrimitves, etc) the GraphicsDevice
		//render state can be updated to match the DrawState.RenderState by calling:
		//
		//state.ApplyRenderStateChanges()
		//
		//This method is called automatically by all classed within xen, so it only needs to be called
		//in the rare event the GraphicsDevice is used directly.
		//However...
		//In the case that a manual call to DrawIndexedPrimitves() is required, the DrawState offers the method:
		//
		//state.DrawVertexBuffer(...)
		//
		//Which takes in a vertex buffer, vertex declaration, index buffer and all the stride/offset/primitve 
		//information that is needed in 95% of cases.
		//This method also automatically calls state.ApplyRenderStateChanges(), so the cases with a need to call 
		//ApplyRenderStateChanges directly should be minimal.



		//the rest of the code in this file is identical to tutorial 02
		//(expect the lighting shader outputs an alpha of 0.75, and a background rect is drawn for contrast...)


		//geometry of the sphere
		private Xen.Ex.Geometry.Sphere sphereGeometry;
		//world matrix (position and rotation) of the sphere
		private Matrix worldMatrix;
		//shader used to display the sphere
		private IShader shader;

		//constructor
		public SphereDrawer(Vector3 position)
		{
			//setup the sphere
			Vector3 size = new Vector3(1,1,1);
			//use a prebuilt sphere geometry class
			sphereGeometry = new Sphere(size, 32);

			//setup the world matrix
			worldMatrix = Matrix.CreateTranslation(position);

			//create a lighting shader with some nice looking lighting
			MaterialShader material = new MaterialShader();
			Vector3 lightDirection = new Vector3(0.5f,1,-0.5f); //a dramatic direction
			material.Lights = new MaterialLightCollection();
			material.Lights.AddDirectionalLight(false, lightDirection, Color.Gray);//two light sources
			material.Lights.AddDirectionalLight(false, -lightDirection, Color.DarkSlateBlue);
			material.SpecularColour = Color.LightYellow.ToVector3();//with a nice sheen
			material.Alpha = 0.75f;

			this.shader = material;
		}

		//draw the sphere
		private void DrawGeometry(DrawState state)
		{
			//push the world matrix, multiplying by the current matrix if there is one
			state.PushWorldMatrixMultiply(ref this.worldMatrix);

			//cull test the sphere
			if (sphereGeometry.CullTest(state))
			{
				//bind the shader
				shader.Bind(state);

				//draw the sphere geometry
				sphereGeometry.Draw(state);
			}

			//always pop the matrix afterwards
			state.PopWorldMatrix();
		}

		//always draw this object.. don't cull yet (cull within in the Draw method)
		public bool CullTest(ICuller culler)
		{
			return true;
		}
	}


	//a application that draws a sphere in the middle of the screen
	[DisplayName(Name = "Tutorial 07: Render State")]
	public class Tutorial : Application
	{
		//a DrawTargetScreen is a draw target that draws items directly to the screen.
		//in this case it will only draw a SphereDrawer
		private DrawTargetScreen drawToScreen;

		protected override void Initialise()
		{
			//draw targets usually need a camera.
			Camera3D camera = new Camera3D();
			//look at the sphere, which will be at 0,0,0
			camera.LookAt(Vector3.Zero, new Vector3(0, 0, 4), Vector3.UnitY);

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);

			//create the sphere
			SphereDrawer sphere = new SphereDrawer(Vector3.Zero);


			//before adding the sphere, add a rect over half the background to show blending is active

			//element covers half the screen
			SolidColourElement element = new SolidColourElement(Color.DarkGray, new Vector2(0.5f, 1), true);
			//element is added before the sphere (so it draws first)
			drawToScreen.Add(element);

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
