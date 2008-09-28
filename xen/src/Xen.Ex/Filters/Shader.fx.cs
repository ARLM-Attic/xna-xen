namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 22 instruction slots used</para><para>PS: approximately 49 instruction slots used (16 texture, 33 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Kernel16 : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Kernel16()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F);
			this.vreg.Set(16, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(20, 1F, 1F, 0F, 0F);
			this.preg.Set(0, 0F, 0F, 0F, 0F);
			this.a_0 = new Xen.Graphics.ShaderSystem.Constants.DualArray<Microsoft.Xna.Framework.Vector3>(new Xen.Graphics.ShaderSystem.Constants.Vector3Array(this.preg, 0, 16), new Xen.Graphics.ShaderSystem.Constants.Vector3Array(this.vreg, 0, 16));
			this.v_16 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Kernel16.init_gd = state.DeviceUniqueIndex;
			Kernel16.id_0 = state.GetNameUniqueID("kernel");
			Kernel16.id_1 = state.GetNameUniqueID("textureSize");
			Kernel16.ptid_0 = state.GetNameUniqueID("Texture");
			Kernel16.psid_0 = state.GetNameUniqueID("TextureSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(21);
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray preg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(16);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,148,0,0,1,200,143,0,1,36,142,0,6,1,200,0,113,0,10,138,0,2,129,8,131,0,1,1,131,0,1,2,131,0,71,8,0,0,2,144,0,16,0,5,0,32,80,6,0,0,240,80,0,1,241,81,0,2,242,82,0,3,243,83,0,4,244,84,0,5,245,85,0,6,246,86,0,7,247,87,0,0,16,28,0,0,16,29,0,0,16,30,0,0,16,31,0,0,16,32,0,0,16,33,0,0,15,16,34,0,0,16,35,48,5,32,5,0,0,18,0,196,133,0,7,96,7,96,13,18,0,18,133,0,7,96,19,32,25,18,0,18,135,0,5,96,27,194,0,18,133,0,7,48,33,16,36,18,0,34,131,0,8,5,248,48,0,0,36,6,136,132,0,8,3,248,16,0,0,36,6,136,132,0,4,68,65,0,2,131,0,9,64,143,3,16,20,68,130,0,2,131,0,9,128,143,3,17,20,200,4,0,2,132,0,8,175,3,18,0,200,8,0,2,132,0,58,175,3,19,0,200,3,0,3,0,10,0,0,171,0,0,1,200,3,0,4,0,10,0,0,171,0,1,1,200,3,0,5,0,10,0,0,171,0,2,1,200,3,0,6,0,10,0,0,171,0,3,1,200,3,0,7,0,10,59,0,0,171,0,4,1,200,3,0,8,0,10,0,0,171,0,5,1,200,3,0,9,0,10,0,0,171,0,6,1,200,3,0,10,0,10,0,0,171,0,7,1,200,12,0,3,0,0,160,160,171,0,8,1,200,12,0,4,0,60,0,160,160,171,0,9,1,200,12,0,5,0,0,160,160,171,0,10,1,200,12,0,6,0,0,160,160,171,0,11,1,200,12,0,7,0,0,160,160,171,0,12,1,200,12,0,8,0,0,160,160,171,0,13,1,200,12,0,9,0,23,0,160,160,171,0,14,1,200,12,0,10,0,0,160,160,171,0,15,1,200,15,128,62,132,0,7,226,2,2,0,200,15,128,133,0,8,226,3,3,0,200,15,128,1,132,0,8,226,4,4,0,200,15,128,2,132,0,8,226,5,5,0,200,15,128,3,132,0,8,226,6,6,0,200,15,128,4,132,0,8,226,7,7,0,200,15,128,5,132,0,8,226,8,8,0,200,15,128,6,132,0,8,226,9,9,0,200,15,128,7,132,0,5,226,10,10,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,100,0,0,2,52,143,0,1,36,142,0,5,2,52,16,0,15,132,0,1,4,134,0,6,129,8,0,255,0,255,131,0,57,1,0,0,240,80,0,0,241,81,0,0,242,82,0,0,243,83,0,0,244,84,0,0,245,85,0,0,246,86,0,0,247,87,0,0,96,5,96,11,18,0,18,0,5,112,5,87,96,17,96,23,18,0,18,0,5,87,132,0,5,96,29,196,0,18,133,0,7,96,35,64,41,18,0,18,133,0,5,16,45,0,0,34,133,0,9,200,3,0,8,0,10,10,0,226,131,0,66,200,3,0,9,0,10,10,0,226,1,1,0,200,3,0,10,0,10,10,0,226,2,2,0,200,3,0,11,0,10,10,0,226,3,3,0,200,3,0,12,0,10,10,0,226,4,4,0,200,3,0,13,0,10,10,0,226,5,5,0,200,3,0,14,0,10,67,10,0,226,6,6,0,200,3,0,15,0,10,10,0,226,7,7,0,144,0,16,33,31,255,246,136,0,0,64,0,144,0,0,1,31,255,246,136,0,0,64,0,144,0,32,65,31,255,246,136,0,0,64,0,144,0,48,97,31,255,246,136,0,0,64,0,144,68,0,64,129,31,255,246,136,0,0,64,0,144,0,80,161,31,255,246,136,0,0,64,0,144,0,96,193,31,255,246,136,0,0,64,0,144,0,112,225,31,255,246,136,0,0,64,0,144,0,129,1,31,255,246,136,0,0,64,0,144,0,145,33,31,255,246,136,0,69,0,64,0,144,0,161,65,31,255,246,136,0,0,64,0,144,0,177,97,31,255,246,136,0,0,64,0,144,0,193,129,31,255,246,136,0,0,64,0,144,0,209,161,31,255,246,136,0,0,64,0,144,0,225,193,31,255,246,136,0,0,64,0,144,0,241,225,31,255,20,246,136,0,0,64,0,200,15,0,1,0,0,198,0,161,1,1,0,200,15,132,0,8,198,0,171,0,0,1,200,15,132,0,8,198,0,171,2,2,0,200,15,132,0,8,198,0,171,3,3,0,200,15,132,0,8,198,0,171,4,4,0,200,15,132,0,8,198,0,171,5,5,0,200,15,132,0,8,198,0,171,6,6,0,200,15,132,0,8,198,0,171,7,7,0,200,15,132,0,8,198,0,171,8,8,0,200,15,132,0,8,198,0,171,9,9,0,200,15,132,0,8,198,0,171,10,10,0,200,15,132,0,8,198,0,171,11,11,0,200,15,132,0,8,198,0,171,12,12,0,200,15,132,0,8,198,0,171,13,13,0,200,15,132,0,9,198,0,171,14,14,0,200,15,128,131,0,7,198,0,171,15,15,0,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,16,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,17,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,18,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,19,0,228,160,6,0,0,2,0,0,4,128,20,0,0,160,6,0,0,2,0,0,8,128,102,20,0,85,160,4,0,0,4,0,0,3,224,0,0,228,160,0,0,238,128,1,0,228,144,4,0,0,4,1,0,3,224,1,0,228,160,0,0,238,128,1,0,228,144,4,0,0,4,2,0,3,224,2,0,228,160,0,0,238,128,1,0,228,144,4,0,0,4,3,0,3,224,3,0,228,160,0,0,238,128,1,0,228,144,4,0,0,4,4,0,3,224,4,0,228,160,0,0,238,128,1,0,103,228,144,4,0,0,4,5,0,3,224,5,0,228,160,0,0,238,128,1,0,228,144,4,0,0,4,6,0,3,224,6,0,228,160,0,0,238,128,1,0,228,144,4,0,0,4,7,0,3,224,7,0,228,160,0,0,238,128,1,0,228,144,4,0,0,4,0,0,12,224,8,0,68,160,0,0,228,128,1,0,68,144,4,0,0,4,1,0,12,224,9,0,68,160,0,0,228,128,1,0,68,144,4,104,0,0,4,2,0,12,224,10,0,68,160,0,0,228,128,1,0,68,144,4,0,0,4,3,0,12,224,11,0,68,160,0,0,228,128,1,0,68,144,4,0,0,4,4,0,12,224,12,0,68,160,0,0,228,128,1,0,68,144,4,0,0,4,5,0,12,224,13,0,68,160,0,0,228,128,1,0,68,144,4,0,0,4,6,0,12,224,14,0,68,160,0,0,228,128,1,0,68,144,4,0,0,4,7,19,0,12,224,15,0,68,160,0,0,228,128,1,0,68,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,9,128,0,0,15,176,31,0,0,2,131,0,9,128,1,0,15,176,31,0,0,2,131,0,9,128,2,0,15,176,31,0,0,2,131,0,9,128,3,0,15,176,31,0,0,2,131,0,9,128,4,0,15,176,31,0,0,2,131,0,9,128,5,0,15,176,31,0,0,2,131,0,9,128,6,0,15,176,31,0,0,2,131,0,9,128,7,0,15,176,31,0,0,2,131,0,101,144,0,8,15,160,66,0,0,3,7,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,6,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,5,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,4,0,15,128,3,0,228,176,0,8,228,160,66,0,0,3,3,0,15,128,4,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,5,0,228,176,0,8,228,160,102,66,0,0,3,1,0,15,128,6,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,7,0,228,176,0,8,228,160,5,0,0,3,7,0,15,128,7,0,228,128,1,0,170,160,4,0,0,4,6,0,15,128,6,0,228,128,0,0,170,160,7,0,228,128,4,0,0,4,5,0,15,128,5,0,228,128,2,0,170,160,6,0,228,128,4,0,0,4,4,0,15,128,4,0,228,128,3,0,103,170,160,5,0,228,128,4,0,0,4,3,0,15,128,3,0,228,128,4,0,170,160,4,0,228,128,4,0,0,4,2,0,15,128,2,0,228,128,5,0,170,160,3,0,228,128,4,0,0,4,1,0,15,128,1,0,228,128,6,0,170,160,2,0,228,128,4,0,0,4,8,0,15,128,0,0,228,128,7,0,170,160,1,0,228,128,1,0,0,2,7,0,1,128,0,0,170,176,1,0,0,2,7,104,0,2,128,0,0,255,176,1,0,0,2,6,0,1,128,1,0,170,176,1,0,0,2,6,0,2,128,1,0,255,176,1,0,0,2,5,0,1,128,2,0,170,176,1,0,0,2,5,0,2,128,2,0,255,176,1,0,0,2,4,0,1,128,3,0,170,176,1,0,0,2,4,0,2,128,3,0,255,176,1,0,0,2,3,0,1,128,4,0,170,176,1,0,0,2,3,0,2,128,4,0,255,176,1,105,0,0,2,2,0,1,128,5,0,170,176,1,0,0,2,2,0,2,128,5,0,255,176,1,0,0,2,1,0,1,128,6,0,170,176,1,0,0,2,1,0,2,128,6,0,255,176,1,0,0,2,0,0,1,128,7,0,170,176,1,0,0,2,0,0,2,128,7,0,255,176,66,0,0,3,7,0,15,128,7,0,228,128,0,8,228,160,66,0,0,3,6,0,15,128,6,0,228,128,0,8,228,160,66,0,106,0,3,5,0,15,128,5,0,228,128,0,8,228,160,66,0,0,3,4,0,15,128,4,0,228,128,0,8,228,160,66,0,0,3,3,0,15,128,3,0,228,128,0,8,228,160,66,0,0,3,2,0,15,128,2,0,228,128,0,8,228,160,66,0,0,3,1,0,15,128,1,0,228,128,0,8,228,160,66,0,0,3,0,0,15,128,0,0,228,128,0,8,228,160,4,0,0,4,7,0,15,128,7,0,228,128,107,8,0,170,160,8,0,228,128,4,0,0,4,6,0,15,128,6,0,228,128,9,0,170,160,7,0,228,128,4,0,0,4,5,0,15,128,5,0,228,128,10,0,170,160,6,0,228,128,4,0,0,4,4,0,15,128,4,0,228,128,11,0,170,160,5,0,228,128,4,0,0,4,3,0,15,128,3,0,228,128,12,0,170,160,4,0,228,128,4,0,0,4,2,0,15,128,2,0,228,128,13,0,170,160,3,0,228,57,128,4,0,0,4,1,0,15,128,1,0,228,128,14,0,170,160,2,0,228,128,4,0,0,4,0,0,15,128,0,0,228,128,15,0,170,160,1,0,228,128,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Kernel16.gd))
			{
				return;
			}
			Kernel16.gd = devIndex;
			if ((Kernel16.vs != null))
			{
				Kernel16.vs.Dispose();
				Kernel16.ps.Dispose();
			}
			state.CreateShaders(out Kernel16.vs, out Kernel16.ps, Kernel16.vsb, Kernel16.psb, 22, 49, 0, 0);
			if ((Kernel16.init_gd != state.DeviceUniqueIndex))
			{
				Kernel16._init(state);
			}
		}
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.DualArray<Microsoft.Xna.Framework.Vector3> a_0;
		
		public void SetKernel(Microsoft.Xna.Framework.Vector3[] value)
		{
			this.a_0.SetArray(value);
		}
		
		public Xen.Graphics.ShaderSystem.Constants.IArray<Microsoft.Xna.Framework.Vector3> Kernel
		{
			get
			{
				return this.a_0;
			}
		}
		
		private static int id_0;
		
		public void SetTextureSize(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(20, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 TextureSize
		{
			set
			{
				this.SetTextureSize(ref value);
			}
		}
		
		private static int id_1;
		
		private int v_16;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(13330));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState TextureSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Kernel16.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Kernel16.vs, Kernel16.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(16), ref this.v_16);
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
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Kernel16.init_gd != state.DeviceUniqueIndex))
			{
				Kernel16._init(state);
			}
			if ((name_uid == Kernel16.id_1))
			{
				this.SetTextureSize(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Vector3[] value)
		{
			if ((Kernel16.init_gd != state.DeviceUniqueIndex))
			{
				Kernel16._init(state);
			}
			if ((name_uid == Kernel16.id_0))
			{
				this.SetKernel(value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Kernel16.init_gd != state.DeviceUniqueIndex))
			{
				Kernel16._init(state);
			}
			if ((name_uid == Kernel16.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Kernel16.init_gd != state.DeviceUniqueIndex))
			{
				Kernel16._init(state);
			}
			if ((name_uid == Kernel16.psid_0))
			{
				this.TextureSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Kernel16._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Kernel16._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return ((this.vreg.change || this.preg.change) 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0,196624,-1896033488,16};
			}
			else
			{
				return new int[] {0,196624,-1896033488,16,524292,-888819319,20,131073,-1737439929,21};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 14 instruction slots used</para><para>PS: approximately 17 instruction slots used (8 texture, 9 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Kernel8 : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Kernel8()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F);
			this.vreg.Set(8, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(12, 1F, 1F, 0F, 0F);
			this.preg.Set(0, 0F, 0F, 0F, 0F);
			this.a_0 = new Xen.Graphics.ShaderSystem.Constants.DualArray<Microsoft.Xna.Framework.Vector3>(new Xen.Graphics.ShaderSystem.Constants.Vector3Array(this.preg, 0, 8), new Xen.Graphics.ShaderSystem.Constants.Vector3Array(this.vreg, 0, 8));
			this.v_8 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Kernel8.init_gd = state.DeviceUniqueIndex;
			Kernel8.id_0 = state.GetNameUniqueID("kernel");
			Kernel8.id_1 = state.GetNameUniqueID("textureSize");
			Kernel8.ptid_0 = state.GetNameUniqueID("Texture");
			Kernel8.psid_0 = state.GetNameUniqueID("TextureSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(13);
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray preg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(8);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,148,131,0,1,252,143,0,1,36,143,0,5,252,0,113,0,3,138,0,2,65,8,131,0,1,1,131,0,1,2,131,0,70,8,0,0,2,144,0,16,0,4,0,32,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,4,52,84,0,5,53,85,0,6,54,86,0,7,55,87,0,0,16,10,0,0,16,11,0,0,16,12,0,0,16,13,0,0,16,14,0,0,16,15,0,16,0,16,16,0,0,16,17,48,5,32,4,0,0,18,0,196,133,0,7,96,6,96,12,18,0,18,135,0,5,16,18,194,0,18,133,0,5,16,19,0,0,34,133,0,8,5,248,16,0,0,36,6,136,132,0,8,3,248,32,0,0,36,6,136,132,0,4,68,17,0,3,131,0,9,64,143,1,8,12,68,34,0,3,131,0,9,128,143,1,9,12,200,4,0,3,132,0,8,175,1,10,0,200,8,0,3,132,0,7,175,1,11,0,200,3,128,133,0,8,171,0,0,2,200,3,128,1,132,0,8,171,0,1,2,200,3,128,2,132,0,8,171,0,2,2,200,3,128,3,132,0,8,171,0,3,2,200,3,128,4,132,0,8,171,0,4,2,200,3,128,5,132,0,8,171,0,5,2,200,3,128,6,132,0,8,171,0,6,2,200,3,128,7,132,0,8,171,0,7,2,200,15,128,62,132,0,5,226,3,3,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,100,131,0,1,252,143,0,1,36,143,0,4,252,16,2,7,132,0,1,4,134,0,6,65,8,0,255,0,255,131,0,45,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,0,0,52,84,0,0,53,85,0,0,54,86,0,0,55,87,5,85,96,3,32,9,18,0,18,0,0,7,132,0,5,96,11,196,0,18,133,0,7,32,17,16,19,18,0,34,131,0,69,144,0,16,33,31,255,246,136,0,0,64,0,144,0,0,1,31,255,246,136,0,0,64,0,144,0,32,65,31,255,246,136,0,0,64,0,144,0,48,97,31,255,246,136,0,0,64,0,144,0,64,129,31,255,246,136,0,0,64,0,144,0,80,161,31,255,246,136,0,41,0,64,0,144,0,96,193,31,255,246,136,0,0,64,0,144,0,112,225,31,255,246,136,0,0,64,0,200,15,0,1,0,0,198,0,161,1,1,0,200,15,132,0,8,198,0,171,0,0,1,200,15,132,0,8,198,0,171,2,2,0,200,15,132,0,8,198,0,171,3,3,0,200,15,132,0,8,198,0,171,4,4,0,200,15,132,0,8,198,0,171,5,5,0,200,15,132,0,9,198,0,171,6,6,0,200,15,128,131,0,7,198,0,171,7,7,0,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,8,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,9,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,10,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,11,0,228,160,6,0,0,2,0,0,1,128,12,0,0,160,6,0,0,2,0,0,2,128,102,12,0,85,160,4,0,0,4,0,0,3,224,0,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,1,0,3,224,1,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,2,0,3,224,2,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,3,0,3,224,3,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,4,0,3,224,4,0,228,160,0,0,228,128,1,0,66,228,144,4,0,0,4,5,0,3,224,5,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,6,0,3,224,6,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,7,0,3,224,7,0,228,160,0,0,228,128,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,9,128,4,0,3,176,31,0,0,2,131,0,9,128,5,0,3,176,31,0,0,2,131,0,9,128,6,0,3,176,31,0,0,2,131,0,9,128,7,0,3,176,31,0,0,2,131,0,101,144,0,8,15,160,66,0,0,3,7,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,6,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,5,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,4,0,15,128,3,0,228,176,0,8,228,160,66,0,0,3,3,0,15,128,4,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,5,0,228,176,0,8,228,160,102,66,0,0,3,1,0,15,128,6,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,7,0,228,176,0,8,228,160,5,0,0,3,7,0,15,128,7,0,228,128,1,0,170,160,4,0,0,4,6,0,15,128,6,0,228,128,0,0,170,160,7,0,228,128,4,0,0,4,5,0,15,128,5,0,228,128,2,0,170,160,6,0,228,128,4,0,0,4,4,0,15,128,4,0,228,128,3,0,102,170,160,5,0,228,128,4,0,0,4,3,0,15,128,3,0,228,128,4,0,170,160,4,0,228,128,4,0,0,4,2,0,15,128,2,0,228,128,5,0,170,160,3,0,228,128,4,0,0,4,1,0,15,128,1,0,228,128,6,0,170,160,2,0,228,128,4,0,0,4,0,0,15,128,0,0,228,128,7,0,170,160,1,0,228,128,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Kernel8.gd))
			{
				return;
			}
			Kernel8.gd = devIndex;
			if ((Kernel8.vs != null))
			{
				Kernel8.vs.Dispose();
				Kernel8.ps.Dispose();
			}
			state.CreateShaders(out Kernel8.vs, out Kernel8.ps, Kernel8.vsb, Kernel8.psb, 14, 17, 0, 0);
			if ((Kernel8.init_gd != state.DeviceUniqueIndex))
			{
				Kernel8._init(state);
			}
		}
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.DualArray<Microsoft.Xna.Framework.Vector3> a_0;
		
		public void SetKernel(Microsoft.Xna.Framework.Vector3[] value)
		{
			this.a_0.SetArray(value);
		}
		
		public Xen.Graphics.ShaderSystem.Constants.IArray<Microsoft.Xna.Framework.Vector3> Kernel
		{
			get
			{
				return this.a_0;
			}
		}
		
		private static int id_0;
		
		public void SetTextureSize(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(12, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 TextureSize
		{
			set
			{
				this.SetTextureSize(ref value);
			}
		}
		
		private static int id_1;
		
		private int v_8;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(13330));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState TextureSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Kernel8.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Kernel8.vs, Kernel8.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(8), ref this.v_8);
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
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Kernel8.init_gd != state.DeviceUniqueIndex))
			{
				Kernel8._init(state);
			}
			if ((name_uid == Kernel8.id_1))
			{
				this.SetTextureSize(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Vector3[] value)
		{
			if ((Kernel8.init_gd != state.DeviceUniqueIndex))
			{
				Kernel8._init(state);
			}
			if ((name_uid == Kernel8.id_0))
			{
				this.SetKernel(value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Kernel8.init_gd != state.DeviceUniqueIndex))
			{
				Kernel8._init(state);
			}
			if ((name_uid == Kernel8.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Kernel8.init_gd != state.DeviceUniqueIndex))
			{
				Kernel8._init(state);
			}
			if ((name_uid == Kernel8.psid_0))
			{
				this.TextureSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Kernel8._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Kernel8._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return ((this.vreg.change || this.preg.change) 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0,196616,-1896033488,8};
			}
			else
			{
				return new int[] {0,196616,-1896033488,8,524292,-888819319,12,131073,-1737439929,13};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 10 instruction slots used</para><para>PS: approximately 9 instruction slots used (4 texture, 5 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Kernel4 : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Kernel4()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 0F, 0F, 0F, 0F);
			this.vreg.Set(8, 1F, 1F, 0F, 0F);
			this.preg.Set(0, 0F, 0F, 0F, 0F);
			this.a_0 = new Xen.Graphics.ShaderSystem.Constants.DualArray<Microsoft.Xna.Framework.Vector3>(new Xen.Graphics.ShaderSystem.Constants.Vector3Array(this.preg, 0, 4), new Xen.Graphics.ShaderSystem.Constants.Vector3Array(this.vreg, 4, 4));
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Kernel4.init_gd = state.DeviceUniqueIndex;
			Kernel4.id_0 = state.GetNameUniqueID("kernel");
			Kernel4.id_1 = state.GetNameUniqueID("textureSize");
			Kernel4.ptid_0 = state.GetNameUniqueID("Texture");
			Kernel4.psid_0 = state.GetNameUniqueID("TextureSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(9);
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray preg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(4);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,116,131,0,1,204,143,0,1,36,143,0,5,204,0,49,0,3,138,0,2,32,132,131,0,1,1,131,0,1,2,131,0,54,4,0,0,2,144,0,16,0,4,0,32,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,0,16,10,0,0,16,11,0,0,16,12,0,0,16,13,48,5,32,4,0,0,18,0,196,133,0,7,96,6,32,12,18,0,18,135,0,5,16,14,194,0,18,133,0,5,16,15,0,0,34,133,0,8,5,248,16,0,0,36,6,136,132,0,8,3,248,32,0,0,36,6,136,132,0,4,68,17,0,3,131,0,9,64,143,1,0,8,68,34,0,3,131,0,9,128,143,1,1,8,200,4,0,3,132,0,8,175,1,2,0,200,8,0,3,132,0,7,175,1,3,0,200,3,128,133,0,8,171,0,4,2,200,3,128,1,132,0,8,171,0,5,2,200,3,128,2,132,0,8,171,0,6,2,200,3,128,3,132,0,8,171,0,7,2,200,15,128,62,132,0,5,226,3,3,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,84,131,0,1,144,143,0,1,36,143,0,4,144,16,2,3,132,0,1,4,134,0,6,32,132,0,15,0,15,131,0,26,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,0,85,64,2,0,0,18,0,196,133,0,7,64,6,16,10,18,0,34,131,0,62,144,0,16,33,31,255,246,136,0,0,64,0,144,0,0,1,31,255,246,136,0,0,64,0,144,0,32,65,31,255,246,136,0,0,64,0,144,0,48,97,31,255,246,136,0,0,64,0,200,15,0,1,0,0,198,0,161,1,1,0,200,15,132,0,8,198,0,171,0,0,1,200,15,132,0,9,198,0,171,2,2,0,200,15,128,131,0,7,198,0,171,3,3,0,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,6,0,0,2,0,0,1,128,8,0,0,160,6,0,0,2,0,0,2,128,88,8,0,85,160,4,0,0,4,0,0,3,224,4,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,1,0,3,224,5,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,2,0,3,224,6,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,3,0,3,224,7,0,228,160,0,0,228,128,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,101,144,0,8,15,160,66,0,0,3,3,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,1,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,3,0,228,176,0,8,228,160,5,0,0,3,3,0,15,128,3,0,228,128,1,0,170,160,4,0,0,4,2,0,15,128,2,0,228,128,0,0,170,160,60,3,0,228,128,4,0,0,4,1,0,15,128,1,0,228,128,2,0,170,160,2,0,228,128,4,0,0,4,0,0,15,128,0,0,228,128,3,0,170,160,1,0,228,128,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Kernel4.gd))
			{
				return;
			}
			Kernel4.gd = devIndex;
			if ((Kernel4.vs != null))
			{
				Kernel4.vs.Dispose();
				Kernel4.ps.Dispose();
			}
			state.CreateShaders(out Kernel4.vs, out Kernel4.ps, Kernel4.vsb, Kernel4.psb, 10, 9, 0, 0);
			if ((Kernel4.init_gd != state.DeviceUniqueIndex))
			{
				Kernel4._init(state);
			}
		}
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.DualArray<Microsoft.Xna.Framework.Vector3> a_0;
		
		public void SetKernel(Microsoft.Xna.Framework.Vector3[] value)
		{
			this.a_0.SetArray(value);
		}
		
		public Xen.Graphics.ShaderSystem.Constants.IArray<Microsoft.Xna.Framework.Vector3> Kernel
		{
			get
			{
				return this.a_0;
			}
		}
		
		private static int id_0;
		
		public void SetTextureSize(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(8, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 TextureSize
		{
			set
			{
				this.SetTextureSize(ref value);
			}
		}
		
		private static int id_1;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(13330));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState TextureSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Kernel4.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Kernel4.vs, Kernel4.ps);
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
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Kernel4.init_gd != state.DeviceUniqueIndex))
			{
				Kernel4._init(state);
			}
			if ((name_uid == Kernel4.id_1))
			{
				this.SetTextureSize(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Vector3[] value)
		{
			if ((Kernel4.init_gd != state.DeviceUniqueIndex))
			{
				Kernel4._init(state);
			}
			if ((name_uid == Kernel4.id_0))
			{
				this.SetKernel(value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Kernel4.init_gd != state.DeviceUniqueIndex))
			{
				Kernel4._init(state);
			}
			if ((name_uid == Kernel4.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Kernel4.init_gd != state.DeviceUniqueIndex))
			{
				Kernel4._init(state);
			}
			if ((name_uid == Kernel4.psid_0))
			{
				this.TextureSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Kernel4._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Kernel4._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return ((this.vreg.change || this.preg.change) 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0,196612,-1896033488,4};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,196612,-1896033488,8,131073,-1737439929,9};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 8 instruction slots used</para><para>PS: approximately 5 instruction slots used (2 texture, 3 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Kernel2 : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Kernel2()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 0F, 0F, 0F, 0F);
			this.vreg.Set(6, 1F, 1F, 0F, 0F);
			this.preg.Set(0, 0F, 0F, 0F, 0F);
			this.a_0 = new Xen.Graphics.ShaderSystem.Constants.DualArray<Microsoft.Xna.Framework.Vector3>(new Xen.Graphics.ShaderSystem.Constants.Vector3Array(this.preg, 0, 2), new Xen.Graphics.ShaderSystem.Constants.Vector3Array(this.vreg, 4, 2));
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Kernel2.init_gd = state.DeviceUniqueIndex;
			Kernel2.id_0 = state.GetNameUniqueID("kernel");
			Kernel2.id_1 = state.GetNameUniqueID("textureSize");
			Kernel2.ptid_0 = state.GetNameUniqueID("Texture");
			Kernel2.psid_0 = state.GetNameUniqueID("TextureSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(7);
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray preg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(2);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,100,131,0,1,168,143,0,1,36,143,0,5,168,0,17,0,3,138,0,2,16,66,131,0,1,1,131,0,1,2,131,0,38,2,0,0,2,144,0,16,0,3,0,32,80,4,0,0,48,80,0,1,49,81,0,0,16,9,0,0,16,10,48,5,32,3,0,0,18,0,196,133,0,7,96,5,0,0,18,0,194,133,0,7,16,11,16,12,18,0,34,131,0,8,5,248,16,0,0,36,6,136,132,0,8,3,248,32,0,0,36,6,136,132,0,4,68,17,0,3,131,0,9,64,143,1,0,6,68,34,0,3,131,0,9,128,143,1,1,6,200,4,0,3,132,0,8,175,1,2,0,200,8,0,3,132,0,7,175,1,3,0,200,3,128,133,0,8,171,0,4,2,200,3,128,1,132,0,8,171,0,5,2,200,15,128,62,132,0,5,226,3,3,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,76,131,0,1,96,143,0,1,36,143,0,4,96,16,2,1,132,0,1,4,134,0,6,16,66,0,3,0,3,131,0,18,1,0,0,48,80,0,0,49,81,0,5,32,2,0,0,18,0,196,133,0,7,32,4,16,6,18,0,34,131,0,39,144,0,16,33,31,255,246,136,0,0,64,0,144,0,0,1,31,255,246,136,0,0,64,0,200,15,0,1,0,0,198,0,161,1,1,0,200,15,128,131,0,7,198,0,171,0,0,1,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,6,0,0,2,0,0,1,128,6,0,0,160,6,0,0,2,0,0,2,128,48,6,0,85,160,4,0,0,4,0,0,3,224,4,0,228,160,0,0,228,128,1,0,228,144,4,0,0,4,1,0,3,224,5,0,228,160,0,0,228,128,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,89,144,0,8,15,160,66,0,0,3,1,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,5,0,0,3,1,0,15,128,1,0,228,128,1,0,170,160,4,0,0,4,0,0,15,128,0,0,228,128,0,0,170,160,1,0,228,128,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Kernel2.gd))
			{
				return;
			}
			Kernel2.gd = devIndex;
			if ((Kernel2.vs != null))
			{
				Kernel2.vs.Dispose();
				Kernel2.ps.Dispose();
			}
			state.CreateShaders(out Kernel2.vs, out Kernel2.ps, Kernel2.vsb, Kernel2.psb, 8, 5, 0, 0);
			if ((Kernel2.init_gd != state.DeviceUniqueIndex))
			{
				Kernel2._init(state);
			}
		}
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.DualArray<Microsoft.Xna.Framework.Vector3> a_0;
		
		public void SetKernel(Microsoft.Xna.Framework.Vector3[] value)
		{
			this.a_0.SetArray(value);
		}
		
		public Xen.Graphics.ShaderSystem.Constants.IArray<Microsoft.Xna.Framework.Vector3> Kernel
		{
			get
			{
				return this.a_0;
			}
		}
		
		private static int id_0;
		
		public void SetTextureSize(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(6, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 TextureSize
		{
			set
			{
				this.SetTextureSize(ref value);
			}
		}
		
		private static int id_1;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(13330));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState TextureSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Kernel2.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Kernel2.vs, Kernel2.ps);
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
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Kernel2.init_gd != state.DeviceUniqueIndex))
			{
				Kernel2._init(state);
			}
			if ((name_uid == Kernel2.id_1))
			{
				this.SetTextureSize(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Vector3[] value)
		{
			if ((Kernel2.init_gd != state.DeviceUniqueIndex))
			{
				Kernel2._init(state);
			}
			if ((name_uid == Kernel2.id_0))
			{
				this.SetKernel(value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Kernel2.init_gd != state.DeviceUniqueIndex))
			{
				Kernel2._init(state);
			}
			if ((name_uid == Kernel2.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Kernel2.init_gd != state.DeviceUniqueIndex))
			{
				Kernel2._init(state);
			}
			if ((name_uid == Kernel2.psid_0))
			{
				this.TextureSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Kernel2._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Kernel2._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return ((this.vreg.change || this.preg.change) 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0,196610,-1896033488,2};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,196610,-1896033488,6,131073,-1737439929,7};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 13 instruction slots used</para><para>PS: approximately 17 instruction slots used (8 texture, 9 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample8 : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample8()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample8.init_gd = state.DeviceUniqueIndex;
			Downsample8.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample8.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample8.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,188,0,0,1,72,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,9,64,0,0,1,8,0,113,0,2,138,0,2,65,8,131,0,1,1,131,0,1,2,131,0,50,8,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,4,52,84,0,5,53,85,0,6,54,86,0,7,55,87,0,0,16,11,0,27,0,16,12,0,0,16,13,0,0,16,14,0,0,16,18,0,0,16,15,0,0,16,16,0,0,16,17,144,0,9,192,128,0,0,192,64,0,0,192,131,0,1,64,163,0,6,48,5,32,4,0,0,3,18,0,196,133,0,5,96,6,96,12,18,2,0,18,133,0,4,16,18,0,0,3,18,0,194,133,0,3,16,19,16,4,20,18,0,34,131,0,4,5,248,16,0,4,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,3,200,1,0,1,2,132,0,3,175,1,0,4,0,200,2,0,1,2,132,0,4,175,1,1,0,4,200,4,0,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,0,7,0,1,0,171,1,5,0,8,200,3,128,2,0,0,6,0,9,171,1,5,0,200,3,128,3,2,131,0,8,160,0,4,0,200,3,128,5,132,0,8,160,0,4,0,200,3,128,6,9,0,0,11,0,171,1,5,0,200,10,3,128,7,2,0,1,0,171,1,5,5,0,200,3,128,4,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,140,0,0,1,60,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,1,64,131,0,4,252,16,2,7,132,0,1,4,134,0,6,65,8,0,255,0,255,131,0,34,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,0,0,52,84,0,0,53,85,0,0,54,86,0,0,55,87,62,191,0,0,1,5,1,85,1,96,1,3,1,32,1,9,1,18,1,0,1,18,2,0,0,1,7,132,0,2,96,11,3,196,0,18,133,0,1,32,2,17,16,3,19,18,0,1,34,131,0,4,144,0,0,1,5,31,255,246,136,0,6,0,64,0,144,0,16,7,33,31,255,246,136,0,0,8,64,0,144,0,32,65,31,255,9,246,136,0,0,64,0,144,0,48,10,97,31,255,246,136,0,0,64,0,144,11,0,64,129,31,255,246,136,0,0,64,0,12,144,0,80,161,31,255,246,136,0,0,64,0,13,144,0,96,193,31,255,246,136,0,0,64,0,144,13,0,112,225,31,255,246,136,0,0,64,0,200,15,134,0,6,224,0,1,0,200,15,134,0,6,224,2,0,0,200,15,134,0,5,224,3,0,0,200,1,15,134,0,3,224,4,0,3,0,200,15,134,0,1,224,2,5,0,3,0,200,15,134,0,0,1,224,2,6,0,3,0,200,15,134,0,0,1,224,2,7,0,3,0,200,15,1,128,131,0,3,108,0,161,131,0,1,200,135,0,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,254,255,81,0,0,5,5,0,15,160,0,0,128,192,0,0,64,192,131,0,1,192,131,0,5,64,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,160,4,0,0,4,0,0,3,224,102,0,0,228,128,5,0,0,160,1,0,228,144,4,0,0,4,1,0,3,224,0,0,228,128,5,0,85,160,1,0,228,144,4,0,0,4,2,0,3,224,0,0,228,128,5,0,170,160,1,0,228,144,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,5,0,3,224,1,0,228,144,4,0,228,160,4,0,0,4,6,0,3,224,5,0,255,160,0,0,228,128,1,0,38,228,144,4,0,0,4,7,0,3,224,0,0,228,128,5,0,85,161,1,0,228,144,1,0,0,2,4,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {12,0,2,255,255,81,0,0,5,0,0,15,160,131,0,1,62,140,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,9,128,4,0,3,176,31,0,0,2,131,0,9,128,5,0,3,176,31,0,0,2,131,0,9,128,6,0,3,176,31,0,0,2,131,0,9,128,7,0,3,176,31,0,0,2,131,0,92,144,0,8,15,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,7,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,6,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,5,0,15,128,3,0,228,176,0,8,228,160,66,0,0,3,4,0,15,128,4,0,228,176,0,8,228,160,66,0,0,3,3,0,15,93,128,5,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,6,0,228,176,0,8,228,160,66,0,0,3,1,0,15,128,7,0,228,176,0,8,228,160,2,0,0,3,0,0,15,128,0,0,228,128,7,0,228,128,2,0,0,3,0,0,15,128,6,0,228,128,0,0,228,128,2,0,0,3,0,0,15,128,5,0,228,128,0,0,228,128,2,0,0,3,72,0,0,15,128,4,0,228,128,0,0,228,128,2,0,0,3,0,0,15,128,3,0,228,128,0,0,228,128,2,0,0,3,0,0,15,128,2,0,228,128,0,0,228,128,2,0,0,3,0,0,15,128,1,0,228,128,0,0,228,128,5,0,0,3,0,0,15,128,0,0,228,128,131,0,17,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample8.gd))
			{
				return;
			}
			Downsample8.gd = devIndex;
			if ((Downsample8.vs != null))
			{
				Downsample8.vs.Dispose();
				Downsample8.ps.Dispose();
			}
			state.CreateShaders(out Downsample8.vs, out Downsample8.ps, Downsample8.vsb, Downsample8.psb, 14, 18, 0, 0);
			if ((Downsample8.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample8.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample8.vs, Downsample8.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample8.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8._init(state);
			}
			if ((name_uid == Downsample8.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample8.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8._init(state);
			}
			if ((name_uid == Downsample8.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample8.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8._init(state);
			}
			if ((name_uid == Downsample8.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample8._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample8._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 9 instruction slots used</para><para>PS: approximately 9 instruction slots used (4 texture, 5 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample4 : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample4()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample4.init_gd = state.DeviceUniqueIndex;
			Downsample4.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample4.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample4.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,156,0,0,1,24,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,1,64,131,0,5,216,0,49,0,2,138,0,2,32,132,131,0,1,1,131,0,1,2,131,0,45,4,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,0,16,11,0,0,16,12,0,0,16,14,0,0,16,13,144,0,1,192,175,0,0,1,48,1,5,1,32,1,4,1,0,1,0,1,18,1,0,1,196,133,0,0,1,96,2,6,48,3,12,18,0,1,18,135,0,0,1,16,2,15,194,2,0,18,133,0,1,16,2,16,0,2,0,34,133,0,1,5,2,248,16,3,0,0,36,2,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,2,200,1,2,0,2,132,0,2,175,1,3,0,0,200,3,2,0,2,132,0,3,175,1,1,4,0,200,4,0,1,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,2,131,0,6,160,0,4,0,200,3,2,128,3,132,0,6,160,0,4,0,200,3,2,128,2,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,124,131,0,1,208,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,1,64,131,0,4,144,16,2,3,132,0,1,4,134,0,6,32,132,0,15,0,15,131,0,19,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,62,128,191,0,0,1,85,1,64,1,2,1,0,1,0,1,18,1,0,1,196,133,0,0,1,64,1,6,1,16,2,10,18,2,0,34,131,0,3,144,0,0,4,1,31,255,246,5,136,0,0,64,0,6,144,0,16,33,31,255,7,246,136,0,0,64,0,144,8,0,32,65,31,255,246,136,0,9,0,64,0,144,0,48,97,31,255,8,246,136,0,0,64,0,200,15,134,0,6,224,0,1,0,200,15,134,0,4,224,2,0,0,2,200,15,134,0,2,224,3,3,0,0,200,2,15,128,131,0,3,108,0,161,131,0,1,200,135,0,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {12,0,2,254,255,81,0,0,5,5,0,15,160,131,0,1,192,140,0,4,31,0,0,2,131,0,92,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,69,160,4,0,0,4,0,0,3,224,0,0,228,128,5,0,0,160,1,0,228,144,2,0,0,3,1,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,160,1,0,0,2,2,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,255,255,81,0,0,5,0,0,15,160,0,0,128,62,140,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,92,144,0,8,15,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,3,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,1,0,15,128,3,0,228,176,0,8,228,160,2,0,0,3,0,0,15,128,0,0,228,128,3,0,228,128,2,0,0,3,0,0,15,37,128,2,0,228,128,0,0,228,128,2,0,0,3,0,0,15,128,1,0,228,128,0,0,228,128,5,0,0,3,0,0,15,128,0,0,228,128,131,0,17,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample4.gd))
			{
				return;
			}
			Downsample4.gd = devIndex;
			if ((Downsample4.vs != null))
			{
				Downsample4.vs.Dispose();
				Downsample4.ps.Dispose();
			}
			state.CreateShaders(out Downsample4.vs, out Downsample4.ps, Downsample4.vsb, Downsample4.psb, 10, 10, 0, 0);
			if ((Downsample4.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample4.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample4.vs, Downsample4.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample4.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4._init(state);
			}
			if ((name_uid == Downsample4.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample4.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4._init(state);
			}
			if ((name_uid == Downsample4.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample4.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4._init(state);
			}
			if ((name_uid == Downsample4.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample4._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample4._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 6 instruction slots used</para><para>PS: approximately 5 instruction slots used (2 texture, 3 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample2 : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample2()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample2.init_gd = state.DeviceUniqueIndex;
			Downsample2.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample2.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample2.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,100,131,0,1,168,143,0,1,36,143,0,5,168,0,17,0,2,138,0,2,16,66,131,0,1,1,131,0,1,2,131,0,38,2,0,0,2,144,0,16,0,3,0,48,80,4,0,0,48,80,0,1,49,81,0,0,16,9,0,0,16,10,48,5,32,3,0,0,18,0,196,133,0,7,96,5,0,0,18,0,194,133,0,7,16,11,16,12,18,0,34,131,0,8,5,248,16,0,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,4,200,1,0,2,132,0,8,175,1,0,0,200,2,0,2,132,0,8,175,1,1,0,200,4,0,2,132,0,8,175,1,2,0,200,8,0,2,132,0,9,175,1,3,0,200,3,128,0,2,131,0,8,160,0,4,0,200,3,128,1,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,116,131,0,1,160,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,1,64,131,0,4,96,16,2,1,132,0,1,4,134,0,6,16,66,0,3,0,3,131,0,10,1,0,0,48,80,0,0,49,81,63,192,0,0,1,5,1,32,1,2,1,0,1,0,1,18,1,0,1,196,133,0,0,1,32,1,4,1,16,1,6,2,18,0,1,34,131,0,3,144,0,0,4,1,31,255,246,5,136,0,0,64,0,6,144,0,16,33,31,255,7,246,136,0,0,64,0,200,1,15,134,0,5,224,0,1,0,200,2,15,128,131,0,3,108,0,161,131,0,1,200,135,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,2,0,0,3,0,0,3,224,1,0,228,144,4,0,228,161,1,0,0,2,12,1,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {12,0,2,255,255,81,0,0,5,0,0,15,160,131,0,1,63,140,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,65,144,0,8,15,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,1,0,15,128,1,0,228,176,0,8,228,160,2,0,0,3,0,0,15,128,0,0,228,128,1,0,228,128,5,0,0,3,0,0,15,128,0,0,228,128,131,0,17,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample2.gd))
			{
				return;
			}
			Downsample2.gd = devIndex;
			if ((Downsample2.vs != null))
			{
				Downsample2.vs.Dispose();
				Downsample2.ps.Dispose();
			}
			state.CreateShaders(out Downsample2.vs, out Downsample2.ps, Downsample2.vsb, Downsample2.psb, 6, 6, 0, 0);
			if ((Downsample2.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample2.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample2.vs, Downsample2.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample2.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2._init(state);
			}
			if ((name_uid == Downsample2.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample2.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2._init(state);
			}
			if ((name_uid == Downsample2.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample2.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2._init(state);
			}
			if ((name_uid == Downsample2.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample2._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample2._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 13 instruction slots used</para><para>PS: approximately 50 instruction slots used (8 texture, 42 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample8LogRGBA : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample8LogRGBA()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample8LogRGBA.init_gd = state.DeviceUniqueIndex;
			Downsample8LogRGBA.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample8LogRGBA.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample8LogRGBA.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,188,0,0,1,72,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,9,64,0,0,1,8,0,113,0,2,138,0,2,65,8,131,0,1,1,131,0,1,2,131,0,50,8,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,4,52,84,0,5,53,85,0,6,54,86,0,7,55,87,0,0,16,11,0,27,0,16,12,0,0,16,13,0,0,16,14,0,0,16,18,0,0,16,15,0,0,16,16,0,0,16,17,144,0,9,192,128,0,0,192,64,0,0,192,131,0,1,64,163,0,6,48,5,32,4,0,0,3,18,0,196,133,0,5,96,6,96,12,18,2,0,18,133,0,4,16,18,0,0,3,18,0,194,133,0,3,16,19,16,4,20,18,0,34,131,0,4,5,248,16,0,4,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,3,200,1,0,1,2,132,0,3,175,1,0,4,0,200,2,0,1,2,132,0,4,175,1,1,0,4,200,4,0,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,0,7,0,1,0,171,1,5,0,8,200,3,128,2,0,0,6,0,9,171,1,5,0,200,3,128,3,2,131,0,8,160,0,4,0,200,3,128,5,132,0,8,160,0,4,0,200,3,128,6,9,0,0,11,0,171,1,5,0,200,10,3,128,7,2,0,1,0,171,1,5,5,0,200,3,128,4,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,140,0,0,2,224,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,8,64,0,0,2,160,16,2,7,132,0,1,4,134,0,6,65,8,0,255,0,255,131,0,38,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,0,0,52,84,0,0,53,85,0,0,54,86,0,0,55,87,63,49,114,24,62,187,0,0,1,5,1,85,1,96,1,6,1,32,2,12,18,3,0,18,0,2,0,7,132,0,3,96,14,196,2,0,18,133,0,2,96,20,3,96,26,18,2,0,18,133,0,2,96,32,3,96,38,18,2,0,18,133,0,2,96,44,3,64,50,18,2,0,18,133,0,2,16,54,3,0,0,34,133,0,1,144,2,0,16,3,33,31,255,4,246,136,0,0,5,64,0,144,0,0,6,1,31,255,246,136,0,7,0,64,0,144,0,32,65,8,31,255,246,136,0,0,64,0,9,144,0,48,97,31,255,246,136,0,10,0,64,0,144,0,64,129,31,255,246,11,136,0,0,64,0,144,0,80,161,31,255,12,246,136,0,0,64,0,144,0,96,193,31,255,13,246,136,0,0,64,0,144,0,112,225,31,255,246,8,136,0,0,64,0,60,16,1,132,0,8,64,226,0,0,129,60,32,1,132,0,8,128,226,0,0,129,60,64,1,132,0,8,192,226,0,0,129,60,128,1,133,0,9,226,0,0,129,60,31,0,1,0,9,0,108,64,161,1,0,128,60,32,133,0,7,128,226,0,0,128,60,64,133,0,6,192,226,0,0,128,60,1,128,134,0,4,226,0,0,128,2,200,15,132,0,4,108,0,171,0,5,0,1,60,16,2,132,0,4,64,226,0,0,4,130,60,32,2,132,0,4,128,226,0,0,4,130,60,64,2,132,0,4,192,226,0,0,4,130,60,128,2,133,0,3,226,0,0,3,130,200,15,132,0,3,108,0,171,4,2,0,0,60,2,16,3,132,0,4,64,226,0,0,4,131,60,32,3,132,0,4,128,226,0,0,4,131,60,64,3,132,0,4,192,226,0,0,4,131,60,128,3,133,0,3,226,0,0,3,131,200,15,132,0,3,108,0,171,4,3,0,0,60,2,16,4,132,0,4,64,226,0,0,4,132,60,32,4,132,0,4,128,226,0,0,4,132,60,64,4,132,0,4,192,226,0,0,4,132,60,128,4,133,0,3,226,0,0,3,132,200,15,132,0,3,108,0,171,4,4,0,0,60,2,16,5,132,0,4,64,226,0,0,4,133,60,32,5,132,0,4,128,226,0,0,4,133,60,64,5,132,0,4,192,226,0,0,4,133,60,128,5,133,0,3,226,0,0,3,133,200,15,132,0,3,108,0,171,4,5,0,0,60,2,16,6,132,0,4,64,226,0,0,4,134,60,32,6,132,0,4,128,226,0,0,4,134,60,64,6,132,0,4,192,226,0,0,4,134,60,16,7,132,0,4,64,226,0,0,4,135,60,128,6,133,0,3,226,0,0,3,134,200,15,132,0,3,108,0,171,4,6,0,0,60,2,32,7,132,0,4,128,226,0,0,4,135,60,64,7,132,0,4,192,226,0,0,4,135,60,128,7,133,0,3,226,0,0,3,135,200,15,132,0,3,108,0,171,4,7,0,0,200,2,15,128,131,0,3,177,0,161,131,0,1,200,135,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,254,255,81,0,0,5,5,0,15,160,0,0,128,192,0,0,64,192,131,0,1,192,131,0,5,64,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,160,4,0,0,4,0,0,3,224,102,0,0,228,128,5,0,0,160,1,0,228,144,4,0,0,4,1,0,3,224,0,0,228,128,5,0,85,160,1,0,228,144,4,0,0,4,2,0,3,224,0,0,228,128,5,0,170,160,1,0,228,144,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,5,0,3,224,1,0,228,144,4,0,228,160,4,0,0,4,6,0,3,224,5,0,255,160,0,0,228,128,1,0,38,228,144,4,0,0,4,7,0,3,224,0,0,228,128,5,0,85,161,1,0,228,144,1,0,0,2,4,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,131,0,1,62,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,9,128,4,0,3,176,31,0,0,2,131,0,9,128,5,0,3,176,31,0,0,2,131,0,9,128,6,0,3,176,31,0,0,2,131,0,9,128,7,0,3,176,31,0,0,2,131,0,96,144,0,8,15,160,66,0,0,3,7,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,6,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,5,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,4,0,15,128,3,0,228,176,0,8,228,160,66,0,0,3,3,0,15,128,4,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,5,0,228,97,176,0,8,228,160,66,0,0,3,1,0,15,128,6,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,7,0,228,176,0,8,228,160,15,0,0,2,7,0,1,128,7,0,0,128,15,0,0,2,7,0,2,128,7,0,85,128,15,0,0,2,7,0,4,128,7,0,170,128,15,0,0,2,7,0,8,128,7,0,255,128,5,0,0,3,7,0,15,128,7,0,228,128,131,0,61,160,15,0,0,2,6,0,1,128,6,0,0,128,15,0,0,2,6,0,2,128,6,0,85,128,15,0,0,2,6,0,4,128,6,0,170,128,15,0,0,2,6,0,8,128,6,0,255,128,4,0,0,4,6,0,15,128,6,0,228,128,131,0,65,160,7,0,228,128,15,0,0,2,5,0,1,128,5,0,0,128,15,0,0,2,5,0,2,128,5,0,85,128,15,0,0,2,5,0,4,128,5,0,170,128,15,0,0,2,5,0,8,128,5,0,255,128,4,0,0,4,5,0,15,128,5,0,228,128,131,0,65,160,6,0,228,128,15,0,0,2,4,0,1,128,4,0,0,128,15,0,0,2,4,0,2,128,4,0,85,128,15,0,0,2,4,0,4,128,4,0,170,128,15,0,0,2,4,0,8,128,4,0,255,128,4,0,0,4,4,0,15,128,4,0,228,128,131,0,65,160,5,0,228,128,15,0,0,2,3,0,1,128,3,0,0,128,15,0,0,2,3,0,2,128,3,0,85,128,15,0,0,2,3,0,4,128,3,0,170,128,15,0,0,2,3,0,8,128,3,0,255,128,4,0,0,4,3,0,15,128,3,0,228,128,131,0,65,160,4,0,228,128,15,0,0,2,2,0,1,128,2,0,0,128,15,0,0,2,2,0,2,128,2,0,85,128,15,0,0,2,2,0,4,128,2,0,170,128,15,0,0,2,2,0,8,128,2,0,255,128,4,0,0,4,2,0,15,128,2,0,228,128,131,0,65,160,3,0,228,128,15,0,0,2,1,0,1,128,1,0,0,128,15,0,0,2,1,0,2,128,1,0,85,128,15,0,0,2,1,0,4,128,1,0,170,128,15,0,0,2,1,0,8,128,1,0,255,128,4,0,0,4,1,0,15,128,1,0,228,128,131,0,13,160,2,0,228,128,15,0,0,2,0,0,1,128,131,0,49,128,15,0,0,2,0,0,2,128,0,0,85,128,15,0,0,2,0,0,4,128,0,0,170,128,15,0,0,2,0,0,8,128,0,0,255,128,4,0,0,4,0,0,15,128,0,0,228,128,131,0,37,160,1,0,228,128,5,0,0,3,0,0,15,128,0,0,228,128,0,0,85,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample8LogRGBA.gd))
			{
				return;
			}
			Downsample8LogRGBA.gd = devIndex;
			if ((Downsample8LogRGBA.vs != null))
			{
				Downsample8LogRGBA.vs.Dispose();
				Downsample8LogRGBA.ps.Dispose();
			}
			state.CreateShaders(out Downsample8LogRGBA.vs, out Downsample8LogRGBA.ps, Downsample8LogRGBA.vsb, Downsample8LogRGBA.psb, 14, 51, 0, 0);
			if ((Downsample8LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRGBA._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample8LogRGBA.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample8LogRGBA.vs, Downsample8LogRGBA.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample8LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRGBA._init(state);
			}
			if ((name_uid == Downsample8LogRGBA.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample8LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRGBA._init(state);
			}
			if ((name_uid == Downsample8LogRGBA.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample8LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRGBA._init(state);
			}
			if ((name_uid == Downsample8LogRGBA.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample8LogRGBA._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample8LogRGBA._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 9 instruction slots used</para><para>PS: approximately 26 instruction slots used (4 texture, 22 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample4LogRGBA : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample4LogRGBA()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample4LogRGBA.init_gd = state.DeviceUniqueIndex;
			Downsample4LogRGBA.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample4LogRGBA.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample4LogRGBA.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,156,0,0,1,24,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,1,64,131,0,5,216,0,49,0,2,138,0,2,32,132,131,0,1,1,131,0,1,2,131,0,45,4,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,0,16,11,0,0,16,12,0,0,16,14,0,0,16,13,144,0,1,192,175,0,0,1,48,1,5,1,32,1,4,1,0,1,0,1,18,1,0,1,196,133,0,0,1,96,2,6,48,3,12,18,0,1,18,135,0,0,1,16,2,15,194,2,0,18,133,0,1,16,2,16,0,2,0,34,133,0,1,5,2,248,16,3,0,0,36,2,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,2,200,1,2,0,2,132,0,2,175,1,3,0,0,200,3,2,0,2,132,0,3,175,1,1,4,0,200,4,0,1,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,2,131,0,6,160,0,4,0,200,3,2,128,3,132,0,6,160,0,4,0,200,3,2,128,2,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,124,0,0,1,168,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,8,64,0,0,1,104,16,2,3,132,0,1,4,134,0,6,32,132,0,15,0,15,131,0,23,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,63,49,114,24,62,128,187,0,0,1,85,1,64,1,4,1,0,1,0,2,18,0,1,196,133,0,1,96,2,8,96,3,14,18,0,1,18,133,0,2,96,20,3,32,26,18,2,0,18,133,0,2,16,28,3,0,0,34,133,0,1,144,2,0,16,3,33,31,255,4,246,136,0,0,5,64,0,144,0,0,6,1,31,255,246,136,0,7,0,64,0,144,0,32,65,8,31,255,246,136,0,0,64,0,9,144,0,48,97,31,255,246,136,0,6,0,64,0,60,16,1,132,0,8,64,226,0,0,129,60,32,1,132,0,8,128,226,0,0,129,60,64,1,132,0,7,192,226,0,0,129,60,128,1,1,133,0,6,226,0,0,129,60,31,7,0,1,0,0,108,64,161,5,1,0,128,60,32,133,0,6,128,226,0,0,128,60,1,64,133,0,5,192,226,0,0,128,2,60,128,134,0,3,226,0,0,3,128,200,15,132,0,3,108,0,171,4,0,0,1,60,2,16,2,132,0,4,64,226,0,0,4,130,60,32,2,132,0,4,128,226,0,0,4,130,60,64,2,132,0,4,192,226,0,0,4,130,60,16,3,132,0,4,64,226,0,0,4,131,60,128,2,133,0,3,226,0,0,3,130,200,15,132,0,3,108,0,171,4,2,0,0,60,2,32,3,132,0,4,128,226,0,0,4,131,60,64,3,132,0,4,192,226,0,0,4,131,60,128,3,133,0,3,226,0,0,3,131,200,15,132,0,3,108,0,171,4,3,0,0,200,2,15,128,131,0,3,177,0,161,131,0,1,200,135,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {12,0,2,254,255,81,0,0,5,5,0,15,160,131,0,1,192,140,0,4,31,0,0,2,131,0,92,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,69,160,4,0,0,4,0,0,3,224,0,0,228,128,5,0,0,160,1,0,228,144,2,0,0,3,1,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,160,1,0,0,2,2,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,0,0,128,62,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,96,144,0,8,15,160,66,0,0,3,3,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,1,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,3,0,228,176,0,8,228,160,15,0,0,2,3,0,1,128,3,0,0,128,15,0,0,2,3,0,2,128,3,0,85,128,15,0,0,33,2,3,0,4,128,3,0,170,128,15,0,0,2,3,0,8,128,3,0,255,128,5,0,0,3,3,0,15,128,3,0,228,128,131,0,61,160,15,0,0,2,2,0,1,128,2,0,0,128,15,0,0,2,2,0,2,128,2,0,85,128,15,0,0,2,2,0,4,128,2,0,170,128,15,0,0,2,2,0,8,128,2,0,255,128,4,0,0,4,2,0,15,128,2,0,228,128,131,0,65,160,3,0,228,128,15,0,0,2,1,0,1,128,1,0,0,128,15,0,0,2,1,0,2,128,1,0,85,128,15,0,0,2,1,0,4,128,1,0,170,128,15,0,0,2,1,0,8,128,1,0,255,128,4,0,0,4,1,0,15,128,1,0,228,128,131,0,13,160,2,0,228,128,15,0,0,2,0,0,1,128,131,0,49,128,15,0,0,2,0,0,2,128,0,0,85,128,15,0,0,2,0,0,4,128,0,0,170,128,15,0,0,2,0,0,8,128,0,0,255,128,4,0,0,4,0,0,15,128,0,0,228,128,131,0,37,160,1,0,228,128,5,0,0,3,0,0,15,128,0,0,228,128,0,0,85,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample4LogRGBA.gd))
			{
				return;
			}
			Downsample4LogRGBA.gd = devIndex;
			if ((Downsample4LogRGBA.vs != null))
			{
				Downsample4LogRGBA.vs.Dispose();
				Downsample4LogRGBA.ps.Dispose();
			}
			state.CreateShaders(out Downsample4LogRGBA.vs, out Downsample4LogRGBA.ps, Downsample4LogRGBA.vsb, Downsample4LogRGBA.psb, 10, 27, 0, 0);
			if ((Downsample4LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRGBA._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample4LogRGBA.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample4LogRGBA.vs, Downsample4LogRGBA.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample4LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRGBA._init(state);
			}
			if ((name_uid == Downsample4LogRGBA.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample4LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRGBA._init(state);
			}
			if ((name_uid == Downsample4LogRGBA.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample4LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRGBA._init(state);
			}
			if ((name_uid == Downsample4LogRGBA.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample4LogRGBA._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample4LogRGBA._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 6 instruction slots used</para><para>PS: approximately 14 instruction slots used (2 texture, 12 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample2LogRGBA : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample2LogRGBA()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample2LogRGBA.init_gd = state.DeviceUniqueIndex;
			Downsample2LogRGBA.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample2LogRGBA.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample2LogRGBA.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,100,131,0,1,168,143,0,1,36,143,0,5,168,0,17,0,2,138,0,2,16,66,131,0,1,1,131,0,1,2,131,0,38,2,0,0,2,144,0,16,0,3,0,48,80,4,0,0,48,80,0,1,49,81,0,0,16,9,0,0,16,10,48,5,32,3,0,0,18,0,196,133,0,7,96,5,0,0,18,0,194,133,0,7,16,11,16,12,18,0,34,131,0,8,5,248,16,0,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,4,200,1,0,2,132,0,8,175,1,0,0,200,2,0,2,132,0,8,175,1,1,0,200,4,0,2,132,0,8,175,1,2,0,200,8,0,2,132,0,9,175,1,3,0,200,3,128,0,2,131,0,8,160,0,4,0,200,3,128,1,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,116,0,0,1,12,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,1,64,131,0,4,204,16,2,1,132,0,1,4,134,0,6,16,66,0,3,0,3,131,0,14,1,0,0,48,80,0,0,49,81,63,49,114,24,63,188,0,0,1,5,1,32,1,3,1,0,1,0,1,18,2,0,196,133,0,0,1,96,2,5,64,3,11,18,0,1,18,133,0,2,16,15,3,0,0,34,133,0,1,144,2,0,16,3,33,31,255,4,246,136,0,0,5,64,0,144,0,0,6,1,31,255,246,136,0,6,0,64,0,60,16,1,132,0,6,64,226,0,0,129,60,2,32,1,132,0,6,128,226,0,0,129,60,1,16,133,0,5,64,226,0,0,128,3,60,64,1,132,0,5,192,226,0,0,129,3,60,128,1,133,0,4,226,0,0,129,5,60,47,0,1,0,6,0,108,128,161,1,0,3,128,60,64,133,0,5,192,226,0,0,128,2,60,128,134,0,3,226,0,0,3,128,200,15,132,0,3,108,0,171,4,0,0,1,200,2,15,128,131,0,3,177,0,161,131,0,1,200,135,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,2,0,0,3,0,0,3,224,1,0,228,144,4,0,228,161,1,0,0,2,12,1,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,131,0,1,63,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,96,144,0,8,15,160,66,0,0,3,1,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,15,0,0,2,1,0,1,128,1,0,0,128,15,0,0,2,1,0,2,128,1,0,85,128,15,0,0,2,1,0,4,128,1,0,170,128,15,0,0,2,1,0,8,128,1,0,255,128,5,0,0,3,1,0,15,128,1,0,228,1,128,131,0,9,160,15,0,0,2,0,0,1,128,131,0,49,128,15,0,0,2,0,0,2,128,0,0,85,128,15,0,0,2,0,0,4,128,0,0,170,128,15,0,0,2,0,0,8,128,0,0,255,128,4,0,0,4,0,0,15,128,0,0,228,128,131,0,37,160,1,0,228,128,5,0,0,3,0,0,15,128,0,0,228,128,0,0,85,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample2LogRGBA.gd))
			{
				return;
			}
			Downsample2LogRGBA.gd = devIndex;
			if ((Downsample2LogRGBA.vs != null))
			{
				Downsample2LogRGBA.vs.Dispose();
				Downsample2LogRGBA.ps.Dispose();
			}
			state.CreateShaders(out Downsample2LogRGBA.vs, out Downsample2LogRGBA.ps, Downsample2LogRGBA.vsb, Downsample2LogRGBA.psb, 6, 15, 0, 0);
			if ((Downsample2LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRGBA._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample2LogRGBA.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample2LogRGBA.vs, Downsample2LogRGBA.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample2LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRGBA._init(state);
			}
			if ((name_uid == Downsample2LogRGBA.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample2LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRGBA._init(state);
			}
			if ((name_uid == Downsample2LogRGBA.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample2LogRGBA.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRGBA._init(state);
			}
			if ((name_uid == Downsample2LogRGBA.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample2LogRGBA._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample2LogRGBA._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 13 instruction slots used</para><para>PS: approximately 43 instruction slots used (8 texture, 35 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample8LogRGB : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample8LogRGB()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample8LogRGB.init_gd = state.DeviceUniqueIndex;
			Downsample8LogRGB.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample8LogRGB.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample8LogRGB.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,188,0,0,1,72,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,9,64,0,0,1,8,0,113,0,2,138,0,2,65,8,131,0,1,1,131,0,1,2,131,0,50,8,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,4,52,84,0,5,53,85,0,6,54,86,0,7,55,87,0,0,16,11,0,27,0,16,12,0,0,16,13,0,0,16,14,0,0,16,18,0,0,16,15,0,0,16,16,0,0,16,17,144,0,9,192,128,0,0,192,64,0,0,192,131,0,1,64,163,0,6,48,5,32,4,0,0,3,18,0,196,133,0,5,96,6,96,12,18,2,0,18,133,0,4,16,18,0,0,3,18,0,194,133,0,3,16,19,16,4,20,18,0,34,131,0,4,5,248,16,0,4,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,3,200,1,0,1,2,132,0,3,175,1,0,4,0,200,2,0,1,2,132,0,4,175,1,1,0,4,200,4,0,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,0,7,0,1,0,171,1,5,0,8,200,3,128,2,0,0,6,0,9,171,1,5,0,200,3,128,3,2,131,0,8,160,0,4,0,200,3,128,5,132,0,8,160,0,4,0,200,3,128,6,9,0,0,11,0,171,1,5,0,200,10,3,128,7,2,0,1,0,171,1,5,5,0,200,3,128,4,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,140,0,0,2,140,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,8,64,0,0,2,76,16,2,7,132,0,1,4,134,0,6,65,8,0,255,0,255,131,0,38,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,0,0,52,84,0,0,53,85,0,0,54,86,0,0,55,87,63,49,114,24,62,187,0,0,1,5,1,85,1,96,1,6,1,32,2,12,18,3,0,18,0,2,0,7,132,0,3,96,14,196,2,0,18,133,0,2,96,20,3,96,26,18,2,0,18,133,0,2,96,32,3,96,38,18,2,0,18,133,0,2,32,44,3,16,46,18,2,0,18,133,0,2,16,47,3,0,0,34,133,0,1,144,2,0,16,3,33,31,255,4,254,136,0,0,5,64,0,144,0,0,6,1,31,255,254,136,0,7,0,64,0,144,0,32,65,8,31,255,254,136,0,0,64,0,9,144,0,48,97,31,255,254,136,0,10,0,64,0,144,0,64,129,31,255,254,11,136,0,0,64,0,144,0,80,161,31,255,12,254,136,0,0,64,0,144,0,96,193,31,255,13,254,136,0,0,64,0,144,0,112,225,31,255,254,8,136,0,0,64,0,60,16,1,132,0,8,64,226,0,0,129,60,32,1,132,0,8,128,226,0,0,129,60,64,1,132,0,11,192,226,0,0,129,60,23,0,1,0,0,8,44,64,161,1,0,128,60,32,133,0,7,128,226,0,0,128,60,64,133,0,7,192,226,0,0,128,200,7,132,0,7,44,0,171,0,0,1,60,2,16,2,132,0,7,64,226,0,0,130,60,32,1,2,132,0,7,128,226,0,0,130,60,64,1,2,132,0,7,192,226,0,0,130,200,7,132,0,6,44,0,171,2,0,0,3,60,16,3,132,0,6,64,226,0,0,131,60,2,32,3,132,0,6,128,226,0,0,131,60,2,64,3,132,0,6,192,226,0,0,131,200,1,7,132,0,6,44,0,171,3,0,0,3,60,16,4,132,0,6,64,226,0,0,132,60,2,32,4,132,0,6,128,226,0,0,132,60,2,64,4,132,0,6,192,226,0,0,132,200,1,7,132,0,6,44,0,171,4,0,0,3,60,16,5,132,0,6,64,226,0,0,133,60,2,32,5,132,0,6,128,226,0,0,133,60,2,64,5,132,0,6,192,226,0,0,133,200,1,7,132,0,6,44,0,171,5,0,0,3,60,16,6,132,0,6,64,226,0,0,134,60,2,32,6,132,0,6,128,226,0,0,134,60,2,64,6,132,0,6,192,226,0,0,134,60,2,16,7,132,0,6,64,226,0,0,135,200,1,7,132,0,6,44,0,171,6,0,0,3,60,32,7,132,0,6,128,226,0,0,135,60,2,64,7,132,0,6,192,226,0,0,135,200,1,7,132,0,6,44,0,171,7,0,0,2,20,135,132,0,3,49,198,129,131,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,254,255,81,0,0,5,5,0,15,160,0,0,128,192,0,0,64,192,131,0,1,192,131,0,5,64,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,160,4,0,0,4,0,0,3,224,102,0,0,228,128,5,0,0,160,1,0,228,144,4,0,0,4,1,0,3,224,0,0,228,128,5,0,85,160,1,0,228,144,4,0,0,4,2,0,3,224,0,0,228,128,5,0,170,160,1,0,228,144,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,5,0,3,224,1,0,228,144,4,0,228,160,4,0,0,4,6,0,3,224,5,0,255,160,0,0,228,128,1,0,38,228,144,4,0,0,4,7,0,3,224,0,0,228,128,5,0,85,161,1,0,228,144,1,0,0,2,4,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,131,0,1,62,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,9,128,4,0,3,176,31,0,0,2,131,0,9,128,5,0,3,176,31,0,0,2,131,0,9,128,6,0,3,176,31,0,0,2,131,0,9,128,7,0,3,176,31,0,0,2,131,0,96,144,0,8,15,160,66,0,0,3,7,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,6,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,5,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,4,0,15,128,3,0,228,176,0,8,228,160,66,0,0,3,3,0,15,128,4,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,5,0,228,85,176,0,8,228,160,66,0,0,3,1,0,15,128,6,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,7,0,228,176,0,8,228,160,15,0,0,2,7,0,1,128,7,0,0,128,15,0,0,2,7,0,2,128,7,0,85,128,15,0,0,2,7,0,4,128,7,0,170,128,5,0,0,3,7,0,7,128,7,0,228,128,131,0,49,160,15,0,0,2,6,0,1,128,6,0,0,128,15,0,0,2,6,0,2,128,6,0,85,128,15,0,0,2,6,0,4,128,6,0,170,128,4,0,0,4,6,0,7,128,6,0,228,128,131,0,53,160,7,0,228,128,15,0,0,2,5,0,1,128,5,0,0,128,15,0,0,2,5,0,2,128,5,0,85,128,15,0,0,2,5,0,4,128,5,0,170,128,4,0,0,4,5,0,7,128,5,0,228,128,131,0,53,160,6,0,228,128,15,0,0,2,4,0,1,128,4,0,0,128,15,0,0,2,4,0,2,128,4,0,85,128,15,0,0,2,4,0,4,128,4,0,170,128,4,0,0,4,4,0,7,128,4,0,228,128,131,0,53,160,5,0,228,128,15,0,0,2,3,0,1,128,3,0,0,128,15,0,0,2,3,0,2,128,3,0,85,128,15,0,0,2,3,0,4,128,3,0,170,128,4,0,0,4,3,0,7,128,3,0,228,128,131,0,53,160,4,0,228,128,15,0,0,2,2,0,1,128,2,0,0,128,15,0,0,2,2,0,2,128,2,0,85,128,15,0,0,2,2,0,4,128,2,0,170,128,4,0,0,4,2,0,7,128,2,0,228,128,131,0,53,160,3,0,228,128,15,0,0,2,1,0,1,128,1,0,0,128,15,0,0,2,1,0,2,128,1,0,85,128,15,0,0,2,1,0,4,128,1,0,170,128,4,0,0,4,1,0,7,128,1,0,228,128,131,0,13,160,2,0,228,128,15,0,0,2,0,0,1,128,131,0,37,128,15,0,0,2,0,0,2,128,0,0,85,128,15,0,0,2,0,0,4,128,0,0,170,128,4,0,0,4,0,0,7,128,0,0,228,128,131,0,49,160,1,0,228,128,5,0,0,3,0,0,7,128,0,0,228,128,0,0,85,160,1,0,0,2,0,0,8,128,0,0,170,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample8LogRGB.gd))
			{
				return;
			}
			Downsample8LogRGB.gd = devIndex;
			if ((Downsample8LogRGB.vs != null))
			{
				Downsample8LogRGB.vs.Dispose();
				Downsample8LogRGB.ps.Dispose();
			}
			state.CreateShaders(out Downsample8LogRGB.vs, out Downsample8LogRGB.ps, Downsample8LogRGB.vsb, Downsample8LogRGB.psb, 14, 44, 0, 0);
			if ((Downsample8LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRGB._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample8LogRGB.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample8LogRGB.vs, Downsample8LogRGB.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample8LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRGB._init(state);
			}
			if ((name_uid == Downsample8LogRGB.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample8LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRGB._init(state);
			}
			if ((name_uid == Downsample8LogRGB.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample8LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRGB._init(state);
			}
			if ((name_uid == Downsample8LogRGB.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample8LogRGB._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample8LogRGB._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 9 instruction slots used</para><para>PS: approximately 23 instruction slots used (4 texture, 19 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample4LogRGB : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample4LogRGB()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample4LogRGB.init_gd = state.DeviceUniqueIndex;
			Downsample4LogRGB.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample4LogRGB.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample4LogRGB.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,156,0,0,1,24,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,1,64,131,0,5,216,0,49,0,2,138,0,2,32,132,131,0,1,1,131,0,1,2,131,0,45,4,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,0,16,11,0,0,16,12,0,0,16,14,0,0,16,13,144,0,1,192,175,0,0,1,48,1,5,1,32,1,4,1,0,1,0,1,18,1,0,1,196,133,0,0,1,96,2,6,48,3,12,18,0,1,18,135,0,0,1,16,2,15,194,2,0,18,133,0,1,16,2,16,0,2,0,34,133,0,1,5,2,248,16,3,0,0,36,2,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,2,200,1,2,0,2,132,0,2,175,1,3,0,0,200,3,2,0,2,132,0,3,175,1,1,4,0,200,4,0,1,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,2,131,0,6,160,0,4,0,200,3,2,128,3,132,0,6,160,0,4,0,200,3,2,128,2,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,124,0,0,1,132,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,8,64,0,0,1,68,16,2,3,132,0,1,4,134,0,6,32,132,0,15,0,15,131,0,23,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,63,49,114,24,62,128,187,0,0,1,85,1,64,1,4,1,0,1,0,2,18,0,1,196,133,0,1,96,2,8,96,3,14,18,0,1,18,133,0,2,64,20,3,16,24,18,2,0,18,133,0,2,16,25,3,0,0,34,133,0,1,144,2,0,16,3,33,31,255,4,254,136,0,0,5,64,0,144,0,0,6,1,31,255,254,136,0,7,0,64,0,144,0,32,65,8,31,255,254,136,0,0,64,0,9,144,0,48,97,31,255,254,136,0,6,0,64,0,60,16,1,132,0,8,64,226,0,0,129,60,32,1,132,0,8,128,226,0,0,129,60,64,1,132,0,7,192,226,0,0,129,60,23,8,0,1,0,0,44,64,161,1,4,0,128,60,32,133,0,7,128,226,0,0,128,60,64,133,0,5,192,226,0,0,128,2,200,7,132,0,5,44,0,171,0,0,4,1,60,16,2,132,0,5,64,226,0,0,130,3,60,32,2,132,0,5,128,226,0,0,130,3,60,64,2,132,0,5,192,226,0,0,130,3,60,16,3,132,0,5,64,226,0,0,131,2,200,7,132,0,5,44,0,171,2,0,4,0,60,32,3,132,0,5,128,226,0,0,131,3,60,64,3,132,0,5,192,226,0,0,131,2,200,7,132,0,5,44,0,171,3,0,3,0,20,135,132,0,3,49,198,129,131,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {12,0,2,254,255,81,0,0,5,5,0,15,160,131,0,1,192,140,0,4,31,0,0,2,131,0,92,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,69,160,4,0,0,4,0,0,3,224,0,0,228,128,5,0,0,160,1,0,228,144,2,0,0,3,1,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,160,1,0,0,2,2,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,0,0,128,62,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,96,144,0,8,15,160,66,0,0,3,3,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,1,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,3,0,228,176,0,8,228,160,15,0,0,2,3,0,1,128,3,0,0,128,15,0,0,2,3,0,2,128,3,0,85,128,15,0,0,21,2,3,0,4,128,3,0,170,128,5,0,0,3,3,0,7,128,3,0,228,128,131,0,49,160,15,0,0,2,2,0,1,128,2,0,0,128,15,0,0,2,2,0,2,128,2,0,85,128,15,0,0,2,2,0,4,128,2,0,170,128,4,0,0,4,2,0,7,128,2,0,228,128,131,0,53,160,3,0,228,128,15,0,0,2,1,0,1,128,1,0,0,128,15,0,0,2,1,0,2,128,1,0,85,128,15,0,0,2,1,0,4,128,1,0,170,128,4,0,0,4,1,0,7,128,1,0,228,128,131,0,13,160,2,0,228,128,15,0,0,2,0,0,1,128,131,0,37,128,15,0,0,2,0,0,2,128,0,0,85,128,15,0,0,2,0,0,4,128,0,0,170,128,4,0,0,4,0,0,7,128,0,0,228,128,131,0,49,160,1,0,228,128,5,0,0,3,0,0,7,128,0,0,228,128,0,0,85,160,1,0,0,2,0,0,8,128,0,0,170,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample4LogRGB.gd))
			{
				return;
			}
			Downsample4LogRGB.gd = devIndex;
			if ((Downsample4LogRGB.vs != null))
			{
				Downsample4LogRGB.vs.Dispose();
				Downsample4LogRGB.ps.Dispose();
			}
			state.CreateShaders(out Downsample4LogRGB.vs, out Downsample4LogRGB.ps, Downsample4LogRGB.vsb, Downsample4LogRGB.psb, 10, 24, 0, 0);
			if ((Downsample4LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRGB._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample4LogRGB.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample4LogRGB.vs, Downsample4LogRGB.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample4LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRGB._init(state);
			}
			if ((name_uid == Downsample4LogRGB.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample4LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRGB._init(state);
			}
			if ((name_uid == Downsample4LogRGB.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample4LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRGB._init(state);
			}
			if ((name_uid == Downsample4LogRGB.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample4LogRGB._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample4LogRGB._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 6 instruction slots used</para><para>PS: approximately 13 instruction slots used (2 texture, 11 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample2LogRGB : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample2LogRGB()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample2LogRGB.init_gd = state.DeviceUniqueIndex;
			Downsample2LogRGB.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample2LogRGB.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample2LogRGB.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,100,131,0,1,168,143,0,1,36,143,0,5,168,0,17,0,2,138,0,2,16,66,131,0,1,1,131,0,1,2,131,0,38,2,0,0,2,144,0,16,0,3,0,48,80,4,0,0,48,80,0,1,49,81,0,0,16,9,0,0,16,10,48,5,32,3,0,0,18,0,196,133,0,7,96,5,0,0,18,0,194,133,0,7,16,11,16,12,18,0,34,131,0,8,5,248,16,0,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,4,200,1,0,2,132,0,8,175,1,0,0,200,2,0,2,132,0,8,175,1,1,0,200,4,0,2,132,0,8,175,1,2,0,200,8,0,2,132,0,9,175,1,3,0,200,3,128,0,2,131,0,8,160,0,4,0,200,3,128,1,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,4,116,0,0,1,140,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,1,64,131,0,4,192,16,2,1,132,0,1,4,134,0,6,16,66,0,3,0,3,131,0,14,1,0,0,48,80,0,0,49,81,63,49,114,24,63,188,0,0,1,5,1,32,1,3,1,0,1,0,1,18,1,0,1,196,133,0,0,1,96,2,5,32,3,11,18,0,1,18,133,0,2,16,13,3,16,14,18,2,0,34,131,0,4,144,0,16,33,5,31,255,254,136,0,6,0,64,0,144,0,0,7,1,31,255,254,136,0,0,5,64,0,60,16,1,132,0,7,64,226,0,0,129,60,32,1,1,132,0,7,128,226,0,0,129,60,16,133,0,5,64,226,0,0,128,3,60,64,1,132,0,5,192,226,0,0,129,6,60,39,0,1,0,0,7,44,128,161,1,0,128,60,1,64,133,0,6,192,226,0,0,128,200,1,7,132,0,6,44,0,171,0,0,1,2,20,135,132,0,3,49,198,129,131,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,2,0,0,3,0,0,3,224,1,0,228,144,4,0,228,161,1,0,0,2,12,1,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,131,0,1,63,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,85,144,0,8,15,160,66,0,0,3,1,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,15,0,0,2,1,0,1,128,1,0,0,128,15,0,0,2,1,0,2,128,1,0,85,128,15,0,0,2,1,0,4,128,1,0,170,128,5,0,0,3,1,0,7,128,1,0,228,128,131,0,9,160,15,0,0,2,0,0,1,128,131,0,37,128,15,0,0,2,0,0,2,128,0,0,85,128,15,0,0,2,0,0,4,128,0,0,170,128,4,0,0,4,0,0,7,128,0,0,228,128,131,0,49,160,1,0,228,128,5,0,0,3,0,0,7,128,0,0,228,128,0,0,85,160,1,0,0,2,0,0,8,128,0,0,170,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample2LogRGB.gd))
			{
				return;
			}
			Downsample2LogRGB.gd = devIndex;
			if ((Downsample2LogRGB.vs != null))
			{
				Downsample2LogRGB.vs.Dispose();
				Downsample2LogRGB.ps.Dispose();
			}
			state.CreateShaders(out Downsample2LogRGB.vs, out Downsample2LogRGB.ps, Downsample2LogRGB.vsb, Downsample2LogRGB.psb, 6, 14, 0, 0);
			if ((Downsample2LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRGB._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample2LogRGB.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample2LogRGB.vs, Downsample2LogRGB.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample2LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRGB._init(state);
			}
			if ((name_uid == Downsample2LogRGB.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample2LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRGB._init(state);
			}
			if ((name_uid == Downsample2LogRGB.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample2LogRGB.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRGB._init(state);
			}
			if ((name_uid == Downsample2LogRGB.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample2LogRGB._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample2LogRGB._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 13 instruction slots used</para><para>PS: approximately 35 instruction slots used (8 texture, 27 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample8LogRG : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample8LogRG()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample8LogRG.init_gd = state.DeviceUniqueIndex;
			Downsample8LogRG.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample8LogRG.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample8LogRG.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,188,0,0,1,72,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,9,64,0,0,1,8,0,113,0,2,138,0,2,65,8,131,0,1,1,131,0,1,2,131,0,50,8,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,4,52,84,0,5,53,85,0,6,54,86,0,7,55,87,0,0,16,11,0,27,0,16,12,0,0,16,13,0,0,16,14,0,0,16,18,0,0,16,15,0,0,16,16,0,0,16,17,144,0,9,192,128,0,0,192,64,0,0,192,131,0,1,64,163,0,6,48,5,32,4,0,0,3,18,0,196,133,0,5,96,6,96,12,18,2,0,18,133,0,4,16,18,0,0,3,18,0,194,133,0,3,16,19,16,4,20,18,0,34,131,0,4,5,248,16,0,4,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,3,200,1,0,1,2,132,0,3,175,1,0,4,0,200,2,0,1,2,132,0,4,175,1,1,0,4,200,4,0,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,0,7,0,1,0,171,1,5,0,8,200,3,128,2,0,0,6,0,9,171,1,5,0,200,3,128,3,2,131,0,8,160,0,4,0,200,3,128,5,132,0,8,160,0,4,0,200,3,128,6,9,0,0,11,0,171,1,5,0,200,10,3,128,7,2,0,1,0,171,1,5,5,0,200,3,128,4,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,140,0,0,2,32,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,8,64,0,0,1,224,16,2,7,132,0,1,4,134,0,6,65,8,0,255,0,255,131,0,38,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,0,0,52,84,0,0,53,85,0,0,54,86,0,0,55,87,63,49,114,24,62,187,0,0,1,5,1,85,1,96,1,5,1,32,2,11,18,3,0,18,0,2,0,7,132,0,3,96,13,196,2,0,18,133,0,2,96,19,3,96,25,18,2,0,18,133,0,2,96,31,3,16,37,18,2,0,18,133,0,2,16,38,3,0,0,34,133,0,1,144,2,0,16,3,33,31,255,4,255,200,0,0,5,64,0,144,0,0,6,1,31,255,255,200,0,7,0,64,0,144,0,32,65,8,31,255,255,200,0,0,64,0,9,144,0,48,97,31,255,255,200,0,10,0,64,0,144,0,64,129,31,255,255,11,200,0,0,64,0,144,0,80,161,31,255,12,255,200,0,0,64,0,144,0,96,193,31,255,13,255,200,0,0,64,0,144,0,112,225,31,255,255,8,200,0,0,64,0,60,16,1,132,0,8,64,226,0,0,129,60,32,1,132,0,12,128,226,0,0,129,60,19,0,1,0,0,12,7,64,161,1,0,128,60,32,133,0,7,128,226,0,0,128,200,3,132,0,9,12,0,171,0,0,1,60,16,2,132,0,8,64,226,0,0,130,60,32,2,132,0,7,128,226,0,0,130,200,3,132,0,7,12,0,171,2,0,0,60,2,16,3,132,0,7,64,226,0,0,131,60,32,1,3,132,0,7,128,226,0,0,131,200,3,132,0,6,12,0,171,3,0,0,3,60,16,4,132,0,6,64,226,0,0,132,60,2,32,4,132,0,6,128,226,0,0,132,200,1,3,132,0,6,12,0,171,4,0,0,3,60,16,5,132,0,6,64,226,0,0,133,60,2,32,5,132,0,6,128,226,0,0,133,200,1,3,132,0,6,12,0,171,5,0,0,3,60,16,6,132,0,6,64,226,0,0,134,60,2,32,6,132,0,6,128,226,0,0,134,200,1,3,132,0,6,12,0,171,6,0,0,3,60,16,7,132,0,6,64,226,0,0,135,60,2,32,7,132,0,6,128,226,0,0,135,200,1,3,132,0,6,12,0,171,7,0,0,2,20,195,132,0,3,1,198,129,131,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,254,255,81,0,0,5,5,0,15,160,0,0,128,192,0,0,64,192,131,0,1,192,131,0,5,64,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,160,4,0,0,4,0,0,3,224,102,0,0,228,128,5,0,0,160,1,0,228,144,4,0,0,4,1,0,3,224,0,0,228,128,5,0,85,160,1,0,228,144,4,0,0,4,2,0,3,224,0,0,228,128,5,0,170,160,1,0,228,144,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,5,0,3,224,1,0,228,144,4,0,228,160,4,0,0,4,6,0,3,224,5,0,255,160,0,0,228,128,1,0,38,228,144,4,0,0,4,7,0,3,224,0,0,228,128,5,0,85,161,1,0,228,144,1,0,0,2,4,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,131,0,1,62,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,9,128,4,0,3,176,31,0,0,2,131,0,9,128,5,0,3,176,31,0,0,2,131,0,9,128,6,0,3,176,31,0,0,2,131,0,9,128,7,0,3,176,31,0,0,2,131,0,96,144,0,8,15,160,66,0,0,3,7,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,6,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,5,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,4,0,15,128,3,0,228,176,0,8,228,160,66,0,0,3,3,0,15,128,4,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,5,0,228,73,176,0,8,228,160,66,0,0,3,1,0,15,128,6,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,7,0,228,176,0,8,228,160,15,0,0,2,7,0,1,128,7,0,0,128,15,0,0,2,7,0,2,128,7,0,85,128,5,0,0,3,7,0,3,128,7,0,228,128,131,0,37,160,15,0,0,2,6,0,1,128,6,0,0,128,15,0,0,2,6,0,2,128,6,0,85,128,4,0,0,4,6,0,3,128,6,0,228,128,131,0,41,160,7,0,228,128,15,0,0,2,5,0,1,128,5,0,0,128,15,0,0,2,5,0,2,128,5,0,85,128,4,0,0,4,5,0,3,128,5,0,228,128,131,0,41,160,6,0,228,128,15,0,0,2,4,0,1,128,4,0,0,128,15,0,0,2,4,0,2,128,4,0,85,128,4,0,0,4,4,0,3,128,4,0,228,128,131,0,41,160,5,0,228,128,15,0,0,2,3,0,1,128,3,0,0,128,15,0,0,2,3,0,2,128,3,0,85,128,4,0,0,4,3,0,3,128,3,0,228,128,131,0,41,160,4,0,228,128,15,0,0,2,2,0,1,128,2,0,0,128,15,0,0,2,2,0,2,128,2,0,85,128,4,0,0,4,2,0,3,128,2,0,228,128,131,0,41,160,3,0,228,128,15,0,0,2,1,0,1,128,1,0,0,128,15,0,0,2,1,0,2,128,1,0,85,128,4,0,0,4,1,0,3,128,1,0,228,128,131,0,13,160,2,0,228,128,15,0,0,2,0,0,1,128,131,0,25,128,15,0,0,2,0,0,2,128,0,0,85,128,4,0,0,4,0,0,3,128,0,0,228,128,131,0,49,160,1,0,228,128,5,0,0,3,0,0,3,128,0,0,228,128,0,0,85,160,1,0,0,2,0,0,12,128,0,0,170,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample8LogRG.gd))
			{
				return;
			}
			Downsample8LogRG.gd = devIndex;
			if ((Downsample8LogRG.vs != null))
			{
				Downsample8LogRG.vs.Dispose();
				Downsample8LogRG.ps.Dispose();
			}
			state.CreateShaders(out Downsample8LogRG.vs, out Downsample8LogRG.ps, Downsample8LogRG.vsb, Downsample8LogRG.psb, 14, 36, 0, 0);
			if ((Downsample8LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRG._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample8LogRG.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample8LogRG.vs, Downsample8LogRG.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample8LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRG._init(state);
			}
			if ((name_uid == Downsample8LogRG.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample8LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRG._init(state);
			}
			if ((name_uid == Downsample8LogRG.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample8LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogRG._init(state);
			}
			if ((name_uid == Downsample8LogRG.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample8LogRG._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample8LogRG._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 9 instruction slots used</para><para>PS: approximately 19 instruction slots used (4 texture, 15 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample4LogRG : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample4LogRG()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample4LogRG.init_gd = state.DeviceUniqueIndex;
			Downsample4LogRG.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample4LogRG.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample4LogRG.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,156,0,0,1,24,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,1,64,131,0,5,216,0,49,0,2,138,0,2,32,132,131,0,1,1,131,0,1,2,131,0,45,4,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,0,16,11,0,0,16,12,0,0,16,14,0,0,16,13,144,0,1,192,175,0,0,1,48,1,5,1,32,1,4,1,0,1,0,1,18,1,0,1,196,133,0,0,1,96,2,6,48,3,12,18,0,1,18,135,0,0,1,16,2,15,194,2,0,18,133,0,1,16,2,16,0,2,0,34,133,0,1,5,2,248,16,3,0,0,36,2,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,2,200,1,2,0,2,132,0,2,175,1,3,0,0,200,3,2,0,2,132,0,3,175,1,1,4,0,200,4,0,1,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,2,131,0,6,160,0,4,0,200,3,2,128,3,132,0,6,160,0,4,0,200,3,2,128,2,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,124,0,0,1,72,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,8,64,0,0,1,8,16,2,3,132,0,1,4,134,0,6,32,132,0,15,0,15,131,0,23,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,63,49,114,24,62,128,187,0,0,1,85,1,64,1,3,1,0,1,0,2,18,0,1,196,133,0,1,96,2,7,96,3,13,18,0,1,18,133,0,2,16,19,3,16,20,18,2,0,34,131,0,4,144,0,16,33,5,31,255,255,200,0,6,0,64,0,144,0,0,7,1,31,255,255,200,0,0,8,64,0,144,0,32,65,31,255,9,255,200,0,0,64,0,144,0,48,10,97,31,255,255,200,0,0,64,0,60,2,16,1,132,0,8,64,226,0,0,129,60,32,1,132,0,9,128,226,0,0,129,60,19,0,1,10,0,0,12,64,161,1,0,128,60,32,133,0,7,128,226,0,0,128,200,3,132,0,7,12,0,171,0,0,1,60,2,16,2,132,0,7,64,226,0,0,130,60,32,1,2,132,0,7,128,226,0,0,130,200,3,132,0,6,12,0,171,2,0,0,3,60,16,3,132,0,6,64,226,0,0,131,60,2,32,3,132,0,6,128,226,0,0,131,200,1,3,132,0,6,12,0,171,3,0,0,2,20,195,132,0,3,1,198,129,131,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {12,0,2,254,255,81,0,0,5,5,0,15,160,131,0,1,192,140,0,4,31,0,0,2,131,0,92,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,69,160,4,0,0,4,0,0,3,224,0,0,228,128,5,0,0,160,1,0,228,144,2,0,0,3,1,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,160,1,0,0,2,2,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,0,0,128,62,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,96,144,0,8,15,160,66,0,0,3,3,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,1,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,3,0,228,176,0,8,228,160,15,0,0,2,3,0,1,128,3,0,0,128,15,0,0,2,3,0,2,128,3,0,85,128,5,0,0,9,3,3,0,3,128,3,0,228,128,131,0,37,160,15,0,0,2,2,0,1,128,2,0,0,128,15,0,0,2,2,0,2,128,2,0,85,128,4,0,0,4,2,0,3,128,2,0,228,128,131,0,41,160,3,0,228,128,15,0,0,2,1,0,1,128,1,0,0,128,15,0,0,2,1,0,2,128,1,0,85,128,4,0,0,4,1,0,3,128,1,0,228,128,131,0,13,160,2,0,228,128,15,0,0,2,0,0,1,128,131,0,25,128,15,0,0,2,0,0,2,128,0,0,85,128,4,0,0,4,0,0,3,128,0,0,228,128,131,0,49,160,1,0,228,128,5,0,0,3,0,0,3,128,0,0,228,128,0,0,85,160,1,0,0,2,0,0,12,128,0,0,170,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample4LogRG.gd))
			{
				return;
			}
			Downsample4LogRG.gd = devIndex;
			if ((Downsample4LogRG.vs != null))
			{
				Downsample4LogRG.vs.Dispose();
				Downsample4LogRG.ps.Dispose();
			}
			state.CreateShaders(out Downsample4LogRG.vs, out Downsample4LogRG.ps, Downsample4LogRG.vsb, Downsample4LogRG.psb, 10, 20, 0, 0);
			if ((Downsample4LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRG._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample4LogRG.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample4LogRG.vs, Downsample4LogRG.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample4LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRG._init(state);
			}
			if ((name_uid == Downsample4LogRG.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample4LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRG._init(state);
			}
			if ((name_uid == Downsample4LogRG.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample4LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogRG._init(state);
			}
			if ((name_uid == Downsample4LogRG.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample4LogRG._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample4LogRG._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 6 instruction slots used</para><para>PS: approximately 11 instruction slots used (2 texture, 9 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample2LogRG : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample2LogRG()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample2LogRG.init_gd = state.DeviceUniqueIndex;
			Downsample2LogRG.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample2LogRG.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample2LogRG.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,100,131,0,1,168,143,0,1,36,143,0,5,168,0,17,0,2,138,0,2,16,66,131,0,1,1,131,0,1,2,131,0,38,2,0,0,2,144,0,16,0,3,0,48,80,4,0,0,48,80,0,1,49,81,0,0,16,9,0,0,16,10,48,5,32,3,0,0,18,0,196,133,0,7,96,5,0,0,18,0,194,133,0,7,16,11,16,12,18,0,34,131,0,8,5,248,16,0,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,4,200,1,0,2,132,0,8,175,1,0,0,200,2,0,2,132,0,8,175,1,1,0,200,4,0,2,132,0,8,175,1,2,0,200,8,0,2,132,0,9,175,1,3,0,200,3,128,0,2,131,0,8,160,0,4,0,200,3,128,1,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,116,131,0,1,232,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,1,64,131,0,4,168,16,2,1,132,0,1,4,134,0,6,16,66,0,3,0,3,131,0,14,1,0,0,48,80,0,0,49,81,63,49,114,24,63,188,0,0,1,5,1,32,1,3,1,0,1,0,1,18,2,0,196,133,0,0,1,96,2,5,16,3,11,18,0,1,18,133,0,2,16,12,3,0,0,34,133,0,1,144,2,0,16,3,33,31,255,4,255,200,0,0,5,64,0,144,0,0,6,1,31,255,255,200,0,6,0,64,0,60,16,1,132,0,6,64,226,0,0,129,60,2,32,1,132,0,6,128,226,0,0,129,60,7,19,0,1,0,0,12,64,6,161,1,0,128,60,32,133,0,6,128,226,0,0,128,200,1,3,132,0,6,12,0,171,0,0,1,2,20,195,132,0,3,1,198,129,131,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,2,0,0,3,0,0,3,224,1,0,228,144,4,0,228,161,1,0,0,2,12,1,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,131,0,1,63,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,73,144,0,8,15,160,66,0,0,3,1,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,15,0,0,2,1,0,1,128,1,0,0,128,15,0,0,2,1,0,2,128,1,0,85,128,5,0,0,3,1,0,3,128,1,0,228,128,131,0,9,160,15,0,0,2,0,0,1,128,131,0,25,128,15,0,0,2,0,0,2,128,0,0,85,128,4,0,0,4,0,0,3,128,0,0,228,128,131,0,49,160,1,0,228,128,5,0,0,3,0,0,3,128,0,0,228,128,0,0,85,160,1,0,0,2,0,0,12,128,0,0,170,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample2LogRG.gd))
			{
				return;
			}
			Downsample2LogRG.gd = devIndex;
			if ((Downsample2LogRG.vs != null))
			{
				Downsample2LogRG.vs.Dispose();
				Downsample2LogRG.ps.Dispose();
			}
			state.CreateShaders(out Downsample2LogRG.vs, out Downsample2LogRG.ps, Downsample2LogRG.vsb, Downsample2LogRG.psb, 6, 12, 0, 0);
			if ((Downsample2LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRG._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample2LogRG.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample2LogRG.vs, Downsample2LogRG.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample2LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRG._init(state);
			}
			if ((name_uid == Downsample2LogRG.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample2LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRG._init(state);
			}
			if ((name_uid == Downsample2LogRG.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample2LogRG.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogRG._init(state);
			}
			if ((name_uid == Downsample2LogRG.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample2LogRG._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample2LogRG._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 13 instruction slots used</para><para>PS: approximately 27 instruction slots used (8 texture, 19 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample8LogR : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample8LogR()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample8LogR.init_gd = state.DeviceUniqueIndex;
			Downsample8LogR.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample8LogR.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample8LogR.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,188,0,0,1,72,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,9,64,0,0,1,8,0,113,0,2,138,0,2,65,8,131,0,1,1,131,0,1,2,131,0,50,8,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,4,52,84,0,5,53,85,0,6,54,86,0,7,55,87,0,0,16,11,0,27,0,16,12,0,0,16,13,0,0,16,14,0,0,16,18,0,0,16,15,0,0,16,16,0,0,16,17,144,0,9,192,128,0,0,192,64,0,0,192,131,0,1,64,163,0,6,48,5,32,4,0,0,3,18,0,196,133,0,5,96,6,96,12,18,2,0,18,133,0,4,16,18,0,0,3,18,0,194,133,0,3,16,19,16,4,20,18,0,34,131,0,4,5,248,16,0,4,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,3,200,1,0,1,2,132,0,3,175,1,0,4,0,200,2,0,1,2,132,0,4,175,1,1,0,4,200,4,0,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,0,7,0,1,0,171,1,5,0,8,200,3,128,2,0,0,6,0,9,171,1,5,0,200,3,128,3,2,131,0,8,160,0,4,0,200,3,128,5,132,0,8,160,0,4,0,200,3,128,6,9,0,0,11,0,171,1,5,0,200,10,3,128,7,2,0,1,0,171,1,5,5,0,200,3,128,4,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,140,0,0,1,180,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,8,64,0,0,1,116,16,2,7,132,0,1,4,134,0,6,65,8,0,255,0,255,131,0,38,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,0,0,52,84,0,0,53,85,0,0,54,86,0,0,55,87,63,49,114,24,62,187,0,0,1,5,1,85,1,96,1,4,1,32,2,10,18,3,0,18,0,2,0,7,132,0,3,96,12,196,2,0,18,133,0,2,96,18,3,64,24,18,2,0,18,133,0,2,16,28,3,16,29,18,2,0,34,131,0,4,144,0,16,33,5,31,255,255,248,0,6,0,64,0,144,0,0,7,1,31,255,255,248,0,0,8,64,0,144,0,32,65,31,255,9,255,248,0,0,64,0,144,0,48,10,97,31,255,255,248,0,0,64,0,144,11,0,64,129,31,255,255,248,0,0,64,0,12,144,0,80,161,31,255,255,248,0,0,64,0,13,144,0,96,193,31,255,255,248,0,0,64,0,144,14,0,112,225,31,255,255,248,0,0,64,0,60,128,7,132,0,13,64,226,0,0,129,60,136,7,6,0,0,64,64,14,161,7,0,128,200,8,0,6,0,0,64,0,171,7,5,0,6,60,128,7,132,0,14,64,226,0,0,130,200,8,0,6,0,0,64,0,171,6,7,0,6,60,128,7,132,0,14,64,226,0,0,131,200,8,0,6,0,0,64,0,171,6,7,0,6,60,128,7,132,0,14,64,226,0,0,132,200,8,0,6,0,0,64,0,171,6,7,0,6,60,128,7,132,0,14,64,226,0,0,133,200,8,0,6,0,0,64,0,171,6,7,0,6,60,128,7,132,0,7,64,226,0,0,134,200,8,132,0,9,64,0,171,7,0,6,60,128,1,132,0,7,64,226,0,0,135,200,8,132,0,8,64,0,171,1,0,0,20,225,131,0,4,3,1,198,129,131,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,254,255,81,0,0,5,5,0,15,160,0,0,128,192,0,0,64,192,131,0,1,192,131,0,5,64,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,160,4,0,0,4,0,0,3,224,102,0,0,228,128,5,0,0,160,1,0,228,144,4,0,0,4,1,0,3,224,0,0,228,128,5,0,85,160,1,0,228,144,4,0,0,4,2,0,3,224,0,0,228,128,5,0,170,160,1,0,228,144,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,5,0,3,224,1,0,228,144,4,0,228,160,4,0,0,4,6,0,3,224,5,0,255,160,0,0,228,128,1,0,38,228,144,4,0,0,4,7,0,3,224,0,0,228,128,5,0,85,161,1,0,228,144,1,0,0,2,4,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,131,0,1,62,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,9,128,4,0,3,176,31,0,0,2,131,0,9,128,5,0,3,176,31,0,0,2,131,0,9,128,6,0,3,176,31,0,0,2,131,0,9,128,7,0,3,176,31,0,0,2,131,0,96,144,0,8,15,160,66,0,0,3,7,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,6,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,5,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,4,0,15,128,3,0,228,176,0,8,228,160,66,0,0,3,3,0,15,128,4,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,5,0,228,61,176,0,8,228,160,66,0,0,3,1,0,15,128,6,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,7,0,228,176,0,8,228,160,15,0,0,2,0,0,8,128,7,0,0,128,5,0,0,3,1,0,8,128,0,0,255,128,131,0,25,160,15,0,0,2,0,0,8,128,6,0,0,128,4,0,0,4,1,0,8,128,0,0,255,128,131,0,29,160,1,0,255,128,15,0,0,2,0,0,8,128,5,0,0,128,4,0,0,4,1,0,8,128,0,0,255,128,131,0,29,160,1,0,255,128,15,0,0,2,0,0,8,128,4,0,0,128,4,0,0,4,1,0,8,128,0,0,255,128,131,0,29,160,1,0,255,128,15,0,0,2,0,0,8,128,3,0,0,128,4,0,0,4,1,0,8,128,0,0,255,128,131,0,29,160,1,0,255,128,15,0,0,2,0,0,8,128,2,0,0,128,4,0,0,4,1,0,8,128,0,0,255,128,131,0,29,160,1,0,255,128,15,0,0,2,0,0,8,128,1,0,0,128,4,0,0,4,1,0,8,128,0,0,255,128,131,0,13,160,1,0,255,128,15,0,0,2,0,0,8,128,131,0,13,128,4,0,0,4,0,0,8,128,0,0,255,128,131,0,49,160,1,0,255,128,5,0,0,3,0,0,1,128,0,0,255,128,0,0,85,160,1,0,0,2,0,0,14,128,0,0,170,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample8LogR.gd))
			{
				return;
			}
			Downsample8LogR.gd = devIndex;
			if ((Downsample8LogR.vs != null))
			{
				Downsample8LogR.vs.Dispose();
				Downsample8LogR.ps.Dispose();
			}
			state.CreateShaders(out Downsample8LogR.vs, out Downsample8LogR.ps, Downsample8LogR.vsb, Downsample8LogR.psb, 14, 28, 0, 0);
			if ((Downsample8LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogR._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample8LogR.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample8LogR.vs, Downsample8LogR.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample8LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogR._init(state);
			}
			if ((name_uid == Downsample8LogR.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample8LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogR._init(state);
			}
			if ((name_uid == Downsample8LogR.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample8LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample8LogR._init(state);
			}
			if ((name_uid == Downsample8LogR.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample8LogR._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample8LogR._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 9 instruction slots used</para><para>PS: approximately 15 instruction slots used (4 texture, 11 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample4LogR : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample4LogR()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample4LogR.init_gd = state.DeviceUniqueIndex;
			Downsample4LogR.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample4LogR.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample4LogR.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,156,0,0,1,24,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,1,64,131,0,5,216,0,49,0,2,138,0,2,32,132,131,0,1,1,131,0,1,2,131,0,45,4,0,0,2,144,0,16,0,4,0,48,80,5,0,0,48,80,0,1,49,81,0,2,50,82,0,3,51,83,0,0,16,11,0,0,16,12,0,0,16,14,0,0,16,13,144,0,1,192,175,0,0,1,48,1,5,1,32,1,4,1,0,1,0,1,18,1,0,1,196,133,0,0,1,96,2,6,48,3,12,18,0,1,18,135,0,0,1,16,2,15,194,2,0,18,133,0,1,16,2,16,0,2,0,34,133,0,1,5,2,248,16,3,0,0,36,2,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,2,200,1,2,0,2,132,0,2,175,1,3,0,0,200,3,2,0,2,132,0,3,175,1,1,4,0,200,4,0,1,2,132,0,4,175,1,2,0,4,200,8,0,2,132,0,4,175,1,3,0,4,200,3,0,1,132,0,4,34,4,4,0,3,200,3,128,131,0,5,12,0,171,1,5,6,0,200,3,128,1,2,131,0,6,160,0,4,0,200,3,2,128,3,132,0,6,160,0,4,0,200,3,2,128,2,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,5,124,0,0,1,24,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,1,64,131,0,4,216,16,2,3,132,0,1,4,134,0,6,32,132,0,15,0,15,131,0,23,1,0,0,48,80,0,0,49,81,0,0,50,82,0,0,51,83,63,49,114,24,62,128,187,0,0,1,85,1,64,1,3,1,0,1,0,2,18,0,1,196,133,0,1,96,2,7,32,3,13,18,0,1,18,133,0,2,16,15,3,16,16,18,2,0,34,131,0,4,144,0,16,33,5,31,255,255,248,0,6,0,64,0,144,0,0,7,1,31,255,255,248,0,0,8,64,0,144,0,32,65,31,255,9,255,248,0,0,64,0,144,0,48,10,97,31,255,255,248,0,0,64,0,60,2,128,3,132,0,10,64,226,0,0,129,60,136,3,2,0,11,0,64,64,161,3,0,128,200,8,0,2,11,0,0,64,0,171,3,0,2,60,128,3,132,0,7,64,226,0,0,130,200,8,132,0,9,64,0,171,3,0,2,60,128,1,132,0,7,64,226,0,0,131,200,8,132,0,8,64,0,171,1,0,0,20,225,131,0,4,3,1,198,129,131,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {12,0,2,254,255,81,0,0,5,5,0,15,160,131,0,1,192,140,0,4,31,0,0,2,131,0,92,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,128,4,0,228,69,160,4,0,0,4,0,0,3,224,0,0,228,128,5,0,0,160,1,0,228,144,2,0,0,3,1,0,3,224,1,0,228,144,4,0,228,161,2,0,0,3,3,0,3,224,1,0,228,144,4,0,228,160,1,0,0,2,2,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,0,0,128,62,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,9,128,2,0,3,176,31,0,0,2,131,0,9,128,3,0,3,176,31,0,0,2,131,0,93,144,0,8,15,160,66,0,0,3,3,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,2,0,15,128,0,0,228,176,0,8,228,160,66,0,0,3,1,0,15,128,2,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,3,0,228,176,0,8,228,160,15,0,0,2,0,0,8,128,3,0,0,128,5,0,0,3,1,0,8,128,0,0,255,128,131,0,25,160,15,0,0,2,0,0,8,128,2,0,0,128,4,0,0,4,1,0,8,128,0,0,255,128,131,0,29,160,1,0,255,128,15,0,0,2,0,0,8,128,1,0,0,128,4,0,0,4,1,0,8,128,0,0,255,128,131,0,13,160,1,0,255,128,15,0,0,2,0,0,8,128,131,0,13,128,4,0,0,4,0,0,8,128,0,0,255,128,131,0,49,160,1,0,255,128,5,0,0,3,0,0,1,128,0,0,255,128,0,0,85,160,1,0,0,2,0,0,14,128,0,0,170,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample4LogR.gd))
			{
				return;
			}
			Downsample4LogR.gd = devIndex;
			if ((Downsample4LogR.vs != null))
			{
				Downsample4LogR.vs.Dispose();
				Downsample4LogR.ps.Dispose();
			}
			state.CreateShaders(out Downsample4LogR.vs, out Downsample4LogR.ps, Downsample4LogR.vsb, Downsample4LogR.psb, 10, 16, 0, 0);
			if ((Downsample4LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogR._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample4LogR.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample4LogR.vs, Downsample4LogR.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample4LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogR._init(state);
			}
			if ((name_uid == Downsample4LogR.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample4LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogR._init(state);
			}
			if ((name_uid == Downsample4LogR.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample4LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample4LogR._init(state);
			}
			if ((name_uid == Downsample4LogR.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample4LogR._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample4LogR._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

namespace Xen.Ex.Filters
{
	
	
	/// <summary><para>VS: approximately 6 instruction slots used</para><para>PS: approximately 9 instruction slots used (2 texture, 7 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class Downsample2LogR : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Downsample2LogR()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 1F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Downsample2LogR.init_gd = state.DeviceUniqueIndex;
			Downsample2LogR.id_0 = state.GetNameUniqueID("sampleDirection");
			Downsample2LogR.ptid_0 = state.GetNameUniqueID("Texture");
			Downsample2LogR.psid_0 = state.GetNameUniqueID("PointSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,100,131,0,1,168,143,0,1,36,143,0,5,168,0,17,0,2,138,0,2,16,66,131,0,1,1,131,0,1,2,131,0,38,2,0,0,2,144,0,16,0,3,0,48,80,4,0,0,48,80,0,1,49,81,0,0,16,9,0,0,16,10,48,5,32,3,0,0,18,0,196,133,0,7,96,5,0,0,18,0,194,133,0,7,16,11,16,12,18,0,34,131,0,8,5,248,16,0,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,4,200,1,0,2,132,0,8,175,1,0,0,200,2,0,2,132,0,8,175,1,1,0,200,4,0,2,132,0,8,175,1,2,0,200,8,0,2,132,0,9,175,1,3,0,200,3,128,0,2,131,0,8,160,0,4,0,200,3,128,1,132,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,116,131,0,1,208,139,0,1,36,131,0,1,76,136,0,1,128,146,0,5,20,1,0,0,16,147,0,1,64,131,0,4,144,16,2,1,132,0,1,4,134,0,6,16,66,0,3,0,3,131,0,14,1,0,0,48,80,0,0,49,81,63,49,114,24,63,188,0,0,1,5,1,32,1,3,1,0,1,0,1,18,2,0,196,133,0,0,1,64,2,5,16,3,9,18,0,1,18,133,0,2,16,10,3,0,0,34,133,0,1,144,2,0,16,3,33,31,255,4,255,248,0,0,5,64,0,144,0,0,6,1,31,255,255,248,0,5,0,64,0,60,128,133,0,5,64,226,0,0,129,6,60,136,0,1,0,0,7,64,64,161,0,0,128,200,1,8,132,0,7,64,0,171,0,0,1,20,1,225,131,0,4,3,1,198,129,131,0,3,200,15,128,133,0,1,226,131,0,1,200,135,0,1,226,142,0,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,2,0,0,3,0,0,3,224,1,0,228,144,4,0,228,161,1,0,0,2,12,1,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,255,255,81,0,0,5,0,0,15,160,24,114,49,63,131,0,1,63,136,0,4,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,3,176,31,0,0,2,131,0,61,144,0,8,15,160,66,0,0,3,1,0,15,128,1,0,228,176,0,8,228,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,15,0,0,2,0,0,8,128,1,0,0,128,5,0,0,3,1,0,8,128,0,0,255,128,131,0,9,160,15,0,0,2,0,0,8,128,131,0,13,128,4,0,0,4,0,0,8,128,0,0,255,128,131,0,49,160,1,0,255,128,5,0,0,3,0,0,1,128,0,0,255,128,0,0,85,160,1,0,0,2,0,0,14,128,0,0,170,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Downsample2LogR.gd))
			{
				return;
			}
			Downsample2LogR.gd = devIndex;
			if ((Downsample2LogR.vs != null))
			{
				Downsample2LogR.vs.Dispose();
				Downsample2LogR.ps.Dispose();
			}
			state.CreateShaders(out Downsample2LogR.vs, out Downsample2LogR.ps, Downsample2LogR.vsb, Downsample2LogR.psb, 6, 10, 0, 0);
			if ((Downsample2LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogR._init(state);
			}
		}
		
		public void SetSampleDirection(ref Microsoft.Xna.Framework.Vector2 value)
		{
			this.vreg.SetVector2(4, ref value);
		}
		
		public Microsoft.Xna.Framework.Vector2 SampleDirection
		{
			set
			{
				this.SetSampleDirection(ref value);
			}
		}
		
		private static int id_0;
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(2578));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D Texture
		{
			get
			{
				return this.ps_0t;
			}
			set
			{
				if ((value != this.ps_0t))
				{
					this.ps_0t = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		public Xen.Graphics.State.TextureSamplerState PointSampler
		{
			get
			{
				return this.ps_0;
			}
			set
			{
				if ((value != this.ps_0))
				{
					this.ps_0 = value;
					this.ps_m = (this.ps_m | 1);
				}
			}
		}
		
		private static int ptid_0;
		
		private static int psid_0;
		
		private int ps_m;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 1, 0, out tc, out ic);
			if ((Downsample2LogR.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Downsample2LogR.vs, Downsample2LogR.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.v_0);
			if (((this.vreg.change == true) 
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1) 
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				this.ps_m = 0;
			}
		}
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Microsoft.Xna.Framework.Vector2 value)
		{
			if ((Downsample2LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogR._init(state);
			}
			if ((name_uid == Downsample2LogR.id_0))
			{
				this.SetSampleDirection(ref value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((Downsample2LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogR._init(state);
			}
			if ((name_uid == Downsample2LogR.ptid_0))
			{
				this.Texture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((Downsample2LogR.init_gd != state.DeviceUniqueIndex))
			{
				Downsample2LogR._init(state);
			}
			if ((name_uid == Downsample2LogR.psid_0))
			{
				this.PointSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,0};
		
readonly 
		
		private static int[] _vusage = new int[] {0,5};
		
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = Downsample2LogR._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Downsample2LogR._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change 
						|| (this.ps_m != 0));
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,131073,518893518,5};
			}
		}
	}
}

