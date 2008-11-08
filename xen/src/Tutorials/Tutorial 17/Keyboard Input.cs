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
using Microsoft.Xna.Framework.Input;


/*
 * This sample demonstrates:
 * 
 * Using the keyboard to read text input
 * 
 */
namespace Tutorials.Tutorial_17
{

	//This is a very simple application, demonstrating reading text input from the keyboard

	[DisplayName(Name = "Tutorial 17: Keyboard text input")]
	public class Tutorial : Application
	{
		private DrawTargetScreen drawToScreen;

		//This is text element will display the custom text
		private TextElementRect textElement;
		private TextElement helpDisplay;

		//this list stores the keys pressed during the frame
		private List<Microsoft.Xna.Framework.Input.Keys> pressedKeys;

		protected override void Initialise()
		{
			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, new Camera3D());

			//this element will display some help text
			helpDisplay = new TextElement();
			helpDisplay.Position = new Vector2(100, -100);


#if XBOX360
			helpDisplay.Text.SetText("Use a chatpad to input text!");
#else
			helpDisplay.Text.SetText("Use the keyboard to input text!");
#endif

			//add it to the screen
			drawToScreen.Add(helpDisplay);



			//create the text
			this.textElement = new TextElementRect(new Vector2(400, 200));
			this.textElement.Colour = Color.Yellow;


			//align the element to the bottom centre of the screen
			this.textElement.VerticalAlignment = VerticalAlignment.Bottom;
			this.textElement.HorizontalAlignment = HorizontalAlignment.Centre;

			//centre align the text
			this.textElement.TextHorizontalAlignment = TextHorizontalAlignment.Centre;
			//centre the text in the middle of the 400x200 area of the element
			this.textElement.TextVerticalAlignment = VerticalAlignment.Centre;

			//add it to the screen
			drawToScreen.Add(textElement);
		}

		//this is where the text reading logic will be put.
		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
			{
				this.Shutdown();
				return;
			}

			//first, choose the keyboard input to use...

			Xen.Input.State.KeyboardInputState keyboard;

			//note that ChatPadState is only available on the xbox
			//and state.KeyboardState is only on windows
#if XBOX360
			keyboard = state.PlayerInput[PlayerIndex.One].ChatPadState;
#else
			keyboard = state.KeyboardState;
#endif

			//in xen, the keyboard state stores a list of Button objects
			//the button struct stores many useful things, such as the duration the
			//button has been held down, etc.

			//for example:
			//this sample will exit if the F10 key is held down for more than 2 seconds
			//(not a very practical example :)
			if (keyboard.KeyState.F10.DownDuration > 2.0f)
			{
				this.Shutdown();
				return;
			}
			//this could also be done with:
			//keyboard[Keys.F10].DownDuration


			//the keyboard also provides a callback method to return a list of the
			//keys that are pressed or held.

			//this method will be used to add the keys to the 'pressedKeys' array

			if (this.pressedKeys == null)
				this.pressedKeys = new List<Microsoft.Xna.Framework.Input.Keys>();

			//
			keyboard.GetPressedKeys(this.KeyPressCallback);
			//note that KeyPressCallback is the callback *method*

			//pressedKeys will not have any keys that have been pressed added to it

			foreach (Microsoft.Xna.Framework.Input.Keys key in pressedKeys)
			{
				//the keyboard class can also translate a Keys enumerator into a
				//character. Eg, Keys.A will become 'a'. It cannot do this for 
				//all keys. (Eg, F1 doesn't have a character)

				char character;

				if (keyboard.TryGetKeyChar(key, out character))
				{
					//we have the character that was pressed.

					//if shift is held, make it upper case
					if (keyboard[Keys.LeftShift] || keyboard[Keys.RightShift])
						character = char.ToUpper(character);

					//now add it to the text string
					this.textElement.Text.Append(character);
				}
				else
				{
					//a special key was pressed...
					if (key == Keys.Back) //backspace
						this.textElement.Text.TrimEnd(1);//trim a character off the end
				}
			}

			//Finally, clear the pressedKeys list (now the keys have been processed)
			pressedKeys.Clear();
		}

		//this is the callback method, which takes the key as a parametre
		private void KeyPressCallback(Microsoft.Xna.Framework.Input.Keys key)
		{
			//add the key to the list
			pressedKeys.Add(key);
		}


		//load the font used by the text and overlay
		protected override void LoadContent(DrawState state, ContentManager manager)
		{
			//Load a normal XNA sprite font
			SpriteFont xnaSpriteFont = manager.Load<SpriteFont>("Arial");

			//both elements require the font to be set before they are drawn
			this.textElement.Font = xnaSpriteFont;
			this.helpDisplay.Font = xnaSpriteFont;
		}


		protected override void Draw(DrawState state)
		{
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

	}
}
