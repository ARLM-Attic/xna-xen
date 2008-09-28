namespace Xen.Ex.Shaders
{
	
	
	/// <summary><para>VS: approximately 5 instruction slots used</para><para>PS: approximately 1 instruction slot used</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	public sealed class FillVertexColour : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public FillVertexColour()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.v_0 = -1;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			FillVertexColour.init_gd = state.DeviceUniqueIndex;
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(4);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,92,131,0,1,156,143,0,1,36,143,0,5,156,0,1,0,2,138,0,2,16,33,131,0,1,1,131,0,1,2,131,0,30,1,0,0,2,144,0,16,0,3,0,48,160,4,0,0,240,160,0,0,16,9,48,5,32,3,0,0,18,0,196,133,0,7,80,5,0,0,18,0,194,133,0,7,16,10,16,11,18,0,34,131,0,8,5,248,16,0,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,4,200,1,0,2,132,0,8,175,1,0,0,200,2,0,2,132,0,8,175,1,1,0,200,4,0,2,132,0,8,175,1,2,0,200,8,0,2,132,0,7,175,1,3,0,201,15,128,133,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,72,131,0,1,60,143,0,1,36,143,0,2,60,16,141,0,2,16,33,131,0,1,1,131,0,5,1,0,0,240,160,132,0,5,16,2,196,0,18,133,0,5,16,3,0,0,34,133,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,97,128,0,0,15,144,31,0,0,2,10,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,15,208,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,21,128,0,0,15,144,1,0,0,2,0,8,15,128,0,0,228,144,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == FillVertexColour.gd))
			{
				return;
			}
			FillVertexColour.gd = devIndex;
			if ((FillVertexColour.vs != null))
			{
				FillVertexColour.vs.Dispose();
				FillVertexColour.ps.Dispose();
			}
			state.CreateShaders(out FillVertexColour.vs, out FillVertexColour.ps, FillVertexColour.vsb, FillVertexColour.psb, 5, 1, 0, 0);
			if ((FillVertexColour.init_gd != state.DeviceUniqueIndex))
			{
				FillVertexColour._init(state);
			}
		}
		
		private int v_0;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 0, 0, out tc, out ic);
			if ((FillVertexColour.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(FillVertexColour.vs, FillVertexColour.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,10};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = FillVertexColour._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(FillVertexColour._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return this.vreg.change;
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4};
			}
		}
	}
}

namespace Xen.Ex.Shaders
{
	
	
	/// <summary><para>VS: approximately 4 instruction slots used</para><para>PS: approximately 1 instruction slot used</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	public sealed class FillSolidColour : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public FillSolidColour()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.preg.Set(0, 1F, 1F, 1F, 1F);
			this.v_0 = -1;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			FillSolidColour.init_gd = state.DeviceUniqueIndex;
			FillSolidColour.id_0 = state.GetNameUniqueID("FillColour");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(4);
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray preg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(1);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,80,131,0,1,132,143,0,1,36,143,0,5,132,0,1,0,1,143,0,1,1,131,0,1,1,134,0,2,2,144,131,0,10,3,16,1,16,3,0,0,18,0,196,133,0,7,64,4,0,0,18,0,194,133,0,7,16,8,16,9,18,0,34,131,0,2,5,248,131,0,3,36,6,136,132,0,4,200,1,0,1,132,0,1,175,131,0,4,200,2,0,1,132,0,8,175,0,1,0,200,4,0,1,132,0,8,175,0,2,0,200,8,0,1,132,0,8,175,0,3,0,200,15,128,62,132,0,5,226,1,1,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,68,131,0,1,60,143,0,1,36,143,0,2,60,16,150,0,1,1,132,0,5,16,2,196,0,18,133,0,5,16,3,0,0,34,133,0,3,200,15,128,133,0,1,34,131,0,1,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,73,128,0,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,255,255,1,0,0,2,0,8,15,128,0,0,228,160,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == FillSolidColour.gd))
			{
				return;
			}
			FillSolidColour.gd = devIndex;
			if ((FillSolidColour.vs != null))
			{
				FillSolidColour.vs.Dispose();
				FillSolidColour.ps.Dispose();
			}
			state.CreateShaders(out FillSolidColour.vs, out FillSolidColour.ps, FillSolidColour.vsb, FillSolidColour.psb, 4, 1, 0, 0);
			if ((FillSolidColour.init_gd != state.DeviceUniqueIndex))
			{
				FillSolidColour._init(state);
			}
		}
		
		public void SetFillColour(ref Microsoft.Xna.Framework.Vector4 value)
		{
			this.preg.SetVector4(0, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector4 FillColour
		{
			set
			{
				this.SetFillColour(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 0, 0, out tc, out ic);
			if ((FillSolidColour.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(FillSolidColour.vs, FillSolidColour.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if (((this.preg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(null, this.preg.array);
				this.preg.change = false;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector4 value)
		{
			if ((FillSolidColour.init_gd != state.DeviceUniqueIndex))
			{
				FillSolidColour._init(state);
			}
			if ((name_uid == FillSolidColour.id_0))
			{
				this.SetFillColour(ref value);
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0};
		
readonly 
		
		private static int[] _vusage = new int[] {0};
		
		protected override int GetVertexInputCount()
		{
			return 1;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = FillSolidColour._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(FillSolidColour._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change || this.preg.change);
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0,262145,759062677,1};
			}
			else
			{
				return new int[] {0,524292,-888819319,4};
			}
		}
	}
}

namespace Xen.Ex.Shaders
{
	
	
	/// <summary><para>VS: approximately 9 instruction slots used (+ preshader 1 approx. instructions)</para><para>PS: approximately 2 instruction slots used</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	public sealed class NonLinearDepthOutput : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public NonLinearDepthOutput()
		{
			this.vreg_pre.Set(0, 0F, 0F, 0F, 0F);
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(7, 0F, 0F, 0F, 0F);
			this.v_0p = -1;
			this.v_0 = -1;
			this.v_4 = -1;
			this.v_7 = -1;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			NonLinearDepthOutput.init_gd = state.DeviceUniqueIndex;
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(9);
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg_pre = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(1);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,88,131,0,1,204,143,0,1,36,143,0,5,204,0,1,0,2,138,0,2,12,33,131,0,1,1,131,0,1,1,131,0,5,1,0,0,2,144,131,0,18,4,0,0,112,80,0,0,16,13,16,1,16,4,0,0,18,0,196,133,0,7,96,5,48,11,18,0,18,135,0,5,16,14,194,0,18,133,0,5,16,15,0,0,34,133,0,8,5,248,16,0,0,36,6,136,132,0,4,200,1,0,2,132,0,8,175,1,0,0,200,2,0,2,132,0,6,175,1,1,0,200,1,134,0,6,175,1,4,0,200,2,134,0,6,175,1,5,0,200,4,134,0,8,175,1,6,0,200,4,0,2,132,0,9,175,1,2,0,200,7,0,0,2,131,0,8,160,0,7,0,200,8,0,2,132,0,7,175,1,3,0,200,7,128,131,0,10,44,0,161,0,8,0,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,72,131,0,1,60,143,0,1,36,143,0,2,60,16,134,0,1,4,134,0,6,12,33,0,1,0,1,131,0,5,1,0,0,112,80,132,0,5,16,2,196,0,18,133,0,5,16,3,0,0,34,133,0,3,200,15,128,133,0,1,240,131,0,1,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,1,128,0,0,228,144,4,0,228,160,9,0,0,3,0,0,2,128,0,0,228,144,5,0,228,160,9,0,0,3,0,0,4,128,0,0,228,144,6,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,52,2,0,0,3,0,0,7,128,0,0,228,128,7,0,228,161,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,5,0,0,3,0,0,7,224,0,0,228,128,8,0,0,160,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,37,128,0,0,7,176,8,0,0,3,0,0,15,128,0,0,228,176,0,0,228,176,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == NonLinearDepthOutput.gd))
			{
				return;
			}
			NonLinearDepthOutput.gd = devIndex;
			if ((NonLinearDepthOutput.vs != null))
			{
				NonLinearDepthOutput.vs.Dispose();
				NonLinearDepthOutput.ps.Dispose();
			}
			state.CreateShaders(out NonLinearDepthOutput.vs, out NonLinearDepthOutput.ps, NonLinearDepthOutput.vsb, NonLinearDepthOutput.psb, 9, 2, 1, 0);
			if ((NonLinearDepthOutput.init_gd != state.DeviceUniqueIndex))
			{
				NonLinearDepthOutput._init(state);
			}
		}
		
		private int v_0p;
		
		private int v_0;
		
		private int v_4;
		
		private int v_7;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 0, 0, out tc, out ic);
			if ((NonLinearDepthOutput.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(NonLinearDepthOutput.vs, NonLinearDepthOutput.ps);
			}
			state.SetCameraNearFarVector2(this.vreg_pre.Vector2(0), ref this.v_0p);
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			state.SetWorldMatrix(this.vreg.Matrix3Transpose(4), ref this.v_4);
			state.SetViewPointVector4(this.vreg.Vector4(7), ref this.v_7);
			if ((this.vreg_pre.change == true))
			{
				this.vs_pre();
				this.vreg_pre.change = false;
			}
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
		}
		
		private void vs_pre()
		{
			Microsoft.Xna.Framework.Vector4[] con = this.vreg.array;
			Microsoft.Xna.Framework.Vector4[] pre = this.vreg_pre.array;
			bool[] ci = this.vreg_pre.changed;
			bool change = false;
			if ((ci[0] == true))
			{
				con[8].X = (1F / pre[0].Y);
				change = true;
			}
			ci[0] = false;
			this.vreg.change = (this.vreg.change | change);
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0};
		
readonly 
		
		private static int[] _vusage = new int[] {0};
		
		protected override int GetVertexInputCount()
		{
			return 1;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = NonLinearDepthOutput._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(NonLinearDepthOutput._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change || this.vreg_pre.change);
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,524291,-1040038837,7,262145,1606226950,9};
			}
		}
	}
}

namespace Xen.Ex.Shaders
{
	
	
	/// <summary><para>VS: approximately 9 instruction slots used (+ preshader 1 approx. instructions)</para><para>PS: approximately 4 instruction slots used</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	public sealed class LinearDepthOutput : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public LinearDepthOutput()
		{
			this.vreg_pre.Set(0, 0F, 0F, 0F, 0F);
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(7, 0F, 0F, 0F, 0F);
			this.v_0p = -1;
			this.v_0 = -1;
			this.v_4 = -1;
			this.v_7 = -1;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			LinearDepthOutput.init_gd = state.DeviceUniqueIndex;
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(9);
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg_pre = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(1);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,88,131,0,1,204,143,0,1,36,143,0,5,204,0,1,0,2,138,0,2,12,33,131,0,1,1,131,0,1,1,131,0,5,1,0,0,2,144,131,0,18,4,0,0,112,80,0,0,16,13,16,1,16,4,0,0,18,0,196,133,0,7,96,5,48,11,18,0,18,135,0,5,16,14,194,0,18,133,0,5,16,15,0,0,34,133,0,8,5,248,16,0,0,36,6,136,132,0,4,200,1,0,2,132,0,8,175,1,0,0,200,2,0,2,132,0,6,175,1,1,0,200,1,134,0,6,175,1,4,0,200,2,134,0,6,175,1,5,0,200,4,134,0,8,175,1,6,0,200,4,0,2,132,0,9,175,1,2,0,200,7,0,0,2,131,0,8,160,0,7,0,200,8,0,2,132,0,7,175,1,3,0,200,7,128,131,0,10,44,0,161,0,8,0,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,72,131,0,1,84,143,0,1,36,143,0,2,84,16,134,0,1,4,134,0,6,12,33,0,1,0,1,131,0,5,1,0,0,112,80,132,0,5,48,2,196,0,18,133,0,5,16,5,0,0,34,133,0,2,200,1,134,0,1,240,131,0,2,88,128,133,0,8,64,226,0,0,128,76,240,128,133,0,1,226,131,0,1,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,1,128,0,0,228,144,4,0,228,160,9,0,0,3,0,0,2,128,0,0,228,144,5,0,228,160,9,0,0,3,0,0,4,128,0,0,228,144,6,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,52,2,0,0,3,0,0,7,128,0,0,228,128,7,0,228,161,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,5,0,0,3,0,0,7,224,0,0,228,128,8,0,0,160,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,29,128,0,0,7,176,8,0,0,3,0,0,1,128,0,0,228,176,0,0,228,176,7,0,0,2,0,0,8,128,131,0,29,128,6,0,0,2,0,0,15,128,0,0,255,128,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == LinearDepthOutput.gd))
			{
				return;
			}
			LinearDepthOutput.gd = devIndex;
			if ((LinearDepthOutput.vs != null))
			{
				LinearDepthOutput.vs.Dispose();
				LinearDepthOutput.ps.Dispose();
			}
			state.CreateShaders(out LinearDepthOutput.vs, out LinearDepthOutput.ps, LinearDepthOutput.vsb, LinearDepthOutput.psb, 9, 4, 1, 0);
			if ((LinearDepthOutput.init_gd != state.DeviceUniqueIndex))
			{
				LinearDepthOutput._init(state);
			}
		}
		
		private int v_0p;
		
		private int v_0;
		
		private int v_4;
		
		private int v_7;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 0, 0, out tc, out ic);
			if ((LinearDepthOutput.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(LinearDepthOutput.vs, LinearDepthOutput.ps);
			}
			state.SetCameraNearFarVector2(this.vreg_pre.Vector2(0), ref this.v_0p);
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			state.SetWorldMatrix(this.vreg.Matrix3Transpose(4), ref this.v_4);
			state.SetViewPointVector4(this.vreg.Vector4(7), ref this.v_7);
			if ((this.vreg_pre.change == true))
			{
				this.vs_pre();
				this.vreg_pre.change = false;
			}
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
		}
		
		private void vs_pre()
		{
			Microsoft.Xna.Framework.Vector4[] con = this.vreg.array;
			Microsoft.Xna.Framework.Vector4[] pre = this.vreg_pre.array;
			bool[] ci = this.vreg_pre.changed;
			bool change = false;
			if ((ci[0] == true))
			{
				con[8].X = (1F / pre[0].Y);
				change = true;
			}
			ci[0] = false;
			this.vreg.change = (this.vreg.change | change);
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0};
		
readonly 
		
		private static int[] _vusage = new int[] {0};
		
		protected override int GetVertexInputCount()
		{
			return 1;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = LinearDepthOutput._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(LinearDepthOutput._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change || this.vreg_pre.change);
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,524291,-1040038837,7,262145,1606226950,9};
			}
		}
	}
}

