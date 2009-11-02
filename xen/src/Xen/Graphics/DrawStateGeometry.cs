using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Xen
{
	sealed partial class DrawState
	{
		//when rendering a batch, store indices in a stream buffer
		//stream buffers are cleared every frame
		sealed class StreamBuffer
		{
			private readonly Graphics.StreamFrequency.InstanceMatrix[] instanceMatricesData;
			private readonly Graphics.IVertices instanceMatrices;
			private Graphics.VerticesGroup instanceBuffer;
			private int index;
			private bool bufferActive;
			private static int InstanceSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Graphics.StreamFrequency.InstanceMatrix));
			internal Graphics.StreamFrequency.InstanceBuffer FrequencyInstanceBuffer;

			public StreamBuffer(int count)
			{
				int createSize = 256;
				while (count > createSize)
					createSize *= 2;

				this.instanceMatricesData = new Graphics.StreamFrequency.InstanceMatrix[createSize];
				this.instanceMatrices = new Graphics.Vertices<Graphics.StreamFrequency.InstanceMatrix>(this.instanceMatricesData);
				this.instanceMatrices.ResourceUsage = Xen.Graphics.ResourceUsage.DynamicSequential;
				this.index = 0;
			}

			public int FreeIndices
			{
				get { return this.instanceMatricesData.Length - index; }
			}
			public int WrittenIndices
			{
				get { return index; }
			}

			public Graphics.StreamFrequency.InstanceMatrix[] Prepare(out int startIndex)
			{
				if (bufferActive)
					throw new InvalidOperationException("A call to BeginDrawBatch has been made while already within a BeginDrawBatch/EndDrawBatch operation.");
				bufferActive = true;
				startIndex = index;
				return instanceMatricesData;
			}

			public Graphics.VerticesGroup Fill(Graphics.IVertices vertices, int count)
			{
				if (this.instanceBuffer == null)
					this.instanceBuffer = new Graphics.VerticesGroup(vertices, this.instanceMatrices);

				bufferActive = false;

				this.instanceBuffer.Count = vertices.Count;
				this.instanceBuffer.SetChild(0, vertices);
				this.instanceBuffer.SetIndexOffset(1, index);
				this.instanceMatrices.SetDirtyRange(index, count);
				this.index += count;

				return this.instanceBuffer;
			}

			public void Clear()
			{
				index = 0;
			}
		}

		internal void PrepareForNewFrame()
		{
			foreach (StreamBuffer sb in streamBuffers)
				sb.Clear();
		}

		private List<StreamBuffer> streamBuffers = new List<StreamBuffer>();
		private Graphics.StreamFrequency frequency;

		private Graphics.StreamFrequency.InstanceBuffer nullInstanceBuffer = new Xen.Graphics.StreamFrequency.InstanceBuffer();

		private StreamBuffer GetBuffer(int count)
		{
#if XBOX360
			StreamBuffer closestBuffer = null;

			//on the xbox,
			//when tiling is active, a buffer can not be reused within the frame
			//technically it can, but XNA detects it and thinks the entire buffer is
			//being rewritten (which it isn't) and throws an exception.

			foreach (StreamBuffer buf in streamBuffers)
			{
				if (buf.WrittenIndices == 0 &&
					buf.FreeIndices >= count)
				{
					if (closestBuffer == null)
						closestBuffer = buf;
					else
					{
						if (buf.FreeIndices > closestBuffer.FreeIndices)
							closestBuffer = buf;
					}
				}
			}

			if (closestBuffer != null)
				return closestBuffer;

#else
			foreach (StreamBuffer buf in streamBuffers)
			{
				if (buf.FreeIndices >= count)
					return buf;
			}
#endif

			StreamBuffer buffer = new StreamBuffer(count);
			streamBuffers.Add(buffer);
			return buffer;
		}

		/// <summary>
		/// [Requires <see cref="SupportsHardwareInstancing"/>] Draws multiple instances of a vertex buffer in an efficient way, using an optional callback to determine if an instance should be drawn. Using Shader Instancing is highly recommended for smaller batches of simple geometry.
		/// </summary>
		/// <param name="vertices">Vertex buffer of the instances to be drawn</param>
		/// <param name="indices">Index buffer of the instances to be drawn</param>
		/// <param name="primitiveType">Primitive type to be drawn</param>
		/// <param name="CanDrawItem">[optional] callback to determine if an instance index should be drawn or not (may be null)</param>
		/// <param name="instances">Array of instances to draw</param>
		/// <returns>number of instances drawn</returns>
		public int DrawBatch(Xen.Graphics.IVertices vertices, Graphics.IIndices indices, PrimitiveType primitiveType, Callback<bool, int, ICuller> CanDrawItem, params Matrix[] instances)
		{
			return DrawBatch(vertices, indices, primitiveType, CanDrawItem, instances, instances.Length);
		}

		/// <summary>
		/// [Requires <see cref="SupportsHardwareInstancing"/>] Draws multiple instances of a vertex buffer in an efficient way, using an optional callback to determine if an instance should be drawn. Using Shader Instancing is highly recommended for smaller batches of simple geometry.
		/// </summary>
		/// <param name="vertices">Vertex buffer of the instances to be drawn</param>
		/// <param name="indices">Index buffer of the instances to be drawn</param>
		/// <param name="primitiveType">Primitive type to be drawn</param>
		/// <param name="CanDrawItem">[optional] callback to determine if an instance index should be drawn or not (may be null)</param>
		/// <param name="instances">Array of instances to draw</param>
		/// <param name="instancesLength">Number of instances in the array to draw</param>
		/// <returns>number of instances drawn</returns>
		public int DrawBatch(Xen.Graphics.IVertices vertices, Graphics.IIndices indices, PrimitiveType primitiveType, Callback<bool, int, ICuller> CanDrawItem, Matrix[] instances, int instancesLength)
		{
			return DrawBatch(vertices, indices, primitiveType, CanDrawItem, instances, instancesLength, 0);
		}
		/// <summary>
		/// [Requires <see cref="SupportsHardwareInstancing"/>] Draws multiple instances of a vertex buffer in an efficient way, using an optional callback to determine if an instance should be drawn. Using Shader Instancing is highly recommended for smaller batches of simple geometry.
		/// </summary>
		/// <param name="vertices">Vertex buffer of the instances to be drawn</param>
		/// <param name="indices">Index buffer of the instances to be drawn</param>
		/// <param name="primitiveType">Primitive type to be drawn</param>
		/// <param name="CanDrawItem">[optional] callback to determine if an instance index should be drawn or not (may be null)</param>
		/// <param name="instances">Array of instances to draw</param>
		/// <param name="instancesLength">Number of instances in the array to draw</param>
		/// <returns>number of instances drawn</returns>
		/// <param name="startIndex">start read index in the <paramref name="instances"/> array</param>
		public int DrawBatch(Xen.Graphics.IVertices vertices, Graphics.IIndices indices, PrimitiveType primitiveType, Callback<bool, int, ICuller> CanDrawItem, Matrix[] instances, int instancesLength, int startIndex)
		{
			ValidateProtected();

			if (instancesLength <= 0)
				return 0;

			StreamBuffer buffer = GetBuffer(instancesLength);

			int start;
			Graphics.StreamFrequency.InstanceMatrix[] instanceMartixData = buffer.Prepare(out start);

			int count = 0;
			if (CanDrawItem != null)
			{
				ICuller culler = this;
				for (int i = 0; i < instancesLength; i++)
				{
					PushWorldMatrixMultiply(ref instances[i + startIndex]);

					if (CanDrawItem(i + startIndex, culler))
					{
						instanceMartixData[start].Set(ref ms_World.value);
						start++;
						count++;
					}

					PopWorldMatrix();
				}
			}
			else
			{
				if (ms_World.isIdentity || ms_World.value == Matrix.Identity)
				{
					for (int i = 0; i < instancesLength; i++)
					{
						instanceMartixData[start++].Set(ref instances[i + startIndex]);
					}
					count = instancesLength;
				}
				else
				{
					for (int i = 0; i < instancesLength; i++)
					{
						PushWorldMatrixMultiply(ref instances[i + startIndex]);
						instanceMartixData[start].Set(ref ms_World.value);
						start++;
						count++;
						PopWorldMatrix();
					}
				}
			}

			if (count == 0)
				return 0;

			return DrawBatch(buffer.Fill(vertices, count), indices, primitiveType, count);
		}

		/// <summary>
		/// [Requires <see cref="SupportsHardwareInstancing"/>] Draws multiple instances of a vertex buffer in an efficient way, using an optional callback to determine if an instance should be drawn. Using Shader Instancing is highly recommended for smaller batches of simple geometry.
		/// </summary>
		/// <param name="vertices">Vertex buffer of the instances to be drawn</param>
		/// <param name="indices">Index buffer of the instances to be drawn</param>
		/// <param name="primitiveType">Primitive type to be drawn</param>
		/// <param name="CanDrawItem">[optional] callback to determine if an instance index should be drawn or not (may be null)</param>
		/// <param name="instances">Array of instances to draw</param>
		/// <returns>number of instances drawn</returns>
		public int DrawBatch(Xen.Graphics.IVertices vertices, Graphics.IIndices indices, PrimitiveType primitiveType, Callback<bool, int, ICuller> CanDrawItem, params Vector3[] instances)
		{
			return DrawBatch(vertices, indices, primitiveType, CanDrawItem, instances, instances.Length);
		}


		/// <summary>
		/// [Requires <see cref="SupportsHardwareInstancing"/>] Returns a buffer that instances can be written to. Follow with a call to <see cref="EndDrawBatch"/> to draw the buffered instances
		/// </summary>
		/// <param name="maxInstanceCount"></param>
		/// <returns></returns>
		public Graphics.StreamFrequency.InstanceBuffer BeginDrawBatch(int maxInstanceCount)
		{
			ValidateProtected();

			if (maxInstanceCount < 0)
				throw new ArgumentException();

			if (maxInstanceCount == 0)
				return nullInstanceBuffer;

			StreamBuffer buffer = GetBuffer(maxInstanceCount);
			int start;
			Graphics.StreamFrequency.InstanceMatrix[] instanceMartixData = buffer.Prepare(out start);

			if (buffer.FrequencyInstanceBuffer == null)
				buffer.FrequencyInstanceBuffer = new Xen.Graphics.StreamFrequency.InstanceBuffer();
			buffer.FrequencyInstanceBuffer.Set(buffer, instanceMartixData, start, maxInstanceCount);

			return buffer.FrequencyInstanceBuffer;
		}

		/// <summary>
		/// [Requires <see cref="SupportsHardwareInstancing"/>] Complete a batch draw process (being with <see cref="BeginDrawBatch"/>)
		/// </summary>
		/// <param name="vertices">Vertex buffer of the instances to be drawn</param>
		/// <param name="indices">Index buffer of the instances to be drawn</param>
		/// <param name="primitiveType">Primitive type to be drawn</param>
		/// <param name="instances">Instance buffer that contains the instances to be drawn</param>
		public void EndDrawBatch(Xen.Graphics.IVertices vertices, Graphics.IIndices indices, PrimitiveType primitiveType, Graphics.StreamFrequency.InstanceBuffer instances)
		{
			ValidateProtected();

			if (instances == null)
				throw new ArgumentNullException();

			if (instances.InstanceCount == 0)
				return;

			DrawBatch(((StreamBuffer)instances.instanceBuffer).Fill(vertices, instances.InstanceCount), indices, primitiveType, instances.InstanceCount);
		}


		/// <summary>
		/// [Requires <see cref="SupportsHardwareInstancing"/>] Draws multiple instances of a vertex buffer in an efficient way, using an optional callback to determine if an instance should be drawn. Using Shader Instancing is highly recommended for smaller batches of simple geometry.
		/// </summary>
		/// <param name="vertices">Vertex buffer of the instances to be drawn</param>
		/// <param name="indices">Index buffer of the instances to be drawn</param>
		/// <param name="primitiveType">Primitive type to be drawn</param>
		/// <param name="CanDrawItem">[optional] callback to determine if an instance index should be drawn or not (may be null)</param>
		/// <param name="instances">Array of instances to draw</param>
		/// <param name="instancesLength">Number of instances in the array to draw</param>
		/// <returns>number of instances drawn</returns>
		public int DrawBatch(Xen.Graphics.IVertices vertices, Graphics.IIndices indices, PrimitiveType primitiveType, Callback<bool, int, ICuller> CanDrawItem, Vector3[] instances, int instancesLength)
		{
			return DrawBatch(vertices, indices, primitiveType, CanDrawItem, instances, instancesLength, 0);
		}

		/// <summary>
		/// [Requires <see cref="SupportsHardwareInstancing"/>] Draws multiple instances of a vertex buffer in an efficient way, using an optional callback to determine if an instance should be drawn. Using Shader Instancing is highly recommended for smaller batches of simple geometry.
		/// </summary>
		/// <param name="vertices">Vertex buffer of the instances to be drawn</param>
		/// <param name="indices">Index buffer of the instances to be drawn</param>
		/// <param name="primitiveType">Primitive type to be drawn</param>
		/// <param name="CanDrawItem">[optional] callback to determine if an instance index should be drawn or not (may be null)</param>
		/// <param name="instances">Array of instances to draw</param>
		/// <param name="instancesLength">Number of instances in the array to draw</param>
		/// <returns>number of instances drawn</returns>
		/// <param name="startIndex">starting index in the <paramref name="instances"/> array</param>
		public int DrawBatch(Xen.Graphics.IVertices vertices, Graphics.IIndices indices, PrimitiveType primitiveType, Callback<bool, int, ICuller> CanDrawItem, Vector3[] instances, int instancesLength, int startIndex)
		{
			ValidateProtected();

			if (instancesLength == 0)
				return 0;

			StreamBuffer buffer = GetBuffer(instancesLength);
			int start;
			Graphics.StreamFrequency.InstanceMatrix[] instanceMartixData = buffer.Prepare(out start);

			int count = 0;
			if (CanDrawItem != null)
			{
				ICuller culler = this;
				for (int i = 0; i < instancesLength; i++)
				{
					PushWorldTranslateMultiply(ref instances[i + startIndex]);

					if (CanDrawItem(i + startIndex, culler))
					{
						GetWorldMatrix(ref instanceMartixData[start++]);
						count++;
					}

					PopWorldMatrix();
				}
			}
			else
			{
				if (ms_World.isIdentity || (instancesLength > 4 && ms_World.value == Matrix.Identity))
				{
					for (int i = 0; i < instancesLength; i++)
						instanceMartixData[start++].Set(ref instances[i + startIndex]);
					count = instancesLength;
				}
				else
				{
					for (int i = 0; i < instancesLength; i++)
					{
						PushWorldTranslateMultiply(ref instances[i + startIndex]);
						instanceMartixData[start].Set(ref ms_World.value);
						start++;
						count++;
						PopWorldMatrix();
					}
				}
			}

			return DrawBatch(buffer.Fill(vertices, count), indices, primitiveType, count);
		}

		private int DrawBatch(Xen.Graphics.VerticesGroup vertices, Graphics.IIndices indices, PrimitiveType primitiveType, int count)
		{
			if (frequency == null)
				frequency = new Xen.Graphics.StreamFrequency(vertices, Xen.Graphics.StreamFrequency.DataLayout.Stream0Geometry_Stream1InstanceData);

			frequency.RepeatCount = count;
			vertices.Draw(this, indices, primitiveType, frequency);

			return count;
		}

		/// <summary>
		/// Draw an XNA vertex buffer, using state management to reduce redundant state changes.
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="indices"></param>
		/// <param name="declaration"></param>
		/// <param name="primitiveType"></param>
		/// <param name="baseVertex"></param>
		/// <param name="numVertices"></param>
		/// <param name="primitiveCount"></param>
		/// <param name="startIndex"></param>
		/// <param name="streamOffset"></param>
		/// <param name="vertexStride"></param>
		public void DrawVertexBuffer(VertexBuffer vertices, IndexBuffer indices, VertexDeclaration declaration, PrimitiveType primitiveType, int baseVertex, int numVertices, int primitiveCount, int startIndex, int streamOffset, int vertexStride)
		{
			ApplyRenderStateChanges(numVertices);
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DrawXnaVerticesCount);

			CalcBoundTextures();
#endif
			if (indices != null)
				this.IndexBuffer = indices;
			this.SetStream(0, vertices, streamOffset, vertexStride);
			this.VertexDeclaration = declaration;
			if (indices != null)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref Application.currentFrame.DrawIndexedPrimitiveCallCount);
#endif

				this.graphics.DrawIndexedPrimitives(primitiveType, baseVertex, 0, numVertices, startIndex, primitiveCount);
			}
			else
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DrawPrimitivesCallCount);
#endif

				this.graphics.DrawPrimitives(primitiveType, baseVertex, primitiveCount);
			}

#if DEBUG
			switch (primitiveType)
			{
				case PrimitiveType.LineList:
				case PrimitiveType.LineStrip:
					Application.currentFrame.LinesDrawn += primitiveCount;
					break;
				case PrimitiveType.PointList:
					Application.currentFrame.PointsDrawn += primitiveCount;
					break;
				case PrimitiveType.TriangleList:
				case PrimitiveType.TriangleFan:
				case PrimitiveType.TriangleStrip:
					Application.currentFrame.TrianglesDrawn += primitiveCount;
					break;
			}
#endif
		}

#if DEBUG
		internal void ValidateVertexDeclarationForShader(VertexDeclaration decl, Type vertType)
		{
			if (boundShader != null && decl != null)
				application.declarationBuilder.ValidateVertexDeclarationForShader(decl, boundShader, vertType);
		}
#endif



		//drawing dynamic vertices


		/// <summary>
		/// <para>Draw dynamic vertices as primitives with extended parametres, using an index buffer (indices)</para>
		/// <para>NOTE: When using this method, the vertex and index data will be copied every frame.</para>
		/// <para>This method is best for volatile vertex data that is changing every frame.</para>
		/// <para>Use a dynamic Vertices object for dynamic data that changes less frequently, or requires use with a VerticesGroup objcet.</para>
		/// </summary>
		/// <param name="vertices">source vertex data to use for drawing vertices</param>
		/// <param name="indices">indices to use when drawing</param>
		/// <param name="primitiveType">Primitive type to use when drawing the buffer</param>
		/// <param name="primitveCount">The number of primitives to draw</param>
		/// <param name="startIndex">The start index in the index buffer (defaults to the first index - 0)</param>
		/// <param name="vertexOffset">Starting offset into the vertex buffer (defaults to the first vertex - 0)</param>
		/// <param name="maximumIndex">The maximum index used by the index buffer. This determines how many vertices will be copied (zero will copy all vertices)</param>
		public void DrawDynamicIndexedVertices<VertexType>(VertexType[] vertices, int[] indices, PrimitiveType primitiveType, int primitveCount, int startIndex, int maximumIndex, int vertexOffset) where VertexType : struct
		{
			DrawDynamicVerticesArray(vertices, indices, primitiveType, primitveCount, startIndex, vertexOffset, maximumIndex);
		}
		/// <summary>
		/// <para>Draw dynamic vertices as primitives with extended parametres, using an index buffer (indices)</para>
		/// <para>NOTE: When using this method, the vertex and index data will be copied every frame.</para>
		/// <para>This method is best for volatile vertex data that is changing every frame.</para>
		/// <para>Use a dynamic Vertices object for dynamic data that changes less frequently, or requires use with a VerticesGroup objcet.</para>
		/// </summary>
		/// <param name="vertices">source vertex data to use for drawing vertices</param>
		/// <param name="indices">indices to use when drawing</param>
		/// <param name="primitiveType">Primitive type to use when drawing the buffer</param>
		/// <param name="primitveCount">The number of primitives to draw</param>
		/// <param name="startIndex">The start index in the index buffer (defaults to the first index - 0)</param>
		/// <param name="vertexOffset">Starting offset into the vertex buffer (defaults to the first vertex - 0)</param>
		/// <param name="maximumIndex">The maximum index used by the index buffer. This determines how many vertices will be copied (zero will copy all vertices)</param>
		public void DrawDynamicIndexedVertices<VertexType>(VertexType[] vertices, short[] indices, PrimitiveType primitiveType, int primitveCount, int startIndex, int maximumIndex, int vertexOffset) where VertexType : struct
		{
			DrawDynamicVerticesArray(vertices, indices, primitiveType, primitveCount, startIndex, vertexOffset, maximumIndex);
		}

		/// <summary>
		/// <para>Draw dynamic vertices as primitives with extended parametres</para>
		/// <para>NOTE: When using this method, the vertex data will be copied every frame.</para>
		/// <para>This method is best for volatile vertex data that is changing every frame.</para>
		/// <para>Use a dynamic Vertices object for dynamic data that changes less frequently, or requires use with a VerticesGroup objcet.</para>
		/// </summary>
		/// <param name="vertices">source vertex data to use for drawing vertices</param>
		/// <param name="primitiveType">Primitive type to use when drawing the buffer</param>
		/// <param name="primitveCount">The number of primitives to draw</param>
		/// <param name="vertexOffset">Starting offset into the vertex buffer (defaults to the first vertex - 0)</param>
		public void DrawDynamicVertices<VertexType>(VertexType[] vertices, PrimitiveType primitiveType, int primitveCount, int vertexOffset) where VertexType : struct
		{
			DrawDynamicVerticesArray(vertices, null, primitiveType, primitveCount, 0, vertexOffset, 0);
		}


		private void DrawDynamicVerticesArray<VertexType>(VertexType[] vertices, Array indices, PrimitiveType primitiveType, int primitveCount, int startIndex, int vertexOffset, int maximumIndex) where VertexType : struct
		{
			if (DrawTarget == null)
				throw new InvalidOperationException("Vertices Draw calls should be done within the Draw() call of a DrawTarget object. (otherwise the draw target is undefined)");
			if (vertices == null)
				throw new ArgumentException();

			VertexDeclaration vd = application.declarationBuilder.GetDeclaration<VertexType>(graphics);

#if DEBUG
			ValidateVertexDeclarationForShader(vd, typeof(VertexType));
#endif

			VertexDeclaration = vd;

			int vertexCount = vertices.Length;
			if (indices != null)
				vertexCount = indices.Length;

			int primitives = 0;
			switch (primitiveType)
			{
				case PrimitiveType.LineList:
					primitives = vertexCount / 2;
					break;
				case PrimitiveType.LineStrip:
					primitives = vertexCount - 1;
					break;
				case PrimitiveType.PointList:
					primitives = vertexCount;
					break;
				case PrimitiveType.TriangleList:
					primitives = vertexCount / 3;
					break;
				case PrimitiveType.TriangleFan:
				case PrimitiveType.TriangleStrip:
					primitives = vertexCount - 2;
					break;
			}

			ApplyRenderStateChanges(vertexCount);

			if (primitveCount > primitives ||
				primitveCount <= 0)
				throw new ArgumentException("primitiveCount");

#if DEBUG
			CalcBoundTextures();
#endif

			if (indices != null)
			{
				if (maximumIndex <= 0)
					maximumIndex = vertexCount - 1;

#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DrawIndexedPrimitiveCallCount);
#endif

				if (indices is int[])
					graphics.DrawUserIndexedPrimitives<VertexType>(primitiveType, vertices, vertexOffset, maximumIndex + 1, (int[])indices, startIndex, primitveCount);
				else
					graphics.DrawUserIndexedPrimitives<VertexType>(primitiveType, vertices, vertexOffset, maximumIndex + 1, (short[])indices, startIndex, primitveCount);
			}
			else
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DrawPrimitivesCallCount);
#endif

				graphics.DrawUserPrimitives<VertexType>(primitiveType, vertices, vertexOffset, primitveCount);
			}

			//draw indexed primitives mucks up the stream settings.
			//Need to clear out the internal tracking data
			for (int i = 0; i < vertexStreams.Length; i++)
			{
				vertexStreams[i].vb = null;
				vertexStreams[i].offset = -1;
				vertexStreams[i].stride = -1;
			}
			indexBuffer = null;

#if DEBUG
			switch (primitiveType)
			{
				case PrimitiveType.LineList:
				case PrimitiveType.LineStrip:
					application.currentFrame.LinesDrawn += primitveCount;
					break;
				case PrimitiveType.PointList:
					application.currentFrame.PointsDrawn += primitveCount;
					break;
				case PrimitiveType.TriangleList:
				case PrimitiveType.TriangleFan:
				case PrimitiveType.TriangleStrip:
					application.currentFrame.TrianglesDrawn += primitveCount;
					break;
			}
#endif
		}


		/// <summary>
		/// <para>Draw dynamic vertices as primitives, using an optional index buffer (indices)</para>
		/// <para>NOTE: When using this method, the vertex and index data will be copied every frame.</para>
		/// <para>This method is best for volatile vertex data that is changing every frame.</para>
		/// <para>Use a dynamic Vertices object for dynamic data that changes less frequently, or requires use with a VerticesGroup objcet.</para>
		/// </summary>
		/// <param name="vertices">source vertex data to use for drawing vertices</param>
		/// <param name="indices">indices to use when drawing (may be null)</param>
		/// <param name="primitiveType">Primitive type to use when drawing the buffer</param>
		public void DrawDynamicIndexedVertices<VertexType>(VertexType[] vertices, int[] indices, PrimitiveType primitiveType) where VertexType : struct
		{
			DrawDynamicVerticesArray(vertices, indices, primitiveType);
		}
		/// <summary>
		/// <para>Draw dynamic vertices as primitives, using an optional index buffer (indices)</para>
		/// <para>NOTE: When using this method, the vertex and index data will be copied every frame.</para>
		/// <para>This method is best for volatile vertex data that is changing every frame.</para>
		/// <para>Use a dynamic Vertices object for dynamic data that changes less frequently, or requires use with a VerticesGroup objcet.</para>
		/// </summary>
		/// <param name="vertices">source vertex data to use for drawing vertices</param>
		/// <param name="indices">indices to use when drawing (may be null)</param>
		/// <param name="primitiveType">Primitive type to use when drawing the buffer</param>
		public void DrawDynamicIndexedVertices<VertexType>(VertexType[] vertices, short[] indices, PrimitiveType primitiveType) where VertexType : struct
		{
			DrawDynamicVerticesArray(vertices, indices, primitiveType);
		}

		/// <summary>
		/// <para>Draw dynamic vertices as primitives</para>
		/// <para>NOTE: When using this method, the vertex data will be copied every frame.</para>
		/// <para>This method is best for volatile vertex data that is changing every frame.</para>
		/// <para>Use a dynamic Vertices object for dynamic data that changes less frequently, or requires use with a VerticesGroup objcet.</para>
		/// </summary>
		/// <param name="vertices">source vertex data to use for drawing vertices</param>
		/// <param name="primitiveType">Primitive type to use when drawing the buffer</param>
		public void DrawDynamicVertices<VertexType>(VertexType[] vertices, PrimitiveType primitiveType) where VertexType : struct
		{
			DrawDynamicVerticesArray(vertices, null, primitiveType);
		}

		private void DrawDynamicVerticesArray<VertexType>(VertexType[] vertices, Array indices, PrimitiveType primitiveType) where VertexType : struct
		{
			if (DrawTarget == null)
				throw new InvalidOperationException("Vertices Draw calls should be done within the Draw() call of a DrawTarget object. (otherwise the draw target is undefined)");
			if (vertices == null)
				throw new ArgumentException();

			VertexDeclaration vd = application.declarationBuilder.GetDeclaration<VertexType>(graphics);

#if DEBUG
			ValidateVertexDeclarationForShader(vd, typeof(VertexType));
#endif

			VertexDeclaration = vd;

			int vertexCount = vertices.Length;
			if (indices != null)
				vertexCount = indices.Length;

			int primitives = 0;
			switch (primitiveType)
			{
				case PrimitiveType.LineList:
					primitives = vertexCount / 2;
					break;
				case PrimitiveType.LineStrip:
					primitives = vertexCount - 1;
					break;
				case PrimitiveType.PointList:
					primitives = vertexCount;
					break;
				case PrimitiveType.TriangleList:
					primitives = vertexCount / 3;
					break;
				case PrimitiveType.TriangleFan:
				case PrimitiveType.TriangleStrip:
					primitives = vertexCount - 2;
					break;
			}

			ApplyRenderStateChanges(vertexCount);

#if DEBUG
			CalcBoundTextures();
#endif

			if (indices != null)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DrawIndexedPrimitiveCallCount);
#endif

				if (indices is int[])
					graphics.DrawUserIndexedPrimitives<VertexType>(primitiveType, vertices, 0, vertices.Length, (int[])indices, 0, primitives);
				else
					graphics.DrawUserIndexedPrimitives<VertexType>(primitiveType, vertices, 0, vertices.Length, (short[])indices, 0, primitives);
			}
			else
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DrawPrimitivesCallCount);
#endif

				graphics.DrawUserPrimitives<VertexType>(primitiveType, vertices, 0, primitives);
			}

			//draw indexed primitives mucks up the stream settings.
			//Need to clear out the internal tracking data
			for (int i = 0; i < vertexStreams.Length; i++)
			{
				vertexStreams[i].vb = null;
				vertexStreams[i].offset = -1;
				vertexStreams[i].stride = -1;
			}
			indexBuffer = null;

#if DEBUG
			switch (primitiveType)
			{
				case PrimitiveType.LineList:
				case PrimitiveType.LineStrip:
					application.currentFrame.LinesDrawn += primitives;
					break;
				case PrimitiveType.PointList:
					application.currentFrame.PointsDrawn += primitives;
					break;
				case PrimitiveType.TriangleList:
				case PrimitiveType.TriangleFan:
				case PrimitiveType.TriangleStrip:
					application.currentFrame.TrianglesDrawn += primitives;
					break;
			}
#endif
		}
	}
}