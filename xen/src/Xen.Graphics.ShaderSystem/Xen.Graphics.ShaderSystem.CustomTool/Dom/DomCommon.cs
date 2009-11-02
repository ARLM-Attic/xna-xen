﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using Xen.Graphics.ShaderSystem.CustomTool.FX;

namespace Xen.Graphics.ShaderSystem.CustomTool.Dom
{
	public interface IShaderDom
	{
		CodeFieldReferenceExpression GraphicsDeviceUID { get; }
		CodeTypeReference ShaderClass { get; }
		CodeTypeReferenceExpression ShaderClassEx { get; }
		CodeParameterDeclarationExpression ShaderSystemArg { get; }
		CodeArgumentReferenceExpression ShaderSystemRef { get; }

		CodeVariableReferenceExpression BindShaderTypeChange { get; }
		CodeVariableReferenceExpression BindShaderInstanceChange { get; }

		CodeFieldReferenceExpression VertexShaderRef { get; }
		CodeFieldReferenceExpression PixelShaderRef { get; }

		CodeFieldReferenceExpression VertexShaderBytesRef { get; }
		CodeFieldReferenceExpression PixelShaderBytesRef { get; }
		
		CodeFieldReferenceExpression VertexShaderRegistersRef { get; }
		CodeFieldReferenceExpression PixelShaderRegistersRef { get; }

		CodeFieldReferenceExpression VertexShaderBooleanRegistersRef { get; }
		CodeFieldReferenceExpression PixelShaderBooleanRegistersRef { get; }
		CodeFieldReferenceExpression VertexShaderBooleanRegistersChangedRef { get; }
		CodeFieldReferenceExpression PixelShaderBooleanRegistersChangedRef { get; }

		CodeFieldReferenceExpression VertexPreShaderRegistersRef { get; }
		CodeFieldReferenceExpression PixelPreShaderRegistersRef { get; }

		CodeExpression AttributeAssignId { get; }
		CodeExpression AttributeAssignValue { get; }

		CodeExpression Instance { get; }
		//expression to statement
		CodeStatement ETS(CodeExpression expression);
		CompileDirectives CompileDirectives { get; }

		SourceShader SourceShader { get; }
		string TechniqueName { get; }
		Platform Platform { get; }
	}

	public abstract class DomBase
	{
		public virtual void Setup(IShaderDom shader)
		{
		}
		public virtual void AddMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
		}
		public virtual void AddReadonlyMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
		}
		public virtual void AddConstructor(IShaderDom shader, Action<CodeStatement> add)
		{
		}
		public virtual void AddStaticGraphicsInit(IShaderDom shader, Action<CodeStatement, string> add)
		{
		}
		public virtual void AddWarm(IShaderDom shader, Action<CodeStatement, string> add)
		{
		}
		public virtual void AddBindBegin(IShaderDom shader, Action<CodeStatement, string> add)
		{
		}
		public virtual void AddBind(IShaderDom shader, Action<CodeStatement, string> add)
		{
		}
		public virtual void AddBindEnd(IShaderDom shader, Action<CodeStatement, string> add)
		{
		}
		public virtual void AddSetAttribute(IShaderDom shader, Action<CodeStatement> add, Type type)
		{
		}
		public virtual void AddChangedCondition(IShaderDom shader, Action<CodeExpression> add)
		{
		}

		public virtual void SetupMembers(IShaderDom shader)
		{
		}
	}
}
