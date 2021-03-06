using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Xen
{
	internal delegate void GameEvent(GameTime time, GameComponentHost component);

	/// <summary>
	/// An XNA Game Component hosts a Xen Application.
	/// </summary>
	public sealed class GameComponentHost : Microsoft.Xna.Framework.DrawableGameComponent
	{
		private readonly Application application;
		private readonly Game game;
		private readonly GraphicsDeviceManager graphicsManager;
		private Microsoft.Xna.Framework.Graphics.StateBlock stateBlock;
		internal event GameEvent DrawEvent, UpdateEvent, InitaliseEvent;

		/// <summary>Construct the Xen GameComponent host</summary>
		public GameComponentHost(Game host, Application xenApplication, GraphicsDeviceManager graphicsManager)
			: base(host)
		{
			if (xenApplication == null || host == null || graphicsManager == null)
				throw new ArgumentNullException();

			this.game = host;
			this.graphicsManager = graphicsManager;
			this.application = xenApplication;

			xenApplication.Run(this);
		}

		/// <summary>
		/// Initialises the Xen Application
		/// </summary>
		public override void Initialize()
		{
			application.SetWindowSizeAndFormat(graphicsManager.PreferredBackBufferWidth, graphicsManager.PreferredBackBufferHeight, graphicsManager.PreferredBackBufferFormat, graphicsManager.PreferredDepthStencilFormat);
			application.SetGraphicsDevice(game.GraphicsDevice);

			if (this.InitaliseEvent != null)
				this.InitaliseEvent(new GameTime(), this);

			base.Initialize();
		}

		/// <summary>
		/// Updates the Xen Application (if the game does not use FixedTimeSteps)
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			if (!game.IsFixedTimeStep && UpdateEvent != null)
				UpdateEvent(gameTime, this);

			base.Update(gameTime);
		}

		/// <summary>Updates the Xen Application (if the game uses fixed time steps) and then calls the application to draw</summary>
		public override void Draw(GameTime gameTime)
		{
			if (this.stateBlock == null)
				this.stateBlock = new StateBlock(game.GraphicsDevice);

			if (game.IsFixedTimeStep && UpdateEvent != null)
				UpdateEvent(gameTime, this);

			this.stateBlock.Capture();

			if (DrawEvent != null)
				DrawEvent(gameTime, this);

			this.stateBlock.Apply();

			base.Draw(gameTime);
		}
	}







	//internal wrapper class for providing application services
	sealed class XNAGameComponentHostAppWrapper : IXNAAppWrapper
	{
		internal XNAGameComponentHostAppWrapper(XNALogic logic, Application parent, GameComponentHost host)
		{
			this.parent = parent;
			this.logic = logic;

			this.control = host;
			this.game = host.Game;

			content = new ContentManager(game.Services);

			UpdateWindowSize();

			host.DrawEvent += new GameEvent(host_Draw);
			host.UpdateEvent += new GameEvent(host_Update);
			host.InitaliseEvent += new GameEvent(host_InitaliseEvent);
		}

		private void UpdateWindowSize()
		{
			SurfaceFormat format = SurfaceFormat.Color;
			Rectangle bounds = game.Window.ClientBounds;

			this.parent.SetWindowSizeAndFormat(bounds.Width, bounds.Height, format, DepthFormat.Depth24Stencil8);
		}



		void host_InitaliseEvent(GameTime time, GameComponentHost component)
		{
			logic.Initialise();
			logic.LoadContent();
		}

		void host_Update(GameTime time, GameComponentHost component)
		{
			UpdateWindowSize();

			if (ticksSet == false)
			{
				initalTick = time.TotalRealTime.Ticks;
				initialGameTick = time.TotalGameTime.Ticks;
				ticksSet = true;
			}

			logic.Update(time.TotalRealTime.Ticks - initalTick, time.TotalGameTime.Ticks - initialGameTick);
		}

		void host_Draw(GameTime time, GameComponentHost component)
		{
			logic.Draw(true);
		}

		public void Dispose()
		{
			if (Exiting != null)
				Exiting(this, EventArgs.Empty);
		}

		#region members and getters

		private readonly XNALogic logic;
		private readonly ContentManager content;
		private readonly GameComponentHost control;
		private readonly Application parent;
		private readonly Game game;
		public event EventHandler Exiting;
		private bool ticksSet;
		private long initalTick, initialGameTick;

		public GraphicsDevice GraphicsDevice { get { return game.GraphicsDevice; } }
		public bool VSyncEnabled { get { return true; } }
		public GameServiceContainer Services { get { return game.Services; } }
		public ContentManager Content { get { return content; } }
		public GameComponentCollection Components { get { return game.Components; } }
		public bool IsActive { get { return game.IsActive; } }
		public GameWindow Window { get { return game.Window; } }
		public IntPtr WindowHandle { get { return game.Window.Handle; } }
		int? IXNAAppWrapper.GetMouseWheelValue() { return null; }

		public void Run() { }
		public void Exit() { }

		#endregion
	}

}