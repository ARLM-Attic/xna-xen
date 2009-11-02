// XenFX
// Assembly = Xen.Graphics.ShaderSystem.CustomTool, Version=6.0.0.1, Culture=neutral, PublicKeyToken=e706afd07878dfca
// SourceFile = C:\Users\Graham\Documents\Projects\xen\xen_codeplex_svn\xen\src\Xen.Graphics.ShaderSystem\ShaderSystemTester\bin\Debug\test.fx
// Namespace = test

namespace test
{

	/// <summary><para>Technique 'RenderInScreanSpace' generated from file 'test.fx'</para><para>Vertex Shader: approximately 2 instruction slots used, 0 registers</para><para>Pixel Shader: approximately 4 instruction slots used (1 texture, 3 arithmetic), 1 register</para></summary>
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Xen.Graphics.ShaderSystem.CustomTool.dll", "19673903-d770-436e-bfb7-3ad37a71ac02")]
	internal sealed class RenderInScreanSpace : Xen.Graphics.ShaderSystem.BaseShader
	{
		/// <summary>Construct an instance of the 'RenderInScreanSpace' shader</summary>
		public RenderInScreanSpace()
		{
			this.preg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(1);
			this.preg.Set(0, RenderInScreanSpace.preg_def);
			this.psm = 2147483647;
			this.ts0 = ((Xen.Graphics.State.TextureSamplerState)(21522));
		}
		/// <summary>Setup shader static values</summary><param name="state"/>
		private static void gdInit(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			// set the graphics ID
			RenderInScreanSpace.gd = state.DeviceUniqueIndex;
			RenderInScreanSpace.cid0 = state.GetNameUniqueID("color");
			RenderInScreanSpace.sid0 = state.GetNameUniqueID("colorSampler");
			RenderInScreanSpace.tid0 = state.GetNameUniqueID("colorMap");
		}
		/// <summary>Bind the shader</summary><param name="state"/>
		public override void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			// Shader type changed?
			bool tc = false;
			// Shader instance changed?
			bool ic = false;
			// About to start the bind
			int devIndex = state.Begin(this, 0, 0, out tc, out ic);
			// if the device changed, call Warm()
			if ((devIndex != RenderInScreanSpace.gd))
			{
				this.WarmShader(state);
				ic = true;
				tc = true;
			}
			// If the type has changed, and this class owns it's shaders, then bind the shaders
			if (((tc == true)
						&& (this.owner == true)))
			{
				state.SetShaders(RenderInScreanSpace.vs, RenderInScreanSpace.ps);
			}
			// Reset change masks
			if ((ic == true))
			{
				this.psm = 2147483647;
			}
			// Set pixel shader samplers
			if ((this.psm != 0))
			{
				if (((this.psm & 1)
							== 1))
				{
					state.SetPixelShaderSampler(0, this.tx0, this.ts0);
				}
				this.psm = 0;
			}
			// Vertex shader registers
			Microsoft.Xna.Framework.Vector4[] pc = null;
			if (((this.preg.change == true)
						|| (ic == true)))
			{
				pc = this.preg.array;
				this.preg.change = false;
			}
			if ((pc != null))
			{
				state.SetShaderConstants(null, pc);
			}
		}
		/// <summary>Warm (Preload) the shader</summary><param name="state"/>
		protected override void WarmShader(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			// Shader is already warmed
			if ((RenderInScreanSpace.gd == state.DeviceUniqueIndex))
			{
				return;
			}
			// Setup the shader
			if ((RenderInScreanSpace.gd != state.DeviceUniqueIndex))
			{
				RenderInScreanSpace.gdInit(state);
			}
			if ((RenderInScreanSpace.vs != null))
			{
				RenderInScreanSpace.vs.Dispose();
			}
			if ((RenderInScreanSpace.ps != null))
			{
				RenderInScreanSpace.ps.Dispose();
			}
			// Create the shader instances
			state.CreateShaders(out RenderInScreanSpace.vs, out RenderInScreanSpace.ps, RenderInScreanSpace.vsb, RenderInScreanSpace.psb, 4, 6, 0, 0);
		}
		/// <summary>True if a shader constant has changed since the last Bind()</summary>
		protected override bool Changed()
		{
			return (this.preg.change
						| (this.psm != 0));
		}
		/// <summary>Returns the number of vertex inputs used by this shader</summary>
		protected override int GetVertexInputCount()
		{
			return 2;
		}
		/// <summary>Returns a vertex input used by this shader</summary><param name="i"/><param name="usage"/><param name="index"/>
		protected override void GetVertexInput(int i, out Microsoft.Xna.Framework.Graphics.VertexElementUsage usage, out int index)
		{
			usage = ((Microsoft.Xna.Framework.Graphics.VertexElementUsage)(RenderInScreanSpace.vin[i]));
			index = RenderInScreanSpace.vin[(i + 2)];
		}
		/// <summary>Gets an array containing approximate hash codes for the constants used by this shader (Used for validation when merging two shaders)</summary><param name="ps"/>
		protected override int[] GetShaderConstantHash(bool ps)
		{
			if (ps)
			{
				return new int[] { 0, 65537, 1586258015, 1 };
			}
			else
			{
				return new int[] { 0 };
			}
		}
		/// <summary>Static graphics ID</summary>
		private static int gd;
		/// <summary>Static vertex shader instance</summary>
		private static Microsoft.Xna.Framework.Graphics.VertexShader vs;
		/// <summary>Static pixel shader instance</summary>
		private static Microsoft.Xna.Framework.Graphics.PixelShader ps;
		/// <summary>Name ID for 'color'</summary>
		private static int cid0;
		/// <summary>Set the shader value 'float4 color'</summary><param name="value"/>
		public void SetColor(ref Microsoft.Xna.Framework.Vector4 value)
		{
			this.preg.SetVector4(0, ref value);
		}
		/// <summary>Assign the shader value 'float4 color'</summary>
		public Microsoft.Xna.Framework.Vector4 Color
		{
			set
			{
				this.SetColor(ref value);
			}
		}
		/// <summary>Pixel Sampler dirty state bitmask</summary>
		private int psm;
		/// <summary>Sampler state for 'Sampler2D colorSampler'</summary>
		Xen.Graphics.State.TextureSamplerState ts0;
		/// <summary>Bound texture for 'Texture colorMap'</summary>
		Microsoft.Xna.Framework.Graphics.Texture tx0;
		/// <summary>Get/Set the Texture Sampler State for 'Sampler2D colorSampler'</summary>
		public Xen.Graphics.State.TextureSamplerState ColorSampler
		{
			get
			{
				return this.ts0;
			}
			set
			{
				if ((this.ts0 != value))
				{
					this.psm = (this.psm | 1);
					this.ts0 = value;
				}
			}
		}
		/// <summary>Get/Set the Bound texture for 'Texture colorMap'</summary>
		public Microsoft.Xna.Framework.Graphics.Texture ColorMap
		{
			get
			{
				return this.tx0;
			}
			set
			{
				if ((this.tx0 != value))
				{
					this.psm = (this.psm | 1);
					this.tx0 = value;
				}
			}
		}
		/// <summary>Name uid for sampler for 'Sampler2D colorSampler'</summary>
		static int sid0;
		/// <summary>Name uid for texture for 'Texture colorMap'</summary>
		static int tid0;
		/// <summary>array storing vertex usages, and element indices</summary>
		readonly
				private static int[] vin = new int[] { 0, 5, 0, 0 };
		/// <summary>Pixel shader register storage</summary>
		readonly
				private Xen.Graphics.ShaderSystem.Constants.ConstantArray preg;
		/// <summary>Register default values</summary>
		readonly
				private static float[] preg_def = new float[] { 1F, 1F, 1F, 1F };
#if XBOX360
		/// <summary>Static vertex shader byte code (Xbox360)</summary>
readonly 
		private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {4,16,42,17,1,131,0,1,144,131,0,1,96,135,0,1,36,135,0,1,88,139,0,1,48,131,0,1,28,131,0,4,35,255,254,3,139,0,1,18,132,0,21,28,118,115,95,51,95,48,0,50,46,48,46,56,50,55,53,46,48,0,171,171,135,0,5,96,0,1,0,1,138,0,2,16,33,131,0,1,1,131,0,1,2,131,0,30,1,0,0,2,144,0,16,0,3,0,48,80,4,0,0,240,80,0,0,16,6,48,5,32,3,0,0,18,0,194,133,0,7,16,5,0,0,18,0,196,133,0,5,16,6,0,0,34,133,0,3,5,248,16,131,0,2,6,136,132,0,2,5,248,132,0,2,15,200,132,0,4,200,15,128,62,132,0,13,226,1,1,0,200,3,192,0,0,176,176,0,226,142,0,1,0});
		/// <summary>Static pixel shader byte code (Xbox360)</summary>
readonly 
		private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] {3,16,42,17,132,0,1,204,131,0,1,60,135,0,1,36,135,0,1,168,139,0,1,128,131,0,1,28,131,0,4,115,255,255,3,132,0,1,2,131,0,4,28,0,0,18,132,0,1,108,131,0,3,68,0,2,131,0,1,1,133,0,1,72,135,0,3,88,0,3,131,0,1,1,133,0,1,92,132,0,14,95,99,0,171,0,1,0,3,0,1,0,4,0,1,134,0,14,95,115,48,0,0,4,0,12,0,1,0,1,0,1,134,0,20,112,115,95,51,95,48,0,50,46,48,46,56,50,55,53,46,48,0,171,171,135,0,2,60,16,134,0,1,4,134,0,6,16,33,0,1,0,1,131,0,14,1,0,0,240,80,0,1,16,2,0,0,18,0,196,133,0,5,16,3,0,0,34,133,0,15,16,8,0,1,31,31,246,136,0,0,64,0,200,15,128,133,0,1,161,142,0,1,0});
#else
		/// <summary>Static vertex shader byte code (Windows)</summary>
		readonly
				private static byte[] vsb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] { 8, 0, 2, 254, 255, 31, 0, 0, 2, 131, 0, 45, 128, 0, 0, 15, 144, 31, 0, 0, 2, 5, 0, 0, 128, 1, 0, 15, 144, 1, 0, 0, 2, 0, 0, 15, 192, 0, 0, 228, 144, 1, 0, 0, 2, 0, 0, 3, 224, 1, 0, 228, 144, 255, 255, 0, 0 });
		/// <summary>Static pixel shader byte code (Windows)</summary>
		readonly
				private static byte[] psb = Xen.Graphics.ShaderSystem.Constants.ConstantArray.ArrayUtils.SimpleDecompress(new byte[] { 8, 0, 2, 255, 255, 31, 0, 0, 2, 131, 0, 9, 128, 0, 0, 3, 176, 31, 0, 0, 2, 131, 0, 69, 144, 0, 8, 15, 160, 66, 0, 0, 3, 0, 0, 15, 128, 0, 0, 228, 176, 0, 8, 228, 160, 5, 0, 0, 3, 0, 0, 7, 128, 0, 0, 228, 128, 0, 0, 228, 160, 5, 0, 0, 3, 0, 0, 8, 128, 0, 0, 255, 128, 0, 0, 255, 160, 1, 0, 0, 2, 0, 8, 15, 128, 0, 0, 228, 128, 255, 255, 0, 0 });
#endif
		/// <summary>Set a shader attribute of type 'Vector4' by global unique ID, see <see cref="Xen.Graphics.ShaderSystem.IShaderSystem.GetNameUniqueID"/> for details.</summary><param name="state"/><param name="id"/><param name="value"/>
		protected override bool SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int id, ref Microsoft.Xna.Framework.Vector4 value)
		{
			if ((RenderInScreanSpace.gd != state.DeviceUniqueIndex))
			{
				this.WarmShader(state);
			}
			if ((id == RenderInScreanSpace.cid0))
			{
				this.SetColor(ref value);
				return true;
			}
			return false;
		}
		/// <summary>Set a shader sampler of type 'TextureSamplerState' by global unique ID, see <see cref="Xen.Graphics.ShaderSystem.IShaderSystem.GetNameUniqueID"/> for details.</summary><param name="state"/><param name="id"/><param name="value"/>
		protected override bool SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int id, Xen.Graphics.State.TextureSamplerState value)
		{
			if ((RenderInScreanSpace.gd != state.DeviceUniqueIndex))
			{
				this.WarmShader(state);
			}
			if ((id == RenderInScreanSpace.sid0))
			{
				this.ColorSampler = value;
				return true;
			}
			return false;
		}
		/// <summary>Set a shader texture of type 'Texture' by global unique ID, see <see cref="Xen.Graphics.ShaderSystem.IShaderSystem.GetNameUniqueID"/> for details.</summary><param name="state"/><param name="id"/><param name="value"/>
		protected override bool SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int id, Microsoft.Xna.Framework.Graphics.Texture value)
		{
			if ((RenderInScreanSpace.gd != state.DeviceUniqueIndex))
			{
				this.WarmShader(state);
			}
			if ((id == RenderInScreanSpace.tid0))
			{
				this.ColorMap = value;
				return true;
			}
			return false;
		}
	}
}
