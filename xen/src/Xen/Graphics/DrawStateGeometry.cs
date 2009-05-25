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
	}
}