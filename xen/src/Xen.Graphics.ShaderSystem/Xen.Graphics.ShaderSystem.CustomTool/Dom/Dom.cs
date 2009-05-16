using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xen.Graphics.ShaderSystem.CustomTool.FX;
using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;

namespace Xen.Graphics.ShaderSystem.CustomTool.Dom
{
	//this is the entry point, where the shader is converted to a CodeDom for a set of IShader classes

	//builds a CodeDom representing the shader
	public sealed class SourceDom
	{
		private readonly SourceShader source;
		private readonly CodeNamespace rootNamespace;
		private readonly CodeNamespace rootXboxNamespace;
		private readonly CodeNamespace rootPcNamespace;
		private readonly CompileDirectives directives;
		private readonly string baseNamespace, outputNamespace;
		private readonly CodeDomProvider codeProvider;

		public SourceDom(SourceShader shader, string baseNamespace, CodeDomProvider provider)
		{
			this.codeProvider = provider;
			this.directives = new CompileDirectives(provider);
			this.source = shader;
			this.baseNamespace = baseNamespace;
			
			string namespaceName = baseNamespace;
			if (!shader.UseParentNamespace)
				namespaceName += "." + Common.ToUpper(Path.GetFileNameWithoutExtension(source.FileName));
			this.outputNamespace = namespaceName;

			HlslTechnique[] techniques = shader.GetAllTechniques();

			if (shader.DefinePlatform)
			{
				//create a copy for each platform
				//xbox first

				this.rootPcNamespace = new CodeNamespace(namespaceName);
				this.rootXboxNamespace = new CodeNamespace(namespaceName);
				
				for (int i = 0; i < techniques.Length; i++)
				{
					//build the PC shaders
					if ((techniques[i].Platform & Platform.Windows) == Platform.Windows)
					{
						ShaderDom dom = new ShaderDom(source, techniques[i].Name, Platform.Windows, directives);
						this.rootPcNamespace.Types.Add(dom.CodeTypeDeclaration);
					}
				}

				for (int i = 0; i < techniques.Length; i++)
				{
					//build the xbox shaders
					if ((techniques[i].Platform & Platform.Xbox) == Platform.Xbox)
					{
						ShaderDom dom = new ShaderDom(source, techniques[i].Name, Platform.Xbox, directives);
						this.rootXboxNamespace.Types.Add(dom.CodeTypeDeclaration);
					}
				}
			}
			else
			{
				this.rootNamespace = new CodeNamespace(namespaceName);

				for (int i = 0; i < techniques.Length; i++)
				{
					//build the combined pc / xbox shaders
					if ((techniques[i].Platform & Platform.Both) == Platform.Both)
					{
						ShaderDom dom = new ShaderDom(source, techniques[i].Name, Platform.Both, directives);
						this.rootNamespace.Types.Add(dom.CodeTypeDeclaration);
					}
				}
			}
		}

		public static string GenerateErrorString(Exception ex, CodeDomProvider codeProvider)
		{
			CodeGeneratorOptions gen = new CodeGeneratorOptions();
			gen.BracingStyle = "C";
			gen.IndentString = "\t";
			gen.VerbatimOrder = true;
			gen.BlankLinesBetweenMembers = false;

			CodeCommentStatement comment;
			if (ex is CompileException)
			{
				CompileException cx = (CompileException)ex;
				comment = new CodeCommentStatement(string.Format("Error generating shader:{0}{1} (line: {2} col: {3})",Environment.NewLine,cx.Text,cx.Line,cx.Coloumn));
			}
			else
			{
				comment = new CodeCommentStatement(string.Format("Unhandled exception in XenFX:{0}{1}",Environment.NewLine,ex.ToString()));
			}
			
			StringBuilder sb = new StringBuilder();
			using (TextWriter writer = new StringWriter(sb))
			{
				codeProvider.GenerateCodeFromStatement(comment, writer, gen);
			}
			return sb.ToString();
		}

		public string GenerateCodeString()
		{
			CodeGeneratorOptions gen = new CodeGeneratorOptions();
			gen.BracingStyle = "C";
			gen.IndentString = "\t";
			gen.VerbatimOrder = true;
			gen.BlankLinesBetweenMembers = false;

			StringBuilder sb = new StringBuilder();
			using (TextWriter writer = new StringWriter(sb))
			{

				//a header is written to the file.
				//this embeds some information about how the file was compiled
				CodeCommentStatementCollection headerCommentes = new CodeCommentStatementCollection();
				GenerateHeader(headerCommentes);
				for (int i = 0; i < headerCommentes.Count; i++)
					codeProvider.GenerateCodeFromStatement(headerCommentes[i], writer, gen);

				sb.AppendLine();

				//generate shader byte pool first, if needed
				if (source.PoolShaderBytes)
				{
					//add the shader pools
					sb.AppendLine(directives.IfXboxStatement.Text);

					CodeNamespace space = new CodeNamespace(this.outputNamespace);
					source.BytePoolXbox.GeneratePool(space.Types, directives);
					codeProvider.GenerateCodeFromNamespace(space, writer, gen);

					sb.AppendLine(directives.ElseStatement.Text);

					space = new CodeNamespace(this.outputNamespace);
					source.BytePoolPC.GeneratePool(space.Types, directives);
					codeProvider.GenerateCodeFromNamespace(space, writer, gen);

					sb.AppendLine(directives.EndifStatement.Text);
				}


				if (!source.DefinePlatform)
				{
					//geneate the single root namespace
					codeProvider.GenerateCodeFromNamespace(rootNamespace, writer, gen);
				}
				else
				{
					//generate the two namespaces, broken up by #if block


					//#if
					sb.AppendLine(directives.IfXboxStatement.Text);

					//xbox code
					codeProvider.GenerateCodeFromNamespace(rootXboxNamespace, writer, gen);

					//#else
					sb.AppendLine(directives.ElseStatement.Text);

					//pc code
					codeProvider.GenerateCodeFromNamespace(rootPcNamespace, writer, gen);

					//#endif
					sb.AppendLine(directives.EndifStatement.Text);
				}
			}

			return sb.ToString();
		}

		//add a header, so external tools can figure out the compile settings (ie, the namespace)
		private void GenerateHeader(CodeCommentStatementCollection header)
		{
			StringBuilder comment = new StringBuilder();
			
			comment.Append("XenFX");
			header.Add(new CodeCommentStatement(comment.ToString()));
		
			//detection block
			comment.Length = 0;
			comment.Append("Assembly = ");
			comment.Append(GetType().Assembly.FullName);
			header.Add(new CodeCommentStatement(comment.ToString()));
		
			comment.Length = 0;
			comment.Append("SourceFile = ");
			comment.Append(Path.GetFullPath(source.FileName));
			header.Add(new CodeCommentStatement(comment.ToString()));

			comment.Length = 0;
			comment.Append("Namespace = ");
			comment.Append(baseNamespace);
			header.Add(new CodeCommentStatement(comment.ToString()));

		}
	}
}
