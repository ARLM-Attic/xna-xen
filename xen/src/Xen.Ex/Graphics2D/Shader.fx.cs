namespace Xen.Ex.Graphics2D
{
	
	
	/// <summary><para>VS: approximately 30 instruction slots used</para><para>PS: approximately 3 instruction slots used (1 texture, 2 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class InstancingSprite : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public InstancingSprite()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.v_0 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			InstancingSprite.init_gd = state.DeviceUniqueIndex;
			InstancingSprite.ptid_0 = state.GetNameUniqueID("CustomTexture");
			InstancingSprite.psid_0 = state.GetNameUniqueID("CustomTextureSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(4);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,152,0,0,1,228,139,0,1,36,131,0,1,76,136,0,1,64,146,0,5,20,0,4,0,16,147,0,9,64,0,0,1,164,0,17,0,8,138,0,2,24,66,131,0,1,1,131,0,1,5,131,0,46,2,0,0,2,144,0,28,0,5,0,13,0,6,0,14,0,7,0,15,0,8,0,32,0,9,0,0,48,80,0,1,241,81,0,0,16,26,0,0,16,32,62,34,249,131,63,131,0,14,64,201,15,219,192,73,15,219,63,128,0,0,191,128,138,0,21,181,208,13,1,183,182,11,97,59,42,170,171,57,136,136,137,188,170,170,171,190,131,0,5,63,128,0,0,63,131,0,9,241,85,80,5,0,0,18,1,196,133,0,7,96,10,96,16,18,0,18,133,0,7,96,22,48,28,18,0,18,135,0,5,32,31,194,0,18,133,0,5,16,33,0,0,34,133,0,8,5,248,64,0,0,36,6,136,132,0,8,3,248,80,0,0,36,6,136,132,0,8,1,248,96,0,0,36,6,136,132,0,8,5,232,112,0,0,36,6,136,132,0,8,3,232,128,0,0,36,6,136,132,0,2,200,8,131,0,9,64,64,128,139,7,4,4,44,128,134,0,1,226,131,0,2,200,8,132,0,22,192,0,139,0,4,4,20,131,0,1,2,0,5,27,224,8,7,0,192,35,2,1,22,0,0,10,0,225,1,4,0,196,20,2,1,0,108,108,0,34,5,5,0,200,3,134,0,8,226,2,2,0,200,3,0,2,131,0,19,14,139,8,5,5,200,3,0,3,0,13,13,0,161,0,5,0,200,12,134,0,6,226,4,4,0,200,4,132,0,16,48,0,225,0,7,0,200,4,0,3,0,16,0,0,225,0,4,7,0,200,1,134,0,6,240,0,1,0,200,2,134,0,6,240,1,3,0,200,3,134,0,6,224,0,4,0,200,12,134,0,5,226,8,8,0,200,2,3,128,131,0,6,10,0,235,2,5,5,4,200,1,0,1,132,0,1,175,131,0,4,200,2,0,1,132,0,5,175,0,1,0,200,3,4,0,1,132,0,5,175,0,2,0,200,3,8,0,1,132,0,5,175,0,3,0,200,3,15,128,62,132,0,5,226,1,1,0,200,3,15,128,1,132,0,5,226,6,6,0,200,135,0,1,226,142,0,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,76,131,0,1,72,143,0,1,36,143,0,4,72,16,2,1,132,0,1,4,134,0,6,24,66,0,3,0,3,131,0,18,1,0,0,48,80,0,0,241,81,0,1,16,2,0,0,18,0,196,133,0,7,16,3,16,4,18,0,34,131,0,15,144,0,0,1,31,255,246,136,0,0,64,0,200,15,128,133,0,5,225,0,1,0,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {16,0,2,254,255,81,0,0,5,4,0,15,160,131,249,34,62,131,0,25,63,219,15,201,64,219,15,73,192,81,0,0,5,5,0,15,160,0,0,128,63,0,0,128,191,136,0,36,81,0,0,5,6,0,15,160,1,13,208,181,97,11,182,183,171,170,42,59,137,136,136,57,81,0,0,5,7,0,15,160,171,170,170,188,131,0,5,190,0,0,128,63,131,0,53,63,31,0,0,2,0,0,12,128,0,0,15,144,31,0,0,2,0,0,13,128,1,0,15,144,31,0,0,2,0,0,14,128,2,0,15,144,31,0,0,2,0,0,15,128,3,0,15,144,31,0,0,2,131,0,96,128,4,0,15,144,4,0,0,4,0,0,8,128,3,0,0,144,4,0,0,160,4,0,85,160,19,0,0,2,0,0,8,128,0,0,255,128,4,0,0,4,1,0,8,128,0,0,255,128,4,0,170,160,4,0,255,160,37,0,0,4,0,0,3,128,1,0,255,128,6,0,228,160,7,0,228,160,4,0,0,4,3,0,3,128,4,0,228,144,5,0,228,160,5,0,226,97,160,5,0,0,3,2,0,3,128,0,0,225,128,5,0,225,160,1,0,0,2,1,0,3,128,4,0,228,144,2,0,0,3,1,0,3,128,1,0,228,128,3,0,233,145,1,0,0,2,0,0,12,128,0,0,228,144,5,0,0,3,0,0,4,128,0,0,170,128,3,0,85,144,5,0,0,3,1,0,3,128,1,0,228,128,0,0,238,144,1,0,0,2,1,0,4,128,98,5,0,0,160,5,0,0,3,2,0,4,128,0,0,255,128,3,0,170,144,8,0,0,3,0,0,1,128,1,0,228,128,0,0,228,128,8,0,0,3,0,0,2,128,1,0,228,128,2,0,228,128,2,0,0,3,0,0,3,128,0,0,228,128,0,0,228,144,1,0,0,2,0,0,12,128,4,0,228,144,4,0,0,4,0,0,3,224,3,0,228,128,1,0,238,144,1,0,82,228,144,9,0,0,3,0,0,1,192,0,0,228,128,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,128,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,128,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,128,3,0,228,160,1,0,0,2,1,0,15,224,2,0,228,144,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,15,176,31,0,0,2,131,0,53,144,0,8,15,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,5,0,0,3,0,0,15,128,0,0,228,128,1,0,228,176,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == InstancingSprite.gd))
			{
				return;
			}
			InstancingSprite.gd = devIndex;
			if ((InstancingSprite.vs != null))
			{
				InstancingSprite.vs.Dispose();
				InstancingSprite.ps.Dispose();
			}
			state.CreateShaders(out InstancingSprite.vs, out InstancingSprite.ps, InstancingSprite.vsb, InstancingSprite.psb, 27, 3, 0, 0);
			if ((InstancingSprite.init_gd != state.DeviceUniqueIndex))
			{
				InstancingSprite._init(state);
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
			if ((InstancingSprite.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(InstancingSprite.vs, InstancingSprite.ps);
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
			if ((InstancingSprite.init_gd != state.DeviceUniqueIndex))
			{
				InstancingSprite._init(state);
			}
			if ((name_uid == InstancingSprite.ptid_0))
			{
				this.CustomTexture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((InstancingSprite.init_gd != state.DeviceUniqueIndex))
			{
				InstancingSprite._init(state);
			}
			if ((name_uid == InstancingSprite.psid_0))
			{
				this.CustomTextureSampler = sampler;
				return true;
			}
			return false;
		}
		
readonly 
		
		private static int[] _vinds = new int[] {0,12,13,14,15};
		
readonly 
		
		private static int[] _vusage = new int[] {0,0,0,0,0};
		
		protected override int GetVertexInputCount()
		{
			return 5;
		}
		
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			index = InstancingSprite._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(InstancingSprite._vusage[i]));
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

namespace Xen.Ex.Graphics2D
{
	
	
	/// <summary><para>VS: approximately 47 instruction slots used</para><para>PS: approximately 3 instruction slots used (1 texture, 2 arithmetic)</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	internal sealed class NonInstancingSprite : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public NonInstancingSprite()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(240, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.a_0 = new Xen.Graphics.ShaderSystem.Constants.Matrix4Array(this.vreg, 0, 240, true);
			this.v_240 = -1;
			this.ps_m = 65535;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			NonInstancingSprite.init_gd = state.DeviceUniqueIndex;
			NonInstancingSprite.id_0 = state.GetNameUniqueID("instances");
			NonInstancingSprite.ptid_0 = state.GetNameUniqueID("CustomTexture");
			NonInstancingSprite.psid_0 = state.GetNameUniqueID("CustomTextureSampler");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(244);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,5,140,0,0,2,220,139,0,1,36,131,0,1,76,143,0,1,7,139,0,5,20,0,244,0,48,147,0,9,192,0,0,2,28,0,17,0,5,138,0,2,24,66,131,0,1,1,131,0,1,2,131,0,38,2,0,0,2,144,0,16,0,6,0,32,80,7,0,0,48,80,0,1,241,81,0,0,16,23,0,0,16,42,64,128,0,0,62,34,249,131,63,131,0,26,63,128,0,0,64,201,15,219,192,73,15,219,191,128,0,0,63,128,0,0,63,128,0,0,191,128,138,0,21,181,208,13,1,183,182,11,97,59,42,170,171,57,136,136,137,188,170,170,171,190,131,0,5,63,128,0,0,63,227,0,0,1,63,143,0,0,1,48,1,5,1,32,1,6,1,0,1,0,1,18,1,0,1,196,133,0,0,1,96,1,8,1,96,1,14,1,18,1,0,1,18,133,0,0,1,96,1,20,1,96,1,26,1,18,1,0,1,18,133,0,0,1,96,1,32,1,48,1,38,1,18,1,0,1,18,135,0,0,1,32,1,41,1,194,1,0,1,18,133,0,0,1,16,1,43,1,0,1,0,1,34,133,0,0,1,5,1,248,1,80,1,0,1,0,1,36,1,6,1,136,132,0,0,1,3,1,248,1,32,1,0,1,0,1,36,1,6,1,136,132,0,0,1,44,1,136,1,0,1,1,1,4,131,64,0,1,229,131,2,0,1,200,1,8,1,0,1,2,1,4,1,0,1,64,1,0,1,224,1,0,1,2,1,0,1,200,1,8,1,0,1,0,1,2,131,0,0,1,229,131,0,0,1,200,1,8,134,0,0,1,235,1,1,2,0,2,2,200,8,132,0,2,64,0,3,161,0,244,3,0,200,8,131,0,4,27,108,0,160,5,0,255,0,52,128,134,0,1,226,131,0,2,92,128,133,0,0,1,3,1,226,131,0,2,200,3,2,0,1,131,0,3,14,139,5,4,246,246,20,16,5,2,0,160,0,0,2,177,194,131,0,6,200,5,0,3,224,33,7,33,0,34,2,2,0,200,8,10,0,3,224,64,64,0,34,9,3,3,0,200,6,0,2,224,16,10,16,0,34,1,1,0,20,16,4,0,5,160,0,0,27,194,131,0,11,200,8,0,2,0,64,128,192,139,4,244,4,244,200,3,128,133,0,10,235,1,3,2,44,136,1,2,224,27,3,27,0,34,131,2,11,200,8,0,1,0,0,64,128,139,1,245,12,245,20,131,1,3,4,10,0,27,224,2,5,6,1,192,41,2,4,224,131,0,13,34,2,2,1,196,19,2,1,0,10,0,0,225,6,3,3,1,20,64,1,132,0,13,27,194,0,0,244,200,3,0,3,0,13,10,0,14,161,2,245,0,200,4,0,2,160,32,16,0,161,4,15,1,0,200,4,0,3,160,16,32,0,161,4,3,0,20,16,17,4,2,160,0,0,108,208,2,1,0,20,34,4,2,160,17,0,0,108,208,1,3,1,20,19,0,1,160,0,0,198,192,2,18,4,0,20,44,0,1,160,0,0,198,194,5,5,1,20,65,0,2,19,96,0,0,198,143,1,240,2,20,130,0,2,96,0,0,198,143,1,241,5,3,200,4,0,2,132,0,8,175,1,242,0,200,8,0,2,132,0,8,175,1,243,0,200,15,128,62,132,0,8,226,2,2,0,200,15,128,1,132,0,1,226,131,0,1,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,76,131,0,1,72,143,0,1,36,143,0,4,72,16,2,1,132,0,1,4,134,0,6,24,66,0,3,0,3,131,0,18,1,0,0,48,80,0,0,241,81,0,1,16,2,0,0,18,0,196,133,0,7,16,3,16,4,18,0,34,131,0,15,144,0,0,1,31,255,246,136,0,0,64,0,200,15,128,133,0,5,225,0,1,0,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,254,255,81,0,0,5,244,0,15,160,0,0,128,64,131,249,34,62,131,0,45,63,0,0,128,63,81,0,0,5,245,0,15,160,219,15,201,64,219,15,73,192,0,0,128,191,0,0,128,63,81,0,0,5,246,0,15,160,0,0,128,63,0,0,128,191,136,0,36,81,0,0,5,247,0,15,160,1,13,208,181,97,11,182,183,171,170,42,59,137,136,136,57,81,0,0,5,248,0,15,160,171,170,170,188,131,0,5,190,0,0,128,63,131,0,5,63,31,0,0,2,131,0,96,128,0,0,15,144,31,0,0,2,5,0,0,128,1,0,15,144,12,0,0,3,0,0,8,128,1,0,0,144,1,0,0,145,19,0,0,2,1,0,8,128,1,0,0,144,2,0,0,3,2,0,8,128,1,0,255,129,1,0,0,144,12,0,0,3,1,0,8,128,1,0,255,129,1,0,255,128,4,0,0,4,0,0,8,128,0,0,255,128,1,0,255,128,2,0,255,97,128,5,0,0,3,0,0,8,128,0,0,255,128,244,0,0,160,46,0,0,2,0,0,8,176,0,0,255,128,4,0,0,4,2,0,3,128,0,0,228,144,246,0,228,160,246,0,226,160,1,0,0,3,0,0,1,128,0,32,85,160,0,0,255,176,1,0,0,3,1,0,5,128,2,32,197,160,0,0,255,176,1,0,0,3,1,0,10,128,3,32,36,160,0,0,255,176,98,1,0,0,3,0,0,6,128,1,32,244,160,0,0,255,176,1,0,0,3,3,0,1,128,0,32,255,160,0,0,255,176,4,0,0,4,0,0,8,128,3,0,0,128,244,0,85,160,244,0,170,160,4,0,0,4,0,0,3,224,2,0,228,128,1,0,228,128,0,0,228,128,19,0,0,2,2,0,8,128,0,0,255,128,1,0,0,3,0,0,8,128,2,32,255,160,0,0,99,255,176,4,0,0,4,2,0,8,128,2,0,255,128,245,0,0,160,245,0,85,160,2,0,0,3,1,0,3,128,0,0,238,129,0,0,228,144,37,0,0,4,0,0,3,128,2,0,255,128,247,0,228,160,248,0,228,160,5,0,0,3,1,0,3,128,1,0,238,128,1,0,228,128,5,0,0,3,2,0,3,128,0,0,225,128,245,0,238,160,1,0,0,3,3,0,9,128,2,100,32,228,160,0,0,255,176,5,0,0,4,0,0,4,128,3,0,0,128,1,32,255,160,0,0,255,176,1,0,0,2,1,0,4,128,244,0,255,160,5,0,0,4,2,0,4,128,3,0,255,128,3,32,0,160,0,0,255,176,8,0,0,3,0,0,1,128,1,0,228,128,0,0,228,128,8,0,0,3,0,0,2,128,1,0,228,128,2,0,228,128,1,0,0,3,1,0,1,128,0,101,32,0,160,0,0,255,176,1,0,0,3,1,0,2,128,1,32,0,160,0,0,255,176,2,0,0,3,0,0,3,128,0,0,228,128,1,0,228,128,1,0,0,2,0,0,12,128,0,0,228,144,9,0,0,3,0,0,1,192,0,0,228,128,240,0,228,160,9,0,0,3,0,0,2,192,0,0,228,128,241,0,228,160,9,0,0,3,0,0,4,192,0,0,228,128,242,0,228,160,9,0,82,0,3,0,0,8,192,0,0,228,128,243,0,228,160,1,0,0,3,1,0,1,224,0,32,170,160,0,0,255,176,1,0,0,3,1,0,2,224,1,32,170,160,0,0,255,176,1,0,0,3,1,0,4,224,2,32,170,160,0,0,255,176,1,0,0,3,1,0,8,224,3,32,170,160,0,0,255,176,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,255,255,31,0,0,2,131,0,9,128,0,0,3,176,31,0,0,2,131,0,9,128,1,0,15,176,31,0,0,2,131,0,53,144,0,8,15,160,66,0,0,3,0,0,15,128,0,0,228,176,0,8,228,160,5,0,0,3,0,0,15,128,0,0,228,128,1,0,228,176,1,0,0,2,0,8,15,128,0,0,228,128,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == NonInstancingSprite.gd))
			{
				return;
			}
			NonInstancingSprite.gd = devIndex;
			if ((NonInstancingSprite.vs != null))
			{
				NonInstancingSprite.vs.Dispose();
				NonInstancingSprite.ps.Dispose();
			}
			state.CreateShaders(out NonInstancingSprite.vs, out NonInstancingSprite.ps, NonInstancingSprite.vsb, NonInstancingSprite.psb, 45, 3, 0, 0);
			if ((NonInstancingSprite.init_gd != state.DeviceUniqueIndex))
			{
				NonInstancingSprite._init(state);
			}
		}
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.Matrix4Array a_0;
		
		public void SetInstances(Microsoft.Xna.Framework.Matrix[] value)
		{
			this.a_0.SetArray(value);
		}
		
		public Xen.Graphics.ShaderSystem.Constants.IArray<Microsoft.Xna.Framework.Matrix> Instances
		{
			get
			{
				return this.a_0;
			}
		}
		
		private static int id_0;
		
		private int v_240;
		
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
			if ((NonInstancingSprite.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(NonInstancingSprite.vs, NonInstancingSprite.ps);
			}
			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(240), ref this.v_240);
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
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Matrix[] value)
		{
			if ((NonInstancingSprite.init_gd != state.DeviceUniqueIndex))
			{
				NonInstancingSprite._init(state);
			}
			if ((name_uid == NonInstancingSprite.id_0))
			{
				this.SetInstances(value);
				return true;
			}
			return false;
		}
		
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Microsoft.Xna.Framework.Graphics.Texture2D texture)
		{
			if ((NonInstancingSprite.init_gd != state.DeviceUniqueIndex))
			{
				NonInstancingSprite._init(state);
			}
			if ((name_uid == NonInstancingSprite.ptid_0))
			{
				this.CustomTexture = texture;
				return true;
			}
			return false;
		}
		
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			if ((NonInstancingSprite.init_gd != state.DeviceUniqueIndex))
			{
				NonInstancingSprite._init(state);
			}
			if ((name_uid == NonInstancingSprite.psid_0))
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
			index = NonInstancingSprite._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(NonInstancingSprite._vusage[i]));
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
				return new int[] {0,524528,-722341874,240,524292,-888819319,244};
			}
		}
	}
}

