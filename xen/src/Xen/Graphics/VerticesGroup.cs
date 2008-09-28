using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Xen.Graphics
{
	/// <summary>
	/// Sets parametres for hardware instancing when drawing with a VerticesGroup object (Supported on windows with vertex shader 3 or greater)
	/// </summary>
	/// <remarks>
	/// <para>Hardware instancing is not currently supported on the xbox 360, only PCs with vertex shader model 3.0 are supported.</para>
	/// <para>Hardware instancing uses two or more vertex buffers, with each buffer being repeated.</para>
	/// <para>Common usage, would be to have the first buffer store the model geometry, repeated X number of times.
	/// With the second buffer storing per-instance data, such as the world matrix.
	/// Instance data is repeated for each vertex. A 5 vertex mesh drawn 3 times will repeat the instance (world matrix) data AAAAABBBBBCCCCC, where the vertices are repeated ABCDEABCDEABCDE.</para>
	/// <para>The DrawBatch methods in <see cref="DrawState"/> are an easy way to automatically setup the instance data buffer in an optimal way.</para>
	/// <para></para>
	/// <para>Supporting instancing in a shader is fairly simple:</para>
	/// <para>Instead of using the WORLDVIEWPROJECTION matrix to transform your vertices,
	/// use the VIEWPROJECTION matrix multipled by the world matrix from instance data:</para>
	/// <para></para>
	/// <para>Non instancing example:</para>
	/// <code>
	/// float4x4 worldViewProjection : WORLDVIEWPROJECTION;
	/// 
	/// void SimpleVS(in float4 position : POSITION, out float4 out_position : POSITION)
	/// {
	///		out_position = mul(pos,worldViewProjection);
	/// }
	/// </code>
	/// <para>Instancing example:</para>
	/// <code>
	/// float4x4 viewProjection : VIEWPROJECTION;
	/// 
	/// void SimpleInstancedVS(	in  float4 position		: POSITION, 
	///							out float4 out_position : POSITION,
	///							in  float4 worldX		: POSITION12,
	/// 						in  float4 worldY		: POSITION13,
	/// 						in  float4 worldZ		: POSITION14,
	/// 						in  float4 worldW		: POSITION15)
	/// {
	///		float4x4 world = float4x4(worldX,worldY,worldZ,worldW);
	/// 
	///		out_position = mul(mul(pos,world),viewProjection);
	/// }
	/// </code>
	/// <para>This can also be done with a simple structure:</para>
	/// <code>
	/// struct __InstanceWorldMatirx
	/// {
	/// 	float4 position12 : POSITION12;
	/// 	float4 position13 : POSITION13;
	/// 	float4 position14 : POSITION14;
	/// 	float4 position15 : POSITION15;
	/// };
	/// 
	/// float4x4 viewProjection : VIEWPROJECTION;
	/// 
	/// void SimpleInstancedVS(	in  float4 position		: POSITION, 
	///							out float4 out_position : POSITION,
	///							in  __InstanceWorldMatirx world)
	/// {
	///		out_position = mul(mul(pos,(float4x4)world),viewProjection);
	/// }
	/// </code>
	/// <para></para>
	/// <para><b>Shader Instancing:</b></para>
	/// <para>It is highly recommended to implement shader-instancing for rendring smaller batches of small objects, or for supporting instancing on older hardware and the xbox</para>
	/// <para>Shader instancing is a technique where the instance matrices are stored as shader constants, and the vertex/index data is duplicated to draw multiple instances.</para>
	/// <para>The disadvantage is the extra memory overhead is considerable for large meshes. Each vertex must also store an index to the instance matrix to use.</para>
	/// <para>For vertex-shader 2.0, there is uaully room for an array of 50-60 matrices</para>
	/// <code>
	/// float4x4 viewProjection : VIEWPROJECTION;
	/// float4x4 world[60];
	/// 
	/// void ShaderInstancedVS(	in  float4 position		: POSITION, 
	///							out float4 out_position : POSITION, 
	///							in  float  index		: POSITION1)
	/// {
	///		out_position = mul(mul(pos,world[index]),viewProjection);
	/// }
	/// </code>
	/// </remarks>
	/// <seealso cref="DrawState"/>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class StreamFrequency
	{
		/// <summary>
		/// A structure used to store per-instance world matrix data
		/// </summary>
		/// <remarks>This data is stored in vertex shader inputs POSITION12,POSITION13,POSITION14,POSITION15 to ease implementation</remarks>
#if !DEBUG_API
		[System.Diagnostics.DebuggerStepThrough]
#endif
		public struct InstanceMatrix
		{
			/// <summary>
			/// Copy the matrix into the position values
			/// </summary>
			/// <param name="mat"></param>
			public void Set(ref Matrix mat)
			{
				this.position12.X = mat.M11;
				this.position12.Y = mat.M12;
				this.position12.Z = mat.M13;
				this.position12.W = mat.M14;

				this.position13.X = mat.M21;
				this.position13.Y = mat.M22;
				this.position13.Z = mat.M23;
				this.position13.W = mat.M24;

				this.position14.X = mat.M31;
				this.position14.Y = mat.M32;
				this.position14.Z = mat.M33;
				this.position14.W = mat.M34;

				this.position15.X = mat.M41;
				this.position15.Y = mat.M42;
				this.position15.Z = mat.M43;
				this.position15.W = mat.M44;
			}
			/// <summary>
			/// Copy the position into the local buffer as a Translation matrix
			/// </summary>
			/// <param name="position"></param>
			public void Set(ref Vector3 position)
			{
				this.position12.X = 1;
				this.position12.Y = 0;
				this.position12.Z = 0;
				this.position12.W = 0;

				this.position13.X = 0;
				this.position13.Y = 1;
				this.position13.Z = 0;
				this.position13.W = 0;

				this.position14.X = 0;
				this.position14.Y = 0;
				this.position14.Z = 1;
				this.position14.W = 0;

				this.position15.X = position.X;
				this.position15.Y = position.Y;
				this.position15.Z = position.Z;
				this.position15.W = 1;
			}

			/// <summary>
			/// Copy the raw position values into the structure
			/// </summary>
			/// <param name="position12"></param>
			/// <param name="position13"></param>
			/// <param name="position14"></param>
			/// <param name="position15"></param>
			public void Set(ref Vector4 position12,ref Vector4 position13,ref Vector4 position14,ref Vector4 position15)
			{
				this.position12 = position12;
				this.position13 = position13;
				this.position14 = position14;
				this.position15 = position15;
			}

			/// <summary></summary>
			public Vector4 position12;
			/// <summary></summary>
			public Vector4 position13;
			/// <summary></summary>
			public Vector4 position14;
			/// <summary></summary>
			public Vector4 position15;
		}

		/// <summary>
		/// Data layout for the frequency data. Most implementations will want to use <see cref="DataLayout.Stream0Geometry_Stream1InstanceData"/>.
		/// </summary>
		public enum DataLayout
		{
			/// <summary>
			/// First vertex buffer contains geometry data, second buffer contains instance data (eg world matrix)
			/// </summary>
			Stream0Geometry_Stream1InstanceData,
			/// <summary>
			/// Use this if you know what your doing...
			/// </summary>
			Custom
		}

		internal readonly int[] frequency, indexFrequency, dataFrequency;

		/// <summary>
		/// Setup the frequency data and source vertex buffer
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="repeatCount">Number of times the vertex data should be repeated</param>
		public StreamFrequency(IVertices vertices, int repeatCount)
		{
			if (vertices is VerticesGroup)
			{
				this.frequency = new int[(vertices as VerticesGroup).ChildCount];
				this.indexFrequency = new int[(vertices as VerticesGroup).ChildCount];
				this.dataFrequency = new int[(vertices as VerticesGroup).ChildCount];

				this.RepeatCount = repeatCount;
			}
			else
			{
				this.frequency = new int[1];
				this.indexFrequency = new int[1];
				this.dataFrequency = new int[1];

				this.indexFrequency[0] = repeatCount;
			}
		}

		/// <summary>
		/// Automatic setup of frequency data from a vertices group (eg, geometry and instance data in two buffers)
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="layout"></param>
		public StreamFrequency(VerticesGroup vertices, DataLayout layout)
		{
			this.frequency = new int[vertices.ChildCount];
			this.indexFrequency = new int[vertices.ChildCount];
			this.dataFrequency = new int[vertices.ChildCount];
			
			if (layout == DataLayout.Stream0Geometry_Stream1InstanceData)
			{
				RepeatCount = vertices.GetChild(1).Count;
			}
		}

		/// <summary>
		/// Specifies how many times the geometry should be drawn
		/// </summary>
		public int RepeatCount
		{
			get { return indexFrequency[0]; }
			set
			{
				indexFrequency[0] = value;
				if (dataFrequency.Length>1)
					dataFrequency[1] = 1;
			}
		}

		/// <summary>
		/// Set the raw frequency/indexFrequency/dataFrequency values. Only exposed as their use is so badly documented by microsoft, and there may be hidden unexpected uses.
		/// </summary>
		/// <param name="index">the stream index to set</param>
		/// <param name="frequency">Unknown use. Use with caution</param>
		/// <param name="indexFrequency">Only valid for the first stream (stream zero), sets the number of times indices are repeated</param>
		/// <param name="instanceDataFrequency">Not valid for the first stream (stream zero), repeat count for the actual data. Best kept at 1.</param>
		public void SetData(int index, int frequency, int indexFrequency, int instanceDataFrequency)
		{
			this.frequency[index] = frequency;
			this.indexFrequency[index] = indexFrequency;
			this.dataFrequency[index] = instanceDataFrequency;
		}
	}

	/// <summary>
	/// Stores a group of <see cref="IVertices"/> instances, sharing their unique elements in a single <see cref="VertexDeclaration"/>
	/// </summary>
	/// <remarks>
	/// <para>This class can be used to draw vertices from multiple <see cref="IVertices"/> vertex buffers, sharing the first instance of each element type.</para>
	/// <para>For example, the first vertex buffer may specify positions, while the second may specify normals. Using a <see cref="VerticesGroup"/> is a way to share this data, without creating a new resource.</para>
	/// </remarks>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class VerticesGroup : Resource, IVertices, IDeviceVertexBuffer, IContentOwner
	{
		static int maxStreams = -1;
		/// <summary>
		/// The maximum group size supported by the hardware
		/// </summary>
		public static int MaxGroupSize
		{
			get
			{
				if (maxStreams == -1)
					maxStreams = GraphicsAdapter.DefaultAdapter.GetCapabilities(DeviceType.Hardware).MaxStreams;
				return maxStreams;
			}
		}
		/// <summary>
		/// Construct the group of vertices
		/// </summary>
		/// <param name="children"></param>
		public VerticesGroup(params IVertices[] children)
		{
			if (children.Length <= 1)
				throw new ArgumentException("Specify at least two children");

			if (children.Length > MaxGroupSize)
				throw new ArgumentException("Hardware does not support this many streams, see VerticesGroup.MaxGroupSize");

			int index = 0;
			foreach (IVertices vb in children)
			{
				if (vb == null)
					throw new ArgumentNullException();
				if (vb is VerticesGroup)
					throw new ArgumentException("VerticesGroup as Child");
				if (vb is IDeviceVertexBuffer == false)
					throw new ArgumentException(vb.GetType().Name + " as Child");
				if ((vb.ResourceUsage & ResourceUsage.Dynamic) == ResourceUsage.Dynamic)
					this.containsDynamicBuffers = true;
				if (Array.IndexOf(children, vb) != index++)
					throw new ArgumentException("Duplicate entry detected");
			}

			bufferTypes = new Type[children.Length];
			for (int i = 0; i < children.Length; i++)
				bufferTypes[i] = children[i].VertexType;

			this.buffers = children;
			this.offsets = new int[children.Length];
		}


		private readonly IVertices[] buffers;
		private readonly int[] offsets;
		private readonly Type[] bufferTypes;
		private int count = -1;
		private readonly bool containsDynamicBuffers;
		private VertexDeclaration decl;
		private bool ownerRegistered = false;
 
		/// <summary>
		/// Get a group member by index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IVertices GetChild(int index)
		{
			return buffers[index];
		}

		internal void SetChild(int index, IVertices child)
		{
			if (buffers[index] != child)
			{
				buffers[index] = child;
				if (child.VertexType != bufferTypes[index])
				{
					decl = null;
					bufferTypes[index] = child.VertexType;
				}
			}
		}

		ResourceUsage IVertices.ResourceUsage { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

		/// <summary>
		/// Number of group members
		/// </summary>
		public int ChildCount
		{
			get { return buffers.Length; }
		}

		/// <summary>
		/// Approximate number of vertices (minimum over the group), -1 is not yet known
		/// </summary>
		public int Count
		{
			get 
			{
				if (count != -1)
					return count;
				if (containsDynamicBuffers)
					throw new NotSupportedException("Count is not supported with Dynamic children");
				int c = int.MaxValue;
				foreach (IVertices vb in buffers)
				{
					c = Math.Min(vb.Count, c);
				}
				this.count = c;
				return count;
			}
			set
			{
				count = value;
			}
		}

		/// <summary>
		/// Set the read offset to a group member
		/// </summary>
		/// <param name="index"></param>
		/// <param name="offset">Element offset to start reading into the buffer</param>
		public void SetIndexOffset(int index, int offset)
		{
			offsets[index] = offset * buffers[index].Stride;
		}

		/// <summary>
		/// Get the read offset of a group member
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public int GetIndexOffset(int index)
		{
			return offsets[index] / buffers[index].Stride;
		}

		int IVertices.Stride
		{
			get { throw new NotSupportedException(); }
		}

		Type IVertices.VertexType
		{
			get { throw new NotSupportedException(); }
		}

		#region IDeviceVertexBuffer Members

		VertexBuffer IDeviceVertexBuffer.GetVertexBuffer(DrawState state)
		{
			return null;
		}

		VertexDeclaration IDeviceVertexBuffer.GetVertexDeclaration(Application game)
		{
			if (decl == null)
				decl = game.declarationBuilder.GetDeclaration(game.GraphicsDevice, bufferTypes);
			return decl;
		}

		#endregion

		/// <summary>
		/// Draw the vertices group as primitives, using an optional index buffer (indices)
		/// </summary>
		/// <param name="state"></param>
		/// <param name="indices">(optional) indices to use during drawing</param>
		/// <param name="primitiveType">Primitive type to draw, eg PrimitiveType.TriangleList</param>
		/// <remarks></remarks>
		public void Draw(DrawState state, IIndices indices, PrimitiveType primitiveType)
		{
			Draw(state, indices, primitiveType, null,int.MaxValue,0,0);
		}
		
		/// <summary>
		/// Draw the vertices group as primitives, using an optional index buffer (indices) and a <see cref="StreamFrequency"/> object to specify Hardware Instancing information (Shader Model 3 and Windows Only)
		/// </summary>
		/// <param name="state"></param>
		/// <param name="indices">(optional) indices to use during drawing</param>
		/// <param name="primitiveType">Primitive type to draw, eg PrimitiveType.TriangleList</param>
		/// <param name="frequency">(optional) <see cref="StreamFrequency"/> setting the shader model 3 instance frequency data (used for hardware instancing)</param>
		/// <remarks></remarks>
		public void Draw(DrawState state, IIndices indices, PrimitiveType primitiveType, StreamFrequency frequency)
		{
			Draw(state, indices, primitiveType, frequency, int.MaxValue, 0, 0);
		}

		/// <summary>
		/// Draw the vertices group as primitives, using an optional index buffer (indices) and a <see cref="StreamFrequency"/> object to specify Hardware Instancing information (Shader Model 3 and Windows Only)
		/// </summary>
		/// <param name="state"></param>
		/// <param name="indices">(optional) indices to use during drawing</param>
		/// <param name="primitiveType">Primitive type to draw, eg PrimitiveType.TriangleList</param>
		/// <param name="frequency">(optional) <see cref="StreamFrequency"/> setting the shader model 3 instance frequency data (used for hardware instancing)</param>
		/// <param name="primitveCount">The number of primitives to draw</param>
		/// <param name="startIndex">The start index in the index buffer (defaults to the first index - 0)</param>
		/// <param name="vertexOffset">Starting offset into the vertex buffer (defaults to the first vertex - 0)</param>
		/// <remarks></remarks>
		public void Draw(DrawState state, IIndices indices, PrimitiveType primitiveType, StreamFrequency frequency, int primitveCount, int startIndex, int vertexOffset)
		{
			if (frequency != null && !state.SupportsHardwareInstancing)
			{
				throw new InvalidOperationException("Only windows devices supporting Shader Model 3.0 or better to can use hardware instancing. Check DrawState.SupportsHardwareInstancing");
			}

			state.ApplyRenderStateChanges();

			GraphicsDevice device = state.graphics;

			IDeviceIndexBuffer devib = indices as IDeviceIndexBuffer;
			IndexBuffer ib = null;
			if (devib != null)
				ib = devib.GetIndexBuffer(state);

			if (decl == null)
				decl = ((IDeviceVertexBuffer)this).GetVertexDeclaration(state.Application);

			state.VertexDeclaration = decl;

			int vertices = 0;
			for (int i = 0; i < buffers.Length; i++)
			{
				IDeviceVertexBuffer dev = buffers[i] as IDeviceVertexBuffer;
				if (dev != null)
					state.SetStream(i, dev.GetVertexBuffer(state), offsets[i], buffers[i].Stride);
				else
					state.SetStream(i,null, 0, 0);

				if (i == 0)
					vertices = buffers[i].Count;
				else
					if (frequency == null)
						vertices = Math.Min(buffers[i].Count, vertices);
			}

			state.IndexBuffer = ib;

			if (ib != null)
				vertices = indices.Count;

			int primitives = 0;
			switch (primitiveType)
			{
				case PrimitiveType.LineList:
					primitives = vertices / 2;
					break;
				case PrimitiveType.LineStrip:
					primitives = vertices - 1;
					break;
				case PrimitiveType.PointList:
					primitives = vertices;
					break;
				case PrimitiveType.TriangleList:
					primitives = vertices / 3;
					break;
				case PrimitiveType.TriangleFan:
				case PrimitiveType.TriangleStrip:
					primitives = vertices - 2;
					break;
			}
			
#if DEBUG
			int instances = 1;
#endif

#if !XBOX360
			if (frequency != null && frequency.frequency.Length >= this.buffers.Length)
			{
#if DEBUG
				if (frequency.indexFrequency.Length > 0)
				{
					state.Application.currentFrame.InstancesDrawBatchCount++;
					state.Application.currentFrame.InstancesDrawn += frequency.indexFrequency[0];
					instances = frequency.indexFrequency[0];
				}
#endif

				for (int i = 0; i < this.buffers.Length; i++)
				{
					VertexStream vs = device.Vertices[i];
					if (frequency.frequency[i] != 0)
						vs.SetFrequency(frequency.frequency[i]);
					if (frequency.indexFrequency[i] != 0)
						vs.SetFrequencyOfIndexData(frequency.indexFrequency[i]);
					if (frequency.dataFrequency[i] != 0)
						vs.SetFrequencyOfInstanceData(frequency.dataFrequency[i]);
				}
			}
#endif

			if (primitveCount != int.MaxValue)
			{
				if (primitveCount > primitives ||
					primitveCount <= 0)
					throw new ArgumentException("primitiveCount");
			}
			else
				primitveCount = primitives;

#if DEBUG
			state.CalcBoundTextures();
#endif
			
			//it is possible to have the debug runtime throw an exception here when using instancing,
			//as it thinks stream1 doesn't have enough data.
			//This is most common with sprite groups (however sprite groups will use shader-instancing with small groups)
			//eg, drawing a single instance requires only 64bytes in stream1, yet the triangle count could be very large
			//this makes the debug runtime think that stream1 doesn't have enough data
			if (ib != null)
			{
#if DEBUG
				state.Application.currentFrame.DrawIndexedPrimitiveCallCount++;
#endif
				device.DrawIndexedPrimitives(primitiveType, vertexOffset, indices.MinIndex, indices.MaxIndex - indices.MinIndex, startIndex, primitveCount);
			}
			else
			{
#if DEBUG
				state.Application.currentFrame.DrawPrimitivesCallCount++;
#endif
				device.DrawPrimitives(primitiveType, vertexOffset, primitveCount);
			}


#if DEBUG
			switch (primitiveType)
			{
				case PrimitiveType.LineList:
				case PrimitiveType.LineStrip:
					state.Application.currentFrame.LinesDrawn += primitives * instances;
					break;
				case PrimitiveType.PointList:
					state.Application.currentFrame.PointsDrawn += primitives * instances;
					break;
				case PrimitiveType.TriangleList:
				case PrimitiveType.TriangleFan:
				case PrimitiveType.TriangleStrip:
					state.Application.currentFrame.TrianglesDrawn += primitives * instances;
					break;
			}
#endif
			
#if !XBOX360
			if (frequency != null && frequency.frequency.Length >= this.buffers.Length)
			{
				device.Vertices[0].SetFrequency(1);
			}
#endif

			for (int i = 0; i < buffers.Length; i++)
			{
				if (i > 0)
					state.SetStream(i, null, 0, 0);
			}
		}
		
		/// <summary>
		/// Draw the vertices as primitives with extended parametres, using an optional index buffer (indices)
		/// </summary>
		/// <param name="state"></param>
		/// <param name="indices">indices to use when drawing (may be null)</param>
		/// <param name="primitiveType">Primitive type to use when drawing the buffer</param>
		/// <param name="primitveCount">The number of primitives to draw</param>
		/// <param name="startIndex">The start index in the index buffer (defaults to the first index - 0)</param>
		/// <param name="vertexOffset">Starting offset into the vertex buffer (defaults to the first vertex - 0)</param>
		void IVertices.Draw(DrawState state, IIndices indices, PrimitiveType primitiveType, int primitveCount, int startIndex, int vertexOffset)
		{
			this.Draw(state, indices, primitiveType, null, primitveCount, startIndex, vertexOffset);
		}

		void IDisposable.Dispose()
		{
			//has no effect
		}

		/// <summary>
		/// Not supported for vertices groups
		/// </summary>
		void IVertices.SetDirty()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Not supported for vertices groups
		/// </summary>
		/// <param name="count"></param>
		/// <param name="startIndex"></param>
		void IVertices.SetDirtyRange(int startIndex, int count)
		{
			throw new NotSupportedException();
		}

		internal override int GetAllocatedManagedBytes()
		{
			return 0;
		}

		internal override int GetAllocatedDeviceBytes()
		{
			return 0;
		}

		internal override ResourceType ResourceType
		{
			get { return ResourceType.VertexBuffer; }
		}

		internal override bool InUse
		{
			get { return decl != null; }
		}

		internal override bool IsDisposed
		{
			get { return this.buffers == null; }
		}

		internal override void Warm(Application application, GraphicsDevice device)
		{
			if (!ownerRegistered)
			{
				ownerRegistered = true;
				application.Content.Add(this);
				return;
			}
			if (buffers != null)
			{
				foreach (IVertices buffer in buffers)
				{
					if (buffer is Resource)
						(buffer as Resource).Warm(application);
				}
			}
			((IDeviceVertexBuffer)this).GetVertexDeclaration(application);
		}

		#region IContentOwner Members

		void IContentOwner.LoadContent(ContentRegister content, DrawState state, Microsoft.Xna.Framework.Content.ContentManager manager)
		{
			Warm(state);
		}

		void IContentOwner.UnloadContent(ContentRegister content, DrawState state)
		{
			decl = null;
		}

		#endregion
	}
}