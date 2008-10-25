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
#if XBOX360
		bool AllocateForInstancing(DrawState state);
		int MaxInstances { get; }
#endif
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

#if XBOX360

		//instancing support

		static short[] copyBufferS;
		static ushort[] copyBufferUS;
		static int[] copyBufferI;
		static uint[] copyBufferUI;

		static object Length<T>(ref T[] array, int length)
		{
			int l = array == null ? 32 : array.Length;
			while (length > l)
				l *= 2;
			array = new T[l];
			return array;
		}

		static T[] Switch<T>(int length)
		{
			if (typeof(T) == typeof(ushort))
				return (T[])Length(ref copyBufferUS, length);
			if (typeof(T) == typeof(short))
				return (T[])Length(ref copyBufferS, length);
			if (typeof(T) == typeof(uint))
				return (T[])Length(ref copyBufferUI, length);
			if (typeof(T) == typeof(int))
				return (T[])Length(ref copyBufferI, length);
			throw new ArgumentException();
		}

		public static T[] BeginLock<T>(int length)
		{
			T[] array = Switch<T>(length);
			System.Threading.Monitor.Enter(array);
			return array;
		}
		public static void EndLock<T>(T[] array)
		{
			System.Threading.Monitor.Exit(array);
		}

		public static void CopyIncrement<T>(T[] array, T[] destination, int length, uint increment) where T : struct
		{
			CopyIncrement<T>((object)array, (object)destination, length, increment);
		}

		public static void CopyIncrementToInt<T>(T[] array, uint[] destination, int length, uint increment)
		{
			if (typeof(T) == typeof(ushort))
				CopyIncrementToIntType(array as ushort[], destination, length, increment);
			if (typeof(T) == typeof(short))
				CopyIncrementToIntType(array as short[], destination, length, increment);
		}
		static void CopyIncrementToIntType(ushort[] src, uint[] dst, int length, uint increment)
		{
			for (int i = 0; i < length; i++)
				dst[i] = (src[i] + increment);
		}
		static void CopyIncrementToIntType(short[] src, uint[] dst, int length, uint increment)
		{
			for (int i = 0; i < length; i++)
				dst[i] = (uint)(src[i] + increment);
		}

		static void CopyIncrement<T>(object array, object destination, int length, uint increment)
		{
			if (typeof(T) == typeof(ushort))
				CopyIncrementType((ushort[])array, (ushort[])destination, length, increment);
			if (typeof(T) == typeof(short))
				CopyIncrementType((short[])array, (short[])destination, length, increment);
			if (typeof(T) == typeof(uint))
				CopyIncrementType((uint[])array, (uint[])destination, length, increment);
			if (typeof(T) == typeof(int))
				CopyIncrementType((int[])array, (int[])destination, length, increment);
		}

		static void CopyIncrementType(ushort[] src, ushort[] dst, int length, uint increment)
		{
			for (int i = 0; i < length; i++)
				dst[i] = (ushort)(src[i] + increment);
		}
		static void CopyIncrementType(short[] src, short[] dst, int length, uint increment)
		{
			for (int i = 0; i < length; i++)
				dst[i] = (short)(src[i] + increment);
		}
		static void CopyIncrementType(uint[] src, uint[] dst, int length, uint increment)
		{
			for (int i = 0; i < length; i++)
				dst[i] = (src[i] + increment);
		}
		static void CopyIncrementType(int[] src, int[] dst, int length, uint increment)
		{
			for (int i = 0; i < length; i++)
				dst[i] = (int)(src[i] + increment);
		}


#endif
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

#if XBOX360
			public int instancingCount = 0;
			public bool convertToInt = false;
#endif

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
#if !XBOX360
				if (target is DynamicIndexBuffer)
				{
#if DEBUG
					state.Application.currentFrame.DynamicIndexBufferByesCopied += Stride * length;
#endif
					SetDataOptions setOp = SetDataOptions.None;
				
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

					((DynamicIndexBuffer)target).SetData(start, data, startIndex, length, setOp);
				}
				else
#endif
				{
#if DEBUG
#if XBOX360
					if (target is DynamicIndexBuffer)
						state.Application.currentFrame.DynamicIndexBufferByesCopied += Stride * length;
					else
#endif
						state.Application.currentFrame.IndexBufferByesCopied += Stride * length;
#endif
#if XBOX360
					int instancingCount = Math.Max(this.instancingCount, 1);

					//instancing requires data is duplicated over the index buffer multiple times, with an offset
					//added to each duplication
					if (instancingCount >= 1 && !convertToInt)
						((IndexBuffer)target).SetData(start, data, startIndex, length);

					if (instancingCount > 1)
					{
						int max = this.i_max;
						IndexBufferProcessor.Update(this, out i_min, out i_max);

						if (max == 0)
						{
							if (start != 0 || length * Stride != ((IndexBuffer)target).SizeInBytes / instancingCount / (convertToInt ? 2 : 1))
								throw new ArgumentException("XBOX Instancing error: Instancing requires the first index buffer write to copy the entire buffer");
						}
						else
						{
							if (this.i_max > max)
								throw new ArgumentException("XBOX Instancing error: Max index in the index buffer has increased from the first write");
						}

						//copy in multiple versions of the same indices
						if (convertToInt)
						{
							uint[] buffer = null;
							try
							{
								uint increment = (uint)(this.i_max+1);
								buffer = IndexBufferProcessor.BeginLock<uint>(length);

								//foreach instance copy, make a copy of the data.
								for (int i = 0; i < instancingCount; i++)
								{
									IndexBufferProcessor.CopyIncrementToInt(data, buffer, length, (uint)(increment * i));
									((IndexBuffer)target).SetData(start + length * 4 * i, buffer, startIndex, length);
								}
							}
							finally
							{
								IndexBufferProcessor.EndLock<uint>(buffer);
							}
						}
						else
						{
							IndexType[] buffer = null;
							try
							{
								uint increment = (uint)(this.i_max+1);
								buffer = IndexBufferProcessor.BeginLock<IndexType>(length);

								//foreach instance copy, make a copy of the data.
								for (int i = 1; i < instancingCount; i++)
								{
									IndexBufferProcessor.CopyIncrement(data, buffer, length, (uint)(increment * i));
									((IndexBuffer)target).SetData(start + length * Stride * i, buffer, startIndex, length);
								}
							}
							finally
							{
								IndexBufferProcessor.EndLock<IndexType>(buffer);
							}
						}

						IndexType[] da = new IndexType[((IndexBuffer)target).SizeInBytes / Stride];
						((IndexBuffer)target).GetData(da);

						da = null;
					}
#else
					((IndexBuffer)target).SetData(start, data, startIndex, length);
#endif

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

#if XBOX360

		int IDeviceIndexBuffer.MaxInstances
		{
			get { return buffer.instancingCount; }
		}

		bool IDeviceIndexBuffer.AllocateForInstancing(DrawState state)
		{
			if (buffer.instancingCount > 0)
				return buffer.instancingCount > 1;

			int instances = 128;
			if (this.buffer.Count > 100)
				instances = 64;
			if (this.buffer.Count > 2000)
				instances = 32;
			if (this.buffer.Count > 20000)
				instances = 16;
			if (this.buffer.Count > 175000)
				instances = 8;
			if (this.buffer.Count > 250000)
				instances = 4;
			if (this.buffer.Count > 300000)
				instances = 2;
			if (this.buffer.Count > 400000)
				instances = 1;

			if ((this.ResourceUsage & ResourceUsage.Dynamic) == ResourceUsage.Dynamic)
				instances = 1;

			bool convertToInt = false;

			if (instances > 1)
			{
				if (this.buffer.Stride == 2)
				{
					if (typeof(IndexType) == typeof(ushort))
					{
						if ((this.buffer.i_max+1) * instances > ushort.MaxValue)
						{
							if ((int)ushort.MaxValue / (this.buffer.i_max+1) >= 16)
								instances = (int)ushort.MaxValue / (this.buffer.i_max+1);
							else
								convertToInt = true;
						}
					}
					else
					{
						if (this.buffer.i_max * instances > short.MaxValue)
						{
							if ((int)short.MaxValue / (this.buffer.i_max+1) >= 16)
								instances = (int)short.MaxValue / (this.buffer.i_max+1);
							else
								convertToInt = true;
						}
					}
				}
				else
				{
					if ((this.buffer.i_max+1) * 64 > state.graphics.GraphicsDeviceCapabilities.MaxVertexIndex)
						instances = Math.Max(1, state.graphics.GraphicsDeviceCapabilities.MaxVertexIndex / (this.buffer.i_max+1));
				}
			}

			buffer.instancingCount = 1;

			if (instances > 1)
			{
				IEnumerable<IndexType> data = buffer.Data;
				
				if (ib != null)
				{
					if (data == null)
					{
						data = new IndexType[buffer.Count];
						ib.GetData<IndexType>((IndexType[])data);
					}
					ib.Dispose();
					ib = null;
				}
				if (data == null)
				{
					buffer.instancingCount = 1;
					return false;
				}

				buffer.Dispose();

				buffer = new Implementation(data);
				buffer.instancingCount = instances;
				buffer.convertToInt = convertToInt;
			}
			return buffer.instancingCount > 1;
		}

#endif

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
#if XBOX360
				BufferUsage bufferUsage = BufferUsage.None;
#else
				BufferUsage bufferUsage = BufferUsage.WriteOnly;
#endif
				if ((usage & ResourceUsage.Points) == ResourceUsage.Points)
					bufferUsage |= BufferUsage.Points;
				return bufferUsage;
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

		Type BufferIndexType
		{
			get 
			{ 
#if XBOX360
				if (buffer.convertToInt)
					return typeof(uint);
#endif
				return typeof(IndexType); 
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

				Type indexType = BufferIndexType;
				int instanceCount = 1;

#if XBOX360
				instanceCount = Math.Max(1, this.buffer.instancingCount);
#endif

				if (size == 0)
					throw new ArgumentException(string.Format("Indices<{0}> data size is zero", typeof(IndexType).Name));
				if ((usage & ResourceUsage.Dynamic) == ResourceUsage.Dynamic)
					ib = new DynamicIndexBuffer(device, indexType, size * instanceCount, Usage);
				else
					ib = new IndexBuffer(device, indexType, size * instanceCount, Usage);

				if ((ResourceUsage & ResourceUsage.DynamicSequential) != ResourceUsage.DynamicSequential)
				{
					int written = buffer.WriteBuffer(state, 0, size * buffer.Stride, ib, this);
					if (written < buffer.Count)
					{
						ib.Dispose();
						if ((usage & ResourceUsage.Dynamic) == ResourceUsage.Dynamic)
							ib = new DynamicIndexBuffer(device, indexType, buffer.Count * instanceCount, Usage);
						else
							ib = new IndexBuffer(device, indexType, buffer.Count * instanceCount, Usage);
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
				this.buffer.UpdateDirtyRegions(state, this.ib, this);

			return ib;
		}
	}

}