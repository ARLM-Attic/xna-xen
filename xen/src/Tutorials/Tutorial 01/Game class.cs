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
 * This tutorial shows how to implement the Application class.
 * (It's quite similar to the XNA Game class)
 * 
 * 
 * 
 * This sample demonstrates:
 * 
 * implementing the Application class
 * creating a draw target that draws to the screen
 * setting the clear colour of the draw target
 * 
 */
namespace Tutorials.Tutorial_01
{
	//Starting simple...
	//
	//This is a basic implementation of the Application class.
	//All it will do is draw to the screen, clearing it to blue.

	[DisplayName(Name = "Tutorial 01: Application Class")]
	public class Tutorial : Application
	{
		//A DrawTarget is a class that performs all the logic needed to complete a draw operation to
		//a surface (such as the screen or a texture).
		//
		//Drawing in xen is very explicit, the call to Draw() will perform the entire draw operation.
		//
		//A DrawTargetScreen is a draw target that draws items directly to the screen.
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
			drawToScreen.ClearBuffer.ClearColour = Color.CornflowerBlue;
		}

		//this is the default Update method.
		//Update() is called 60 times per second, which is the same rate that player input
		//is updated.
		//Note: Player input and Updating is explained in more detail in Tutorial 13
		protected override void Update(UpdateState state)
		{
			//quit when the back button is pressed (escape on the PC)
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}

		//This is the main application draw method. All drawing code should go in here.
		//
		//Any drawing should be done through, or using the DrawState object.
		//Do not store a reference to a DrawState - if a method doesn't give access to it, you shouldn't be drawing in that method.
		//The most useful GraphicsDevice functionality is covered by the DrawState or other objects in Xen/Xen.Ex
		//The vast majority of applications shouldn't need to directly access the graphics device.
		protected override void Draw(DrawState state)
		{
			//perform the draw to the screen.
			drawToScreen.Draw(state);

			//at this point the screen has been drawn...
		}
	}
}
