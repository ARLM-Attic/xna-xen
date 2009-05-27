using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Xen.Graphics.ShaderSystem.CustomTool.FX;
using Xen.Graphics.ShaderSystem.CustomTool.Dom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Threading;

namespace Xen.Graphics.ShaderSystem.CustomTool
{
	[ComVisible(true)]
	[Guid("43ACA195-467F-4EEC-A949-5873BBD5413A")]
	public sealed class ShaderCodeGenerator : Microsoft.CustomTool.BaseCodeGeneratorWithSite
	{
		public override string GetDefaultExtension()
		{
			return ".fx" + base.GetDefaultExtension();
		}


		protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
		{
			return GenerateCode(inputFileName, inputFileContent, this.FileNameSpace, this.CodeProvider);
		}

		public byte[] GenerateCode(string inputFileName, string inputFileContent, string fileNameSpace, CodeDomProvider codeProvider)
		{
			//make sure the culture is invariant while processing.
			CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;

			try
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

				try
				{
					SourceDom source = new SourceDom(new SourceShader(inputFileContent, inputFileName, true), fileNameSpace, codeProvider);

					return Encoding.ASCII.GetBytes(source.GenerateCodeString());
				}
				catch (CompileException ex)
				{
					this.GeneratorErrorCallback(false, 1, ex.Text, ex.Line, ex.Coloumn);
					Console.WriteLine(string.Format("Error generating shader: {0} (line: {1} col: {2})", ex.Text, ex.Line, ex.Coloumn));
					return Encoding.ASCII.GetBytes(SourceDom.GenerateErrorString(ex, codeProvider));
				}
				catch (Exception ex)
				{
					this.GeneratorErrorCallback(false, 1, string.Format("Unhandled exception in XenFX:{0}{1}", Environment.NewLine, ex.ToString()), 0, 0);
					Console.WriteLine(string.Format("Unhandled exception in XenFX: {0}", ex.ToString()));
					return Encoding.ASCII.GetBytes(SourceDom.GenerateErrorString(ex, codeProvider));
				}
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = previousCulture;
			}
		}

		//used for development, decodes then displays the generated code
		public void GenerateDebug(string inputFileContent, string inputFileName)
		{
			CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();

			SourceDom source = new SourceDom(new SourceShader(inputFileContent, inputFileName, true), "test", provider);

			string src = source.GenerateCodeString();
			System.Windows.Forms.Clipboard.SetText(src);

			if (src.Length > 8192)
				src = src.Substring(0, 8192);
			System.Windows.Forms.MessageBox.Show(src);
		}
	}
}
