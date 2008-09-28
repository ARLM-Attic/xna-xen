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


/*
 * This sample extends from Tutorial_01 (Application class)
 * This sample demonstrates:
 * 
 * How to load content using a class that implements IContentOwner (part 2)
 * 
 */
namespace Tutorials.Tutorial_09
{
	//This class simply draws an image loaded through the XNA content pipeline.
	//The image is drawn to the centre of the screen.
	//It implements the IContentOwner interface to load the image
	class ImageDisplayer : IDraw, IContentOwner
	{
		//Helper element that will display the image on screen
		private TexturedElement element;

		//construct the displayer.
		//Because this class needs to load content, a ContentRegister is a required
		//constructor parameter. This makes sure it will always load content
		public ImageDisplayer(ContentRegister contentRegister)
		{
			if (contentRegister == null)
				throw new ArgumentNullException();

			//create the element that will display the texture
			Vector2 sizeInPixels = new Vector2(768,384);
			//create the element, but don't set the texture yet.
			this.element = new TexturedElement(sizeInPixels);

			//place the element in the centre of the screen
			this.element.HorizontalAlignment = HorizontalAlignment.Centre;
			this.element.VerticalAlignment = VerticalAlignment.Centre;

			//add this object to the content register
			//items added to a content register do not need to be removed, as they
			//are tracked by a weak reference.
			//LoadContent() will be called whenever the device is created/reset.
			contentRegister.Add(this);

			//At this point, if the device had been created, LoadContent() would have
			//been called already. However in this tutorial, the device hasn't been 
			//created yet - so LoadContent() will be called at a later time.

			//LoadContent() will be called before Draw() is called.
		}

		//Load content
		public void LoadContent(ContentRegister register, DrawState state, ContentManager manager)
		{
			//This is the only place the API is designed to give access to the XNA
			//content manager.
			//This is to encorage keeping all content loading in the one place.

			//load and assign the texture
			this.element.Texture = manager.Load<Texture2D>(@"skyline");
		}

		//for most implementations of IContentOwner, unload content can be left empty.
		//the texture does not need to be set to null here
		public void UnloadContent(ContentRegister register, DrawState state)
		{
		}



		//draw the element on screen
		public void Draw(DrawState state)
		{
			element.Draw(state);
		}

		//always return true, so Draw() is always called
		public bool CullTest(ICuller culler)
		{
			return true;
		}
	}




	//This application class simply creates the image displayer and draws it to the screen
	//The Application class stores it's own ContentRegister (this.Content)
	//Extra content registers can be created as well
	[DisplayName(Name = "Tutorial 09: Content 02")]
	public class Tutorial : Application
	{
		//screen draw target
		private DrawTargetScreen drawToScreen;

		protected override void Initialise()
		{
			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, new Camera3D());
			
			//create the image displayer, passing in a reference
			//to the ContentRegister for this Application instance
			ImageDisplayer imageDisplayer = new ImageDisplayer(this.Content);
			//add it to the screen
			drawToScreen.Add(imageDisplayer);
		}

		protected override void Draw(DrawState state)
		{
			drawToScreen.Draw(state);
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}
	}
}
