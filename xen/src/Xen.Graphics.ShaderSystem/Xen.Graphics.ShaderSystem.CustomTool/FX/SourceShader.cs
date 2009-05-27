
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Xen.Graphics.ShaderSystem.CustomTool.FX
{

	//stores an FX file, and extracts user options
	public sealed class SourceShader
	{
		private readonly string shaderSource;
		private readonly string[] sourceLines;
		private readonly CompilerOptions compilerOptions;
	
		private readonly bool mixedMode;
		private readonly bool useVfetch;
		private readonly bool debugHlslProcessXboxShader;
		private readonly bool poolShaderBytes;
		private readonly bool internalClass;
		private readonly bool useParentNamespace;
		private readonly bool skipConstantValidation;

		private readonly HlslStructure hlslShader;
		private readonly string filename;
		private readonly List<SourceShader> includedSource;

		private readonly List<HlslTechnique> techniques;
		private readonly List<HlslMethod> methods;

		private readonly List<AsmTechnique> asmTechniques, xboxAsmTechniques;

		private readonly Dictionary<string, string> shaderExtensions;
		private readonly Dom.BytePool bytePoolPc, bytePoolXbox;

		public Dom.BytePool BytePoolPC { get { return bytePoolPc; } }
		public Dom.BytePool BytePoolXbox { get { return bytePoolXbox; } }

		public SourceShader(string shaderSource, string filename, bool extractAsmTechniques)
		{
			this.shaderSource = shaderSource;
			this.filename = filename;
			this.sourceLines = shaderSource.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

			this.shaderExtensions = new Dictionary<string, string>();

			this.techniques = new List<HlslTechnique>();
			this.methods = new List<HlslMethod>();

			this.hlslShader = new HlslStructure(shaderSource);
			this.includedSource = new List<SourceShader>();

			if (sourceLines.Length > 0)
			{
				this.compilerOptions = ComputeCompilerOps(sourceLines[0], out internalClass, out poolShaderBytes, out useParentNamespace, out mixedMode, out debugHlslProcessXboxShader, out skipConstantValidation);
				ComputeClassExtensions(sourceLines[0], this.shaderExtensions);
			}
			if (sourceLines.Length > 1)
				ComputeClassExtensions(sourceLines[1], this.shaderExtensions);

			if (poolShaderBytes)
			{
				this.bytePoolPc = new Dom.BytePool();
				this.bytePoolXbox = new Dom.BytePool();
			}

			this.ExtractIncludeSource(ref this.useVfetch);

			this.mixedMode &= !this.useVfetch;

			this.ExtractMethods();


			asmTechniques = new List<AsmTechnique>();
			if (!mixedMode)
				xboxAsmTechniques = new List<AsmTechnique>();

			if (extractAsmTechniques)
				this.ExtractAsmTechniques();
		}

		private void ExtractAsmTechniques()
		{
			if (!mixedMode)
			{
				//both platforms compile their own effects

				//pull the actual asm data out..
				this.asmTechniques.AddRange(AsmTechnique.ExtractTechniques(this, Platform.Windows));
				this.xboxAsmTechniques.AddRange(AsmTechnique.ExtractTechniques(this, Platform.Xbox));
			}
			else
			{
				//one set of techniques for both windows and xbox

				//pull the actual asm data out..
				this.asmTechniques.AddRange(AsmTechnique.ExtractTechniques(this, Platform.Both));
			}
		}

		private void ExtractIncludeSource(ref bool useVfetch)
		{
			if (filename != null)
			{
				//parse the hs, look for #include's in the root level only

				foreach (HlslStructure hs in this.hlslShader.Children)
				{
					if (hs.Elements.Length > 4)
					{
						if (hs.Elements[0] == "#" &&
							hs.Elements[1] == "include")
						{
							bool global = hs.Elements[2] == "<";
							StringBuilder file = new StringBuilder();

							for (int i = 3; i < hs.Elements.Length-1; i++)
							{
								if ((global && hs.Elements[i] == ">") || (!global && hs.Elements[i] == "\""))
									break;

								file.Append(hs.Elements[i]);
							}

							string includeName = file.ToString();

							if (includeName == VFetchIncludeHandler.IncludeSymbol)
							{
								//vfetch requires shaders are built separately
								useVfetch = true;
							}
							else
							{
								//find the file
								string path = VFetchIncludeHandler.ResolveIncludePath(file.ToString(), this.filename);

								if (File.Exists(path))
								{
									//load the file and parse it as well
									SourceShader include = new SourceShader(File.ReadAllText(path), path, false);
									includedSource.Add(include);
								}
							}
						}
					}
				}
			}
		}


		private void ExtractMethods()
		{
			string str = this.hlslShader.ToString();

			List<Platform> platformStack = new List<Platform>();

			//pull out methods and techniques, but only from the root level...
			foreach (HlslStructure hs in this.hlslShader.Children)
			{
				//could be an #if, #else, #etc
				if (hs.Children.Length == 0 &&
					hs.Elements.Length > 1 &&
					hs.Elements[0].Length == 1 &&
					hs.Elements[0][0] == '#')
				{
					//need to account for #if XBOX360 blocks
					switch (hs.Elements[1])
					{
						case "if":
						case "ifdef":
							if (hs.Elements.Length > 2)
							{
								if (hs.Elements[2] == "XBOX360" ||
									hs.Elements[2] == "XBOX")
									platformStack.Add(Platform.Xbox);
								else
								if (hs.Elements[2] == "!XBOX360" ||
									hs.Elements[2] == "!XBOX")
									platformStack.Add(Platform.Windows);
								else
									platformStack.Add(Platform.Both);
							}
							break;

						case "ifndef":
							if (hs.Elements.Length > 2)
							{
								if (hs.Elements[2] == "XBOX360" ||
									hs.Elements[2] == "XBOX")
									platformStack.Add(Platform.Windows);
								else
								if (hs.Elements[2] == "!XBOX360" ||
									hs.Elements[2] == "!XBOX")
									platformStack.Add(Platform.Xbox);
								else
									platformStack.Add(Platform.Both);
							}
							break;

						case "else":
							if (platformStack.Count > 0)
							{
								Platform peek = platformStack[platformStack.Count - 1];
								platformStack.RemoveAt(platformStack.Count - 1);

								if (peek == Platform.Xbox)
									platformStack.Add(Platform.Windows);
								if (peek == Platform.Windows)
									platformStack.Add(Platform.Xbox);
							}
							break;
						case "endif":
							if (platformStack.Count > 0)
								platformStack.RemoveAt(platformStack.Count - 1);
							break;
					}
				}


				if (hs.BraceEnclosedChildren) 
				{
					if (hs.Elements.Length > 0 && hs.Elements[0] == "technique")
					{
						//figure out the platform, based on #if blocks stack.
						Platform platform = Platform.Both;
						for (int i = 0; i < platformStack.Count; i++)
							platform &= platformStack[i];

						this.techniques.Add(new HlslTechnique(hs, platform));
					}

					//finding a method is a bit trickier

					if (hs.Elements.Length > 2)
					{
						//should have a (...) block in it to be a method...
						int openDepth = 0;
						for (int i = 2; i < hs.Elements.Length; i++)
						{
							if (hs.Elements[i] == "(")
								openDepth++;

							if (hs.Elements[i] == ")")
							{
								if (--openDepth == 0)
								{
									//figure out the platform, based on #if blocks stack.
									Platform platform = Platform.Both;
									for (int p = 0; p < platformStack.Count; p++)
										platform &= platformStack[p];

									//found the method.
									this.methods.Add(new HlslMethod(hs, platform));
									break;
								}
							}
						}
					}
				}
			}

			foreach (SourceShader child in this.includedSource)
				child.ExtractMethods();
		}

		public HlslMethod GetMethod(string name, Platform platform)
		{
			for (int i = 0; i < this.methods.Count; i++)
			{
				if (this.methods[i].Name == name)
				{
					if ((this.methods[i].Platform & platform) == platform)
						return this.methods[i];
				}
			}
			for (int i = 0; i < this.includedSource.Count; i++)
			{
				HlslMethod method = this.includedSource[i].GetMethod(name, platform);
				if (method != null)
					return method;
			}
			return null;
		}


		public HlslTechnique GetTechnique(string name, Platform platform)
		{
			for (int i = 0; i < this.techniques.Count; i++)
			{
				if (this.techniques[i].Name == name &&
					(this.techniques[i].Platform & platform) == platform)
					return this.techniques[i];
			}
			return null;
		}
		public AsmTechnique GetAsmTechnique(string name, Platform platform)
		{
			List<AsmTechnique> techniques = this.asmTechniques;

			if (platform == Platform.Xbox)
				techniques = xboxAsmTechniques;

			for (int i = 0; i < techniques.Count; i++)
			{
				if (techniques[i].Name == name)
					return techniques[i];
			}
			return null;
		}


		//expose the data


		public string ShaderSource { get { return shaderSource; } }
		public string GetShaderLine(int line) { return sourceLines[line]; }
		public int ShaderLines { get { return sourceLines.Length; } }
		public string FileName { get { return filename; } }

		public HlslStructure HlslShader { get { return hlslShader; } }

		public CompilerOptions CompilerOptions { get { return compilerOptions; } }

		public bool DefinePlatform { get { return !mixedMode; } }
		public bool VFetchMacroImported { get { return useVfetch; } }
		public bool DebugHlslProcessXboxShader { get { return debugHlslProcessXboxShader; } }
		public bool PoolShaderBytes { get { return poolShaderBytes; } }
		public bool GenerateInternalClass { get { return internalClass; } }
		public bool UseParentNamespace { get { return useParentNamespace; } }
		public bool SkipConstantValidation { get { return skipConstantValidation; } }

		public HlslTechnique[] GetAllTechniques() { return techniques.ToArray(); }

		public string GetShaderExtension(string shader)
		{
			string value = null;
			shaderExtensions.TryGetValue(shader, out value);
			return value;
		}
		public bool GetShaderExtended(string shader)
		{
			return shaderExtensions.ContainsValue(shader);
		}

		#region data extraction

		private static CompilerOptions ComputeCompilerOps(string firstLine, out bool internalClass, out bool poolShaderBytes, out bool useParentNamespace, out bool mixedMode, out bool debugHlslProcessXboxShader, out bool skipConstantValidation)
		{
			CompilerOptions options = CompilerOptions.None;
			string cop = "compileroptions";

			/*
				looking for
			
			// CompilerOptions = ...

				on the firstLine line
			 */

			mixedMode = true;
			debugHlslProcessXboxShader = false;
			poolShaderBytes = false;
			internalClass = false;
			useParentNamespace = false;
			skipConstantValidation = false;

			if (firstLine.Length < cop.Length + 2)
				return options;

			firstLine = firstLine.ToLower();

			//yes this could be done better...
			//remove white space and comment slashes
			firstLine = firstLine.Replace(" ", "").Replace("/", "").Replace("\t", "");

			if (firstLine.Length < cop.Length + 2)
				return options;

			//first part needs to be compileroptions
			if (firstLine.Substring(0, cop.Length) != cop)
				return options;

			//
			firstLine = firstLine.Substring(cop.Length);

			//then =
			if (firstLine.Length == 0)
				return options;
			if (firstLine[0] != '=')
				return options;

			//parse the options!

			string[] ops = firstLine.Substring(1).Split(',');

			foreach (string op in ops)
			{
				switch (op)
				{
					case "avoidflowcontrol":
						options |= CompilerOptions.AvoidFlowControl;
						break;
					case "nopreshader":
						options |= CompilerOptions.NoPreShader;
						break;
					case "partialprecision":
						options |= CompilerOptions.PartialPrecision;
						break;
					case "preferflowcontrol":
						options |= CompilerOptions.PreferFlowControl;
						break;
					case "internalclass":
						internalClass = true;
						break;
					case "skipconstantvalidation":
						skipConstantValidation = true;
						break;
					case "poolshaderbytes":
						poolShaderBytes = true;
						break;
					case "parentnamespace":
						useParentNamespace = true;
						break;
					case "defineplatform":
						mixedMode = false;
						break;
					case "useasmtohlslxboxconverter":
						debugHlslProcessXboxShader = true;
						break;

				}
			}
			return options;
		}

		internal static void ComputeClassExtensions(string line, Dictionary<string, string> extensions)
		{
			string cop = "constantoverride";

			if (line.Length < cop.Length + 2)
				return;

			line = line.ToLower();

			//yes this could be done better...
			line = line.Replace(" ", "").Replace("/", "").Replace("\t", "");

			if (line.Length < cop.Length + 2)
				return;

			if (line.Substring(0, cop.Length) != cop)
				return;

			line = line.Substring(cop.Length);

			if (line.Length == 0)
				return;
			if (line[0] != '=')
				return;

			//parse the extensions!

			int index = 1;
			StringBuilder name = new StringBuilder();
			string currentBase = null;

			while (index < line.Length)
			{
				switch (line[index])
				{
					case ':':
						currentBase = name.ToString();
						extensions.Add(currentBase, null);
						name.Length = 0;
						break;
					case ';':
					case ',':
						extensions.Add(name.ToString(), currentBase);
						name.Length = 0;
						break;
					default:
						name.Append(line[index]);
						break;
				}
				index++;
			}
		}

		#endregion
	}
}
