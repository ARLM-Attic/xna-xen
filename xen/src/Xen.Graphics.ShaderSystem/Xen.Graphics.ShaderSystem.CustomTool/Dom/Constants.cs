using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xen.Graphics.ShaderSystem.CustomTool.FX;
using System.CodeDom;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Xen.Graphics.ShaderSystem.CustomTool.Dom
{

	struct SemanticType
	{
		public Type Type;
		public string Mapping;
		public bool Transpose;
	}

	struct SemanticMapping
	{
		public SemanticType Type;
		public CodeFieldReferenceExpression[] ChangeRefs; //may be more than one register set (pixel/vertex/preshader) using this semantic
		public Register Register;
	}

	struct GlobalAttribute
	{
		public Register Register;
		public Type Type;
		public CodeFieldReferenceExpression[] ChangeRefs;
		public CodeFieldReferenceExpression[] ArrayRefs;
		public CodeFieldReferenceExpression GlobalIdRef;
	}

	public sealed class ConstantSetup : DomBase
	{
		private readonly SourceShader source;
		private readonly string techniqueName;
		private readonly AsmTechnique asm;
		private readonly List<string> attributeNames;
		private readonly List<CodeFieldReferenceExpression> attributeFields;
		private readonly List<CodeFieldReferenceExpression> attributeArrayFields;
		private readonly Dictionary<Type, List<CodeStatement>> attributeAssignment;
		private SemanticType[] semanticTypes;
		private readonly List<SemanticMapping> semanticMapping;
		private int semanticMappingRefCount;
		//listings, and their registers (eg, VertexShader and vreg)
		private KeyValuePair<AsmListing, CodeExpression>[] listingRegisters;
		private readonly List<GlobalAttribute> globals;
		private int globalRefCount;

		public ConstantSetup(SourceShader source, string techniqueName, Platform platform)
		{
			this.source = source;
			this.techniqueName = techniqueName;

			this.attributeNames = new List<string>();
			this.attributeFields = new List<CodeFieldReferenceExpression>();
			this.attributeArrayFields = new List<CodeFieldReferenceExpression>();
			this.attributeAssignment = new Dictionary<Type, List<CodeStatement>>();
			this.semanticMapping = new List<SemanticMapping>();
			this.globals = new List<GlobalAttribute>();

			this.asm = source.GetAsmTechnique(techniqueName, platform);

			ComputeAllValidSemantics();
		}

		private void ProcessConstants(IShaderDom shader)
		{
			RegisterSet registers = asm.CommonRegisters;

			for (int i = 0; i < registers.RegisterCount; i++)
			{
				Register reg = registers.GetRegister(i);
				if (reg.Category == RegisterCategory.Float4)
				{
					if (reg.Semantic == null)
						this.attributeNames.Add(reg.Name);
					else
						ExtractSemantic(shader, reg);
				}
				else if (reg.Semantic != null && reg.Category != RegisterCategory.Texture)
				{
					throw new CompileException(string.Format("Error parsing semantic for '{1} {0}'. Semantic bound types may only be processed as Float4 or Texture registers", reg.Name, reg.Type));
				}
			}
		}

		private void ComputeAllValidSemantics()
		{
			//the methods in IShaderSystem are parsed.
			//anything in the format Set...Type is considered a semantic setter method.
			//eg, SetWorldMatrix is semantic 'WORLD' for Matrix.
			
			//valid types are Matrix, VectorX and Single
			Type[] types = new Type[] { typeof(Matrix), typeof(Vector2), typeof(Vector3), typeof(Vector4), typeof(Single) };
			MethodInfo[] methods = typeof(IShaderSystem).GetMethods();

			List<SemanticType> semantics = new List<SemanticType>();

			foreach (MethodInfo method in methods)
			{
				if (method.Name.Length > 8 &&
					method.Name.StartsWith("Set"))
				{
					//see if it ends with a type

					for (int i = 0; i < types.Length; i++)
					{
						if (method.Name.EndsWith(types[i].Name))
						{
							//got it.

							string mapping = method.Name.Substring(3, method.Name.Length - 3 - types[i].Name.Length);
							bool transpose = false;

							if (mapping.EndsWith("Transpose"))
							{
								mapping = mapping.Substring(0, mapping.Length - 9);
								transpose = true;
							}

							SemanticType semantic = new SemanticType();
							semantic.Mapping = mapping;
							semantic.Type = types[i];
							semantic.Transpose = transpose;

							semantics.Add(semantic);
							break;
						}
					}
				}
			}

			this.semanticTypes = semantics.ToArray();
		}

		//pull a semantic bound register
		private void ExtractSemantic(IShaderDom shader, Register reg)
		{
			string semantic = reg.Semantic;

			Type dataType = null;
			switch (reg.Rank)
			{
				case RegisterRank.FloatNx1:
				{
					switch (reg.Type)
					{
						case "float":
						case "float1"://?
							dataType = typeof(Single);
							break;
						case "float2":
							dataType = typeof(Vector2);
							break;
						case "float3":
							dataType = typeof(Vector3);
							break;
						case "float4":
							dataType = typeof(Vector4);
							break;
					}
				}
				break;
				case RegisterRank.FloatNx2:
				case RegisterRank.FloatNx3:
				case RegisterRank.FloatNx4:
					dataType = typeof(Matrix);
				break;
				case RegisterRank.IntNx1:
				case RegisterRank.IntNx2:
				case RegisterRank.IntNx3:
				case RegisterRank.IntNx4:
				{
					//ints are almost always mapped to floats for semantic bound types (EG vertex count)
					//since the register category has been validated to Float4, this is the case here
					switch (reg.Type)
					{
						case "int":
						case "int1"://?
							dataType = typeof(Single);
							break;
						case "int2":
							dataType = typeof(Vector2);
							break;
						case "int3":
							dataType = typeof(Vector3);
							break;
						case "int4":
							dataType = typeof(Vector4);
							break;
					}
				}
				break;
				case RegisterRank.Bool:
				dataType = typeof(Single);
				break;
			}
			

			if (semantic.Length == 6 && semantic.Equals("global", StringComparison.InvariantCultureIgnoreCase))
			{
				//special case global value.

				if (dataType == null)
					throw new CompileException(string.Format("Error parsing semantic for '{0}'. Global values of type '{1}' are not supported.",reg.Name, reg.Type));
				
				GlobalAttribute global = new GlobalAttribute();
				global.Register = reg;
				global.Type = dataType;

				global.GlobalIdRef = new CodeFieldReferenceExpression(shader.ShaderClassEx, string.Format("gid{0}", globals.Count));

				List<CodeFieldReferenceExpression> globalRefs = new List<CodeFieldReferenceExpression>();
				List<CodeFieldReferenceExpression> arrayRefs = new List<CodeFieldReferenceExpression>();

				foreach (KeyValuePair<AsmListing, CodeExpression> listing in listingRegisters)
				{
					Register sreg;
					RegisterSet registers = listing.Key.RegisterSet;
					CodeExpression registersRef = listing.Value;

					if (registers.TryGetRegister(reg.Name, out sreg))
					{
						string refId = string.Format("gc{0}", globalRefCount);
						globalRefs.Add(new CodeFieldReferenceExpression(shader.Instance, refId));

						if (reg.ArraySize != -1)
						{
							refId = string.Format("ga{0}", globalRefCount);
							arrayRefs.Add(new CodeFieldReferenceExpression(shader.Instance, refId));
						}
						globalRefCount++;
					}
				}

				global.ChangeRefs = globalRefs.ToArray();
				global.ArrayRefs = arrayRefs.ToArray();

				globals.Add(global);
				return;
			}

			if (reg.ArraySize != -1)
			{
				//INVALID. EXTERMINATE.
				throw new CompileException(string.Format("Shader attribute '{0}' is defined as an array and has a semantic '{1}'. Semantics other than 'GLOBAL' are invalid for Array types.", reg.Name, reg.Semantic));
			}

			bool isTranspose = semantic.Length > 9 && semantic.EndsWith("transpose", StringComparison.InvariantCultureIgnoreCase);

			if (isTranspose)
				semantic = semantic.Substring(0, semantic.Length - 9);

			SemanticType? dataSemanticType = null;

			foreach (SemanticType semanticType in semanticTypes)
			{
				if (semanticType.Transpose == isTranspose &&
					semanticType.Type == dataType &&
					semanticType.Mapping.Equals(semantic, StringComparison.InvariantCultureIgnoreCase))
				{
					dataSemanticType = semanticType;
					break;
				}
			}

			if (dataSemanticType == null)
			{
				//INVALID. EXTERMINATE.
				throw new CompileException(string.Format("Shader attribute '{0}' has unrecognised semantic '{1}'.", reg.Name, reg.Semantic));
			}

			//create the mapping...
			SemanticMapping mapping = new SemanticMapping();
			mapping.Register = reg;
			mapping.Type = dataSemanticType.Value;

			//figure out how often this semantic is used..
			List<CodeFieldReferenceExpression> changeRefs = new List<CodeFieldReferenceExpression>();

			foreach (KeyValuePair<AsmListing, CodeExpression> listing in listingRegisters)
			{
				Register sreg;
				RegisterSet registers = listing.Key.RegisterSet;
				CodeExpression registersRef = listing.Value;

				if (registers.TryGetRegister(reg.Name, out sreg))
				{
					string changeId = string.Format("sc{0}", semanticMappingRefCount++);
					changeRefs.Add(new CodeFieldReferenceExpression(shader.Instance, changeId));
				}
			}

			mapping.ChangeRefs = changeRefs.ToArray();

			this.semanticMapping.Add(mapping);
		}

		//extract pairs of asmlistings / code registers
		private void ComputeListings(IShaderDom shader)
		{
			List<KeyValuePair<AsmListing, CodeExpression>> listings = new List<KeyValuePair<AsmListing, CodeExpression>>();

			if (asm.VertexShader != null)
				listings.Add(new KeyValuePair<AsmListing,CodeExpression>(asm.VertexShader, shader.VertexShaderRegistersRef));
			
			if (asm.PixelShader != null)
				listings.Add(new KeyValuePair<AsmListing,CodeExpression>(asm.PixelShader, shader.PixelShaderRegistersRef));

			//preshaders

			if (asm.VertexPreShader != null)
				listings.Add(new KeyValuePair<AsmListing, CodeExpression>(asm.VertexPreShader, shader.VertexPreShaderRegistersRef));

			if (asm.PixelPreShader != null)
				listings.Add(new KeyValuePair<AsmListing, CodeExpression>(asm.PixelPreShader, shader.PixelPreShaderRegistersRef));

			
			this.listingRegisters = listings.ToArray();
		}

		//add the member fields
		public override void SetupMembers(IShaderDom shader)
		{
			ComputeListings(shader);

			ProcessConstants(shader);

			for (int i = 0; i < attributeNames.Count; i++)
			{
				//create a field ref for the static that will be created
				//these are used to assign the value using 'SetAttribute'
				CodeFieldReferenceExpression field = new CodeFieldReferenceExpression(shader.ShaderClassEx, string.Format("cid{0}",i));
				this.attributeFields.Add(field);

				//if it's an array, an array object needs to be created too. Create the ref to it now

				field = null;

				Register reg;
				Type type;//unused
				
				if (ExtractRegType(attributeNames[i], out reg, out type) && reg.ArraySize != -1)
					field = new CodeFieldReferenceExpression(shader.Instance, string.Format("ca{0}", i));

				attributeArrayFields.Add(field);//may be null.
			}
		}

		public override void AddMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
			//create static ID's for the registers
			if (platform != Platform.Both)
				return;

			//static id's for named attributes
			for (int i = 0; i < attributeNames.Count; i++)
			{
				//static id of this attribute name
				CodeMemberField field = new CodeMemberField(typeof(int), attributeFields[i].FieldName);
				field.Attributes = MemberAttributes.Static | MemberAttributes.Private | MemberAttributes.Final;

				add(field, string.Format("Name ID for '{0}'", attributeNames[i]));


				CreateConstantSetters(shader, add, attributeNames[i], attributeFields[i], attributeArrayFields[i]);
			}

			//add the semantic change IDs
			foreach (SemanticMapping mapping in semanticMapping)
			{
				for (int i = 0; i < mapping.ChangeRefs.Length; i++)
				{
					CodeMemberField field = new CodeMemberField(typeof(int), mapping.ChangeRefs[i].FieldName);
					field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

					add(field, i != 0 ? null : string.Format("Change ID for Semantic bound attribute '{0}'", mapping.Register.Name));
				}
			}

			//add the global refs and ids


			//add the global change IDs
			foreach (GlobalAttribute global in globals)
			{
				//global ID staics
				CodeMemberField field = new CodeMemberField(typeof(int), global.GlobalIdRef.FieldName);
				field.Attributes = MemberAttributes.Private | MemberAttributes.Final | MemberAttributes.Static;

				add(field, string.Format("TypeID for global attribute '{0}'", global.Register.Name));

				for (int i = 0; i < global.ChangeRefs.Length; i++)
				{
					field = new CodeMemberField(typeof(int), global.ChangeRefs[i].FieldName);
					field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

					add(field, i != 0 ? null : string.Format("Change ID for global attribute '{0} {1}'", global.Register.Name, global.Register.Name));
				}
			}
		}

		public override void AddReadonlyMembers(IShaderDom shader, Action<CodeTypeMember, string> add, Platform platform)
		{
			if (platform != Platform.Both)
				return;

			for (int i = 0; i < attributeNames.Count; i++)
			{
				//if it's an array, it needs an array object...

				Register reg;
				Type dataType;

				if (ExtractRegType(attributeNames[i], out reg, out dataType) && reg.ArraySize != -1)
				{
					//get the interface type for the field.
					Type interfaceType = typeof(Xen.Graphics.ShaderSystem.Constants.IArray<float>).GetGenericTypeDefinition();
					interfaceType = interfaceType.MakeGenericType(dataType);

					CodeMemberField field = new CodeMemberField(interfaceType, this.attributeArrayFields[i].FieldName);
					field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

					add(field, string.Format("Array wrapper for '{0} {1}[{2}]'", reg.Type, attributeNames[i], reg.ArraySize));
				}
			}

			//add the global array IDs (if there are any)
			foreach (GlobalAttribute global in globals)
			{
				for (int i = 0; i < global.ChangeRefs.Length; i++)
				{
					//if it's an array, also add the array objects
					if (global.Register.ArraySize != -1)
					{
						Type interfaceType = typeof(Xen.Graphics.ShaderSystem.Constants.IArray<float>).GetGenericTypeDefinition();
						interfaceType = interfaceType.MakeGenericType(global.Type);

						//arrays are stored differently.. eg as IArray<Matrix>
						CodeMemberField field = new CodeMemberField(interfaceType, global.ArrayRefs[i].FieldName);
						field.Attributes = MemberAttributes.Private | MemberAttributes.Final;

						add(field, i != 0 ? null : string.Format("Array access for global attribute '{0} {1}[{2}]'", global.Register.Type, global.Register.Name, global.Register.ArraySize));
					}
				}
			}
		}

		public override void AddConstructor(IShaderDom shader, Action<CodeStatement> add)
		{
			//set the semantic change IDs to -1
			foreach (SemanticMapping mapping in semanticMapping)
			{
				for (int i = 0; i < mapping.ChangeRefs.Length; i++)
				{
					CodeAssignStatement assign = new CodeAssignStatement(mapping.ChangeRefs[i], new CodePrimitiveExpression(-1));

					add(assign);
				}
			}

			//init the array attributes
			for (int i = 0; i < attributeNames.Count; i++)
			{
				//if it's an array, it needs an array object...

				Register reg;
				Type dataType;

				if (ExtractRegType(attributeNames[i], out reg, out dataType) && reg.ArraySize != -1)
				{
					//things get a bit funky here.
					//if the array is used by more than one register set, it must be wrapped up in a 'DualArray'
					//so a call to 'SetValue' on the array gets passed to both copies.
					//This can occur if both the vertex and pixel shader access a constant array, or a preshader, etc.
					Type dualType = typeof(Xen.Graphics.ShaderSystem.Constants.DualArray<float>).GetGenericTypeDefinition();
					dualType = dualType.MakeGenericType(dataType);

					//this.attributeArrayFields[i].FieldName

					CodeExpression initExpression = null;
					Type arrayType = GetArrayType(reg);

					foreach (KeyValuePair<AsmListing, CodeExpression> listing in listingRegisters)
					{
						Register sreg;
						RegisterSet registers = listing.Key.RegisterSet;
						CodeExpression registersRef = listing.Value;
						
						if (registers.TryGetRegister(reg.Name, out sreg))
						{
							CodeExpression create;

							create = new CodeObjectCreateExpression(arrayType,
								registersRef, //vreg
								new CodePrimitiveExpression(sreg.Index),
								new CodePrimitiveExpression(sreg.Size));

							if (initExpression == null)
							{
								initExpression = create;
							}
							else
							{
								//darn. wrap in a dual array.
								initExpression = new CodeObjectCreateExpression(
									dualType,
									create, initExpression);
							}
						}
					}

					CodeAssignStatement assign = new CodeAssignStatement(this.attributeArrayFields[i], initExpression);
					add(assign);
				}
			}

			//set the global change IDs to -1 (unless it's an array)...
			foreach (GlobalAttribute global in globals)
			{
				int changeRefIndex = 0;
				foreach (KeyValuePair<AsmListing, CodeExpression> listing in listingRegisters)
				{
					Register sreg;
					RegisterSet registers = listing.Key.RegisterSet;
					CodeExpression registersRef = listing.Value;

					if (registers.TryGetRegister(global.Register.Name, out sreg))
					{
						CodeAssignStatement assign = new CodeAssignStatement(global.ChangeRefs[changeRefIndex], new CodePrimitiveExpression(-1));

						add(assign);



						//arrays require the array wrapper to be initalised
						if (global.Register.ArraySize != -1)
						{
							//arrays are stored differently.. eg as IArray<Matrix>
							//eg:
							//new Xen.Graphics.ShaderSystem.Constants.Matrix4Array(this.vreg, 217, 4);

							Type arrayType = GetArrayType(global.Register);

							CodeExpression create;

							create = new CodeObjectCreateExpression(arrayType,
								registersRef, //vreg
								new CodePrimitiveExpression(sreg.Index),
								new CodePrimitiveExpression(sreg.Size));

							assign = new CodeAssignStatement(global.ArrayRefs[changeRefIndex], create);
							add(assign);
						}





						changeRefIndex++;
					}
				}
			}
		}

		private Type GetArrayType(Register register)
		{
			Type arrayType = null;
			switch (register.Rank)
			{
				case RegisterRank.FloatNx1:
					switch (register.Type)
					{
						case "float":
						case "float1": //?
							arrayType = typeof(Xen.Graphics.ShaderSystem.Constants.SingleArray);
							break;
						case "float2":
							arrayType = typeof(Xen.Graphics.ShaderSystem.Constants.Vector2Array);
							break;
						case "float3":
							arrayType = typeof(Xen.Graphics.ShaderSystem.Constants.Vector3Array);
							break;
						case "float4":
							arrayType = typeof(Xen.Graphics.ShaderSystem.Constants.Vector4Array);
							break;
					}
					break;
				case RegisterRank.FloatNx2:
					arrayType = typeof(Xen.Graphics.ShaderSystem.Constants.Matrix2Array);
					break;
				case RegisterRank.FloatNx3:
					arrayType = typeof(Xen.Graphics.ShaderSystem.Constants.Matrix3Array);
					break;
				case RegisterRank.FloatNx4:
					arrayType = typeof(Xen.Graphics.ShaderSystem.Constants.Matrix4Array);
					break;
			}

			if (arrayType == null)
				throw new CompileException(string.Format("Attribute '{0}' is invalid. Global arrays of type '{1}' are not supported.", register.Name, register.Type));
			return arrayType;
		}

		public override void AddBind(IShaderDom shader, Action<CodeStatement, string> add)
		{
			//bind the semantics bound attributes
			foreach (SemanticMapping mapping in semanticMapping)
			{
				//eg:
				//state.SetWorldMatrix(this.vreg.Matrix4Transpose(8), ref this.v_8);

				string method = string.Format("Set{0}{1}", mapping.Type.Mapping, mapping.Type.Type.Name);
				bool transpose = mapping.Type.Transpose ^ (mapping.Type.Type == typeof(Matrix));

				string registerTypeName = mapping.Type.Type.Name;
				if (mapping.Type.Type == typeof(Matrix))
					registerTypeName += (int)mapping.Register.Rank;
				if (transpose)
					registerTypeName += "Transpose";

				//for each register set, see if it uses this mapping

				int changeRefIndex = 0;
				foreach (KeyValuePair<AsmListing, CodeExpression> listing in listingRegisters)
				{
					Register sreg;
					RegisterSet registers = listing.Key.RegisterSet;
					CodeExpression registersRef = listing.Value;

					if (registers.TryGetRegister(mapping.Register.Name, out sreg))
					{
						//it does.. so the constants need setting..
						//state.SetWorldMatrix(this.vreg.Matrix4Transpose(8), ref this.v_8);

						CodeExpression changeRef = new CodeDirectionExpression(FieldDirection.Ref, mapping.ChangeRefs[changeRefIndex]);

						CodeExpression getRegister =	//this.vreg.Matrix4Transpose(8)
							new CodeMethodInvokeExpression(registersRef, registerTypeName, new CodePrimitiveExpression(sreg.Index));

						//invoke
						CodeExpression invokeSet =
							new CodeMethodInvokeExpression(shader.ShaderSystemRef, method, getRegister, changeRef);

						add(shader.ETS(invokeSet), changeRefIndex != 0 ? null : string.Format("Set the value for attribute '{0}'", mapping.Register.Name));

						changeRefIndex++;
					}
				}
			}

			//bind the shader globals

			foreach (GlobalAttribute global in globals)
			{
				string registerTypeName = global.Type.Name;
				if (global.Type == typeof(Matrix))
				{
					registerTypeName += (int)global.Register.Rank;
					registerTypeName += "Transpose";
				}
				
				int changeRefIndex = 0;
				foreach (KeyValuePair<AsmListing, CodeExpression> listing in listingRegisters)
				{
					Register sreg;
					RegisterSet registers = listing.Key.RegisterSet;
					CodeExpression registersRef = listing.Value;

					if (registers.TryGetRegister(global.Register.Name, out sreg))
					{
						//eg:
						//state.SetGlobal(this.vreg.Matrix4Transpose(8), ShadowShaderBlend.g_id0, ref this.g_0);

						CodeExpression getRegister =	//this.vreg.Matrix4Transpose(8)
							new CodeMethodInvokeExpression(registersRef, registerTypeName, new CodePrimitiveExpression(sreg.Index));

						CodeExpression changeParam = new CodeDirectionExpression(FieldDirection.Ref, global.ChangeRefs[changeRefIndex]);

						CodeExpression invokeSet;

						//logic changes for arrays

						if (global.Register.ArraySize != -1)
						{
							invokeSet =
								new CodeMethodInvokeExpression(shader.ShaderSystemRef, "SetGlobal", global.ArrayRefs[changeRefIndex], global.GlobalIdRef, changeParam);
							//state.SetGlobal(this.ga0, ShadowShaderBlend.g_id0, ref this.g_0);
						}
						else
						{
							//state.SetGlobal(this.vreg.Matrix4Transpose(8), ShadowShaderBlend.g_id0, ref this.g_0);
							invokeSet =
								new CodeMethodInvokeExpression(shader.ShaderSystemRef, "SetGlobal", getRegister, global.GlobalIdRef, changeParam);
						}

						add(shader.ETS(invokeSet), changeRefIndex != 0 ? null : string.Format("Set the value for global '{0}'", global.Register.Name));

						changeRefIndex++;
					}
				}
			}
		}

		public override void AddStaticGraphicsInit(IShaderDom shader, Action<CodeStatement, string> add)
		{
			//initalise the static ID's

			for (int i = 0; i < attributeNames.Count; i++)
			{
				//call state.GetNameUniqueID()

				CodeExpression call = new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(shader.ShaderSystemRef, "GetNameUniqueID"),
					new CodePrimitiveExpression(attributeNames[i]));

				CodeStatement assign = new CodeAssignStatement(attributeFields[i], call);

				add(assign, null);
			}

			//setup the TypeIDs for global attributes
			//eg:
			//ShadowShaderBlend.g_id0 = state.GetGlobalUniqueID<Microsoft.Xna.Framework.Matrix[]>("shadowMapProjection");

			foreach (GlobalAttribute global in globals)
			{
				//call state.GetGlobalUniqueID()

				CodeExpression call = new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(shader.ShaderSystemRef, "GetGlobalUniqueID", new CodeTypeReference(global.Type)),
					new CodePrimitiveExpression(global.Register.Name));

				CodeStatement assign = new CodeAssignStatement(global.GlobalIdRef, call);

				add(assign, null);
			}
		}


		private bool ExtractRegType(string name, out Register reg, out Type dataType)
		{
			bool hasSetMethod;
			int stride;
			return ExtractRegType(name, out reg, out dataType, out hasSetMethod, out stride);
		}
		private bool ExtractRegType(string name, out Register reg, out Type dataType, out bool hasSetMethod, out int stride)
		{
			dataType = null;
			hasSetMethod = true;
			stride = 1;
			reg = new Register();

			if (!this.asm.CommonRegisters.TryGetRegister(name, out reg))
				return false;

			if (reg.Category != RegisterCategory.Float4)
				return false;

			switch (reg.Type)
			{
				case "float":
				case "float1"://?
				case "int": // integers or bools may be processed as floats by the FX compiler
				case "int1":
				case "bool":
					dataType = typeof(float);
					hasSetMethod = false;
					break;
				case "float2":
				case "int2":
					dataType = typeof(Vector2);
					break;
				case "float3":
				case "int3":
					dataType = typeof(Vector3);
					break;
				case "float4":
				case "int4":
					dataType = typeof(Vector4);
					break;

				default:
					if (reg.Rank >= RegisterRank.IntNx1)
						return false;
					dataType = typeof(Matrix);
					stride = (int)reg.Rank;
					break;
			}
			return true;
		}

		private void CreateConstantSetters(IShaderDom shader, Action<CodeTypeMember, string> add, string name, CodeExpression assignmentField, CodeExpression assignmentArrayField)
		{
			/*
			 * Something like:

			public void SetInvTargetSize(ref Microsoft.Xna.Framework.Vector2 value)
			{
				this.vreg.SetVector2(130, ref value);
			}

			public Microsoft.Xna.Framework.Vector2 InvTargetSize
			{
				set
				{
					this.SetInvTargetSize(ref value);
				}
			}*/

			Register reg;
			Type dataType;
			bool hasSetMethod;
			int stride;

			if (!ExtractRegType(name, out reg, out dataType, out hasSetMethod, out stride))
				return;

			Type arrayOrSingleType = dataType;

			//right...

			//create the method of the given type.


			//public void SetInvTargetSize(ref Microsoft.Xna.Framework.Vector2 value)
			CodeStatementCollection methodStatements = new CodeStatementCollection();

			CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression(dataType, "value");
			if (reg.ArraySize == -1)
				param.Direction = FieldDirection.Ref;
			else
			{
				arrayOrSingleType = dataType.MakeArrayType();
				param.Type = new CodeTypeReference(arrayOrSingleType);
			}

			CodeExpression valueRef = new CodeArgumentReferenceExpression(param.Name);

			//when there isn't a set method, there is just a set property
			if (!hasSetMethod)
				valueRef = new CodePropertySetValueReferenceExpression();

			//create the guts
			//depends on what constants use it...

			//eg:
			//this.vreg.SetVector2(130, ref value);
		
			string targetName = "Set" + dataType.Name;

			if (dataType == typeof(Matrix))
			{
				targetName += stride;
				targetName += "Transpose";
			}

			Register sreg;

			if (reg.ArraySize == -1)
			{
				//not an array..

				foreach (KeyValuePair<AsmListing, CodeExpression> listing in listingRegisters)
				{
					RegisterSet registers = listing.Key.RegisterSet;
					CodeExpression registersRef = listing.Value;

					if (registers.TryGetRegister(name, out sreg))
					{
						//set the regiser (eg, may be vertex shader register)
						CodeExpression methodInvoke =
							new CodeMethodInvokeExpression(registersRef, targetName,
								new CodePrimitiveExpression(sreg.Index),
								new CodeDirectionExpression(FieldDirection.Ref, valueRef));

						methodStatements.Add(shader.ETS(methodInvoke));
					}
				}
			}
			else
			{
				//is an array...
				//simply call SetArray on the array object.
				CodeExpression methodInvoke =
					new CodeMethodInvokeExpression(assignmentArrayField, "SetArray",
						valueRef);

				methodStatements.Add(shader.ETS(methodInvoke));
			}
			

			string upperName = Common.ToUpper(name);

			//there is always a setable property
			CodeMemberProperty property = new CodeMemberProperty();
			property.Name = upperName;
			property.Type = param.Type;
			property.Attributes = MemberAttributes.Final | MemberAttributes.Public;
			property.HasSet = reg.ArraySize == -1;
			property.HasGet = reg.ArraySize != -1;

			//there isn't always a set method
			CodeMemberMethod method = null;

			CodeStatement assignAttribute = null;

			if (hasSetMethod || reg.ArraySize != -1)
			{
				//create the method to set the value
				string methodName = "Set" + upperName;

				method = new CodeMemberMethod();
				method.Name = methodName;
				method.Attributes = MemberAttributes.Final | MemberAttributes.Public;

				method.Parameters.Add(param);
				method.Statements.AddRange(methodStatements);


				//create a property that calls the Set method if not an array, get method if it is an array

				if (reg.ArraySize == -1)
				{
					//is not an array
					CodeExpression invokeSetter =
						new CodeMethodInvokeExpression(
							shader.Instance, method.Name,
							new CodeDirectionExpression(FieldDirection.Ref, new CodePropertySetValueReferenceExpression()));

					property.SetStatements.Add(invokeSetter);
				}
				else
				{
					//is an array, return the array object directly.
					property.GetStatements.Add(new CodeMethodReturnStatement(assignmentArrayField));

					//set the type of the property to IArray<float>, etc
					Type interfaceType = typeof(Xen.Graphics.ShaderSystem.Constants.IArray<float>).GetGenericTypeDefinition();
					interfaceType = interfaceType.MakeGenericType(dataType);

					property.Type = new CodeTypeReference(interfaceType);
				}



				//call the method as well for attribute assign
				CodeExpression assignSetter = 
					new CodeMethodInvokeExpression(
						shader.Instance, method.Name,
						new CodeDirectionExpression(param.Direction, shader.AttributeAssignValue));

				assignAttribute = shader.ETS(assignSetter);
			}
			else
			{
				//create a property to directly set the value

				property.SetStatements.AddRange(methodStatements);

				//attribute assign sets the property
				assignAttribute = new CodeAssignStatement(
					new CodePropertyReferenceExpression(shader.Instance, property.Name),
						shader.AttributeAssignValue);
			}
			

			if (reg.ArraySize > 0)
			{
				if (method != null)
					add(method, string.Format("Set the shader array value '{0} {1}[{2}]'", reg.Type, reg.Name, reg.ArraySize));
				add(property, string.Format("Get the array for the shader value '{0} {1}[{2}]'", reg.Type, reg.Name, reg.ArraySize));
			}
			else
			{
				if (method != null)
					add(method, string.Format("Set the shader value '{0} {1}'", reg.Type, reg.Name));
				add(property, string.Format("Assign the shader value '{0} {1}'", reg.Type, reg.Name));
			}

			//create the attribute assignment value statement.

			List<CodeStatement> assignList;
			if (!attributeAssignment.TryGetValue(arrayOrSingleType, out assignList))
			{
				assignList = new List<CodeStatement>();
				attributeAssignment.Add(arrayOrSingleType, assignList);
			}

			//create the statement...
			
			CodeExpression assignIdsMatch =
				new CodeBinaryOperatorExpression(shader.AttributeAssignId,  CodeBinaryOperatorType.IdentityEquality, assignmentField);

			CodeConditionStatement performAssign =
				new CodeConditionStatement(assignIdsMatch,
					assignAttribute, //call the assignment code
					new CodeMethodReturnStatement(new CodePrimitiveExpression(true))); //return true, set correctly.

			assignList.Add(performAssign);
		}

		public override void AddSetAttribute(IShaderDom shader, Action<CodeStatement> add, Type type)
		{

			List<CodeStatement> statements;

			if (attributeAssignment.TryGetValue(type, out statements))
			{
				foreach (CodeStatement statement in statements)
				{
					add(statement);
				}
			}
		}
	}
}
