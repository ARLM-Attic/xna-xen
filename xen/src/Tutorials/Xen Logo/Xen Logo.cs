using System;
using System.Text;
using System.Collections.Generic;



using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


/*
 * This sample demonstrates displaying the Xen Logo
 * 
 * The Xen Logo is external, in the XenLogo.dll file. These .dlls must be included in the project.
 * It is an animated logo that can be used during application startup, if desired.
 * 
 */
namespace Tutorials.XenLogo
{

	[DisplayName(Name = "Xen Startup Logo")]
	public class Tutorial : Application
	{
		//screen draw target
		private DrawTargetScreen drawToScreen;

		//This is the logo displaying class
		private Xen.Logo.XenLogo xenLogo;

		protected override void Initialise()
		{
			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, new Camera2D());

			//create the logo
			this.xenLogo = new Xen.Logo.XenLogo(this);
			
			//test xenLogo.EffectFinished to determine if the effect has completed.

			//add it to the screen
			this.drawToScreen.Add(xenLogo);
		}

		protected override void Draw(DrawState state)
		{
			//draw the screen / effect
			drawToScreen.Draw(state);
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}

		protected override void SetupGraphicsDeviceManager(GraphicsDeviceManager graphics, ref RenderTargetUsage presentation)
		{
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
		}
	}
}
