using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Xen.Camera;
using Xen.Graphics.Modifier;
using Microsoft.Xna.Framework;

namespace Xen.Graphics
{
	/// <summary>
	/// Abstract base class for a Draw object that renders a list of drawable items to the screen or a render target
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public abstract class DrawTarget : Resource, IDraw
	{
		private ICamera camera;
		private bool enabled;
		private bool rendering;
		private List<IDraw> drawList = new List<IDraw>();
		private List<IBeginEndDraw> modifiers, activeModifiers;
		internal static int baseSizeIndex = 1;
		private ClearBufferModifier bufferClear = new ClearBufferModifier(true);

		internal void CloneTo(DrawTarget clone, bool cloneModifiers, bool cloneDrawList)
		{
			clone.activeModifiers = new List<IBeginEndDraw>();
			clone.camera = this.camera;
			clone.drawList = cloneDrawList ? new List<IDraw>(this.drawList) : new List<IDraw>();
			if (this.modifiers != null)
			{
				clone.modifiers = cloneModifiers ? new List<IBeginEndDraw>(this.modifiers) : null;
				if (clone.modifiers != null)
					clone.activeModifiers = new List<IBeginEndDraw>(clone.modifiers.Capacity);
			}
			clone.rendering = false;
			clone.enabled = true;

			clone.bufferClear = new ClearBufferModifier(true);
			clone.bufferClear.ClearColour = this.bufferClear.ClearColour;
			clone.bufferClear.ClearColourEnabled = this.bufferClear.ClearColourEnabled;
			clone.bufferClear.ClearDepth = this.bufferClear.ClearDepth;
			clone.bufferClear.ClearDepthEnabled = this.bufferClear.ClearDepthEnabled;
			clone.bufferClear.ClearStencilEnabled = this.bufferClear.ClearStencilEnabled;
			clone.bufferClear.ClearStencilValue = this.bufferClear.ClearStencilValue;
			clone.bufferClear.Enabled = this.bufferClear.Enabled;
		}

		internal DrawTarget(ICamera camera)
		{
			if (camera == null)
				throw new ArgumentNullException("camera");
			this.camera = camera;
			this.enabled = true;
		}

		internal DrawTarget(ICamera camera, bool enabled)
		{
			if (camera == null)
				throw new ArgumentNullException("camera");
			this.camera = camera;
			this.enabled = enabled;
		}

		/// <summary>
		/// Gets/Changes the clear operations performed when the draw target is drawn
		/// </summary>
		public ClearBufferModifier ClearBuffer
		{
			get { return bufferClear; }
		}

		/// <summary>
		/// Gets the surface format of the colour buffer for this draw target
		/// </summary>
		public abstract SurfaceFormat SurfaceFormat { get; }
		/// <summary>
		/// Gets the multisample level of the draw target
		/// </summary>
		public abstract MultiSampleType MultiSampleType { get; }
		/// <summary>
		/// Gets/Sets the camera used by this draw target
		/// </summary>
		public ICamera Camera
		{
			get { return camera; }
			set 
			{
				if (value != camera)
				{
					if (value == null)
						throw new ArgumentNullException();
					if (rendering)
						throw new InvalidOperationException("DrawTarget is in use");
					camera = value;
				}
			}
		}

		/// <summary>
		/// Adds a drawable item to the list of items to be drawn to the draw target
		/// </summary>
		/// <param name="drawable"></param>
		public void Add(IDraw drawable)
		{
#if DEBUG
			if (drawable is DrawTarget)
				throw new ArgumentException("Cannot nest draw targets");
#endif
			drawList.Add(drawable);
		}

		/// <summary>
		/// Adds a drawable item into the list of items to be drawn to the draw target
		/// </summary>
		/// <param name="index"></param>
		/// <param name="drawable"></param>
		public void Insert(int index, IDraw drawable)
		{
#if DEBUG
			if (drawable is DrawTarget)
				throw new ArgumentException("Cannot nest draw targets");
#endif
			drawList.Insert(index, drawable);
		}

		/// <summary>
		/// Removes a drawable item from the list of items to be drawn to the draw target
		/// </summary>
		/// <param name="drawable"></param>
		/// <returns>true if the item was removed</returns>
		public bool Remove(IDraw drawable)
		{
			return drawList.Remove(drawable);
		}

		/// <summary>
		/// Adds a begin/end drawing modified (such as a viewport modified) to the list of modifiers to be used while the draw target is being drawn
		/// </summary>
		/// <param name="modifier"></param>
		public void AddModifier(IBeginEndDraw modifier)
		{
			if (modifiers == null)
			{
				modifiers = new List<IBeginEndDraw>();
				activeModifiers = new List<IBeginEndDraw>();
			}
			modifiers.Add(modifier);
		}

		/// <summary>
		/// Adds a begin/end drawing modified (such as a clear buffer modified) into the list of modifiers to be used while the draw target is being drawn
		/// </summary>
		/// <param name="modifier"></param>
		/// <param name="index"></param>
		public void InsertModifier(int index, IBeginEndDraw modifier)
		{
			if (modifiers == null)
			{
				modifiers = new List<IBeginEndDraw>();
				activeModifiers = new List<IBeginEndDraw>();
			}
			modifiers.Insert(index,modifier);
		}

		/// <summary>
		/// Removes a begin/end drawing modified (such as a clear buffer modified) from the list of modifiers that is used while the draw target is being drawn
		/// </summary>
		/// <param name="modifier"></param>
		public bool RemoveModifier(IBeginEndDraw modifier)
		{
			if (modifiers != null)
				return modifiers.Remove(modifier);
			return false;
		}

		/// <summary>
		/// Gets/Sets if this draw target is enabled (no drawing will occur when disabled)
		/// </summary>
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}
		/// <summary></summary>
		/// <returns></returns>
		protected virtual bool GetEnabled()
		{
			return enabled;
		}

		bool ICullable.CullTest(ICuller culler) 
		{
			return enabled; 
		}

		/// <summary>
		/// Gets the byte size of a render target format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static int FormatSize(SurfaceFormat format)
		{
			switch (format)
			{
				case SurfaceFormat.Color:
				case SurfaceFormat.Bgr32:
				case SurfaceFormat.Rgb32:
				case SurfaceFormat.Rgba1010102:
				case SurfaceFormat.Rg32:
				case SurfaceFormat.HalfVector2:
				case SurfaceFormat.Bgra1010102:
				case SurfaceFormat.Single:
					return 4;
				case SurfaceFormat.Bgr565:
				case SurfaceFormat.Bgr555:
				case SurfaceFormat.Bgra5551:
				case SurfaceFormat.Bgra4444:
				case SurfaceFormat.HalfSingle:
				case SurfaceFormat.Luminance16:
					return 2;
				case SurfaceFormat.Rgba64:
				case SurfaceFormat.HalfVector4:
				case SurfaceFormat.Vector2:
					return 8;
				case SurfaceFormat.Vector4:
					return 16;
				default:
					//throw new NotImplementedException(format.ToString());
					return 0;
			}
		}
		/// <summary>
		/// Gets the number of channels used by a render target format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static int FormatChannels(SurfaceFormat format)
		{
			switch (format)
			{
				case SurfaceFormat.Single:
				case SurfaceFormat.HalfSingle:
				case SurfaceFormat.Luminance16:
					return 1;

				case SurfaceFormat.Rg32:
				case SurfaceFormat.HalfVector2:
				case SurfaceFormat.Vector2:
					return 2;


				case SurfaceFormat.Bgr32:
				case SurfaceFormat.Rgb32:
				case SurfaceFormat.Bgr565:
				case SurfaceFormat.Bgr555:
					return 3;

				case SurfaceFormat.Color:
				case SurfaceFormat.Rgba1010102:
				case SurfaceFormat.Bgra1010102:
				case SurfaceFormat.Bgra4444:
				case SurfaceFormat.Bgra5551:
				case SurfaceFormat.Rgba64:
				case SurfaceFormat.HalfVector4:
				case SurfaceFormat.Vector4:
					return 4;
				default:
					//throw new NotImplementedException(format.ToString());
					return 0;
			}
		}

		private static Xen.Graphics.State.DeviceRenderState nullState = new Xen.Graphics.State.DeviceRenderState();

		/// <summary>
		/// Perform all drawing to this draw target. All modifiers will be applied, and all drawable items in draw list will be drawn to the draw target
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			if (state.DrawTarget != null)
				throw new ArgumentException("Already rendering to another draw target: " + state.DrawTarget.ToString());

			if (GetEnabled())
			{
				int repeats = GetRepeatCount();
				state.PushRenderState(ref nullState);

				rendering = true;
				ushort stackHeight, stateHeight, cameraHeight,preCull,postCull;

				state.DrawTarget = this;

#if DEBUG
				System.Threading.Interlocked.Increment(ref state.Application.currentFrame.DrawTargetsDrawCount);
#endif
				Begin(state);

				for (int repeat = 0; repeat < repeats; repeat++)
				{
					state.GetStackHeight(out stackHeight, out stateHeight, out cameraHeight, out preCull, out postCull);

					ICamera cam = camera;
					if (repeats > 1)
					{
						if (!BeginRepeat(state, repeat, ref cam))
							continue;
					}

#if DEBUG
					System.Threading.Interlocked.Increment(ref state.Application.currentFrame.DrawTargetsPassCount);

#if XBOX360
					state.Application.currentFrame.XboxPixelFillBias += this.Width * this.Height;
#endif
#endif

					if (bufferClear.Enabled)
						bufferClear.Draw(state);

					if (modifiers != null)
					{
						foreach (IBeginEndDraw mod in modifiers)
						{
							if (mod.Enabled)
							{
								activeModifiers.Add(mod);
								mod.Begin(state);
							}
						}
					}

					state.PushCamera(cam);

					foreach (IDraw block in drawList)
					{
						if (block.CullTest(state))
							block.Draw(state);
					}

#if XEN_EXTRA
					state.RunDeferredDrawCalls();
#endif

					state.PopCamera();

					//end in reverse order
					if (modifiers != null)
					{
						for (int i = activeModifiers.Count - 1; i >= 0; i--)
						{
							activeModifiers[i].End(state);
						}
						activeModifiers.Clear();
					}

					if (repeats > 1)
						EndRepeat(state, repeat);

					state.ValidateStackHeight(stackHeight, stateHeight, cameraHeight, preCull, postCull);
				}
					
				End(state);

				state.DrawTarget = null;
				rendering = false;

				state.PopRenderState();

				state.EndFrameCleanup();
			}
		}

		/// <summary></summary>
		/// <param name="state"></param>
		protected internal abstract void Begin(DrawState state);
		/// <summary></summary>
		/// <param name="state"></param>
		protected internal abstract void End(DrawState state);

		/// <summary></summary>
		protected internal abstract bool HasDepthBuffer		{ get; }
		/// <summary></summary>
		protected internal abstract bool HasStencilBuffer	{ get; }

		//void IBeginEndDraw.Begin(DrawState state)	{ this.Begin(state); }
		//void IBeginEndDraw.End(DrawState state)		{ this.End(state); }

		/// <summary>
		/// Gets the width of the draw target
		/// </summary>
		public abstract int Width  { get; }
		/// <summary>
		/// Gets the height of the draw target
		/// </summary>
		public abstract int Height { get; }
		/// <summary>
		/// Gets the width/height of the draw target as a Vector2
		/// </summary>
		public Vector2 Size
		{
			get
			{
				Vector2 v;
				int i = -1;
				GetWidthHeightAsVector(out v, ref i);
				return v;
			}
		}
		/// <summary></summary>
		/// <param name="size"></param>
		/// <param name="changeIndex"></param>
		/// <returns></returns>
		internal protected abstract bool GetWidthHeightAsVector(out Vector2 size, ref int changeIndex);

		internal override int GetAllocatedManagedBytes()
		{
			return 0;
		}

		internal virtual int GetRepeatCount() 
		{
			return 1; 
		}
		internal virtual bool BeginRepeat(DrawState state, int repeat, ref ICamera camera)
		{
			return true;
		}
		internal virtual void EndRepeat(DrawState state, int repeat)
		{
		}

		internal sealed override ResourceType ResourceType
		{
			get { return ResourceType.RenderTarget; }
		}
	}

	/// <summary>
	/// A draw target that draws directly to the screen
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class DrawTargetScreen : DrawTarget
	{
		private readonly Xen.Application application;
		private bool hasDepth, hasStencil;
		private Vector2 windowSize;
		private int windowSizeChangeIndex = 1;

		/// <summary>
		/// Construct the draw target
		/// </summary>
		/// <param name="application"></param>
		/// <param name="camera"></param>
		public DrawTargetScreen(Application application, ICamera camera) : base(camera)
		{
			this.application = application;

			if (!application.IsInitailised)
				throw new InvalidOperationException("Application instance has not had Initalise() called yet");
			SetDepth(application);

			Vector2 ws = new Vector2((float)Width, (float)Height);
			if (ws.X != windowSize.X || ws.Y != windowSize.Y)
			{
				windowSize = ws;
				windowSizeChangeIndex = System.Threading.Interlocked.Increment(ref DrawTarget.baseSizeIndex);
			}
		}

		private void SetDepth(Application application)
		{
			DepthStencilBuffer depth = application.Depth;

			hasDepth = depth != null;
			hasStencil = depth != null && (depth.Format == DepthFormat.Depth15Stencil1 ||
											depth.Format == DepthFormat.Depth24Stencil4 ||
											depth.Format == DepthFormat.Depth24Stencil8 ||
											depth.Format == DepthFormat.Depth24Stencil8Single);
		}

		/// <summary></summary>
		/// <param name="state"></param>
		protected internal override void Begin(DrawState state)
		{
			GraphicsDevice device = state.graphics;

			state.ResetTextures();

			device.SetRenderTarget(0, null);
			device.DepthStencilBuffer = state.Application.Depth;

			Vector2 ws = new Vector2((float)Width, (float)Height);
			if (ws.X != windowSize.X || ws.Y != windowSize.Y)
			{
				windowSize = ws;
				windowSizeChangeIndex = System.Threading.Interlocked.Increment(ref DrawTarget.baseSizeIndex);
			}
		}

		/// <summary></summary>
		/// <param name="state"></param>
		protected internal override void End(DrawState state)
		{
		}

		/// <summary>
		/// Gets the surface format of the colour buffer for the screen
		/// </summary>
		public override SurfaceFormat SurfaceFormat
		{
			get { return application.GetScreenFormat(); }
		}
		/// <summary>
		/// Gets the multisample level for this draw target
		/// </summary>
		public override MultiSampleType MultiSampleType
		{
			get { return application.GetScreenMultisample(); }
		}

		/// <summary>
		/// Gets the width of the screen
		/// </summary>
		public override int Width
		{
			get 
			{
				return application.WindowWidth;
			}
		}

		/// <summary>
		/// Gets the height of the screen
		/// </summary>
		public override int Height
		{
			get 
			{
				return application.WindowHeight; 
			}
		}

		/// <summary></summary>
		/// <param name="size"></param>
		/// <param name="ci"></param>
		/// <returns></returns>
		internal protected override bool GetWidthHeightAsVector(out Vector2 size, ref int ci) 
		{
			size = windowSize;
			if (windowSizeChangeIndex != ci)
			{
				ci = windowSizeChangeIndex;
				return true;
			}
			return false;
		}

		/// <summary></summary>
		protected internal override bool HasDepthBuffer
		{
			get { return hasDepth; }
		}

		/// <summary></summary>
		protected internal override bool HasStencilBuffer
		{
			get { return hasStencil; }
		}

		internal override void Warm(Application application,GraphicsDevice device)
		{
		}

		internal override int GetAllocatedDeviceBytes()
		{
			//approximate
			return 0;// 2 * (4 + hasDepth ? 2 : 0) * Width * Height;
		}
		internal override int GetAllocatedManagedBytes()
		{
			return 0;
		}
		internal override bool InUse
		{
			get { return true; }
		}
		internal override bool IsDisposed
		{
			get { return false; }
		}
	}

	/// <summary>
	/// Stores a list of <see cref="DrawTargetTexture2D"/> as a group, for use with Multiple Render Target (MRT) support. See <see cref="MaxSimultaneousRenderTargets"/> for the maximum group size (usually 4)
	/// </summary>
	/// <remarks>
	/// <para>When using multiple render targets, each target must be the same byte size.</para>
	/// <para>Most hardware does not support blending with MRT</para>
	/// <para>The pixel shader being used must ouput a colour value for each render target being used</para>
	/// <para>For example, drawing to three render targets at once:</para>
	/// <code>
	/// void pixelShader(out float4 colour0 : COLOR0, out float4 colour1 : COLOR1, out float4 colour2 : COLOR2)
	/// {
	///		colour0 = float4(...);
	///		colour1 = float4(...);
	///		colour2 = float4(...);
	///		...
	/// }
	/// </code>
	/// </remarks>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class DrawTargetTexture2DGroup : DrawTarget, IDisposable
	{
		private DrawTargetTexture2D[] targets;

		/// <summary>
		/// Construct the render target group
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="targets"></param>
		public DrawTargetTexture2DGroup(ICamera camera, params DrawTargetTexture2D[] targets)
			: base(camera)
		{
			this.targets = (DrawTargetTexture2D[])targets.Clone();

			if (targets.Length < 1)
				throw new ArgumentException("At least one render targets must be specified");
			if (targets.Length > MaxSimultaneousRenderTargets)
				throw new ArgumentException(string.Format("Device only supports {0} simultaneous render targets",MaxSimultaneousRenderTargets));

			DrawTargetTexture2D baseTarget = targets[0];
			if (baseTarget == null)
				throw new ArgumentNullException(string.Format("target[{0}]", 0));

			for (int i = 1; i < targets.Length; i++)
			{
				if (targets[i] == null)
					throw new ArgumentNullException(string.Format("target[{0}]",i));
				if (targets[i].Width != baseTarget.Width ||
					targets[i].Height != baseTarget.Height)
					throw new ArgumentException(string.Format("target[{0}] size mismatch with target[0]", i));
				if (FormatSize(targets[i].SurfaceFormat) !=
					FormatSize(baseTarget.SurfaceFormat))
					throw new ArgumentException(string.Format("target[{0}] SurfaceFormat size mismatch with target[0]", i));
				if (targets[i].MultiSampleType != baseTarget.MultiSampleType)
					throw new ArgumentException(string.Format("target[{0}] multisample mismatch with target[0]", i));
			}
		}

		/// <summary>
		/// Gets the number of render targets stored in the group
		/// </summary>
		public int Count
		{
			get
			{
				if (targets == null)
					throw new ObjectDisposedException("this"); 
				return targets.Length;
			}
		}

		/// <summary>
		/// Creates a clone of this draw target that shares the same rendering resources (no new resources are allocated)
		/// </summary>
		/// <param name="copyModifiers">copy modifier list into the clone</param>
		/// <param name="copyDrawList">copy draw list into the clone</param>
		/// <returns></returns>
		public DrawTargetTexture2DGroup Clone(bool copyModifiers, bool copyDrawList)
		{
			DrawTargetTexture2DGroup clone = (DrawTargetTexture2DGroup)MemberwiseClone();
			CloneTo(clone, copyModifiers, copyDrawList);
			return clone;
		}

		/// <summary>
		/// Get a draw target by index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DrawTargetTexture2D GetTarget(int index)
		{
			if (targets == null)
				throw new ObjectDisposedException("this");
			return targets[index];
		}

		/// <summary>
		/// Gets the surface format of the first draw target in the group
		/// </summary>
		public override SurfaceFormat SurfaceFormat
		{
			get { return targets[0].SurfaceFormat; }
		}
		/// <summary>
		/// Gets the multisample level for the first draw target in the group
		/// </summary>
		public override MultiSampleType MultiSampleType
		{
			get { return targets[0].MultiSampleType; }
		}

		/// <summary>
		/// Gets the maximum group size supported by the hardware (usually 4 - the maximum value)
		/// </summary>
		public static int MaxSimultaneousRenderTargets
		{
			get
			{
				return GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware).MaxSimultaneousRenderTargets;
			}
		}

		/// <summary></summary>
		/// <param name="state"></param>
		protected internal override void Begin(DrawState state)
		{
			if (targets == null)
				throw new ObjectDisposedException("this");

			for (int i = 0; i < targets.Length; i++)
				targets[i].Warm(state);

			targets[0].Begin(state);

			for (int i = 1; i < targets.Length; i++)
				state.graphics.SetRenderTarget(i, targets[i].GetRenderTarget2D());
		}

		/// <summary></summary>
		/// <param name="state"></param>
		protected internal override void End(DrawState state)
		{
			if (targets == null)
				throw new ObjectDisposedException("this");

			targets[0].End(state);

			for (int i = 1; i < targets.Length; i++)
				state.graphics.SetRenderTarget(i, null);
		}

		/// <summary></summary>
		protected internal override bool HasDepthBuffer
		{
			get
			{
				if (targets == null)
					throw new ObjectDisposedException("this");
				return targets[0].HasDepthBuffer;
			}
		}

		/// <summary></summary>
		protected internal override bool HasStencilBuffer
		{
			get
			{
				if (targets == null)
					throw new ObjectDisposedException("this");
				return targets[0].HasStencilBuffer;
			}
		}

		/// <summary>
		/// Gets the width of the draw targets in the group
		/// </summary>
		public override int Width
		{
			get
			{
				if (targets == null)
					throw new ObjectDisposedException("this");
				return targets[0].Width;
			}
		}

		/// <summary>
		/// Gets the height of the draw targets in the group
		/// </summary>
		public override int Height
		{
			get
			{
				if (targets == null)
					throw new ObjectDisposedException("this");
				return targets[0].Height;
			}
		}

		/// <summary></summary>
		/// <param name="size"></param>
		/// <param name="changeIndex"></param>
		/// <returns></returns>
		protected internal override bool GetWidthHeightAsVector(out Vector2 size, ref int changeIndex)
		{
			if (targets == null)
					throw new ObjectDisposedException("this"); 
			return targets[0].GetWidthHeightAsVector(out size, ref changeIndex);
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			this.targets = null;
		}

		internal override int GetAllocatedDeviceBytes()
		{
			return 0;
		}
		internal override int GetAllocatedManagedBytes()
		{
			return 0;
		}
		internal override bool InUse
		{
			get { return true; }
		}
		internal override bool IsDisposed
		{
			get { return targets == null; }
		}
		internal override void Warm(Application application,GraphicsDevice device)
		{
			if (targets == null)
				throw new ObjectDisposedException("this");
			for (int i = 0; i < targets.Length; i++)
				targets[i].Warm(application,device);
		}

		#endregion
	}

	/// <summary>
	/// A draw target that draws to a <see cref="Texture2D"/> render target. 
	/// </summary>
	/// <remarks>
	/// <para>Most draw targets will create the render target resoures. Note these resoures are not created until either the first time the target is drawn, or <see cref="Resource.Warm(DrawState)"/> is called.</para>
	/// <para>To share the resources used by a draw target texture, use <see cref="Clone"/></para>
	/// <para>A draw target can be created from XNA render targets using <see cref="CreateFromRenderTarget2D(ICamera,RenderTarget2D,DepthStencilBuffer)"/> (not recommended)</para>
	/// </remarks>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class DrawTargetTexture2D : DrawTarget, IDisposable, IContentOwner
	{
		private RenderTarget2D texture;
		private DepthStencilBuffer depth;
		private readonly int width, height;
		private bool hasDepth, hasStencil, ownsDepth;
		private bool mipmap, depthEnabled = true;
		private SurfaceFormat format;
		private MultiSampleType multisample;
		private RenderTargetUsage usage;
		private DepthFormat? depthFormat = null;
		private Texture2D renderTexture;
		private readonly Vector2 sizeAsVector;
		private readonly int sizeIndex = System.Threading.Interlocked.Increment(ref DrawTarget.baseSizeIndex);
		private bool ownerRegistered, isDisposed;
		private DrawTargetTexture2D cloneOf, sharedDepth;

		/// <summary>
		/// Gets the XNA depth stencil buffer created or shared by this draw texture. Note: Resources are not created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called. Directly accessing this resource is not recommended
		/// </summary>
		public DepthStencilBuffer GetDepthStencilBuffer()
		{
			return depth;
		}
		/// <summary>
		/// Gets the surface format of the colour buffer for this draw texture
		/// </summary>
		public override SurfaceFormat SurfaceFormat
		{
			get { return format; }
		}
		/// <summary>
		/// Gets the multisample level for this draw target
		/// </summary>
		public override MultiSampleType MultiSampleType
		{
			get { return multisample; }
		}

		/// <summary>
		/// Gets the XNA render target created or shared by this draw texture. Note: Resources are not created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called. Directly accessing this resource is not recommended
		/// </summary>
		public RenderTarget2D GetRenderTarget2D()
		{
			return texture;
		}

		/// <summary>
		/// Creates a clone of this draw target that shares the same rendering resources (no new resources are allocated)
		/// </summary>
		/// <param name="copyModifiers">copy modifier list into the clone</param>
		/// <param name="copyDrawList">copy draw list into the clone</param>
		/// <param name="retainDepth">cloned draw target should also retain the depth stencil buffer</param>
		/// <returns></returns>
		public DrawTargetTexture2D Clone(bool retainDepth, bool copyModifiers, bool copyDrawList)
		{
			DrawTargetTexture2D clone = (DrawTargetTexture2D)MemberwiseClone();
			clone.cloneOf = this;
			clone.depthEnabled &= retainDepth;
			CloneTo(clone, copyModifiers, copyDrawList);

			if (!retainDepth)
			{
				clone.ClearBuffer.ClearDepthEnabled = false;
				clone.ClearBuffer.ClearStencilEnabled = false;
			}

			return clone;
		}

		private DrawTargetTexture2D CloneRoot
		{
			get
			{
				if (cloneOf == null)
					return this;
				else
					return cloneOf.CloneRoot;
			}
		}

		/// <summary>
		/// Returns true when comparing equivalent draw targets, including cloned targets
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is DrawTargetTexture2D)
				return CloneRoot == (obj as DrawTargetTexture2D).CloneRoot;
			return false;
		}
		/// <summary></summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (cloneOf == null)
				return base.GetHashCode();
			return CloneRoot.GetHashCode();
		}

		/// <summary>
		/// Gets the texture for this draw target. Returns NULL if the resource hasn't been created. NOTE: this texture will become invalid after a device reset (see <see cref="IContentOwner"/> for details)
		/// </summary>
		/// <remarks>Call <see cref="GetTexture(DrawState)"/> to get the texture, creating the resource beforehand if required.</remarks>
		/// <returns>Texture for this draw target</returns>
		public Texture2D GetTexture()
		{
			if (texture == null)
				return null;
			if (renderTexture == null)
				renderTexture = texture.GetTexture();
			return renderTexture;
		}

		/// <summary>
		/// Gets the texture for this draw target, Warming the resource if required. NOTE: this texture will become invalid after a device reset (see <see cref="IContentOwner"/> for details)
		/// </summary>
		/// <returns>Texture for this draw target</returns>
		public Texture2D GetTexture(DrawState state)
		{
			if (this.texture == null)
			{
				if (state.GetDrawTarget() != null)
					throw new InvalidOperationException("A DrawTargetTexture2D Resource may not be created while rendering to another DrawTarget");
				this.Warm(state);
			}

			return GetTexture();
		}
	

		private void SetHasDepth()
		{
			if (depthFormat != null)
			{
				hasDepth = true;
				hasStencil =   (depthFormat.Value == DepthFormat.Depth15Stencil1 ||
								depthFormat.Value == DepthFormat.Depth24Stencil4 ||
								depthFormat.Value == DepthFormat.Depth24Stencil8 ||
								depthFormat.Value == DepthFormat.Depth24Stencil8Single);
			}
		}

		/// <summary>
		/// Creates the draw texture. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="format"></param>
		public DrawTargetTexture2D(ICamera camera, int width, int height, SurfaceFormat format)
			: this(camera, width, height, format, false, MultiSampleType.None, RenderTargetUsage.PlatformContents)
		{
		}

		/// <summary>
		/// Creates the draw texture. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="format"></param>
		/// <param name="depthFormat"></param>
		public DrawTargetTexture2D(ICamera camera, int width, int height, SurfaceFormat format, DepthFormat? depthFormat)
			: this(camera, width, height, format, depthFormat, false, MultiSampleType.None)
		{
		}

		//public DrawTargetTexture2D(ICamera camera, int width, int height, SurfaceFormat format, bool mipmap)
		//    : this(camera, width, height, format, mipmap, MultiSampleType.None)
		//{
		//}

		/// <summary>
		/// Creates the draw texture. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="format"></param>
		/// <param name="depthFormat"></param>
		/// <param name="mipmap"></param>
		public DrawTargetTexture2D(ICamera camera, int width, int height, SurfaceFormat format, DepthFormat? depthFormat, bool mipmap)
			: this(camera, width, height, format, depthFormat, mipmap, MultiSampleType.None)
		{
		}

		//public DrawTargetTexture2D(ICamera camera, int width, int height, SurfaceFormat format, bool mipmap, MultiSampleType multisample)
		//    : this(camera, width, height, format, mipmap, multisample, RenderTargetUsage.PlatformContents)
		//{
		//}

		/// <summary>
		/// Creates the draw texture. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="format"></param>
		/// <param name="depthFormat"></param>
		/// <param name="mipmap"></param>
		/// <param name="multisample"></param>
		public DrawTargetTexture2D(ICamera camera, int width, int height, SurfaceFormat format, DepthFormat? depthFormat, bool mipmap, MultiSampleType multisample)
			: this(camera, width, height, format, depthFormat, mipmap, multisample, RenderTargetUsage.PlatformContents)
		{
		}

		private DrawTargetTexture2D(ICamera camera, int width, int height, SurfaceFormat format, bool mipmap, MultiSampleType multisample, RenderTargetUsage usage) :
			base(camera)
		{
			if (!SupportsFormat(format))
				throw new ArgumentException("Graphics adapter does not support the specified format");
#if !XBOX360
#if XNA_2_0
			if (mipmap)
				throw new NotSupportedException("XNA GS2.0 doesn't consistently support mipmapped render textures on the PC for all hardware (Use DrawTargetTexture2D.ForceGenerateMipmap if you want to force using mipmaps)");
#endif
#endif

			this.mipmap = mipmap;
			this.format = format;
			this.multisample = multisample;
			this.usage = usage;
			this.width = width;
			this.height = height;

			this.sizeAsVector = new Vector2((float)this.width, (float)this.height);
		}


		
#if XNA_2_0
		/// <summary>
		/// XNA doesn't correctly implement mipmap generation on the PC on some hardware. However this method allows forcing a created draw target to be created with mipmaps (use with caution, as mipmap generation *will* fail on some systems)
		/// </summary>
		/// <param name="texture"></param>
		public static void ForceGenerateMipmap(DrawTargetTexture2D texture)
		{
			if (texture == null)
				throw new ArgumentNullException();
			if (texture.texture != null)
				throw new ArgumentException("Resource already created");
			texture.mipmap = true;
		}
#endif

		/// <summary>
		/// Creates the draw texture. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="format"></param>
		/// <param name="depthFormat"></param>
		/// <param name="mipmap"></param>
		/// <param name="multisample"></param>
		/// <param name="usage"></param>
		public DrawTargetTexture2D(ICamera camera, int width, int height, SurfaceFormat format, DepthFormat? depthFormat, bool mipmap, MultiSampleType multisample, RenderTargetUsage usage)
			: this(camera, width, height, format, mipmap, multisample, usage)
		{
			if (depthFormat != null && !SupportsFormat(depthFormat.Value))
				throw new ArgumentException("Graphics adapter does not support the specified depth format");

			this.depthFormat = depthFormat;
			this.ownsDepth = true;

			if (this.depthFormat == null)
			{
				this.ClearBuffer.ClearDepthEnabled = false;
				this.ClearBuffer.ClearStencilEnabled = false;
			}
			else
			{
				if (this.depthFormat.Value != DepthFormat.Depth15Stencil1 &&
					this.depthFormat.Value != DepthFormat.Depth24Stencil4 &&
					this.depthFormat.Value != DepthFormat.Depth24Stencil8 &&
					this.depthFormat.Value != DepthFormat.Depth24Stencil8Single)
					this.ClearBuffer.ClearStencilEnabled = false;
			}

			SetHasDepth();
		}


		/// <summary>
		/// Create a draw texture directly from an XNA render target (not recommended)
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="renderTexture"></param>
		/// <returns></returns>
		public static DrawTargetTexture2D CreateFromRenderTarget2D(ICamera camera, RenderTarget2D renderTexture)
		{
			return new DrawTargetTexture2D(camera, renderTexture);
		}

		/// <summary>
		/// Create a draw texture directly from an XNA render target and depth buffer (not recommended)
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="renderTexture"></param>
		/// <param name="depthBuffer"></param>
		/// <returns></returns>
		public static DrawTargetTexture2D CreateFromRenderTarget2D(ICamera camera, RenderTarget2D renderTexture, DepthStencilBuffer depthBuffer)
		{
			return new DrawTargetTexture2D(camera, renderTexture, depthBuffer);
		}

		private DrawTargetTexture2D(ICamera camera, RenderTarget2D xnaRenderTexture)
			: this(camera, xnaRenderTexture, null)
		{
		}

		private DrawTargetTexture2D(ICamera camera, RenderTarget2D xnaRenderTexture, DepthStencilBuffer depthBuffer)
			: base(camera)
		{
			if (xnaRenderTexture == null)
				throw new ArgumentNullException();
			this.texture = xnaRenderTexture;
			this.depth = depthBuffer;
			this.ownsDepth = false;
			if (depth != null)
				if (xnaRenderTexture.Width != depth.Width || xnaRenderTexture.Height != depth.Height)
					throw new ArgumentException("Depth/Texture Width/Height mismatch");

			this.width = xnaRenderTexture.Width;
			this.height = xnaRenderTexture.Height;

			this.sizeAsVector = new Vector2((float)this.width, (float)this.height);

			if (depthBuffer != null)
				this.depthFormat = depthBuffer.Format;

			SetHasDepth();
		}

		/// <summary>
		/// Share the depth buffer created by this draw texture with another draw texture
		/// </summary>
		/// <param name="shareDepthTo"></param>
		public void ShareDepthBuffer(DrawTargetTexture2D shareDepthTo)
		{
			if (shareDepthTo == null || shareDepthTo == this)
				throw new ArgumentNullException();
			if (shareDepthTo.depth != null)
				throw new ArgumentException("Shared depth target already has a created depth buffer");
			if (shareDepthTo.cloneOf != null ||
				cloneOf != null)
				throw new ArgumentException("Cannot share to/from clones");
			if (shareDepthTo.depthFormat != this.depthFormat)
				throw new ArgumentException("Depth format mismatch");
			if (shareDepthTo.sharedDepth != null)
				throw new ArgumentException("Shared depth target already uses a shared depth buffer");
			if (this.Width < shareDepthTo.Width ||
				this.Height < shareDepthTo.Height)
				throw new ArgumentException("Size mismatch");

			shareDepthTo.sharedDepth = sharedDepth ?? this;
		}

		/// <summary>
		/// Returns true if the hardware supports the given colour buffer surface format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static bool SupportsFormat(SurfaceFormat format)
		{
			return GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.None, ResourceType.RenderTarget, format);
		}
		/// <summary>
		/// Returns true if the hardware supports the given depth buffer format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static bool SupportsFormat(DepthFormat format)
		{
			return GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.None, ResourceType.DepthStencilBuffer, format);
		}
		/// <summary>
		/// Returns true if the hardware supports the alpha blending on the given colour buffer surface format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static bool SupportsFormatBlending(SurfaceFormat format)
		{
			return GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.PostPixelShaderBlending, ResourceType.RenderTarget, format);
		}
		/// <summary>
		/// Returns true if the hardware supports the texture filtering on a texture of the given surface format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static bool SupportsFormatFiltering(SurfaceFormat format)
		{
			return GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.Filter, ResourceType.RenderTarget, format);
		}

		/// <summary>
		/// Dispose the draw target and all resources used. If this render target is a clone, the shared resources are NOT disposed
		/// </summary>
		public void Dispose()
		{
			isDisposed = true;

			if (cloneOf == null)
			{
				if (texture != null)
					texture.Dispose();
				if (depth != null)
					depth.Dispose();
			}
			this.texture = null;
			this.depth = null;
		}

		/// <summary></summary>
		/// <param name="state"></param>
		protected internal override void Begin(DrawState state)
		{
			if (isDisposed)
				throw new ObjectDisposedException("this");

#if XBOX360
			state.nonScreenRenderComplete = true;
#endif

			GraphicsDevice device = state.graphics;
			
			if (texture == null)
			{
				Warm(state);
			}
			if (this.depthFormat != null && this.depth == null && depthEnabled)
			{
				Warm(state);
			}

			if (texture.IsDisposed || (depth != null && depthEnabled && depth.IsDisposed))
				throw new ObjectDisposedException("RenderTexture");

			state.ResetTextures();

			device.DepthStencilBuffer = depthEnabled ? depth : null;
			device.SetRenderTarget(0, texture);
		}

		/// <summary></summary>
		/// <param name="state"></param>
		protected internal override void End(DrawState state)
		{
			GraphicsDevice device = state.graphics;

			if (texture.IsDisposed || (depth != null && depth.IsDisposed))
				throw new ObjectDisposedException("RenderTexture");

			device.SetRenderTarget(0, null);

			if (mipmap)
				GetTexture().GenerateMipMaps(TextureFilter.Linear);
		}

		/// <summary>
		/// Gets the width of the draw target
		/// </summary>
		public override int Width
		{
			get { return width; }
		}

		/// <summary>
		/// Gets the height of the draw target
		/// </summary>
		public override int Height
		{
			get { return height; }
		}

		/// <summary></summary>
		/// <param name="size"></param>
		/// <param name="ci"></param>
		/// <returns></returns>
		internal protected override bool GetWidthHeightAsVector(out Vector2 size, ref int ci)
		{
			size = sizeAsVector;
			if (ci != sizeIndex)
			{
				ci = sizeIndex;
				return true;
			}
			return false;
		}

		/// <summary></summary>
		protected internal override bool HasDepthBuffer
		{
			get { return hasDepth && depthEnabled; }
		}

		/// <summary></summary>
		protected internal override bool HasStencilBuffer
		{
			get { return hasStencil && depthEnabled; }
		}

		internal override int GetAllocatedDeviceBytes()
		{
			if (cloneOf != null)
				return 0;
			int bytes = 0;
			if (texture != null)
				bytes += FormatSize(this.format) * Width * Height;
			if (depth != null && depthFormat != null)
			{
				int depthSize = 0;
				switch (depthFormat.Value)
				{
					case DepthFormat.Depth15Stencil1:
					case DepthFormat.Depth16:
						depthSize = 2;
						break;
					case DepthFormat.Depth24:
					case DepthFormat.Depth24Stencil4:
					case DepthFormat.Depth24Stencil8:
					case DepthFormat.Depth24Stencil8Single:
					case DepthFormat.Depth32:
						depthSize = 4;
						break;
				}
				bytes += depthSize * Width * Height;
			}
			return bytes;
		}

		internal override int GetAllocatedManagedBytes()
		{
			return 0;
		}

		internal override bool InUse
		{
			get { return texture != null; }
		}

		internal override bool IsDisposed
		{
			get
			{
				return isDisposed;
			}
		}

		internal override void Warm(Application application, GraphicsDevice device)
		{
			if (cloneOf != null)
			{
				CloneRoot.Warm(application);
				this.texture = CloneRoot.texture;
				this.depth = CloneRoot.depth;
				return;
			}

			if (!ownerRegistered)
			{
				ownerRegistered = true;
				application.Content.AddHighPriority(this);
			}

			if (this.depthFormat != null && this.depth == null)
			{
				if (sharedDepth != null)
				{
					this.sharedDepth.Warm(application);
					this.depth = this.sharedDepth.depth;
				}
				else
					this.depth = new DepthStencilBuffer(device, width, height, depthFormat.Value, multisample, 0);
			}
			if (texture == null)
			{
				if (format == (SurfaceFormat)0)
					throw new InvalidOperationException("Base render target is null or has been disposed");

				this.texture = new RenderTarget2D(device, width, height, mipmap ? 0 : 1, format, multisample, 0, usage);
				//xna bug forces this...
				device.SetRenderTarget(0, texture);
				device.SetRenderTarget(0, null);
			}
		}

		void IContentOwner.LoadContent(ContentRegister content, DrawState state, Microsoft.Xna.Framework.Content.ContentManager manager)
		{
			if (IsDisposed)
				return;

			GraphicsDevice device = state.graphics;

			if (texture != null &&
				texture.GraphicsDevice != device)
			{
				if (format != (SurfaceFormat)0 && cloneOf == null)
				{
					texture.Dispose();
					if (depth != null && ownsDepth)
						depth.Dispose();
				}
				texture = null;
				depth = null;
			}

			Warm(state);
		}

		void IContentOwner.UnloadContent(ContentRegister content, DrawState state)
		{
			this.renderTexture = null;
		}
	}







	/// <summary>
	/// A draw target that draws to a <see cref="TextureCube"/> render target. 
	/// </summary>
	/// <remarks>
	/// <para>Most draw targets will create the render target resoures. Note these resoures are not created until either the first time the target is drawn, or <see cref="Resource.Warm(DrawState)"/> is called.</para>
	/// <para>To share the resources used by a draw target texture, use <see cref="Clone"/></para>
	/// <para>A draw target can be created from XNA render targets using <see cref="CreateFromRenderTargetCube(ICamera,RenderTargetCube,DepthStencilBuffer)"/> (not recommended)</para>
	/// </remarks>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class DrawTargetTextureCube : DrawTarget, IDisposable, IContentOwner
	{
		private RenderTargetCube texture;
		private DepthStencilBuffer depth;
		private readonly int resolution;
		private bool hasDepth, hasStencil, ownsDepth;
		private bool mipmap, depthEnabled = true;
		private SurfaceFormat format;
		private MultiSampleType multisample;
		private RenderTargetUsage usage;
		private DepthFormat? depthFormat = null;
		private TextureCube renderTexture;
		private readonly Vector2 sizeAsVector;
		private readonly int sizeIndex = System.Threading.Interlocked.Increment(ref DrawTarget.baseSizeIndex);
		private Camera3D cubeCamera;
		private Matrix cubeCameraMatrix;
		private readonly bool[] facesEnabled = new bool[6] { true, true, true, true, true, true };
		private bool anyFaceEnabled = true;
		private int maxFaceEnabled = 5;
		private int minFaceEnabled = 0;
		private bool ownerRegistered = false;
		private bool isDisposed = false;
		private DrawTargetTextureCube cloneOf,sharedDepth;

		/// <summary>
		/// Gets if rendering to a cube face is enabled
		/// </summary>
		/// <param name="face"></param>
		/// <returns></returns>
		public bool GetFaceRenderEnabled(CubeMapFace face)
		{
			return facesEnabled[(int)face];
		}
		/// <summary>
		/// Set enabled/disabled rendering to a cubemap face
		/// </summary>
		/// <param name="face"></param>
		/// <param name="enabled"></param>
		public void SetFaceRenderEnabled(CubeMapFace face, bool enabled)
		{
			if (facesEnabled[(int)face] != enabled)
			{
				facesEnabled[(int)face] = enabled;
				ComputeEnabledFaces();
			}
		}

		/// <summary>
		/// Creates a clone of this draw target that shares the same rendering resources (no new resources are allocated)
		/// </summary>
		/// <param name="copyModifiers">copy modifier list into the clone</param>
		/// <param name="copyDrawList">copy draw list into the clone</param>
		/// <param name="retainDepth">cloned draw target should also retain the depth stencil buffer</param>
		/// <returns></returns>
		public DrawTargetTextureCube Clone(bool retainDepth, bool copyModifiers, bool copyDrawList)
		{
			DrawTargetTextureCube clone = (DrawTargetTextureCube)MemberwiseClone();
			clone.cloneOf = this;
			clone.depthEnabled &= retainDepth;
			CloneTo(clone, copyModifiers, copyDrawList);

			if (!retainDepth)
			{
				clone.ClearBuffer.ClearDepthEnabled = false;
				clone.ClearBuffer.ClearStencilEnabled = false;
			}

			return clone;
		}

		private DrawTargetTextureCube CloneRoot
		{
			get
			{
				if (cloneOf == null)
					return this;
				else
					return cloneOf.CloneRoot;
			}
		}

		/// <summary>
		/// Returns true when comparing equivalent draw targets, including cloned targets
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is DrawTargetTextureCube)
				return CloneRoot == (obj as DrawTargetTextureCube).CloneRoot;
			return false;
		}
		/// <summary></summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (cloneOf == null)
				return base.GetHashCode();
			return CloneRoot.GetHashCode();
		}

		void ComputeEnabledFaces()
		{
			anyFaceEnabled = false;
			maxFaceEnabled = int.MinValue;
			minFaceEnabled = int.MaxValue;

			for (int i = 0; i < facesEnabled.Length; i++)
			{
				if (facesEnabled[i])
				{
					anyFaceEnabled = true;
					maxFaceEnabled = Math.Max(maxFaceEnabled, i);
					minFaceEnabled = Math.Min(minFaceEnabled, i);
				}
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		protected override bool GetEnabled()
		{
			return anyFaceEnabled && base.GetEnabled();
		}

		/*
		PositiveX = 0,
		NegativeX = 1,
		PositiveY = 2,
		NegativeY = 3,
		PositiveZ = 4,
		NegativeZ = 5,*/
		private static Matrix[] CubeMapFaceMatrices = new Matrix[]
		{
			Matrix.CreateLookAt(Vector3.Zero,new Vector3(1,0,0),new Vector3(0,1,0)),
			Matrix.CreateLookAt(Vector3.Zero,new Vector3(-1,0,0),new Vector3(0,1,0)),
			Matrix.CreateLookAt(Vector3.Zero,new Vector3(0,1,0),new Vector3(0,0,1)),
			Matrix.CreateLookAt(Vector3.Zero,new Vector3(0,-1,0),new Vector3(0,0,-1)),
			Matrix.CreateLookAt(Vector3.Zero,new Vector3(0,0,-1),new Vector3(0,1,0)),
			Matrix.CreateLookAt(Vector3.Zero,new Vector3(0,0,1),new Vector3(0,1,0)),
		};

		/// <summary>
		/// Gets the matrix that represents the rotation of a cubemap face
		/// </summary>
		/// <param name="face"></param>
		/// <param name="matrix"></param>
		public static void GetCubeMapFaceMatrix(CubeMapFace face, out Matrix matrix)
		{
			matrix = CubeMapFaceMatrices[(int)face];
		}

		/// <summary>
		/// Returns true if the hardware supports the given colour buffer surface format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static bool SupportsFormat(SurfaceFormat format)
		{
			return GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.None, ResourceType.RenderTarget, format);
		}
		/// <summary>
		/// Returns true if the hardware supports the given depth buffer format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static bool SupportsFormat(DepthFormat format)
		{
			return GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.None, ResourceType.DepthStencilBuffer, format);
		}
		/// <summary>
		/// Returns true if the hardware supports the alpha blending on the given colour buffer surface format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static bool SupportsFormatBlending(SurfaceFormat format)
		{
			return GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.PostPixelShaderBlending, ResourceType.RenderTarget, format);
		}
		/// <summary>
		/// Returns true if the hardware supports the texture filtering on a texture of the given surface format
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public static bool SupportsFormatFiltering(SurfaceFormat format)
		{
			return GraphicsAdapter.DefaultAdapter.CheckDeviceFormat(DeviceType.Hardware, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.Filter, ResourceType.RenderTarget, format);
		}

		/// <summary>
		/// Gets the XNA depth stencil buffer created or shared by this draw cubemap. Note: Resources are not created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called. Directly accessing this resource is not recommended
		/// </summary>
		public DepthStencilBuffer GetDepthStencilBuffer()
		{
			return depth;
		}

		/// <summary>
		/// Gets the surface format of the colour buffer for this draw cubemap
		/// </summary>
		public override SurfaceFormat SurfaceFormat
		{
			get { return format; }
		}
		/// <summary>
		/// Gets the multisample level for this draw target
		/// </summary>
		public override MultiSampleType MultiSampleType
		{
			get { return multisample; }
		}

		/// <summary>
		/// Gets the XNA render target created or shared by this draw cubemap. Note: Resources are not created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called. Directly accessing this resource is not recommended
		/// </summary>
		public RenderTargetCube GetRenderTargetCube()
		{
			return texture;
		}

		/// <summary>
		/// Gets the texture for this draw target. Returns NULL if the resource hasn't been created. NOTE: this texture will become invalid after a device reset (see <see cref="IContentOwner"/> for details)
		/// </summary>
		/// <remarks>Call <see cref="GetTexture(DrawState)"/> to get the texture, creating the resource beforehand if required.</remarks>
		/// <returns>Texture for this draw target</returns>
		public TextureCube GetTexture()
		{
			if (texture == null)
				return null;
			if (renderTexture == null)
				renderTexture = texture.GetTexture();
			return renderTexture;
		}

		/// <summary>
		/// Gets the texture for this draw target, Warming the resource if required. NOTE: this texture will become invalid after a device reset (see <see cref="IContentOwner"/> for details)
		/// </summary>
		/// <returns>Texture for this draw target</returns>
		public TextureCube GetTexture(DrawState state)
		{
			if (texture == null)
			{
				if (state.GetDrawTarget() != null)
					throw new InvalidOperationException("A DrawTargetTextureCube Resource may not be created while rendering to another DrawTarget");
				this.Warm(state);
			}

			return GetTexture();
		}


		private void SetHasDepth()
		{
			if (depthFormat != null)
			{
				hasDepth = true;
				hasStencil = (depthFormat.Value == DepthFormat.Depth15Stencil1 ||
								depthFormat.Value == DepthFormat.Depth24Stencil4 ||
								depthFormat.Value == DepthFormat.Depth24Stencil8 ||
								depthFormat.Value == DepthFormat.Depth24Stencil8Single);
			}
		}

		/// <summary>
		/// Creates the draw target cubemap. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="resolution"></param>
		/// <param name="format"></param>
		public DrawTargetTextureCube(ICamera camera, int resolution, SurfaceFormat format)
			: this(camera, resolution, format, false, MultiSampleType.None, RenderTargetUsage.PlatformContents)
		{
		}

		/// <summary>
		/// Creates the draw target cubemap. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="resolution"></param>
		/// <param name="format"></param>
		/// <param name="depthFormat"></param>
		public DrawTargetTextureCube(ICamera camera, int resolution, SurfaceFormat format, DepthFormat depthFormat)
			: this(camera, resolution, format, depthFormat, false, MultiSampleType.None)
		{
		}

		/// <summary>
		/// Creates the draw target cubemap. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="resolution"></param>
		/// <param name="format"></param>
		/// <param name="depthFormat"></param>
		/// <param name="mipmap"></param>
		public DrawTargetTextureCube(ICamera camera, int resolution, SurfaceFormat format, DepthFormat depthFormat, bool mipmap)
			: this(camera, resolution, format, depthFormat, mipmap, MultiSampleType.None)
		{
		}

		/// <summary>
		/// Creates the draw target cubemap. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="resolution"></param>
		/// <param name="format"></param>
		/// <param name="depthFormat"></param>
		/// <param name="mipmap"></param>
		/// <param name="multisample"></param>
		public DrawTargetTextureCube(ICamera camera, int resolution, SurfaceFormat format, DepthFormat depthFormat, bool mipmap, MultiSampleType multisample)
			: this(camera, resolution, format, depthFormat, mipmap, multisample, RenderTargetUsage.PlatformContents)
		{
		}

		/// <summary>
		/// Creates the draw target cubemap. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="resolution"></param>
		/// <param name="format"></param>
		/// <param name="mipmap"></param>
		/// <param name="multisample"></param>
		/// <param name="usage"></param>
		private DrawTargetTextureCube(ICamera camera, int resolution, SurfaceFormat format, bool mipmap, MultiSampleType multisample, RenderTargetUsage usage)
			:
			base(camera)
		{
			if (!SupportsFormat(format))
				throw new ArgumentException("Graphics adapter does not support the specified format");
#if !XBOX360
#if XNA_2_0
			if (mipmap)
				throw new NotSupportedException("XNA GS2.0 doesn't consistently support mipmapped render textures on the PC for all hardware (Use DrawTargetTextureCube.ForceGenerateMipmap if you want to force using mipmaps)");
#endif
#endif
			this.mipmap = mipmap;
			this.format = format;
			this.multisample = multisample;
			this.usage = usage;
			this.resolution = resolution;

			this.sizeAsVector = new Vector2((float)this.resolution, (float)this.resolution);
		}
		
#if XNA_2_0
		/// <summary>
		/// XNA doesn't correctly implement mipmap generation on the PC on some hardware. However this method allows forcing a created draw target to be created with mipmaps (use with caution, as mipmap generation *will* fail on some systems)
		/// </summary>
		/// <param name="cube"></param>
		public static void ForceGenerateMipmap(DrawTargetTextureCube cube)
		{
			if (cube == null)
				throw new ArgumentNullException();
			if (cube.texture != null)
				throw new ArgumentException("Resource already created");
			cube.mipmap = true;
		}
#endif

		/// <summary>
		/// Creates the draw target cubemap. Note: Rendering resources will not be created until the first time the target is drawn or <see cref="Resource.Warm(DrawState)"/> is called
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="resolution"></param>
		/// <param name="format"></param>
		/// <param name="depthFormat"></param>
		/// <param name="mipmap"></param>
		/// <param name="multisample"></param>
		/// <param name="usage"></param>
		public DrawTargetTextureCube(ICamera camera, int resolution, SurfaceFormat format, DepthFormat depthFormat, bool mipmap, MultiSampleType multisample, RenderTargetUsage usage)
			: this(camera, resolution, format, mipmap, multisample, usage)
		{
			if (!SupportsFormat(depthFormat))
				throw new ArgumentException("Graphics adapter does not support the specified depth format");

			this.depthFormat = depthFormat;
			this.ownsDepth = true;

			if (this.depthFormat == null)
			{
				this.ClearBuffer.ClearDepthEnabled = false;
				this.ClearBuffer.ClearStencilEnabled = false;
			}
			else
			{
				if (this.depthFormat.Value != DepthFormat.Depth15Stencil1 &&
					this.depthFormat.Value != DepthFormat.Depth24Stencil4 &&
					this.depthFormat.Value != DepthFormat.Depth24Stencil8 &&
					this.depthFormat.Value != DepthFormat.Depth24Stencil8Single)
					this.ClearBuffer.ClearStencilEnabled = false;
			}


			SetHasDepth();
		}

		/// <summary>
		/// Share the depth buffer created by this draw texture with another draw texture
		/// </summary>
		/// <param name="shareDepthTo"></param>
		public void ShareDepthBuffer(DrawTargetTextureCube shareDepthTo)
		{
			if (shareDepthTo == null || shareDepthTo == this)
				throw new ArgumentNullException();
			if (shareDepthTo.cloneOf != null ||
				cloneOf != null)
				throw new ArgumentException("Cannot share to/from clones");
			if (shareDepthTo.depth != null)
				throw new ArgumentException("Shared depth target already has a created depth buffer");
			if (shareDepthTo.depthFormat != this.depthFormat)
				throw new ArgumentException("Depth format mismatch");
			if (shareDepthTo.sharedDepth != null)
				throw new ArgumentException("Shared depth target already uses a shared depth buffer");
			if (this.Width < shareDepthTo.Width ||
				this.Height < shareDepthTo.Height)
				throw new ArgumentException("Size mismatch");

			shareDepthTo.sharedDepth = sharedDepth ?? this;
		}

		/// <summary>
		/// Create a draw target cubemap directly from an XNA render target (not recommended)
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="renderTexture"></param>
		/// <returns></returns>
		public static DrawTargetTextureCube CreateFromRenderTargetCube(ICamera camera, RenderTargetCube renderTexture)
		{
			return new DrawTargetTextureCube(camera, renderTexture);
		}

		/// <summary>
		/// Create a draw target cubemap directly from an XNA render target and depth buffer (not recommended)
		/// </summary>
		/// <param name="camera"></param>
		/// <param name="renderTexture"></param>
		/// <param name="depthBuffer"></param>
		/// <returns></returns>
		public static DrawTargetTextureCube CreateFromRenderTargetCube(ICamera camera, RenderTargetCube renderTexture, DepthStencilBuffer depthBuffer)
		{
			return new DrawTargetTextureCube(camera, renderTexture, depthBuffer);
		}

		private DrawTargetTextureCube(ICamera camera, RenderTargetCube texture)
			: this(camera, texture, null)
		{
		}

		private DrawTargetTextureCube(ICamera camera, RenderTargetCube texture, DepthStencilBuffer depthBuffer)
			: base(camera)
		{
			if (texture == null)
				throw new ArgumentNullException();
			this.texture = texture;
			this.depth = depthBuffer;
			this.ownsDepth = false;
			if (depth != null)
				if (texture.Width != depth.Width || texture.Height != depth.Height)
					throw new ArgumentException("Depth/Texture Width/Height mismatch");

			this.resolution = texture.Width;

			this.sizeAsVector = new Vector2((float)this.resolution, (float)this.resolution);

			if (depthBuffer != null)
				this.depthFormat = depthBuffer.Format;

			SetHasDepth();
		}

		/// <summary>
		/// Dispose the draw target and all resources used. If this render target is a clone, the shared resources are NOT disposed
		/// </summary>
		public void Dispose()
		{
			isDisposed = true;

			if (cloneOf == null)
			{
				if (texture != null)
					texture.Dispose();
				if (depth != null)
					depth.Dispose();
			}
			this.texture = null;
			this.depth = null;
		}

		/// <summary></summary>
		/// <param name="state"></param>
		protected internal override void Begin(DrawState state)
		{
		}

		/// <summary></summary>
		/// <param name="state"></param>
		/// <param name="repeat"></param>
		/// <param name="camera"></param>
		/// <returns></returns>
		internal override bool BeginRepeat(DrawState state, int repeat, ref ICamera camera)
		{
			if (isDisposed)
				throw new ObjectDisposedException("this");

			GraphicsDevice device = state.graphics;

			if (!facesEnabled[repeat])
				return false;

#if XBOX360
			state.nonScreenRenderComplete = true;
#endif

			if (repeat == minFaceEnabled)
			{
				if (texture == null)
				{
					Warm(state);
				}
				if (this.depthFormat != null && this.depth == null && depthEnabled)
				{
					Warm(state);
				}

				if (texture.IsDisposed || (depth != null && depthEnabled && depth.IsDisposed))
					throw new ObjectDisposedException("RenderTexture");

				state.ResetTextures();
			}

			device.SetRenderTarget(0, texture, (CubeMapFace)repeat);
			if (repeat == minFaceEnabled)
				device.DepthStencilBuffer = depthEnabled ? depth : null;

			if (cubeCamera == null)
				cubeCamera = new Camera3D(new Projection(MathHelper.PiOver2,1,100,1));

			if (repeat == minFaceEnabled)
			{
				camera.GetCameraMatrix(out cubeCameraMatrix);

				if (camera is Camera3D)
				{
					this.cubeCamera.Projection.NearClip = ((Camera3D)camera).Projection.NearClip;
					this.cubeCamera.Projection.FarClip = ((Camera3D)camera).Projection.FarClip;
					this.cubeCamera.Projection.UseLeftHandedProjection = true;// ((Camera3D)camera).Projection.UseLeftHandedProjection;
					this.cubeCamera.ReverseBackfaceCulling = true;
				}
				else
				{
					this.cubeCamera.Projection.NearClip = 1;
					this.cubeCamera.Projection.FarClip = 100;
					this.cubeCamera.Projection.UseLeftHandedProjection = false;
				}
			}

			Matrix view;
			Matrix.Multiply(ref CubeMapFaceMatrices[repeat], ref cubeCameraMatrix, out view);
			this.cubeCamera.SetCameraMatrix(ref view);

			camera = this.cubeCamera;
			return true;
		}

		/// <summary></summary>
		/// <param name="state"></param>
		/// <param name="repeat"></param>
		internal override void EndRepeat(DrawState state, int repeat)
		{
			GraphicsDevice device = state.graphics;

			bool forceDownsample = true;
#if !XBOX360
			forceDownsample = this.texture.RenderTargetUsage != RenderTargetUsage.PlatformContents;
#endif
			if (forceDownsample)
				device.SetRenderTarget(0, null);

			if (repeat == maxFaceEnabled)
			{
				if (texture.IsDisposed || (depth != null && depth.IsDisposed))
					throw new ObjectDisposedException("RenderTexture");

				if (!forceDownsample)
					device.SetRenderTarget(0, null);

				if (mipmap)
					GetTexture().GenerateMipMaps(TextureFilter.Linear);
			}
		}

		/// <summary></summary>
		/// <param name="state"></param>
		protected internal override void End(DrawState state)
		{
		}

		/// <summary>
		/// Gets the width of the draw target cubemap
		/// </summary>
		public override int Width
		{
			get { return resolution; }
		}

		/// <summary>
		/// Gets the width of the draw target cubemap
		/// </summary>
		public override int Height
		{
			get { return resolution; }
		}

		/// <summary></summary>
		/// <param name="size"></param>
		/// <param name="ci"></param>
		/// <returns></returns>
		internal protected override bool GetWidthHeightAsVector(out Vector2 size, ref int ci)
		{
			size = sizeAsVector;
			if (ci != sizeIndex)
			{
				ci = sizeIndex;
				return true;
			}
			return false;
		}

		/// <summary></summary>
		protected internal override bool HasDepthBuffer
		{
			get { return hasDepth && depthEnabled; }
		}

		/// <summary></summary>
		protected internal override bool HasStencilBuffer
		{
			get { return hasStencil && depthEnabled; }
		}

		internal override int GetAllocatedDeviceBytes()
		{
			if (cloneOf != null)
				return 0;
			int bytes = 0;
			if (texture != null)
				bytes += FormatSize(this.format) * Width * Height * 6;
			if (depth != null && depthFormat != null)
			{
				int depthSize = 0;
				switch (depthFormat.Value)
				{
					case DepthFormat.Depth15Stencil1:
					case DepthFormat.Depth16:
						depthSize = 2;
						break;
					case DepthFormat.Depth24:
					case DepthFormat.Depth24Stencil4:
					case DepthFormat.Depth24Stencil8:
					case DepthFormat.Depth24Stencil8Single:
					case DepthFormat.Depth32:
						depthSize = 4;
						break;
				}
				bytes += depthSize * Width * Height;
			}
			return bytes;
		}

		internal override int GetAllocatedManagedBytes()
		{
			return 0;
		}

		internal override bool InUse
		{
			get { return texture != null; }
		}

		internal override bool IsDisposed
		{
			get
			{
				return isDisposed;
			}
		}

		internal override int GetRepeatCount()
		{
			return 6;
		}

		internal override void Warm(Application application,GraphicsDevice device)
		{
			if (cloneOf != null)
			{
				CloneRoot.Warm(application);
				this.texture = CloneRoot.texture;
				this.depth = CloneRoot.depth;
				return;
			}

			if (!ownerRegistered)
			{
				ownerRegistered = true;
				application.Content.AddHighPriority(this);
			}

			if (this.depthFormat != null && this.depth == null)
			{
				if (sharedDepth != null)
				{
					this.sharedDepth.Warm(application);
					this.depth = this.sharedDepth.depth;
				}
				else
					this.depth = new DepthStencilBuffer(device, resolution, resolution, depthFormat.Value, multisample, 0);
			}
			if (texture == null)
			{
				this.texture = new RenderTargetCube(device, resolution, mipmap ? 0 : 1, format, multisample, 0, usage);
				//xna bug forces this...
				device.SetRenderTarget(0, this.texture, CubeMapFace.NegativeX);
				device.SetRenderTarget(0, null);
			}
		}

		void IContentOwner.LoadContent(ContentRegister content, DrawState state, Microsoft.Xna.Framework.Content.ContentManager manager)
		{
			if (IsDisposed)
				return;

			GraphicsDevice device = state.graphics;

			if (texture != null &&
				texture.GraphicsDevice != device)
			{
				if (format != (SurfaceFormat)0 && cloneOf == null)
				{
					texture.Dispose();
					if (depth != null && ownsDepth)
						depth.Dispose();
				}
				texture = null;
				depth = null;
			}

			Warm(state);
		}

		void IContentOwner.UnloadContent(ContentRegister content, DrawState state)
		{
			this.renderTexture = null;
		}
	}
}
