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
	/// <para>Draws <see cref="BatchModelInstance"/> objects in a batch, using <see cref="ModelData"/> loaded through the content pipeline</para>
	/// <para>Note: BatchModel does not support model animation</para>
	/// </summary>
	public sealed class BatchModel : IDraw
	{
		class GeometrySet
		{
			public Matrix[] instances;
			public int count;
		}

		private ModelData modelData;
		private int childCount;
		private int drawCount;
		private GeometrySet[] geometry;
		private BatchModelShaderProvider shaderProvider;
		private MaterialLightCollection lights;

		/// <summary>
		/// <para>A structure that can be used as a Draw Flag to force drawn batch model to use a specific Shader Provider</para>
		/// </summary>
		public struct ShaderProviderFlag
		{
			private bool overrideShaderProvider;
			private BatchModelShaderProvider shaderProvider;

			/// <summary>
			/// <para>Force drawn batch model to use <see cref="ShaderProvider"/></para>
			/// </summary>
			public bool OverrideShaderProvider { get { return overrideShaderProvider; } set { overrideShaderProvider = value; } }
			/// <summary></summary>
			public BatchModelShaderProvider ShaderProvider { get { return shaderProvider; } set { shaderProvider = value; } }

			/// <summary></summary>
			/// <param name="shaderProvider"></param>
			public ShaderProviderFlag(BatchModelShaderProvider shaderProvider)
			{
				this.overrideShaderProvider = true;
				this.shaderProvider = shaderProvider;
			}
		}

		/// <summary>
		/// Extend the <see cref="BatchModelShaderProvider"/> abstract class to override shaders used by this batch model
		/// </summary>
		/// <param name="provider"></param>
		public void SetShaderOverride(BatchModelShaderProvider provider)
		{
			shaderProvider = provider;
		}

		/// <summary>
		/// <see cref="ModelData"/> used by this batch model. ModelData content must be assigned before the batch or an instance is drawn
		/// </summary>
		/// <remarks>ModelData may only be assigned once per batch instance</remarks>
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


		internal void CountChild()
		{
			childCount++;
		}

		//the child isn't drawn right now, but for every bit of geometry that is visible, the world matrix is stored
		internal void DrawChild(DrawState state)
		{
			if (modelData == null)
				throw new InvalidOperationException("ModelData is null");

			if (geometry == null)
				SetupGeometry();

			ContainmentType cullModel = state.Culler.IntersectBox(ref modelData.staticBounds.minimum, ref modelData.staticBounds.maximum);

			int geometryIndex = 0;
			bool drawn = false;

			//loop through the model data
			if (cullModel != ContainmentType.Disjoint)
			{
				for (int m = 0; m < modelData.meshes.Length; m++)
				{
					MeshData mesh = modelData.meshes[m];

					ContainmentType cullMesh = cullModel;

					//cull testing along the way
					if (cullModel == ContainmentType.Intersects && modelData.meshes.Length > 1)
						cullMesh = state.Culler.IntersectBox(ref mesh.staticBounds.minimum, ref mesh.staticBounds.maximum);

					if (cullMesh != ContainmentType.Disjoint)
					{
						for (int g = 0; g < mesh.geometry.Length; g++)
						{
							GeometryData geom = mesh.geometry[g];

							bool cullTest = true;

							if (cullMesh == ContainmentType.Intersects && mesh.geometry.Length > 1)
								cullTest = state.Culler.TestBox(ref geom.staticBounds.minimum, ref geom.staticBounds.maximum);

							//finally, is the geometry visible?
							if (cullTest)
							{
								//add the world matrix to the geometry set
								GeometrySet set = this.geometry[geometryIndex];

								if (set.count == set.instances.Length)
									Array.Resize(ref set.instances,set.instances.Length*2);

								state.GetWorldMatrix(out set.instances[set.count++]);

								drawn = true;
							}

							geometryIndex++;
						}
					}
					else
						geometryIndex += mesh.geometry.Length;
				}
			}
			if (drawn)
				drawCount++;
		}

		internal bool CullChild(ICuller culler)
		{
			if (modelData == null)
				throw new InvalidOperationException("ModelData is null");
			return culler.TestBox(ref modelData.staticBounds.minimum, ref modelData.staticBounds.maximum);
		}
		internal bool CullChild(ICuller culler, ref Matrix instance)
		{
			if (modelData == null)
				throw new InvalidOperationException("ModelData is null");
			return culler.TestBox(ref modelData.staticBounds.minimum, ref modelData.staticBounds.maximum, ref instance);
		}

		/// <summary>
		/// Draw all the model batch instances
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			if (modelData == null)
				throw new InvalidOperationException("ModelData is null");

			if (geometry == null)
				SetupGeometry();

			int geometryIndex = 0;

			BatchModelShaderProvider shaderProvider = this.shaderProvider;
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
				shaderProvider.BeginDraw(state);

			//loop through the model data
			for (int m = 0; m < modelData.meshes.Length; m++)
			{
				MeshData mesh = modelData.meshes[m];

				if (shaderProvider != null)
					shaderProvider.BeginMesh(state, mesh);

				for (int g = 0; g < mesh.geometry.Length; g++)
				{
					GeometryData geom = mesh.geometry[g];
					GeometrySet set = this.geometry[geometryIndex];

					if (set.count > 0)
					{
						bool instancing = state.SupportsHardwareInstancing && set.count > 2;

						if (shaderProvider == null || !shaderProvider.BeginGeometryShaderOverride(state, geom, lights, instancing))
						{
							MaterialShader shader = geom.MaterialShader;
							
							shader.AnimationTransforms = null;
							shader.UseHardwareInstancing = instancing;
							shader.Lights = lights;

							shader.Bind(state);
						}

						//draw the geometry
						if (instancing)
						{
							state.DrawBatch(geom.Vertices, geom.Indices, PrimitiveType.TriangleList, null, set.instances, set.count);
						}
						else
						{
							for (int i = 0; i < set.count; i++)
							{
								state.PushWorldMatrixMultiply(ref set.instances[i]);

								geom.Vertices.Draw(state, geom.Indices, PrimitiveType.TriangleList);

								state.PopWorldMatrix();
							}
						}

						if (shaderProvider != null)
							shaderProvider.EndGeometry(state, geom);
					}


					set.count = 0;
					geometryIndex++;
				}

				if (shaderProvider != null)
					shaderProvider.EndMesh(state, mesh);
			}

			if (shaderProvider != null)
				shaderProvider.EndDraw(state);

			drawCount = 0;
		}

		bool ICullable.CullTest(ICuller culler)
		{
			return drawCount > 0;
		}

		private void SetupGeometry()
		{
			//count up the total number of geometry objects in the modelData

			int count = 0;
			foreach (MeshData mesh in modelData.Meshes)
				count += mesh.Geometry.Length;

			//allocate a GeometrySet for each

			this.geometry = new GeometrySet[count];
			
			//fill each geometry set with how many children there are...

			count = Math.Max(2,this.childCount);
			for (int i = 0; i < this.geometry.Length; i++)
			{
				this.geometry[i] = new GeometrySet();
				this.geometry[i].instances = new Matrix[count];
			}
		}
	}

	/// <summary>
	/// <para>Represents an instance of a model, drawn with a <see cref="BatchModel"/></para>
	/// </summary>
	public sealed class BatchModelInstance : IDraw, ICullableInstance
	{
		private readonly BatchModel parent;

		/// <summary>
		/// Construct the batch model instance
		/// </summary>
		/// <param name="model">The BatchModel that will draw this instance</param>
		public BatchModelInstance(BatchModel model)
		{
			if (model == null)
				throw new ArgumentNullException();
			this.parent = model;
			this.parent.CountChild();
		}

		/// <summary>
		/// Draw this instance of the BatchModel
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			parent.DrawChild(state);
		}

		/// <summary>
		/// Cull test this instance of the BatchModel
		/// </summary>
		/// <param name="culler"></param>
		/// <returns></returns>
		public bool CullTest(ICuller culler)
		{
			return parent.CullChild(culler);
		}

		/// <summary>
		/// Cull test this instance of the BatchModel
		/// </summary>
		public bool CullTest(ICuller culler, ref Matrix instance)
		{
			return parent.CullChild(culler, ref instance);
		}
	}


	/// <summary>
	/// <para>Implement this abstract class to override shaders used by a <see cref="BatchModel"/>.</para>
	/// <para>Return false in the <see cref="BeginGeometryShaderOverride"/> method if the batch model should use the default lighting shader</para>
	/// </summary>
	public abstract class BatchModelShaderProvider
	{
		/// <summary>
		/// <para>Called before drawing geometry. The shader bound must implement instancing if <paramref name="instancingEnabled"/> is true.</para>
		/// <para>Return true if the shader has been overridden</para>
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="instancingEnabled"></param>
		/// <param name="lights"></param>
		/// <returns></returns>
		/// <param name="state"></param>
		public virtual bool BeginGeometryShaderOverride(DrawState state, GeometryData geometry, MaterialLightCollection lights, bool instancingEnabled) { return false; }
		/// <summary>
		/// Called after drawing geometry
		/// </summary>
		/// <param name="state"></param>
		/// <param name="geometry"></param>
		public virtual void EndGeometry(DrawState state, GeometryData geometry) { }
		/// <summary>
		/// This method is called before drawing beings
		/// </summary>
		/// <param name="state"></param>
		/// <remarks></remarks>
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
		/// <param name="state"></param>
		public virtual void BeginMesh(DrawState state, MeshData mesh) { }
		/// <summary>
		/// Called after drawing a mesh
		/// </summary>
		/// <param name="state"></param>
		/// <param name="mesh"></param>
		public virtual void EndMesh(DrawState state, MeshData mesh) { }
	}

}
