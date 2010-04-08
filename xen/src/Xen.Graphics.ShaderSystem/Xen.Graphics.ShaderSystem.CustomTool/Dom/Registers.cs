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
	//stores the shader register classes (vreg, etc)
	public sealed class ShaderRegisters : DomBase
	{
		private readonly RegisterSet vsReg, psReg, vsPreReg, psPreReg;
		private readonly Vector4[] vsDefault, psDefault;
		private readonly bool[] vsBooleanDefault, psBooleanDefault;
		private readonly Dictionary<string, CodeExpression> staticArrays;
		private readonly TechniqueExtraData techniqueData;

		public ShaderRegisters(SourceShader source, string techniqueName, Platform platform)
		{
			AsmTechnique technique = source.GetAsmTechnique(techniqueName, platform);

			vsReg = technique.VertexShader.RegisterSet;
			psReg = technique.PixelShader.RegisterSet;


			if (technique.VertexPreShader != null)
				vsPreReg = technique.VertexPreShader.RegisterSet;

			if (technique.PixelPreShader != null)
				psPreReg = technique.PixelPreShader.RegisterSet;

			staticArrays = new Dictionary<string, CodeExpression>();

			if (technique.TechniqueExtraData != null)
			{
				this.techniqueData = technique.TechniqueExtraData;

				psDefault = technique.TechniqueExtraData.PixelShaderConstants;
				vsDefault = technique.TechniqueExtraData.VertexShaderConstants;

				psBooleanDefault = technique.TechniqueExtraData.PixelShaderBooleanConstants;
				vsBooleanDefault = technique.TechniqueExtraData.VertexShaderBooleanConstants;
			}
		}

		public override void AddReadonlyMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
			if (platform != Platform.Both)
				return;

			if (vsReg.FloatRegisterCount > 0)
			{
				//create the vertex registers
				CodeMemberField field = new CodeMemberField(typeof(ConstantArray), shader.VertexShaderRegistersRef.FieldName);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

				add(field, "Vertex shader register storage");
			}

			//and the PS

			if (psReg.FloatRegisterCount > 0)
			{
				//create the pixel registers
				CodeMemberField field = new CodeMemberField(typeof(ConstantArray), shader.PixelShaderRegistersRef.FieldName);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

				add(field, "Pixel shader register storage");
			}

			//now the Preshaders
			if (vsPreReg != null && vsPreReg.FloatRegisterCount > 0)
			{
				//create the vertex preshader registers
				CodeMemberField field = new CodeMemberField(typeof(ConstantArray), shader.VertexPreShaderRegistersRef.FieldName);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
				add(field, "Vertex preshader register storage");
			}

			//and the PS
			if (psPreReg != null && psPreReg.FloatRegisterCount > 0)
			{
				//create the pixel preshader registers
				CodeMemberField field = new CodeMemberField(typeof(ConstantArray), shader.PixelPreShaderRegistersRef.FieldName);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
				add(field, "Pixel preshader register storage");
			}


			//now do the boolean registers
			if (vsReg.BooleanRegisterCount > 0)
			{
				//create the vertex boolean registers
				CodeMemberField field = new CodeMemberField(typeof(bool[]), shader.VertexShaderBooleanRegistersRef.FieldName);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

				add(field, "Vertex shader boolean register storage");
			}
			//now the PS
			if (psReg.BooleanRegisterCount > 0)
			{
				//create the vertex boolean registers
				CodeMemberField field = new CodeMemberField(typeof(bool[]), shader.PixelShaderBooleanRegistersRef.FieldName);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

				add(field, "Pixel shader boolean register storage");
			}


			//add any statics created
			foreach (KeyValuePair<string, CodeExpression> array in staticArrays)
			{
				CodeMemberField field = new CodeMemberField(typeof(float[]), array.Key);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final | MemberAttributes.Static;
				field.InitExpression = array.Value;

				add(field, "Register default values");
			}
		}

		public override void AddMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
			if (platform != Platform.Both)
				return;

			//add change values for boolean registers
			if (vsReg.BooleanRegisterCount > 0)
			{
				CodeMemberField field = new CodeMemberField(typeof(bool), shader.VertexShaderBooleanRegistersChangedRef.FieldName);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
				add(field, null);
			}
			//now the PS
			if (psReg.BooleanRegisterCount > 0)
			{
				CodeMemberField field = new CodeMemberField(typeof(bool), shader.PixelShaderBooleanRegistersChangedRef.FieldName);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
				add(field, null);
			}
		}

		public override void AddConstructor(IShaderDom shader, Action<CodeStatement> add)
		{
			//setup registers
			if (vsReg.FloatRegisterCount > 0)
			{
				//assign
				CodeExpression create = new CodeObjectCreateExpression(typeof(ConstantArray),
					new CodePrimitiveExpression(vsReg.FloatRegisterCount)); //create with the number of registers

				add(new CodeAssignStatement(shader.VertexShaderRegistersRef,create));

				//call the set method?
				int offset;
				CodeExpression initArray = InitaliseConstants(vsReg.FloatRegisterCount, vsDefault, shader.CompileDirectives, out offset);

				if (initArray != null)
				{
					//the array will be stored statically as a member, so not to leak.
					string name = "vreg_def";
					staticArrays.Add(name, initArray); 

					CodeMethodInvokeExpression setCall = new CodeMethodInvokeExpression(shader.VertexShaderRegistersRef, "Set",
						new CodePrimitiveExpression(offset), new CodeFieldReferenceExpression(shader.ShaderClassEx, name)); // start from zero
					add(shader.ETS(setCall));
				}
			}

			//and the PS

			if (psReg.FloatRegisterCount > 0)
			{
				//create the vertex registers
				CodeExpression create = new CodeObjectCreateExpression(typeof(ConstantArray),
					new CodePrimitiveExpression(psReg.FloatRegisterCount)); //create with the number of registers

				add(new CodeAssignStatement(shader.PixelShaderRegistersRef, create));

				//call the set method?
				int offset;
				CodeExpression initArray = InitaliseConstants(psReg.FloatRegisterCount, psDefault, shader.CompileDirectives, out offset);

				if (initArray != null)
				{
					//the array will be stored statically as a member, so not to leak.
					string name = "preg_def";
					staticArrays.Add(name, initArray); 

					CodeMethodInvokeExpression setCall = new CodeMethodInvokeExpression(shader.PixelShaderRegistersRef, "Set",
						new CodePrimitiveExpression(offset), new CodeFieldReferenceExpression(shader.ShaderClassEx, name));
					add(shader.ETS(setCall));
				}
			}

			
			//setup preshader registers
			if (vsPreReg != null && vsPreReg.FloatRegisterCount > 0)
			{
				CodeExpression create = new CodeObjectCreateExpression(typeof(ConstantArray),
					new CodePrimitiveExpression(vsPreReg.FloatRegisterCount)); //create with the number of registers
				add(new CodeAssignStatement(shader.VertexPreShaderRegistersRef, create));

				//call the set method?
				int offset;
				CodeExpression initArray = GeneratePreShaderFloatDefaults(this.techniqueData, vsPreReg, vsPreReg.FloatRegisterCount, shader.CompileDirectives, out offset);

				if (initArray != null)
				{
					string name = "vpreg_def";
					staticArrays.Add(name, initArray);

					CodeMethodInvokeExpression setCall = new CodeMethodInvokeExpression(shader.VertexPreShaderRegistersRef, "Set",
						new CodePrimitiveExpression(offset), new CodeFieldReferenceExpression(shader.ShaderClassEx, name)); // start from zero
					add(shader.ETS(setCall));
				}
			}
			//pixel preshader
			if (psPreReg != null && psPreReg.FloatRegisterCount > 0)
			{
				//assign
				CodeExpression create = new CodeObjectCreateExpression(typeof(ConstantArray),
					new CodePrimitiveExpression(psPreReg.FloatRegisterCount)); //create with the number of registers
				add(new CodeAssignStatement(shader.PixelPreShaderRegistersRef, create));

				//call the set method?
				int offset;
				CodeExpression initArray = GeneratePreShaderFloatDefaults(this.techniqueData, psPreReg, psPreReg.FloatRegisterCount, shader.CompileDirectives, out offset);

				if (initArray != null)
				{
					string name = "ppreg_def";
					staticArrays.Add(name, initArray);

					CodeMethodInvokeExpression setCall = new CodeMethodInvokeExpression(shader.PixelPreShaderRegistersRef, "Set",
						new CodePrimitiveExpression(offset), new CodeFieldReferenceExpression(shader.ShaderClassEx, name)); // start from zero
					add(shader.ETS(setCall));
				}
			}

			//boolean registers
			if (vsReg.BooleanRegisterCount > 0)
			{
				//assign
				CodeExpression create = new CodeArrayCreateExpression(typeof(bool),
					new CodePrimitiveExpression(vsReg.BooleanRegisterCount)); //create with the number of registers

				add(new CodeAssignStatement(shader.VertexShaderBooleanRegistersRef, create));
				add(new CodeAssignStatement(shader.VertexShaderBooleanRegistersChangedRef, new CodePrimitiveExpression(true)));

				//if any are default non-zero, assign them.
				if (vsBooleanDefault != null)
				{
					for (int i = 0; i < this.vsBooleanDefault.Length && i < vsReg.BooleanRegisterCount; i++)
					{
						if (this.vsBooleanDefault[i])
							add(new CodeAssignStatement(new CodeArrayIndexerExpression(shader.VertexShaderBooleanRegistersRef, new CodePrimitiveExpression(i)), new CodePrimitiveExpression(true)));
					}
				}
			}
			//and the PS...
			if (psReg.BooleanRegisterCount > 0)
			{
				//assign
				CodeExpression create = new CodeArrayCreateExpression(typeof(bool),
					new CodePrimitiveExpression(psReg.BooleanRegisterCount)); //create with the number of registers

				add(new CodeAssignStatement(shader.PixelShaderBooleanRegistersRef, create));
				add(new CodeAssignStatement(shader.PixelShaderBooleanRegistersChangedRef, new CodePrimitiveExpression(true)));

				//if any are default non-zero, assign them.
				if (psBooleanDefault != null)
				{
					for (int i = 0; i < this.psBooleanDefault.Length && i < psReg.BooleanRegisterCount; i++)
					{
						if (this.psBooleanDefault[i])
							add(new CodeAssignStatement(new CodeArrayIndexerExpression(shader.PixelShaderBooleanRegistersRef, new CodePrimitiveExpression(i)), new CodePrimitiveExpression(true)));
					}
				}
			}
		}
		
		public override void AddBindEnd(IShaderDom shader, Action<CodeStatement, string> add)
		{
			//set the constants
			//eg:
			/*
			 * 
			if (((this.vreg.change == true)
						|| (ic == true)))
			{
				state.SetShaderConstants(this.vreg.array, null);
				this.vreg.change = false;
			}
			 */

			CodeExpression vreg = new CodePrimitiveExpression(null);
			CodeExpression preg = vreg;

			CodeExpression setConstantsCondition = null;

			if (vsReg.FloatRegisterCount > 0)
			{
				//local variable storing the registers
				CodeVariableDeclarationStatement vregd = new CodeVariableDeclarationStatement(typeof(Vector4[]), "vc");
				vregd.InitExpression = vreg;
				vreg = new CodeVariableReferenceExpression(vregd.Name);
				add(vregd,"Vertex shader registers");

				//assign if changed
				CodeBinaryOperatorExpression assignCondition = new CodeBinaryOperatorExpression(
					new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(shader.VertexShaderRegistersRef, "change"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)),
					CodeBinaryOperatorType.BooleanOr,
					new CodeBinaryOperatorExpression(shader.BindShaderInstanceChange, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)));

				//assign
				CodeConditionStatement assign = new CodeConditionStatement(assignCondition,
					new CodeAssignStatement(new CodeVariableReferenceExpression(vregd.Name), new CodeFieldReferenceExpression(shader.VertexShaderRegistersRef, "array")),
					new CodeAssignStatement(new CodeFieldReferenceExpression(shader.VertexShaderRegistersRef, "change"), new CodePrimitiveExpression(false)));

				add(assign, null);

				setConstantsCondition = new CodeBinaryOperatorExpression(vreg, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
			}

			//again for PS
			if (psReg.FloatRegisterCount > 0)
			{
				//local variable storing the registers
				CodeVariableDeclarationStatement pregd = new CodeVariableDeclarationStatement(typeof(Vector4[]), "pc");
				pregd.InitExpression = preg;
				preg = new CodeVariableReferenceExpression(pregd.Name);
				add(pregd, "Pixel shader registers");

				//assign if changed
				CodeBinaryOperatorExpression assignCondition = new CodeBinaryOperatorExpression(
					new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(shader.PixelShaderRegistersRef, "change"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)),
					CodeBinaryOperatorType.BooleanOr,
					new CodeBinaryOperatorExpression(shader.BindShaderInstanceChange, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true)));

				//assign
				CodeConditionStatement assign = new CodeConditionStatement(assignCondition,
					new CodeAssignStatement(new CodeVariableReferenceExpression(pregd.Name), new CodeFieldReferenceExpression(shader.PixelShaderRegistersRef, "array")),
					new CodeAssignStatement(new CodeFieldReferenceExpression(shader.PixelShaderRegistersRef, "change"), new CodePrimitiveExpression(false)));

				add(assign, null);

				CodeExpression condition = new CodeBinaryOperatorExpression(preg, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
				if (setConstantsCondition != null)
					setConstantsCondition = new CodeBinaryOperatorExpression(condition, CodeBinaryOperatorType.BooleanOr, setConstantsCondition);
				else
					setConstantsCondition = condition;
			}

			if (setConstantsCondition != null)
			{
				//set the shaders
				CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(shader.ShaderSystemRef, "SetShaderConstants", vreg, preg);

				//invoke if the constants are not null.

				add(new CodeConditionStatement(setConstantsCondition, shader.ETS(invoke)), null);
			}


			setConstantsCondition = null;

			//set the boolean registers
			if (vsReg.BooleanRegisterCount > 0 ||
				psReg.BooleanRegisterCount > 0)
			{
				CodeExpression vsb = new CodePrimitiveExpression(null);
				CodeExpression psb = new CodePrimitiveExpression(null);

				if (vsReg.BooleanRegisterCount > 0)
				{
					CodeVariableDeclarationStatement vs_var = new CodeVariableDeclarationStatement(typeof(bool[]), "vcb", new CodePrimitiveExpression(null));
					add(vs_var, null);
					vsb = new CodeVariableReferenceExpression(vs_var.Name);
					CodeConditionStatement changed = new CodeConditionStatement(shader.VertexShaderBooleanRegistersChangedRef,
						new CodeAssignStatement(vsb, shader.VertexShaderBooleanRegistersRef),
						new CodeAssignStatement(shader.VertexShaderBooleanRegistersChangedRef, new CodePrimitiveExpression(false)));
					add(changed, null);
					setConstantsCondition = new CodeBinaryOperatorExpression(vsb, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
				}
				if (psReg.BooleanRegisterCount > 0)
				{
					CodeVariableDeclarationStatement ps_var = new CodeVariableDeclarationStatement(typeof(bool[]), "pcb", new CodePrimitiveExpression(null));
					add(ps_var, null);
					psb = new CodeVariableReferenceExpression(ps_var.Name);
					CodeConditionStatement changed = new CodeConditionStatement(shader.PixelShaderBooleanRegistersChangedRef,
						new CodeAssignStatement(psb, shader.PixelShaderBooleanRegistersRef),
						new CodeAssignStatement(shader.PixelShaderBooleanRegistersChangedRef, new CodePrimitiveExpression(false)));
					add(changed, null);

					CodeBinaryOperatorExpression condition = new CodeBinaryOperatorExpression(psb, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
					if (setConstantsCondition == null)
						setConstantsCondition = condition;
					else
						setConstantsCondition = new CodeBinaryOperatorExpression(setConstantsCondition, CodeBinaryOperatorType.BooleanOr, condition);
				}
				//invoke the call to set the constants
				//set the shaders
				CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(shader.ShaderSystemRef, "SetShaderBooleanConstants", vsb, psb);

				add(new CodeConditionStatement(setConstantsCondition, shader.ETS(invoke)), null);
			}
		}

		private CodeExpression GeneratePreShaderFloatDefaults(TechniqueExtraData defaults, RegisterSet registers, int count, CompileDirectives directives, out int offset)
		{
			offset = 0;
			if (count == 0 || defaults == null) return null;

			Vector4[] constants = new Vector4[count];
			bool set = false;

			foreach (KeyValuePair<string,Vector4[]> entry in defaults.DefaultSingleValues)
			{
				Register reg;
				if (registers.TryGetRegister(entry.Key, out reg))
				{
					if (reg.Category == RegisterCategory.Float4)
					{
						for (int i = 0; i < Math.Max(1,reg.ArraySize) && i < entry.Value.Length && i < count - reg.Index; i++)
						{
							constants[i + reg.Index] = entry.Value[i];
							set = true;
						}
					}
				}
			}

			if (!set) return null;
			return InitaliseConstants(count, constants, directives, out offset);
		}

		//compresses a vector4 array down by removing the zero elements on the start/end (which is often A LOT)
		private CodeExpression InitaliseConstants(int count, Vector4[] values, CompileDirectives directives, out int offset)
		{
			offset = 0;

			if (count > values.Length)
				count = values.Length;

			if (values == null)
				return null;

			int maxNonZero = -1;
			int minNonZero = int.MaxValue;
			for (int i = 0; i < count; i++)
			{
				if (values[i] != Vector4.Zero)
				{
					maxNonZero = Math.Max(maxNonZero, i);
					minNonZero = Math.Min(minNonZero, i);
				}
			}

			if (maxNonZero >= minNonZero)
			{
				//sub array
				float[] array = new float[Math.Min(values.Length, maxNonZero - minNonZero + 1) * 4];
				for (int i = minNonZero; i <= maxNonZero; i++)
				{
					array[(i - minNonZero) * 4 + 0] = values[i].X;
					array[(i - minNonZero) * 4 + 1] = values[i].Y;
					array[(i - minNonZero) * 4 + 2] = values[i].Z;
					array[(i - minNonZero) * 4 + 3] = values[i].W;
				}

				for (int i = 0; i < array.Length; i++)
				{
					//remove any NaN values
					if (float.IsNaN(array[i]))
						array[i] = 0;
				}

				offset = minNonZero;
				return ShaderBytes.ToArray(array, directives);
			}
			return null;
		}


		public override void AddChangedCondition(IShaderDom shader, Action<CodeExpression> add)
		{
			if (vsReg.FloatRegisterCount > 0)
				add(new CodeFieldReferenceExpression(shader.VertexShaderRegistersRef, "change"));
			if (psReg.FloatRegisterCount > 0)
				add(new CodeFieldReferenceExpression(shader.PixelShaderRegistersRef, "change"));
			if (vsPreReg != null && vsPreReg.FloatRegisterCount > 0)
				add(new CodeFieldReferenceExpression(shader.VertexPreShaderRegistersRef, "change"));
			if (psPreReg != null && psPreReg.FloatRegisterCount > 0)
				add(new CodeFieldReferenceExpression(shader.PixelPreShaderRegistersRef, "change"));
		}
	}
}
