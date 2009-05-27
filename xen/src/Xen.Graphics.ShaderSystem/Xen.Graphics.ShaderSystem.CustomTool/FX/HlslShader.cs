using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xen.Graphics.ShaderSystem.CustomTool.FX
{
	//creates a hirachical representation of the shader
	public sealed class HlslStructure
	{
		public readonly string[] Elements;
		public readonly HlslStructure[] Children;
		public readonly bool BraceEnclosedChildren;

		public HlslStructure(string source)
		{
			List<HlslStructure> nodes = new List<HlslStructure>();
			Tokenizer tokenizer = new Tokenizer(source, true, true, true);

			while (tokenizer.Index < tokenizer.Length)
				Parse(tokenizer, nodes);

			this.Elements = new string[0];
			this.Children = nodes.ToArray();
			this.BraceEnclosedChildren = false;
		}

		private HlslStructure(string[] el, HlslStructure[] ch, bool be)
		{
			this.Elements = el;
			this.Children = ch;
			this.BraceEnclosedChildren = be;
		}

		public IEnumerable<HlslStructure> GetEnumerator()
		{
			yield return this;
			foreach (HlslStructure hs in this.Children)
			{
				foreach (HlslStructure child in hs.GetEnumerator())
				{
					yield return child;
				}
			}
		}



		private static void Parse(Tokenizer tokenizer, List<HlslStructure> list)
		{
			int depth = tokenizer.BraceDepth;
			List<string> buffer = new List<string>();

			bool breakOnNewLine = false;
			bool isNewLine = false;

			//parsing a top level type declaration, eg, float4 test = ...;
			//will be set to false when hitting the '=' or a brace.
			//this is used to detect annotations on types (and ignore them!)
			bool processingTypeDeclaration = depth == 0;

			while (tokenizer.NextToken())
			{
				if (isNewLine)
				{
					if (tokenizer.Token.Length == 1 &&
						tokenizer.Token[0] == '#') //#include, #if, etc.
					{
						breakOnNewLine = true;
						processingTypeDeclaration = false;
					}
				}

				if (processingTypeDeclaration && (tokenizer.Token == "=" || tokenizer.BraceDepth > 0))
					processingTypeDeclaration = false;


				//detect annotations (in a <> block)
				if (processingTypeDeclaration && tokenizer.Token == "<")
				{
					//skip the annotation entirely.

					int blockDepth = 1;
					//parse until the matching '>'
					while (tokenizer.NextToken() && blockDepth != 0)
					{
						if (tokenizer.Token.Length == 1)
						{
							if (tokenizer.Token[0] == '<')
								blockDepth++;
							else
							if (tokenizer.Token[0] == '>')
								blockDepth--;
						}
					}
				}


				if (tokenizer.TokenIsNewLine)
					isNewLine = true;
				else
					isNewLine = false;


				if (depth < tokenizer.BraceDepth)
				{
					List<HlslStructure> nodes = new List<HlslStructure>();

					while (depth < tokenizer.BraceDepth)
						Parse(tokenizer, nodes);

					list.Add(new HlslStructure(buffer.ToArray(), nodes.ToArray(), true));

					buffer.Clear();
					processingTypeDeclaration = tokenizer.BraceDepth == 0;

					continue;
				}

				if (depth > tokenizer.BraceDepth)
					break;

				if (!tokenizer.TokenIsNewLine)
					buffer.Add(tokenizer.Token);

				if (tokenizer.Token == ";" || (breakOnNewLine && tokenizer.TokenIsNewLine))
					break;
			}

			if (buffer.Count > 0)
				list.Add(new HlslStructure(buffer.ToArray(), new HlslStructure[0], false));
		}


		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			ToString(sb,0);
			return sb.ToString();
		}
		internal void ToString(StringBuilder sb, int depth)
		{
			sb.AppendLine();
			for (int i = 0; i < depth; i++)
				sb.Append('\t');

			for (int i = 0; i < Elements.Length; i++)
			{
				if (i != 0 && Tokenizer.IsIdentifierToken(Elements[i]) && Tokenizer.IsIdentifierToken(Elements[i-1]))
					sb.Append(' ');
				sb.Append(Elements[i]);
			}

			if (BraceEnclosedChildren)
			{
				sb.AppendLine();
				for (int i = 0; i < depth; i++)
					sb.Append('\t');

				sb.Append('{');
				depth++;
			}

			foreach (HlslStructure node in Children)
				node.ToString(sb, depth);

			if (BraceEnclosedChildren)
			{
				sb.AppendLine();
				for (int i = 1; i < depth; i++)
					sb.Append('\t');

				sb.Append('}');

				if (Children.Length > 0)
					sb.AppendLine();
			}
		}
	}
}
