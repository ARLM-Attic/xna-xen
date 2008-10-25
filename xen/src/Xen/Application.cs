using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using Xen.Camera;
using Xen.Threading;

namespace Xen
{
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	sealed class XNAGame : Game
	{
		GraphicsDeviceManager graphics;
		RenderTargetUsage presentation;
		Application parent;
		internal AppState state;

		internal XNAGame(Application parent)
		{
			this.state = new AppState(parent);

			this.parent = parent;
			parent.UpdateManager.Add(state);
			parent.UpdateManager.Add(parent);

			graphics = new GraphicsDeviceManager(this);
			graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(PreparingDeviceSettings);
			graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
			graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
			graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
			graphics.SynchronizeWithVerticalRetrace = true;

#if DEBUG
#if !XBOX360
			Window.Title += " [DEBUG API]";
#endif
#endif
			presentation = RenderTargetUsage.PlatformContents;

			parent.SetWindowSize(graphics);
			parent.SetupGraphicsDeviceManager(graphics, ref presentation);
			parent.SetWindowSize(graphics);
		}

		void PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
#if DEBUG
#if !XBOX360
			foreach (GraphicsAdapter adapter in GraphicsAdapter.Adapters)
			{
				if (adapter.Description.Contains("PerfHUD"))
				{
					e.GraphicsDeviceInformation.DeviceType = DeviceType.Reference;
					e.GraphicsDeviceInformation.Adapter = adapter;
				}
			}
#endif
#endif
			e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = presentation;
		}

		public bool VSyncEnabled
		{
			get { return graphics.SynchronizeWithVerticalRetrace; }
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			base.OnExiting(sender, args);

			Xen.Graphics.Resource.ClearResourceTracking();
		}

		protected override void Initialize()
		{
			parent.SetGraphicsDevice(GraphicsDevice);
			
			parent.Initialise();
			parent.InitialisePlayerInput(this.state.GetUpdateState().PlayerInput);

			parent.SetReadyToLoadContent();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			parent.SetGraphicsDevice(GraphicsDevice);

			parent.SupportsHardwareInstancing = true;
#if !XBOX360
			parent.SupportsHardwareInstancing = graphics.GraphicsDevice.GraphicsDeviceCapabilities.VertexShaderVersion.Major >= 3;
#endif

			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
		}

		protected override void Draw(GameTime gameTime)
		{
			//state.SetGameTime(gameTime);
			DrawState state = this.state.GetRenderState(this.graphics.GraphicsDevice);
			state.IncrementFrameIndex();

#if XBOX360
			state.nonScreenRenderComplete = false;
#endif

			parent.OnDraw(state);
			
			base.Draw(gameTime);
		}


#if !XBOX360
		System.Windows.Forms.Form windowForm;

		static MouseState mouse;
		static bool centreMouse, mouseCentred, centreMousePrevious, windowFocused = true, windowFocusedBuffer = true;
		static Point mouseCentrePoint;
#endif
		static object inputLock = new object();
		static KeyboardState keyboard;

		protected override void Update(GameTime gameTime)
		{
			this.IsFixedTimeStep = false;
			//parent.updateSyncStart.WaitOne();

			lock (inputLock)
			{
				keyboard = Keyboard.GetState();

#if !XBOX360
				mouse = Mouse.GetState();

				if (windowForm == null)
				{
					windowForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
					windowForm.Deactivate += delegate { windowFocusedBuffer = false; };
					windowForm.Activated += delegate { windowFocusedBuffer = true; };
				}

				Point p = new Point(windowForm.Width / 2, windowForm.Height / 2);
				
				windowFocused = windowFocusedBuffer;

				if (centreMouse && windowFocused)
					Mouse.SetPosition(p.X, p.Y);

				if (centreMousePrevious)
				{
					mouseCentred = true;
					mouseCentrePoint = p;
				}

				centreMousePrevious = centreMouse;
#endif
			}

			//parent.updateSyncEnd.Set();
			//	base.Update(gameTime);

			state.SetGameTime(gameTime);

			parent.CallUpdate(state.GetUpdateState());
		}

#if !XBOX360
		internal static void GetInputState(ref KeyboardState k, ref MouseState m, ref bool windowFocused, ref bool mouseCentred, ref Point centerPoint)
		{
			lock (inputLock)
			{

				m = mouse;
				k = keyboard;
				mouseCentred = XNAGame.mouseCentred;
				centerPoint = XNAGame.mouseCentrePoint;
				windowFocused = XNAGame.windowFocused;
			}
		}
#else
		internal static void GetInputState(ref KeyboardState k)
		{
			lock (inputLock)
				k = keyboard;
		}
#endif
		internal static void SetMouseCentreState(bool centreMouse)
		{
			lock (inputLock)
			{

#if !XBOX360
				XNAGame.centreMouse = centreMouse;
#endif

			}
		}

	}

	/// <summary>
	/// Generic callback delegate that takes a single arguement and returns a value, where T is the return type, and A is the arguement
	/// </summary>
	/// <typeparam name="T">Return type</typeparam>
	/// <typeparam name="A">First arguement type</typeparam>
	/// <param name="a">First arguement</param>
	/// <returns></returns>
	public delegate T Callback<T, A>(A a);
	/// <summary>
	/// Generic callback delegate that takes a two arguements and returns a value, where T is the return type, and A,B are the arguements
	/// </summary>
	/// <typeparam name="T">Return type</typeparam>
	/// <typeparam name="A">First arguement type</typeparam>
	/// <typeparam name="B">Second arguement type</typeparam>
	/// <param name="a">First arguement</param>
	/// <param name="b">Second arguement</param>
	/// <returns></returns>
	public delegate T Callback<T, A, B>(A a, B b);

	/// <summary>
	/// <para>Enumeration representing the frequency an <see cref="IUpdate"/> object wishes to have it's Update() method called (With a special <see cref="Terminate"/> case to have the object removed from the update list).</para>
	/// <para>NOTE: User input is updated at 60hz, any class responding to user input should use <see cref="UpdateFrequency.FullUpdate60hz"/></para>
	/// </summary>
	public enum UpdateFrequency : byte
	{
		/// <summary>
		/// Call update once per frame
		/// </summary>
		[UpdateFrequency(Interval = 1)]
		OncePerFrame = 0,
		/// <summary>
		/// <para>Call update once per frame asynchronously with other <see cref="OncePerFrameAsync"/> instances</para>
		/// <para>Note: Update implementations need to be thread safe when using asynchronous updates. Async adds overhead which can make simple Update methods slower!</para>
		/// </summary>
		/// <remarks>
		/// <para>Asynchronous updates have an overhead for setup of the asynchronous update process.</para>
		/// <para>If there are fewer items to be updated than threads, then the update processes will not be run asynchronously.</para>
		/// </remarks>
		[UpdateFrequency(Interval = 1, Async = true)]
		OncePerFrameAsync = 1,
		/// <summary>
		/// <para>Call update 60 times per second</para>
		/// <para>NOTE: User input is updated at 60hz, any class responding to user input should use <see cref="FullUpdate60hz"/></para>
		/// </summary>
		[UpdateFrequency(Interval = 1)]
		FullUpdate60hz = 2,
		/// <summary>
		/// <para>Call update 60 times per second asynchronously with other <see cref="FullUpdate60hzAsync"/> instances</para>
		/// <para>NOTE: User input is updated at 60hz, any class responding to user input should use <see cref="FullUpdate60hz"/></para>
		/// <para>Note: Update implementations need to be thread safe when using asynchronous updates. Async adds overhead which can make simple Update methods slower!</para>
		/// </summary>
		/// <remarks>
		/// <para>Asynchronous updates have an overhead for setup of the asynchronous update process.</para>
		/// <para>If there are fewer items to be updated than threads, then the update processes will not be run asynchronously.</para>
		/// </remarks>
		[UpdateFrequency(Interval = 1, Async = true)]
		FullUpdate60hzAsync = 3,
		/// <summary>
		/// Call update at least 30 times per second
		/// </summary>
		[UpdateFrequency(Interval = 2)]
		HalfUpdate30hz = 4,
		/// <summary>
		/// <para>Call update at least 30 times per second asynchronously with other <see cref="HalfUpdate30hzAsync"/> instances</para>
		/// <para>Note: Update implementations need to be thread safe when using asynchronous updates. Async adds overhead which can make simple Update methods slower!</para>
		/// </summary>
		/// <remarks>
		/// <para>Asynchronous updates have an overhead for setup of the asynchronous update process.</para>
		/// <para>If there are fewer items to be updated than threads, then the update processes will not be run asynchronously.</para>
		/// </remarks>
		[UpdateFrequency(Interval = 2, Async = true)]
		HalfUpdate30hzAsync = 5,
		/// <summary>
		/// Call update at least 15 times per second
		/// </summary>
		[UpdateFrequency(Interval = 4)]
		PartialUpdate15hz = 6,
		/// <summary>
		/// Call update at least 10 times per second
		/// </summary>
		[UpdateFrequency(Interval = 6)]
		PartialUpdate10hz = 7,
		/// <summary>
		/// Call update at least 5 times per second
		/// </summary>
		[UpdateFrequency(Interval = 12)]
		PartialUpdate5hz = 8,
		/// <summary>
		/// Call update at least once every second
		/// </summary>
		[UpdateFrequency(Interval = 60)]
		PartialUpdate1hz = 9,
		/// <summary>
		/// This object no longer needs to be updated, and should be removed from the update list.
		/// </summary>
		/// <remarks>This is the preferred method for removing an object from an <see cref="UpdateManager"/></remarks>
		Terminate = 255
	}
	///// <summary>
	///// Enumeration representing the update status of an <see cref="IUpdate"/> or <see cref="IAsyncUpdate"/> object. Returned from the Update() method. Specify <see cref="Terminate"/> to have the object efficiently removed from the update list.
	///// </summary>
	//public enum UpdateResult : byte
	//{
	//    /// <summary>
	//    /// Future updating should continue as normal
	//    /// </summary>
	//    Continue,
	//    /// <summary>
	//    /// This object no longer needs to be updated, and should be removed from the update list.
	//    /// </summary>
	//    /// <remarks>This is the preferred method for removing an object from an <see cref="UpdateManager"/></remarks>
	//    Terminate
	//}

	/// <summary>
	/// Interface for an object that wishes to be updated at a consistent interval (specified by <see cref="UpdateFrequency"/>)
	/// </summary>
	public interface IUpdate
	{
		///// <summary>
		///// Get the frequency the object wishes to have <see cref="Update"/> called.
		///// </summary>
		///// <remarks>This value may be changed dynamically. It is checked after each call to <see cref="Update"/>, changes should be applied by the following update loop.</remarks>
		//UpdateFrequency UpdateFrequency { get; }
		/// <summary>
		/// Method for updating the object and performing logic. Called at a desired frequency
		/// </summary>
		/// <param name="state">Current state of the application</param>
		/// <returns>Returns a <see cref="UpdateFrequency"/>, indicating the frequency the object desires to be updated in the following update loop (Or <see cref="UpdateFrequency.Terminate"/> if the object wishes to be removed from the update list)</returns>
		UpdateFrequency Update(UpdateState state);
	}

	/// <summary>
	/// Interface for an object that can be culled by an <see cref="ICuller"/>. An example use may be off-screen culling
	/// </summary>
	public interface ICullable
	{
		/// <summary>
		/// Perform a cull test on the current object. Returns false if the CullTest fails (for example, the object is offscreen)
		/// </summary>
		/// <param name="culler"></param>
		/// <returns>Returns false if the CullTest fails (for example, the object is offscreen)</returns>
		bool CullTest(ICuller culler);
	}
	/// <summary>
	/// Interface to an object that can be drawn. All drawable objects also are <see cref="ICullable"/>
	/// </summary>
	/// <remarks>
	/// <para>Some objects, such as <see cref="Graphics.DrawTarget">draw targets</see> need to have <see cref="Graphics.DrawTarget.Draw">Draw</see> called explicitly, while some objects may have <see cref="Draw"/> called from another object.</para>
	/// <para>A call to <see cref="Draw"/> should not assume the object knows it's full render state. Simple geometry classes, for example, may only draw themselves. In such a case, the 'owner' IDraw object should make sure the correct render states, shaders, etc are setup first.</para>
	/// </remarks>
	public interface IDraw : ICullable
	{
		/// <summary>
		/// Draw the object
		/// </summary>
		/// <param name="state">Current state of the application</param>
		void Draw(DrawState state);
	}
	/// <summary>
	/// Interface to a sepecial object that can drawn itself as a batch. All batch drawable objects also are <see cref="IDraw"/> instances
	/// </summary>
	/// <remarks>
	/// <para>This interface is usually used by low level geometry classes</para>
	/// <para>See the <see cref="Graphics.StreamFrequency"/> class for details on using hardware instancing to draw large numbers of complex objects efficiently.</para>
	/// </remarks>
	public interface IDrawBatch : IDraw
	{
		/// <summary>
		/// <para>Draw multiple instances of the object</para>
		/// <para>NOTE: Calling this method assumes that hardware instancing is supported <i>and</i> that a hardware instancing compatible shader is bound</para>
		/// </summary>
		/// <param name="state">Current state of the application</param>
		/// <param name="CanDrawItemIndex">An optional callback to indicate if an instance index should be drawn (may be null)</param>
		/// <param name="instances">World matrix of the instances to be drawn</param>
		/// <param name="instanceCount">The number of instances to draw</param>
		void DrawBatch(DrawState state, Callback<bool, int, ICuller> CanDrawItemIndex, Matrix[] instances, int instanceCount);
	}
	/// <summary>
	/// Interface to an object that modifies draw state and must be setup/shutdown. Usually used by internal classes only
	/// </summary>
	public interface IBeginEndDraw
	{
		/// <summary>
		/// Enable/Disable the begin/end process
		/// </summary>
		bool Enabled { get; set;}
		/// <summary>
		/// Drawing is about to begin
		/// </summary>
		/// <param name="state">Current state of the application</param>
		void Begin(DrawState state);
		/// <summary>
		/// Drawing has completed
		/// </summary>
		/// <param name="state">Current state of the application</param>
		void End(DrawState state);
	}

	/// <summary>
	/// Base class for implementing a xen application. Implement this class in a simliar manner to the XNA <see cref="Game"/> class
	/// </summary>
	/// <remarks>This class can be explicitaly cast to an XNA Game instance:
	/// <example>
	/// <code>
	/// Xen.Application app = ...;
	/// 
	/// //...
	/// Microsoft.Xna.Framework.Game xnaGame = (Microsoft.Xna.Framework.Game)app;
	/// </code>
	/// </example></remarks>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public abstract class Application : IDisposable, IContentOwner, IUpdate
	{
		private DepthStencilBuffer defaultDepthStencil;
		private LinkedList<double> approxFps = new LinkedList<double>();
		internal float approximateFrameRate;
		private GraphicsDevice graphics;
		private SurfaceFormat screenFormat;
		private MultiSampleType screenMultisample;
		private int graphicsIndex;
		internal int graphicsId;
		private UpdateManager updateManager;
		internal readonly Xen.Graphics.VertexDeclarationBuilder declarationBuilder
			= new Xen.Graphics.VertexDeclarationBuilder();
		private ThreadPool threadPool = new ThreadPool();
		private bool supportsInstancing;
		private bool supportsInstancingSet;
		private bool initalised;
		private XNAGame xnaGame;
		private ContentRegister content;
		private int windowWidth, windowHeight;
		private bool readToLoadContent;
		private int minimumFrameRate = 5;

		/// <summary>
		/// XNA Service that returns the <see cref="Application"/>. Useful for warming resources when loading through the content pipeline.
		/// </summary>
		public class ApplicationProviderService
		{
			Application app;
			internal ApplicationProviderService(Application application)
			{
				this.app = application;
			}
			/// <summary>
			/// Gets the <see cref="Application"/>
			/// </summary>
			public Application Application
			{
				get { return app; }
			}
		}

		/// <summary>
		/// <para>Gets/Sets the minimum desired frame rate. If the frame rate dips below this value, the application update loop will be slowed down to compensate</para>
		/// <para>Note: The reported frame rate from the application will not drop below this value</para>
		/// </summary>
		public int MinimumDesiredFrameRate
		{
			get { return minimumFrameRate; }
			set { if (value < 0 || value > 60) throw new ArgumentException(); minimumFrameRate = value; }
		}


#if DEBUG
		internal DrawStatistics currentFrame, previousFrame;
#endif


		internal bool VSyncEnabled
		{
			get { return xnaGame.VSyncEnabled; }
		}

		internal GraphicsDevice GraphicsDevice
		{
			get 
			{ 
				if (graphics == null) 
					throw new InvalidOperationException("GraphicsDevice has not been initalised yet");
				return graphics; 
			}
		}

		/// <summary>
		/// Gets the application thread pool instance
		/// </summary>
		public ThreadPool ThreadPool
		{
			get { return this.threadPool; }
		}

		internal bool IsInitailised
		{
			get { return initalised; }
		}

		internal void SetReadyToLoadContent()
		{
			readToLoadContent = true;
			this.LoadContent(this.GetProtectedDrawState(graphics), this.xnaGame.Content);
		}


		internal bool SupportsHardwareInstancing
		{
			get { if (!supportsInstancingSet) throw new ArgumentException("Graphics device not created yet"); return supportsInstancing; }
			set { supportsInstancing = value; supportsInstancingSet = true; }
		}


		/// <summary>
		/// A dictionary for placing global values, if desired
		/// </summary>
		public System.Collections.IDictionary UserValues
		{
			get { return xnaGame.state.UserValues; }
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>This method sets up the input manager</remarks>
		public Application()
		{
			this.updateManager = new UpdateManager();
		}

		/// <summary>
		/// Get an instance to an application wide Content manager
		/// </summary>
		public ContentRegister Content
		{
			get 
			{
				return content; 
			}
		}


		/// <summary>
		/// Access the XNA base game <see cref="GameComponentCollection"/>. Use with caution, Using Game Components is not tested/supported
		/// </summary>
		public GameComponentCollection XnaComponents
		{
			get { return xnaGame.Components; }
		}

		/// <summary>
		/// Access the XNA base game <see cref="GameServiceContainer"/> Services
		/// </summary>
		public GameServiceContainer Services
		{
			get { return xnaGame.Services; }
		}

		/// <summary>
		///  Indicates whether this application is currently the active application.
		/// </summary>
		public bool IsActive
		{
			get { return xnaGame.IsActive; }
		}

		/// <summary>
		/// Get the XNA Application instance
		/// </summary>
		/// <param name="application"></param>
		/// <returns></returns>
		public static implicit operator Game(Application application)
		{
			return application.xnaGame;
		}

		/// <summary>
		/// Start the main application loop. This method should be called from the entry point of the application
		/// </summary>
		public void Run()
		{
			xnaGame = new XNAGame(this);
			xnaGame.Services.AddService(typeof(ApplicationProviderService), new ApplicationProviderService(this));
			xnaGame.Exiting += new EventHandler(ShutdownThreadPool);
			xnaGame.Content.RootDirectory += @"\Content";

			content = new ContentRegister(this, xnaGame.Content);
			content.Add(this);

			xnaGame.Run();
		}

		void ShutdownThreadPool(object sender, EventArgs e)
		{
			threadPool.Dispose();
		}

		/// <summary>
		/// Immediately shuts down the application
		/// </summary>
		public virtual void Shutdown()
		{
			xnaGame.Exit();

			xnaGame.Dispose();
		}

		internal void OnDraw(DrawState state)
		{
#if DEBUG
			previousFrame = currentFrame;
			currentFrame.Reset();
			currentFrame.Begin();
#endif

			ushort stackHeight, stateHeight, cameraHeight, preCull,postCull;
			state.GetStackHeight(out stackHeight, out stateHeight, out cameraHeight,out preCull,out postCull);

			if (approxFps.Count == 20)
			{
				LinkedListNode<double> first = approxFps.First;
				approxFps.Remove(first);
				first.Value = state.DeltaTimeFrequency;
				approxFps.AddLast(first);
			}
			else
				approxFps.AddLast(state.DeltaTimeFrequency);
			
			double total = 0;
			foreach (double d in approxFps)
				total += d;
			if (approxFps.Count > 0)
				total /= approxFps.Count;

			approximateFrameRate = (float)total;

			state.PrepareForNewFrame();

			Draw(state);

			state.RunDeferredDrawCalls();

			state.ValidateStackHeight(stackHeight, stateHeight, cameraHeight, preCull, postCull);

#if DEBUG
			currentFrame.End();
#endif
		}

		/// <summary>
		/// Implement all application specific initalisation code in here
		/// </summary>
		protected internal abstract void Initialise();

		internal void CallUpdate(UpdateState state)
		{
			((IUpdate)updateManager).Update(state);
		}

		/// <summary>
		/// Called at the start of the update loop, 60 times per frame
		/// </summary>
		/// <param name="state"></param>
		protected internal abstract void Update(UpdateState state);

		/// <summary>
		/// Run all drawing code in this method
		/// </summary>
		/// <param name="state"></param>
		protected internal abstract void Draw(DrawState state);

		/// <summary>
		/// Override this method to modify the setup of the player input objects before the application starts
		/// </summary>
		/// <param name="playerInput"></param>
		protected internal virtual void InitialisePlayerInput(Xen.Input.PlayerInputCollection playerInput) { }
		
		/// <summary>
		/// Override this method to change the graphics device options before the application starts (eg, set resolution, fullscreen, etc)
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="presentation">Allows modification of the <see cref="PresentationParameters"/> <see cref="RenderTargetUsage"/> for the primary display</param>
		protected internal virtual void SetupGraphicsDeviceManager(GraphicsDeviceManager graphics, ref RenderTargetUsage presentation)
		{
		}

		internal void SetWindowSize(GraphicsDeviceManager manager)
		{
			this.windowWidth = manager.PreferredBackBufferWidth;
			this.windowHeight = manager.PreferredBackBufferHeight;

			screenFormat = manager.PreferredBackBufferFormat;
			screenMultisample = MultiSampleType.None;
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			xnaGame.Dispose();
		}

		#endregion

		/// <summary>
		/// Gets the Window the application is being displayed on
		/// </summary>
		public GameWindow Window
		{
			get { return xnaGame.Window; }
		}

		/// <summary>
		/// Gets the width (in pixels) of the render window
		/// </summary>
		public int WindowWidth
		{
			get
			{
				return windowWidth;
			}
		}
		/// <summary>
		/// Gets the height (in pixels) of the render window
		/// </summary>
		public int WindowHeight
		{
			get
			{
				return windowHeight;
			}
		}

		/// <summary>
		/// Gets the application <see cref="UpdateManager"/> instance. Add objects implementing <see cref="IUpdate"/> to this manager to perform update logic
		/// </summary>
		public UpdateManager UpdateManager
		{
			get { return updateManager; }
		}

		internal DepthStencilBuffer Depth
		{
			get
			{
				return this.defaultDepthStencil;
			}
		}

		internal void SetGraphicsDevice(GraphicsDevice graphics)
		{
			//calculate a system wide unique id for the graphics device
#if XBOX360
			this.graphicsId = 999;
#else
			this.graphicsId = System.Diagnostics.Process.GetCurrentProcess().Id;
#endif
			screenFormat = graphics.PresentationParameters.BackBufferFormat;
			screenMultisample = graphics.PresentationParameters.MultiSampleType;

			if (graphics != this.graphics)
				graphicsIndex++;

			this.graphicsId ^= graphicsIndex << 16;
			this.graphicsId ^= (int)(DateTime.Now.Ticks & 0xFFFF);

			//directly accessing graphics.DepthStencilBuffer will memory leak.. so it's only used here
			this.defaultDepthStencil = graphics.DepthStencilBuffer;
			this.initalised = true;
			this.graphics = graphics;

			this.windowWidth = graphics.PresentationParameters.BackBufferWidth;
			this.windowHeight = graphics.PresentationParameters.BackBufferHeight;
		}

		#region IContentOwner Members

		internal DrawState GetProtectedDrawState(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics)
		{
			return xnaGame.state.GetProtectedDrawState(graphics);
		}

		/// <summary>
		/// Override this method to perform custom content loading through an XNA ContentManager
		/// </summary>
		/// <param name="state"></param>
		/// <param name="manager"></param>
		protected virtual void LoadContent(DrawState state, ContentManager manager)
		{
		}

		/// <summary>
		/// Override this method to perform custom content unloading
		/// </summary>
		/// <param name="state"></param>
		protected virtual void UnloadContent(DrawState state)
		{
		}

		void IContentOwner.LoadContent(ContentRegister content, DrawState state, ContentManager manager)
		{
			xnaGame.state.GetRenderState(state.BeginGetGraphicsDevice(Xen.Graphics.State.StateFlag.None)).DirtyInternalRenderState(Xen.Graphics.State.StateFlag.All);
			state.EndGetGraphicsDevice();
			if (readToLoadContent)
				this.LoadContent(state, manager);
		}

		void IContentOwner.UnloadContent(ContentRegister content, DrawState state)
		{
			xnaGame.state.GetRenderState(state.BeginGetGraphicsDevice(Xen.Graphics.State.StateFlag.None)).DirtyInternalRenderState(Xen.Graphics.State.StateFlag.All);
			state.EndGetGraphicsDevice();
			this.UnloadContent(state);
		}

		#endregion

		internal SurfaceFormat GetScreenFormat()
		{
			return screenFormat;
		}
		internal MultiSampleType GetScreenMultisample()
		{
			return screenMultisample;
		}

		#region IUpdate Members

		UpdateFrequency IUpdate.Update(UpdateState state)
		{
			this.Update(state);
			return UpdateFrequency.FullUpdate60hz;
		}

		#endregion
	}



}
