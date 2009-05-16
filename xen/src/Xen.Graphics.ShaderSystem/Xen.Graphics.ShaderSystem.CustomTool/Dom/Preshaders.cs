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
	//this class manages the preshaders, if they are in use.
	public sealed class Preshaders : DomBase
	{
		private readonly PreshaderSrc pixelPreShader, vertexPreShader;
		private readonly CodeStatementCollection pixelPreShaderStatements, vertexPreShaderStatements;

		public Preshaders(SourceShader source, string techniqueName, Platform platform)
		{
			AsmTechnique technique = source.GetAsmTechnique(techniqueName, platform);

			if (technique.PixelPreShader != null)
			{
				pixelPreShaderStatements = new CodeStatementCollection();
				pixelPreShader = new PreshaderSrc(technique.PixelPreShader, pixelPreShaderStatements);

				technique.PixelShader.RegisterSet.SetMinFloatRegisterCount(pixelPreShader.MaxConstantRegisterAccess);
			}

			if (technique.VertexPreShader != null)
			{
				vertexPreShaderStatements = new CodeStatementCollection();
				vertexPreShader = new PreshaderSrc(technique.VertexPreShader, vertexPreShaderStatements);

				technique.VertexShader.RegisterSet.SetMinFloatRegisterCount(vertexPreShader.MaxConstantRegisterAccess);
			}
		}

		public override void AddMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
			if (platform != Platform.Both)
				return;

			//add the preshader methods.

			if (vertexPreShader != null)
			{
				CodeMemberMethod method = new CodeMemberMethod();
				method.Name = "vspre";
				method.Attributes = MemberAttributes.Private | MemberAttributes.Final;

				//add local variables to access arrays
				CodeStatement constants = new CodeVariableDeclarationStatement(typeof(Vector4[]), "c", new CodeFieldReferenceExpression(shader.VertexShaderRegistersRef, "array"));
				CodeStatement preconstants = new CodeVariableDeclarationStatement(typeof(Vector4[]), "p", new CodeFieldReferenceExpression(shader.VertexPreShaderRegistersRef, "array"));

				method.Statements.Add(constants);
				method.Statements.Add(preconstants);

				//add temporary registers
				for (int i = 0; i < vertexPreShader.MaxTempRegisters; i++)
				{
					CodeStatement temp = new CodeVariableDeclarationStatement(typeof(Vector4), "r" + i);
					method.Statements.Add(temp);
				}

				method.Statements.AddRange(vertexPreShaderStatements);

				//finally, update the changed values
				method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(shader.VertexPreShaderRegistersRef, "change"), new CodePrimitiveExpression(false)));
				method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(shader.VertexShaderRegistersRef, "change"), new CodePrimitiveExpression(true)));

				add(method, "Vertex PreShader");
			}

			//do it all again for pixel shaders
			if (pixelPreShader != null)
			{
				CodeMemberMethod method = new CodeMemberMethod();
				method.Name = "pspre";
				method.Attributes = MemberAttributes.Private | MemberAttributes.Final;

				//add local variables to access arrays
				CodeStatement constants = new CodeVariableDeclarationStatement(typeof(Vector4[]), "c", new CodeFieldReferenceExpression(shader.PixelShaderRegistersRef, "array"));
				CodeStatement preconstants = new CodeVariableDeclarationStatement(typeof(Vector4[]), "p", new CodeFieldReferenceExpression(shader.PixelPreShaderRegistersRef, "array"));

				method.Statements.Add(constants);
				method.Statements.Add(preconstants);

				//add temporary registers
				for (int i = 0; i < vertexPreShader.MaxTempRegisters; i++)
				{
					CodeStatement temp = new CodeVariableDeclarationStatement(typeof(Vector4), "r" + i);
					method.Statements.Add(temp);
				}
				method.Statements.AddRange(pixelPreShaderStatements);

				//finally, update the changed values
				method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(shader.PixelPreShaderRegistersRef, "change"), new CodePrimitiveExpression(false)));
				method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(shader.PixelShaderRegistersRef, "change"), new CodePrimitiveExpression(true)));

				add(method, "Pixel PreShader");
			}
		}

		public override void AddBind(IShaderDom shader, Action<CodeStatement, string> add)
		{
			//call the preshaders

			if (vertexPreShader != null)
			{
				CodeExpression call = new CodeMethodInvokeExpression(shader.Instance, "vspre");
				CodeExpression condition = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(shader.VertexPreShaderRegistersRef, "change"),  CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true));
				CodeStatement invoke = new CodeConditionStatement(condition, shader.ETS(call));

				add(invoke, "run the vertex preshader");
			}
			
			//pixel
			if (pixelPreShader != null)
			{
				CodeExpression call = new CodeMethodInvokeExpression(shader.Instance, "pspre");
				CodeExpression condition = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(shader.PixelPreShaderRegistersRef, "change"),  CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(true));
				CodeStatement invoke = new CodeConditionStatement(condition, shader.ETS(call));

				add(invoke, "run the pixel preshader");
			}
		}
	}
}