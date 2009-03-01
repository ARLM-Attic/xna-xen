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
 * This sample extends from Tutorial_10 (Model Instance)
 * This sample demonstrates:
 * 
 * Using a BatchModel and BatchModelInstance to draw a large number of models
 * 
 */
namespace Tutorials.Tutorial_24
{
	//this is a very simple tutorial.
	//it shows how BatchModel can be used to draw a large number of models in an efficient manner

	//note, batch model does not support animation

	class Actor : IDraw
	{
		private Matrix worldMatrix;
		private readonly BatchModelInstance instance;

		public Actor(BatchModel model, Vector3 position)
		{
			this.instance = new BatchModelInstance(model);
			this.worldMatrix = Matrix.CreateTranslation(position);
		}

		public void Draw(DrawState state)
		{
			state.PushWorldMatrixMultiply(ref this.worldMatrix);

			if (instance.CullTest(state))
			{
				instance.Draw(state);
			}

			state.PopWorldMatrix();
		}

		public bool CullTest(ICuller culler)
		{
			return true;
		}
	}


	[DisplayName(Name = "Tutorial 24: Batch Model")]
	public class Tutorial : Application
	{
		//screen draw target
		private DrawTargetScreen drawToScreen;
		private BatchModel batchModel;

		private Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay statistics;

		protected override void Initialise()
		{
			Resource.EnableResourceTracking();

			Camera3D camera = new Xen.Ex.Camera.FirstPersonControlledCamera3D(this.UpdateManager, Vector3.Zero,true);

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);
			drawToScreen.ClearBuffer.ClearColour = Color.CornflowerBlue;

			//NEW CODE
			//create a BatchModel, this class stores the ModelData and will draw BatchModelInstances
			this.batchModel = new BatchModel();

			//NEW CODE
			//create a large number of actors (1600)
			for (float x = -20; x < 20; x++)
			for (float y = -20; y < 20; y++)
			{
				drawToScreen.Add(new Actor(this.batchModel, new Vector3(x * 5, y * 5, -5)));
			}

			//this is the most important bit...
			//always add the BatchModel itself to the draw target,
			//this should be added *after* all BatchModelInstances have been added

			//Note: each time a BatchModelInstance is drawn, it will store it's world matrix in the BatchModel
			//If the BatchModel is not drawn, the buffer storing these matrices will not be emptied, and will 
			//eventually throw an OutOfMemoryException exception.
			
			this.drawToScreen.Add(batchModel);

			statistics = new Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay(this.UpdateManager);
			this.drawToScreen.Add(statistics);
		}

		protected override void LoadContent(DrawState state, ContentManager manager)
		{
			//load the model content
			batchModel.ModelData = manager.Load<Xen.Ex.Graphics.Content.ModelData>(@"tiny_4anim");
			statistics.Font = manager.Load<SpriteFont>("Arial");
		}

		protected override void Draw(DrawState state)
		{
			drawToScreen.Draw(state);
		}

		protected override void SetupGraphicsDeviceManager(GraphicsDeviceManager graphics, ref RenderTargetUsage presentation)
		{
			if (graphics != null)
			{
				graphics.PreferredBackBufferWidth = 1280;
				graphics.PreferredBackBufferHeight = 720;
				graphics.SynchronizeWithVerticalRetrace = false;
			}
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}
	}
}
