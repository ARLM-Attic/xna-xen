using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xen.Graphics.ShaderSystem.CustomTool.FX
{
	public sealed class AsmTechnique
	{
		private AsmListing vsListing, psListing, vsPreshaderListing, psPreshaderListing;
		private string vertexShaderComment, pixelShaderComment;
		private RegisterSet registers;
		private readonly string name;
		private readonly TechniqueExtraData defaultValues;

		public string Name
		{
			get { return name; }
		}

		public AsmListing VertexShader		{ get { return vsListing; } }
		public AsmListing VertexPreShader	{ get { return vsPreshaderListing; } }
		public AsmListing PixelShader		{ get { return psListing; } }
		public AsmListing PixelPreShader	{ get { return psPreshaderListing; } }
		public RegisterSet CommonRegisters	{ get { return registers; } }
		public TechniqueExtraData TechniqueExtraData { get { return defaultValues; } }
		public string VertexShaderComment { get { return vertexShaderComment; } }
		public string PixelShaderComment { get { return pixelShaderComment; } }

		//this is a bit of a hack.
		//it relies on the fact that the DirectX shader compiler
		//marks up the disasembled shader with comments detailing the shader inputs.
		public static AsmTechnique[] ExtractTechniques(SourceShader shader, Platform platform)
		{
			//decompile the shader
			DecompiledEffect fx = new DecompiledEffect(shader, platform);

			//break it up into techniques
			Tokenizer assemblyTokens = new Tokenizer(fx.DecompiledAsm, false, true, true);

			List<AsmTechnique> techniqes = new List<AsmTechnique>();

			while (assemblyTokens.NextToken())
			{
				switch (assemblyTokens.Token)
				{
					case "technique":
						//should be format:
						//technique NAME
						//{
						//}

						assemblyTokens.NextToken();
						string name = assemblyTokens.Token;

						assemblyTokens.NextToken();

						//may be a line break
						if (assemblyTokens.Token.Trim().Length == 0)
							assemblyTokens.NextToken();

						//should be a {
						if (assemblyTokens.Token != "{")
							throw new CompileException("Unexpected token in assembly technique declaration, expected '{': " + assemblyTokens.Token);

						// read the entire technique {} block
						if (!assemblyTokens.ReadBlock())
							throw new CompileException("Unexpected end of string in assembly technique pass declaration");

						AsmTechnique asm = new AsmTechnique(name, assemblyTokens.Token, fx.GetTechniqueDefaultValues(name));

						if (!shader.SkipConstantValidation)
						{
							//do some extra validation to make sure pixel inputs match vertex outputs
							asm.ValidatePixleShaderInput(shader, platform);
						}

						techniqes.Add(asm);

						break;
				}
			}

			for (int i = 0; i < techniqes.Count; i++)
			{
				techniqes[i].MergeSemantics(fx.EffectRegisters);
			}

			return techniqes.ToArray();
		}

		private void MergeSemantics(RegisterSet fxRegisters)
		{
			if (psListing != null)
				this.psListing.RegisterSet.MergeSemantics(fxRegisters);
			if (vsListing != null)
				this.vsListing.RegisterSet.MergeSemantics(fxRegisters);
			if (psPreshaderListing != null)
				this.psPreshaderListing.RegisterSet.MergeSemantics(fxRegisters);
			if (vsPreshaderListing != null)
				this.vsPreshaderListing.RegisterSet.MergeSemantics(fxRegisters);

			this.registers.MergeSemantics(fxRegisters);
		}

		private void ValidatePixleShaderInput(SourceShader source, Platform platform)
		{
			if (psListing != null && vsListing != null)
			{
				for (int i = 0; i < psListing.InputCount; i++)
				{
					if (!vsListing.ContainsOutput(psListing.GetInput(i)))
					{
						throw new CompileException(string.Format(
							"Pixel Shader '{0}' for Technique '{1}' tries to access input '{2} {3}', which is not output by Vertex Shader '{4}'{5}(Use the CompilerOption 'SkipConstantValidation' to disable this check)",
							source.GetTechnique(name, platform).PixelShaderMethodName,
							name,
							psListing.GetInput(i).Usage,
							psListing.GetInput(i).Index,
							source.GetTechnique(name, platform).VertexShaderMethodName,
							Environment.NewLine));
					}
				}
			}
		}

		private AsmTechnique(string name, string source, TechniqueExtraData defaultValues)
		{
			Tokenizer tokenizer = new Tokenizer(source, false, true, true);
			this.name = name;
			this.defaultValues = defaultValues;

			//parse the asm, and extract the first pass.
			string pass = null;

			while (tokenizer.NextToken())
			{
				if (tokenizer.Token == "pass")
				{
					//may have a name next...
					tokenizer.NextToken();
					string token = tokenizer.Token;

					if (token != "{")
					{
						//the name is specified
						tokenizer.NextToken();
						token = tokenizer.Token;
					}

					//may be a new line
					while (token.Trim().Length == 0)
					{
						tokenizer.NextToken();
						token = tokenizer.Token;
					}

					if (token != "{")
						throw new CompileException("Unexpected token in assembly technique pass declaration, expected '{': " + token);

					if (!tokenizer.ReadBlock())
						throw new CompileException("Unexpected end of string in assembly technique pass declaration");

					if (pass != null)
						throw new CompileException("A shader technique may only define a single Pass");

					pass = tokenizer.Token;
				}
			}

			if (pass == null)
				throw new CompileException("Technique '" + name + "' does not define a pass");

			ProcessPass(pass);
		}

		private void ProcessPass(string pass)
		{
			//vsListing, psListing

			//extract the shader code
			Tokenizer tokenizer = new Tokenizer(pass, false, true, true);

			while (tokenizer.NextToken())
			{
				if (tokenizer.Token == "vertexshader")
				{
					while (tokenizer.NextToken())
					{
						if (tokenizer.Token == "asm")
						{
							//extract the vertex shader
							tokenizer.NextToken();
							if (tokenizer.Token != "{")
								throw new CompileException("Expected token in asm vertexshader: '" + tokenizer.Token + "' - expected '{'");
							tokenizer.ReadBlock();
							ProcessShader(tokenizer.Token, out this.vsListing, out this.vsPreshaderListing, out vertexShaderComment);
							break;
						}
					}
				}
				if (tokenizer.Token == "pixelshader")
				{
					while (tokenizer.NextToken())
					{
						if (tokenizer.Token == "asm")
						{
							//extract the pixel shader
							tokenizer.NextToken();
							if (tokenizer.Token != "{")
								throw new CompileException("Expected token in asm pixelshader: '" + tokenizer.Token + "' - expected '{'");
							tokenizer.ReadBlock();
							ProcessShader(tokenizer.Token, out this.psListing, out this.psPreshaderListing, out pixelShaderComment);
							break;
						}
					}
				}
			}

			SetupCommonRegisters();
		}

		private void SetupCommonRegisters()
		{
			Dictionary<string, Register> common = new Dictionary<string, Register>();

			AsmListing[] listings = new AsmListing[] { vsListing, psListing, vsPreshaderListing, psPreshaderListing };

			foreach (AsmListing listing in listings)
			{
				if (listing == null)
					continue;

				for (int i = 0; i < listing.RegisterSet.RegisterCount; i++)
				{
					Register reg = listing.RegisterSet.GetRegister(i);

					if (!common.ContainsKey(reg.Name))
						common.Add(reg.Name, reg);
				}
			}

			Register[] registers = new Register[common.Count];
			int count = 0;

			foreach (Register reg in common.Values)
			{
				registers[count++] = reg;
			}

			this.registers = new RegisterSet(registers);
		}

		private void ProcessShader(string asm, out AsmListing shader, out AsmListing preshader, out string comment)
		{
			preshader = null;

			//format:

			//either,

			/*
			 * //header
			 * preshader
			 * //header
			 * shader
			 * //comment
			 */
			
			//or

			/*
			 * //header
			 * shader
			 * //comment
			 */

			string[] asmLinesSource = asm.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			//ignore first and last line (the { and })
			string[] asmLines = new string[Math.Max(0,asmLinesSource.Length - 2)];
			for (int i = 1; i < asmLinesSource.Length-1; i++)
				asmLines[i - 1] = asmLinesSource[i].Trim();

			comment = "";
			if (asmLines.Length > 0)
			{
				comment = asmLines[asmLines.Length - 1];
				if (comment.StartsWith("// "))
					comment = comment.Substring(3);
			}

			bool[] isComment = new bool[asmLines.Length];

			for (int i = 0; i < asmLines.Length - 1; i++)
				isComment[i] = asmLines[i].StartsWith("//");

			//top header
			int headerLength = 0;
			int start = 0;
			for (int i = 1; i < asmLines.Length; i++)
			{
				if (!isComment[i] && asmLines[i].Length != 0)
					break;

				headerLength = i+1;
				start = i+1;
			}

			int firstBlockLength = 0;
			int firstBlockStart = start;
			for (int i = start; i < asmLines.Length; i++)
			{
				firstBlockLength = i - headerLength + 1;
				start = i+1;

				if (isComment[i])
					break;
			}

			int secondHeaderLength = 0;
			int secondHeaderStart = start;
			for (int i = start; i < asmLines.Length; i++)
			{
				if (!isComment[i] && asmLines[i].Length != 0)
					break;

				secondHeaderLength = i - firstBlockLength + 1;
				start = i+1;
			}

			int secondBlockLength = 0;
			int secondBlockStart = start;
			for (int i = start; i < asmLines.Length; i++)
			{
				secondBlockLength = i - secondHeaderLength + 1;
				start = i+1;

				if (isComment[i])
					break;
			}

			if (secondHeaderLength > 0 && 
				secondBlockLength  > 0)
			{
				//preshader is used
				preshader = new AsmListing(CombineLines(asmLines, firstBlockStart, firstBlockLength), new RegisterSet(CombineLines(asmLines, 1, headerLength)));
				shader = new AsmListing(CombineLines(asmLines, secondBlockStart, secondBlockLength), new RegisterSet(CombineLines(asmLines, secondHeaderStart, secondHeaderLength)));
			}
			else
			{
				//no preshader
				shader = new AsmListing(CombineLines(asmLines, firstBlockStart, firstBlockLength), new RegisterSet(CombineLines(asmLines, 1, headerLength)));
			}
		}

		string CombineLines(string[] lines, int start, int length)
		{
			StringBuilder builder = new StringBuilder(32 * length);
			for (int i = 0; i < length; i++)
			{
				if (start + i < lines.Length)
					builder.AppendLine(lines[start + i]);
			}
			return builder.ToString();
		}
	}

	public sealed class HlslTechnique
	{
		private readonly HlslStructure hs;
		private readonly string name, pixelShaderMethodName, vertexShaderMethodName, psVersion, vsVersion;
		private readonly string[] psArgs, vsArgs;
		private readonly Platform platform;

		//extracts the techniques
		internal HlslTechnique(HlslStructure hs, Platform platform)
		{
			this.hs = hs;
			this.platform = platform;
			this.name = hs.Elements[1];

			//first child should be the first pass
			for (int i = 0; i < hs.Children.Length; i++)
			{
				if (hs.Children[i].Elements.Length > 0 && 
					hs.Children[i].Elements[0] == "pass")
				{
					//good.

					ExtractPass(hs.Children[i], out pixelShaderMethodName, out vertexShaderMethodName, out psVersion, out vsVersion, out psArgs, out vsArgs);
					break;
				}
			}
		}

		private static void ExtractPass(HlslStructure pass, out string ps, out string vs, out string psVersion, out string vsVersion, out string[] psArgs, out string[] vsArgs)
		{
			ps = vs = vsVersion = psVersion = null;
			psArgs = vsArgs = null;

			//a bit nasty...
			foreach (HlslStructure hs in pass.Children)
			{
				//should be:
				//
				//VertexShader = compile vs_2_0 Zomg(true!);
				//
				//or similar

				int type = -1;
				string target = null, method = null;
				List<string> args = new List<string>();
				int paranethDepth = 0;

				for (int i = 0; i < hs.Elements.Length; i++)
				{
					if (hs.Elements[i] == "VertexShader")
						type = 1;
					if (hs.Elements[i] == "PixelShader")
						type = 2;

					if (hs.Elements[i] == "compile")
						target = hs.Elements[i + 1];

					if (hs.Elements[i] == ")")
					{
						paranethDepth--;
					}

					if (paranethDepth > 0)
						args.Add(hs.Elements[i]);

					if (hs.Elements[i] == "(")
					{
						if (paranethDepth == 0)
							method = hs.Elements[i - 1];
						paranethDepth++;
					}
				}

				if (type == 1)
				{
					vs = method;
					vsArgs = args.ToArray();
					vsVersion = target;
				}
				if (type == 2)
				{
					ps = method;
					psArgs = args.ToArray();
					psVersion = target;
				}
			}
		}

		public HlslStructure HlslShader { get { return hs; } }
		public Platform Platform { get { return platform; } }
		public string Name { get { return name; } }
		public string PixelShaderMethodName { get { return pixelShaderMethodName; } }
		public string VertexShaderMethodName { get { return vertexShaderMethodName; } }
		public string PixelShaderVersion { get { return psVersion; } }
		public string VertexShaderVersion { get { return vsVersion; } }
		public IEnumerator<string> PixelShaderArgs { get { return ((IList<string>)psArgs).GetEnumerator(); } }
		public IEnumerator<string> VertexShaderArgs { get { return ((IList<string>)vsArgs).GetEnumerator(); } }
	}

	public sealed class HlslMethod
	{
		private readonly HlslStructure hs;
		private readonly string name;
		private readonly bool usesVFetch;
		private readonly Platform platform;

		//extracts the techniques
		internal HlslMethod(HlslStructure hs, Platform platform)
		{
			this.hs = hs;
			this.platform = platform;

			for (int i = 1; i < hs.Elements.Length; i++)
			{
				if (hs.Elements[i] == "(")
					name = hs.Elements[i - 1];
			}

			foreach (HlslStructure child in hs.GetEnumerator())
			{
				if (child != hs)
				{
					for (int i = 0; i < child.Elements.Length-1; i++)
					{
						if (child.Elements[i] == VFetchIncludeHandler.IncludeSymbol && child.Elements[i + 1] == "(")
							usesVFetch = true;
					}
					if (usesVFetch)
						break;
				}
			}
		}

		public Platform Platform { get { return platform; } }
		public HlslStructure HlslShader { get { return hs; } }
		public string Name { get { return name; } }
		public bool UsesVFetch { get { return usesVFetch; } }
	}
}
