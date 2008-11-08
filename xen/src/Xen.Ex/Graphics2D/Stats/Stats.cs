using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Xen.Graphics;
using Microsoft.Xna.Framework;
using Xen.Graphics.State;
using Microsoft.Xna.Framework.Input;

namespace Xen.Ex.Graphics2D.Statistics
{
	class GraphDrawer : ElementRect
	{
		private Vertices<VertexPositionColor> graph;
		private VertexPositionColor[] graphValues;
		private readonly float[] values;
		private int index;
		private bool dirty;
		private float widthScale, maxValue, maxValueInv;
		private Color colour;
		private float minScale = 0;

		public GraphDrawer(int samples, Color colour, Vector2 size, float minScale) : base(size)
		{
			this.minScale = minScale;
			if (samples < 1)
				throw new ArgumentException("Samples must be 2 or greater");

			this.values = new float[samples];
			graphValues = new VertexPositionColor[samples];

			this.graph = new Vertices<VertexPositionColor>(graphValues);
			this.graph.ResourceUsage = ResourceUsage.Dynamic;

			this.widthScale = 1.0f / ((float)(samples));
			dirty = true;

			this.colour = colour;
			this.AlphaBlendState = AlphaBlendState.Additive;
		}
		
#if XBOX360
		static double log2 = Math.Log(2);
#endif


		public void SetValue(float value)
		{
			values[(index++) % values.Length] = value;
			dirty = true;

			float max = 0;
			for (int i = 0; i < values.Length; i++)
				max = Math.Max(max, values[i]);

			max = Math.Max(minScale, max);
#if XBOX360
			maxValue = (float)Math.Pow(2, Math.Ceiling(Math.Log(max * 1.25) / log2));
#else
			maxValue = (float)Math.Pow(2, Math.Ceiling(Math.Log(max * 1.25, 2)));
#endif
			maxValueInv = 1.0f / maxValue;
		}

		public float MaxValue
		{
			get { return maxValue; }
		}

		protected override void PreDraw(Vector2 size)
		{
			if (dirty)
			{
				float x = widthScale;
				for (int i = 0; i < graphValues.Length; i++)
				{
					int index = (i + this.index) % graphValues.Length;
					graphValues[i]
						= new VertexPositionColor(new Vector3(x, values[index] * maxValueInv, 0), colour);
					x += widthScale;
				}
				dirty = false;
				this.graph.SetDirty();
			}
		}

		protected override void BindShader(DrawState state, bool maskOnly)
		{
			state.GetShader<Xen.Ex.Shaders.FillVertexColour>().Bind(state);
		}

		protected override void DrawElement(DrawState state)
		{
			graph.Draw(state, null, PrimitiveType.LineStrip);
		}

		public void ResetAllGraphValues()
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = 0;
			}
			dirty = true;
		}
	}

	/// <summary>
	/// An element that displays a graph with a title
	/// </summary>
	public class Graph : ElementRect
	{
		private GraphDrawer graph;
		private Vertices<VertexPositionColor> outline;
		private Vertices<VertexPositionColor> background;
		private TextElementRect title, value;
		private float previousValue;

		/// <summary>
		/// Gets/Sets the graph font
		/// </summary>
		public SpriteFont Font
		{
			get
			{
				return title.Font;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException();

				title.Font = value;
				this.value.Font = value;
			}
		}

		/// <summary></summary>
		/// <param name="name"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="samples"></param>
		/// <param name="fontHeight"></param>
		public Graph(string name, int width, int height, int samples, int fontHeight)
			: this(name, width, height, samples, fontHeight, 0, null)
		{
		}
		/// <summary></summary>
		/// <param name="name"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="samples"></param>
		/// <param name="fontHeight"></param>
		/// <param name="minScale"></param>
		public Graph(string name, int width, int height, int samples, int fontHeight, float minScale)
			: this(name, width, height, samples, fontHeight, minScale, null)
		{
		}
		/// <summary></summary>
		/// <param name="name"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="samples"></param>
		/// <param name="fontHeight"></param>
		/// <param name="font"></param>
		/// <param name="minScale"></param>
		public Graph(string name, int width, int height, int samples, int fontHeight, float minScale, SpriteFont font)
			: base(new Vector2((float)width, (float)height), false)
		{
			if (width < fontHeight ||
				height < fontHeight)
				throw new ArgumentException("size is too small for font size");

			graph = new GraphDrawer(samples, Color.Red, new Vector2(), minScale);
			float edge = 0.25f;

			graph.VerticalScaling = ElementScaling.FillToParentPlusSize;
			graph.HorizontalScaling = ElementScaling.FillToParentPlusSize;
			graph.Position = new Vector2((float)(fontHeight) *edge, (float)(fontHeight));
			graph.Size = -graph.Position;

			title = new TextElementRect(Vector2.Zero, name,font);
			title.VerticalAlignment = VerticalAlignment.Top;
			title.HorizontalAlignment = HorizontalAlignment.Left;
			title.HorizontalScaling = ElementScaling.FillToParentPlusSize;
			title.VerticalScaling = ElementScaling.FillToParentPlusSize;
			title.TextHorizontalAlignment = TextHorizontalAlignment.Centre;
			title.TextVerticalAlignment = VerticalAlignment.Top;
			title.Size = new Vector2(0, 0);

			value = new TextElementRect(Vector2.Zero, "0",font);
			value.VerticalAlignment = VerticalAlignment.Bottom;
			value.HorizontalAlignment = HorizontalAlignment.Left;
			value.HorizontalScaling = ElementScaling.FillToParentPlusSize;
			value.VerticalScaling = ElementScaling.FillToParentPlusSize;
			value.TextHorizontalAlignment = TextHorizontalAlignment.Right;
			value.TextVerticalAlignment = VerticalAlignment.Bottom;
			value.Size = new Vector2(0, 0);
			value.Position = new Vector2(-10, 0);

			this.Add(title);
			this.Add(value);
			this.Add(graph);

			float x = fontHeight / ((float)width);
			float y = fontHeight / ((float)height);

			this.AlphaBlendState = AlphaBlendState.Alpha;

			VertexPositionColor[] verts = new VertexPositionColor[]
			{
				new VertexPositionColor(new Vector3(x*edge,y,0),Color.White),
				new VertexPositionColor(new Vector3(1-x*edge,y,0),Color.White),
				new VertexPositionColor(new Vector3(x*edge,y,0),Color.White),
				new VertexPositionColor(new Vector3(x*edge,1-y,0),Color.White),
				
				new VertexPositionColor(new Vector3(edge*x,y + 0.25f * (1-y*2),0),new Color(255,255,255,16)),
				new VertexPositionColor(new Vector3(1-x*edge,y + 0.25f * (1-y*2),0),new Color(255,255,255,16)),
				
				new VertexPositionColor(new Vector3(edge*x,y + 0.5f * (1-y*2),0),new Color(255,255,255,16)),
				new VertexPositionColor(new Vector3(1-x*edge,y + 0.5f * (1-y*2),0),new Color(255,255,255,16)),
				
				new VertexPositionColor(new Vector3(edge*x,y + 0.75f * (1-y*2),0),new Color(255,255,255,16)),
				new VertexPositionColor(new Vector3(1-x*edge,y + 0.75f * (1-y*2),0),new Color(255,255,255,16)),
				
				new VertexPositionColor(new Vector3(edge*x,y,0),new Color(255,255,255,16)),
				new VertexPositionColor(new Vector3(1-x*edge,y,0),new Color(255,255,255,16)),
				
			};

			outline = new Vertices<VertexPositionColor>(verts);

			background = new Vertices<VertexPositionColor>(new VertexPositionColor[]
				{
					new VertexPositionColor(new Vector3(0,0,0),new Color(0,0,0,200)),
					new VertexPositionColor(new Vector3(0,1,0),new Color(0,0,0,200)),
					new VertexPositionColor(new Vector3(1,0,0),new Color(0,0,0,200)),
					new VertexPositionColor(new Vector3(1,1,0),new Color(0,0,0,200)),
				});
		}

		/// <summary></summary>
		public void ResetAllGraphValues()
		{
			this.graph.ResetAllGraphValues();
		}

		/// <summary>Set the next value in the graph</summary>
		/// <param name="value"></param>
		public void SetGraphValue(float value)
		{
			if (value != previousValue)
			{
				this.value.Text.Clear();

				if (value == 0)
				{
					this.value.Text.Append('0');
				}
				else
				{
					if (value > 999 || value < -999)
					{
						//really don't want to allocate here, even though it's tiny, this graph may be used to show GC activity :P
					
						//display number as ###,###,### making it easier to read big numbers
						//this.value.Text.Append(((int)value).ToString("N0"));
						
						
						int i = (int)value;

						int thousands = 1;
						while (Math.Abs(i) >= thousands * 1000 && thousands != 1000000000)
							thousands *= 1000;

						this.value.Text.Append(i / thousands);
						i = Math.Abs(i);

						while (thousands != 1)
						{
							this.value.Text.Append(',');
							int range = i / thousands;
							i -= range * thousands;

							thousands /= 1000;

							range = i / thousands;
							if (range < 10)
								this.value.Text.Append("00");
							else if (range < 100)
								this.value.Text.Append("0");
							this.value.Text.Append(range);
						}
					}
					else
					{
						if (Math.Abs(value) > 1 && Math.Abs(Math.Floor(value) - value) < 1e-10f)
							this.value.Text.Append((int)value);
						else
							this.value.Text.Append(value);
					}
				}
			}
			this.graph.SetValue(value);
			previousValue = value;
		}

		/// <summary></summary>
		/// <param name="size"></param>
		protected override void PreDraw(Vector2 size)
		{
		}
		/// <summary></summary>
		/// <param name="state"></param>
		/// <param name="maskOnly"></param>
		protected override void BindShader(DrawState state, bool maskOnly)
		{
			state.GetShader<Xen.Ex.Shaders.FillVertexColour>().Bind(state);
		}
		/// <summary></summary>
		/// <param name="state"></param>
		protected override void DrawElement(DrawState state)
		{
			background.Draw(state, null, PrimitiveType.TriangleStrip);
			outline.Draw(state, null, PrimitiveType.LineList);
		}
	}

	
#if XBOX360
	class ThreadActivity
	{
		private System.Threading.Thread thread;
		private float usage = 1;
		private bool active = false;
		System.Diagnostics.Stopwatch timer;
		System.Threading.ManualResetEvent block = new System.Threading.ManualResetEvent(false); 

		public float Usage
		{
			get { return usage; }
		}

		private static int[] threadIds = new int[] { 1, 3, 4, 5 };
		private int[] affinity;

		public ThreadActivity(int index)
		{
			this.affinity = new int[] { threadIds[index] };

			this.thread = new System.Threading.Thread(Process);
			this.thread.Priority = System.Threading.ThreadPriority.BelowNormal;
			this.thread.IsBackground = true;
			this.thread.Start();
		}

		public void Start()
		{
			block.Set();
			active = true;
			usage = 0;
		}

		public void Stop()
		{
			block.Reset();
			active = false;
		}

		void Process()
		{
			if (timer == null)
				timer = new System.Diagnostics.Stopwatch();
			this.thread.SetProcessorAffinity(affinity);

			timer.Reset();
			timer.Start();
			int iterations = 0;

			while (true)
			{
				block.WaitOne();

				while (active)
				{
					if (timer.ElapsedTicks > UpdateState.TicksInOneSecond)
					{
						timer.Reset();

						float use = 1 - (float)(iterations) * 0.01f;
						if (use < 0.01f)
							use = 0;
						this.usage = use;

						iterations = 0;
						timer.Start();
					}
					else
					{
						iterations++;

						System.Threading.Thread.Sleep(1);
					}
				}
			}
		}
	}
#endif


	/// <summary>
	/// [Debug mode only] Displays various statistics gathered from the previous rendered frame
	/// </summary>
	public class DrawStatisticsDisplay : IDraw, IUpdate
	{
#if DEBUG
		private delegate float Call(ref DrawStatistics stats, DrawState state);
		private Graph[] graphs;
		private Call[] setGraphCalls;
		private DrawStatistics previousFrameOverhead;
		private bool enabled, keyState;
		private TextElement toggleText, toggleTextGamepad, toggleTextDisplay;
		private WeakReference garbageTracker = new WeakReference(null);

#if XBOX360
		ThreadActivity[] threads = new ThreadActivity[4]
			{
				new ThreadActivity(0),
				new ThreadActivity(1),
				new ThreadActivity(2),
				new ThreadActivity(3),
			};
#endif
#endif
		private SpriteFont font;
		private Keys debugKey = Keys.F12;

		/// <summary>
		/// Gets/Sets the key used to toggle the graphs
		/// </summary>
		public Keys DebugToggleKey
		{
			get { return debugKey; }
			set 
			{
				debugKey = value;
#if DEBUG
				toggleText.Text.SetText(string.Format("Press {0} to toggle debug graphs", debugKey.ToString()));
#endif
			}
		}

		/// <summary>Construct the statistics display</summary>
		/// <param name="manager"></param>
		public DrawStatisticsDisplay(UpdateManager manager) : this(manager,null)
		{
		}

		/// <summary>Construct the statistics display</summary>
		/// <param name="manager"></param>
		/// <param name="font"></param>
		public DrawStatisticsDisplay(UpdateManager manager, SpriteFont font)
		{
			this.font = font;

#if DEBUG
			if (manager != null)
				manager.Add(this);

			toggleText = new TextElement("", font);
			toggleText.HorizontalAlignment = HorizontalAlignment.Left;
			toggleText.VerticalAlignment = VerticalAlignment.Top;
			toggleText.Position = new Vector2(50, -40);

			toggleTextGamepad = new TextElement("Hold both thumbsticks to toggle debug graphs", font);
			toggleTextGamepad.HorizontalAlignment = HorizontalAlignment.Left;
			toggleTextGamepad.VerticalAlignment = VerticalAlignment.Top;
			toggleTextGamepad.Position = new Vector2(50, -40);

			DebugToggleKey = Keys.F12;
			
			if (!Resource.EnableResourceTracking(false))
				throw new InvalidOperationException("DrawStatisticsDisplay requires 'Resource.EnableResourceTracking()' to be called during application initalisation. A resource may have already been created without resource tracking enabled first.");
#endif
		}

		/// <summary>
		/// Gets/Sets the font used by the display
		/// </summary>
		public SpriteFont Font
		{
			get { return font; }
			set 
			{
				if (value == null)
					throw new ArgumentNullException();

				if (this.font != value)
				{
					this.font = value;
					
#if DEBUG
					toggleText.Font = font;
					toggleTextGamepad.Font = font;

					if (graphs != null)
						for (int i = 0; i < graphs.Length; i++)
							graphs[i].Font = (value);
#endif
				}
			}
		}

		static float Avg(int total, int count)
		{
			if (count == 0)
				return 0;
			return ((float)total) / ((float)count);
		}

		/// <summary>
		/// Draw the statistics
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
#if DEBUG

			float fade = 8 - state.TotalTimeSeconds * 0.5f;
			if (fade > 1)
				fade = 1;
			if (fade < 0)
				fade = 0;


			if (!enabled)
			{
				if (toggleTextDisplay != null && fade > 0)
				{
					this.toggleTextDisplay.ColourFloat = new Vector4(1, 1, 1, fade);
					toggleTextDisplay.Draw(state);
				}
				return;
			}

			DrawStatistics stats;
			state.GetPreviousFrameStatistics(out stats);

			stats -= previousFrameOverhead;

			if (graphs == null)
			{
				const int width = 210;
				const int height = 128;
				const int fontPix = 20;
				List<Call> calls = new List<Call>();
				
				Callback<Graph,string,Call> add = delegate(string name,Call call) { calls.Add(call); return new Graph(name, width, height, width - fontPix/2, fontPix, 0, font); };
				Callback<Graph,string,Call> addHalf = delegate(string name, Call call) { calls.Add(call); return new Graph(name, width / 2, height, width / 2 - fontPix / 2, fontPix, 0, font); };
				Callback<Graph, string, Call> addHalfMin1 = delegate(string name, Call call) { calls.Add(call); return new Graph(name, width / 2, height, width / 2 - fontPix / 2, fontPix, 1, font); };

				graphs = new Graph[]
				{
					add("Frame Rate (Approx)",delegate(ref DrawStatistics s, DrawState dstate)
						{return (float)dstate.ApproximateFrameRate;}),
					
					add("Frame Draw Time (Ticks)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return s.ApproximateDrawTimeTicks;}),
					
					addHalf("Draw Target Passes",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)s.DrawTargetsPassCount;}),
					
					add("Triangles Drawn",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.TrianglesDrawn);}),
					
					addHalf("Lines Drawn",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.LinesDrawn);}),
					addHalf("Points Drawn",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.PointsDrawn);}),
					
					add("Draw Calls (Total)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.DrawIndexedPrimitiveCallCount + s.DrawPrimitivesCallCount);}),
					addHalf("Draw Calls (Indexed)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.DrawIndexedPrimitiveCallCount);}),
					

					addHalf("Instances Drawn",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.InstancesDrawn);}),
					addHalf("Inst.Batch Size (Avg)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return Avg(s.InstancesDrawn,s.InstancesDrawBatchCount);}),


					
					addHalf("Set Camera Count",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)s.SetCameraCount;}),
					
					addHalf("Shader Bind Count",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)s.ShaderBindCount;}),

					addHalf("Shader Constant Bytes",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.VertexShaderConstantBytesSetTotalCount + s.PixelShaderConstantBytesSetTotalCount);}),
					
					addHalf("VS Constant Bytes (Avg)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return Avg(s.VertexShaderConstantBytesSetTotalCount,s.VertexShaderConstantBytesSetCount);}),
					addHalf("PS Constant Bytes (Avg)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return Avg(s.PixelShaderConstantBytesSetTotalCount,s.PixelShaderConstantBytesSetCount);}),
					
					addHalf("VS Complexity (Avg)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return Avg(s.VertexShaderApproximateInstructionsTotal,s.VertexShaderBoundWithKnownInstructionsCount);}),
					addHalf("PS Complexity (Avg)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return Avg(s.PixelShaderApproximateInstructionsTotal,s.PixelShaderBoundWithKnownInstructionsCount);}),
					
					addHalf("VS Preshader Complexity (Avg)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return Avg(s.VertexShaderApproximatePreshaderInstructionsTotal,s.VertexShaderBoundWithKnownInstructionsCount);}),
					addHalf("PS Preshader Complexity (Avg)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return Avg(s.PixelShaderApproximatePreshaderInstructionsTotal,s.PixelShaderBoundWithKnownInstructionsCount);}),
					
					addHalf("Dirty Render State Count",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)s.DirtyRenderStateCount;}),

					
					add("Vertex Bytes Copied",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.VertexBufferByesCopied+s.DynamicVertexBufferByesCopied);}),
					add("Index Bytes Copied",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.IndexBufferByesCopied+s.DynamicIndexBufferByesCopied);}),
					
					add("Resource Device Bytes (Tracked)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(Resource.GetAllAllocatedDeviceBytes());}),
					addHalf("Resource Count (Tracked)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(Resource.GetResourceCount());}),
					addHalf("Resource Managed Bytes (Tracked)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(Resource.GetAllAllocatedManagedBytes());}),
					addHalf("Unused Resources (Tracked)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(Resource.CountResourcesNotUsedByDevice());}),
					

					
					addHalfMin1("Culler Efficiency (Sphere)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return 1-Avg(s.DefaultCullerTestSphereCulledCount,s.DefaultCullerTestSphereCount);}),
					addHalfMin1("Culler Efficiency (Box)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return 1-Avg(s.DefaultCullerTestBoxCulledCount,s.DefaultCullerTestBoxCount);}),
					
					addHalf("AlphaBlend State Changed",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.RenderStateAlphaBlendChangedCount);}),
					addHalf("AlphaTest State Changed",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.RenderStateAlphaTestChangedCount);}),
					addHalf("StencilTest State Changed",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.RenderStateStencilTestChangedCount);}),
					addHalf("Depth/FrustumCull State Changed",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.RenderStateDepthColourCullChangedCount);}),

					addHalf("Tex Sampler Address Changed",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.TextureSamplerAddressStateChanged);}),
					addHalf("Tex Sampler Filter Changed",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.TextureSamplerFilterStateChanged);}),
					addHalf("Texture Changed",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.TextureUnitTextureChanged);}),
					addHalf("Vertex Texture Changed",delegate(ref DrawStatistics s, DrawState dstate)
					    {return (float)(s.VertexTextureUnitTextureChanged);}),
					
					add("Bound Textures (Avg)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return Avg(s.BoundTextureCount,s.BoundTextureCountTotalSamples);}),
					addHalf("Vertex Textures (Avg)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return Avg(s.BoundVertexTextureCount,s.BoundTextureCountTotalSamples);}),

					addHalf("Garbage Collected",delegate(ref DrawStatistics s, DrawState dstate)
					    {
							if (garbageTracker.Target == null) { garbageTracker.Target = new object(); return 1; }
							return 0;
						}),
					
#if XBOX360
					
					addHalfMin1("Thread (1)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return threads[0].Usage;}),
					addHalfMin1("Thread (3)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return threads[1].Usage;}),
					addHalfMin1("Thread (4)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return threads[2].Usage;}),
					addHalfMin1("Thread (5)",delegate(ref DrawStatistics s, DrawState dstate)
					    {return threads[3].Usage;}),
#endif
					//add("DT",delegate(ref DrawStatistics s, DrawState dstate)
					//    {
					//        return dstate.DeltaTimeTicks;
					//    }),
				};

				setGraphCalls = calls.ToArray();
			}

			for (int i = 0; i < graphs.Length; i++)
				graphs[i].SetGraphValue(setGraphCalls[i](ref stats,state));

			AlignGraphs(state);


			DrawStatistics currentPreDraw;
			state.GetCurrentFrameStatistics(out currentPreDraw);


			for (int i = 0; i < graphs.Length; i++)
				graphs[i].Draw(state);


			DrawStatistics currentPostDraw;
			state.GetCurrentFrameStatistics(out currentPostDraw);

			previousFrameOverhead = currentPostDraw - currentPreDraw;
#endif
		}
		
#if DEBUG
		private void AlignGraphs(DrawState state)
		{
			if (state.DrawTarget == null)
				return;

			Vector2 size = state.DrawTarget.Size;
			float x=0, y=0,height=0;
			for (int i = 0; i < graphs.Length; i++)
			{
				if (x + graphs[i].Size.X > size.X)
				{
					y -= height+2;
					x = 0;
					height = 0;
				}

				graphs[i].HorizontalAlignment = HorizontalAlignment.Left;
				graphs[i].VerticalAlignment = VerticalAlignment.Top;
				graphs[i].Position = new Vector2(x, y);

				height = Math.Max(height, graphs[i].Size.Y);
				x += graphs[i].Size.X+2;
			}
		}
#endif

		bool ICullable.CullTest(ICuller culler)
		{
#if DEBUG
			return true;
#else
			return false;
#endif
		}

		/// <summary>
		/// Toggle the graphs on/off
		/// </summary>
		public void ToggleGraphs()
		{
#if DEBUG
			this.enabled = !enabled;
			if (enabled)
			{
				previousFrameOverhead = new DrawStatistics();
				if (graphs != null)
					foreach (Graph g in graphs)
						g.ResetAllGraphValues();
				garbageTracker.Target = new object();
			}

#if XBOX360

			if (enabled)
			{
				for (int i = 0; i < threads.Length; i++)
					threads[i].Start();
			}
			else
			{
				for (int i = 0; i < threads.Length; i++)
					threads[i].Stop();
			}

#endif
#endif
		}

		UpdateFrequency IUpdate.Update(UpdateState state)
		{
#if DEBUG
			bool kstate = false;
#if !XBOX
			if (state.PlayerInput[0].ControlInput == Xen.Input.ControlInput.KeyboardMouse)
			{
				kstate = state.KeyboardState.KeyState[debugKey];
				toggleTextDisplay = toggleText;
			}
			else
#endif
			{
				bool held = state.PlayerInput[0].InputState.Buttons.LeftStickClick.DownDuration > 0.5f && state.PlayerInput[0].InputState.Buttons.RightStickClick.DownDuration > 0.5f;

				kstate = !held;
				toggleTextDisplay = toggleTextGamepad;
			}

			if (!kstate && this.keyState)
			{
				ToggleGraphs();
			}
			this.keyState = kstate;

			return UpdateFrequency.FullUpdate60hz;
#else
			return UpdateFrequency.Terminate;
#endif
		}
	}
}
