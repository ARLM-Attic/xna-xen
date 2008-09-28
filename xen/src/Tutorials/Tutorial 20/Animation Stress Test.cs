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
 * This sample builds on Tutorial 14
 * 
 * This sample is not really a Tutorial, it's a performance stress test based on the other tutorials
 * 
 */
namespace Tutorials.Tutorial_20
{

	//this class draws the Tiny model,
	//it also walks it randomly across the ground plane from Tutorial 14
	//When moving, a walk/run animation is played, when still, the 'Loiter'
	//animation is played.
	//
	//The movement logic is not very scientific! :-)
	class Actor : IDraw, IContentOwner, IUpdate
	{
		//static random number generator
		private static Random random = new Random();

		private readonly float groundRadius;
		private readonly ModelInstance model;

		//animation controller, with two animations (move and idle)
		private AnimationController control;
		private AnimationInstance move, idle;

		//current position of the model, 'desired' target position, and velocity
		private Vector3 position, target, velocity;
		//the timer value that works out when to move to a new target
		//the current look angle (rotation of the model)
		//and the target angle, which is the angle of the next target
		private float moveTargetTimer, lookAngle, targetAngle;
		//move speed of the model
		private readonly float moveSpeed;

		//create the actor
		public Actor(ContentRegister content, UpdateManager updateManager, MaterialLightCollection lights, float groundRadius)
		{
			this.groundRadius = groundRadius;

			model = new ModelInstance();
			model.LightCollection = lights;

			//random starting position
			position = GetRandomPosition();

			//initially target the current position
			//the 'move to next target' timer will wind down and then the actor will move
			target = position;

			//randomize a bit
			lookAngle = (float)random.NextDouble() * MathHelper.TwoPi;
			moveSpeed = (float)random.NextDouble() * 0.9f + 0.1f;

			content.Add(this);
			updateManager.Add(this);


			InitaliseAnimations(updateManager);
		}

		private void InitaliseAnimations(UpdateManager updateManager)
		{
			//create the controller as an asynchronously controller.
			//this will process animations as a thread task
			//This occurs between the update look and the draw loop,
			//which is why the UpdateManager must be provided.
			control = model.GetAsyncAnimationController(updateManager);

			//create the idle animation
			idle = control.PlayLoopingAnimation(3);

			//give it a random speed
			idle.PlaybackSpeed = (float)random.NextDouble() * 0.5f + 0.6f;


			if (moveSpeed > 0.75)
			{
				//run animation
				move = control.PlayLoopingAnimation(1);
				move.PlaybackSpeed = 0.5f;
			}
			else
			{
				//walk animation
				move = control.PlayLoopingAnimation(2);
			}
			//initially don't want the move animation being visible
			move.Weighting = 0;
		}



		public void Draw(DrawState state)
		{
			//create the world matrix based on the current rotation and position...
			Matrix world;
			Matrix.CreateRotationZ(lookAngle - MathHelper.PiOver2,out world);
			world.Translation = position;

			//draw the model
			state.PushWorldMatrixMultiply(ref world);

			if (model.CullTest(state))
				model.Draw(state);

			state.PopWorldMatrix();
		}

		public bool CullTest(ICuller culler)
		{
			return true;
		}

		public void LoadContent(ContentRegister content, DrawState state, ContentManager manager)
		{
			//load the model
			model.ModelData = manager.Load<ModelData>(@"tiny_4anim");
		}

		public void UnloadContent(ContentRegister content, DrawState state)
		{

		}

		#region movement logic

		//this is the complex, and very hacky bit. :-)
		//this shouldn't be studied, it's just to look nice
		public UpdateFrequency Update(UpdateState state)
		{
			//if the move timer is above 0, then the actor is stationary
			//slowly reduce the timer
			if (moveTargetTimer > 0)
				moveTargetTimer -= state.DeltaTimeSeconds;

			//vector between the position and target
			Vector3 vectorToTarget = (target - position);
			float distance = vectorToTarget.Length();

			//if the model is really close to the target, and the timer
			//isn't counting down..
			if (distance < 0.5f && moveTargetTimer == 0)
			{
				//then wait 5 seconds to find a new target
				moveTargetTimer = 5;
			}

			//timer has gone below zero, then it's time to find a new target
			if (moveTargetTimer < 0)
			{
				moveTargetTimer = 0;

				//new random target
				target = GetRandomPosition();

				//and a new angle to that target
				Vector3 difference = (target - position);
				if (difference != Vector3.Zero)
					targetAngle = (float)Math.Atan2(difference.Y, difference.X);
			}

			//over time, reduce movement speed with a magic number
			//move speed is stored in the animation weighting (a bit of a hack..)
			move.Weighting *= 0.975f;

			//and increment the position with another magic number
			position += velocity * 0.1f;

			//the target is a good distance away...
			if (distance > 0.25f)
			{
				//and the actor isn't looking at the target
				if (targetAngle != this.lookAngle)
				{
					//slowly rotate to look at the new target
					//(this often rotates the wrong way.. :-)
					if (lookAngle > targetAngle)
						lookAngle -= 0.05f;
					if (lookAngle < targetAngle)
						lookAngle += 0.05f;

					//if really close to the right angle, then snap to it.
					if (Math.Abs(lookAngle - targetAngle) <= 0.05f)
						lookAngle = targetAngle;
				}
				else
				{
					//increase the move speed.. with a lot more magic numbers!
					move.Weighting = moveSpeed * 0.1f + (move.Weighting * (1 - moveSpeed * 0.1f));
					velocity += vectorToTarget * 0.1f / distance * move.Weighting;
				}
			}
			//dampen the velocity too... another magic number
			velocity *= 0.9f;

			//idle weighting and move weighting should add up to one
			idle.Weighting = 1 - move.Weighting;

			//don't both processing the animation if it's weighting is zero
			idle.Enabled = idle.Weighting != 0;
			move.Enabled = move.Weighting != 0;

			//finally, return the update frequency.
			//In this case, it's 60hz Async, which indicates
			//that this update method is thread safe and can be run in parallel
			//to other instances.

			//Note that Async has an overhead, so it may actually slow down for
			//very simple update methods.

			//perf boost here is fairly minor for using async,
			//as this method isn't all that heavy on the cpu.
			return UpdateFrequency.FullUpdate60hzAsync;
		}


		private Vector3 GetRandomPosition()
		{
			float rndAngle = (float)random.NextDouble() * MathHelper.TwoPi;
			//non linear random, keeps away from the centre
			float rndDist = (float)(1 - random.NextDouble() * random.NextDouble()) * groundRadius;

			Vector3 pos = new Vector3((float)Math.Sin(rndAngle) * rndDist, (float)Math.Cos(rndAngle) * rndDist, 0);
			return pos;
		}

		#endregion
	}


	[DisplayName(Name = "Tutorial 20: Animation stress test!")]
	public class Tutorial : Application
	{
		private const float diskRadius = 50;

		private DrawTargetScreen drawToScreen;
		private Camera3D camera;
		private MaterialLightCollection lights;

		private Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay stats;

		protected override void Initialise()
		{
			Resource.EnableResourceTracking();

			camera = new Camera3D();
			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);

			//no need to clear the colour buffer, as a special background will be drawn
			drawToScreen.ClearBuffer.ClearColourEnabled = false;
			//this is the special background element
			drawToScreen.Add(new BackgroundGradient(Color.Black,new Color(40,40,80)));


			//create the lights, similar to Tutorial 14
			lights = new MaterialLightCollection();
			lights.AmbientLightColour = new Vector3(0.15f, 0.15f, 0.3f);

			Vector3[] lightPositions = new Vector3[] 
			{ 
				new Vector3(0, 30, 1), 
				new Vector3(0, -30, 1) 
			};

			IDraw lightGeometry = null;

			for (int i = 0; i < lightPositions.Length; i++)
			{
				IMaterialPointLight light = lights.AddPointLight(i < 2, lightPositions[i], 15, Color.LightYellow, Color.WhiteSmoke);
				light.ConstantAttenuation = 0.1f;

				if (lightGeometry == null)
					lightGeometry = new Xen.Ex.Geometry.Sphere(Vector3.One, 8, true, false, false);

				//visually show the light
				drawToScreen.Add(new Tutorial_14.LightSourceDrawer(lightPositions[i], lightGeometry, Color.LightYellow));
			}

			//add the ground also from tutorial 14
			Tutorial_14.GroundDisk ground = new Tutorial_14.GroundDisk(this.Content, lights, diskRadius);
			drawToScreen.Add(ground);

			//create 500 actors!
			for (int i = 0; i < 500; i++)
			{
				Actor actor = new Actor(this.Content, this.UpdateManager, lights, diskRadius);
				drawToScreen.Add(actor);
			}

			//create the draw statistics display
			stats = new Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay(this.UpdateManager);
			drawToScreen.Add(stats);
		}

		protected override void Draw(DrawState state)
		{
			//rotate the camera around the scene (similar to tutorial 14)
			RotateCamera(state);

			//draw the scene
			drawToScreen.Draw(state);
		}

		private void RotateCamera(DrawState state)
		{

			Vector3 lookAt = new Vector3(0, 0, 0);
			float angle = state.TotalTimeSeconds * 0.05f;
			Vector3 lookFrom = new Vector3((float)Math.Sin(angle) * 50, (float)Math.Cos(angle) * 50, 15);
			lookFrom.Z += (float)Math.Sin(angle) * 5;

			camera.LookAt(lookAt, lookFrom, new Vector3(0, 0, 1));
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[0].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}

		//setup the desired resolution
		protected override void SetupGraphicsDeviceManager(GraphicsDeviceManager graphics, ref RenderTargetUsage presentation)
		{
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 768;
			graphics.SynchronizeWithVerticalRetrace = false;
#if !DEBUG
			graphics.IsFullScreen = true;
#endif
		}

		//set the font for the statistics overlay
		protected override void LoadContent(DrawState state, ContentManager manager)
		{
			stats.Font = manager.Load<SpriteFont>(@"Arial");
		}

	}


	//a simple element that displays a gradient background
	class BackgroundGradient : SolidColourElement
	{
		private Color topColour;

		public BackgroundGradient(Color top, Color bottom) : base(bottom,new Vector2(1,1),true)
		{
			this.topColour = top;
		}

		protected override void WriteColours(ref Color topLeft, ref Color topRight, ref Color bottomLeft, ref Color bottomRight)
		{
			topLeft = topColour;
			topRight = topColour;
			bottomLeft = this.Colour;
			bottomRight = this.Colour;
		}
	}
}
