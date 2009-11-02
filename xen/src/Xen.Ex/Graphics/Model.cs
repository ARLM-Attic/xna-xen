using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Xen.Graphics;
using Xen.Ex.Graphics.Content;
using Xen.Ex.Material;
using Xen.Ex.Compression;
using Xen.Threading;

namespace Xen.Ex.Graphics
{
	/// <summary>
	/// <para>Implement this abstract class to override shaders used by a <see cref="ModelInstance"/>.</para>
	/// <para>Return false in the <see cref="BeginGeometryShaderOverride"/> method if the model instance should use the default lighting shader</para>
	/// </summary>
	public abstract class ModelInstanceShaderProvider
	{
		/// <summary>
		/// <para>Called before drawing geometry</para>
		/// <para>Return true if the shader has been overridden</para>
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="lights"></param>
		/// <returns></returns>
		/// <remarks>If pushing the world matrix, make sure to pop it in <see cref="EndGeometry"/></remarks>
		/// <param name="state"></param>
		public virtual bool BeginGeometryShaderOverride(DrawState state, GeometryData geometry, MaterialLightCollection lights) { return false; }
		/// <summary>
		/// Called after drawing geometry
		/// </summary>
		/// <param name="state"></param>
		/// <param name="geometry"></param>
		public virtual void EndGeometry(DrawState state, GeometryData geometry) { }
		/// <summary>
		/// This method is called when animation transforms have been calculated, before drawing beings
		/// </summary>
		/// <param name="animationBoneHierarchy"></param>
		/// <param name="animationBoneHierarchyMatrix4x3">An array of Vector4 values storing the transform hierarchy in float4x3 format</param>
		/// <param name="state"></param>
		/// <remarks>If pushing the world matrix, make sure to pop it in <see cref="EndDraw"/>, and return true in <see cref="ProviderModifiesWorldMatrixInBeginDraw"/></remarks>
		public virtual void BeginDraw(DrawState state, Transform[] animationBoneHierarchy, Vector4[] animationBoneHierarchyMatrix4x3) { }
		/// <summary>
		/// Called before drawing the model instance begins, and no animation is active
		/// </summary>
		/// <param name="state"></param>
		public virtual void BeginDraw(DrawState state) { }
		/// <summary>
		/// Called after drawing the model
		/// </summary>
		/// <param name="state"></param>
		public virtual void EndDraw(DrawState state) { }
		/// <summary>
		/// Called before drawing a mesh
		/// </summary>
		/// <param name="mesh"></param>
		/// <remarks>If pushing the world matrix, make sure to pop it in <see cref="EndMesh"/></remarks>
		/// <param name="state"></param>
		public virtual void BeginMesh(DrawState state, MeshData mesh) { }
		/// <summary>
		/// Called after drawing a mesh
		/// </summary>
		/// <param name="state"></param>
		/// <param name="mesh"></param>
		public virtual void EndMesh(DrawState state, MeshData mesh) { }

		/// <summary>
		/// <para>Returns true if this provider modifies the world matrix in the <see cref="BeginDraw(DrawState)"/> method</para>
		/// <para>If true, the default CullTest will be skipped until BeginDraw() has been called</para>
		/// </summary>
		/// <remarks>Note: This property may be called frequenty, so should not run complex logic - it should simply return either true/false</remarks>
		public abstract bool ProviderModifiesWorldMatrixInBeginDraw { get;}
	}

	/// <summary>
	/// A generic implementation of ModelInstanceShaderProvider, storing an instance of a shader
	/// </summary>
	public class ModelInstanceShaderProvider<T> : ModelInstanceShaderProvider where T : IShader
	{
		private readonly T shader;

		/// <summary>
		/// Construct the generic ModelInstanceShaderProvider
		/// </summary>
		/// <param name="shader"></param>
		public ModelInstanceShaderProvider(T shader)
		{
			if (shader == null)
				throw new ArgumentNullException();
			this.shader = shader;
		}

		/// <summary>
		/// Gets the shader for this generic ModelInstanceShaderProvider
		/// </summary>
		public T Shader
		{
			get { return shader; }
		}
		/// <summary>
		/// <para>Returns true if this provider modifies the world matrix in the <see cref="ModelInstanceShaderProvider.BeginDraw(DrawState)"/> method</para>
		/// <para>If true, the default CullTest will be skipped until BeginDraw() has been called</para>
		/// </summary>
		/// <remarks>Note: This property may be called frequenty, so should not run complex logic - it should simply return either true/false</remarks>
		public override bool ProviderModifiesWorldMatrixInBeginDraw
		{
			get { return false; }
		}
		/// <summary>
		/// <para>Called before drawing geometry</para>
		/// <para>Return true if the shader has been overridden</para>
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="lights"></param>
		/// <returns></returns>
		/// <remarks>If pushing the world matrix, make sure to pop it in <see cref="ModelInstanceShaderProvider.EndGeometry"/></remarks>
		/// <param name="state"></param>
		public override bool BeginGeometryShaderOverride(DrawState state, GeometryData geometry, MaterialLightCollection lights)
		{
			if (typeof(T) == typeof(MaterialShader))
				((MaterialShader)(IShader)shader).Lights = lights;

			shader.Bind(state);
			return true;
		}
	}

	/// <summary>
	/// Draws <see cref="ModelData"/> loaded through the content pipeline
	/// </summary>
	/// <remarks>
	/// <para>
	/// When implementing shaders to draw the model, the following shader code example shows how to get the blend matrix when animation is enabled:
	/// </para>
	/// <para>
	/// <example>
	/// <code>
	/// //note: blendMatrices format is Matrix4x3 stored in Vector4s
	/// //in this example, there would be a maximum of 72 bones
	/// float4 blendMatrices[72*3];
	/// float4x4 worldViewProj : WORLDVIEWPROJECTION;
	/// 
	/// //This shader will be approximately 20 instructions
	/// 
	///	void vertexShader(
	///			float4	pos			: POSITION,
	///		out	float4	o_pos		: POSITION,
	///			float4	weights		: BLENDWEIGHT,
	///			int4	indices		: BLENDINDICES)
	///	{
	///		//transpose makes this easier to write (it gets optimised away by the shader compiler)
	///		float4x3 blendMat = transpose(float3x4(
	///						blendMatrices[indices.x*3+0] * weights.x + blendMatrices[indices.y*3+0] * weights.y + blendMatrices[indices.z*3+0] * weights.z + blendMatrices[indices.w*3+0] * weights.w,
	///						blendMatrices[indices.x*3+1] * weights.x + blendMatrices[indices.y*3+1] * weights.y + blendMatrices[indices.z*3+1] * weights.z + blendMatrices[indices.w*3+1] * weights.w,
	///						blendMatrices[indices.x*3+2] * weights.x + blendMatrices[indices.y*3+2] * weights.y + blendMatrices[indices.z*3+2] * weights.z + blendMatrices[indices.w*3+2] * weights.w
	///					   ));
	///
	///		float3 blendedPosition = mul(pos,blendMat).xyz;
	///		
	///		o_pos = mul(float4(blendedPosition,1),worldViewProj);
	///	}
	/// </code>
	/// </example>
	/// </para>
	/// </remarks>
	public sealed class ModelInstance : IDraw, ICullableInstance
	{
		private ModelData modelData;
		private AnimationController controller;
		private MaterialLightCollection lights;
		private MaterialAnimationTransformHierarchy hierarchy;
		private ModelInstanceShaderProvider shaderProvider;

		/// <summary>
		/// Construct the model instance. Setting the <see cref="ModelData"/> content is required before drawing
		/// </summary>
		public ModelInstance()
		{
		}

		/// <summary>
		/// Construct the model instance with existing model data
		/// </summary>
		/// <param name="sourceData"></param>
		public ModelInstance(ModelData sourceData)
		{
			this.modelData = sourceData;
		}

		#region draw flags

		/// <summary>
		/// <para>A structure that can be used as a Draw Flag to force drawn model instances to use a specific Shader Provider</para>
		/// </summary>
		public struct ShaderProviderFlag
		{
			private bool overrideShaderProvider;
			private ModelInstanceShaderProvider shaderProvider;

			/// <summary>
			/// <para>Force drawn model instances to use <see cref="ShaderProvider"/></para>
			/// </summary>
			public bool OverrideShaderProvider { get { return overrideShaderProvider; } set { overrideShaderProvider = value; } }
			/// <summary></summary>
			public ModelInstanceShaderProvider ShaderProvider
			{
				get { return shaderProvider; }
				set
				{
					if (value != null && value.ProviderModifiesWorldMatrixInBeginDraw)
						throw new ArgumentException("ShaderProvider.ProviderModifiesWorldMatrixInBeginDraw cannot be true for ShaderProviderFlag");
					shaderProvider = value;
				}
			}

			/// <summary></summary>
			/// <param name="shaderProvider"></param>
			public ShaderProviderFlag(ModelInstanceShaderProvider shaderProvider)
			{
				this.overrideShaderProvider = true;
				if (shaderProvider != null && shaderProvider.ProviderModifiesWorldMatrixInBeginDraw)
					throw new ArgumentException("shaderProvider.ProviderModifiesWorldMatrixInBeginDraw cannot be true for ShaderProviderFlag");
				this.shaderProvider = shaderProvider;
			}
		}

		#endregion

		/// <summary>
		/// Extend the <see cref="ModelInstanceShaderProvider"/> abstract class to override shaders or world matrices used by this model instance
		/// </summary>
		/// <param name="provider"></param>
		public void SetShaderOverride(ModelInstanceShaderProvider provider)
		{
			shaderProvider = provider;
		}

		/// <summary>
		/// <see cref="ModelData"/> used by this model instance. ModelData content must be assigned before the instance is drawn
		/// </summary>
		/// <remarks>ModelData may only be assigned once per instance</remarks>
		public ModelData ModelData
		{
			get { return modelData; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				if (this.modelData != null && this.modelData != value)
					throw new InvalidOperationException("ModelData may only be assigned once");
				this.modelData = value;
				if (controller != null)
					this.controller.SetModelData(value);
			}
		}

		/// <summary>
		/// Gets/Sets the lights collection used by any material shaders loaded with the model
		/// </summary>
		public MaterialLightCollection LightCollection
		{
			get { return lights; }
			set { lights = value; }
		}

		/// <summary>
		/// Gets/Creates an animation controller for this mesh instance
		/// </summary>
		/// <returns></returns>
		public AnimationController GetAnimationController()
		{
			if (modelData != null && modelData.skeleton == null)
				throw new InvalidOperationException("ModelData has no skeleton");
			if (controller == null)
				controller = new AnimationController(this.modelData, null, this);
			return controller;
		}

		/// <summary>
		/// <para>Gets/Creates an animation controller that runs as a thread task</para>
		/// <para>Async animations require adding to an <see cref="UpdateManager"/> because their processing is initalised at the end of the update loop</para>
		/// </summary>
		/// <param name="manager"></param>
		/// <returns></returns>
		public AnimationController GetAsyncAnimationController(UpdateManager manager)
		{
			if (manager == null)
				throw new ArgumentNullException();
			if (modelData != null && modelData.skeleton == null)
				throw new InvalidOperationException("ModelData has no skeleton");
			if (controller == null)
				controller = new AnimationController(this.modelData, manager, this);
			return controller;
		}

		/// <summary>
		/// Share an existing animation controller with this model
		/// </summary>
		/// <param name="controller"></param>
		public void SetSharedAnimationController(AnimationController controller)
		{
			if (controller == null)
				throw new ArgumentNullException();
			if (this.controller == controller)
				return;
			if (this.controller != null)
				throw new InvalidOperationException("AnimationController already set");
			if (controller.ModelData != this.modelData || (this.modelData == null && controller.ModelData == null))
				throw new ArgumentException("ModelData mismatch");
			controller.AddParnet(this);
			this.controller = controller;
		}

		/// <summary>
		/// Draw the model. This class automatically assigns shaders when drawing
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			if (modelData == null)
				throw new InvalidOperationException("ModelData is null");
			
			if (controller != null)
			{
				controller.WaitForAsyncAnimation(state,state.FrameIndex,true);

				if (controller.IsDisposed)
					controller = null;
			}

			if (controller != null && hierarchy == null)
				hierarchy = new MaterialAnimationTransformHierarchy(modelData.skeleton);

			if (hierarchy != null)
				hierarchy.UpdateTransformHierarchy(controller.transformedBones);

			ModelInstanceShaderProvider shaderProvider = this.shaderProvider;
			MaterialLightCollection lights = this.lights;

			ShaderProviderFlag providerFlag;
			MaterialLightCollection.LightCollectionFlag lightsFlag;

			state.GetDrawFlag(out providerFlag);
			if (providerFlag.OverrideShaderProvider)
				shaderProvider = providerFlag.ShaderProvider;

			state.GetDrawFlag(out lightsFlag);
			if (lightsFlag.OverrideLightCollection)
				lights = lightsFlag.LightCollection;

			if (shaderProvider != null)
			{
				if (controller != null)
					shaderProvider.BeginDraw(state,controller.transformedBones, hierarchy.GetMatrixData());
				else
					shaderProvider.BeginDraw(state);
			}

			Vector3 boundsMin, boundsMax;

			ContainmentType cullModel = ContainmentType.Contains;

			//if there is just one geometry object, then the ICullable.CullTest() call will have been suficient.
			bool skipCullTest = this.modelData != null && this.modelData.meshes.Length == 1 && this.modelData.meshes[0].geometry.Length == 1;

			if (!skipCullTest)
			{
				if (controller != null)
					cullModel = state.Culler.IntersectBox(ref controller.boundsMin, ref controller.boundsMax);
				else
					cullModel = state.Culler.IntersectBox(ref modelData.staticBounds.minimum, ref modelData.staticBounds.maximum);
			}

			if (cullModel != ContainmentType.Disjoint)
			{
				for (int m = 0; m < modelData.meshes.Length; m++)
				{
					MeshData mesh = modelData.meshes[m];

					if (shaderProvider != null)
						shaderProvider.BeginMesh(state, mesh);

					ContainmentType cullMesh = cullModel;

					if (cullModel == ContainmentType.Intersects && modelData.meshes.Length > 1)
					{
						if (controller != null)
						{
							controller.ComputeMeshBounds(m, out boundsMin, out boundsMax);
							cullMesh = state.Culler.IntersectBox(ref boundsMin, ref boundsMax);
						}
						else
							cullMesh = state.Culler.IntersectBox(ref mesh.staticBounds.minimum, ref mesh.staticBounds.maximum);

					}

					if (cullMesh != ContainmentType.Disjoint)
					{
						for (int g = 0; g < mesh.geometry.Length; g++)
						{
							GeometryData geom = mesh.geometry[g];
							MaterialShader shader = geom.MaterialShader;

							if (shaderProvider != null && shaderProvider.BeginGeometryShaderOverride(state, geom, lights))
								shader = null;

							bool cullTest = true;

							if (cullMesh == ContainmentType.Intersects && mesh.geometry.Length > 1)
							{
								if (controller != null)
								{
									controller.ComputeGeometryBounds(m, g, out boundsMin, out boundsMax);
									cullTest = state.Culler.TestBox(ref boundsMin, ref boundsMax);
								}
								else
									cullTest = state.Culler.TestBox(ref geom.staticBounds.minimum, ref geom.staticBounds.maximum);
							}

							if (cullTest)
							{
								if (shader != null)
								{
									shader.AnimationTransforms = hierarchy;
									shader.Lights = lights;
									shader.Bind(state);
								}

								geom.Vertices.Draw(state, geom.Indices, PrimitiveType.TriangleList);
							}

							if (shaderProvider != null)
								shaderProvider.EndGeometry(state, geom);
						}
					}

					if (shaderProvider != null)
						shaderProvider.EndMesh(state, mesh);
				}
			}

			if (shaderProvider != null)
				shaderProvider.EndDraw(state);
		}

		/// <summary>
		/// FrustumCull test the model
		/// </summary>
		/// <param name="culler"></param>
		/// <returns></returns>
		public bool CullTest(ICuller culler)
		{
			if (modelData == null)
				throw new InvalidOperationException("ModelData is null");

			if (shaderProvider != null && shaderProvider.ProviderModifiesWorldMatrixInBeginDraw)
				return true;

			if (controller == null)
				return culler.TestBox(ref modelData.staticBounds.minimum, ref modelData.staticBounds.maximum);
			else
			{
				controller.WaitForAsyncAnimation(culler.GetState(), culler.FrameIndex,false);
				return culler.TestBox(ref controller.boundsMin, ref controller.boundsMax);
			}
		}


		/// <summary>
		/// FrustumCull test the model at the given location
		/// </summary>
		public bool CullTest(ICuller culler, ref Matrix instance)
		{
			if (modelData == null)
				return false;

			if (shaderProvider != null && shaderProvider.ProviderModifiesWorldMatrixInBeginDraw)
				return true;

			if (controller == null)
				return culler.TestBox(ref modelData.staticBounds.minimum, ref modelData.staticBounds.maximum, ref instance);
			else
			{
				controller.WaitForAsyncAnimation(culler.GetState(), culler.FrameIndex, false);
				return culler.TestBox(ref controller.boundsMin, ref controller.boundsMax, ref instance);
			}
		}
	}
}
