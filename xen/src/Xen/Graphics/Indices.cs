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
	/// Interface to an index buffer. Stores an list of indexes to vertices.
	/// </summary>
	/// <remarks>
	/// <para>An example index buffer, to draw a Square from four vertices using a <see cref="PrimitiveType.TriangleList"/> (A list of triangles)
	/// the vertices would be the four corners, while the indices would be:</para>
	/// <para>0,1,2,0,2,3</para>
	/// <para>In this case (<see cref="PrimitiveType.TriangleList"/>), each set of 3 numbers index vertices to make a triangle. (0,1,2) is the first triangle, between the first, second and third vertex. While (0,2,3) is the second triangle, between the first, third and fourth (last) vertices. This creates a Square (or Quad).</para>
	/// </remarks>
	public interface IIndices : IDisposable
	{
		/// <summary>
		/// Number of indices in the buffer
		/// </summary>
		int Count { get;}
		/// <summary>
		/// True if the buffer stores 16bit indices (<see cref="System.UInt16">unsigned short</see> and <see cref="System.Int16">short</see>). 16bit numbers have a maximum value of 65535 and 32767 respectivly (see <see cref="System.UInt16.MaxValue"/> and <see cref="System.Int16.MaxValue"/>).
		/// </summary>
		bool Is16bit { get;}
		/// <summary>
		/// True if the stored value is signed (ie it can have negative values).
		/// </summary>
		/// <remarks>
		/// <para><see cref="System.Int32">int</see> and <see cref="System.Int16">short</see> are signed, <see cref="System.UInt32">unsigned int</see> and <see cref="System.UInt16">unsigned short</see> are not, but may contain larger numbers.</para>
		/// <para>Note that negatives values should almost always be avoided in index buffers.</para></remarks>
		bool IsSigned { get;}
		/// <summary>
		/// Maximum value stored in the index buffer, computed when the buffer is created
		/// </summary>
		int MinIndex { get;}
		/// <summary>
		/// Minimum value stored in the index buffer, computed when the buffer is created
		/// </summary>
		int MaxIndex { get;}
		/// <summary>
		/// Gets or Sets the resource usage of the buffer. Set to <see cref="ResourceUsage"/>.Dynamic to allow use of <see cref="SetDirtyRange"/>
		/// </summary>
		ResourceUsage ResourceUsage { get; set;}
		/// <summary>
		/// Tells the buffer that the source data it was created with has changed (Requires that <see cref="ResourceUsage"/> is set to <see cref="Xen.Graphics.ResourceUsage.Dynamic"/>)
		/// </summary>
		void SetDirty();
		/// <summary>
		/// Tells the buffer that the source data it was created with has changed in the specified range (Requires that <see cref="ResourceUsage"/> is set to <see cref="Xen.Graphics.ResourceUsage.Dynamic"/>)
		/// </summary>
		/// <param name="count">number of elements that should be updated</param>
		/// <param name="startIndex"></param>
		void SetDirtyRange(int startIndex, int count);
		/// <summary>
		/// Preload (warm) the resource before its first use
		/// </summary>
		/// <param name="state"></param>
		void Warm(DrawState state);
		/// <summary>
		/// Preload (warm) the resource before its first use
		/// </summary>
		/// <param name="application"></param>
		void Warm(Application application);
	}

	interface IDeviceIndexBuffer
	{
		IndexBuffer GetIndexBuffer(DrawState state);
	}


#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	static class IndexBufferProcessor
	{
		internal delegate void ProcessMinMax(object o_values, int count, object o_ib);

		internal static ProcessMinMax Method<T>()
		{
			if (typeof(T) == typeof(int))
				return ProcessMinMaxInt;
			if (typeof(T) == typeof(uint))
				return ProcessMinMaxUInt;
			if (typeof(T) == typeof(short))
				return ProcessMinMaxShort;
			if (typeof(T) == typeof(ushort))
				return ProcessMinMaxUShort;
			throw new ArgumentException();
		}

		public static void Init(object item)
		{
			if (item is Indices<int>.Implementation)
			{
				(item as Indices<int>.Implementation).min = int.MaxValue;
				return;
			}
			if (item is Indices<uint>.Implementation)
			{
				(item as Indices<uint>.Implementation).min = uint.MaxValue;
				return;
			}
			if (item is Indices<short>.Implementation)
			{
				(item as Indices<short>.Implementation).min = short.MaxValue;
				return;
			}
			if (item is Indices<ushort>.Implementation)
			{
				(item as Indices<ushort>.Implementation).min = ushort.MaxValue;
				return;
			}
			throw new ArgumentException();
		}

		public static void Update(object item, out int min, out int max)
		{
			if (item is Indices<int>.Implementation)
			{
				min = (int)(item as Indices<int>.Implementation).min;
				max = (int)(item as Indices<int>.Implementation).max;
				return;
			}
			if (item is Indices<uint>.Implementation)
			{
				min = (int)(item as Indices<uint>.Implementation).min;
				max = (int)(item as Indices<uint>.Implementation).max;
				return;
			}
			if (item is Indices<short>.Implementation)
			{
				min = (int)(item as Indices<short>.Implementation).min;
				max = (int)(item as Indices<short>.Implementation).max;
				return;
			}
			if (item is Indices<ushort>.Implementation)
			{
				min = (int)(item as Indices<ushort>.Implementation).min;
				max = (int)(item as Indices<ushort>.Implementation).max;
				return;
			}
			throw new ArgumentException();
		}

		static void ProcessMinMaxInt(object o_values, int count, object o_ib)
		{
			int[] values = (int[])o_values;
			Indices<int>.Implementation ib = (Indices<int>.Implementation)o_ib;
			if (ib.max == ib.min && ib.min == 0 && count != 0)
				ib.min = int.MaxValue;
			for (int i = 0; i < count; i++)
			{
				ib.min = Math.Min(values[i], ib.min);
				ib.max = Math.Max(values[i], ib.max);
			}
		}
		static void ProcessMinMaxUInt(object o_values, int count, object o_ib)
		{
			uint[] values = (uint[])o_values;
			Indices<uint>.Implementation ib = (Indices<uint>.Implementation)o_ib;
			if (ib.max == ib.min && ib.min == 0 && count != 0)
				ib.min = uint.MaxValue;
			for (int i = 0; i < count; i++)
			{
				ib.min = Math.Min(values[i], ib.min);
				ib.max = Math.Max(values[i], ib.max);
			}
		}
		static void ProcessMinMaxShort(object o_values, int count, object o_ib)
		{
			short[] values = (short[])o_values;
			Indices<short>.Implementation ib = (Indices<short>.Implementation)o_ib;
			if (ib.max == ib.min && ib.min == 0 && count != 0)
				ib.min = short.MaxValue;
			for (int i = 0; i < count; i++)
			{
				ib.min = Math.Min(values[i], ib.min);
				ib.max = Math.Max(values[i], ib.max);
			}
		}
		static void ProcessMinMaxUShort(object o_values, int count, object o_ib)
		{
			ushort[] values = (ushort[])o_values;
			Indices<ushort>.Implementation ib = (Indices<ushort>.Implementation)o_ib;
			if (ib.max == ib.min && ib.min == 0 && count != 0)
				ib.min = ushort.MaxValue;
			for (int i = 0; i < count; i++)
			{
				ib.min = Math.Min(values[i], ib.min);
				ib.max = Math.Max(values[i], ib.max);
			}
		}
	}

	/// <summary>
	/// Stores an list of indexes to vertices.
	/// </summary>
	/// <remarks>
	/// <para>An example index buffer, to draw a Square from four vertices using a <see cref="PrimitiveType.TriangleList"/> (A list of triangles)
	/// the vertices would be the four corners, while the indices would be:</para>
	/// <para>0,1,2,0,2,3</para>
	/// <para>In this case (<see cref="PrimitiveType.TriangleList"/>), each set of 3 numbers index vertices to make a triangle. (0,1,2) is the first triangle, between the first, second and third vertex. While (0,2,3) is the second triangle, between the first, third and fourth (last) vertices. This creates a Square (or Quad).</para>
	/// </remarks>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class Indices<IndexType> : Resource, IIndices, IDeviceIndexBuffer
		where IndexType : struct
	{
#if !DEBUG_API
		[System.Diagnostics.DebuggerStepThrough]
#endif
		internal sealed class Implementation : Bufffer<IndexType>
		{
			internal bool sequentialWriteFlag;
			static IndexBufferProcessor.ProcessMinMax processor = IndexBufferProcessor.Method<IndexType>();

			public IndexType min = default(IndexType), max = default(IndexType);
			public int i_min, i_max;
			public bool complete;
#if DEBUG
#if !XBOX360
			private int previousCopyEnd;
#endif
#endif

			public Implementation(IEnumerable<IndexType> vertices) : base(vertices)
			{
				IndexBufferProcessor.Init(this);
			}
			protected override void WriteBlock(DrawState state, IndexType[] data, int startIndex, int start, int length, object target, object parent)
			{
				state.UnbindBuffer((IndexBuffer)target);

				if (start == 0 && length == ((IndexBuffer)target).SizeInBytes)
				{
					min = default(IndexType);
					max = default(IndexType);
				}

				processor(data, length, this);

				if (target is DynamicIndexBuffer)
				{
#if DEBUG
					state.Application.currentFrame.DynamicIndexBufferByesCopied += Stride * length;
#endif
					SetDataOptions setOp = SetDataOptions.None;
					
#if !XBOX360
					if (sequentialWriteFlag)
					{
						if (start == 0)
							setOp = SetDataOptions.Discard;
						else
						{
							setOp = SetDataOptions.NoOverwrite;

#if DEBUG
							if (start < previousCopyEnd)
								throw new InvalidOperationException("ResourceUsage.DynamicSequential data overwrite detected");
#endif
						}
#if DEBUG
						previousCopyEnd = start + length * Stride;
#endif
					}
					else
						if (start == 0 && length * Stride == ((DynamicIndexBuffer)target).SizeInBytes)
							setOp = SetDataOptions.Discard;
#endif

					((DynamicIndexBuffer)target).SetData(start, data, startIndex, length, setOp);
				}
				else
				{
#if DEBUG
					state.Application.currentFrame.IndexBufferByesCopied += Stride * length;
#endif
					((IndexBuffer)target).SetData(start, data, startIndex, length);
				}
			}
			protected override void WriteComplete()
			{
				IndexBufferProcessor.Update(this, out i_min, out i_max);
				complete = true;
			}
		}
		private Implementation buffer;
		private IndexBuffer ib;
		private ResourceUsage usage;

		/// <summary>
		/// Constructs the index buffer with an array of values
		/// </summary>
		/// <param name="indices"></param>
		public Indices(params IndexType[] indices)
			: this(ResourceUsage.None, (IEnumerable<IndexType>)indices)
		{
		}
		/// <summary>
		/// Constructs the index buffer with a collection of values
		/// </summary>
		/// <param name="indices"></param>
		public Indices(IEnumerable<IndexType> indices)
			: this(ResourceUsage.None, indices)
		{
		}


		private Indices(ResourceUsage usage, IEnumerable<IndexType> indices)
		{

			Type indexType = typeof(IndexType);
			if (indexType != typeof(Int16) &&
				indexType != typeof(UInt16) &&
				indexType != typeof(Int32) &&
				indexType != typeof(UInt32))
				throw new ArgumentException("DataType for IndexBuffer<> must be of type int,uint,short or ushort");

			this.usage = usage;
			buffer = new Implementation(indices);
		}

		/// <summary>
		/// Disposes the graphics resources
		/// </summary>
		~Indices()
		{
			Dispose();
		}

		internal override int GetAllocatedDeviceBytes()
		{
			if (ib != null)
				return ib.SizeInBytes;
			return 0;
		}
		internal override int GetAllocatedManagedBytes()
		{
			int count = buffer != null ? buffer.GetCount() : 0;
			return Math.Max(0, count);
		}
		internal override ResourceType ResourceType
		{
			get { return Microsoft.Xna.Framework.Graphics.ResourceType.IndexBuffer; }
		}
		internal override bool InUse
		{
			get { return buffer != null && ib != null; }
		}
		internal override bool IsDisposed
		{
			get { return buffer == null && ib == null; }
		}

		int IIndices.MinIndex 
		{
			get { ValidateState(); return buffer.i_min; }
		}
		int IIndices.MaxIndex 
		{
			get { ValidateState(); return buffer.i_max; }
		}
		/// <summary>
		/// Minimum index value stored within the buffer
		/// </summary>
		public IndexType MinIndex { get { ValidateState(); return buffer.min; } }
		/// <summary>
		/// Maximum index value stored within the buffer
		/// </summary>
		public IndexType MaxIndex { get { ValidateState(); return buffer.max; } }

		void ValidateState()
		{
			ValidateDisposed();
				
			if (!buffer.complete)
				throw new ArgumentException("Value is not available until buffer has been processed");
		}

		bool IIndices.Is16bit
		{
			get
			{
				ValidateDisposed();
				return buffer.Stride == 2;
			}
		}

		bool IIndices.IsSigned
		{
			get
			{
				ValidateDisposed();
				return !(buffer.Type == typeof(uint) || buffer.Type == typeof(ushort));
			}
		}

		/// <summary>
		/// Call when source index data has changed and the index buffer requires updating.
		/// </summary>
		/// <remarks><see cref="ResourceUsage"/> must be set to <see cref="Graphics.ResourceUsage"/>.Dynamic</remarks>
		public void SetDirty()
		{
			ValidateDisposed();
			ValidateDirty();
			buffer.AddDirtyRange(0, this.buffer.Count, GetType(), true);
		}

		/// <summary>
		/// Call when source index data has changed within a known range, and the index buffer requires updating.
		/// </summary>
		/// <remarks><see cref="ResourceUsage"/> must be set to <see cref="Graphics.ResourceUsage"/>.Dynamic</remarks>
		public void SetDirtyRange(int startIndex, int count)
		{
			ValidateDisposed();
			ValidateDirty();
			buffer.AddDirtyRange(startIndex, count, GetType(), false);
		}


		void ValidateDirty()
		{
			if ((usage & ResourceUsage.Dynamic) == 0)
				throw new InvalidOperationException("this.ResourceUsage lacks ResourceUsage.Dynamic flag");
		}

		/// <summary>
		/// Number of indices stored within the buffer, -1 if not yet known
		/// </summary>
		public int Count
		{
			get
			{
				ValidateDisposed();
				return buffer.Count;
			}
		}

		internal override void Warm(Application application,GraphicsDevice device)
		{
			ValidateDisposed();
			if (this.ib == null)
				((IDeviceIndexBuffer)this).GetIndexBuffer(application.GetProtectedDrawState(device));
		}

		/// <summary>
		/// Disposes all graphics resources
		/// </summary>
		public void Dispose()
		{
			if (buffer != null)
			{
				buffer.Dispose();
				buffer = null;
			}
			if (ib != null)
			{
				ib.Dispose();
				ib = null;
			}
		}

		void ValidateDisposed()
		{
			if (buffer == null)
				throw new ObjectDisposedException("this");
		}

		BufferUsage Usage
		{
			get
			{
				BufferUsage b = BufferUsage.WriteOnly;
				if ((usage & ResourceUsage.Points) == ResourceUsage.Points)
					b |= BufferUsage.Points;
				return b;
			}
		}

		/// <summary>
		/// Gets/Sets the <see cref="ResourceUsage"/> of the indices
		/// </summary>
		/// <remarks>This value may only be set before the resource's first use</remarks>
		public ResourceUsage ResourceUsage 
		{
			get { return usage; }
			set
			{
				if (ib != null)
					throw new InvalidOperationException("Cannot set ResourceUsage when resource is in use");
				usage = value;
				this.buffer.sequentialWriteFlag = (value & ResourceUsage.DynamicSequential) == ResourceUsage.DynamicSequential;
			}
		}

		IndexBuffer IDeviceIndexBuffer.GetIndexBuffer(DrawState state)
		{
			GraphicsDevice device = state.graphics;
			ValidateDisposed();

			if (this.ib == null)
			{
				int size = 32;
				if (buffer.CountKnown)
					size = buffer.Count;
				if (size == 0)
					throw new ArgumentException(string.Format("Indices<{0}> data size is zero", typeof(IndexType).Name));
				if ((usage & ResourceUsage.Dynamic) == ResourceUsage.Dynamic)
					ib = new DynamicIndexBuffer(device, typeof(IndexType), size, Usage);
				else
					ib = new IndexBuffer(device, typeof(IndexType), size, Usage);

				if ((ResourceUsage & ResourceUsage.DynamicSequential) != ResourceUsage.DynamicSequential)
				{
					int written = buffer.WriteBuffer(state, 0, size * buffer.Stride, ib, this);
					if (written < buffer.Count)
					{
						ib.Dispose();
						if ((usage & ResourceUsage.Dynamic) == ResourceUsage.Dynamic)
							ib = new DynamicIndexBuffer(device, typeof(IndexType), buffer.Count, Usage);
						else
							ib = new IndexBuffer(device, typeof(IndexType), buffer.Count, Usage);
						buffer.WriteBuffer(state, 0, buffer.Count * buffer.Stride, ib, this);
					}
					this.buffer.ClearDirtyRange();
				}

				if ((usage & ResourceUsage.Dynamic) != ResourceUsage.Dynamic)
					buffer.ClearBuffer();
			}

			if ((usage & ResourceUsage.Dynamic) == ResourceUsage.Dynamic &&
				ib != null && ((DynamicIndexBuffer)ib).IsContentLost)
				SetDirty();

			if (this.buffer.IsDirty)
			{
				this.buffer.UpdateDirtyRegions(state, this.ib, this);
			}

			return ib;
		}
	}

}