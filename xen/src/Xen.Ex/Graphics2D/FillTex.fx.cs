namespace Xen.Ex.Graphics2D
{
	
	
	/// <summary><para>VS: approximately 5 instruction slots used</para><para>PS: approximately 2 instruction slots used (1 texture, 1 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class FillCustomTexture : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public FillCustomTexture()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			FillCustomTexture.init_gd = state.DeviceUniqueIndex;
			FillCustomTexture.ptid_0 = state.GetNameUniqueID("CustomTexture");
			FillCustomTexture.psid_0 = state.GetNameUniqueID("CustomTextureSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(4);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,92,131,0,1,156,143,0,1,36,143,0,5,156,0,1,0,2,138,0,2,8,33,131,0,1,1,131,0,1,2,131,0,30,1,0,0,2,144,0,16,0,3,0,48,80,4,0,0,48,80,0,0,16,9,48,5,32,3,0,0,18,0,196,133,0,7,80,5,0,0,18,0,194,133,0,7,16,10,16,11,18,0,34,131,0,8,5,248,16,0,0,36,6,136,132,0,2,3,248,131,0,3,36,6,136,132,0,4,200,1,0,2,132,0,8,175,1,0,0,200,2,0,2,132,0,8,175,1,1,0,200,4,0,2,132,0,8,175,1,2,0,200,8,0,2,132,0,7,175,1,3,0,200,3,128,133,0,1,226,131,0,4,200,15,128,62,132,0,5,226,2,2,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,72,131,0,1,72,143,0,1,36,143,0,3,72,16,2,133,0,1,4,134,0,6,8,33,0,1,0,1,131,0,14,1,0,0,48,80,0,1,16,2,0,0,18,0,196,133,0,7,16,3,16,4,18,0,34,131,0,15,144,0,0,1,31,255,246,136,0,0,64,0,200,15,128,133,0,1,226,131,0,1,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,97,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,9,0,0,3,0,0,1,192,0,0,228,144,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,144,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,144,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,144,3,0,228,160,1,0,0,2,0,0,3,224,1,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,37,144,0,8,15,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == FillCustomTexture.gd))
			{
				return;
			}
			FillCustomTexture.gd = devIndex;
			if ((FillCustomTexture.vs != null))
			{
				FillCustomTexture.vs.Dispose();
				FillCustomTexture.ps.Dispose();
			}
			state.CreateShaders(out FillCustomTexture.vs, out FillCustomTexture.ps, FillCustomTexture.vsb, FillCustomTexture.psb, 5, 2, 0, 0);
			if ((FillCustomTexture.init_gd != state.DeviceUniqueIndex))
			{
				FillCustomTexture._init(state);
			}
		}
		
		private int v_0;
		
		private Xen.Graphics.State.TextureSamplerState ps_0 = ((Xen.Graphics.State.TextureSamplerState)(13312));
		
		private Microsoft.Xna.Framework.Graphics.Texture2D ps_0t;
		
		public Microsoft.Xna.Framework.Graphics.Texture2D CustomTexture
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
		
		public Xen.Graphics.State.TextureSamplerState CustomTextureSampler
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
			if ((FillCustomTexture.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(FillCustomTexture.vs, FillCustomTexture.ps);
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
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((FillCustomTexture.init_gd != state.DeviceUniqueIndex))
			{
				FillCustomTexture._init(state);
			}
			if ((name_uid == FillCustomTexture.ptid_0))
			{
				this.CustomTexture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((FillCustomTexture.init_gd != state.DeviceUniqueIndex))
			{
				FillCustomTexture._init(state);
			}
			if ((name_uid == FillCustomTexture.psid_0))
			{
				this.CustomTextureSampler = sampler;
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
			index = FillCustomTexture._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(FillCustomTexture._vusage[i]));
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
				return new int[] {0,524292,-888819319,4};
			}
		}
	}
}

