using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xen.Ex.Scene
{
	/// <summary>
	/// Abstract base class for a scene partitioning class
	/// </summary>
	public abstract class StaticPartition : IDraw
	{
		private BoundsCalculatingPreCuller preCuller = new BoundsCalculatingPreCuller();
		private List<IDraw> addList = new List<IDraw>();
		private List<IDraw> drawList = new List<IDraw>();
		private bool firstDraw = true, processingActive = false;

		protected abstract void AddItem(IDraw item, ref Vector3 minBounds, ref Vector3 maxBounds);
		protected abstract void DrawItems(DrawState state);
		protected abstract bool CullTestItems(ICuller culler);
		private AABBToAABBPrimitive queryBox;
		private static Random random = new Random(0);

		/// <summary>
		/// Query the partition, returning all instances that intersect the primitive
		/// </summary>
		/// <param name="queryShape"></param>
		/// <param name="resultCallback">Callback for results. Return true to continue the query</param>
		protected abstract bool RunQuery(ICullPrimitive queryShape, Callback<bool, IDraw> resultCallback);

		/// <summary>
		/// <para>Query the partition, returning instances that may potentially intersect the primitive</para>
		/// </summary>
		/// <param name="queryShape"></param>
		/// <param name="resultCallback">Callback for results. Return true to continue the query</param>
		public void Query(ICullPrimitive queryShape, Callback<bool, IDraw> resultCallback)
		{
			if (RunQuery(queryBox, resultCallback))
				foreach (IDraw item in addList)
					if (!resultCallback(item))
						return;
		}

		/// <summary>
		/// Query the partition, returning instances that may potentially intersect the bounding box area
		/// </summary>
		/// <param name="boundsMax"></param>
		/// <param name="boundsMin"></param>
		/// <param name="resultCallback">Callback for results. Return true to continue the query</param>
		public void Query(ref Vector3 boundsMin, ref Vector3 boundsMax, Callback<bool, IDraw> resultCallback)
		{
			if (queryBox == null)
				queryBox = new AABBToAABBPrimitive();
			queryBox.boundsMax = boundsMax;
			queryBox.boundsMin = boundsMin;
			Query(queryBox, resultCallback);
		}

		/// <summary>
		/// Query the partition, returning instances that may potentially intersect the bounding box area
		/// </summary>
		/// <param name="boundsMax"></param>
		/// <param name="boundsMin"></param>
		/// <param name="resultCallback">Callback for results. Return true to continue the query</param>
		public void Query(Vector3 boundsMin, Vector3 boundsMax, Callback<bool, IDraw> resultCallback)
		{
			Query(ref boundsMin, ref boundsMax, resultCallback);
		}

		/// <summary>
		/// Add a drawable item to this partition
		/// </summary>
		/// <param name="item"></param>
		public void Add(IDraw item)
		{
			if (processingActive)
				this.drawList.Add(item);
			else
				this.addList.Add(item);
		}

		/// <summary></summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			DrawItems(state);

			//items need to be added to the partition
			if (addList.Count > 0)
			{
				processingActive = true;

				state.PushPreCuller(preCuller);
				Vector3 min,max;

				//add as many items as possible in a sortof-random order
				//most tree structures work best when the data that is added
				//is in as random order as possible.
				//for example, if all the items are in a straight line,
				//then some tree structures will be very badly unballanced

				//randomize the array
				for (int i = 0; i < addList.Count; i++)
				{
					//swap two random items
					int indexA = random.Next(addList.Count);
					int indexB = random.Next(addList.Count);

					IDraw itemA = addList[indexA];
					IDraw itemB = addList[indexB];

					addList[indexB] = itemA;
					addList[indexA] = itemB;
				}
				
				foreach (IDraw item in addList)
				{
					preCuller.BeginPreCullItem(state);

					if (item.CullTest(state))
						item.Draw(state);

					if (preCuller.TryGetBounds(out min, out max))
						AddItem(item, ref min, ref max);
					else
						drawList.Add(item);
				}
				addList.Clear();

				//reset the capacity on the first draw, 
				//as there are usually a lot of objects added in the first draw
				if (drawList.Count == 0)
				{
					if (firstDraw)
					{
						addList.Capacity = 4;
						drawList.Capacity = 4;
					}
					firstDraw = false;
				}

				if (drawList.Count>0)
					addList.AddRange(drawList);
				drawList.Clear();

				state.PopPreCuller();

				processingActive = false;
			}
		}

		/// <summary></summary>
		/// <param name="culler"></param>
		/// <returns></returns>
		public bool CullTest(ICuller culler)
		{
			return addList.Count > 0 || CullTestItems(culler);
		}
	}


	//ICullPrimitive for performing box querys on partitions
	class AABBToAABBPrimitive : ICullPrimitive
	{
		public Vector3 boundsMin, boundsMax;

		public bool TestWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
			throw new NotSupportedException();
		}

		public bool TestWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
			throw new NotSupportedException();
		}

		public bool TestWorldBox(ref Vector3 min, ref Vector3 max)
		{
			return !(
				max.X < boundsMin.X ||
				max.Y < boundsMin.Y ||
				max.Z < boundsMin.Z ||

				min.X > boundsMax.X ||
				min.Y > boundsMax.Y ||
				min.Z > boundsMax.Z);
		}

		public bool TestWorldBox(Vector3 min, Vector3 max)
		{
			return TestWorldBox(ref min, ref max);
		}

		public bool TestWorldSphere(float radius, ref Vector3 position)
		{
			return !(
				position.X + radius < boundsMin.X ||
				position.Y + radius < boundsMin.Y ||
				position.Z + radius < boundsMin.Z ||

				position.X - radius > boundsMax.X ||
				position.Y - radius > boundsMax.Y ||
				position.Z - radius > boundsMax.Z);
		}

		public bool TestWorldSphere(float radius, Vector3 position)
		{
			return TestWorldSphere(radius, ref position);
		}

		public ContainmentType IntersectWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
			throw new NotSupportedException();
		}

		public ContainmentType IntersectWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
			throw new NotSupportedException();
		}

		public ContainmentType IntersectWorldBox(ref Vector3 min, ref Vector3 max)
		{
			if (!(
				max.X < boundsMin.X ||
				max.Y < boundsMin.Y ||
				max.Z < boundsMin.Z ||

				min.X > boundsMax.X ||
				min.Y > boundsMax.Y ||
				min.Z > boundsMax.Z))
			{
				if (
					max.X <= boundsMin.X &&
					max.Y <= boundsMin.Y &&
					max.Z <= boundsMin.Z &&

					min.X >= boundsMax.X &&
					min.Y >= boundsMax.Y &&
					min.Z >= boundsMax.Z)
					return ContainmentType.Contains;
				else
					return ContainmentType.Intersects;
			}
			return ContainmentType.Disjoint;
		}

		public ContainmentType IntersectWorldBox(Vector3 min, Vector3 max)
		{
			return IntersectWorldBox(ref min, ref max);
		}

		public ContainmentType IntersectWorldSphere(float radius, ref Vector3 position)
		{
			if (!(
				position.X + radius < boundsMin.X ||
				position.Y + radius < boundsMin.Y ||
				position.Z + radius < boundsMin.Z ||

				position.X - radius > boundsMax.X ||
				position.Y - radius > boundsMax.Y ||
				position.Z - radius > boundsMax.Z))
			{
				if (
					position.X + radius <= boundsMin.X &&
					position.Y + radius <= boundsMin.Y &&
					position.Z + radius <= boundsMin.Z &&

					position.X - radius >= boundsMax.X &&
					position.Y - radius >= boundsMax.Y &&
					position.Z - radius >= boundsMax.Z)
					return ContainmentType.Contains;
				else
					return ContainmentType.Intersects;
			}
			return ContainmentType.Disjoint;
		}

		public ContainmentType IntersectWorldSphere(float radius, Vector3 position)
		{
			return IntersectWorldSphere(radius, ref position);
		}
	}

	/// <summary>
	/// <para>When this class is used as a preculler, it will compute the cull bounds of the cull test calls made when drawing</para>
	/// </summary>
	public class BoundsCalculatingPreCuller : ICullPrimitive
	{
		private Matrix matrix;
		private bool isIdentity;
		private float scale;
		private Vector3 minBound, maxBound;
		private bool reset;

		/// <summary>
		/// Call this method before drawing/culling an item. Match this method call with a call to TryGetBounds after the cull/draw is complete
		/// </summary>
		/// <param name="state"></param>
		public void BeginPreCullItem(DrawState state)
		{
			this.reset = true;
			state.GetWorldMatrix(out matrix);
			isIdentity = matrix == Matrix.Identity;
			scale = 1;

			minBound = new Vector3();
			maxBound = new Vector3();

			if (!isIdentity)
			{
				Matrix.Invert(ref matrix, out matrix);

				scale = (float)Math.Sqrt(
							Math.Max(Math.Max(							
								(matrix.M11 * matrix.M11 + matrix.M21 * matrix.M21 + matrix.M31 * matrix.M31),
								(matrix.M12 * matrix.M12 + matrix.M22 * matrix.M22 + matrix.M32 * matrix.M32)),
								(matrix.M13 * matrix.M13 + matrix.M23 * matrix.M23 + matrix.M33 * matrix.M33)));
			}
		}

		/// <summary>
		/// <para>Call this method to get the computed bounds. Call after culling/drawing an item. Call <see cref="BeginPreCullItem"/> before culling/drawing the item.</para>
		/// <para>Returns false if the bounds could not be determined</para>
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public bool TryGetBounds(out Vector3 min, out Vector3 max)
		{
			min = minBound;
			max = maxBound;

			return !reset;
		}

		void UpdateBounds(ref Vector3 point)
		{
			if (!isIdentity)
				Vector3.Transform(ref point, ref matrix, out point);

			if (reset)
			{
				minBound = point;
				maxBound = point;
				reset = false;
				return;
			}

			minBound.X = Math.Min(minBound.X, point.X);
			minBound.Y = Math.Min(minBound.Y, point.Y);
			minBound.Z = Math.Min(minBound.Z, point.Z);

			maxBound.X = Math.Max(maxBound.X, point.X);
			maxBound.Y = Math.Max(maxBound.Y, point.Y);
			maxBound.Z = Math.Max(maxBound.Z, point.Z);
		}

		void UpdateBounds(ref Vector3 point, float radius)
		{
			if (!isIdentity)
				Vector3.Transform(ref point, ref matrix, out point);
			radius *= scale;

			if (reset)
			{
				minBound.X = point.X - radius;
				minBound.Y = point.Y - radius;
				minBound.Z = point.Z - radius;

				maxBound.X = point.X + radius;
				maxBound.Y = point.Y + radius;
				maxBound.Z = point.Z + radius;

				reset = false;
				return;
			}

			minBound.X = Math.Min(minBound.X, point.X - radius);
			minBound.Y = Math.Min(minBound.Y, point.Y - radius);
			minBound.Z = Math.Min(minBound.Z, point.Z - radius);

			maxBound.X = Math.Max(maxBound.X, point.X + radius);
			maxBound.Y = Math.Max(maxBound.Y, point.Y + radius);
			maxBound.Z = Math.Max(maxBound.Z, point.Z + radius);
		}

		//returning true or ContainmentType.Contains means this pre-culler failed to cull
		//the input, so the next pre-culler is tried, or the main on-screen culler is tried.
		//in any event, the bounds can be determined by the cull process

		bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
			Vector3 v = new Vector3();

			for (int x = 0; x < 2; x++)
			for (int y = 0; y < 2; y++)
			for (int z = 0; z < 2; z++)
			{
				v.X = x == 0 ? min.X : max.X;
				v.Y = y == 0 ? min.Y : max.Y;
				v.Z = z == 0 ? min.Z : max.Z;

				Vector3.Transform(ref v, ref world, out v);
				UpdateBounds(ref v);
			}

			return true;
		}

		bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
			return ((ICullPrimitive)this).TestWorldBox(ref min, ref max, ref world);
		}

		bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max)
		{
			Vector3 v = new Vector3();

			for (int x = 0; x < 2; x++)
			for (int y = 0; y < 2; y++)
			for (int z = 0; z < 2; z++)
			{
				v.X = x == 0 ? min.X : max.X;
				v.Y = y == 0 ? min.Y : max.Y;
				v.Z = z == 0 ? min.Z : max.Z;

				UpdateBounds(ref v);
			}

			return true;
		}

		bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max)
		{
			return ((ICullPrimitive)this).TestWorldBox(ref min, ref max);
		}

		bool ICullPrimitive.TestWorldSphere(float radius, ref Vector3 position)
		{
			UpdateBounds(ref position, radius);
			return true;
		}

		bool ICullPrimitive.TestWorldSphere(float radius, Vector3 position)
		{
			UpdateBounds(ref position, radius);
			return true;
		}

		ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
			((ICullPrimitive)this).TestWorldBox(ref min, ref max, ref world);
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
			((ICullPrimitive)this).TestWorldBox(ref min, ref max, ref world);
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max)
		{
			((ICullPrimitive)this).TestWorldBox(ref min, ref max);
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max)
		{
			((ICullPrimitive)this).TestWorldBox(ref min, ref max);
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, ref Vector3 position)
		{
			UpdateBounds(ref position, radius);
			return ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, Vector3 position)
		{
			UpdateBounds(ref position, radius);
			return ContainmentType.Contains;
		}
	}
}
