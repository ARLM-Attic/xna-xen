using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Xen
{
#if !XBOX360

	/// <summary>
	/// <para>A Windows Forms control that can host an <see cref="Application"/>. Add this Control to your Windows Forms project using the Windows Forms Designer</para>
	/// <para>Use <see cref="Application.Run(WinFormsHostControl)"/> to attach a xen application to this control</para>
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public class WinFormsHostControl : System.Windows.Forms.Control
	{
		WinFormsHostGraphicsDeviceService device;
		Application application;
		XNAWinFormsHostAppWrapper winFormWrapper;

		internal void SetApplication(Application app, XNAWinFormsHostAppWrapper formsWrapper, WinFormsHostGraphicsDeviceService device)
		{
			this.device = device;
			this.application = app;
			this.winFormWrapper = formsWrapper;
			this.winFormWrapper.Exiting += new EventHandler(winFormWrapper_Exiting);
		}

		void winFormWrapper_Exiting(object sender, EventArgs e)
		{
			if (this.winFormWrapper != null)
				this.winFormWrapper.Exiting -= new EventHandler(winFormWrapper_Exiting);
			this.device = null;
			this.application = null;
			this.winFormWrapper = null;
		}

		/// <summary>
		/// Disposes the control.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (application != null)
				application.Shutdown();

			application = null;
			device = null;
			winFormWrapper = null;

			base.Dispose(disposing);
		}

		/// <summary></summary>
		/// <param name="e"></param>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			if (device != null && !DesignMode && HandleDeviceReset())
			{
				// Draw the control using the GraphicsDevice.
				winFormWrapper.RunMainLoop();

				Rectangle sourceRectangle = new Rectangle(0, 0, ClientSize.Width,
															ClientSize.Height);
				try
				{
					if (device != null)
						device.GraphicsDevice.Present(sourceRectangle, null, this.Handle);
				}
				catch (InvalidOperationException)
				{
				}
				catch (DeviceLostException)
				{
				}
				catch (DeviceNotResetException)
				{
				}
				catch (DriverInternalErrorException)
				{
				}

				Invalidate();
			}
			else
			{
				// If BeginDraw failed, show an error message using System.Drawing.
				PaintUsingSystemDrawing(e.Graphics);
			}
		}


		bool HandleDeviceReset()
		{
			bool deviceNeedsReset = false;

			switch (device.GraphicsDevice.GraphicsDeviceStatus)
			{
				case GraphicsDeviceStatus.Lost:
					// If the graphics device is lost, we cannot use it at all.
					return false;

				case GraphicsDeviceStatus.NotReset:
					// If device is in the not-reset state, we should try to reset it.
					deviceNeedsReset = true;
					break;

				default:
					// If the device state is ok, check whether it is big enough.
					PresentationParameters pp = device.GraphicsDevice.PresentationParameters;

					deviceNeedsReset = (ClientSize.Width != pp.BackBufferWidth) ||
									   (ClientSize.Height != pp.BackBufferHeight);
					break;
			}

			// Do we need to reset the device?
			if (deviceNeedsReset)
			{
				device.ResetDevice(Math.Max(1, ClientSize.Width), Math.Max(1, ClientSize.Height));
			}
			return true;
		}


		/// <summary></summary>
		/// <param name="graphics"></param>
		protected virtual void PaintUsingSystemDrawing(System.Drawing.Graphics graphics)
		{
			graphics.Clear(System.Drawing.Color.CornflowerBlue);

			if (DesignMode)
			{
				using (System.Drawing.Brush brush = new System.Drawing.SolidBrush(System.Drawing.Color.WhiteSmoke))
				{
					using (System.Drawing.StringFormat format = new System.Drawing.StringFormat())
					{
						format.Alignment = System.Drawing.StringAlignment.Center;
						format.LineAlignment = System.Drawing.StringAlignment.Center;

						graphics.DrawString("Xen WinForms Host Control", Font, brush, ClientRectangle, format);
					}
				}
			}
		}


		/// <summary></summary>
		/// <param name="pevent"></param>
		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
		{
		}
	}


	//this class is used for WinForms support, and is a modification
	//of the official winforms sample
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	sealed class WinFormsHostGraphicsDeviceService : IGraphicsDeviceService, IGraphicsDeviceManager, IDisposable
	{
		XNAWinFormsHostAppWrapper winFormsHost;

		public WinFormsHostGraphicsDeviceService(XNAWinFormsHostAppWrapper winForms, int width, int height)
		{
			this.winFormsHost = winForms;

			parameters = new PresentationParameters();

			parameters.BackBufferWidth = Math.Max(width, 1);
			parameters.BackBufferHeight = Math.Max(height, 1);
			parameters.BackBufferFormat = SurfaceFormat.Color;

			parameters.EnableAutoDepthStencil = true;
			parameters.AutoDepthStencilFormat = DepthFormat.Depth24;
		}

		public void CreateDevice(RenderTargetUsage usage, WinFormsHostControl host)
		{
			parameters.RenderTargetUsage = usage;

			graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter,
												DeviceType.Hardware,
												host.Handle,
												parameters);

			graphicsDevice.DeviceReset += delegate { this.DeviceReset(this, EventArgs.Empty); };
			graphicsDevice.Disposing += delegate { this.DeviceDisposing(this, EventArgs.Empty); };
			graphicsDevice.DeviceResetting += delegate { this.DeviceResetting(this, EventArgs.Empty); };

			if (DeviceCreated != null)
				DeviceCreated(this, EventArgs.Empty);
		}

		public void ResetDevice(int width, int height)
		{
			if (DeviceResetting != null)
				DeviceResetting(this, EventArgs.Empty);

			parameters.BackBufferWidth = Math.Max(1, width);
			parameters.BackBufferHeight = Math.Max(1, height);

			graphicsDevice.Reset(parameters);

			winFormsHost.AdjustWindowSize(width, height);

			if (DeviceReset != null)
				DeviceReset(this, EventArgs.Empty);
		}

		public GraphicsDevice GraphicsDevice
		{
			get { return graphicsDevice; }
		}

		GraphicsDevice graphicsDevice;
		PresentationParameters parameters;

		public event EventHandler DeviceCreated;
		public event EventHandler DeviceDisposing;
		public event EventHandler DeviceReset;
		public event EventHandler DeviceResetting;

		public void Dispose()
		{
			graphicsDevice.Dispose();
			graphicsDevice = null;
		}

		bool IGraphicsDeviceManager.BeginDraw()
		{
			return graphicsDevice != null;
		}

		void IGraphicsDeviceManager.CreateDevice()
		{
		}

		void IGraphicsDeviceManager.EndDraw()
		{
		}
	}

#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	sealed class XNAWinFormsHostAppWrapper : IXNAAppWrapper
	{
		WinFormsHostGraphicsDeviceService formsDeviceService;
		RenderTargetUsage presentation;
		GameServiceContainer services;
		XNALogic logic;
		ContentManager content;
		WinFormsHostControl control;
		System.Windows.Forms.Form parentForm;
		Application parent;

		internal XNAWinFormsHostAppWrapper(XNALogic logic, Application parent, WinFormsHostControl host)
		{
			this.parent = parent;
			this.logic = logic;

			this.control = host;

			System.Windows.Forms.Control parentControl = this.control;
			while (parentControl != null)
			{
				if (parentControl is System.Windows.Forms.Form)
				{
					this.parentForm = (System.Windows.Forms.Form)parentControl;
					break;
				}
				parentControl = parentControl.Parent;
			}
			if (parentForm == null)
				throw new ArgumentException("Unable to find Parent Form for display handle");

			parentForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(parentForm_FormClosed);

			formsDeviceService = new WinFormsHostGraphicsDeviceService(this, this.control.ClientSize.Width, this.control.ClientSize.Width);

			services = new GameServiceContainer();
			services.AddService(typeof(IGraphicsDeviceService), formsDeviceService);
			services.AddService(typeof(IGraphicsDeviceManager), formsDeviceService);

			presentation = RenderTargetUsage.PlatformContents;
			content = new ContentManager(services);

			int width = 0;
			int height = 0;
			SurfaceFormat format = SurfaceFormat.Color;

			width = control.ClientSize.Width;
			height = control.ClientSize.Height;

			parent.SetWindowSizeAndFormat(width, height, format);

			parent.SetupGraphicsDeviceManager(null, ref presentation);

			formsDeviceService.CreateDevice(presentation, host);

			host.SetApplication(parent, this, formsDeviceService);

			host.BeginInvoke((EventHandler)delegate
			{
				logic.Initialize();
				logic.LoadContent();
			});
		}

		void parentForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
		{
			if (Exiting != null)
				Exiting(this, EventArgs.Empty);
		}

		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return formsDeviceService.GraphicsDevice;
			}
		}

		public bool VSyncEnabled
		{
			get
			{
				return true;
			}
		}


		public GameServiceContainer Services
		{
			get { return services; }
		}

		public ContentManager Content
		{
			get { return content; }
		}

		public GameComponentCollection Components
		{
			get { return null; }
		}

		public bool IsActive
		{
			get { return true; }
		}

		public event EventHandler Exiting;

		public void Run()
		{
		}

		public void Exit()
		{
		}

		public GameWindow Window
		{
			get { return null; }
		}
		public IntPtr WindowHandle
		{
			get { return this.parentForm.Handle; }
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (Exiting != null)
				Exiting(this, EventArgs.Empty);
		}

		#endregion


		long initalTick;

		internal void RunMainLoop()
		{
			long tick = DateTime.Now.Ticks;
			if (initalTick == 0)
				initalTick = tick;
			logic.Update(tick - initalTick, tick - initalTick);
			logic.Draw();
		}

		internal void AdjustWindowSize(int width, int height)
		{
			parent.SetWindowSizeAndFormat(width, height, SurfaceFormat.Color);
		}
	}

#endif
}
