using System;
using System.Collections.Generic;
using System.Text;
using Xen.Graphics;
using Xen.Ex.Graphics2D;
using Microsoft.Xna.Framework;
using Xen.Graphics.Modifier;
using Xen.Graphics.State;

namespace Xen.Ex.Filters
{
	/// <summary>
	/// Stores an 16 sample filter
	/// </summary>
	public struct Filter16Sample
	{
		private static readonly Filter16Sample horizontalBlur = Blur(new Vector2(1, 0));
		private static readonly Filter16Sample verticalBlur = Blur(new Vector2(0, 1));

		private static Filter16Sample Blur(Vector2 axis)
		{
			Filter16Sample f;

			f.PixelOffset0 = axis * 14.25f;
			f.PixelOffset1 = axis * 12.40f;
			f.PixelOffset2 = axis * 10.45f;
			f.PixelOffset3 = axis * 8.475f;
			f.PixelOffset4 = axis * 6.475f;
			f.PixelOffset5 = axis * 4.475f;
			f.PixelOffset6 = axis * 2.475f;
			f.PixelOffset7 = axis * 0.65f;
			f.PixelOffset8 = axis * -0.65f;
			f.PixelOffset9 = axis * -2.475f;
			f.PixelOffset10= axis * -4.475f;
			f.PixelOffset11= axis * -6.475f;
			f.PixelOffset12= axis * -8.475f;
			f.PixelOffset13= axis * -10.45f;
			f.PixelOffset14= axis * -12.40f;
			f.PixelOffset15= axis * -14.25f;

			//approx consistent drops
			//.35 * 2     = 0.70
			//.65         = 0.65
			//.525 * 1.2  = 0.63
			//.475 * 1.2  = 0.57
			//.525 * 1.0  = 0.525
			//.475 * 1.0  = 0.475
			//.525 * 0.8  = 0.42
			//.475 * 0.8  = 0.38
			//.525 * 0.65 = 0.34
			//.475 * 0.65 = 0.31
			//.55  * 0.475= 0.26
			//.45  * 0.475= 0.215
			//.6   * 0.275= 0.16
			//.4   * 0.275= 0.11
			//.75  * 0.1  = 0.07
			//.25  * 0.1  = 0.03


			f.Weight0 = 0.1f;
			f.Weight1 = 0.275f;
			f.Weight2 = 0.475f;
			f.Weight3 = 0.65f;
			f.Weight4 = 0.8f;
			f.Weight5 = 1.0f;
			f.Weight6 = 1.2f;
			f.Weight7 = 1;
			f.Weight8 = 1;
			f.Weight9 = 1.2f;
			f.Weight10= 1.0f;
			f.Weight11= 0.8f;
			f.Weight12= 0.65f;
			f.Weight13= 0.475f;
			f.Weight14= 0.275f;
			f.Weight15= 0.1f;

			float total = 11;

			f.Weight0 /= total;
			f.Weight1 /= total;
			f.Weight2 /= total;
			f.Weight3 /= total;
			f.Weight4 /= total;
			f.Weight5 /= total;
			f.Weight6 /= total;
			f.Weight7 /= total;
			f.Weight8 /= total;
			f.Weight9 /= total;
			f.Weight10 /= total;
			f.Weight11 /= total;
			f.Weight12 /= total;
			f.Weight13 /= total;
			f.Weight14 /= total;
			f.Weight15 /= total;

			return f;
		}

		/// <summary>
		/// Horizontal filter pass used in the <see cref="BlurFilter"/>
		/// </summary>
		public static Filter16Sample HorizontalBlur { get { return horizontalBlur; } }
		/// <summary>
		/// Vertical filter pass used in the <see cref="BlurFilter"/>
		/// </summary>
		public static Filter16Sample VerticalBlur { get { return verticalBlur; } }

		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset0;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset1;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset2;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset3;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset4;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset5;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset6;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset7;

		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset8;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset9;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset10;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset11;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset12;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset13;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset14;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset15;

		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight0;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight1;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight2;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight3;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight4;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight5;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight6;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight7;

		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight8;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight9;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight10;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight11;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight12;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight13;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight14;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight15;
	}

	/// <summary>
	/// Stores an 8 sample filter
	/// </summary>
	public struct Filter8Sample
	{
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset0;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset1;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset2;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset3;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset4;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset5;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset6;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset7;

		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight0;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight1;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight2;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight3;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight4;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight5;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight6;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight7;
	}

	/// <summary>
	/// Stores an 4 sample filter
	/// </summary>
	public struct Filter4Sample
	{
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset0;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset1;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset2;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset3;

		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight0;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight1;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight2;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight3;
	}

	/// <summary>
	/// Stores an 2 sample filter
	/// </summary>
	public struct Filter2Sample
	{
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset0;
		/// <summary>Pixel offset from the sampling centre point</summary>
		public Vector2 PixelOffset1;

		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight0;
		/// <summary>Pixel weighting for the sample (all weights should usually add to 1)</summary>
		public float Weight1;
	}

	/// <summary>
	/// Applies a single pass texture filter (up to 16 sample) to a draw target
	/// </summary>
	public sealed class SinglePassTextureFilter : IDraw
	{
		private readonly DrawTargetTexture2D source, targetClone;
		private readonly ShaderElement element;
		private readonly Vector3[] filter = new Vector3[16];
		private int kernelSize;


		/// <summary>
		/// Creates a single pass 16 sample filter
		/// </summary>
		/// <param name="source">soure texture to filter</param>
		/// <param name="target">target to filter</param>
		/// <param name="filter">Filter to apply</param>
		public SinglePassTextureFilter(DrawTargetTexture2D source, DrawTargetTexture2D target, ref Filter16Sample filter)
			: this(source,target)
		{
			SetFilter(ref filter);
		}
		/// <summary>
		/// Creates a single pass 16 sample filter
		/// </summary>
		/// <param name="source">soure texture to filter</param>
		/// <param name="target">target to filter</param>
		/// <param name="filter">Filter to apply</param>
		public SinglePassTextureFilter(DrawTargetTexture2D source, DrawTargetTexture2D target, Filter16Sample filter)
			: this(source, target)
		{
			SetFilter(ref filter);
		}


		/// <summary>
		/// Creates a single pass 8 sample filter
		/// </summary>
		/// <param name="source">soure texture to filter</param>
		/// <param name="target">target to filter</param>
		/// <param name="filter">Filter to apply</param>
		public SinglePassTextureFilter(DrawTargetTexture2D source, DrawTargetTexture2D target, ref Filter8Sample filter)
			: this(source, target)
		{
			SetFilter(ref filter);
		}
		/// <summary>
		/// Creates a single pass 8 sample filter
		/// </summary>
		/// <param name="source">soure texture to filter</param>
		/// <param name="target">target to filter</param>
		/// <param name="filter">Filter to apply</param>
		public SinglePassTextureFilter(DrawTargetTexture2D source, DrawTargetTexture2D target, Filter8Sample filter)
			: this(source, target)
		{
			SetFilter(ref filter);
		}


		/// <summary>
		/// Creates a single pass 4 sample filter
		/// </summary>
		/// <param name="source">soure texture to filter</param>
		/// <param name="target">target to filter</param>
		/// <param name="filter">Filter to apply</param>
		public SinglePassTextureFilter(DrawTargetTexture2D source, DrawTargetTexture2D target, ref Filter4Sample filter)
			: this(source, target)
		{
			SetFilter(ref filter);
		}
		/// <summary>
		/// Creates a single pass 4 sample filter
		/// </summary>
		/// <param name="source">soure texture to filter</param>
		/// <param name="target">target to filter</param>
		/// <param name="filter">Filter to apply</param>
		public SinglePassTextureFilter(DrawTargetTexture2D source, DrawTargetTexture2D target, Filter4Sample filter)
			: this(source, target)
		{
			SetFilter(ref filter);
		}

		/// <summary>
		/// Creates a single pass 2 sample filter
		/// </summary>
		/// <param name="source">soure texture to filter</param>
		/// <param name="target">target to filter</param>
		/// <param name="filter">Filter to apply</param>
		public SinglePassTextureFilter(DrawTargetTexture2D source, DrawTargetTexture2D target, ref Filter2Sample filter)
			: this(source, target)
		{
			SetFilter(ref filter);
		}
		/// <summary>
		/// Creates a single pass 2 sample filter
		/// </summary>
		/// <param name="source">soure texture to filter</param>
		/// <param name="target">target to filter</param>
		/// <param name="filter">Filter to apply</param>
		public SinglePassTextureFilter(DrawTargetTexture2D source, DrawTargetTexture2D target, Filter2Sample filter)
			: this(source, target)
		{
			SetFilter(ref filter);
		}

		private SinglePassTextureFilter(DrawTargetTexture2D source, DrawTargetTexture2D target)
		{
			if (source == null || target == null)
				throw new ArgumentNullException();
			if (source.SurfaceFormat != target.SurfaceFormat)
				throw new ArgumentException("source.SurfaceFormat != target.SurfaceFormat");
			if (target.MultiSampleType != Microsoft.Xna.Framework.Graphics.MultiSampleType.None)
				throw new ArgumentException("Target may not use multisample anitialiasing");
#if XBOX
			if (source == target && source.Width * source.Height * DrawTarget.FormatSize(source.SurfaceFormat) >
				1000*1000*10)//approx 10mb
				throw new ArgumentException("source == target is invalid for larget render targets on Xbox360");
#else
			
			if (source == target)
				throw new ArgumentException("source == target is invalid on windows");
#endif
			if (source.Width > target.Width ||
				source.Height > target.Height)
				throw new ArgumentException("source is larger than target");

			this.source = source;
			this.targetClone = target.Clone(false,false,false);
			this.element = new ShaderElement(null, source.Width, source.Height,new Vector2(1,1),true);
			this.targetClone.Add(element);

			if (target.Width != source.Width ||
				target.Height != source.Height)
			{
				this.targetClone.AddModifier(new Xen.Graphics.Modifier.ScissorModifier(0, 0, Math.Min(1, (source.Width) / target.Width), Math.Min(1, (source.Height) / target.Height)));
				this.element.TextureCrop = new Rectangle(0, 0, Math.Min(this.source.Width, this.targetClone.Width), Math.Min(this.source.Height, this.targetClone.Height));
			}
		}

		/// <summary>
		/// Set a 16 sample filter
		/// </summary>
		/// <param name="filter"></param>
		public void SetFilter(ref Filter16Sample filter)
		{
			this.filter[0] = new Vector3(filter.PixelOffset0, filter.Weight0);
			this.filter[1] = new Vector3(filter.PixelOffset1, filter.Weight1);
			this.filter[2] = new Vector3(filter.PixelOffset2, filter.Weight2);
			this.filter[3] = new Vector3(filter.PixelOffset3, filter.Weight3);
			this.filter[4] = new Vector3(filter.PixelOffset4, filter.Weight4);
			this.filter[5] = new Vector3(filter.PixelOffset5, filter.Weight5);
			this.filter[6] = new Vector3(filter.PixelOffset6, filter.Weight6);
			this.filter[7] = new Vector3(filter.PixelOffset7, filter.Weight7);

			this.filter[8] = new Vector3(filter.PixelOffset8, filter.Weight8);
			this.filter[9] = new Vector3(filter.PixelOffset9, filter.Weight9);
			this.filter[10] = new Vector3(filter.PixelOffset10, filter.Weight10);
			this.filter[11] = new Vector3(filter.PixelOffset11, filter.Weight11);
			this.filter[12] = new Vector3(filter.PixelOffset12, filter.Weight12);
			this.filter[13] = new Vector3(filter.PixelOffset13, filter.Weight13);
			this.filter[14] = new Vector3(filter.PixelOffset14, filter.Weight14);
			this.filter[15] = new Vector3(filter.PixelOffset15, filter.Weight15);

			kernelSize = 16;
		}

		/// <summary>
		/// Set an 8 sample filter
		/// </summary>
		/// <param name="filter"></param>
		public void SetFilter(ref Filter8Sample filter)
		{
			this.filter[0] = new Vector3(filter.PixelOffset0, filter.Weight0);
			this.filter[1] = new Vector3(filter.PixelOffset1, filter.Weight1);
			this.filter[2] = new Vector3(filter.PixelOffset2, filter.Weight2);
			this.filter[3] = new Vector3(filter.PixelOffset3, filter.Weight3);
			this.filter[4] = new Vector3(filter.PixelOffset4, filter.Weight4);
			this.filter[5] = new Vector3(filter.PixelOffset5, filter.Weight5);
			this.filter[6] = new Vector3(filter.PixelOffset6, filter.Weight6);
			this.filter[7] = new Vector3(filter.PixelOffset7, filter.Weight7);

			kernelSize = 8;
		}

		/// <summary>
		/// Set a 4 sample filter
		/// </summary>
		/// <param name="filter"></param>
		public void SetFilter(ref Filter4Sample filter)
		{
			this.filter[0] = new Vector3(filter.PixelOffset0, filter.Weight0);
			this.filter[1] = new Vector3(filter.PixelOffset1, filter.Weight1);
			this.filter[2] = new Vector3(filter.PixelOffset2, filter.Weight2);
			this.filter[3] = new Vector3(filter.PixelOffset3, filter.Weight3);

			kernelSize = 4;
		}

		/// <summary>
		/// Set a 2 sample filter
		/// </summary>
		/// <param name="filter"></param>
		public void SetFilter(ref Filter2Sample filter)
		{
			this.filter[0] = new Vector3(filter.PixelOffset0, filter.Weight0);
			this.filter[1] = new Vector3(filter.PixelOffset1, filter.Weight1);

			kernelSize = 2;
		}

		/// <summary>
		/// Apply the filter
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			switch (kernelSize)
			{
				case 16:
					Kernel16 shader16 = state.GetShader<Kernel16>();
					shader16.Texture = source.GetTexture();
					shader16.TextureSize = this.source.Size;
					shader16.SetKernel(this.filter);
					element.Shader = shader16;
					break;
				case 8:
					Kernel8 shader8 = state.GetShader<Kernel8>();
					shader8.Texture = source.GetTexture();
					shader8.TextureSize = this.source.Size;
					shader8.SetKernel(this.filter);
					element.Shader = shader8;
					break;
				case 4:
					Kernel4 shader4 = state.GetShader<Kernel4>();
					shader4.Texture = source.GetTexture();
					shader4.TextureSize = this.source.Size;
					shader4.SetKernel(this.filter);
					element.Shader = shader4;
					break;
				case 2:
					Kernel2 shader2 = state.GetShader<Kernel2>();
					shader2.Texture = source.GetTexture();
					shader2.TextureSize = this.source.Size;
					shader2.SetKernel(this.filter);
					element.Shader = shader2;
					break;
			}
			targetClone.Draw(state);
		}

		bool ICullable.CullTest(ICuller culler)
		{
			return true;
		}
	}
	/// <summary>
	/// Applies a two pass, 16 sample vertical and horizontal texture blur filter to a draw target
	/// </summary>
	public sealed class BlurFilter : IDraw
	{
		SinglePassTextureFilter filterV, filterH;

		/// <summary>
		/// Blur the source horizontally to the <paramref name="intermediate"/> target, then blur vertically back to <paramref name="source"/>. (Xbox360 may specify null for <paramref name="intermediate"/>)
		/// </summary>
		/// <param name="source"></param>
		/// <param name="intermediate">draw target to use as a temporary, intermediate target for blurring</param>
		public BlurFilter(DrawTargetTexture2D source, DrawTargetTexture2D intermediate)
		{
#if XBOX
			//if the surface will fit into EDRAM then the intermediate can be skipped
			if (source.Width*source.Height*DrawTarget.FormatSize(source.SurfaceFormat)<1024*1024*9)//approx
			{
				this.filterV = new SinglePassTextureFilter(source, source, Filter16Sample.VerticalBlur);
				this.filterH = new SinglePassTextureFilter(source, source, Filter16Sample.HorizontalBlur);
			}
			else
			{
				this.filterV = new SinglePassTextureFilter(source, intermediate, Filter16Sample.VerticalBlur);
				this.filterH = new SinglePassTextureFilter(intermediate, source, Filter16Sample.HorizontalBlur);
			}
#else
			this.filterV = new SinglePassTextureFilter(source, intermediate, Filter16Sample.VerticalBlur);
			this.filterH = new SinglePassTextureFilter(intermediate, source, Filter16Sample.HorizontalBlur);
#endif
		}

		/// <summary>
		/// Blur the source horizontally to the <paramref name="intermediate"/> target, then blur vertically to <paramref name="target"/>.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="intermediate">draw target to use as a temporary, intermediate target for blurring</param>
		/// <param name="target"></param>
		public BlurFilter(DrawTargetTexture2D source, DrawTargetTexture2D intermediate, DrawTargetTexture2D target)
		{
#if XBOX
			//if the surface will fit into EDRAM then the intermediate can be skipped
			if (source.Width*source.Height*DrawTarget.FormatSize(source.SurfaceFormat)<1024*1024*9)//approx
			{
				this.filterV = new SinglePassTextureFilter(source, target, Filter16Sample.VerticalBlur);
				this.filterH = new SinglePassTextureFilter(target, target, Filter16Sample.HorizontalBlur);
			}
			else
			{
				this.filterV = new SinglePassTextureFilter(source, intermediate, Filter16Sample.VerticalBlur);
				this.filterH = new SinglePassTextureFilter(intermediate, source, Filter16Sample.HorizontalBlur);
			}
#else
			this.filterV = new SinglePassTextureFilter(source, intermediate, Filter16Sample.VerticalBlur);
			this.filterH = new SinglePassTextureFilter(intermediate, target, Filter16Sample.HorizontalBlur);
#endif
		}

		/// <summary>
		/// Apply the filter
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			filterV.Draw(state);
			filterH.Draw(state);
		}

		bool ICullable.CullTest(ICuller culler)
		{
			return true;
		}
	}

	/// <summary>
	/// Performs a number of passes to downsample a draw target to a desired size
	/// </summary>
	public sealed class TextureDownsample : IDraw
	{
		private List<IDraw> passes = new List<IDraw>();

		/// <summary>
		/// Construct the texture downsampler
		/// </summary>
		/// <param name="source">Source texture to read</param>
		/// <param name="target">Target texture to write to</param>
		/// <param name="intermediate">Intermediate texture (if null, will be created as required)</param>
		/// <param name="intermediate2">Second intermediate texture (if null, will be created as required)</param>
		/// <param name="targetWidth">target width to downsample to</param>
		/// <param name="targetHeight">target height to downsample to</param>
		public TextureDownsample(DrawTargetTexture2D source, DrawTargetTexture2D target, ref DrawTargetTexture2D intermediate, ref DrawTargetTexture2D intermediate2, int targetWidth, int targetHeight)
		{
			if (source == null ||
				target == null)
				throw new ArgumentNullException();

			if (targetWidth <= 0 ||
				targetHeight <= 0)
				throw new ArgumentException("Invalid target size");

			if (DrawTarget.FormatChannels(source.SurfaceFormat) != DrawTarget.FormatChannels(target.SurfaceFormat))
			    throw new ArgumentException("source.SurfaceFormat has a different number of channels than target.SurfaceFormat");

			if (target.MultiSampleType != Microsoft.Xna.Framework.Graphics.MultiSampleType.None)
				throw new ArgumentException("Target may not use multisample anitialiasing");

			if (intermediate != null && intermediate.MultiSampleType != Microsoft.Xna.Framework.Graphics.MultiSampleType.None)
				throw new ArgumentException("Intermediate may not use multisample anitialiasing");

			if (intermediate2 != null && intermediate2.MultiSampleType != Microsoft.Xna.Framework.Graphics.MultiSampleType.None)
				throw new ArgumentException("Intermediate2 may not use multisample anitialiasing");

			if (targetWidth > target.Width ||
				targetHeight > target.Height)
				throw new ArgumentException("Size is larger than target");

			if (targetWidth > source.Width ||
				targetHeight > source.Height)
				throw new ArgumentException("Size is larger than source");

			if (intermediate != null)
			{
				if (target.SurfaceFormat != intermediate.SurfaceFormat)
					throw new ArgumentException("target.SurfaceFormat != intermediate.SurfaceFormat");
				if (intermediate == intermediate2)
					throw new ArgumentException("intermediate == intermediate2");
			}
			if (intermediate2 != null)
			{
				if (target.SurfaceFormat != intermediate2.SurfaceFormat)
					throw new ArgumentException("target.SurfaceFormat != intermediate2.SurfaceFormat");
			}

			int w = source.Width;
			int h = source.Height;

			int targetMultipleWidth = targetWidth;
			int targetMultipleHeight = targetHeight;

			while (targetMultipleWidth * 2 <= w)
				targetMultipleWidth *= 2;
			while (targetMultipleHeight * 2 <= h)
				targetMultipleHeight *= 2;

			DrawTargetTexture2D current = null;
			Rectangle sRegion = new Rectangle(0,0,0,0);

			//first pass may require that the source is sized down to a multiple of the target size

			if ((double)targetWidth / (double)w <= 0.5 &&
				(double)targetHeight / (double)h <= 0.5 &&
				(targetMultipleWidth != w || targetMultipleHeight != h))
			{
				DrawTargetTexture2D go = this.PickRT(ref intermediate, ref intermediate2, source, targetMultipleWidth, targetMultipleHeight,target.SurfaceFormat);

				Vector2 size = new Vector2((float)targetMultipleWidth, (float)targetMultipleHeight);

				TexturedElement te = new TexturedElement(source, size, false);
				te.TextureCrop = new Rectangle(0,0,w,h);

				go.Add(te);
				passes.Add(go);
				current = go;
				w = targetMultipleWidth;
				h = targetMultipleHeight;
			}

			//downsample on the larger axis, either 2x, 4x or 8x downsampling, until reached the target size

			while (target.Equals(current) == false)
			{
				DrawTargetTexture2D localSource = current ?? source;

				double difW = (double)targetWidth / (double)w;
				double difH = (double)targetHeight / (double)h;

				sRegion.Width = w;
				sRegion.Height = h;
				sRegion.Y = localSource.Height - h;

				//both width/height difference are less than 50% smaller, so a linear interpolation will do fine
				if (difW > 0.5 &&
					difH > 0.5)
				{
					//write directly to the target
					DrawTargetTexture2D go = target.Clone(false, false, false);
					Vector2 te_size = new Vector2((float)targetWidth, (float)targetHeight);
					TexturedElement te = new TexturedElement(localSource, te_size, false);

					go.AddModifier(new ScissorModifier(0, go.Height - targetHeight, targetWidth, go.Height, go));
					te.TextureCrop = sRegion;

					go.Add(te);
					passes.Add(go);
					current = go;

					continue;
				}

				bool horizontal = difW < difH;
				double dif = Math.Min(difW, difH);
				int size = horizontal ? w : h;

				Vector2 dir = new Vector2(0,0);
				if (horizontal)
					dir.X = 1.0f / localSource.Width;
				else
					dir.Y = 1.0f / localSource.Height;

				if (dif > 0.25) // cutoff for using 2 samples
				{
					DrawTargetTexture2D go;
					int new_width = w;
					int new_height = h;
					if (horizontal)
						new_width /= 2;
					else
						new_height /= 2;

					if (new_width == targetWidth && new_height == targetHeight)
						go = target.Clone(false, false, false);
					else
						go = PickRT(ref intermediate, ref intermediate2, localSource, new_width, new_height, target.SurfaceFormat);

					Vector2 se_size = new Vector2((float)new_width, (float)new_height);
					ShaderElement se = new ShaderElement(new Downsample2(), se_size, false);

					go.AddModifier(new ScissorModifier(0, go.Height - new_height, new_width, go.Height, go));

					se.TextureCrop = sRegion;

					go.Add(new Drawer(dir,se,localSource));
					passes.Add(go);

					w = new_width;
					h = new_height;

					current = go;
					continue;
				}

				if (dif > 0.125) // cutoff for using 4 samples
				{
					DrawTargetTexture2D go;
					int new_width = w;
					int new_height = h;
					if (horizontal)
						new_width /= 4;
					else
						new_height /= 4;

					if (new_width == targetWidth && new_height == targetHeight)
						go = target.Clone(false, false, false);
					else
						go = PickRT(ref intermediate, ref intermediate2, localSource, new_width, new_height, target.SurfaceFormat);

					Vector2 se_size = new Vector2((float)new_width, (float)new_height);
					ShaderElement se = new ShaderElement(new Downsample4(), se_size, false);

					go.AddModifier(new ScissorModifier(0, go.Height - new_height, new_width, go.Height, go));

					se.TextureCrop = sRegion;

					go.Add(new Drawer(dir, se, localSource));
					passes.Add(go);

					w = new_width;
					h = new_height;

					current = go;
					continue;
				}

				// cutoff for using 8 samples
				{
					DrawTargetTexture2D go;
					int new_width = w;
					int new_height = h;
					if (horizontal)
						new_width /= 8;
					else
						new_height /= 8;
					
					if (new_width == targetWidth && new_height == targetHeight)
						go = target.Clone(false, false, false);
					else
						go = PickRT(ref intermediate, ref intermediate2, localSource, new_width, new_height, target.SurfaceFormat);

					Vector2 se_size = new Vector2((float)new_width, (float)new_height);
					ShaderElement se = new ShaderElement(new Downsample8(), se_size, false);

					go.AddModifier(new ScissorModifier(0, go.Height - new_height, new_width, go.Height, go));

					se.TextureCrop = sRegion;

					go.Add(new Drawer(dir, se, localSource));
					passes.Add(go);

					w = new_width;
					h = new_height;

					current = go;
					continue;
				}
			}
		}

		DrawTargetTexture2D PickRT(ref DrawTargetTexture2D int1, ref DrawTargetTexture2D int2, DrawTargetTexture2D source, int w, int h, Microsoft.Xna.Framework.Graphics.SurfaceFormat targetFormat)
		{
			DrawTargetTexture2D target = null;
			if (int1 != null && int1.Equals(source))
				target = int2;
			else
				if (int2 != null && int2.Equals(source))
					target = int1;

			if (target == null)
			{
				int tw = ((w + 15) / 16) * 16;
				int th = ((h + 15) / 16) * 16;

				target = new DrawTargetTexture2D(source.Camera, tw, th, targetFormat);
			}
			else
			{
				if (target.Width < w ||
					target.Height < h)
				{
					string from = "intermediate";
					if (target == int2)
						from += "2";

					throw new ArgumentException(string.Format("'{0}' draw target is too small, minimum required size for '{1}' in the current context is {2}x{3}", from, from, w, h));
				}
			}

			if (int1 == null)
			{
				int1 = target;
			}
			else
			{
				if (int2 == null  &&
					target != int1)
				{
					int2 = target;
				}
			}

			return target.Clone(false, false, false);
		}

		class Drawer : IDraw
		{
			private static int directionId = -1;
			private static int texutreId = -1;

			private Vector2 direction;
			private ShaderElement drawable;
			private DrawTargetTexture2D source;

			public Drawer(Vector2 direction, ShaderElement drawable, DrawTargetTexture2D source)
			{
				drawable.SetTextureSize(source.Width, source.Height);
				this.drawable = drawable;
				this.source = source;
				this.direction = direction;
			}

			public void Draw(DrawState state)
			{
				if (directionId == -1)
				{
					directionId = state.GetShaderAttributeNameUniqueID("sampleDirection");
					texutreId = state.GetShaderAttributeNameUniqueID("Texture");
				}

				drawable.Shader.SetAttribute(state, directionId, ref direction);
				drawable.Shader.SetTexture(state, texutreId, source.GetTexture());

				drawable.Draw(state);
			}

			bool ICullable.CullTest(ICuller culler)
			{
				return true;
			}
		}

		/// <summary>
		/// Perform the texture downsample filter
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			foreach (IDraw draw in passes)
				draw.Draw(state);
		}

		bool ICullable.CullTest(ICuller culler)
		{
			return true;
		}
	}
}
