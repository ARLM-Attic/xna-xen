
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xen.Graphics;
using Xen.Input.Mapping;
using Xen.Input.State;

#endregion


using GamePlus = System.Object;
using Xen.Input;
using Xen.Camera;
using System.Collections;


namespace Xen
{
	/// <summary>
	/// Interface to state storage objects (<see cref="DrawState"/> and <see cref="UpdateState"/>)
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// True if the current state is potentially running on multiple threads
		/// </summary>
		bool IsAsynchronousState { get;}
		/// <summary>
		/// Get the Application instance
		/// </summary>
		Application Application { get; }
		/// <summary>
		/// Time delta (change) for the last frame/update as a frequency. Eg, 60 for 60fps
		/// </summary>
		float DeltaTimeFrequency { get; }
		/// <summary>
		/// Time delta (change) for the last frame/update as a number of seconds. Eg, 0.0166 for 60fps
		/// </summary>
		float DeltaTimeSeconds { get; }
		/// <summary>
		/// Accurate DeltaTime timespan
		/// </summary>
		long DeltaTimeTicks { get; }
		/// <summary>
		/// Accurate performance time (application time may be different to real time if the application has performance problems)
		/// </summary>
		long TotalTimeTicks { get; }
		/// <summary>
		/// Accurate system time since the application started
		/// </summary>
		long TotalRealTimeTicks { get; }
		/// <summary>
		/// Total time in seconds
		/// </summary>
		float TotalTimeSeconds { get; }
		/// <summary>
		/// A dictionary for placing global values, if desired
		/// </summary>
		IDictionary UserValues { get; }
	}
	/// <summary>
	/// State object storing state accessable during an Update() call
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed partial class UpdateState : IState
	{
		private float dt;
		private float hz;
		private Application application;
		private float seconds;
		private long deltaTime, totalTime, realTime;
		private IDictionary userValues;
		private PlayerInputCollection input;
		private readonly GamePadState[] pads;
		private UpdateManager manager;
		private bool async;
#if !XBOX360
		private readonly KeyboardInputState keyboard = new KeyboardInputState();
		private readonly MouseInputState mouse = new MouseInputState();
#endif

		/// <summary>
		/// The number of ticks that are in one second (10000000)
		/// </summary>
		public const long TicksInOneSecond = 10000000L;

		internal UpdateState(IDictionary userValues, Application application, PlayerInputCollection input, GamePadState[] pads)
		{
			this.userValues = userValues;
			this.application = application;
			this.pads = pads;
			this.input = input;
		}

		internal void UpdateDelta(float dt, float hz, long deltaTime)
		{
			this.dt = dt;
			this.hz = hz;
			this.deltaTime = deltaTime;
		}

		internal void Update(float dt, float hz, float seconds, long deltaTime, long totalTime, long realTime)
		{
			this.dt = dt;
			this.hz = hz;
			this.seconds = seconds;

			this.deltaTime = deltaTime;
			this.realTime = realTime;
			this.totalTime = totalTime;
		}
#if !XBOX
		internal void UpdateWindowsInput(ref KeyboardState kb, ref MouseState ms)
		{
			this.keyboard.Update(this.totalTime, ref kb);
			this.mouse.Update(this.totalTime,ref ms);
		}
#else
		internal void UpdateXboxInput(KeyboardState[] pads)
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.PlayerInput[i].ControlInput != ControlInput.KeyboardMouse)
					this.PlayerInput[i].UpdatePadState(this.totalTime, ref pads[(int)this.PlayerInput[i].ControlInput]);
			}
		}
#endif

		/// <summary>
		/// <para>The the item passed in will be drawn at the start of the next frame, before the main application Draw method is called</para>
		/// </summary>
		public void PreFrameDraw(IDraw item)
		{
			if (item == null)
				throw new ArgumentNullException();

			application.PreFrameDraw(item);
		}

		/// <summary>
		/// True if the current state is potentially running on multiple threads
		/// </summary>
		public bool IsAsynchronousState
		{
			get { return async; }
			internal set { async = value; }
		}

		/// <summary>
		/// Application instance
		/// </summary>
		public Application Application
		{
			get { return application; }
		}

		/// <summary>
		/// Gets the update manager managing this object
		/// </summary>
		public UpdateManager UpdateManager
		{
			get { return manager; }
			internal set { manager = value; }
		}


		/// <summary>
		/// Time delta (change) for the last frame/update as a frequency. Eg, 60 for 60fps
		/// </summary>
		public float DeltaTimeFrequency
		{
			get { return hz; }
		}

		/// <summary>
		/// Time delta (change) for the last frame/update as a number of seconds. Eg, 0.0166 for 60fps
		/// </summary>
		public float DeltaTimeSeconds
		{
			get { return dt; }
		}

		/// <summary>
		/// Accurate delta time
		/// </summary>
		public long DeltaTimeTicks
		{
			get { return deltaTime; }
		}

		/// <summary>
		/// Accurate total time
		/// </summary>
		public long TotalTimeTicks
		{
			get { return totalTime; }
		}
		/// <summary>
		/// Accurate system time since the application started
		/// </summary>
		public long TotalRealTimeTicks 
		{
			get { return realTime; }
		}
		/// <summary>
		/// Total time in seconds
		/// </summary>
		public float TotalTimeSeconds
		{
			get { return seconds; }
		}

		/// <summary>
		/// A dictionary for placing global values, if desired
		/// </summary>
		public IDictionary UserValues
		{
			get
			{
				return userValues;
			}
		}


#if XBOX360
		/// <summary>
		/// <para>Stores <see cref="PlayerInput"/> instances for each player.</para>
		/// <para>PlayerInput objects represent player GamePads. To access the Keyboard/Mouse in windows, use KeyboardState and MouseState</para>
		/// </summary>
#else
		/// <summary>
		/// <para>Stores <see cref="PlayerInput"/> instances for each player.</para>
		/// <para>PlayerInput objects represent player GamePads. To access the Keyboard/Mouse in windows, use <see cref="KeyboardState"/> and <see cref="MouseState"/></para>
		/// </summary>
#endif
		public PlayerInputCollection PlayerInput
		{
			get { return input; }
		}

		/// <summary>
		/// [Windows Only]
		/// For gamepad function, using <see cref="PlayerInput"/> is recommended over direct state access for simulating a gamepad
		/// </summary>
#if XBOX360
		[Obsolete("Use PlayerInput.ChatPadState")]
#endif
		public KeyboardInputState KeyboardState
		{
#if XBOX360
			get { throw new InvalidOperationException("Use PlayerInput.ChatPadState"); }
#else
			get { return keyboard; }
#endif
		}

#if !XBOX360

		/// <summary>
		/// [Windows Only]
		/// For gamepad function, using <see cref="PlayerInput"/> is recommended over direct state access for simulating a gamepad
		/// </summary>
		public MouseInputState MouseState
		{
			get { return mouse; }
		}

#endif

		/// <summary>
		/// Using <see cref="PlayerInput"/> is recommended over direct state access
		/// </summary>
		public void GetGamePadState(int index, out GamePadState state)
		{
			state = pads[index];
		}
	}

	//provides a unique id for a shader type (and efficiently too :)
	struct ShaderUID
	{
		private static readonly object sync = new object();
		private static uint id = 0;
		private static uint GetId()
		{
			lock (sync)
			{
				return id++;
			}
		}
		public struct Type<T> where T : IShader, new()
		{
			public static readonly uint ID = GetId();
		}
	}

	/// <summary>
	/// Provides access to the Render State, Camera, Culler and many other methods and objects used during drawing. This class should provide almost everything an application requires when drawing (as opposed to directly accessing the GraphicsDevice)
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed partial class DrawState : IState
	{
		private float dt;
		private float hz;
		private long deltaTime, totalTime, realTime;
		private Application application;
		private DrawTarget target;
		private float seconds;
		private IDictionary userValues;
		internal Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics;
		private bool protectedState;
		private int frameIndex;
		private IShader[] staticShaderCache = new IShader[1024];

		
		bool IState.IsAsynchronousState { get { return false; } }

 		/// <summary>Gets the frame index (incremented every time <see cref="Xen.Application.Draw(DrawState)"/> is called)</summary>
		public int FrameIndex
		{
			get { return frameIndex; }
		}

		internal void IncrementFrameIndex()
		{
			frameIndex++;
		}

		/// <summary>
		/// <para>Gets an application wide global instance of a Shader by type</para>
		/// <para>Use this method to share instances and reduce the number of live objects (most useful for simpler shaders)</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetShader<T>() where T : IShader, new()
		{
#if DEBUG
			ValidateProtected();
#endif

			uint id = ShaderUID.Type<T>.ID;
			if (id == staticShaderCache.Length)
				Array.Resize(ref staticShaderCache, staticShaderCache.Length * 2);

			T shader = (T)staticShaderCache[id];
			if (shader == null)
			{
#if DEBUG
				if (!typeof(Xen.Graphics.ShaderSystem.BaseShader).IsAssignableFrom(typeof(T)))
					throw new ArgumentException(string.Format("Shader {0} does not derive from BaseShader",typeof(T)));
#endif
				shader = new T();
				staticShaderCache[id] = shader;
			}
#if DEBUG
			if (shader.GetType() != typeof(T)) //sanity check
				throw new InvalidOperationException();
#endif
			return shader;
		}

		/// <summary>The number of ticks that are in one second (10000000)</summary>
		public const long TicksInOneSecond = 10000000L;

		internal bool Protected
		{
			set { protectedState = value; }
		}

		/// <summary>
		/// <para>The the item passed in will have it's Draw method called at the start of the next frame, before the main application Draw method is next called</para>
		/// </summary>
		public void PreFrameDraw(IDraw item)
		{
			if (item == null)
				throw new ArgumentNullException();

			application.PreFrameDraw(item);
		}

#if DEBUG

		/// <summary>Gets the <see cref="DrawStatistics"/> for the previous drawn frame (DEBUG ONLY)</summary>
		/// <param name="statistics"></param>
		public void GetPreviousFrameStatistics(out DrawStatistics statistics)
		{
			statistics = application.previousFrame;
		}

		/// <summary>Gets the <see cref="DrawStatistics"/> for the current inprogress frame, the data may be incomplete. Use <see cref="GetPreviousFrameStatistics"/> for full statistics on the previous frame. (DEBUG ONLY)</summary>
		/// <param name="statistics"></param>
		public void GetCurrentFrameStatistics(out DrawStatistics statistics)
		{
			statistics = application.currentFrame;
		}

#endif

		void ValidateProtected()
		{
			if (protectedState)
				throw new InvalidOperationException("This operation is not valid in the current context.");
		}


		internal DrawState(IDictionary userValues, Application application)
		{
			this.userValues = userValues;
			this.application = application;
			
			ms_World = new WorldStackProvider(renderStackSize,this);
			ms_View = new ViewProjectionProvider(false, this);
			ms_Projection = new ViewProjectionProvider(true, this);


			ms_Projection_Inverse = Inverse(ms_Projection);
			ms_Projection_Transpose = Transpose(ms_Projection);

			ms_View_Transpose = Transpose(ms_View);
			ms_View_Inverse = Inverse(ms_View);

			ms_World_Inverse = Inverse(ms_World);
			ms_World_Transpose = Transpose(ms_World);

			ms_ViewProjection = Mult(ms_View, ms_Projection);
			ms_WorldProjection = Mult(ms_World, ms_Projection);
			ms_WorldView = Mult(ms_World, ms_View);


			ms_ViewProjection_Inverse = Inverse(ms_ViewProjection);
			ms_ViewProjection_Transpose = Transpose(ms_ViewProjection);
			ms_WorldProjection_Inverse = Inverse(ms_WorldProjection);
			ms_WorldProjection_Transpose = Transpose(ms_WorldProjection);
			ms_WorldView_Transpose = Transpose(ms_WorldView);
			ms_WorldView_Inverse = Inverse(ms_WorldView);

			ms_WorldViewProjection = Mult(ms_World, ms_ViewProjection);

			ms_WorldViewProjection_Inverse = Inverse(ms_WorldViewProjection);
			ms_WorldViewProjection_Transpose = Transpose(ms_WorldViewProjection);


			InitDrawFlagCollection();
		}

		internal void Update(float dt, float hz, long deltaTime, long totalTime, long realTime, float seconds, Microsoft.Xna.Framework.Graphics.GraphicsDevice device)
		{
			this.deltaTime = deltaTime;
			this.totalTime = totalTime;
			this.realTime = realTime;

			this.dt = dt;
			this.hz = hz;
			this.target = null;
			this.seconds = seconds;
			this.graphics = device;

			if (this.vertexStreams == null)
				this.vertexStreams = new VStream[this.graphics.GraphicsDeviceCapabilities.MaxStreams];
		}

		internal void Update(Microsoft.Xna.Framework.Graphics.GraphicsDevice device)
		{
			this.graphics = device;
		}

		/// <summary>
		/// Application instance
		/// </summary>
		public Application Application
		{
			get { return application; }
		}

		/// <summary>
		/// Current DrawTarget that is being drawn to
		/// </summary>
		public DrawTarget DrawTarget
		{
			get { ValidateProtected();  return target; }
			internal set { target = value; }
		}

		//no validation...
		internal DrawTarget GetDrawTarget()
		{
			return target;
		}

		/// <summary>
		/// Hardware supports Shader Model 3 hardware instancing, or is running on the xbox360
		/// </summary>
		public bool SupportsHardwareInstancing
		{
			get { return application.SupportsHardwareInstancing; }
		}

		/// <summary>
		/// Approximate frame rate average for the last 20 drawn frames
		/// </summary>
		public float ApproximateFrameRate
		{
			get { ValidateProtected(); return application.approximateFrameRate; }
		}

		/// <summary>
		/// Time delta (change) for the last frame as a frequency. Eg, 60 for 60fps
		/// </summary>
		public float DeltaTimeFrequency
		{
			get { ValidateProtected(); return hz; }
		}


		/// <summary>
		/// Time delta (change) for the last frame as a number of seconds. Eg, 0.0166 for 60fps
		/// </summary>
		public float DeltaTimeSeconds
		{
			get { ValidateProtected(); return dt; }
		}

		/// <summary>
		/// Accurate delta time
		/// </summary>
		public long DeltaTimeTicks
		{
			get { ValidateProtected(); return deltaTime; }
		}

		/// <summary>
		/// Accurate total time
		/// </summary>
		public long TotalTimeTicks
		{
			get { ValidateProtected(); return totalTime; }
		}
		/// <summary>
		/// Accurate system time since the application started
		/// </summary>
		public long TotalRealTimeTicks
		{
			get { ValidateProtected(); return realTime; }
		}
		/// <summary>
		/// Total application time in seconds
		/// </summary>
		public float TotalTimeSeconds
		{
			get { ValidateProtected(); return seconds; }
		}

		/// <summary>
		/// A dictionary for placing global values, if desired
		/// </summary>
		public IDictionary UserValues
		{
			get
			{
				return userValues;
			}
		}

		internal DrawState GetProtectedClone()
		{
			DrawState state = (DrawState)this.MemberwiseClone();
			state.Protected = true;
			return state;
		}
	}

#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	sealed partial class AppState : IState, IUpdate
	{

		internal AppState(Application game)
		{
			this.application = game;

			this.playerInputCollection = new PlayerInputCollection(input);

			for (int i = 0; i < input.Length; i++)
				input[i] = new PlayerInput(i, this.playerInputCollection);

			input[0].ControlInput = ControlInput.GamePad1;
			input[1].ControlInput = ControlInput.GamePad2;
			input[2].ControlInput = ControlInput.GamePad3;
			input[3].ControlInput = ControlInput.GamePad4;


#if !XBOX360

			if (!GamePad.GetCapabilities(PlayerIndex.One).IsConnected)
				input[0].ControlInput = ControlInput.KeyboardMouse;

#endif

			this.renderState = new DrawState(this.userValues, this.application);
			this.protectedRenderState = this.renderState.GetProtectedClone();

			this.updateState = new UpdateState(this.userValues, this.application, this.playerInputCollection, this.pads);

			renderState.InitShaderCommon();
		}


		private float dt;
		private float hz;
		private Application application;
		private long deltaTime, totalTime, realTime, slowDownBias, initialRealTime;
		private float seconds;
		private IDictionary userValues = new Hashtable();
		private readonly PlayerInput[] input = new PlayerInput[4];
		private readonly PlayerInputCollection playerInputCollection;
		private readonly GamePadState[] pads = new GamePadState[4];

		private DrawState renderState;
		private DrawState protectedRenderState;
		private UpdateState updateState;

#if !XBOX360
		
		private KeyboardState keyboard;
		private MouseState mouse;
		private Point mousePrev;
		private bool mouseCen = false, mousePosSet = false, windowFocused = false, desireMouseCen;
		private int focusSkip = 0;
		private Point mouseCenTo;

#else

		private readonly KeyboardState[] chatPads = new KeyboardState[4];

#endif

		bool IState.IsAsynchronousState { get { return false; } }

		internal DrawState GetRenderState(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics)
		{
			this.renderState.Update(this.dt,this.hz,this.deltaTime,this.totalTime,this.realTime,this.seconds, graphics);
			return renderState;
		}
		internal DrawState GetProtectedDrawState(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics)
		{
			protectedRenderState.Update(graphics);
			return protectedRenderState;
		}

		internal UpdateState GetUpdateState()
		{
			this.updateState.Update(this.dt, this.hz, this.seconds,this.deltaTime,this.totalTime,this.realTime);

			return updateState;
		}


		public Application Application
		{
			get { return application; }
		}

		public float DeltaTimeFrequency
		{
			get { return hz; }
			internal set { hz = value; }
		}

		public float DeltaTimeSeconds
		{
			get { return dt; }
			internal set { dt = value; }
		}

		/// <summary>
		/// Always an array of length 4 (PlayerInput[4])
		/// </summary>
		public PlayerInput[] PlayerInput
		{
			get { return input; }
		}

#if !XBOX360
		
		public KeyboardState KeyboardState
		{
			get { return keyboard; }
		}

		internal Point MouseCentredPosition
		{
			get { return mouseCenTo; }
		}

		internal bool MouseCentred
		{
			get { return mouseCen && windowFocused; }
		}

		internal bool WindowFocused
		{
			get { return windowFocused; }
		}

		internal bool DesireMouseCentred
		{
			get { return desireMouseCen; }
			set { desireMouseCen = value; }
		}

		internal Point MousePreviousPosition
		{
			get { return mousePrev; }
		}

		/// <summary>
		/// [Windows Only]
		/// Using <see cref="PlayerInput"/> is recommended over direct state access
		/// </summary>
		public MouseState MouseState
		{
			get { return mouse; }
		}

#endif

		/// <summary>
		/// Using <see cref="PlayerInput"/> is recommended over direct state access
		/// </summary>
		public GamePadState[] GamePadState
		{
			get { return pads; }
		}


		public long DeltaTimeTicks
		{
			get { return deltaTime; }
		}

		public long TotalTimeTicks
		{
			get { return totalTime; }
		}
		public long TotalRealTimeTicks
		{
			get { return realTime; }
		}
		public float TotalTimeSeconds
		{
			get { return seconds; }
		}

		public const long TicksInOneSecond = 10000000L;

		internal void SetGameTime(long totalRealTicks, long totalGameTicks)
		{
			if (initialRealTime == 0)
				initialRealTime = totalRealTicks;

			this.realTime = totalRealTicks - initialRealTime;
			this.totalTime += deltaTime;
			long delta = (totalGameTicks - slowDownBias) - totalTime;

			int minFps = application.MinimumDesiredFrameRate;
			if (minFps != 0)
			{
				if (delta > TicksInOneSecond / minFps)
				{
					//getting very slow here, so keep the app running, just slow logic down
					slowDownBias += delta - (TicksInOneSecond / minFps);
					delta = TicksInOneSecond / minFps;
				}
			}

			if (application.VSyncEnabled)
			{
				//when vsync is on, bias the delta time towards it..
				//this will have no impact on game speed, but is intended to keep the delta time
				//as consistent intervals, reducing jitter that results from sampling actual time.

				//this should not have an impact on game timing on a longer term scale
				long refresh = application.GraphicsDevice.DisplayMode.RefreshRate;
				if (refresh == 0 || refresh == 59 || refresh == 61)
					refresh = 60;


				long ticks = TicksInOneSecond / refresh;

				if (delta < 0)
				{
					//getting a little far ahead
				//	System.Threading.Thread.Sleep(new TimeSpan(-delta));
				}

				if (delta < ticks)
				{
					//somehow over 60fps...?
					delta = ticks;
				}
				else
				{
					long dif = ticks;
					for (int i = 0; i < 5; i++)
					{
						if (i == 4)
							break;
						if (Math.Abs(delta - ticks) < dif)
						{
							delta = ticks;
							break;
						}
						ticks *= 2;
						dif *= 2;
					}
				}
			}
			this.deltaTime = delta;

			dt = (float)(delta / (double)TicksInOneSecond);
			hz = delta > 0 ? (float)(TicksInOneSecond / (double)delta) : 0;

			if (Math.Abs(Math.Round(hz) - hz) < 0.001)
			{
				hz = (float)Math.Round(hz);
			}

			seconds = (float)(totalTime / (double)TicksInOneSecond);

			if (seconds == 0)
				UpdateInput(null);
		}

		UpdateFrequency IUpdate.Update(UpdateState state)
		{
			UpdateInput(state);

#if !XBOX360
			this.updateState.UpdateWindowsInput(ref this.keyboard, ref this.mouse);
#else
			this.updateState.UpdateXboxInput(chatPads);
#endif

			return UpdateFrequency.FullUpdate60hz;
		}

		private void UpdateInput(UpdateState state)
		{
			for (int i = 0; i < 4; i++)
			{
				pads[i] = Microsoft.Xna.Framework.Input.GamePad.GetState((PlayerIndex)i);
#if XBOX360
				chatPads[i] = Keyboard.GetState((PlayerIndex)i);
#endif
			}

#if !XBOX360

			mousePrev = new Point(mouse.X, mouse.Y);
			application.Logic.GetInputState(ref keyboard, ref mouse, ref windowFocused, ref mouseCen, ref mouseCenTo);

			if (!windowFocused)
				focusSkip = 5;
			if (!mousePosSet || focusSkip > 0)
			{
				focusSkip--;
				mousePrev = new Point(mouse.X, mouse.Y);
				mouseCenTo = mousePrev;
				mousePosSet = true;
			}

			XNALogic.SetMouseCentreState(desireMouseCen && windowFocused);
			desireMouseCen = false;

#endif
			UpdateState updateState = state ?? GetUpdateState();

			for (int i = 0; i < 4; i++)
			{
				input[i].mapper.SetKMS(this, input[i].ControlInput);
#if XBOX360
				bool windowFocused = true;
#endif
				input[i].mapper.UpdateState(ref input[i].istate, updateState, input[i].KeyboardMouseControlMapping, input[i].ControlInput, windowFocused);
			}
		}

		static internal bool MatrixNotEqual(ref Matrix m1, ref Matrix m2)
		{
			//as ripped out of xna via reflector
			if (((((m1.M11 == m2.M11) && (m1.M12 == m2.M12)) && ((m1.M13 == m2.M13) && (m1.M14 == m2.M14))) && (((m1.M21 == m2.M21) && (m1.M22 == m2.M22)) && ((m1.M23 == m2.M23) && (m1.M24 == m2.M24)))) && ((((m1.M31 == m2.M31) && (m1.M32 == m2.M32)) && ((m1.M33 == m2.M33) && (m1.M34 == m2.M34))) && (((m1.M41 == m2.M41) && (m1.M42 == m2.M42)) && (m1.M43 == m2.M43))))
			{
				return (m1.M44 != m2.M44);
			}
			return true;
		}

		static internal void ApproxMatrixScale(ref Matrix m, out float length)
		{
			length =  m.M11 * m.M11 + m.M12 * m.M12 + m.M13 * m.M13;
			float y = m.M21 * m.M21 + m.M22 * m.M22 + m.M23 * m.M23;
			float z = m.M31 * m.M31 + m.M32 * m.M32 + m.M33 * m.M33;
			length = Math.Max(length, Math.Max(y, z));
			if (length < 0.99995f || length > 1.00005f)
				length = (float)Math.Sqrt(length);
		}

		static internal bool MatrixIsNotIdentiy(ref Matrix matrix)
		{
			return ((matrix.M11 != 1 || matrix.M22 != 1 || matrix.M33 != 1 || matrix.M44 != 1) ||
				(matrix.M12 != 0 || matrix.M13 != 0 || matrix.M14 != 0) ||
				(matrix.M21 != 0 || matrix.M23 != 0 || matrix.M24 != 0) ||
				(matrix.M31 != 0 || matrix.M32 != 0 || matrix.M34 != 0) ||
				(matrix.M41 != 0 || matrix.M42 != 0 || matrix.M43 != 0));
		}

		/// <summary>
		/// A dictionary for placing global values, if desired
		/// </summary>
		public IDictionary UserValues
		{
			get 
			{
				return userValues;
			}
		}

		#region IState Members

		Application IState.Application
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		float IState.DeltaTimeFrequency
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		float IState.DeltaTimeSeconds
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		long IState.DeltaTimeTicks
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		long IState.TotalTimeTicks
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		long IState.TotalRealTimeTicks
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		float IState.TotalTimeSeconds
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		IDictionary IState.UserValues
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		#endregion
	}

#if DEBUG
	/// <summary>
	/// Draw statistics are only avaliable in debug builds
	/// </summary>
	public struct DrawStatistics
	{
		System.Diagnostics.Stopwatch timer;

		internal void Begin()
		{
			if (timer == null)
				timer = new System.Diagnostics.Stopwatch();
			timer.Reset();
			timer.Start();
		}
		internal void End()
		{
			if (timer != null)
			{
				timer.Stop();
				ApproximateDrawTimeTicks = timer.ElapsedTicks;
			}
		}

		internal void Reset()
		{
			System.Diagnostics.Stopwatch timer = this.timer;
			this = new DrawStatistics();
			this.timer = timer;
		}

		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static DrawStatistics operator +(DrawStatistics a, DrawStatistics b)
		{
			return Madd(ref a, ref b, 1);
		}
		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static DrawStatistics operator -(DrawStatistics a, DrawStatistics b)
		{
			return Madd(ref a, ref b, -1);
		}

		private static DrawStatistics Madd(ref DrawStatistics a, ref DrawStatistics b, int mul)
		{
			DrawStatistics s;

			s.ApproximateDrawTimeTicks = a.ApproximateDrawTimeTicks;

			//somtimes, regular expressions are just so very useful...
			s.DrawIndexedPrimitiveCallCount = a.DrawIndexedPrimitiveCallCount + b.DrawIndexedPrimitiveCallCount * mul;
			s.DrawPrimitivesCallCount = a.DrawPrimitivesCallCount + b.DrawPrimitivesCallCount * mul;
			s.TrianglesDrawn = a.TrianglesDrawn + b.TrianglesDrawn * mul;
			s.LinesDrawn = a.LinesDrawn + b.LinesDrawn * mul;
			s.PointsDrawn = a.PointsDrawn + b.PointsDrawn * mul;
			s.InstancesDrawBatchCount = a.InstancesDrawBatchCount + b.InstancesDrawBatchCount * mul;
			s.InstancesDrawn = a.InstancesDrawn + b.InstancesDrawn * mul;
			s.VertexShaderConstantBytesSetTotalCount = a.VertexShaderConstantBytesSetTotalCount + b.VertexShaderConstantBytesSetTotalCount * mul;
			s.PixelShaderConstantBytesSetTotalCount = a.PixelShaderConstantBytesSetTotalCount + b.PixelShaderConstantBytesSetTotalCount * mul;
			s.VertexShaderConstantBytesSetCount = a.VertexShaderConstantBytesSetCount + b.VertexShaderConstantBytesSetCount * mul;
			s.PixelShaderConstantBytesSetCount = a.PixelShaderConstantBytesSetCount + b.PixelShaderConstantBytesSetCount * mul;
			s.ShaderBindCount = a.ShaderBindCount + b.ShaderBindCount * mul;
			s.DrawIndexedPrimitiveCallCount = a.DrawIndexedPrimitiveCallCount + b.DrawIndexedPrimitiveCallCount * mul;
			s.VertexShaderBoundWithKnownInstructionsCount = a.VertexShaderBoundWithKnownInstructionsCount + b.VertexShaderBoundWithKnownInstructionsCount * mul;
			s.VertexShaderBoundWithPreShaderCount = a.VertexShaderBoundWithPreShaderCount + b.VertexShaderBoundWithPreShaderCount * mul;
			s.VertexShaderApproximateInstructionsTotal = a.VertexShaderApproximateInstructionsTotal + b.VertexShaderApproximateInstructionsTotal * mul;
			s.VertexShaderApproximatePreshaderInstructionsTotal = a.VertexShaderApproximatePreshaderInstructionsTotal + b.VertexShaderApproximatePreshaderInstructionsTotal * mul;
			s.PixelShaderBoundWithKnownInstructionsCount = a.PixelShaderBoundWithKnownInstructionsCount + b.PixelShaderBoundWithKnownInstructionsCount * mul;
			s.PixelShaderBoundWithPreShaderCount = a.PixelShaderBoundWithPreShaderCount + b.PixelShaderBoundWithPreShaderCount * mul;
			s.PixelShaderApproximateInstructionsTotal = a.PixelShaderApproximateInstructionsTotal + b.PixelShaderApproximateInstructionsTotal * mul;
			s.PixelShaderApproximatePreshaderInstructionsTotal = a.PixelShaderApproximatePreshaderInstructionsTotal + b.PixelShaderApproximatePreshaderInstructionsTotal * mul;
			s.ShaderRebindCount = a.ShaderRebindCount + b.ShaderRebindCount * mul;
			s.DrawTargetsDrawCount = a.DrawTargetsDrawCount + b.DrawTargetsDrawCount * mul;
			s.DrawTargetsPassCount = a.DrawTargetsPassCount + b.DrawTargetsPassCount * mul;
			s.BinaryShadersCreated = a.BinaryShadersCreated + b.BinaryShadersCreated * mul;
			s.BinaryShadersSet = a.BinaryShadersSet + b.BinaryShadersSet * mul;
			s.DeferredDrawCallsMade = a.DeferredDrawCallsMade + b.DeferredDrawCallsMade * mul;
			s.DeferredDrawCallsCulled = a.DeferredDrawCallsCulled + b.DeferredDrawCallsCulled * mul;
			s.ShaderConstantMatrixValueSetCount = a.ShaderConstantMatrixValueSetCount + b.ShaderConstantMatrixValueSetCount * mul;
			s.ShaderConstantMatrixInverseCalculateCount = a.ShaderConstantMatrixInverseCalculateCount + b.ShaderConstantMatrixInverseCalculateCount * mul;
			s.ShaderConstantMatrixTransposeCalculateCount = a.ShaderConstantMatrixTransposeCalculateCount + b.ShaderConstantMatrixTransposeCalculateCount * mul;
			s.ShaderConstantMatrixMultiplyCalculateCount = a.ShaderConstantMatrixMultiplyCalculateCount + b.ShaderConstantMatrixMultiplyCalculateCount * mul;
			s.ShaderConstantMatrixViewChangedCount = a.ShaderConstantMatrixViewChangedCount + b.ShaderConstantMatrixViewChangedCount * mul;
			s.ShaderConstantMatrixProjectionChangedCount = a.ShaderConstantMatrixProjectionChangedCount + b.ShaderConstantMatrixProjectionChangedCount * mul;
			s.ShaderConstantMatrixSetWorldMatrixCount = a.ShaderConstantMatrixSetWorldMatrixCount + b.ShaderConstantMatrixSetWorldMatrixCount * mul;
			s.ShaderConstantMatrixPushWorldMatrixCount = a.ShaderConstantMatrixPushWorldMatrixCount + b.ShaderConstantMatrixPushWorldMatrixCount * mul;
			s.ShaderConstantMatrixPushTranslateWorldMatrixCount = a.ShaderConstantMatrixPushTranslateWorldMatrixCount + b.ShaderConstantMatrixPushTranslateWorldMatrixCount * mul;
			s.ShaderConstantMatrixPushMultiplyTranslateWorldMatrixCount = a.ShaderConstantMatrixPushMultiplyTranslateWorldMatrixCount + b.ShaderConstantMatrixPushMultiplyTranslateWorldMatrixCount * mul;
			s.ShaderConstantMatrixPushMultiplyWorldMatrixCount = a.ShaderConstantMatrixPushMultiplyWorldMatrixCount + b.ShaderConstantMatrixPushMultiplyWorldMatrixCount * mul;
			s.ShaderConstantMatrixGlobalAssignCount = a.ShaderConstantMatrixGlobalAssignCount + b.ShaderConstantMatrixGlobalAssignCount * mul;
			s.ShaderConstantVectorGlobalAssignCount = a.ShaderConstantVectorGlobalAssignCount + b.ShaderConstantVectorGlobalAssignCount * mul;
			s.ShaderConstantSingleGlobalAssignCount = a.ShaderConstantSingleGlobalAssignCount + b.ShaderConstantSingleGlobalAssignCount * mul;
			s.ShaderConstantArrayGlobalAssignCount = a.ShaderConstantArrayGlobalAssignCount + b.ShaderConstantArrayGlobalAssignCount * mul;
			s.ShaderConstantArrayGlobalAssignTotalBytes = a.ShaderConstantArrayGlobalAssignTotalBytes + b.ShaderConstantArrayGlobalAssignTotalBytes * mul;
			s.ShaderConstantTextureGlobalAssignCount = a.ShaderConstantTextureGlobalAssignCount + b.ShaderConstantTextureGlobalAssignCount * mul;
			s.ShaderMatrixGlobalSetCount = a.ShaderMatrixGlobalSetCount + b.ShaderMatrixGlobalSetCount * mul;
			s.ShaderVectorGlobalSetCount = a.ShaderVectorGlobalSetCount + b.ShaderVectorGlobalSetCount * mul;
			s.ShaderSingleGlobalSetCount = a.ShaderSingleGlobalSetCount + b.ShaderSingleGlobalSetCount * mul;
			s.ShaderTextureGlobalSetCount = a.ShaderTextureGlobalSetCount + b.ShaderTextureGlobalSetCount * mul;
			s.ShaderArrayGlobalSetCount = a.ShaderArrayGlobalSetCount + b.ShaderArrayGlobalSetCount * mul;
			s.ShaderArrayGlobalSetBytesTotal = a.ShaderArrayGlobalSetBytesTotal + b.ShaderArrayGlobalSetBytesTotal * mul;
			s.ShaderArrayGlobalGetCount = a.ShaderArrayGlobalGetCount + b.ShaderArrayGlobalGetCount * mul;
			s.ShaderMatrixGlobalGetCount = a.ShaderMatrixGlobalGetCount + b.ShaderMatrixGlobalGetCount * mul;
			s.ShaderVectorGlobalGetCount = a.ShaderVectorGlobalGetCount + b.ShaderVectorGlobalGetCount * mul;
			s.ShaderSingleGlobalGetCount = a.ShaderSingleGlobalGetCount + b.ShaderSingleGlobalGetCount * mul;
			s.ShaderTextureGlobalGetCount = a.ShaderTextureGlobalGetCount + b.ShaderTextureGlobalGetCount * mul;
			s.SetCameraCount = a.SetCameraCount + b.SetCameraCount * mul;
			s.BeginGetGraphicsDeviceCount = a.BeginGetGraphicsDeviceCount + b.BeginGetGraphicsDeviceCount * mul;
			s.DirtyRenderStateCount = a.DirtyRenderStateCount + b.DirtyRenderStateCount * mul;
			s.DirtyRenderShadersStateCount = a.DirtyRenderShadersStateCount + b.DirtyRenderShadersStateCount * mul;
			s.DirtyRenderVerticesAndIndicesStateCount = a.DirtyRenderVerticesAndIndicesStateCount + b.DirtyRenderVerticesAndIndicesStateCount * mul;
			s.DirtyRenderTexturesStateCount = a.DirtyRenderTexturesStateCount + b.DirtyRenderTexturesStateCount * mul;
			s.DrawXnaVerticesCount = a.DrawXnaVerticesCount + b.DrawXnaVerticesCount * mul;
			s.DefaultCullerTestSphereCount = a.DefaultCullerTestSphereCount + b.DefaultCullerTestSphereCount * mul;
			s.DefaultCullerTestSphereCulledCount = a.DefaultCullerTestSphereCulledCount + b.DefaultCullerTestSphereCulledCount * mul;
			s.DefaultCullerTestSpherePreCulledCount = a.DefaultCullerTestSpherePreCulledCount + b.DefaultCullerTestSpherePreCulledCount * mul;
			s.DefaultCullerTestSpherePostCulledCount = a.DefaultCullerTestSpherePostCulledCount + b.DefaultCullerTestSpherePostCulledCount * mul;
			s.DefaultCullerTestBoxCount = a.DefaultCullerTestBoxCount + b.DefaultCullerTestBoxCount * mul;
			s.DefaultCullerTestBoxCulledCount = a.DefaultCullerTestBoxCulledCount + b.DefaultCullerTestBoxCulledCount * mul;
			s.DefaultCullerTestBoxPreCulledCount = a.DefaultCullerTestBoxPreCulledCount + b.DefaultCullerTestBoxPreCulledCount * mul;
			s.DefaultCullerTestBoxPostCulledCount = a.DefaultCullerTestBoxPostCulledCount + b.DefaultCullerTestBoxPostCulledCount * mul;
			s.BufferClearTargetCount = a.BufferClearTargetCount + b.BufferClearTargetCount * mul;
			s.BufferClearDepthCount = a.BufferClearDepthCount + b.BufferClearDepthCount * mul;
			s.BufferClearStencilCount = a.BufferClearStencilCount + b.BufferClearStencilCount * mul;
			s.RenderStateAlphaBlendChangedCount = a.RenderStateAlphaBlendChangedCount + b.RenderStateAlphaBlendChangedCount * mul;
			s.RenderStateAlphaTestChangedCount = a.RenderStateAlphaTestChangedCount + b.RenderStateAlphaTestChangedCount * mul;
			s.RenderStateDepthColourCullChangedCount = a.RenderStateDepthColourCullChangedCount + b.RenderStateDepthColourCullChangedCount * mul;
			s.RenderStateStencilTestChangedCount = a.RenderStateStencilTestChangedCount + b.RenderStateStencilTestChangedCount * mul;
			s.VertexBufferByesCopied = a.VertexBufferByesCopied + b.VertexBufferByesCopied * mul;
			s.IndexBufferByesCopied = a.IndexBufferByesCopied + b.IndexBufferByesCopied * mul;
			s.DynamicVertexBufferByesCopied = a.DynamicVertexBufferByesCopied + b.DynamicVertexBufferByesCopied * mul;
			s.DynamicIndexBufferByesCopied = a.DynamicIndexBufferByesCopied + b.DynamicIndexBufferByesCopied * mul;
			s.TextureSamplerAddressStateChanged = a.TextureSamplerAddressStateChanged + b.TextureSamplerAddressStateChanged * mul;
			s.TextureSamplerFilterStateChanged = a.TextureSamplerFilterStateChanged + b.TextureSamplerFilterStateChanged * mul;
			s.TextureUnitTextureChanged = a.TextureUnitTextureChanged + b.TextureUnitTextureChanged * mul;
			s.VertexTextureUnitTextureChanged = a.VertexTextureUnitTextureChanged + b.VertexTextureUnitTextureChanged * mul;
			s.BoundTextureCount = a.BoundTextureCount + b.BoundTextureCount * mul;
			s.BoundVertexTextureCount = a.BoundVertexTextureCount + b.BoundVertexTextureCount * mul;
			s.BoundTextureCountTotalSamples = a.BoundTextureCountTotalSamples + b.BoundTextureCountTotalSamples * mul;
#if XBOX360
			s.XboxPixelFillBias = a.XboxPixelFillBias + b.XboxPixelFillBias * mul;
#endif

			s.timer = null;
			return s;
		}

		/// <summary></summary>
		public long ApproximateDrawTimeTicks;
		/// <summary></summary>
		public int DrawIndexedPrimitiveCallCount;
		/// <summary></summary>
		public int DrawPrimitivesCallCount;
		/// <summary></summary>
		public int TrianglesDrawn;
		/// <summary></summary>
		public int LinesDrawn;
		/// <summary></summary>
		public int PointsDrawn;
		/// <summary></summary>
		public int InstancesDrawBatchCount;
		/// <summary></summary>
		public int InstancesDrawn;
		/// <summary></summary>
		public int VertexShaderConstantBytesSetTotalCount;
		/// <summary></summary>
		public int PixelShaderConstantBytesSetTotalCount;
		/// <summary></summary>
		public int VertexShaderConstantBytesSetCount;
		/// <summary></summary>
		public int PixelShaderConstantBytesSetCount;
		/// <summary></summary>
		public int ShaderBindCount;
		/// <summary></summary>
		public int VertexShaderBoundWithKnownInstructionsCount;
		/// <summary></summary>
		public int VertexShaderBoundWithPreShaderCount;
		/// <summary></summary>
		public int VertexShaderApproximateInstructionsTotal;
		/// <summary></summary>
		public int VertexShaderApproximatePreshaderInstructionsTotal;
		/// <summary></summary>
		public int PixelShaderBoundWithKnownInstructionsCount;
		/// <summary></summary>
		public int PixelShaderBoundWithPreShaderCount;
		/// <summary></summary>
		public int PixelShaderApproximateInstructionsTotal;
		/// <summary></summary>
		public int PixelShaderApproximatePreshaderInstructionsTotal;
		/// <summary></summary>
		public int ShaderRebindCount;
		/// <summary></summary>
		public int DrawTargetsDrawCount;
		/// <summary></summary>
		public int DrawTargetsPassCount;
		/// <summary></summary>
		public int BinaryShadersCreated;
		/// <summary></summary>
		public int BinaryShadersSet;
		/// <summary></summary>
		public int DeferredDrawCallsMade;
		/// <summary></summary>
		public int DeferredDrawCallsCulled;
		/// <summary></summary>
		public int ShaderConstantMatrixValueSetCount;
		/// <summary></summary>
		public int ShaderConstantMatrixInverseCalculateCount;
		/// <summary></summary>
		public int ShaderConstantMatrixTransposeCalculateCount;
		/// <summary></summary>
		public int ShaderConstantMatrixMultiplyCalculateCount;
		/// <summary></summary>
		public int ShaderConstantMatrixViewChangedCount;
		/// <summary></summary>
		public int ShaderConstantMatrixProjectionChangedCount;
		/// <summary></summary>
		public int ShaderConstantMatrixSetWorldMatrixCount;
		/// <summary></summary>
		public int ShaderConstantMatrixPushWorldMatrixCount;
		/// <summary></summary>
		public int ShaderConstantMatrixPushTranslateWorldMatrixCount;
		/// <summary></summary>
		public int ShaderConstantMatrixPushMultiplyTranslateWorldMatrixCount;
		/// <summary></summary>
		public int ShaderConstantMatrixPushMultiplyWorldMatrixCount;
		/// <summary></summary>
		public int ShaderConstantMatrixGlobalAssignCount;
		/// <summary></summary>
		public int ShaderConstantVectorGlobalAssignCount;
		/// <summary></summary>
		public int ShaderConstantSingleGlobalAssignCount;
		/// <summary></summary>
		public int ShaderConstantArrayGlobalAssignCount;
		/// <summary></summary>
		public int ShaderConstantArrayGlobalAssignTotalBytes;
		/// <summary></summary>
		public int ShaderConstantTextureGlobalAssignCount;
		/// <summary></summary>
		public int ShaderMatrixGlobalSetCount;
		/// <summary></summary>
		public int ShaderVectorGlobalSetCount;
		/// <summary></summary>
		public int ShaderSingleGlobalSetCount;
		/// <summary></summary>
		public int ShaderTextureGlobalSetCount;
		/// <summary></summary>
		public int ShaderArrayGlobalSetCount;
		/// <summary></summary>
		public int ShaderArrayGlobalSetBytesTotal;
		/// <summary></summary>
		public int ShaderArrayGlobalGetCount;
		/// <summary></summary>
		public int ShaderMatrixGlobalGetCount;
		/// <summary></summary>
		public int ShaderVectorGlobalGetCount;
		/// <summary></summary>
		public int ShaderSingleGlobalGetCount;
		/// <summary></summary>
		public int ShaderTextureGlobalGetCount;
		/// <summary></summary>
		public int SetCameraCount;
		/// <summary></summary>
		public int BeginGetGraphicsDeviceCount;
		/// <summary></summary>
		public int DirtyRenderStateCount;
		/// <summary></summary>
		public int DirtyRenderShadersStateCount;
		/// <summary></summary>
		public int DirtyRenderVerticesAndIndicesStateCount;
		/// <summary></summary>
		public int DirtyRenderTexturesStateCount;
		/// <summary></summary>
		public int DrawXnaVerticesCount;
		/// <summary></summary>
		public int DefaultCullerTestSphereCount;
		/// <summary></summary>
		public int DefaultCullerTestSphereCulledCount;
		/// <summary></summary>
		public int DefaultCullerTestSpherePreCulledCount;
		/// <summary></summary>
		public int DefaultCullerTestSpherePostCulledCount;
		/// <summary></summary>
		public int DefaultCullerTestBoxCount;
		/// <summary></summary>
		public int DefaultCullerTestBoxCulledCount;
		/// <summary></summary>
		public int DefaultCullerTestBoxPreCulledCount;
		/// <summary></summary>
		public int DefaultCullerTestBoxPostCulledCount;
		/// <summary></summary>
		public int BufferClearTargetCount;
		/// <summary></summary>
		public int BufferClearDepthCount;
		/// <summary></summary>
		public int BufferClearStencilCount;
		/// <summary></summary>
		public int RenderStateAlphaBlendChangedCount;
		/// <summary></summary>
		public int RenderStateAlphaTestChangedCount;
		/// <summary></summary>
		public int RenderStateDepthColourCullChangedCount;
		/// <summary></summary>
		public int RenderStateStencilTestChangedCount;

		/// <summary></summary>
		public int VertexBufferByesCopied;
		/// <summary></summary>
		public int IndexBufferByesCopied;
		/// <summary></summary>
		public int DynamicVertexBufferByesCopied;
		/// <summary></summary>
		public int DynamicIndexBufferByesCopied;

		/// <summary></summary>
		public int TextureSamplerAddressStateChanged;
		/// <summary></summary>
		public int TextureSamplerFilterStateChanged;
		/// <summary></summary>
		public int TextureUnitTextureChanged;
		/// <summary></summary>
		public int VertexTextureUnitTextureChanged;

		/// <summary></summary>
		public int BoundTextureCount;
		/// <summary></summary>
		public int BoundVertexTextureCount;
		/// <summary></summary>
		public int BoundTextureCountTotalSamples;

#if XBOX360
		/// <summary>Number of pixels that have been filled by XNA internal render target code</summary>
		public int XboxPixelFillBias;
#endif
	}
#endif
}
