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
	//this adds support for 'draw flags' to the DrawState
	//draw flags are custom elements that can be added to the draw state
	//these store totally custom data, usually enums
	public partial class DrawState
	{
		struct DrawFlag
		{
			public DrawFlag(Array stack)
			{
				this.stack = stack;
				this.index = 0;
			}
			public Array stack;
			public uint index;
		}

		private DrawFlag[] drawFlags = new DrawFlag[8];

		private static int flagCount = 0;
		private static object flagSync = new object();

		private static List<WeakReference> drawStateInstances = new List<WeakReference>();
		private static List<WeakReference> drawStateInstancesBuffer = new List<WeakReference>();
		private static List<Type> flagInstancesTypes = new List<Type>();

		private static object[] emptyObjectList = new object[0];
		private static Type[] singleEntryTypeList = new Type[1];

		private void InitDrawFlagCollection()
		{
			lock (flagSync)
			{
				drawStateInstances.Add(new WeakReference(this));

				for (int i = 0; i < flagCount; i++)
				{
					singleEntryTypeList[0] = flagInstancesTypes[i];
					MethodInfo createMethod = GetType().GetMethod("CreateDrawFlagArray", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(singleEntryTypeList);
					drawFlags[i] = new DrawFlag((Array)createMethod.Invoke(null, emptyObjectList));
				}
			}
		}

		/// <summary>
		/// <para>Push the custom Draw Flag (enum or struct) currently stoed by the DrawState</para>
		/// <para>Draw flags can be any value desired, usually an enum or small struct. They can be used to control draw logic</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void PushDrawFlag<T>() where T : struct
		{
			int index = DrawFlagID<T>.index;
			T[] array = (T[])drawFlags[index].stack;
			if (++drawFlags[index].index == array.Length)
			{
				Array.Resize(ref array, array.Length * 2);
				drawFlags[index].stack = array;
			}
			array[drawFlags[index].index] = array[drawFlags[index].index-1];
		}
		/// <summary>
		/// <para>Push a custom Draw Flag (enum or struct) onto the DrawState</para>
		/// <para>Draw flags can be any value desired, usually an enum or small struct. They can be used to control draw logic</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void PushDrawFlag<T>(T value) where T : struct
		{
			int index = DrawFlagID<T>.index;
			T[] array = (T[])drawFlags[index].stack;
			if (++drawFlags[index].index == array.Length)
			{
				Array.Resize(ref array, array.Length * 2);
				drawFlags[index].stack = array;
			}
			array[drawFlags[index].index] = value;
		}
		/// <summary>
		/// <para>Push a custom Draw Flag (enum or struct) onto the DrawState</para>
		/// <para>Draw flags can be any value desired, usually an enum or small struct. 
		/// They can be used to control draw logic</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void PushDrawFlag<T>(ref T value) where T : struct
		{
			int index = DrawFlagID<T>.index;
			T[] array = (T[])drawFlags[index].stack;
			if (++drawFlags[index].index == array.Length)
			{
				Array.Resize(ref array, array.Length * 2);
				drawFlags[index].stack = array;
			}
			array[drawFlags[index].index] = value;
		}
		/// <summary>
		/// <para>Pop a custom Draw Flag (enum or struct), stored by the DrawState</para>
		/// <para>Draw flags can be any value desired, usually an enum or small struct. 
		/// They can be used to control draw logic</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void PopDrawFlag<T>() where T : struct
		{
			checked { drawFlags[DrawFlagID<T>.index].index--; }
		}
		/// <summary>
		/// <para>Set a custom Draw Flag (enum or struct), stored by the DrawState</para>
		/// <para>Draw flags can be any value desired, usually an enum or small struct. 
		/// They can be used to control draw logic</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void SetDrawFlag<T>(T value) where T : struct
		{
			((T[])drawFlags[DrawFlagID<T>.index].stack)[drawFlags[DrawFlagID<T>.index].index] = value;
		}
		/// <summary>
		/// <para>Set a custom Draw Flag (enum or struct), stored by the DrawState</para>
		/// <para>Draw flags can be any value desired, usually an enum or small struct. 
		/// They can be used to control draw logic</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void SetDrawFlag<T>(ref T value) where T : struct
		{
			((T[])drawFlags[DrawFlagID<T>.index].stack)[drawFlags[DrawFlagID<T>.index].index] = value;
		}
		/// <summary>
		/// <para>Get a custom Draw Flag (enum or struct), stored by the DrawState</para>
		/// <para>Draw flags can be any value desired, usually an enum or small struct. 
		/// They can be used to control draw logic</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public T GetDrawFlag<T>() where T : struct
		{
			return ((T[])drawFlags[DrawFlagID<T>.index].stack)[drawFlags[DrawFlagID<T>.index].index];
		}
		/// <summary>
		/// <para>Get a custom Draw Flag (enum or struct), stored by the DrawState</para>
		/// <para>Draw flags can be any value desired, usually an enum or small struct. 
		/// They can be used to control draw logic</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void GetDrawFlag<T>(out T value) where T : struct
		{
			value = ((T[])drawFlags[DrawFlagID<T>.index].stack)[drawFlags[DrawFlagID<T>.index].index];
		}

		void DrawFlagAdded<T>() where T : struct
		{
			if (drawFlags.Length < flagCount)
				Array.Resize(ref drawFlags, drawFlags.Length * 2);
			drawFlags[flagCount-1] = new DrawFlag(CreateDrawFlagArray<T>());
		}

		static T[] CreateDrawFlagArray<T>() where T : struct
		{
			return new T[] { default(T), default(T) };
		}

		struct DrawFlagID<T> where T : struct
		{
			public static readonly int index;
			static DrawFlagID()
			{
				lock (flagSync)
				{
					index = flagCount++;
					flagInstancesTypes.Add(typeof(T));

					drawStateInstancesBuffer.AddRange(drawStateInstances);
					foreach (WeakReference wr in drawStateInstancesBuffer)
					{
						DrawState instance = wr.Target as DrawState;
						if (instance != null)
							instance.DrawFlagAdded<T>();
						else
							drawStateInstances.Remove(wr);
					}
					drawStateInstancesBuffer.Clear();
				}
			}
		}
	}
}
