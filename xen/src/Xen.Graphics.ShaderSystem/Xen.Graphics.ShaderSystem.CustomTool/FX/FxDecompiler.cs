using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Xen.Graphics.State;

namespace Xen.Graphics.ShaderSystem.CustomTool.FX
{
	//data not stored within an ASM shader listing
	public sealed class TechniqueExtraData
	{
		public Vector4[] PixelShaderConstants, VertexShaderConstants;
		public TextureSamplerState[] PixelSamplerStates, VertexSamplerStates;
		public Register[] TechniqueTextures;
		public int[] PixelSamplerTextureIndex, VertexSamplerTextureIndex;
	}

	//this class takes an FX file, and decompiled it into raw shader assmebly
	//it always compiles the effect for the PC, as an Xbox effect can not be dissaembled.
	//
	//It also attempts to extract default values for shader constants and samplers.
	public sealed class DecompiledEffect
	{
		public static readonly CompilerMacro[] XboxCompileMacros;
		private readonly Dictionary<string, TechniqueExtraData> techniqueDefaults;

		static DecompiledEffect()
		{
			XboxCompileMacros = new CompilerMacro[2];

			XboxCompileMacros[0].Definition = "1";
			XboxCompileMacros[0].Name = "XBOX";
			XboxCompileMacros[1].Definition = "1";
			XboxCompileMacros[1].Name = "XBOX360";
		}

		private readonly string decompiledAsm;
		private readonly RegisterSet effectRegisters;

		public DecompiledEffect(SourceShader source, Platform platform)
		{
			CompilerMacro[] macros = null;
			if (platform == Platform.Xbox)
				macros = XboxCompileMacros;

			CompilerIncludeHandler include = null;
			TargetPlatform target = TargetPlatform.Windows;

			this.techniqueDefaults = new Dictionary<string, TechniqueExtraData>();

			if (platform != Platform.Both)
				include = new VFetchIncludeHandler(source.FileName, true); //ALWAYS target the PC for the vfetch macro

			CompiledEffect compiledEffect = Effect.CompileEffectFromSource(source.ShaderSource, macros, include, source.CompilerOptions, target);

			if (!compiledEffect.Success)
				Common.ThrowError(compiledEffect.ErrorsAndWarnings, source.ShaderSource);
			
			//now pull the good stuff out.
			using (EffectPool pool = new EffectPool())
			using (Effect effect = new Effect(Graphics.GraphicsDevice, compiledEffect.GetEffectCode(), CompilerOptions.None, pool))
			{
				Register[] registers = new Register[effect.Parameters.Count];
				List<Register> textures = new List<Register>();

				for (int i = 0; i < registers.Length; i++)
				{
					if (effect.Parameters[i].ParameterType == EffectParameterType.Single ||
						effect.Parameters[i].ParameterType == EffectParameterType.Int32)
					{
						registers[i].Name = effect.Parameters[i].Name;
						registers[i].Semantic = effect.Parameters[i].Semantic;
					}

					if (effect.Parameters[i].ParameterType >= EffectParameterType.Texture &&
						effect.Parameters[i].ParameterType <= EffectParameterType.TextureCube)
					{
						EffectParameterType type = effect.Parameters[i].ParameterType;
						if (type == EffectParameterType.Texture1D)
							type = EffectParameterType.Texture2D;

						registers[i].Name = effect.Parameters[i].Name;
						registers[i].Semantic = effect.Parameters[i].Semantic;
						registers[i].Type = type.ToString();
						textures.Add(registers[i]);
					}
				}

				this.effectRegisters = new RegisterSet(registers);
				this.decompiledAsm = Effect.Disassemble(effect, false);

				ExtractEffectDefaults(effect, textures);
			}
		}

		public string DecompiledAsm { get { return decompiledAsm; } }
		public RegisterSet EffectRegisters { get { return effectRegisters; } }

		public TechniqueExtraData GetTechniqueDefaultValues(string name)
		{
			TechniqueExtraData value = null;
			techniqueDefaults.TryGetValue(name, out value);
			return value;
		}




		private void ExtractEffectDefaults(Effect effect, List<Register> textures)
		{
			//nasty-ness ensues!
			GraphicsDevice device = Graphics.GraphicsDevice;

			int maxVsConst = device.GraphicsDeviceCapabilities.MaxVertexShaderConstants;
			int maxPsConst = 32;

			int maxPsTextures = 16;
			int maxVsTextures = 4;

			List<Texture> allTextures = new List<Texture>();

			foreach (EffectTechnique technique in effect.Techniques)
			{
				Vector4[] psConstants = new Vector4[maxPsConst]; // pixel 
				Vector4[] vsConstants = new Vector4[maxVsConst]; // not-pixel 

				TextureSamplerState[] psSamplers = new TextureSamplerState[maxPsTextures];
				TextureSamplerState[] vsSamplers = new TextureSamplerState[maxVsTextures];

				int[] psTexturesIndex = new int[maxPsTextures];
				int[] vsTexturesIndex = new int[maxVsTextures];

				allTextures.Clear();

				try
				{
					device.SetPixelShaderConstant(0, psConstants);
					device.SetVertexShaderConstant(0, vsConstants);

					for (int i = 0; i < maxPsTextures; i++)
						ResetSampler(device,i,true);
					for (int i = 0; i < maxVsTextures; i++)
						ResetSampler(device,i,false);

					//assign the technique textures
					foreach (Register texReg in textures)
					{
						Type type = Common.GetTextureType(texReg.Type);
						Texture tex = Graphics.BeginGetTempTexture(type);
						effect.Parameters[texReg.Name].SetValue(tex);

						allTextures.Add(tex);
					}

					//bind the effect technique
					effect.CurrentTechnique = technique;
					effect.Begin();

					if (technique.Passes.Count > 0)
					{
						EffectPass pass = technique.Passes[0];
						pass.Begin();


						pass.End();
					}

					effect.End();

					//all done. Now read back what has changed. :D

					psConstants = device.GetPixelShaderVector4ArrayConstant(0, maxPsConst);
					vsConstants = device.GetVertexShaderVector4ArrayConstant(0, maxVsConst);

					for (int i = 0; i < maxPsTextures; i++)
						psSamplers[i] = GetState(device, i, true, allTextures, out psTexturesIndex[i]);

					for (int i = 0; i < maxVsTextures; i++)
						vsSamplers[i] = GetState(device, i, false, allTextures, out vsTexturesIndex[i]);

					for (int i = 0; i < allTextures.Count; i++)
						Graphics.EndGetTempTexture(allTextures[i]);
					allTextures.Clear();
				}
				catch
				{
					//something went wrong... Eg, binding a SM 3.0 shader on SM 2.0 hardware device
					//Need to be running the Reference device

					throw new CompileException("Unable to compile shader: The DirectX Reference Device may be missing (Is the DirectX SDK Installed?)");
				}

				TechniqueExtraData defaults = new TechniqueExtraData();

				defaults.PixelSamplerStates = psSamplers;
				defaults.PixelShaderConstants = psConstants;
				defaults.VertexSamplerStates = vsSamplers;
				defaults.VertexShaderConstants = vsConstants;
				defaults.PixelSamplerTextureIndex = psTexturesIndex;
				defaults.VertexSamplerTextureIndex = vsTexturesIndex;
				defaults.TechniqueTextures = textures.ToArray();

				if (this.techniqueDefaults.ContainsKey(technique.Name) == false)
					this.techniqueDefaults.Add(technique.Name, defaults);
			}
		}

		private TextureSamplerState GetState(GraphicsDevice device, int index, bool PS, List<Texture> textures, out int textureIndex)
		{
			TextureSamplerState tss = new TextureSamplerState();
			textureIndex = -1;

			try
			{
				SamplerState ss = null;
				tss = TextureSamplerState.BilinearFiltering;
				if (!PS)
					tss = TextureSamplerState.PointFiltering;

				Texture texture = null;

				if (PS)
				{
					ss = device.SamplerStates[index];
					texture = device.Textures[index];
				}
				else
				{
					ss = device.VertexSamplerStates[index];
					texture = device.VertexTextures[index];
				}

				tss.AddressU = ss.AddressU;
				tss.AddressV = ss.AddressV;
				tss.AddressW = ss.AddressW;

				tss.MagFilter = ss.MagFilter;
				tss.MinFilter = ss.MinFilter;
				tss.MipFilter = ss.MipFilter;

				tss.MaxAnisotropy = ss.MaxAnisotropy;
				tss.MaxMipmapLevel = ss.MaxMipLevel;

				for (int i = 0; i < textures.Count; i++)
				{
					if (texture == textures[i] &&
						texture != null && textures[i] != null)
					{
						textureIndex = i;
					}
				}
			}
			catch
			{
				//hmph...
			}
			return tss;
		}
		private void ResetSampler(GraphicsDevice device, int index, bool PS)
		{
			SamplerState ss;
			TextureSamplerState tss = PS ? TextureSamplerState.BilinearFiltering : TextureSamplerState.PointFiltering;

			try
			{
				if (PS)
				{
					ss = device.SamplerStates[index];
					device.Textures[index] = null;
				}
				else
				{
					ss = device.VertexSamplerStates[index];
					device.VertexTextures[index] = null;
				}

				//reset everything
				ss.AddressU = tss.AddressU;
				ss.AddressV = tss.AddressV;
				ss.AddressW = tss.AddressW;

				ss.MagFilter = tss.MagFilter;
				ss.MinFilter = tss.MinFilter;
				ss.MipFilter = tss.MipFilter;

				ss.MaxAnisotropy = tss.MaxAnisotropy;
				ss.MaxMipLevel = tss.MaxMipmapLevel;
			}
			catch
			{
				//ignore...
			}
		}
	}
}
