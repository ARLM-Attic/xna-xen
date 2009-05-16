using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xen.Graphics.ShaderSystem.CustomTool.FX;

namespace Xen.Graphics.ShaderSystem.CustomTool
{
	public static class Common
	{
		//converts the first char to upper
		public static string ToUpper(string name)
		{
			if (name.Length == 0 || char.IsUpper(name[0]))
				return name;
			char[] nameC = name.ToCharArray();
			nameC[0] = char.ToUpper(nameC[0]);
			return new string(nameC);
		}

		public static void ThrowError(string errors, string source)
		{
			ThrowError(null, errors, source);
		}

		public static void ThrowError(string header, string errors, string source)
		{
			try
			{
				//pull out the errors...
				List<CompileException> exceptions = new List<CompileException>();

				if (header != null)
					exceptions.Add(new CompileException(header));

				foreach (string e in errors.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
				{
					string error = e.Trim();

					string[] lines = source.Split('\n');

					char[] errorC = error.Trim().ToCharArray();
					string lineS = "";
					for (int i = 0; i < errorC.Length; i++)
					{
						if (errorC[i] == ':' || errorC[i] == ')' || i > 6)
							break;
						if (char.IsNumber(errorC[i]))
							lineS += errorC[i];
					}

					int line = 0;
					int col = 0;
					bool knownLine = false;
					if (lineS.Length > 0)
						knownLine = int.TryParse(lineS, out line);
					line--;

					if (line >= lines.Length)
						line = lines.Length - 1;
					if (line < 0)
						line = 0;

					if (knownLine && errorC.Length > 1 && lines.Length > 0)
					{
						if (errorC[errorC.Length - 1] == '\'')
						{
							int start = errorC.Length - 2;

							while (start > 0 && errorC[start] != '\'')
								start--;

							if (errorC[start] == '\'')
							{
								string target = error.Substring(start + 1, error.Length - start - 3);

								col = lines[line].IndexOf(target);

								if (col != lines[line].LastIndexOf(target))
									col = 0;

								if (col == -1)
									col = 0;
							}
						}
					}

					exceptions.Add(new CompileException(line, col, error));
				}

				throw new CompileExceptionCollection(exceptions.ToArray());
			}
			catch
			{
				//NOTE:
				//if running directly from the ShaderSystem project,
				//the shader 'test.fx' in ShaderSystemTester project needs to contain a valid shader


				//something went wrong with the parsing
				if (header != null)
					throw new CompileException(header + Environment.NewLine + errors);
				else
					throw new CompileException(errors);
			}
		}

		public static Type GetTextureType(string textureTypeName)
		{
			switch (textureTypeName)
			{
				case "Texture1D":
				case "Texture2D":
					return typeof(Microsoft.Xna.Framework.Graphics.Texture2D);
				case "Texture3D":
					return typeof(Microsoft.Xna.Framework.Graphics.Texture3D);
				case "TextureCube":
					return typeof(Microsoft.Xna.Framework.Graphics.TextureCube);
				default:
					return typeof(Microsoft.Xna.Framework.Graphics.Texture);
			}
		}
	}


	[Flags]
	public enum Platform
	{
		Windows = 1,
		Xbox = 2,
		Both = 3
	}


	#region exceptions

	public sealed class CompileException : Exception
	{
		public CompileException(string text)
		{
			this.line = 0;
			this.col = 0;
			this.text = text;
		}
		public CompileException(int line, int col, string text)
		{
			this.line = line;
			this.col = col;
			this.text = text;
		}
		private readonly int line;
		private readonly int col;
		private readonly string text;

		//getters
		public int Coloumn
		{
			get { return col; }
		}

		public int Line
		{
			get { return line; }
		}

		public string Text
		{
			get { return text; }
		}

	}

	public sealed class CompileExceptionCollection : Exception
	{
		private readonly CompileException[] exceptions;

		public CompileExceptionCollection(params CompileException[] exceptions)
		{
			this.exceptions = exceptions;
		}

		public CompileException GetException(int index)
		{
			return exceptions[index];
		}
		public int Count { get { return exceptions.Length; } }
	}

	#endregion
}
