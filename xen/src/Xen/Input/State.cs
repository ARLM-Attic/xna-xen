
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xen.Graphics;
using Xen.Input.Mapping;
using Xen.Input.State;
#endregion

namespace Xen.Input.State
{
	/// <summary>
	/// Structure storing current keyboard and mouse state
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct KeyboardMouseState
	{
		private KeyboardState keys;

		/// <summary>
		/// Current keyboard state
		/// </summary>
		public KeyboardState KeyboardState
		{
			get { return keys; }
			set { keys = value; }
		}

#if !XBOX360
		private bool windowFocused;

		internal bool WindowFocused
		{
			get { return windowFocused; }
			set 
			{
				if (value != windowFocused)
				{
					windowFocused = value;
					if (value)
					{
						ms = new MouseState(mousePos.X, mousePos.Y, ms.ScrollWheelValue, ms.LeftButton, ms.MiddleButton, ms.RightButton, ms.XButton1, ms.XButton2);
					}
				}
			}
		}

		private MouseState ms;
		private Point mousePos;

		/// <summary>
		/// [Windows Only]
		/// </summary>
		internal Point MousePositionPrevious
		{
			get { return mousePos; }
			set { mousePos = value; }
		}

		/// <summary>
		/// [Windows Only] Current mouse state
		/// </summary>
		public MouseState MouseState
		{
			get { return ms; }
			set { ms = value; }
		}

#endif
	}

	/// <summary>
	/// Structure storing the state of a digital (on/off) button
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct Button
	{
		bool pressed, prev;
		long pressTick, releaseTick;
		float held;
		float releaseTime;

		/// <summary>
		/// True if the button is pressed
		/// </summary>
		public bool IsDown
		{
			get { return pressed; }
		}
		/// <summary>
		/// Will return true for the SINGLE FRAME the button changes state from unpressed to pressed
		/// </summary>
		public bool OnPressed
		{
			get { return pressed && !prev; }
		}
		/// <summary>
		/// Will return true for the SINGLE FRAME the button changes state from pressed to unpressed
		/// </summary>
		public bool OnReleased
		{
			get { return !pressed && prev; }
		}
		/// <summary>
		/// Number of seconds the button was last held down for
		/// </summary>
		public float DownDuration
		{
			get { return held; }
		}
		/// <summary>
		/// Number of seconds since the the botton was last released (May be useful for calculating double presses)
		/// </summary>
		public float ReleaseTime
		{
			get { return releaseTime; }
		}
		internal void SetState(bool value, long tick)
		{
			if (value && !pressed)
				pressTick = tick;
			if (value)
				held = (float)((tick - pressTick) / (double)UpdateState.TicksInOneSecond);
			prev = pressed;
			pressed = value;
			if (!pressed && prev)
				releaseTick = tick;
			if (releaseTick != 0)
				releaseTime = (float)((tick - releaseTick) / (double)UpdateState.TicksInOneSecond);
		}

		/// <summary>
		/// Implicit conversion to a boolean (equivalent of <see cref="IsDown"/>)
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public static implicit operator bool(Button b)
		{
			return b.IsDown;
		}
	}
		
	/// <summary>
	/// Structure storing the current state of a gamepad's buttons
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct InputButtons
	{
		internal Button a, b, x, y, back, start, leftStickClick, rightStickClick, dpadL, dpadR, dpadU, dpadD, shoulderL, shoulderR;

		/// <summary></summary>
		public Button A { get { return a; } }
		/// <summary></summary>
		public Button B { get { return b; } }
		/// <summary></summary>
		public Button X { get { return x; } }
		/// <summary></summary>
		public Button Y { get { return y; } }
		/// <summary></summary>
		public Button Back { get { return back; } }
		/// <summary></summary>
		public Button Start { get { return start; } }
		/// <summary></summary>
		public Button LeftStickClick { get { return leftStickClick; } }
		/// <summary></summary>
		public Button RightStickClick { get { return rightStickClick; } }
		/// <summary></summary>
		public Button DpadLeft { get { return dpadL; } }
		/// <summary></summary>
		public Button DpadRight { get { return dpadR; } }
		/// <summary></summary>
		public Button DpadUp { get { return dpadU; } }
		/// <summary></summary>
		public Button DpadDown { get { return dpadD; } }
		/// <summary></summary>
		public Button LeftShoulder { get { return shoulderL; } }
		/// <summary></summary>
		public Button RightShoulder { get { return shoulderR; } }
	}

	/// <summary>
	/// Structure storing the current state of a gamepad's thumbsticks
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct InputThumbSticks
	{
		internal Vector2 leftStick, rightStick;
		/// <summary></summary>
		public Vector2 LeftStick
		{
			get { return leftStick; }
		}
		/// <summary></summary>
		public Vector2 RightStick
		{
			get { return rightStick; }
		}
	}

	/// <summary>
	/// Structure storing the current state of a gamepad's triggers
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct InputTriggers
	{
		internal float leftTrigger, rightTrigger;
		/// <summary></summary>
		public float LeftTrigger
		{
			get { return leftTrigger; }
		}
		/// <summary></summary>
		public float RightTrigger
		{
			get { return rightTrigger; }
		}
	}

	/// <summary>
	/// Structure storing the current state of a gamepad's buttons, triggers and thumbsticks
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class InputState
	{
		internal InputState()
		{
			//this constructor is never called - but is here to prevent the compiler thinking the members are never used
			this.triggers = new InputTriggers();
			this.sticks = new InputThumbSticks();
			this.buttons = new InputButtons();
		}

		internal InputTriggers triggers;

		/// <summary></summary>
		public InputTriggers Triggers
		{
			get { return triggers; }
		}

		internal InputThumbSticks sticks;

		/// <summary></summary>
		public InputThumbSticks ThumbSticks
		{
			get { return sticks; }
		}

		internal InputButtons buttons;
		/// <summary></summary>
		public InputButtons Buttons
		{
			get { return buttons; }
		}

	}
}


