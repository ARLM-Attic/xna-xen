
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
	/// Structure storing current raw keyboard and mouse state (used for internal logic classes only)
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
		int heldTicks;
		int releasedTicks;

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
			get { return (float)((heldTicks) / (double)UpdateState.TicksInOneSecond); }
		}
		/// <summary>
		/// Number of seconds since the the botton was last released (May be useful for calculating double presses)
		/// </summary>
		public float ReleaseTime
		{
			get { return (float)((releasedTicks) / (double)UpdateState.TicksInOneSecond); }
		}
		internal void SetState(bool value, long tick)
		{
			if (value && !pressed)
				pressTick = tick;
			if (value)
				heldTicks = (int)(tick - pressTick);
			prev = pressed;
			pressed = value;
			if (!pressed && prev)
				releaseTick = tick;
			if (releaseTick != 0)
				releasedTicks = (int)(tick - releasedTicks);
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

	#region keystate

	class MappingAttribute : Attribute
	{
		public readonly char mapping;
		public MappingAttribute(char mapping)
		{
			this.mapping = mapping;
		}
	}

	/// <summary>
	/// Stores a list of <see cref="Button"/> Key States
	/// </summary>
	public sealed class KeyState
	{
		private readonly Button[] buttons;
		internal KeyState(Button[] buttons)
		{
			this.buttons = buttons;
		}

		/// <summary>
		/// Button Indexer (Keys)
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Button this[Keys key]
		{
			get { return this.buttons[(int)key]; }
		}

		//don't worry, this was made with a regular expression - mostly :-)

		/// <summary>The 'A' button</summary>
		[Mapping('a')]
		public Button A { get { return buttons[0x41]; } }
		/// <summary>The 'Add' button</summary>
		[Mapping('+')]
		public Button Add { get { return buttons[0x6b]; } }
		/// <summary>The 'Apps' button</summary>
		public Button Apps { get { return buttons[0x5d]; } }
		/// <summary>The 'Attn' button</summary>
		public Button Attn { get { return buttons[0xf6]; } }
		/// <summary>The 'B' button</summary>
		[Mapping('b')]
		public Button B { get { return buttons[0x42]; } }
		/// <summary>The 'Back' button</summary>
		public Button Back { get { return buttons[8]; } }
		/// <summary>The 'BrowserBack' button</summary>
		public Button BrowserBack { get { return buttons[0xa6]; } }
		/// <summary>The 'BrowserFavorites' button</summary>
		public Button BrowserFavorites { get { return buttons[0xab]; } }
		/// <summary>The 'BrowserForward' button</summary>
		public Button BrowserForward { get { return buttons[0xa7]; } }
		/// <summary>The 'BrowserHome' button</summary>
		public Button BrowserHome { get { return buttons[0xac]; } }
		/// <summary>The 'BrowserRefresh' button</summary>
		public Button BrowserRefresh { get { return buttons[0xa8]; } }
		/// <summary>The 'BrowserSearch' button</summary>
		public Button BrowserSearch { get { return buttons[170]; } }
		/// <summary>The 'BrowserStop' button</summary>
		public Button BrowserStop { get { return buttons[0xa9]; } }
		/// <summary>The 'C' button</summary>
		[Mapping('c')]
		public Button C { get { return buttons[0x43]; } }
		/// <summary>The 'CapsLock' button</summary>
		public Button CapsLock { get { return buttons[20]; } }
		/// <summary>The 'ChatPadGreen' button</summary>
		public Button ChatPadGreen { get { return buttons[0xca]; } }
		/// <summary>The 'ChatPadOrange' button</summary>
		public Button ChatPadOrange { get { return buttons[0xcb]; } }
		/// <summary>The 'Crsel' button</summary>
		public Button Crsel { get { return buttons[0xf7]; } }
		/// <summary>The 'D' button</summary>
		[Mapping('d')]
		public Button D { get { return buttons[0x44]; } }
		/// <summary>The 'D0' button</summary>
		[Mapping('0')]
		public Button D0 { get { return buttons[0x30]; } }
		/// <summary>The 'D1' button</summary>
		[Mapping('1')]
		public Button D1 { get { return buttons[0x31]; } }
		/// <summary>The 'D2' button</summary>
		[Mapping('2')]
		public Button D2 { get { return buttons[50]; } }
		/// <summary>The 'D3' button</summary>
		[Mapping('3')]
		public Button D3 { get { return buttons[0x33]; } }
		/// <summary>The 'D4' button</summary>
		[Mapping('4')]
		public Button D4 { get { return buttons[0x34]; } }
		/// <summary>The 'D5' button</summary>
		[Mapping('5')]
		public Button D5 { get { return buttons[0x35]; } }
		/// <summary>The 'D6' button</summary>
		[Mapping('6')]
		public Button D6 { get { return buttons[0x36]; } }
		/// <summary>The 'D7' button</summary>
		[Mapping('7')]
		public Button D7 { get { return buttons[0x37]; } }
		/// <summary>The 'D8' button</summary>
		[Mapping('8')]
		public Button D8 { get { return buttons[0x38]; } }
		/// <summary>The 'D9' button</summary>
		[Mapping('9')]
		public Button D9 { get { return buttons[0x39]; } }
		/// <summary>The 'Decimal' button</summary>
		[Mapping('.')]
		public Button Decimal { get { return buttons[110]; } }
		/// <summary>The 'Delete' button</summary>
		public Button Delete { get { return buttons[0x2e]; } }
		/// <summary>The 'Divide' button</summary>
		[Mapping('/')]
		public Button Divide { get { return buttons[0x6f]; } }
		/// <summary>The 'Down' button</summary>
		public Button Down { get { return buttons[40]; } }
		/// <summary>The 'E' button</summary>
		[Mapping('e')]
		public Button E { get { return buttons[0x45]; } }
		/// <summary>The 'End' button</summary>
		public Button End { get { return buttons[0x23]; } }
		/// <summary>The 'Enter' button</summary>
		[Mapping('\n')]
		public Button Enter { get { return buttons[13]; } }
		/// <summary>The 'EraseEof' button</summary>
		public Button EraseEof { get { return buttons[0xf9]; } }
		/// <summary>The 'Escape' button</summary>
		public Button Escape { get { return buttons[0x1b]; } }
		/// <summary>The 'Execute' button</summary>
		public Button Execute { get { return buttons[0x2b]; } }
		/// <summary>The 'Exsel' button</summary>
		public Button Exsel { get { return buttons[0xf8]; } }
		/// <summary>The 'F' button</summary>
		[Mapping('f')]
		public Button F { get { return buttons[70]; } }
		/// <summary>The 'F1' button</summary>
		public Button F1 { get { return buttons[0x70]; } }
		/// <summary>The 'F10' button</summary>
		public Button F10 { get { return buttons[0x79]; } }
		/// <summary>The 'F11' button</summary>
		public Button F11 { get { return buttons[0x7a]; } }
		/// <summary>The 'F12' button</summary>
		public Button F12 { get { return buttons[0x7b]; } }
		/// <summary>The 'F13' button</summary>
		public Button F13 { get { return buttons[0x7c]; } }
		/// <summary>The 'F14' button</summary>
		public Button F14 { get { return buttons[0x7d]; } }
		/// <summary>The 'F15' button</summary>
		public Button F15 { get { return buttons[0x7e]; } }
		/// <summary>The 'F16' button</summary>
		public Button F16 { get { return buttons[0x7f]; } }
		/// <summary>The 'F17' button</summary>
		public Button F17 { get { return buttons[0x80]; } }
		/// <summary>The 'F18' button</summary>
		public Button F18 { get { return buttons[0x81]; } }
		/// <summary>The 'F19' button</summary>
		public Button F19 { get { return buttons[130]; } }
		/// <summary>The 'F2' button</summary>
		public Button F2 { get { return buttons[0x71]; } }
		/// <summary>The 'F20' button</summary>
		public Button F20 { get { return buttons[0x83]; } }
		/// <summary>The 'F21' button</summary>
		public Button F21 { get { return buttons[0x84]; } }
		/// <summary>The 'F22' button</summary>
		public Button F22 { get { return buttons[0x85]; } }
		/// <summary>The 'F23' button</summary>
		public Button F23 { get { return buttons[0x86]; } }
		/// <summary>The 'F24' button</summary>
		public Button F24 { get { return buttons[0x87]; } }
		/// <summary>The 'F3' button</summary>
		public Button F3 { get { return buttons[0x72]; } }
		/// <summary>The 'F4' button</summary>
		public Button F4 { get { return buttons[0x73]; } }
		/// <summary>The 'F5' button</summary>
		public Button F5 { get { return buttons[0x74]; } }
		/// <summary>The 'F6' button</summary>
		public Button F6 { get { return buttons[0x75]; } }
		/// <summary>The 'F7' button</summary>
		public Button F7 { get { return buttons[0x76]; } }
		/// <summary>The 'F8' button</summary>
		public Button F8 { get { return buttons[0x77]; } }
		/// <summary>The 'F9' button</summary>
		public Button F9 { get { return buttons[120]; } }
		/// <summary>The 'G' button</summary>
		[Mapping('g')]
		public Button G { get { return buttons[0x47]; } }
		/// <summary>The 'H' button</summary>
		[Mapping('h')]
		public Button H { get { return buttons[0x48]; } }
		/// <summary>The 'Help' button</summary>
		public Button Help { get { return buttons[0x2f]; } }
		/// <summary>The 'Home' button</summary>
		public Button Home { get { return buttons[0x24]; } }
		/// <summary>The 'I' button</summary>
		[Mapping('i')]
		public Button I { get { return buttons[0x49]; } }
		/// <summary>The 'Insert' button</summary>
		public Button Insert { get { return buttons[0x2d]; } }
		/// <summary>The 'J' button</summary>
		[Mapping('j')]
		public Button J { get { return buttons[0x4a]; } }
		/// <summary>The 'K' button</summary>
		[Mapping('k')]
		public Button K { get { return buttons[0x4b]; } }
		/// <summary>The 'L' button</summary>
		[Mapping('l')]
		public Button L { get { return buttons[0x4c]; } }
		/// <summary>The 'LaunchApplication1' button</summary>
		public Button LaunchApplication1 { get { return buttons[0xb6]; } }
		/// <summary>The 'LaunchApplication2' button</summary>
		public Button LaunchApplication2 { get { return buttons[0xb7]; } }
		/// <summary>The 'LaunchMail' button</summary>
		public Button LaunchMail { get { return buttons[180]; } }
		/// <summary>The 'Left' button</summary>
		public Button Left { get { return buttons[0x25]; } }
		/// <summary>The 'LeftAlt' button</summary>
		public Button LeftAlt { get { return buttons[0xa4]; } }
		/// <summary>The 'LeftControl' button</summary>
		public Button LeftControl { get { return buttons[0xa2]; } }
		/// <summary>The 'LeftShift' button</summary>
		public Button LeftShift { get { return buttons[160]; } }
		/// <summary>The 'LeftWindows' button</summary>
		public Button LeftWindows { get { return buttons[0x5b]; } }
		/// <summary>The 'M' button</summary>
		[Mapping('m')]
		public Button M { get { return buttons[0x4d]; } }
		/// <summary>The 'MediaNextTrack' button</summary>
		public Button MediaNextTrack { get { return buttons[0xb0]; } }
		/// <summary>The 'MediaPlayPause' button</summary>
		public Button MediaPlayPause { get { return buttons[0xb3]; } }
		/// <summary>The 'MediaPreviousTrack' button</summary>
		public Button MediaPreviousTrack { get { return buttons[0xb1]; } }
		/// <summary>The 'MediaStop' button</summary>
		public Button MediaStop { get { return buttons[0xb2]; } }
		/// <summary>The 'Multiply' button</summary>
		public Button Multiply { get { return buttons[0x6a]; } }
		/// <summary>The 'N' button</summary>
		[Mapping('n')]
		public Button N { get { return buttons[0x4e]; } }
		/// <summary>The 'NumLock' button</summary>
		public Button NumLock { get { return buttons[0x90]; } }
		/// <summary>The 'NumPad0' button</summary>
		[Mapping('0')]
		public Button NumPad0 { get { return buttons[0x60]; } }
		/// <summary>The 'NumPad1' button</summary>
		[Mapping('1')]
		public Button NumPad1 { get { return buttons[0x61]; } }
		/// <summary>The 'NumPad2' button</summary>
		[Mapping('2')]
		public Button NumPad2 { get { return buttons[0x62]; } }
		/// <summary>The 'NumPad3' button</summary>
		[Mapping('3')]
		public Button NumPad3 { get { return buttons[0x63]; } }
		/// <summary>The 'NumPad4' button</summary>
		[Mapping('4')]
		public Button NumPad4 { get { return buttons[100]; } }
		/// <summary>The 'NumPad5' button</summary>
		[Mapping('5')]
		public Button NumPad5 { get { return buttons[0x65]; } }
		/// <summary>The 'NumPad6' button</summary>
		[Mapping('6')]
		public Button NumPad6 { get { return buttons[0x66]; } }
		/// <summary>The 'NumPad7' button</summary>
		[Mapping('7')]
		public Button NumPad7 { get { return buttons[0x67]; } }
		/// <summary>The 'NumPad8' button</summary>
		[Mapping('8')]
		public Button NumPad8 { get { return buttons[0x68]; } }
		/// <summary>The 'NumPad9' button</summary>
		[Mapping('9')]
		public Button NumPad9 { get { return buttons[0x69]; } }
		/// <summary>The 'O' button</summary>
		[Mapping('o')]
		public Button O { get { return buttons[0x4f]; } }
		/// <summary>The 'Oem8' button</summary>
		public Button Oem8 { get { return buttons[0xdf]; } }
		/// <summary>The 'OemBackslash' button</summary>
		[Mapping('/')]
		public Button OemBackslash { get { return buttons[0xe2]; } }
		/// <summary>The 'OemClear' button</summary>
		public Button OemClear { get { return buttons[0xfe]; } }
		/// <summary>The 'OemCloseBrackets' button</summary>
		[Mapping(']')]
		public Button OemCloseBrackets { get { return buttons[0xdd]; } }
		/// <summary>The 'OemComma' button</summary>
		[Mapping(',')]
		public Button OemComma { get { return buttons[0xbc]; } }
		/// <summary>The 'OemMinus' button</summary>
		[Mapping('-')]
		public Button OemMinus { get { return buttons[0xbd]; } }
		/// <summary>The 'OemOpenBrackets' button</summary>
		[Mapping('[')]
		public Button OemOpenBrackets { get { return buttons[0xdb]; } }
		/// <summary>The 'OemPeriod' button</summary>
		[Mapping('.')]
		public Button OemPeriod { get { return buttons[190]; } }
		/// <summary>The 'OemPipe' button</summary>
		[Mapping('|')]
		public Button OemPipe { get { return buttons[220]; } }
		/// <summary>The 'OemPlus' button</summary>
		[Mapping('+')]
		public Button OemPlus { get { return buttons[0xbb]; } }
		/// <summary>The 'OemQuestion' button</summary>
		[Mapping('?')]
		public Button OemQuestion { get { return buttons[0xbf]; } }
		/// <summary>The 'OemQuotes' button</summary>
		[Mapping('\'')]
		public Button OemQuotes { get { return buttons[0xde]; } }
		/// <summary>The 'OemSemicolon' button</summary>
		[Mapping(';')]
		public Button OemSemicolon { get { return buttons[0xba]; } }
		/// <summary>The 'OemTilde' button</summary>
		[Mapping('~')]
		public Button OemTilde { get { return buttons[0xc0]; } }
		/// <summary>The 'P' button</summary>
		[Mapping('p')]
		public Button P { get { return buttons[80]; } }
		/// <summary>The 'Pa1' button</summary>
		public Button Pa1 { get { return buttons[0xfd]; } }
		/// <summary>The 'PageDown' button</summary>
		public Button PageDown { get { return buttons[0x22]; } }
		/// <summary>The 'PageUp' button</summary>
		public Button PageUp { get { return buttons[0x21]; } }
		/// <summary>The 'Pause' button</summary>
		public Button Pause { get { return buttons[0x13]; } }
		/// <summary>The 'Play' button</summary>
		public Button Play { get { return buttons[250]; } }
		/// <summary>The 'Print' button</summary>
		public Button Print { get { return buttons[0x2a]; } }
		/// <summary>The 'PrintScreen' button</summary>
		public Button PrintScreen { get { return buttons[0x2c]; } }
		/// <summary>The 'ProcessKey' button</summary>
		public Button ProcessKey { get { return buttons[0xe5]; } }
		/// <summary>The 'Q' button</summary>
		[Mapping('q')]
		public Button Q { get { return buttons[0x51]; } }
		/// <summary>The 'R' button</summary>
		[Mapping('r')]
		public Button R { get { return buttons[0x52]; } }
		/// <summary>The 'Right' button</summary>
		public Button Right { get { return buttons[0x27]; } }
		/// <summary>The 'RightAlt' button</summary>
		public Button RightAlt { get { return buttons[0xa5]; } }
		/// <summary>The 'RightControl' button</summary>
		public Button RightControl { get { return buttons[0xa3]; } }
		/// <summary>The 'RightShift' button</summary>
		public Button RightShift { get { return buttons[0xa1]; } }
		/// <summary>The 'RightWindows' button</summary>
		public Button RightWindows { get { return buttons[0x5c]; } }
		/// <summary>The 'S' button</summary>
		[Mapping('s')]
		public Button S { get { return buttons[0x53]; } }
		/// <summary>The 'Scroll' button</summary>
		public Button Scroll { get { return buttons[0x91]; } }
		/// <summary>The 'Select' button</summary>
		public Button Select { get { return buttons[0x29]; } }
		/// <summary>The 'SelectMedia' button</summary>
		public Button SelectMedia { get { return buttons[0xb5]; } }
		/// <summary>The 'Separator' button</summary>
		public Button Separator { get { return buttons[0x6c]; } }
		/// <summary>The 'Sleep' button</summary>
		public Button Sleep { get { return buttons[0x5f]; } }
		/// <summary>The 'Space' button</summary>
		[Mapping(' ')]
		public Button Space { get { return buttons[0x20]; } }
		/// <summary>The 'Subtract' button</summary>
		[Mapping('-')]
		public Button Subtract { get { return buttons[0x6d]; } }
		/// <summary>The 'T' button</summary>
		[Mapping('t')]
		public Button T { get { return buttons[0x54]; } }
		/// <summary>The 'Tab' button</summary>
		[Mapping('\t')]
		public Button Tab { get { return buttons[9]; } }
		/// <summary>The 'U' button</summary>
		[Mapping('u')]
		public Button U { get { return buttons[0x55]; } }
		/// <summary>The 'Up' button</summary>
		public Button Up { get { return buttons[0x26]; } }
		/// <summary>The 'V' button</summary>
		[Mapping('v')]
		public Button V { get { return buttons[0x56]; } }
		/// <summary>The 'VolumeDown' button</summary>
		public Button VolumeDown { get { return buttons[0xae]; } }
		/// <summary>The 'VolumeMute' button</summary>
		public Button VolumeMute { get { return buttons[0xad]; } }
		/// <summary>The 'VolumeUp' button</summary>
		public Button VolumeUp { get { return buttons[0xaf]; } }
		/// <summary>The 'W' button</summary>
		[Mapping('w')]
		public Button W { get { return buttons[0x57]; } }
		/// <summary>The 'X' button</summary>
		[Mapping('x')]
		public Button X { get { return buttons[0x58]; } }
		/// <summary>The 'Y' button</summary>
		[Mapping('y')]
		public Button Y { get { return buttons[0x59]; } }
		/// <summary>The 'Z' button</summary>
		[Mapping('z')]
		public Button Z { get { return buttons[90]; } }
		/// <summary>The 'Zoom' button</summary>
		public Button Zoom { get { return buttons[0xfb]; } }
	}

	#endregion

	//for efficiency reasons, extract the private members from the KeyboardState structure
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
	struct KeyMap
	{
		[System.Runtime.InteropServices.FieldOffset(0)]
		public KeyboardState state;

		[System.Runtime.InteropServices.FieldOffset(0)]
		public uint currentState0;
		[System.Runtime.InteropServices.FieldOffset(4)]
		public uint currentState1;
		[System.Runtime.InteropServices.FieldOffset(8)]
		public uint currentState2;
		[System.Runtime.InteropServices.FieldOffset(12)]
		public uint currentState3;
		[System.Runtime.InteropServices.FieldOffset(16)]
		public uint currentState4;
		[System.Runtime.InteropServices.FieldOffset(20)]
		public uint currentState5;
		[System.Runtime.InteropServices.FieldOffset(24)]
		public uint currentState6;
		[System.Runtime.InteropServices.FieldOffset(28)]
		public uint currentState7;

	}

	/// <summary>
	/// Stores a list of <see cref="Button"/> objects representing keyboard keys
	/// </summary>
	public sealed class KeyboardInputState
	{
		private static readonly Keys[] keyIndices = new Keys[256];
		private static readonly char?[] keyMapping = new char?[256];
		internal readonly Button[] buttons = new Button[256];
		private long initialTick = -1;
		private readonly KeyState state;
		private KeyMap currentFrame,previousFrame;

		static KeyboardInputState()
		{
			System.Reflection.FieldInfo[] enums = typeof(Keys).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			List<Keys> keys = new List<Keys>();

			Type keyStateType = typeof(KeyState);

			foreach (System.Reflection.FieldInfo field in enums)
			{
				Keys key = (Keys)field.GetValue(null);
				keys.Add(key);
				System.Reflection.PropertyInfo prop = keyStateType.GetProperty(key.ToString(), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
				if (prop == null) continue;
				foreach (object att in prop.GetCustomAttributes(true))
				{
					if (att is MappingAttribute)
						keyMapping[(int)key] = ((MappingAttribute)att).mapping;
				}
			}

			keyIndices = keys.ToArray();
		}

		internal KeyboardInputState()
		{
			this.state = new KeyState(buttons);
		}
		
		/// <summary>
		/// Allocates an array of all values in the <see cref="Keys"/> enumerator
		/// </summary>
		/// <returns></returns>
		public static Keys[] GetKeyArray()
		{
			return (Keys[])keyIndices.Clone();
		}

		/// <summary>
		/// <para>Gets the character for a given <see cref="Keys"/> key. Eg: <see cref="Keys.A"/> will output 'a'.</para>
		/// <para>Returns false if the key character is unknown</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="keyChar"></param>
		/// <returns></returns>
		public bool TryGetKeyChar(Keys key, out char keyChar)
		{
			keyChar = default(char);
			int index = (int)key;
			if (keyMapping[index].HasValue)
			{
				keyChar = keyMapping[index].Value;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the current KeyState
		/// </summary>
		public KeyState KeyState
		{
			get { return this.state; }
		}

		/// <summary></summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static implicit operator KeyboardState(KeyboardInputState input)
		{
			return input.currentFrame.state;
		}

		/// <summary>
		/// Button Indexer (Keys)
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Button this[Keys key]
		{
			get { return this.buttons[(int)key]; }
		}

		/// <summary>
		/// Returns true if a key is down
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsKeyDown(Keys key)
		{
			return this.buttons[(int)key];
		}
		/// <summary>
		/// Returns true if a key is up
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsKeyUp(Keys key)
		{
			return !this.buttons[(int)key];
		}

		/// <summary>
		/// Gets the <see cref="Button"/> state of a key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Button GetKey(Keys key)
		{
			return this.buttons[(int)key];
		}

		internal void Update(long tick, ref KeyboardState keyboardState)
		{
			if (initialTick == -1)
			{
				initialTick = tick;
				currentFrame.state = keyboardState;
			}

			tick -= initialTick;

			previousFrame.state = currentFrame.state;
			currentFrame.state = keyboardState;
			
			for (int i = 0; i < 32; i++)
			{
				this.buttons[32 * 0 + i].SetState((currentFrame.currentState0 & ((uint)1) << i) != 0, tick);
				this.buttons[32 * 1 + i].SetState((currentFrame.currentState1 & ((uint)1) << i) != 0, tick);
				this.buttons[32 * 2 + i].SetState((currentFrame.currentState2 & ((uint)1) << i) != 0, tick);
				this.buttons[32 * 3 + i].SetState((currentFrame.currentState3 & ((uint)1) << i) != 0, tick);
				this.buttons[32 * 4 + i].SetState((currentFrame.currentState4 & ((uint)1) << i) != 0, tick);
				this.buttons[32 * 5 + i].SetState((currentFrame.currentState5 & ((uint)1) << i) != 0, tick);
				this.buttons[32 * 6 + i].SetState((currentFrame.currentState6 & ((uint)1) << i) != 0, tick);
				this.buttons[32 * 7 + i].SetState((currentFrame.currentState7 & ((uint)1) << i) != 0, tick);
			}
		}

		/// <summary>
		/// Calls the <paramref name="callback"/> for each <see cref="Keys"/> key where <see cref="Button.OnPressed"/> is true
		/// </summary>
		/// <param name="callback"></param>
		public void GetPressedKeys(Action<Keys> callback)
		{
			if (callback == null)
				throw new ArgumentNullException();

			if (currentFrame.currentState0 != previousFrame.currentState0) PressCallback(callback, 0);
			if (currentFrame.currentState1 != previousFrame.currentState1) PressCallback(callback, 1);
			if (currentFrame.currentState2 != previousFrame.currentState2) PressCallback(callback, 2);
			if (currentFrame.currentState3 != previousFrame.currentState3) PressCallback(callback, 3);
			if (currentFrame.currentState4 != previousFrame.currentState4) PressCallback(callback, 4);
			if (currentFrame.currentState5 != previousFrame.currentState5) PressCallback(callback, 5);
			if (currentFrame.currentState6 != previousFrame.currentState6) PressCallback(callback, 6);
			if (currentFrame.currentState7 != previousFrame.currentState7) PressCallback(callback, 7);
		}

		/// <summary>
		/// Calls the <paramref name="callback"/> for each <see cref="Keys"/> key where <see cref="Button.IsDown"/> is true
		/// </summary>
		/// <param name="callback"></param>
		public void GetHeldKeys(Action<Keys> callback)
		{
			if (callback == null)
				throw new ArgumentNullException();

			if (currentFrame.currentState0 != 0) HeldCallback(callback, 0);
			if (currentFrame.currentState1 != 0) HeldCallback(callback, 1);
			if (currentFrame.currentState2 != 0) HeldCallback(callback, 2);
			if (currentFrame.currentState3 != 0) HeldCallback(callback, 3);
			if (currentFrame.currentState4 != 0) HeldCallback(callback, 4);
			if (currentFrame.currentState5 != 0) HeldCallback(callback, 5);
			if (currentFrame.currentState6 != 0) HeldCallback(callback, 6);
			if (currentFrame.currentState7 != 0) HeldCallback(callback, 7);
		}

		void PressCallback(Action<Keys> callback, int group)
		{
			for (int i = 0; i < 32; i++)
				if (buttons[32 * group + i].OnPressed)
					callback((Keys)(32 * group + i));
		}
		void HeldCallback(Action<Keys> callback, int group)
		{
			for (int i = 0; i < 32; i++)
				if (buttons[32 * group + i].IsDown)
					callback((Keys)(32 * group + i));
		}
	}

#if !XBOX360

	/// <summary>
	/// [Windows Only] Stores <see cref="Button"/> objects representing mouse buttons
	/// </summary>
	public sealed class MouseInputState
	{
		private MouseState state;
		private Button left, right, middle, x1, x2;
		private int x, y, scroll, scrollDelta;

		/// <summary>
		/// Horizontal position of the mouse cursor
		/// </summary>
		public int X { get { return x; } }
		/// <summary>
		/// Vertical position of the mouse cursor
		/// </summary>
		public int Y { get { return y; } }
		/// <summary>
		/// <see cref="Button"/> state of the left mouse button
		/// </summary>
		public Button LeftButton { get { return left; } }
		/// <summary>
		/// <see cref="Button"/> state of the right mouse button
		/// </summary>
		public Button RightButton { get { return right; } }
		/// <summary>
		/// <see cref="Button"/> state of the middle mouse button
		/// </summary>
		public Button MiddleButton { get { return middle; } }
		/// <summary>
		/// <see cref="Button"/> state of the first X mouse button
		/// </summary>
		public Button XButton1 { get { return x2; } }
		/// <summary>
		/// <see cref="Button"/> state of the second X mouse button
		/// </summary>
		public Button XButton2 { get { return x1; } }
		/// <summary>
		/// Gets the total mouse scroll movement
		/// </summary>
		public int ScrollWheelValue { get { return scroll; } }
		/// <summary>
		/// Gets the delta mouse scroll movement
		/// </summary>
		public int ScrollWheelDelta { get { return scrollDelta; } }
		
		internal void Update(long tick, ref MouseState mouseState)
		{
			state = mouseState;

			this.x = state.X;
			this.y = state.Y;
			this.scrollDelta = state.ScrollWheelValue - this.scroll;
			this.scroll = state.ScrollWheelValue;

			this.left.SetState(state.LeftButton == ButtonState.Pressed, tick);
			this.right.SetState(state.RightButton == ButtonState.Pressed, tick);
			this.middle.SetState(state.MiddleButton== ButtonState.Pressed, tick);
			this.x1.SetState(state.XButton1 == ButtonState.Pressed, tick);
			this.x2.SetState(state.XButton2 == ButtonState.Pressed, tick);
		}

		/// <summary></summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static implicit operator MouseState(MouseInputState input)
		{
			return input.state;
		}
	}

#endif
}


