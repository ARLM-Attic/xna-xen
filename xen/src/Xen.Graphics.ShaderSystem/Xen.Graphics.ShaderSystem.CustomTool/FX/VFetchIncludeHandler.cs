using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Xen.Graphics.ShaderSystem.CustomTool.FX
{
	public sealed class VFetchIncludeHandler : CompilerIncludeHandler
	{
		public const string IncludeSymbol = "asm_vfetch";

		//this is the logic that gets inserted into a shader to perform VFetch ops
		private readonly static string vfetchXbox	= "#define asm_vfetch(target,index,semantic)		asm { vfetch target, index, semantic }";
		//wrap it on the PC, to fake support and allow the shader to still compile,
		//but keep the value non-deterministic, so the compiler doesn't optimize things out
		private readonly static string vfetchPc		= "#define asm_vfetch(target,index,semantic)		target = saturate((index++)*float4(1,2,3,4))" + Environment.NewLine + "#define INDEX POSITION";

		private readonly static byte[] vfetchXboxBytes = System.Text.Encoding.ASCII.GetBytes(vfetchXbox);
		private readonly static byte[] vfetchPcBytes = System.Text.Encoding.ASCII.GetBytes(vfetchPc);

		public VFetchIncludeHandler(string rootFile, bool targetPC)
		{
			this.targetPC = targetPC;
			this.rootFile = rootFile;
		}
		
		private readonly bool targetPC;
		private bool requiresPreProcess;
		private readonly string rootFile;

		public bool RequiresXboxPreProcess
		{
			get { return requiresPreProcess; }
		}

		public override Stream Open(CompilerIncludeHandlerType includeType, string includePath)
		{
			if (includePath.ToLower() == IncludeSymbol)
			{
				requiresPreProcess = true;
				if (targetPC)
					return new MemoryStream(vfetchPcBytes);
				else
					return new MemoryStream(vfetchXboxBytes);
			}

			string resolvedPath = ResolveIncludePath(includePath, rootFile);

			return File.OpenRead(resolvedPath);
		}

		public static string ResolveIncludePath(string includePath, string rootFile)
		{
			//thanks to darren grant...
			return Path.IsPathRooted(includePath) ? includePath : Path.Combine(Path.GetDirectoryName(rootFile), includePath);
		}

	}
}
