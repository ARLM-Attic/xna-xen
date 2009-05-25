using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xen.Graphics.ShaderSystem.CustomTool.FX;
using System.CodeDom;
using Xen.Graphics.ShaderSystem.Constants;
using Microsoft.Xna.Framework;

namespace Xen.Graphics.ShaderSystem.CustomTool.Dom
{
	//how a texture is assigned to samplers
	class TextureAssociation
	{
		public readonly Register Texture;
		public readonly List<int> PsSamplers, VsSamplers;
		public readonly bool IsGlobal;

		public TextureAssociation(Register reg)
		{
			this.Texture = reg;
			this.PsSamplers = new List<int>();
			this.VsSamplers = new List<int>();
			if (reg.Semantic != null)
				IsGlobal = reg.Semantic.ToLower() == "global";
		}
	}

	class SharedSampler
	{
		public int PsIndex = -1, VsIndex = -1;
		public Register SamplerDetails;
		public int Index;
		public Xen.Graphics.State.TextureSamplerState DefaultState;
	}

	//generates the shader textures
	public sealed class ShaderTextures : DomBase
	{
		private readonly List<Register> psSamplers, vsSamplers;
		private readonly Dictionary<string, SharedSampler> allSamplers;
		private readonly TextureAssociation[] textures;

		public ShaderTextures(SourceShader source, string techniqueName, Platform platform)
		{
			this.psSamplers = new List<Register>();
			this.vsSamplers = new List<Register>();
			this.allSamplers = new Dictionary<string, SharedSampler>();

			AsmTechnique technique = source.GetAsmTechnique(techniqueName, platform);
			TechniqueExtraData extras = technique.TechniqueExtraData;

			//pull out the textures that will be used
			textures = new TextureAssociation[extras.TechniqueTextures.Length];
			for (int i = 0; i < extras.TechniqueTextures.Length; i++)
				textures[i] = new TextureAssociation(extras.TechniqueTextures[i]);

			//now do the samplers
			RegisterSet set = technique.PixelShader.RegisterSet;

			//pixel first
			foreach (Register reg in set)
			{
				if (reg.Category == RegisterCategory.Sampler)
				{
					psSamplers.Add(reg);

					int textureIndex = extras.PixelSamplerTextureIndex[reg.Index];
					textures[textureIndex].PsSamplers.Add(reg.Index);

					//add the sampler to 'allSamplers'
					SharedSampler ss = new SharedSampler();
					ss.PsIndex = reg.Index;
					ss.SamplerDetails = reg;
					ss.Index = allSamplers.Count;
					ss.DefaultState = extras.PixelSamplerStates[reg.Index];
					allSamplers.Add(reg.Name, ss);
				}
			}

			set = technique.VertexShader.RegisterSet;

			//now vertex
			foreach (Register reg in set)
			{
				if (reg.Category == RegisterCategory.Sampler)
				{
					vsSamplers.Add(reg);

					int textureIndex = extras.VertexSamplerTextureIndex[reg.Index];
					textures[textureIndex].VsSamplers.Add(reg.Index);

					//add the sampler to 'allSamplers'
					SharedSampler ss;
					if (!allSamplers.TryGetValue(reg.Name, out ss))
					{
						ss = new SharedSampler();
						ss.SamplerDetails = reg;
						ss.Index = allSamplers.Count;
						ss.DefaultState = extras.VertexSamplerStates[reg.Index];
						allSamplers.Add(reg.Name, ss);
					}
					ss.VsIndex = reg.Index;
				}
			}
		}

		public override void AddMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
			if (platform != Platform.Both)
				return;

			//if samplers are used, a integer is stored which will use bit masking
			//each bit represents if the sampler is dirty

			if (psSamplers.Count > 0)
			{
				//create mask
				CodeMemberField field = new CodeMemberField(typeof(int), "psm");
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

				add(field, "Pixel Sampler dirty state bitmask");
			}

			if (vsSamplers.Count > 0)
			{
				//create mask
				CodeMemberField field = new CodeMemberField(typeof(int), "vsm");
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

				add(field, "Vertex Sampler dirty state bitmask");
			}

			//add the sampler members
			foreach (SharedSampler ss in allSamplers.Values)
			{
				CodeMemberField field = new CodeMemberField(typeof(Xen.Graphics.State.TextureSamplerState), "ts" + ss.Index);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Family;

				add(field, string.Format("Sampler state for '{0} {1}'", Common.ToUpper(ss.SamplerDetails.Type), ss.SamplerDetails.Name));
			}

			//add the texture members
			for (int i = 0; i < textures.Length; i++)
			{
				TextureAssociation tex = textures[i];
				if (tex.PsSamplers.Count > 0 || tex.VsSamplers.Count > 0) //only add if they are used
				{
					CodeMemberField field = new CodeMemberField(Common.GetTextureType(tex.Texture.Type), "tx" + i);
					field.Attributes = MemberAttributes.Private | MemberAttributes.Family;

					add(field, string.Format("Bound texture for '{0} {1}'", tex.Texture.Type, tex.Texture.Name));
				}
			}

			//create the properties for the sampler states
			GenerateProperties(shader, add);

			GenerateNameIds(shader, add);
		}

		private void GenerateNameIds(IShaderDom shader, Action<CodeTypeMember, string> add)
		{
			//each texture / sampler needs to be settable by ID

			foreach (SharedSampler ss in allSamplers.Values)
			{
				//create a uid for the sampler

				CodeMemberField field = new CodeMemberField(typeof(int), "sid" + ss.Index);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Family | MemberAttributes.Static;

				add(field, string.Format("Name uid for sampler for '{0} {1}'", Common.ToUpper(ss.SamplerDetails.Type), ss.SamplerDetails.Name));
			}

			//create for each texture
			for (int i = 0; i < textures.Length; i++)
			{
				TextureAssociation tex = textures[i];

				if (tex.PsSamplers.Count > 0 || tex.VsSamplers.Count > 0)
				{
					//create a uid for the sampler

					CodeMemberField field = new CodeMemberField(typeof(int), "tid" + i);
					field.Attributes = MemberAttributes.Private | MemberAttributes.Family | MemberAttributes.Static;

					add(field, string.Format("Name uid for texture for '{0} {1}'", tex.Texture.Type, tex.Texture.Name));
				}
			}
		}

		public override void AddStaticGraphicsInit(IShaderDom shader, Action<CodeStatement, string> add)
		{
			//initalise the static UIDs

			foreach (SharedSampler ss in allSamplers.Values)
			{
				//set the uid for the sampler

				CodeExpression uid = new CodeFieldReferenceExpression(shader.ShaderClassEx, "sid" + ss.Index);

				CodeExpression call = new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(shader.ShaderSystemRef, "GetNameUniqueID"),
					new CodePrimitiveExpression(ss.SamplerDetails.Name));

				CodeStatement assign = new CodeAssignStatement(uid, call);

				add(assign, null);
			}

			//and all the textures
			for (int i = 0; i < textures.Length; i++)
			{
				TextureAssociation tex = textures[i];

				if (tex.PsSamplers.Count > 0 || tex.VsSamplers.Count > 0)
				{
					//set the uid for the sampler

					CodeExpression uid = new CodeFieldReferenceExpression(shader.ShaderClassEx, "tid" + i);

					CodeExpression call = new CodeMethodInvokeExpression(
						new CodeMethodReferenceExpression(shader.ShaderSystemRef, "GetNameUniqueID"),
						new CodePrimitiveExpression(tex.Texture.Name));

					if (tex.IsGlobal)
					{
						//slightly differnet for globals. Need to call generic getter for the global index
						call = new CodeMethodInvokeExpression(
							new CodeMethodReferenceExpression(shader.ShaderSystemRef, "GetGlobalUniqueID", new CodeTypeReference(Common.GetTextureType(tex.Texture.Type))),
							new CodePrimitiveExpression(tex.Texture.Name));
					}

					CodeStatement assign = new CodeAssignStatement(uid, call);

					add(assign, null);

				}
			}
		}


		//setup the ID based setters / getters
		public override void AddSetAttribute(IShaderDom shader, Action<CodeStatement> add, Type type)
		{

			//samplers first
			if (type == typeof(Xen.Graphics.State.TextureSamplerState))
			{
				foreach (SharedSampler ss in allSamplers.Values)
				{
					//assign the sampler, if it matches.
					CodeExpression uid = new CodeFieldReferenceExpression(shader.ShaderClassEx, "sid" + ss.Index);
					CodeExpression sampler = new CodePropertyReferenceExpression(shader.Instance, Common.ToUpper(ss.SamplerDetails.Name));

					CodeExpression assignIdsMatch =
						new CodeBinaryOperatorExpression(shader.AttributeAssignId, CodeBinaryOperatorType.IdentityEquality, uid);

					CodeStatement assignAttribute = new CodeAssignStatement(sampler, shader.AttributeAssignValue);

					CodeConditionStatement performAssign =
						new CodeConditionStatement(assignIdsMatch,
							assignAttribute, //call the assignment code
							new CodeMethodReturnStatement(new CodePrimitiveExpression(true))); //return true, set correctly.

					add(performAssign);
				}
			}


			if (!typeof(Microsoft.Xna.Framework.Graphics.Texture).IsAssignableFrom(type))
				return;

			//now, all the non-global textures
			for (int i = 0; i < textures.Length; i++)
			{
				TextureAssociation tex = textures[i];

				if ((tex.PsSamplers.Count > 0 || tex.VsSamplers.Count > 0)
					&& tex.IsGlobal == false)
				{
					if (Common.GetTextureType(tex.Texture.Type) == type)
					{
						//assign
						CodeExpression uid = new CodeFieldReferenceExpression(shader.ShaderClassEx, "tid" + i);
						CodeExpression sampler = new CodePropertyReferenceExpression(shader.Instance, Common.ToUpper(tex.Texture.Name));

						CodeExpression assignIdsMatch =
							new CodeBinaryOperatorExpression(shader.AttributeAssignId, CodeBinaryOperatorType.IdentityEquality, uid);

						CodeStatement assignAttribute = new CodeAssignStatement(sampler, shader.AttributeAssignValue);

						CodeConditionStatement performAssign =
							new CodeConditionStatement(assignIdsMatch,
								assignAttribute, //call the assignment code
								new CodeMethodReturnStatement(new CodePrimitiveExpression(true))); //return true, set correctly.

						add(performAssign);
					}
				}
			}
		}


		public override void AddConstructor(IShaderDom shader, Action<CodeStatement> add)
		{
			//setup the default values for the masks and samplers

			if (psSamplers.Count > 0)
			{
				//set the PS mask to a big number
				CodeStatement assign = new CodeAssignStatement(new CodeFieldReferenceExpression(shader.Instance, "psm"), new CodePrimitiveExpression(int.MaxValue));
				add(assign);
			}

			if (vsSamplers.Count > 0)
			{
				//set the PS mask to a big number
				CodeStatement assign = new CodeAssignStatement(new CodeFieldReferenceExpression(shader.Instance, "vsm"), new CodePrimitiveExpression(int.MaxValue));
				add(assign);
			}

			//set the samplers to their defaults

			foreach (SharedSampler ss in allSamplers.Values)
			{
				//get the default as an int,
				//then cast it to a sampler state using explicit cast construction
				CodeExpression value = new CodeCastExpression(typeof(Xen.Graphics.State.TextureSamplerState), new CodePrimitiveExpression((int)ss.DefaultState));

				//assign the value
				CodeStatement assign = new CodeAssignStatement(new CodeFieldReferenceExpression(shader.Instance, "ts" + ss.Index), value);
				add(assign);
			}
		}


		private void GenerateProperties(IShaderDom shader, Action<CodeTypeMember, string> add)
		{

			foreach (SharedSampler ss in allSamplers.Values)
			{

				//simlar in style to:

				/*
							 
					public Xen.Graphics.State.TextureSamplerState ShadowSampler
					{
						get
						{
							return this.ps_0;
						}
						set
						{
							if ((value != this.ps_0))
							{
								this.ps_0 = value;
								this.ps_m = (this.ps_m | 1);
							}
						}
					}
				 * 
				 */
				CodeExpression current = new CodeFieldReferenceExpression(shader.Instance, "ts" + ss.Index);
				CodeExpression value = new CodePropertySetValueReferenceExpression();
				CodeExpression psm = new CodeFieldReferenceExpression(shader.Instance, "psm");
				CodeExpression vsm = new CodeFieldReferenceExpression(shader.Instance, "vsm");

				CodeMemberProperty prop = new CodeMemberProperty();
				prop.Type = new CodeTypeReference(typeof(Xen.Graphics.State.TextureSamplerState));
				prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;

				prop.Name = Common.ToUpper(ss.SamplerDetails.Name);

				prop.HasGet = true;
				prop.HasSet = true;

				prop.GetStatements.Add(new CodeMethodReturnStatement(current));

				//has changed?
				CodeBinaryOperatorExpression changed = new CodeBinaryOperatorExpression(current, CodeBinaryOperatorType.IdentityInequality, value);

				//changes..
				//3 if sampler is used by both PS and VS. 2 otherwise.
				CodeStatement[] updateOp = new CodeStatement[1 + (ss.PsIndex != -1 ? 1 : 0) + (ss.VsIndex != -1 ? 1 : 0)];

				if (ss.PsIndex != -1) //update the ps mask
					updateOp[0] = new CodeAssignStatement(psm, new CodeBinaryOperatorExpression(psm, CodeBinaryOperatorType.BitwiseOr, new CodePrimitiveExpression(1 << ss.PsIndex)));
				if (ss.VsIndex != -1) //update the vs mask
					updateOp[updateOp.Length - 2] = new CodeAssignStatement(vsm, new CodeBinaryOperatorExpression(vsm, CodeBinaryOperatorType.BitwiseOr, new CodePrimitiveExpression(1 << ss.VsIndex)));

				//finally, assign the sampler
				updateOp[updateOp.Length - 1] = new CodeAssignStatement(current, value);

				//pull it all together
				prop.SetStatements.Add(new CodeConditionStatement(changed, updateOp));

				add(prop, string.Format("Get/Set the Texture Sampler State for '{0} {1}'", Common.ToUpper(ss.SamplerDetails.Type), ss.SamplerDetails.Name));
			}
			
			
			


			//add the texture properties
			for (int i = 0; i < textures.Length; i++)
			{
				TextureAssociation tex = textures[i];

				//only add if they are used
				if (tex.PsSamplers.Count > 0 || tex.VsSamplers.Count > 0)
				{
					//create the property,
					//very similar to the sampler property.

					CodeExpression current = new CodeFieldReferenceExpression(shader.Instance, "tx" + i);
					CodeExpression value = new CodePropertySetValueReferenceExpression();
					CodeExpression psm = new CodeFieldReferenceExpression(shader.Instance, "psm");
					CodeExpression vsm = new CodeFieldReferenceExpression(shader.Instance, "vsm");

					CodeMemberProperty prop = new CodeMemberProperty();
					prop.Type = new CodeTypeReference(Common.GetTextureType(tex.Texture.Type));
					prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;

					if (tex.IsGlobal) //make the property private if it's global
						prop.Attributes = MemberAttributes.Private | MemberAttributes.Final;

					prop.Name = Common.ToUpper(tex.Texture.Name);

					prop.HasGet = true;
					prop.HasSet = true;

					prop.GetStatements.Add(new CodeMethodReturnStatement(current));

					//has changed?
					CodeBinaryOperatorExpression changed = new CodeBinaryOperatorExpression(current, CodeBinaryOperatorType.IdentityInequality, value);


					//changes.. (similar to sampler property)
					CodeStatement[] updateOp = new CodeStatement[1 + tex.PsSamplers.Count + tex.VsSamplers.Count];
					int update = 0;

					for (int n = 0; n < tex.PsSamplers.Count; n++)
						updateOp[update++] = new CodeAssignStatement(psm, new CodeBinaryOperatorExpression(psm, CodeBinaryOperatorType.BitwiseOr, new CodePrimitiveExpression(1 << tex.PsSamplers[n])));
					for (int n = 0; n < tex.VsSamplers.Count; n++)
						updateOp[update++] = new CodeAssignStatement(vsm, new CodeBinaryOperatorExpression(vsm, CodeBinaryOperatorType.BitwiseOr, new CodePrimitiveExpression(1 << tex.VsSamplers[n])));

					//finally, assign the sampler
					updateOp[updateOp.Length - 1] = new CodeAssignStatement(current, value);

					//pull it all together
					prop.SetStatements.Add(new CodeConditionStatement(changed, updateOp));

					add(prop, string.Format("Get/Set the Bound texture for '{0} {1}'", tex.Texture.Type, tex.Texture.Name));
				}
			}
		}


		//bind the samplers / textures
		public override void AddBind(IShaderDom shader, Action<CodeStatement, string> add)
		{
			// assign global textures first...


			bool firstAssign = true;
			for (int i = 0; i < this.textures.Length; i++)
			{
				if ((this.textures[i].PsSamplers.Count > 0 ||
					this.textures[i].VsSamplers.Count > 0) &&
					this.textures[i].IsGlobal)
				{
					//assign the global

					//a bit like:
					//this.ShadowMap = state.GetGlobalTexture2D(ShadowShaderBlend.tid0);

					CodeExpression uid = new CodeFieldReferenceExpression(shader.ShaderClassEx, "tid" + i);
					CodeExpression prop = new CodePropertyReferenceExpression(shader.Instance, Common.ToUpper(this.textures[i].Texture.Name));

					CodeExpression call = new CodeMethodInvokeExpression(shader.ShaderSystemRef, "GetGlobal" + this.textures[i].Texture.Type, uid);

					CodeStatement assign = new CodeAssignStatement(prop, call);

					add(assign, firstAssign ? "Assign global textures" : null);
					firstAssign = false;
				}
			}


			//bind the samplers / textures
			//looks something like this:
			/*
			 
			if ((ic == true))
			{
				this.ps_m = 65535;
			}
			if ((this.ps_m != 0))
			{
				if (((this.ps_m & 1)
							== 1))
				{
					state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
				}
				if (((this.ps_m & 2)
							== 2))
				{
					state.SetPixelShaderSampler(1, this.ps_1t, this.ps_1);
				}
				this.ps_m = 0;
			}
			 
			 */

			CodeExpression psm = new CodeFieldReferenceExpression(shader.Instance, "psm");
			CodeExpression vsm = new CodeFieldReferenceExpression(shader.Instance, "vsm");

			if (psSamplers.Count == 0 && vsSamplers.Count == 0)
				return;

			CodeStatement[] wipe = new CodeStatement[(psSamplers.Count > 0 ? 1 : 0) + (vsSamplers.Count > 0 ? 1 : 0)];
			if (psSamplers.Count > 0)
				wipe[0] = new CodeAssignStatement(psm, new CodePrimitiveExpression(int.MaxValue));
			if (vsSamplers.Count > 0)
				wipe[wipe.Length-1] = new CodeAssignStatement(vsm, new CodePrimitiveExpression(int.MaxValue));

			//if instance changed, set the masks to big numbers
			CodeStatement maskWipe = new CodeConditionStatement(
				new CodeBinaryOperatorExpression(shader.BindShaderInstanceChange, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)),
				wipe);

			add(maskWipe, "Reset change masks");

			//setup the pixel samplers
			if (psSamplers.Count > 0)
			{
				List<CodeStatement> statements = new List<CodeStatement>();

				//set the samplers
				foreach (SharedSampler ss in this.allSamplers.Values)
				{
					/*
						if (((this.ps_m & 1)
									== 1))
						{
							state.SetPixelShaderSampler(0, this.ps_0t, this.ps_0);
						}
					 */
					if (ss.PsIndex != -1)
					{
						//find the matching texture
						int texIndex = -1;
						for (int i = 0; i < this.textures.Length; i++)
						{
							if (textures[i].PsSamplers.Contains(ss.PsIndex))
							{
								texIndex = i;
								break;
							}
						}

						CodeExpression sampler = new CodeFieldReferenceExpression(shader.Instance, "ts" + ss.Index);
						CodeExpression texture = new CodeFieldReferenceExpression(shader.Instance, "tx" + texIndex);
						CodeExpression masked = new CodeBinaryOperatorExpression(psm, CodeBinaryOperatorType.BitwiseAnd, new CodePrimitiveExpression(1 << ss.PsIndex));

						CodeExpression assign = new CodeMethodInvokeExpression(shader.ShaderSystemRef, "SetPixelShaderSampler",
							new CodePrimitiveExpression(ss.PsIndex), texture, sampler);

						CodeStatement full = new CodeConditionStatement(
							new CodeBinaryOperatorExpression(masked, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(1 << ss.PsIndex)),
							shader.ETS(assign));

						statements.Add(full);
					}
				}


				//reset the mask
				statements.Add(new CodeAssignStatement(psm, new CodePrimitiveExpression((int)0)));

				//add it all up.
				//if the mask is non-zero,
				CodeStatement setAll = new CodeConditionStatement(
					new CodeBinaryOperatorExpression(psm, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression((int)0)),
					statements.ToArray());

				add(setAll, "Set pixel shader samplers");
			}


			//do it all again for VS
			if (vsSamplers.Count > 0)
			{
				List<CodeStatement> statements = new List<CodeStatement>();

				//set the samplers
				foreach (SharedSampler ss in this.allSamplers.Values)
				{
					if (ss.VsIndex != -1)
					{
						//find the matching texture
						int texIndex = -1;
						for (int i = 0; i < this.textures.Length; i++)
						{
							if (textures[i].VsSamplers.Contains(ss.VsIndex))
							{
								texIndex = i;
								break;
							}
						}

						CodeExpression sampler = new CodeFieldReferenceExpression(shader.Instance, "ts" + ss.Index);
						CodeExpression texture = new CodeFieldReferenceExpression(shader.Instance, "tx" + texIndex);
						CodeExpression masked = new CodeBinaryOperatorExpression(vsm, CodeBinaryOperatorType.BitwiseAnd, new CodePrimitiveExpression(1 << ss.VsIndex));

						CodeExpression assign = new CodeMethodInvokeExpression(shader.ShaderSystemRef, "SetVertexShaderSampler",
							new CodePrimitiveExpression(ss.VsIndex), texture, sampler);

						CodeStatement full = new CodeConditionStatement(
							new CodeBinaryOperatorExpression(masked, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(1 << ss.VsIndex)),
							shader.ETS(assign));

						statements.Add(full);
					}
				}


				//reset the mask
				statements.Add(new CodeAssignStatement(vsm, new CodePrimitiveExpression((int)0)));

				//add it all up.
				//if the mask is non-zero,
				CodeStatement setAll = new CodeConditionStatement(
					new CodeBinaryOperatorExpression(vsm, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression((int)0)),
					statements.ToArray());

				add(setAll, "Set vertex shader samplers");
			}
		}

		public override void AddChangedCondition(IShaderDom shader, Action<CodeExpression> add)
		{
			if (psSamplers.Count > 0)
			{
				//add the PS sampler mask
				CodeExpression mask = new CodeFieldReferenceExpression(shader.Instance, "psm");
				add(new CodeBinaryOperatorExpression(mask, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0)));
			}

			if (vsSamplers.Count > 0)
			{
				//add the VS sampler mask
				CodeExpression mask = new CodeFieldReferenceExpression(shader.Instance, "vsm");
				add(new CodeBinaryOperatorExpression(mask, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0)));
			}
		}

	}
}