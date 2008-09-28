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
			public StreamBuffer(Graphics.IVertices baseVertices, int count)
			{
				int createSize = 256;
				while (count > createSize)
					createSize *= 2;

				this.instanceMatricesData = new Graphics.StreamFrequency.InstanceMatrix[createSize];
				this.instanceMatrices = new Graphics.Vertices<Graphics.StreamFrequency.InstanceMatrix>(this.instanceMatricesData);
				this.instanceMatrices.ResourceUsage = Xen.Graphics.ResourceUsage.DynamicSequential;
				this.instanceBuffer = new Graphics.VerticesGroup(baseVertices,this.instanceMatrices);
				this.index = 0;
			}
			readonly Graphics.StreamFrequency.InstanceMatrix[] instanceMatricesData;
			readonly Graphics.IVertices instanceMatrices;
			Graphics.VerticesGroup instanceBuffer;
			int index;
			static int InstanceSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Graphics.StreamFrequency.InstanceMatrix));
			public int FreeIndices
			{
				get { return this.instanceMatricesData.Length - index; }
			}
			public Graphics.StreamFrequency.InstanceMatrix[] Prepare(Graphics.IVertices vertices, int maxPossibleCount, out int startIndex)
			{
				instanceBuffer.SetChild(0, vertices);
				instanceBuffer.Count = vertices.Count;
				startIndex = index;
				return instanceMatricesData;
			}
			public Graphics.VerticesGroup Fill(int count)
			{
				instanceBuffer.SetIndexOffset(1, index);
				instanceMatrices.SetDirtyRange(index, count);
				index += count;
				return instanceBuffer;
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

		List<StreamBuffer> streamBuffers = new List<StreamBuffer>();
		Graphics.StreamFrequency frequency;

		private StreamBuffer GetBuffer(Graphics.IVertices vertices, int count)
		{
			foreach (StreamBuffer buf in streamBuffers)
			{
				if (buf.FreeIndices > count)
					return buf;
			}
			StreamBuffer buffer = new StreamBuffer(vertices,count);
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
			ValidateProtected();

			if (instancesLength == 0)
				return 0;

			StreamBuffer buffer = GetBuffer(vertices, instancesLength);

			int start;
			Graphics.StreamFrequency.InstanceMatrix[] instanceMartixData = buffer.Prepare(vertices, instancesLength, out start);

			int count = 0;
			if (CanDrawItem != null)
			{
				ICuller culler = this;
				for (int i = 0; i < instancesLength; i++)
				{
					PushWorldMatrixMultiply(ref instances[i]);

					if (CanDrawItem(i, culler))
					{
						GetWorldMatrix(ref instanceMartixData[start++]);
						count++;
					}

					PopWorldMatrix();
				}
			}
			else
			{
				for (int i = 0; i < instancesLength; i++)
					instanceMartixData[start++].Set(ref instances[i]);
				count = instancesLength;
			}

			if (count == 0)
				return 0;

			return DrawBatch(buffer.Fill(count), indices, primitiveType, count);
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
			ValidateProtected();

			if (instancesLength == 0)
				return 0;

			StreamBuffer buffer = GetBuffer(vertices, instancesLength);
			int start;
			Graphics.StreamFrequency.InstanceMatrix[] instanceMartixData = buffer.Prepare(vertices, instancesLength, out start);

			int count = 0;
			if (CanDrawItem != null)
			{
				ICuller culler = this;
				for (int i = 0; i < instancesLength; i++)
				{
					PushWorldTranslateMultiply(ref instances[i]);

					if (CanDrawItem(i, culler))
					{
						GetWorldMatrix(ref instanceMartixData[start++]);
						count++;
					}

					PopWorldMatrix();
				}
			}
			else
			{
				for (int i = 0; i < instancesLength; i++)
					instanceMartixData[start++].Set(ref instances[i]);
				count = instancesLength;
			}

			return DrawBatch(buffer.Fill(count), indices, primitiveType, count);
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
		/// Draw an XNA vertex buffer, using state managment to reduce redundant state changes.
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
			ApplyRenderStateChanges();
#if DEBUG
			application.currentFrame.DrawXnaVerticesCount++;

			CalcBoundTextures();
#endif
			if (indices != null)
				this.IndexBuffer = indices;
			this.SetStream(0, vertices, streamOffset, vertexStride);
			this.VertexDeclaration = declaration;
			if (indices != null)
			{
#if DEBUG
				Application.currentFrame.DrawIndexedPrimitiveCallCount++;
#endif

				this.graphics.DrawIndexedPrimitives(primitiveType, baseVertex, 0, numVertices, startIndex, primitiveCount);
			}
			else
			{
#if DEBUG
				Application.currentFrame.DrawPrimitivesCallCount++;
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