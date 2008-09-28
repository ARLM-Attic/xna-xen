using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Xen.Ex
{
	/// <summary>
	/// A structure that can be used to do bit casting between primitve types
	/// </summary>
	/// <remarks>
	/// <para>For example, to convert an int to a float:</para>
	/// <example>
	/// <code>
	/// BitCast cast = new BitCast();
	/// 
	/// //...
	/// 
	/// cast.Int32 = 12345;
	/// float value = cast.Single;
	/// </code>
	/// </example>
	/// <para>The same can be done with Bytes, using <see cref="Byte0"/> to <see cref="Byte7"/></para>
	/// </remarks>
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct BitCast
	{
		//big/little endian differences
#if XBOX360
		
		[FieldOffset(4)]
		public int Int32;
		[FieldOffset(4)]
		public uint UInt32;
		[FieldOffset(6)]
		public ushort UInt16;
		[FieldOffset(6)]
		public short Int16;
		[FieldOffset(4)]
		public float Single;
		[FieldOffset(0)]
		public double Double;
		[FieldOffset(0)]
		public ulong UInt64;
		[FieldOffset(0)]
		public long Int64;


		[FieldOffset(7)]
		public byte Byte0;
		[FieldOffset(6)]
		public byte Byte1;
		[FieldOffset(5)]
		public byte Byte2;
		[FieldOffset(4)]
		public byte Byte3;
		[FieldOffset(3)]
		public byte Byte4;
		[FieldOffset(2)]
		public byte Byte5;
		[FieldOffset(1)]
		public byte Byte6;
		[FieldOffset(0)]
		public byte Byte7;
#else

		[FieldOffset(0)]
		public int Int32;
		[FieldOffset(0)]
		public uint UInt32;
		[FieldOffset(0)]
		public ushort UInt16;
		[FieldOffset(0)]
		public short Int16;
		[FieldOffset(0)]
		public float Single;
		[FieldOffset(0)]
		public double Double;
		[FieldOffset(0)]
		public ulong UInt64;
		[FieldOffset(0)]
		public long Int64;


		[FieldOffset(0)]
		public byte Byte0;
		[FieldOffset(1)]
		public byte Byte1;
		[FieldOffset(2)]
		public byte Byte2;
		[FieldOffset(3)]
		public byte Byte3;
		[FieldOffset(4)]
		public byte Byte4;
		[FieldOffset(5)]
		public byte Byte5;
		[FieldOffset(6)]
		public byte Byte6;
		[FieldOffset(7)]
		public byte Byte7;
#endif
	}

	/// <summary>
	/// Wraps an array as a readonly IList collection, and provides a struct based enumerator (reduces garbage collection and need for 'xxxCollection' classes)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct ReadOnlyArrayCollection<T> : IList<T>
	{
		readonly T[] array;
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="array"></param>
		public ReadOnlyArrayCollection(T[] array)
		{
			this.array = array;
		}

		/// <summary>
		/// struct enumerator
		/// </summary>
		public struct ArrayEnumerator : IEnumerator<T>
		{
			readonly T[] array;
			int index;

			internal ArrayEnumerator(T[] array)
			{
				this.array = array;
				this.index = -1;
			}

			public T Current
			{
				get { return array[index]; }
			}

			public void Dispose()
			{
				index = -1;
			}

			object System.Collections.IEnumerator.Current
			{
				get { return array[index]; }
			}

			public bool MoveNext()
			{
				return ++index != array.Length;
			}

			public void Reset()
			{
				index = -1;
			}
		}

		public int IndexOf(T item)
		{
			return ((IList<T>)array).IndexOf(item);
		}

		void IList<T>.Insert(int index, T item)
		{
			((IList<T>)array).Insert(index, item);
		}

		void IList<T>.RemoveAt(int index)
		{
			((IList<T>)array).RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				return array[index];
			}
			set
			{
				throw new InvalidOperationException("readonly");
			}
		}


		void ICollection<T>.Add(T item)
		{
			((ICollection<T>)array).Add(item);
		}

		void ICollection<T>.Clear()
		{
			((ICollection<T>)array).Clear();
		}

		public bool Contains(T item)
		{
			return ((ICollection<T>)array).Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.array.CopyTo(array, arrayIndex);
		}

		int ICollection<T>.Count
		{
			get { return array.Length; }
		}
		public int Length
		{
			get { return array.Length; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		bool ICollection<T>.Remove(T item)
		{
			return ((IList<T>)array).Remove(item);
		}

		ArrayEnumerator GetEnumerator()
		{
			return new ArrayEnumerator(array);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T[] ToArray()
		{
			return (T[])array.Clone();
		}
	}


	/// <summary>
	/// This class wraps a <see cref="StringBuilder"/>, providing an integer count that increments each time the string changes (A change index)
	/// </summary>
	public sealed class TextValue
	{
		internal StringBuilder value = new StringBuilder();
		private int changeIndex;

		/// <summary>
		/// True if the text has changed (change index mismatch)
		/// </summary>
		/// <param name="changeIndex"></param>
		/// <returns></returns>
		public bool HasChanged(ref int changeIndex)
		{
			if (this.changeIndex != changeIndex)
			{
				changeIndex = this.changeIndex;
				return true;
			}
			return false;
		}

		public static implicit operator string(TextValue value)
		{
			return value.value.ToString();
		}

		/// <summary>
		/// Length of the string
		/// </summary>
		public int Length
		{
			get
			{
				return value.Length;
			}
		}

		/// <summary>
		/// Clears the string
		/// </summary>
		public void Clear()
		{
			if (value.Length != 0)
			{
				value.Length = 0;
				changeIndex++;
			}
		}
		public void SetText(string value) 
		{
			this.value.Length = 0; this.value.Append(value); changeIndex++; 
		}
		public void SetText(int value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(float value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(double value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(short value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(ushort value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(uint value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(long value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(ulong value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(byte value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(sbyte value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(bool value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(char value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(char[] value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }
		public void SetText(object value) { this.value.Length = 0; this.value.Append(value); changeIndex++; }

		public void Append(string value) 
		{
			this.value.Append(value); changeIndex++; 
		}
		public void Append(int value) { this.value.Append(value); changeIndex++; }
		public void Append(float value) { this.value.Append(value); changeIndex++; }
		public void Append(double value) { this.value.Append(value); changeIndex++; }
		public void Append(short value) { this.value.Append(value); changeIndex++; }
		public void Append(ushort value) { this.value.Append(value); changeIndex++; }
		public void Append(uint value) { this.value.Append(value); changeIndex++; }
		public void Append(long value) { this.value.Append(value); changeIndex++; }
		public void Append(ulong value) { this.value.Append(value); changeIndex++; }
		public void Append(byte value) { this.value.Append(value); changeIndex++; }
		public void Append(sbyte value) { this.value.Append(value); changeIndex++; }
		public void Append(bool value) { this.value.Append(value); changeIndex++; }
		public void Append(char value) { this.value.Append(value); changeIndex++; }
		public void Append(char[] value) { this.value.Append(value); changeIndex++; }
		public void Append(object value) { this.value.Append(value); changeIndex++; }

		public void AppendLine(string value) { this.value.AppendLine(value); changeIndex++; }
		public void AppendLine() { this.value.AppendLine(); changeIndex++; }

		public static TextValue operator +(TextValue text, string value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, int value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, float value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, double value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, short value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, ushort value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, uint value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, long value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, ulong value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, byte value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, sbyte value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, bool value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, char value) { text.value.Append(value); text.changeIndex++; return text; }
		public static TextValue operator +(TextValue text, object value) { text.value.Append(value); text.changeIndex++; return text; }

		public override string ToString()
		{
			return value.ToString();
		}
	}

}
