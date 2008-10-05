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
 * Displaying text
 * 
 * Displaying the debug DrawStatisticsDisplay overlay
 * 
 * Using the Xen.Ex first person free camera
 * 
 */
namespace Tutorials.Tutorial_15
{

	//this application will create a text rectangle,
	//and it will also create a DrawStatisticsDisplay overlay element

	[DisplayName(Name = "Tutorial 15: Text, DrawStatistics and Free Camera")]
	public class Tutorial : Application
	{
		private Camera3D camera;
		private DrawTargetScreen drawToScreen;

		//This is text element will display some custom text in a rectangle
		private TextElementRect yellowElement;
		//this element will display the position of the camera
		private TextElement positionDisplay;

		//this is a special object that displays a large number of debug graphs
		//this is very useful for debugging performance problems at runtime
		private Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay statisticsOverlay;


		protected override void Initialise()
		{
			//DrawStatisticsDisplay requires that resource tracking is enabled
			Resource.EnableResourceTracking();


			//Xen.Ex provides a very useful Camera3D called 'FirstPersonControlledCamera3D'.
			//This camera uses player input to act as a simple first-person style flythrough camera
			Xen.Ex.Camera.FirstPersonControlledCamera3D camera = null;

			//it uses player input, so the UpdateManager must be passed in
			camera = new Xen.Ex.Camera.FirstPersonControlledCamera3D(this.UpdateManager);

			//in this case, we want the z-axis to be the up/down axis (otherwise it's the Y-axis)
			camera.ZAxisUp = true;
			//also it's default is a bit too fast moving
			camera.MovementSensitivity *= 0.1f;

			this.camera = camera;

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);


			//create a large number of actor instance from tutorial 10..
			for (int n = 0; n <= 16; n++)
			{
				//create in a half circle
				float angle = (n / 16.0f) * MathHelper.Pi;
				Vector3 position = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle), 0);

				//not too close together
				position *= 10;

				drawToScreen.Add(new Tutorial_10.Actor(this.Content, position));
			}


			//this element will display the camera position
			positionDisplay = new TextElement();

			//add it to the screen
			drawToScreen.Add(positionDisplay);



			Vector2 sizeInPixels = new Vector2(400, 200);

			//create the text
			this.yellowElement = new TextElementRect(sizeInPixels);
			this.yellowElement.Colour = Color.Yellow;

			//add a bunch of text...
			this.yellowElement.Text.AppendLine(@"This is a text box with a large amount of custom text!");
			this.yellowElement.Text.AppendLine(@"This class is:");
			this.yellowElement.Text.AppendLine(this.GetType().FullName);
			this.yellowElement.Text.AppendLine(@"It is located in assembly:");
			this.yellowElement.Text.AppendLine(this.GetType().Assembly.FullName);
			this.yellowElement.Text.AppendLine();

#if XBOX360
			this.yellowElement.Text.AppendLine(@"Press and hold both thumbsticks to show the debug overlay");
#else
			this.yellowElement.Text.AppendLine(@"Press F12 to show the debug overlay");
#endif


			//align the element to the bottom centre of the screen
			this.yellowElement.VerticalAlignment = VerticalAlignment.Bottom;
			this.yellowElement.HorizontalAlignment = HorizontalAlignment.Centre;

			//centre align the text
			this.yellowElement.TextHorizontalAlignment = TextHorizontalAlignment.Centre;
			//centre the text in the middle of the 400x200 area of the element
			this.yellowElement.TextVerticalAlignment = VerticalAlignment.Centre;

			//add it to the screen
			drawToScreen.Add(yellowElement);




			//create the statistics display
			//this class will query the DrawState for the previous frames DrawStatistics structure.
			//this structure provides a large number of statistics for the drawn frame.
			//The DrawStatisticsDisplay displays some of the more important statistics. It will also
			//display thread activity on the xbox.

			//DrawStatistics is only available in DEBUG xen builds
			//It can be accessed at runtime with DrawState GetPreviousFrameStatistics()

			//at runtime, pressing 'F12' will toggle the overlay (or holding both thumsticks on x360)
			this.statisticsOverlay = new Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay(this.UpdateManager);

			//then add it to the screen
			drawToScreen.Add(statisticsOverlay);
		}

		protected override void InitialisePlayerInput(Xen.Input.PlayerInputCollection playerInput)
		{
			this.yellowElement.Text.AppendLine();

			//if using keyboard/mouse, then centre the mouse each frame
			if (playerInput[PlayerIndex.One].ControlInput == Xen.Input.ControlInput.KeyboardMouse)
			{
				playerInput[PlayerIndex.One].InputMapper.CentreMouseToWindow = true;
				this.yellowElement.Text.AppendLine("Use the mouse and WASD to move the camera");
			}
			else
				this.yellowElement.Text.AppendLine("Use the gamepad to move the camera");

		}

		//load the font used by the text and overlay
		protected override void LoadContent(DrawState state, ContentManager manager)
		{
			//Load a normal XNA sprite font
			SpriteFont xnaSpriteFont = manager.Load<SpriteFont>("Arial");

			//both elements require the font to be set before they are drawn
			this.yellowElement.Font = xnaSpriteFont;
			this.positionDisplay.Font = xnaSpriteFont;
			this.statisticsOverlay.Font = xnaSpriteFont;
		}


		protected override void Draw(DrawState state)
		{
			//get the camera position
			Vector3 cameraPosition;
			camera.GetCameraPosition(out cameraPosition);

			//Set the position text
			positionDisplay.Text.Clear();
			positionDisplay.Text.Append(cameraPosition.X);
			positionDisplay.Text.Append(", ");
			positionDisplay.Text.Append(cameraPosition.Y);
			positionDisplay.Text.Append(", ");
			positionDisplay.Text.Append(cameraPosition.Z);

			//draw everything
			drawToScreen.Draw(state);
		}


		//Override this method to setup the graphics device before the application starts.
		//This method is called before Initialise()
		protected override void SetupGraphicsDeviceManager(GraphicsDeviceManager graphics, ref RenderTargetUsage presentation)
		{
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 768;
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}
	}
}
