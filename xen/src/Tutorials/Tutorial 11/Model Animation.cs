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
 * This sample extends from Tutorial_10 (ModelInstance)
 * This sample demonstrates:
 * 
 * Playing an animation with an AnimationController
 * 
 */
namespace Tutorials.Tutorial_11
{
	class Actor : IDraw, IContentOwner
	{
		private ModelInstance model;

		//NEW CODE
		//An animation controller animates a model instance
		private AnimationController animationController;
		//This structure is a handle to an animation
		private AnimationInstance animation;

		//NEW CODE
		public Actor(ContentRegister content)
		{
			//A ModelInstance can be created without any content...
			//However it cannot be used until the content is set

			model = new ModelInstance();

			//create the animation controller.
			animationController = model.GetAnimationController();

			//NOTE: Animations cannot be played until the model data has been loaded...

			content.Add(this);

			//At this point in this tutorial, the model is now loaded.

			//get the index of the walk animation
			//this model has 4 animations, Wave, Jog, Walk and Loiter
			//The animations are stored in model.ModelData.Animations
			int animationIndex = animationController.AnimationIndex("Walk");

			//begin playing the animation, looping
			animation = animationController.PlayLoopingAnimation(animationIndex);
			
			//as many animations as you want can be played at any one time
			//to blend between animations, adjust their weighting with:
			//animation.Weighting = ...;
			//Combined weightings usually should add up to 1.
			//A weighting of 0 means the animation has no effect, 1 has normal effect.
			//Values outside the 0-1 range usually produces undesirable results.

			//Note:
			//Animations in xen are lossy compressed.
			//For the model used here, the animation data is reduced from nearly 2mb
			//down to around 200kb. (The model geometry is less than 300kb)
			//The amount of compression change can be configured in the content's properties
			//The 'Animation Compression Tolerance' value is a percentage
			//The default is .5%. This means the animation will always be within .5%
			//of the source. Setting this value to 0 will save a lossless animation.
		}

		//everything else from here on is identical...

		public void Draw(DrawState state)
		{
			//ModelInstances automatically setup the default material shaders
			//Custom shaders can be used with model.SetShaderOverride(...)
			model.Draw(state);
		}

		public bool CullTest(ICuller culler)
		{
			return model.CullTest(culler);
		}


		public void LoadContent(ContentRegister content, DrawState state, ContentManager manager)
		{
			//load the model data into the model instance
			model.ModelData = manager.Load<Xen.Ex.Graphics.Content.ModelData>(@"tiny_4anim");
		}

		public void UnloadContent(ContentRegister content, DrawState state)
		{
		}
	}

	[DisplayName(Name = "Tutorial 11: Model Animation")]
	public class Tutorial : Application
	{
		//screen draw target
		private DrawTargetScreen drawToScreen;

		protected override void Initialise()
		{
			Camera3D camera = new Camera3D();
			camera.LookAt(new Vector3(0, 0, 4), new Vector3(3, 4, 4), new Vector3(0, 0, 1));

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);
			drawToScreen.ClearBuffer.ClearColour = Color.CornflowerBlue;

			//NEW CODE
			//create the actor instance
			drawToScreen.Add(new Actor(this.Content));
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
