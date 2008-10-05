using System;
using System.Collections.Generic;
using System.Text;



using Xen;
using Xen.Camera;
using Xen.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/*
 * Welcome to xen!
 * 
 * 
 * This tutorial is here to show implementing the Application class.
 * It's quite similar to the XNA Game class.
 * 
 * 
 * 
 * This sample demonstrates:
 * 
 * implementing the Application class
 * creating a draw target that draws to the screen
 * setting the clear colour for a draw target
 * 
 */
namespace Tutorials.Tutorial_01
{
	//Staring simple...
	//
	//This is a basic application class implementation.
	//All it does is perform a draw to the screen, and all that does is clear the screen to blue.
	[DisplayName(Name = "Tutorial 01: Game Class")]
	public class Tutorial : Application
	{
		//Anything in xen that can be drawn implements IDraw or has a Draw(DrawState) method
		//
		//This goes from high to low level. So drawing to a surface (such as the screen) is
		//handled by a DrawTarget object.
		//A DrawTarget runs all the logic needed to complete the draw operation.
		//Drawing in xen is very explicit, the call to Draw() will perform the entire draw operation.
		//A DrawTarget stores a list of IDraw objects, these objects are drawn to the target.
		//
		//A DrawTargetScreen is a draw target that draws items directly to the screen.
		//
		//If you wish to have an object drawn to the screen, Add() it to the DrawTargetScreen object.
		//The next tutorial will show this in action...
		//
		//In this tutorial all that will happen is the DrawTarget will clear itself to blue
		//(Most applications will only have one DrawTargetScreen)
		private DrawTargetScreen drawToScreen;


		//This method gets called just before the window is shown, and the device is created
		protected override void Initialise()
		{
			//draw targets usually need a camera.
			//create a 3D camera with default parameters
			Camera3D camera = new Camera3D();

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);

			//Set the screen clear colour to blue
			//(Draw targets have a built in ClearBuffer object)
			drawToScreen.ClearBuffer.ClearColour = Color.DarkBlue;
		}

		//this is the default Update method.
		//Update is called 60 times per second, which is the same rate as player input
		//Note: Player input and Updating is explained in more detail in Tutorial 13
		protected override void Update(UpdateState state)
		{
			//quit when the back button is pressed
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}

		//This is the main application draw method. All drawing code should go in here.
		//Any drawing should be done through, or using the DrawState object.
		//Do not store a reference to a DrawState - if a method doesn't give access to it, you shouldn't be drawing in that method.
		//The most useful GraphicsDevice functionality is covered by the DrawState or other objects in Xen/Xen.Ex
		//The majority of applications shouldn't need to directly access the graphics device.
		protected override void Draw(DrawState state)
		{
			//perform the draw to the screen.
			drawToScreen.Draw(state);

			//at this point the screen has been drawn...
		}
	}
}
