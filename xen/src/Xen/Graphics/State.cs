using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Xen.Camera;
using Xen.Graphics.Modifier;
using Microsoft.Xna.Framework;

namespace Xen.Graphics.State
{

	/// <summary>
	/// Flags to represent aspects of the graphics render state that may be tracked by the application
	/// </summary>
	/// <remarks>
	/// <para>This flag is used to specify what render states has become 'dirty'</para>
	/// <para>Internally the application keeps track of what it thinks the render state is, to make changing state more efficient.</para>
	/// <para>By specifying that a render state group is dirty, the application ignores what it thinks the state is, and assumes it is entirely different.</para>
	/// <para>This forces the application to reapply all states the next time something is drawn.</para>
	/// </remarks>
	[Flags]
	public enum StateFlag
	{
		/// <summary>
		/// No state flag
		/// </summary>
		None = 0,
		/// <summary>
		/// AlphaBlending render states (<see cref="AlphaBlendState"/>)
		/// </summary>
		AlphaBlend = 1,
		/// <summary>
		/// Stencil testing render states (<see cref="StencilTestState"/>)
		/// </summary>
		StencilTest = 2,
		/// <summary>
		/// Alpha testing render states (<see cref="AlphaTestState"/>)
		/// </summary>
		AlphaTest = 4,
		/// <summary>
		/// Depth testing, Colour writed and face culling render states (<see cref="DepthColourCullState"/>)
		/// </summary>
		DepthColourCull = 8,
		/// <summary>
		/// Vertex and pixel shaders currently bound (XNA <see cref="Effect"/> objects also change vertex and pixel shaders)
		/// </summary>
		Shaders = 16,
		/// <summary>
		/// Pixel and vertex shader texture and sampler states
		/// </summary>
		Textures = 32,
		/// <summary>
		/// Currently bound vertex streams, vertex declaration and indices
		/// </summary>
		VerticesAndIndices = 64,
		/// <summary>
		/// Every state tracked by the application (Prefer using a combination of the other flags if possible)
		/// </summary>
		All = AlphaBlend | StencilTest | AlphaTest | DepthColourCull | Shaders | Textures | VerticesAndIndices
	}

	/// <summary>
	/// Exactly the same as <see cref="DeviceRenderState"/> except boxed as a class, not a structure. Use <see cref="DrawState.SetRenderState"/> or <see cref="DrawState.PushRenderState(ref DeviceRenderState)"/> to set the entire render state.
	/// </summary>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 16)]
	public sealed class DeviceRenderStateContainer
	{
		internal DeviceRenderStateContainer()
		{
		}

		[System.Runtime.InteropServices.FieldOffset(0)]
		internal DeviceRenderState state;
		/// <summary>
		/// Get/Set/Modify the alpha stencil test render states to be used during rendering
		/// </summary>
		[System.Runtime.InteropServices.FieldOffset(0)]
		public StencilTestState StencilTest;
		/// <summary>
		/// Get/Set/Modify the alpha blending render states to be used during rendering
		/// </summary>
		[System.Runtime.InteropServices.FieldOffset(8)]
		public AlphaBlendState AlphaBlend;
		/// <summary>
		/// Get/Set/Modify the alpha test render states to be used during rendering
		/// </summary>
		[System.Runtime.InteropServices.FieldOffset(12)]
		public AlphaTestState AlphaTest;
		/// <summary>
		/// Get/Set/Modify the depth test, colour write and face culling render states to be used during rendering
		/// </summary>
		[System.Runtime.InteropServices.FieldOffset(14)]
		public DepthColourCullState DepthColourCull;

		/// <summary>
		/// Cast to a <see cref="DeviceRenderState"/> implicitly
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static implicit operator DeviceRenderState(DeviceRenderStateContainer source)
		{
			return source.state;
		}
	}
	/// <summary>
	/// Stores the state of the most commonly used graphics render states. Packed into 16 bytes (the size of a Vector4)
	/// </summary>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 16)]
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct DeviceRenderState : IComparable<DeviceRenderState>
	{
		/// <summary>
		/// Construct a complete render state
		/// </summary>
		/// <param name="alphaBlendState"></param>
		/// <param name="alphaTestState"></param>
		/// <param name="depthState"></param>
		/// <param name="stencilState"></param>
		public DeviceRenderState(AlphaBlendState alphaBlendState, AlphaTestState alphaTestState, DepthColourCullState depthState, StencilTestState stencilState)
		{
			this.StencilTest = stencilState;
			this.AlphaBlend = alphaBlendState;
			this.AlphaTest = alphaTestState;
			this.DepthColourCull = depthState;
		}

		/// <summary>
		/// Get/Set/Modify the alpha stencil test render states to be used during rendering
		/// </summary>
		[System.Runtime.InteropServices.FieldOffset(0)]
		public StencilTestState StencilTest;
		/// <summary>
		/// Get/Set/Modify the alpha blending render states to be used during rendering
		/// </summary>
		[System.Runtime.InteropServices.FieldOffset(8)]
		public AlphaBlendState AlphaBlend;
		/// <summary>
		/// Get/Set/Modify the alpha test render states to be used during rendering
		/// </summary>
		[System.Runtime.InteropServices.FieldOffset(12)]
		public AlphaTestState AlphaTest;
		/// <summary>
		/// Get/Set/Modify the depth test, colour write and face culling render states to be used during rendering
		/// </summary>
		[System.Runtime.InteropServices.FieldOffset(14)]
		public DepthColourCullState	DepthColourCull;

		internal void ApplyState(ref DeviceRenderState current, GraphicsDevice device, bool reverseCull		
#if DEBUG
				,DrawState state
#endif
			)
		{
			if (current.StencilTest.mode != StencilTest.mode ||
				current.StencilTest.op != StencilTest.op)
			{
#if DEBUG
				if (StencilTest.ApplyState(ref current.StencilTest, device))
					System.Threading.Interlocked.Increment(ref state.Application.currentFrame.RenderStateStencilTestChangedCount);
#else
				StencilTest.ApplyState(ref current.StencilTest, device);
#endif
			}

			if (current.AlphaBlend.mode != AlphaBlend.mode)
			{				
#if DEBUG
				if (AlphaBlend.ApplyState(ref current.AlphaBlend, device))
					System.Threading.Interlocked.Increment(ref state.Application.currentFrame.RenderStateAlphaBlendChangedCount);
#else
				AlphaBlend.ApplyState(ref current.AlphaBlend, device);
#endif
			}

			if (current.AlphaTest.mode != AlphaTest.mode)
			{
#if DEBUG
				if (AlphaTest.ApplyState(ref current.AlphaTest, device))
					System.Threading.Interlocked.Increment(ref state.Application.currentFrame.RenderStateAlphaTestChangedCount);
#else
				AlphaTest.ApplyState(ref current.AlphaTest, device);
#endif
			}

			if (current.DepthColourCull.mode != DepthColourCull.mode)
			{
#if DEBUG
				if (DepthColourCull.ApplyState(ref current.DepthColourCull, device, reverseCull))
					System.Threading.Interlocked.Increment(ref state.Application.currentFrame.RenderStateDepthColourCullChangedCount);
#else
				DepthColourCull.ApplyState(ref current.DepthColourCull, device, reverseCull);
#endif
			}
		}

		internal void ResetState(StateFlag state, ref DeviceRenderState current, GraphicsDevice device, bool reverseCull)
		{
			if ((state & StateFlag.StencilTest) != 0)
				StencilTest.ResetState(ref current.StencilTest, device);
			if ((state & StateFlag.AlphaBlend) != 0)
				AlphaBlend.ResetState(ref current.AlphaBlend, device);
			if ((state & StateFlag.AlphaTest) != 0)
				AlphaTest.ResetState(ref current.AlphaTest, device);
			if ((state & StateFlag.DepthColourCull) != 0)
				DepthColourCull.ResetState(ref current.DepthColourCull, device, reverseCull);
		}

		/// <summary>
		/// Fast has code of all the render states
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return StencilTest.op ^ StencilTest.mode ^ AlphaBlend.mode ^ (AlphaTest.mode | (DepthColourCull.mode<<16));
		}

		/// <summary></summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is DeviceRenderState)
				return ((IComparable<DeviceRenderState>)this).CompareTo((DeviceRenderState)obj) == 0;
			if (obj is DeviceRenderStateContainer)
				return ((IComparable<DeviceRenderState>)this).CompareTo(((DeviceRenderStateContainer)obj).state) == 0;
			return false;
		}

		int IComparable<DeviceRenderState>.CompareTo(DeviceRenderState other)
		{
			if (AlphaBlend.mode > other.AlphaBlend.mode)
				return 1;
			if (AlphaBlend.mode < other.AlphaBlend.mode)
				return -1;
			if (DepthColourCull.mode > other.DepthColourCull.mode)
				return 1;
			if (DepthColourCull.mode < other.DepthColourCull.mode)
				return -1;
			if (AlphaTest.mode > other.AlphaTest.mode)
				return 1;
			if (AlphaTest.mode < other.AlphaTest.mode)
				return -1;
			if (StencilTest.mode > other.StencilTest.mode)
				return 1;
			if (StencilTest.mode < other.StencilTest.mode)
				return -1;
			if (StencilTest.op > other.StencilTest.op)
				return 1;
			if (StencilTest.op < other.StencilTest.op)
				return -1;
			return 0;
		}
	}
	
#if DEBUG
	static class BitWiseTypeValidator
	{
		public static void Validate<T>() where T : new()
		{
			object instance = new T();
			System.Reflection.PropertyInfo[] props = instance.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			object[] defaults = new object[props.Length];
			for (int i = 0; i < props.Length; i++)
			{
				if (props[i].CanRead && props[i].CanWrite)
					defaults[i] = props[i].GetValue(instance, null);
			}


			for (int i = 0; i < props.Length; i++)
			{
				if (!props[i].CanRead || !props[i].CanWrite)
					continue;

				System.Collections.ArrayList values = new System.Collections.ArrayList();
				if (typeof(Enum).IsAssignableFrom(props[i].PropertyType))
				{
					System.Reflection.FieldInfo[] enums = props[i].PropertyType.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
					foreach (System.Reflection.FieldInfo field in enums)
					{
						values.Add(field.GetValue(null));
					}
				}
				else
				{
					if (typeof(bool) == props[i].PropertyType)
					{
						values.AddRange(new object[] { true, false });
					}
					else
					{
						if (typeof(byte) == props[i].PropertyType)
						{
							for (int b = 0; b < 256; b++)
								values.Add((byte)b);
						}
						else
						{
							if (typeof(int) == props[i].PropertyType)
								continue;
							throw new ArgumentException();
						}
					}
				}

				foreach (object value in values)
				{
					props[i].SetValue(instance, value, null);

					if (value.Equals(TextureFilter.PyramidalQuad) ||
						value.Equals(TextureFilter.GaussianQuad) ||
						(value.Equals(TextureFilter.Anisotropic) && props[i].Name == "MagFilter") ||
						(value.Equals(TextureFilter.Anisotropic) && props[i].Name == "MipFilter"))
						continue;//special cases :-)

					if (props[i].GetValue(instance, null).Equals(value) == false)
						throw new ArgumentException();

					for (int p = 0; p < props.Length; p++)
					{
						if (!props[p].CanRead || !props[p].CanWrite)
							continue;
						if (p != i)
						{
							if (props[p].GetValue(instance, null).Equals(defaults[p]) == false)
								throw new ArgumentException();
						}
					}
				}

				props[i].SetValue(instance, defaults[i], null);
			}
		}
	}
#endif

	/// <summary>
	/// Packed representation of common Alpha Blending states. 4 bytes
	/// </summary>
	/// <remarks>
	/// <para>When a pixel is drawn, it usually overwrites the existing pixel.</para>
	/// <para>For example, if you draw a quad in the centre of the screen, drawing that quad draws over the existing pixels on screen. Usually before drawing the quad, you would clear the screen first. If the screen is cleared to black, and the quad is white, the existing black colour values are overwritten with white colour values.</para>
	/// <para>The existing pixels are known as the 'destination' pixels, and the pixels being written are known as 'source' pixels.</para>
	/// <para>In the example, the desination pixels are black colour values, while the source pixels are white colour values.</para>
	/// <code>
	/// //destination values are written
	/// destination.rgba = source.rgba;
	/// </code>
	/// <para>When alpha blending is enabled, the value that is written is the result of an equation.</para>
	/// <para>A common alpha blending operation is 'additive' blending, where the source and destination colours are added together (this is commonly used in particle effects such as light flares and fire, as it 'adds' light, and brightens the on screen image (the destination pixels)) :</para>
	/// <code>
	/// //additive blending
	/// destination.rgba = destination.rgba + source.rgba;
	/// </code>
	/// <para>In such a case, the pixels already on screen (destination) will mostly stay visible, with the pixels being drawn (source) added to them</para>
	/// <para></para>
	/// <para>To set additive blending, the following states should be set:</para>
	/// <code>
	/// 	AlphaBlendState state = new AlphaBlendState();
	///
	///		state.BlendOperation   = BlendFunction.Add;		//This is the default value for BlendOperation
	///		state.DestinationBlend = Blend.One;				//This is the default value for DestinationBlend
	///		state.SourceBlend      = Blend.One;				//This is the default value for SourceBlend
	/// 
	///		state.Enabled = true;							//This is not the default value (the default is false)
	/// 
	/// </code>
	/// <para>What does this mean? Alpha blending can be more complex than simple additive blending. To understand, you need to know the complete blending equation.</para>
	/// <para>The actual blending equation is:</para>
	/// <code>
	/// destination.rgba = (destination.rgba * DestinationBlend) BlendOperation (source.rgba * SourceBlend);
	/// </code>
	/// <para>As can be seen, it's more complex. With the settings used above above, the equation becomes:</para>
	/// <code>
	/// //additive blending
	/// destination.rgba = (destination.rgba * Blend.One) BlendFunction.Add (source.rgba * Blend.One);
	/// 
	/// //which is...
	/// destination.rgba = (destination.rgba * 1) + (source.rgba * 1);
	/// 
	/// //simplified...
	/// destination.rgba = destination.rgba + source.rgba;
	/// </code>
	/// <para>The blend operations and modes control only the mathematical operations used in the equation.</para>
	/// <para>This simple blending mode works well in a lot of cases, but you can quickly add too much colour. Most render targets (and the screen) are low precision, and cannot store r/g/b/a values greater than 1.0, so addtive blending can quickly lead to 'blown out' effects that clamp to full white (RGB = 1,1,1)</para>
	/// <para></para>
	/// <para>An example of a more complex blending mode is Alpha Blending. Not to be confused by the name, 'Alpha Blending' as a blend state and 'Alpha Blending' as a device render state are different things. For clarity, I'll call it 'SourceAlpha blending'</para>
	/// <para>With SourceAlpha blending, the source pixels store an alpha value that represents how transparent they are. Eg, the texture for leaves on a tree will store an alpha value of 1.0 on the leaf pixels, and a value of 0.0 around them in the 'transparent' pixels.</para>
	/// <para>To achieve SourceAlpha blending, the blend equation needs to perform a linear interpolation (fade) from the destination (background) to the source (tree leaves). </para>
	/// <para>A simple linear interpolation is...</para>
	/// <code>
	/// finalColour = treeColour * treeAlpha + backgroundColour * (1-treeAlpha)
	/// </code>
	/// <para>Or in blend states:</para>
	/// <code>
	/// destination.rgba = source.rgba * source.a + destination.rgba * (1-source.a);
	/// </code>
	/// <para>Converted to a AlphaBlendState object, this becomes:</para>
	/// <code>
	/// 	AlphaBlendState state = new AlphaBlendState();
	///
	///		state.BlendOperation   = BlendFunction.Add;	
	///		state.DestinationBlend = Blend.InverseSourceAlpha;				
	///		state.SourceBlend      = Blend.SourceAlpha;		
	/// 
	///		state.Enabled = true;	
	/// 
	/// </code>
	/// <para>Note that 'InverseSourceAlpha' really means 'OneMinusSourceAlpha'. This is important if you use high precision render targets that can store values above 1.0!</para>
	/// <para></para>
	/// <para>Also, For advanced use, <see cref="SeparateAlphaBlendEnabled"/> can be set to true, doing so allows defining a separate equation just for the alpha channel, like so:</para>
	/// <code>
	/// destination.rgb = (destination.rgb * DestinationBlend) BlendOperation (source.rgb * SourceBlend);
	/// destination.a = (destination.a * DestinationBlendAlpha) BlendOperationAlpha (source.a * SourceBlendAlpha);
	/// </code>
	/// <para></para>
	/// <para>Note: If the blend mode modifies the destination (<see cref="DestinationBlend"/> is anything other than <see cref="Blend.One"/>), then drawing pixels in different orders will produce different results. This is especially true if depth writing is still enabled, as drawing the pixel does not completely overwrite the previous colour. For example, if multiple trees are drawn, potentially one on top of the other, then it is best to draw the trees in order from back to front. </para>
	/// </remarks>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 4)]
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct AlphaBlendState
	{
		[System.Runtime.InteropServices.FieldOffset(0)]
		internal int mode;

		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(AlphaBlendState a, AlphaBlendState b)
		{
			return a.mode == b.mode;
		}
		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(AlphaBlendState a, AlphaBlendState b)
		{
			return a.mode != b.mode;
		}
		/// <summary></summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is AlphaBlendState)
				return ((AlphaBlendState)obj).mode == this.mode;
			return base.Equals(obj);
		}
		/// <summary>
		/// Gets the hash code, eqivalent to the internal bitfield
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return mode;
		}

		private static readonly AlphaBlendState _None = new AlphaBlendState();
		private static readonly AlphaBlendState _Alpha = new AlphaBlendState(Blend.SourceAlpha, Blend.InverseSourceAlpha);
		private static readonly AlphaBlendState _PremodulatedAlpha = new AlphaBlendState(Blend.One, Blend.InverseSourceAlpha);
		private static readonly AlphaBlendState _AlphaAdditive = new AlphaBlendState(Blend.SourceAlpha, Blend.One);
		private static readonly AlphaBlendState _Additive = new AlphaBlendState(Blend.One, Blend.One);
		private static readonly AlphaBlendState _AdditiveSaturate = new AlphaBlendState(Blend.InverseDestinationColor, Blend.One);
		private static readonly AlphaBlendState _Modulate = new AlphaBlendState(Blend.DestinationColor, Blend.Zero);
		private static readonly AlphaBlendState _ModulateAdd = new AlphaBlendState(Blend.DestinationColor, Blend.One);
		private static readonly AlphaBlendState _ModulateX2 = new AlphaBlendState(Blend.DestinationColor, Blend.SourceColor);

		/// <summary>State that disables Alpha Blending</summary>
		public static AlphaBlendState None { get { return _None; } }
		/// <summary>State that enables standard Alpha Blending (blending based on the alpha value of the source component, desitination colour is interpolated to the source colour based on source alpha)</summary>
		public static AlphaBlendState Alpha { get { return _Alpha; } }
		/// <summary>State that enables Premodulated Alpha Blending (Assumes the source colour data has been premodulated with the inverse of the alpha value, useful for reducing colour bleeding and accuracy problems at alpha edges)</summary>
		public static AlphaBlendState PremodulatedAlpha { get { return _PremodulatedAlpha; } }
		/// <summary>State that enables Additive Alpha Blending (blending based on the alpha value of the source component, the desitination colour is added to the source colour modulated by alpha)</summary>
		public static AlphaBlendState AlphaAdditive { get { return _AlphaAdditive; } }
		/// <summary>State that enables standard Additive Blending (the alpha value is ignored, the desitination colour is added to the source colour)</summary>
		public static AlphaBlendState Additive { get { return _Additive; } }
		/// <summary>State that enables Additive Saturate Blending (the alpha value is ignored, the desitination colour is added to the source colour, however the source colour is multipled by the inverse of the destination colour, preventing the addition from blowing out to pure white (eg, 0.75 + 0.75 * (1-0.75) = 0.9375))</summary>
		public static AlphaBlendState AdditiveSaturate { get { return _AdditiveSaturate; } }
		/// <summary>State that enables Modulate (multiply) Blending (the alpha value is ignored, the desitination colour is multipled with the source colour)</summary>
		public static AlphaBlendState Modulate { get { return _Modulate; } }
		/// <summary>State that enables Modulate Add (multiply+add) Blending (the alpha value is ignored, the desitination colour multipled with the source colour is added to the desitnation colour)</summary>
		public static AlphaBlendState ModulateAdd { get { return _ModulateAdd; } }
		/// <summary>State that enables Modulate (multiply) Blending, scaled by 2 (the alpha value is ignored, the desitination colour is multipled with the source colour, scaled by two)</summary>
		public static AlphaBlendState ModulateX2 { get { return _ModulateX2; } }

		/// <summary>Set the render state to no Alpha Blending, resetting all states (This is not equivalent to setting <see cref="Enabled"/> to false, however it has the same effect)</summary>
		public void SetToNoBlending() { this.mode = 0; }
		/// <summary>Set the render state to standard Alpha Blending (blending based on the alpha value of the source component, desitination colour is interpolated to the source colour based on source alpha)</summary>
		public void SetToAlphaBlending() { this.mode = _Alpha.mode; }
		/// <summary>Set the render state to Additive Alpha Blending (blending based on the alpha value of the source component, the desitination colour is added to the source colour modulated by alpha)</summary>
		public void SetToAdditiveBlending() { this.mode = _Additive.mode; }
		/// <summary>Set the render state to Premodulated Alpha Blending (Assumes the source colour data has been premodulated with the inverse of the alpha value, useful for reducing colour bleeding and accuracy problems at alpha edges)</summary>
		public void SetToPremodulatedAlphaBlending() { this.mode = _PremodulatedAlpha.mode; }
		/// <summary>Set the render state to Additive Alpha Blending (blending based on the alpha value of the source component, the desitination colour is added to the source colour modulated by alpha)</summary>
		public void SetToAlphaAdditiveBlending() { this.mode = _AlphaAdditive.mode; }
		/// <summary>Set the render state to Additive Saturate Blending (the alpha value is ignored, the desitination colour is added to the source colour, however the source colour is multipled by the inverse of the destination colour, preventing the addition from blowing out to pure white (eg, 0.75 + 0.75 * (1-0.75) = 0.9375))</summary>
		public void SetToAdditiveSaturateBlending() { this.mode = _AdditiveSaturate.mode; }
		/// <summary>Set the render state to Modulate (multiply) Blending (the alpha value is ignored, the desitination colour is multipled with the source colour)</summary>
		public void SetToModulateBlending() { this.mode = _Modulate.mode; }
		/// <summary>Set the render state to Modulate Add (multiply+add) Blending (the alpha value is ignored, the desitination colour multipled with the source colour is added to the desitnation colour)</summary>
		public void SetToModulateAddBlending() { this.mode = _ModulateAdd.mode; }
		/// <summary>Set the render state to Modulate (multiply) Blending, scaled by 2 (the alpha value is ignored, the desitination colour is multipled with the source colour, scaled by two)</summary>
		public void SetToModulateX2Blending() { this.mode = _ModulateX2.mode; }

		/// <summary>
		/// Create a alpha blend state with the given source and destination blend modes
		/// </summary>
		/// <param name="sourceBlend"></param>
		/// <param name="destinationBlend"></param>
		public AlphaBlendState(Blend sourceBlend, Blend destinationBlend)
		{
			mode = 1;
			this.SourceBlend = sourceBlend;
			this.DestinationBlend = destinationBlend;
		}

#if DEBUG
		static AlphaBlendState()
		{
			BitWiseTypeValidator.Validate<AlphaBlendState>();
		}
#endif
		/// <summary>
		/// Gets/Sets if alpha blending is enabled
		/// </summary>
		public bool Enabled
		{
			get { return (mode & 1) == 1; }
			set { mode = (mode & ~1) | (value ? 1 : 0); }
		}

		/// <summary>
		/// Gets/Sets if separate alpha blending is enabled (Separate alpha blending applies an alternative blend equation to the alpha channel than the RGB channels). See <see cref="BlendOperationAlpha"/>, <see cref="SourceBlendAlpha"/> and <see cref="DestinationBlendAlpha"/>
		/// </summary>
		public bool SeparateAlphaBlendEnabled
		{
			get { return (mode & 2) == 2; }
			set { mode = (mode & ~2) | (value ? 2 : 0); }
		}

		/// <summary>
		/// Gets/Sets the blending function operation. See <see cref="AlphaBlendState"/> remarks for details
		/// </summary>
		public BlendFunction BlendOperation
		{
			get
			{
				//1-5
				return (BlendFunction)(((mode>>2) & 7) + 1);
			}
			set
			{
				mode = (mode & ~(7<<2)) | (7 & ((int)value - 1)) << 2;
			}
		}

		/// <summary>
		/// Gets/Sets the blending function operation, this value only effects the alpha channel and only when <see cref="SeparateAlphaBlendEnabled"/> is true. See <see cref="AlphaBlendState"/> remarks for details
		/// </summary>
		public BlendFunction BlendOperationAlpha
		{
			get
			{
				return (BlendFunction)(((mode>>5) & 7) + 1);
			}
			set
			{
				mode = (mode & ~(7<<5)) | (7 & ((int)value - 1)) << 5;
			}
		}

		/// <summary>
		/// Gets/Sets the blending function source (drawn pixel) input multiply value. See <see cref="AlphaBlendState"/> remarks for details
		/// </summary>
		public Blend SourceBlend
		{
			get
			{
				return (Blend)(((mode >> 8) & 15) + 1);
			}
			set
			{
				mode = (mode & ~(15 << 8)) | (15 & ((int)value - 1)) << 8;
			}
		}

		/// <summary>
		/// Gets/Sets the blending function destination (existing pixel) input multiply value. See <see cref="AlphaBlendState"/> remarks for details
		/// </summary>
		public Blend DestinationBlend
		{
			get
			{
				return (Blend)(((mode >> 12) & 15) + 1);
			}
			set
			{
				mode = (mode & ~(15 << 12)) | (15 & ((int)value - 1)) << 12;
			}
		}

		/// <summary>
		/// Gets/Sets the blending function source (drawn pixel) input multiply value, this value only effects the alpha channel and only when <see cref="SeparateAlphaBlendEnabled"/> is true. See <see cref="AlphaBlendState"/> remarks for details
		/// </summary>
		public Blend SourceBlendAlpha
		{
			get
			{
				return (Blend)(((mode >> 16) & 15) + 1);
			}
			set
			{
				mode = (mode & ~(15 << 16)) | (15 & ((int)value - 1)) << 16;
			}
		}

		/// <summary>
		/// Gets/Sets the blending function destination (existing pixel) input multiply value, this value only effects the alpha channel and only when <see cref="SeparateAlphaBlendEnabled"/> is true. See <see cref="AlphaBlendState"/> remarks for details
		/// </summary>
		public Blend DestinationBlendAlpha
		{
			get
			{
				return (Blend)(((mode >> 20) & 15) + 1);
			}
			set
			{
				mode = (mode & ~(15 << 20)) | (15 & ((int)value - 1)) << 20;
			}
		}

		internal void ResetState(ref AlphaBlendState current, GraphicsDevice device)
		{
			device.RenderState.AlphaBlendEnable = this.Enabled;
			device.RenderState.SeparateAlphaBlendEnabled = this.SeparateAlphaBlendEnabled;
			device.RenderState.BlendFunction = this.BlendOperation;
			device.RenderState.AlphaBlendOperation = current.BlendOperationAlpha;
			device.RenderState.SourceBlend = this.SourceBlend;
			device.RenderState.DestinationBlend = this.DestinationBlend;
			device.RenderState.AlphaSourceBlend = this.SourceBlendAlpha;
			device.RenderState.AlphaDestinationBlend = this.DestinationBlendAlpha;

			current.mode = this.mode;
		}

		internal bool ApplyState(ref AlphaBlendState current, GraphicsDevice device)
		{
			bool changed = false;

			if (this.Enabled)
			{
#if DEBUG
				if (mode != current.mode)
					changed = true;
#endif
				if (!current.Enabled)
					device.RenderState.AlphaBlendEnable = true;

				if (this.SeparateAlphaBlendEnabled != current.SeparateAlphaBlendEnabled)
					device.RenderState.SeparateAlphaBlendEnabled = this.SeparateAlphaBlendEnabled;

				if (this.BlendOperation != current.BlendOperation)
					device.RenderState.BlendFunction = this.BlendOperation;

				if (this.BlendOperationAlpha != current.BlendOperationAlpha)
					device.RenderState.AlphaBlendOperation = current.BlendOperationAlpha;

				if (this.SourceBlend != current.SourceBlend)
					device.RenderState.SourceBlend = this.SourceBlend;

				if (this.DestinationBlend != current.DestinationBlend)
					device.RenderState.DestinationBlend = this.DestinationBlend;

				if (this.SourceBlendAlpha != current.SourceBlendAlpha)
					device.RenderState.AlphaSourceBlend = this.SourceBlendAlpha;

				if (this.DestinationBlendAlpha != current.DestinationBlendAlpha)
					device.RenderState.AlphaDestinationBlend = this.DestinationBlendAlpha;

				current.mode = this.mode;
			}
			else
			{
				if (current.Enabled)
				{
					device.RenderState.AlphaBlendEnable = false;
					current.Enabled = false;
#if DEBUG
					changed  =true;
#endif
				}
			}
			return changed;
		}
	}

	/// <summary>
	/// Packed representation of Alpha Testing render state. 2 bytes
	/// </summary>
	/// <remarks>
	/// <para>Alpha testing is an pixel culling operation that occurs after the pixel shader and before the pixel gets written to the render target</para>
	/// <para>The alpha test function is applied to the alpha value output by the pixel shader. If the test is enabled and the alpha test comparison to the reference alpha value <i>fails</i>, then the pixel will be rejected and not written/blended into the render target</para>
	/// <para>For example, setting <see cref="Enabled"/> to true, <see cref="AlphaTestFunction"/> to <see cref="CompareFunction.Greater"/> and <see cref="ReferenceAlpha"/> to 128 will mean only pixels with an alpha of greater than 128 (0.5) will be drawn.</para>
	/// <para>Alpha testing, on windows, can be a lot faster than alpha blending (alpha blending is free on xbox360). In such a case it can be a good performance/looks tradeoff for semi-transparent effects such as grass and leaves. If blending is not involved, it may not have depth-ordering visual problems.</para>
	/// </remarks>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 2)]
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct AlphaTestState
	{
		[System.Runtime.InteropServices.FieldOffset(0)]
		internal ushort mode;

#if DEBUG
		static AlphaTestState()
		{
			BitWiseTypeValidator.Validate<AlphaTestState>();
		}
#endif
		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(AlphaTestState a, AlphaTestState b)
		{
			return a.mode == b.mode;
		}
		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(AlphaTestState a, AlphaTestState b)
		{
			return a.mode != b.mode;
		}
		/// <summary></summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is AlphaTestState)
				return ((AlphaTestState)obj).mode == this.mode;
			return base.Equals(obj);
		}
		/// <summary>
		/// Gets the hash code (direct copy of the internal bitfield value)
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return mode;
		}

		/// <summary>
		/// Gets/Sets if alpha testing is enabled
		/// </summary>
		public bool Enabled
		{
			get { return (mode & 1) == 1; }
			set { mode = (ushort)((mode & ~1) | (value ? 1 : 0)); }
		}

		/// <summary>
		/// Gets/Sets the alpha testing comparison function
		/// </summary>
		public CompareFunction AlphaTestFunction
		{
			get
			{
				return (CompareFunction)((~((mode >> 1)) & 7) + 1);
			}
			set
			{
				mode = (ushort)(((int)mode & ~(7 << 1)) | (7 & (~((int)value - 1))) << 1);
			}
		}

		/// <summary>
		/// Gets/Sets the reference value used in the alpha testing comparison
		/// </summary>
		public byte ReferenceAlpha
		{
			get
			{
				return (byte)(((mode & (255 << 8)) >> 8));
			}
			set
			{
				mode = (ushort)((mode & ~(255 << 8)) | (((int)value) << 8));
			}
		}


		internal void ResetState(ref AlphaTestState current, GraphicsDevice device)
		{
			device.RenderState.AlphaTestEnable = this.Enabled;
			device.RenderState.AlphaFunction = this.AlphaTestFunction;
			device.RenderState.ReferenceAlpha = this.ReferenceAlpha;

			current.mode = this.mode;
		}

		internal bool ApplyState(ref AlphaTestState current, GraphicsDevice device)
		{
			bool changed = false;
			if (this.Enabled)
			{
				
#if DEBUG
				changed = this.mode != current.mode;
#endif

				if (!current.Enabled)
					device.RenderState.AlphaBlendEnable = true;

				if (this.AlphaTestFunction != current.AlphaTestFunction)
					device.RenderState.AlphaFunction = this.AlphaTestFunction;

				if (this.ReferenceAlpha != current.ReferenceAlpha)
					device.RenderState.ReferenceAlpha = this.ReferenceAlpha;

				current.mode = this.mode;
			}
			else
			{
				if (current.Enabled)
				{				
#if DEBUG
					changed = true;
#endif
					device.RenderState.AlphaTestEnable = false;
					current.Enabled = false;
				}
			}
			return changed;
		}
	}

	/// <summary>
	/// Packed representation of Depth Testing, Colour buffer masking and FrustumCull mode states. 2 bytes
	/// </summary>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 2)]
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct DepthColourCullState
	{
		[System.Runtime.InteropServices.FieldOffset(0)]
		internal ushort mode;

#if DEBUG
		static DepthColourCullState()
		{
			BitWiseTypeValidator.Validate<DepthColourCullState>();
		}
#endif

		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(DepthColourCullState a, DepthColourCullState b)
		{
			return a.mode == b.mode;
		}
		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(DepthColourCullState a, DepthColourCullState b)
		{
			return a.mode != b.mode;
		}
		/// <summary></summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is DepthColourCullState)
				return ((DepthColourCullState)obj).mode == this.mode;
			return base.Equals(obj);
		}
		/// <summary>
		/// Gets the hash code. Returned value is the internal bitfield value
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return mode;
		}

		/// <summary>
		/// Gets/Sets if depth testing is enabled
		/// </summary>
		/// <remarks><para>If depth testing is disabled, then pixels will always be drawn, even if they are behind another object on screen.</para></remarks>
		public bool DepthTestEnabled
		{
			get { return (mode & 1) != 1; }
			set { mode = (ushort)((mode & ~1) | (value ? 0 : 1)); }
		}

		/// <summary>
		/// Gets/Sets if depth writing is enabled
		/// </summary>
		/// <remarks><para>If depth writing is disabled, pixels that are drawn will still go through normal depth testing, however they will not write a new depth value into the depth buffer. This is most useful for transparent effects, and any effect that does not have a physical representation (eg, a light cone, particle effects, etc).</para>
		/// <para>Usually, 'solid' objects with depth writing enabled will be drawn first. Such as the world, characters, models, etc. Then non-solid and effect geometry is drawn without depth writing. If this order is reversed, the solid geometry can overwrite the effects.</para></remarks>
		public bool DepthWriteEnabled
		{
			get { return (mode & 2) != 2; }
			set { mode = (ushort)((mode & ~2) | (value ? 0 : 2)); }
		}

		/// <summary>
		/// Changes the comparsion function used when depth testing. WARNING:  On some video cards, changing this value can disable hierarchical z-buffer optimizations for the rest of the frame
		/// </summary>
		/// <remarks>
		/// <para>Changing the depth test function from Less to Greater midframe is <i>not recommended</i>.</para>
		/// <para>Changing between <see cref="CompareFunction.LessEqual"/> and <see cref="CompareFunction.Equal"/> is usually OK.</para>
		/// <para>On newer video cards, keeping the depth test function consistent throughout the frame will still maintain peek effciency, however some older cards are only full speed when using <see cref="CompareFunction.LessEqual"/> or <see cref="CompareFunction.Equal"/></para>
		/// <para>Setting <see cref="DepthTestEnabled"/> to false is the preferred to using <see cref="CompareFunction.Always"/>.</para>
		/// </remarks>
		public CompareFunction DepthTestFunction
		{
			get
			{
				return (CompareFunction)((((mode >> 2) & 15) ^ 4));
			}
			set
			{
				mode = (ushort)((mode & ~(15 << 2)) | (15 & (((int)value)) ^ 4) << 2);
			}
		}

		/// <summary>
		/// Gets/Sets the backface culling render state. Default value of <see cref="Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace">CullCounterClockwiseFace</see>
		/// </summary>
		public CullMode CullMode
		{
			get
			{
				return (CullMode)(((((mode >> 6) & 3)^2))+1);
			}
			set
			{
				mode = (ushort)((mode & ~(3 << 6)) | (3 & ((((int)value)) - 1)^2) << 6);
			}
		}

		internal CullMode GetCullMode(bool reverseCull)
		{
			CullMode mode = (CullMode)(((((this.mode >> 6) & 3) ^ 2)) + 1);
			if (reverseCull)
			{
				switch (mode)
				{
					case  CullMode.CullClockwiseFace:
						return CullMode.CullCounterClockwiseFace;
					case CullMode.CullCounterClockwiseFace:
						return CullMode.CullClockwiseFace;
				}
			}
			return mode;
		}

		/// <summary>
		/// Gets/Sets a mask for the colour channels (RGBA) that are written to the colour buffer. Set to <see cref="ColorWriteChannels.None"/> to disable all writing to the colour buffer.
		/// </summary>
		public ColorWriteChannels ColourWriteMask
		{
			get
			{
				return (ColorWriteChannels)(((~(mode >> 8)) & 15));
			}
			set
			{
				mode = (ushort)((mode & ~(15 << 8)) | (15 & (~((int)value))) << 8);
			}
		}

		internal void ResetState(ref DepthColourCullState current, GraphicsDevice device, bool reverseCull)
		{
			device.RenderState.DepthBufferEnable = this.DepthTestEnabled;
			device.RenderState.DepthBufferWriteEnable = this.DepthWriteEnabled;
			device.RenderState.DepthBufferFunction = this.DepthTestFunction;
			device.RenderState.CullMode = this.GetCullMode(reverseCull);
			device.RenderState.ColorWriteChannels = this.ColourWriteMask;

			current.mode = this.mode;
		}

		internal bool ApplyState(ref DepthColourCullState current, GraphicsDevice device, bool reverseCull)
		{
			bool changed = false;

			if ((current.mode & (~63)) != (mode & (~63)))
			{
#if DEBUG
				changed = true;
#endif
				CullMode cull = this.CullMode;
				if (cull != current.CullMode)
				{
					device.RenderState.CullMode = GetCullMode(reverseCull);
					current.CullMode = cull;
				}
				ColorWriteChannels channels = this.ColourWriteMask;
				if (channels != current.ColourWriteMask)
				{
					device.RenderState.ColorWriteChannels = channels;
					current.ColourWriteMask = channels;
				}
			}

			if (this.DepthTestEnabled)
			{
				if (!current.DepthTestEnabled)
				{
#if DEBUG
					changed = true;
#endif
					device.RenderState.DepthBufferEnable = true;
				}

				if (this.DepthWriteEnabled != current.DepthWriteEnabled)
				{
#if DEBUG
					changed = true;
#endif
					device.RenderState.DepthBufferWriteEnable = this.DepthWriteEnabled;
				}

				if (this.DepthTestFunction != current.DepthTestFunction)
				{
#if DEBUG
					changed = true;
#endif
					device.RenderState.DepthBufferFunction = this.DepthTestFunction;
				}

				current.mode = this.mode;
			}
			else
			{
				if (current.DepthTestEnabled)
				{
#if DEBUG
					changed = true;
#endif
					device.RenderState.DepthBufferEnable = false;
					current.DepthTestEnabled = false;
				}
			}
			return changed;
		}
	}

	//1 bit larger than 6 bytes.. :-(
	/// <summary>
	/// Packed representation of Stencil Testing state. 8 bytes
	/// </summary>
	/// <remarks>
	/// <para>On most systems, the depth buffer is either 32 or 16 bits in size. However, with a 32bit size depth buffer, the accuracy is usually only 24bits. (True 32bit depth buffers are not supported on any DX9 video cards)</para>
	/// <para>When the depth is 24bits, the remaining 8bits can be used as the stencil buffer (see <see cref="DepthFormat.Depth24Stencil8"/> - ignore <see cref="DepthFormat.Depth24Stencil8Single"/>, as practically no DX9 cards support it).</para>
	/// <para>In this case the stencil buffer is an 8bit integer format, (similar to a single colour in 32bit RGBA). Values range from 0 to 255.</para>
	/// <para>The difference with a stencil buffer is that operations performed on it are based almost entirely on bitmasks, increments/decrements and swapping values. It acts in a similar way to a .net <see cref="Byte"/>.</para>
	/// <para></para>
	/// <para>The name 'stencil testing' is somewhat misleading as it involves two operations, stencil reading and stencil writing.</para>
	/// <para>When stencil testing is enabled, both of these are enabled, however the default comparisons and write maskes/options all have no effect.</para>
	/// <para>For example, to 'mask' an area on screen (eg, to draw into a circle, and only a circle) then then stencil testing can be used in two passes:</para>
	/// <para>Assuming the stencil buffer is cleared to zero, the circle can be drawn first.</para>
	/// <para>With stencil testing enabled, the circile is drawn. The stencil reference value (<see cref="ReferenceValue"/>) is set to 1. The <see cref="StencilPassOperation"/> is set to <see cref="StencilOperation.Replace"/>, which means the value in the stencil buffer (when the pixel is drawn) will be <i>replaced</i> with the reference value (which is 1). Setting <see cref="DepthColourCullState.ColourWriteMask"/> to None may be desired to make the circle not visible.</para>
	/// <para>Next, the scene inside the circle is drawn. With stencil testing enabled, the reference is set to 1 again and the <see cref="StencilFunction"/> is set to <see cref="CompareFunction.Equal"/>. This way, the stencil function will only pass when the stencil value in the stencil buffer is <i>equal</i> to the <i>reference value</i> of 1. This way nothing outside the circle is drawn.</para>
	/// <para>Note that the ciricle will still be drawn into the stencil buffer at this stage.</para>
	/// <para></para>
	/// <para>Stencil testing has more complex features. The most significant, is that the stencil value can be modified in different ways for three cases.</para>
	/// <para>These cases are: When the stencil function passes, the stencil function passes but Z-testing fails and finally when the stencil function fails.</para>
	/// <para>Finally, stencil testing can be configured independantly for backfacing triangles (by setting <see cref="TwoSidedStencilModeEnabled"/> to true). (Note that <see cref="DepthColourCullState.CullMode"/> must also be set to None for this to take effect).</para>
	/// <para></para>
	/// <para>Stencil testing can be used for some very complex effects, such as stencil shadows.</para>
	/// <para>An example is determining how many pixels are within a <i>closed convex</i> volume (such as a sphere or cube) that does not intersect the near or far clip plane. Assuming a stencil buffer cleared to zero:</para>
	/// <para>In such a case, drawing the volume (with colour writes <i>and</i> depth writes disabled) and setting the stencil pass operation to <see cref="StencilOperation.Increment"/> and the stencil <i>backface z-fail</i> stencil operation to <see cref="StencilOperation.Decrement"/> will leave the stencil buffer unchanged for all pixels that are within the volume.</para>
	/// <para>Drawing the volume again, with stencil function set to Equal to a reference value of 0, only those existing pixels within the volume will be drawn.</para>
	/// </remarks>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 8)]
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct StencilTestState
	{
		[System.Runtime.InteropServices.FieldOffset(0)]
		internal int op;
		[System.Runtime.InteropServices.FieldOffset(4)]
		internal int mode;

#if DEBUG
		static StencilTestState()
		{
			BitWiseTypeValidator.Validate<StencilTestState>();
		}
#endif
		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(StencilTestState a, StencilTestState b)
		{
			return a.op == b.op && a.mode == b.mode;
		}
		/// <summary></summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(StencilTestState a, StencilTestState b)
		{
			return a.op != b.op || a.mode != b.mode;
		}
		/// <summary></summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is StencilTestState)
				return ((StencilTestState)obj) == this;
			return base.Equals(obj);
		}
		/// <summary>
		/// Gets the hash code. Returned value is the bitwise XOR of the two internal bitfields.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return op ^ mode;
		}

		/// <summary>
		/// Gets/Sets if stencil testing is enabled
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public bool Enabled
		{
			get { return (mode & 1) == 1; }
			set { mode = ((mode & ~1) | (value ? 1 : 0)); }
		}

		/// <summary>
		/// Gets/Sets if using independant stencil testing functions/operations for back and front facing triangles is enabled
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public bool TwoSidedStencilModeEnabled
		{
			get { return (mode & 2) == 2; }
			set { mode = ((mode & ~2) | (value ? 2 : 0)); }
		}

		/// <summary>
		/// Gets/Sets the operation performed on the stencil buffer is the <see cref="StencilFunction"/> passes, the but depth test fails
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public StencilOperation StencilPassZFailOperation
		{
			get { const int offset = 0; return (StencilOperation)(((op >> offset) & 7) + 1); }
			set { const int offset = 0; op = (((int)op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
		}
		/// <summary>
		/// Gets/Sets the operation performed on the stencil buffer is the <see cref="StencilFunction"/> and depth test pass
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public StencilOperation StencilPassOperation
		{
			get { const int offset = 4; return (StencilOperation)(((op >> offset) & 7) + 1); }
			set { const int offset = 4; op = (((int)op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
		}
		/// <summary>
		/// Gets/Sets the operation performed on the stencil buffer is the <see cref="StencilFunction"/> fails
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public StencilOperation StencilFailOperation
		{
			get { const int offset = 8; return (StencilOperation)(((op >> offset) & 7) + 1); }
			set { const int offset = 8; op = (((int)op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
		}

		/// <summary>
		/// Gets/Sets the operation performed on the stencil buffer is the <see cref="StencilFunction"/> passes, the but depth test fails when the pixel being drawn is from a backfacing triangle and <see cref="TwoSidedStencilModeEnabled"/> is true
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public StencilOperation BackfaceStencilPassZFailOperation
		{
			get { const int offset = 12; return (StencilOperation)(((op >> offset) & 7) + 1); }
			set { const int offset = 12; op = (((int)op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
		}
		/// <summary>
		/// Gets/Sets the operation performed on the stencil buffer is the <see cref="StencilFunction"/> and the depth test pass, when the pixel being drawn is from a backfacing triangle and <see cref="TwoSidedStencilModeEnabled"/> is true
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public StencilOperation BackfaceStencilPassOperation
		{
			get { const int offset = 16; return (StencilOperation)(((op >> offset) & 7) + 1); }
			set { const int offset = 16; op = (((int)op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
		}
		/// <summary>
		/// Gets/Sets the operation performed on the stencil buffer is the <see cref="StencilFunction"/> fails, when the pixel being drawn is from a backfacing triangle and <see cref="TwoSidedStencilModeEnabled"/> is true
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public StencilOperation BackfaceStencilFailOperation
		{
			get { const int offset = 20; return (StencilOperation)(((op >> offset) & 7) + 1); }
			set { const int offset = 20; op = (((int)op & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
		}

		/// <summary>
		/// Gets/Sets the comparison function performed with the value in the stencil buffer and the <see cref="ReferenceValue"/>
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public CompareFunction StencilFunction
		{
			get { const int offset = 24; return (CompareFunction)(((~(op >> offset)) & 7) + 1); }
			set { const int offset = 24; op = (((int)op & ~(7 << offset)) | (7 & (~((int)value - 1))) << offset); }
		}
		/// <summary>
		/// Gets/Sets the comparison function performed with the value in the stencil buffer and the <see cref="ReferenceValue"/>, when the pixel being drawn is from a backfacing triangle and <see cref="TwoSidedStencilModeEnabled"/> is true
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public CompareFunction BackfaceStencilFunction
		{
			get { const int offset = 28; return (CompareFunction)(((~(op >> offset)) & 7) + 1); }
			set { const int offset = 28; op = (((int)op & ~(7 << offset)) | (7 & (~((int)value - 1))) << offset); }
		}

		/// <summary>
		/// Gets/Sets the reference value used in the <see cref="StencilFunction"/> comparison
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public byte ReferenceValue
		{
			get { const int offset = 8; return (byte)(~((mode >> offset) & 255)); }
			set { const int offset = 8; mode = (((int)mode & ~(255 << offset)) | (255 & (~value)) << offset); }
		}
		/// <summary>
		/// Gets/Sets a bitmask used during stencil buffer reads. The default value is 255 (full mask).
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public byte StencilReadMask
		{
			get { const int offset = 16; return (byte)(~((mode >> offset) & 255)); }
			set { const int offset = 16; mode = (((int)mode & ~(255 << offset)) | (255 & (~value)) << offset); }
		}
		/// <summary>
		/// Gets/Sets a bitmask used during stencil buffer writes. The default value is 255 (full mask).
		/// </summary>
		/// <remarks>See <see cref="StencilTestState"/> remarks for details</remarks>
		public byte StencilWriteMask
		{
			get { const int offset = 24; return (byte)(~((mode >> offset) & 255)); }
			set { const int offset = 24; mode = (((int)mode & ~(255 << offset)) | (255 & (~value)) << offset); }
		}

		internal void ResetState(ref StencilTestState current, GraphicsDevice device)
		{
			device.RenderState.StencilDepthBufferFail = StencilPassZFailOperation;
			device.RenderState.StencilPass = StencilPassOperation;
			device.RenderState.StencilFail = StencilFailOperation;
			device.RenderState.CounterClockwiseStencilDepthBufferFail = BackfaceStencilPassZFailOperation;
			device.RenderState.CounterClockwiseStencilPass = BackfaceStencilPassOperation;
			device.RenderState.CounterClockwiseStencilFail = BackfaceStencilFailOperation;
			device.RenderState.StencilFunction = StencilFunction;
			device.RenderState.CounterClockwiseStencilFunction = BackfaceStencilFunction;

			device.RenderState.StencilEnable = this.Enabled;
			device.RenderState.TwoSidedStencilMode = this.TwoSidedStencilModeEnabled;

			device.RenderState.ReferenceStencil = ReferenceValue;
			device.RenderState.StencilMask = StencilReadMask;
			device.RenderState.StencilWriteMask = StencilWriteMask;

			current.op = this.op;
			current.mode = this.mode;
		}

		internal bool ApplyState(ref StencilTestState current, GraphicsDevice device)
		{
			bool changed = false;
			if (this.Enabled)
			{
				
#if DEBUG
				changed = mode != current.mode ||
					op != current.op;
#endif

				if (!current.Enabled)
					device.RenderState.StencilEnable = true;

				if (op != current.op)
				{
					if (current.StencilPassZFailOperation != StencilPassZFailOperation)
						device.RenderState.StencilDepthBufferFail = StencilPassZFailOperation;

					if (current.StencilPassOperation != StencilPassOperation)
						device.RenderState.StencilPass = StencilPassOperation;

					if (current.StencilFailOperation != StencilFailOperation)
						device.RenderState.StencilFail = StencilFailOperation;


					if (current.BackfaceStencilPassZFailOperation != BackfaceStencilPassZFailOperation)
						device.RenderState.CounterClockwiseStencilDepthBufferFail = BackfaceStencilPassZFailOperation;

					if (current.BackfaceStencilPassOperation != BackfaceStencilPassOperation)
						device.RenderState.CounterClockwiseStencilPass = BackfaceStencilPassOperation;

					if (current.BackfaceStencilFailOperation != BackfaceStencilFailOperation)
						device.RenderState.CounterClockwiseStencilFail = BackfaceStencilFailOperation;


					if (current.StencilFunction != StencilFunction)
						device.RenderState.StencilFunction = StencilFunction;
					
					if (current.BackfaceStencilFunction != BackfaceStencilFunction)
						device.RenderState.CounterClockwiseStencilFunction = BackfaceStencilFunction;

					current.op = op;
				}

				if (current.mode != mode)
				{
					if (current.TwoSidedStencilModeEnabled != TwoSidedStencilModeEnabled)
						device.RenderState.TwoSidedStencilMode = this.TwoSidedStencilModeEnabled;

					if (current.ReferenceValue != ReferenceValue)
						device.RenderState.ReferenceStencil = ReferenceValue;
					if (current.StencilReadMask != StencilReadMask)
						device.RenderState.StencilMask = StencilReadMask;
					if (current.StencilWriteMask != StencilWriteMask)
						device.RenderState.StencilWriteMask = StencilWriteMask;
				}

				current.mode = this.mode;
			}
			else
			{
				if (current.Enabled)
				{
					
#if DEBUG
					changed = true;
#endif
					device.RenderState.StencilEnable = false;
					current.Enabled = false;
				}
			}
			return changed;
		}
	}


	internal static class TextureSamplerStateInternal
	{
		internal static void ApplyState(TextureSamplerState state, SamplerState sampler, ref TextureSamplerState current		
#if DEBUG
				,DrawState dstate
#endif
			)
		{
			int mode = state;
			int current_mode = current;

			if ((mode & 511) != (current_mode & 511))
			{
				if (state.AddressU != current.AddressU)
					sampler.AddressU = state.AddressU;

				if (state.AddressV != current.AddressV)
					sampler.AddressV = state.AddressV;

				if (state.AddressW != current.AddressW)
					sampler.AddressW = state.AddressW;

#if DEBUG
				dstate.Application.currentFrame.TextureSamplerAddressStateChanged ++;
#endif
			}

			if ((mode & (63 << 9)) != (current_mode & (63 << 9)))
			{
				if (state.MinFilter != current.MinFilter)
					sampler.MinFilter = state.MinFilter;

				if (state.MagFilter != current.MagFilter)
					sampler.MagFilter = state.MagFilter;

				if (state.MipFilter != current.MipFilter)
					sampler.MipFilter = state.MipFilter;

#if DEBUG
				System.Threading.Interlocked.Increment(ref dstate.Application.currentFrame.TextureSamplerFilterStateChanged);
#endif
			}

			if ((mode & (0xFFFF << 16)) != (current_mode & (0xFFFF << 16)))
			{
				if (state.MaxAnisotropy != current.MaxAnisotropy)
					sampler.MaxAnisotropy = state.MaxAnisotropy;

				if (state.MaxMipmapLevel != current.MaxMipmapLevel)
					sampler.MaxMipLevel = state.MaxMipmapLevel;
			}

			current = state;
		}


		internal static void ResetState(SamplerState sampler, TextureSamplerState state)
		{
			sampler.AddressU = state.AddressU;
			sampler.AddressV = state.AddressV;
			sampler.AddressW = state.AddressW;
			sampler.MinFilter = state.MinFilter;
			sampler.MagFilter = state.MagFilter;
			sampler.MipFilter = state.MipFilter;
			sampler.MaxAnisotropy = state.MaxAnisotropy;
			sampler.MaxMipLevel = state.MaxMipmapLevel;
		}

	}

	//now included in the shader system instead.
	/*
	 
	/// <summary>
	/// Packed representation of common Texture Sampler states. 4 bytes
	/// </summary>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 4)]
	public struct TextureSamplerState
	{
		[System.Runtime.InteropServices.FieldOffset(0)]
		internal int mode;

#if DEBUG
		static TextureSamplerState()
		{
			BitWiseTypeValidator.Validate<TextureSamplerState>();
		}
#endif

		public static implicit operator Xen.Graphics.State.TextureSamplerState(TextureSamplerState sampler)
		{
			return (Xen.Graphics.State.TextureSamplerState)sampler.mode;
		}
		public static implicit operator TextureSamplerState(Xen.Graphics.State.TextureSamplerState sampler)
		{
			return new TextureSamplerState((int)sampler);
		}

		internal void ApplyState(SamplerState sampler, ref TextureSamplerState current)
		{
			if ((this.mode & 511) != (current.mode & 511))
			{
				if (AddressU != current.AddressU)
					sampler.AddressU = AddressU;

				if (AddressV != current.AddressV)
					sampler.AddressV = AddressV;

				if (AddressW != current.AddressW)
					sampler.AddressW = AddressW;
			}

			if ((this.mode & (63 << 9)) != (current.mode & (63 << 9)))
			{
				if (MinFilter != current.MinFilter)
					sampler.MinFilter = MinFilter;

				if (MagFilter != current.MagFilter)
					sampler.MagFilter = MagFilter;

				if (MipFilter != current.MipFilter)
					sampler.MipFilter = MipFilter;
			}

			if ((this.mode & (0xFFFF << 16)) != (current.mode & (0xFFFF << 16)))
			{
				if (MaxAnisotropy != current.MaxAnisotropy)
					sampler.MaxAnisotropy = MaxAnisotropy;

				if (MaxMipmapLevel != current.MaxMipmapLevel)
					sampler.MaxMipLevel = MaxMipmapLevel;
			}

			current.mode = this.mode;
		}


		internal void ResetState(SamplerState sampler, ref TextureSamplerState current)
		{
			sampler.AddressU = AddressU;
			sampler.AddressV = AddressV;
			sampler.AddressW = AddressW;
			sampler.MinFilter = MinFilter;
			sampler.MagFilter = MagFilter;
			sampler.MipFilter = MipFilter;
			sampler.MaxAnisotropy = MaxAnisotropy;
			sampler.MaxMipLevel = MaxMipmapLevel;

			current.mode = this.mode;
		}


		internal TextureSamplerState(TextureAddressMode uv, TextureFilter min, TextureFilter mag, TextureFilter mip, int maxAni)
		{
			mode = 0;
			this.AddressUV = uv;
			this.MinFilter = min;
			this.MagFilter = mag;
			this.MipFilter = mip;
			this.MaxAnisotropy = maxAni;
		}
		internal TextureSamplerState(int mode)
		{
			this.mode = mode;
		}

		public static bool operator ==(TextureSamplerState a, TextureSamplerState b)
		{
			return a.mode == b.mode;
		}
		public static bool operator !=(TextureSamplerState a, TextureSamplerState b)
		{
			return a.mode != b.mode;
		}
		public override bool Equals(object obj)
		{
			if (obj is TextureSamplerState)
				return ((TextureSamplerState)obj).mode == this.mode;
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return mode;
		}

		private static TextureSamplerState point = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Point, TextureFilter.Point, TextureFilter.Point, 0);
		private static TextureSamplerState bilinear = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Linear, TextureFilter.Linear, TextureFilter.Point, 0);
		private static TextureSamplerState trilinear = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Linear, TextureFilter.Linear, TextureFilter.Linear, 0);
		private static TextureSamplerState aniLow = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Anisotropic, TextureFilter.Linear, TextureFilter.Linear, 2);
		private static TextureSamplerState aniMed = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Anisotropic, TextureFilter.Linear, TextureFilter.Linear, 4);
		private static TextureSamplerState aniHigh = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Anisotropic, TextureFilter.Linear, TextureFilter.Linear, 8);

		public static TextureSamplerState PointFiltering { get { return point; } }
		public static TextureSamplerState BilinearFiltering { get { return bilinear; } }
		public static TextureSamplerState TrilinearFiltering { get { return trilinear; } }
		public static TextureSamplerState AnisotropicLowFiltering { get { return aniLow; } }
		public static TextureSamplerState AnisotropicMediumFiltering { get { return aniMed; } }
		public static TextureSamplerState AnisotropicHighFiltering { get { return aniHigh; } }


		/// <summary>
		/// Allows setting of both the <see cref="AddressU"/> and <see cref="AddressV"/> coordinate filtering modes at the same time
		/// </summary>
		public TextureAddressMode AddressUV
		{
			set
			{
				{
					const int offset = 0; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset);
				}
				{
					const int offset = 3; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset);
				}
			}
		}
		///// <summary>
		///// Allows setting of all three UVW coordinate filtering modes at the same time
		///// </summary>
		//public TextureAddressMode AddressUVW
		//{
		//    set
		//    {
		//        mode = (mode & (~(7 << 3)) | (~(7 << 0)) | (~(7 << 6))) | (((7 & ((int)value - 1)) << 0) | ((7 & ((int)value - 1)) << 3) | ((7 & ((int)value - 1)) << 6));
		//    }
		//}

		/// <summary>
		/// Controls texture filtering behaviour for the U coordinate sampler (The U coordinate is the x-axis texture coordinate)
		/// </summary>
		public TextureAddressMode AddressU
		{
			get { const int offset = 0; return (TextureAddressMode)(((mode >> offset) & 7) + 1); }
			set { const int offset = 0; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
		}
		/// <summary>
		/// Controls texture filtering behaviour for the V coordinate sampler (The V coordinate is the y-axis texture coordinate)
		/// </summary>
		public TextureAddressMode AddressV
		{
			get { const int offset = 3; return (TextureAddressMode)(((mode >> offset) & 7) + 1); }
			set { const int offset = 3; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
		}
		/// <summary>
		/// Controls texture filtering behaviour for the W coordinate sampler (The W coordinate is the z-axis texture coordinate). This filtering mode only applies to 3D textures.
		/// </summary>
		public TextureAddressMode AddressW
		{
			get { const int offset = 6; return (TextureAddressMode)(((mode >> offset) & 7) + 1); }
			set { const int offset = 6; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
		}
		/// <summary>
		/// Controls texture filtering when the texture is displayed over a smaller than itself (the texture is minified, or reduced in size)
		/// </summary>
		/// <remarks>TextureFilter.None, TextureFilter.Point, TextureFilter.Linear and TextureFilter.Anisotropic are supported.</remarks>
		public TextureFilter MinFilter
		{
			get { const int offset = 9; return (TextureFilter)(((mode >> offset) & 3)); }
			set { const int offset = 9; mode = ((mode & ~(3 << offset)) | (3 & (Math.Min(3, (int)value))) << offset); }
		}
		/// <summary>
		/// Controls texture filtering when the texture is displayed over a larger than itself (the texture is magnified, or enlarged)
		/// </summary>
		/// <remarks>TextureFilter.None, TextureFilter.Point, TextureFilter.Linear are supported.</remarks>
		public TextureFilter MagFilter
		{
			get { const int offset = 11; return (TextureFilter)(((mode >> offset) & 3)); }
			set { const int offset = 11; mode = ((mode & ~(3 << offset)) | (3 & (Math.Min(2, (int)value))) << offset); }
		}
		/// <summary>
		/// <para>Controls texture filtering that takes place between different mipmap levels.</para>
		/// <para>Set TextureFilter.None to disable mipmapping</para>
		/// <para>Set TextureFilter.Point in combination with AddressUV filtering for bilinear filtering (2 axis filtering)</para>
		/// <para>Set TextureFilter.Linear in combination with AddressUV filtering for trilinear filtering (3 axis filtering), where there is linear interpolation between mipmap levels.</para>
		/// </summary>
		/// <remarks>TextureFilter.None, TextureFilter.Point, TextureFilter.Linear</remarks>
		public TextureFilter MipFilter
		{
			get { const int offset = 13; return (TextureFilter)(((mode >> offset) & 3)); }
			set { const int offset = 13; mode = ((mode & ~(3 << offset)) | (3 & (Math.Min(2, (int)value))) << offset); }
		}
		/// <summary>
		/// Set the maximum number of samples used when <see cref="MinFilter"/> is set to <see cref="TextureFilter.Anisotropic"/> filtering. Range of [1-16]
		/// </summary>
		public int MaxAnisotropy
		{
			get { const int offset = 16; return (((mode >> offset) & 15) + 1); }
			set { const int offset = 16; mode = ((mode & ~(15 << offset)) | (15 & (Math.Max(0, Math.Min(16, value) - 1))) << offset); }
		}
		/// <summary>
		/// Set the maximum mipmap level the video card will sample, where 0 is the largest map (and the default value).
		/// </summary>
		public int MaxMipmapLevel
		{
			get { const int offset = 20; return ((((mode >> offset)) & 255)); }
			set { const int offset = 20; mode = ((mode & ~(255 << offset)) | (255 & ((Math.Min(255, value)))) << offset); }
		}
		
		public void SetToTrilinearFiltering()
		{
			this.mode = trilinear.mode;
		}
		public void SetToBilinearFiltering()
		{
			this.mode = bilinear.mode;
		}
		public void SetToPointFiltering()
		{
			this.mode = point.mode;
		}
		public void SetToAnisotropicFilteringLow()
		{
			this.mode = aniLow.mode;
		}
		public void SetToAnisotropicFilteringMedium()
		{
			this.mode = aniMed.mode;
		}
		public void SetToAnisotropicFilteringHigh()
		{
			this.mode = aniHigh.mode;
		}
	}

	*/
}
