using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xen.Graphics.ShaderSystem.CustomTool.FX;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Xen.Graphics.ShaderSystem.Constants;

namespace Xen.Graphics.ShaderSystem.CustomTool.Dom
{
	//this class stores the byte data for the shaders in use
	//it also creates and stores the vertex and pixel shader instances
	public sealed class ShaderBytes : DomBase
	{
		private byte[] vsBytesPc, psBytesPc, vsBytesXbox, psBytesXbox;
		private readonly SourceShader source;
		private readonly AsmTechnique asmTechnique;
		private readonly HlslTechnique hlslTechnique;
		private readonly Platform platform;

		public ShaderBytes(SourceShader source, string techniqueName, Platform platform)
		{
			this.source = source;
			this.platform = platform;

			this.asmTechnique = source.GetAsmTechnique(techniqueName, platform);
			this.hlslTechnique = source.GetTechnique(techniqueName, platform);
		}

		public override void Setup(IShaderDom shader)
		{
			ExtractBytes();
		}

		private void ExtractBytes()
		{
			if (platform == Platform.Both || platform == Platform.Windows)
			{
				string vsAsm = asmTechnique.VertexShader.ToString();
				string psAsm = asmTechnique.PixelShader.ToString();

				//generate PC shaders from ASM (easy)

				//vertex shader
				CompiledShader compiledShader = ShaderCompiler.AssembleFromSource(vsAsm, null, null, CompilerOptions.None, TargetPlatform.Windows);

				if (!compiledShader.Success)
					Common.ThrowError(compiledShader.ErrorsAndWarnings, vsAsm);

				this.vsBytesPc = compiledShader.GetShaderCode();

				//pixel shader
				compiledShader = ShaderCompiler.AssembleFromSource(psAsm, null, null, CompilerOptions.None, TargetPlatform.Windows);

				if (!compiledShader.Success)
					Common.ThrowError(compiledShader.ErrorsAndWarnings, psAsm);

				this.psBytesPc = compiledShader.GetShaderCode();
			}

			if (platform == Platform.Both || platform == Platform.Xbox)
			{
				//this is where things get tricky...

				HlslMethod vertexShaderMethod = source.GetMethod(hlslTechnique.VertexShaderMethodName, platform);
				HlslMethod pixelShaderMethod = source.GetMethod(hlslTechnique.PixelShaderMethodName, platform);

				//if a shader uses vfetch, it gets compiled as HLSL from the effect*, otherwise, HLSL that embeds ASM :-)
				//*or a stub method if it tries to use uniform inputs to the method...

				VFetchXboxMethodExtractor vfetchExtractor = new VFetchXboxMethodExtractor(source, hlslTechnique);

				if (vertexShaderMethod.UsesVFetch)
					this.vsBytesXbox = vfetchExtractor.GetVertexShaderCode();
				else
				{
					//otherwise,
					//run the shader through a processor that try to bypasses ShaderCompiler.AssembleFromSource,
					//which is buggy... But it doesn't always work.

					//need to work out the max number of constants used

					RegisterSet set = asmTechnique.VertexShader.RegisterSet;
					int maxConstant = set.FloatRegisterCount;

					AsmToHlslAsmConverter converter = new AsmToHlslAsmConverter(asmTechnique.VertexShader, TargetPlatform.Xbox360, maxConstant, source.DebugHlslProcessXboxShader);

					this.vsBytesXbox = converter.GetOutput(); 
				}

				//do it all again for the pixel shader
				if (pixelShaderMethod.UsesVFetch)
					this.psBytesXbox = vfetchExtractor.GetPixelShaderCode();
				else
				{
					RegisterSet set = asmTechnique.PixelShader.RegisterSet;
					int maxConstant = set.FloatRegisterCount;

					AsmToHlslAsmConverter converter = new AsmToHlslAsmConverter(asmTechnique.PixelShader, TargetPlatform.Xbox360, maxConstant, source.DebugHlslProcessXboxShader);

					this.psBytesXbox = converter.GetOutput();
				}
				//done!
			}
		}

		public override void AddMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
			//add pixel and static vertex shaders
			if (platform != Platform.Both)
				return;

			CodeMemberField field = new CodeMemberField(typeof(VertexShader), shader.VertexShaderRef.FieldName);
			field.Attributes = MemberAttributes.Private | MemberAttributes.Final | MemberAttributes.Static;

			add(field, "Static vertex shader instance");

			//ps
			field = new CodeMemberField(typeof(PixelShader), shader.PixelShaderRef.FieldName);
			field.Attributes = MemberAttributes.Private | MemberAttributes.Final | MemberAttributes.Static;

			add(field, "Static pixel shader instance");
		}

		public override void AddReadonlyMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
			Platform? writePlatform = null;
			if (this.platform == Platform.Both)
			{
				//shader is being built for both PC and xbox
				if (platform != Platform.Both) // writing system specific shader
					writePlatform = platform;
			}
			else
			{
				//shader is being built for just one platform
				writePlatform = this.platform;
			}

			if (writePlatform == null)
				return;

			if (writePlatform.Value == Platform.Windows)
			{
				//write windows
				WriteBytes(shader, shader.VertexShaderBytesRef.FieldName, this.vsBytesPc, add, shader.CompileDirectives, true, false);
				WriteBytes(shader, shader.PixelShaderBytesRef.FieldName, this.psBytesPc, add, shader.CompileDirectives, false, false);
			}
			else
			{
				//write xbox
				WriteBytes(shader, shader.VertexShaderBytesRef.FieldName, this.vsBytesXbox, add, shader.CompileDirectives, true, true);
				WriteBytes(shader, shader.PixelShaderBytesRef.FieldName, this.psBytesXbox, add, shader.CompileDirectives, false, true);
			}
		}


		public override void AddWarm(IShaderDom shader, Action<CodeStatement, string> add)
		{
			//dispose the shaders (if the exist)

			CodeBinaryOperatorExpression notNull = new CodeBinaryOperatorExpression(shader.VertexShaderRef, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression((VertexShader)null));

			//dipose
			CodeConditionStatement disposer = new CodeConditionStatement(notNull,
				shader.ETS(new CodeMethodInvokeExpression(shader.VertexShaderRef, "Dispose")));
			add(disposer, null);

			//ps
			notNull = new CodeBinaryOperatorExpression(shader.PixelShaderRef, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression((PixelShader)null));

			//dipose
			disposer = new CodeConditionStatement(notNull,
				shader.ETS(new CodeMethodInvokeExpression(shader.PixelShaderRef, "Dispose")));
			add(disposer, null);


			//and then recreate them using the shader system

			//state.CreateShaders(out ShadowShader.vs, out ShadowShader.ps, ShadowShader.vsb, ShadowShader.psb, 23, 15, 3, 0);


			int vsInstructions = asmTechnique.VertexShader.GetCommandCount() - 1;
			int psInstructions = asmTechnique.PixelShader.GetCommandCount() - 1;

			int vsPreShaderInstructions = 0;
			int psPreShaderInstructions = 0;

			if (asmTechnique.VertexPreShader != null)
				psPreShaderInstructions = asmTechnique.VertexPreShader.GetCommandCount() - 1;
			if (asmTechnique.PixelPreShader != null)
				psPreShaderInstructions = asmTechnique.PixelPreShader.GetCommandCount() - 1;

			CodeExpression create = new CodeMethodInvokeExpression(shader.ShaderSystemRef, "CreateShaders",
				new CodeDirectionExpression(FieldDirection.Out, shader.VertexShaderRef),
				new CodeDirectionExpression(FieldDirection.Out, shader.PixelShaderRef),
				shader.VertexShaderBytesRef,
				shader.PixelShaderBytesRef,
				new CodePrimitiveExpression(vsInstructions),
				new CodePrimitiveExpression(psInstructions),
				new CodePrimitiveExpression(vsPreShaderInstructions),
				new CodePrimitiveExpression(psPreShaderInstructions));
			
			add(shader.ETS(create), "Create the shader instances");

		}



		private void WriteBytes(IShaderDom shader, string name, byte[] data, Action<CodeTypeMember, string> add, CompileDirectives compileDirectives, bool isVS, bool isXbox)
		{
			//the byte array gets run through a simple compression scheme first...

			//generate the local byte array
			CodeMemberField field = new CodeMemberField(typeof(byte[]), name);
			field.Attributes = MemberAttributes.Final | MemberAttributes.Private | MemberAttributes.Static;

			data = ConstantArray.ArrayUtils.SimpleCompress(data);

			//if using the byte pool, then defer creating the array
			if (source.PoolShaderBytes)
			{
				if (isXbox)
					field.InitExpression = source.BytePoolXbox.AddArray(data);
				else
					field.InitExpression = source.BytePoolPC.AddArray(data);
			}
			else
			{
				//decompressed in code
				CodeMethodReferenceExpression decompressMethod = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(ConstantArray.ArrayUtils)), "SimpleDecompress");
				CodeExpression dataCode = ToArray(data, compileDirectives);

				//assign it inline
				field.InitExpression = new CodeMethodInvokeExpression(decompressMethod, dataCode);
			}

			add(field,string.Format("Static {0} shader byte code ({1})", isVS ? "vertex" : "pixel", isXbox ? "Xbox360" : "Windows"));
		}


		public static CodeExpression ToArray<T>(T[] data, CompileDirectives compileDirectives)
		{
			CodeExpression[] exp = new CodeExpression[data.Length];

			for (int i = 0; i < exp.Length; i++)
				exp[i] = new CodePrimitiveExpression(data[i]);

			CodeArrayCreateExpression array = new CodeArrayCreateExpression(typeof(T[]), exp);

			if (!compileDirectives.IsCSharp)
				return array;

			//optimize the array a bit.. so it's stored in a smaller space
			StringBuilder output = new StringBuilder(data.Length * 8);
			CodeGeneratorOptions options = new CodeGeneratorOptions();

			options.IndentString = "";

			using (TextWriter writer = new StringWriter(output))
				compileDirectives.CodeDomProvider.GenerateCodeFromExpression(array, writer, options);

			output.Replace(Environment.NewLine, "");

			return new CodeSnippetExpression(output.ToString());
		}
	}
}
