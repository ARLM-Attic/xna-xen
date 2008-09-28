namespace Tutorials.Tutorial_03.Shader
{
	
	
	/// <summary><para>VS: approximately 6 instruction slots used</para><para>PS: approximately 1 instruction slot used</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "f528a88b-1c52-48ab-876d-fa36a9f095dd")]
	public sealed class Tutorial03Technique : Xen.Graphics.ShaderSystem.BaseShader
	{
		
		public Tutorial03Technique()
		{
			this.vreg.Set(0, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F, 0F);
			this.vreg.Set(4, 1F, 0F, 0F, 0F);
			this.preg.Set(0, 0F, 0F, 0F, 0F);
			this.g_0 = -1;
			this.v_0 = -1;
		}
		
		private static void _init(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			Tutorial03Technique.init_gd = state.DeviceUniqueIndex;
			Tutorial03Technique.id_0 = state.GetNameUniqueID("scale");
			Tutorial03Technique.g_id0 = state.GetGlobalUniqueID<Microsoft.Xna.Framework.Vector4>("colour");
		}
		
		private static int init_gd;
		
		private static int gd;
		
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(5);
		
readonly 
		
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray preg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(1);
		
#if XBOX360
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,80,131,0,1,144,143,0,1,36,143,0,5,144,0,1,0,1,143,0,1,1,131,0,1,1,134,0,2,2,144,131,0,10,3,16,1,16,3,0,0,18,0,196,133,0,7,80,4,0,0,18,0,194,133,0,7,16,9,16,10,18,0,34,131,0,2,5,248,131,0,3,36,6,136,132,0,2,20,135,132,0,10,44,27,161,0,4,0,200,1,0,1,132,0,1,175,131,0,4,200,2,0,1,132,0,8,175,0,1,0,200,4,0,1,132,0,8,175,0,2,0,200,8,0,1,132,0,8,175,0,3,0,200,15,128,62,132,0,5,226,1,1,0,200,135,0,1,226,142,0,1,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,68,131,0,1,60,143,0,1,36,143,0,2,60,16,150,0,1,1,132,0,5,16,2,196,0,18,133,0,5,16,3,0,0,34,133,0,3,200,15,128,133,0,1,34,131,0,1,200,135,0,1,226,142,0,1,0});
		
#else
		
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {8,0,2,254,255,31,0,0,2,131,0,101,128,0,0,15,144,5,0,0,3,0,0,7,128,0,0,228,144,4,0,0,160,1,0,0,2,0,0,8,128,0,0,255,144,9,0,0,3,0,0,1,192,0,0,228,128,0,0,228,160,9,0,0,3,0,0,2,192,0,0,228,128,1,0,228,160,9,0,0,3,0,0,4,192,0,0,228,128,2,0,228,160,9,0,0,3,0,0,8,192,0,0,228,128,3,0,228,160,255,255,0,0});
		
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {20,0,2,255,255,1,0,0,2,0,8,15,128,0,0,228,160,255,255,0,0});
		
#endif
		
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			int devIndex = state.DeviceUniqueIndex;
			if ((devIndex == Tutorial03Technique.gd))
			{
				return;
			}
			Tutorial03Technique.gd = devIndex;
			if ((Tutorial03Technique.vs != null))
			{
				Tutorial03Technique.vs.Dispose();
				Tutorial03Technique.ps.Dispose();
			}
			state.CreateShaders(out Tutorial03Technique.vs, out Tutorial03Technique.ps, Tutorial03Technique.vsb, Tutorial03Technique.psb, 6, 1, 0, 0);
			if ((Tutorial03Technique.init_gd != state.DeviceUniqueIndex))
			{
				Tutorial03Technique._init(state);
			}
		}
		
		private void SetScale(ref float value)
		{
			this.vreg.SetSingle(4, ref value);
		}
		
		public float Scale
		{
			set
			{
				this.SetScale(ref value);
			}
		}
		
		private static int id_0;
		
		private static int g_id0;
		
		private int g_0;
		
		private int v_0;
		
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool ic;
			bool tc;
			int devIndex = state.Begin(this, 0, 0, out tc, out ic);
			if ((Tutorial03Technique.gd != devIndex))
			{
				this.WarmShader(state);
				tc = true;
				ic = true;
			}
			if (((tc && this.owner) 
						== true))
			{
				state.SetShaders(Tutorial03Technique.vs, Tutorial03Technique.ps);
			}
			state.SetGlobal(this.preg.Vector4(0), Tutorial03Technique.g_id0, ref this.g_0);
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
		
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, float value)
		{
			if ((Tutorial03Technique.init_gd != state.DeviceUniqueIndex))
			{
				Tutorial03Technique._init(state);
			}
			if ((name_uid == Tutorial03Technique.id_0))
			{
				this.SetScale(ref value);
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
			index = Tutorial03Technique._vinds[i];
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(Tutorial03Technique._vusage[i]));
		}
		
		protected override bool Changed()
		{
			return (this.vreg.change || this.preg.change);
		}
		
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] {0,262145,-1068224993,1};
			}
			else
			{
				return new int[] {0,524292,-888819319,4,65537,763144525,5};
			}
		}
	}
}

