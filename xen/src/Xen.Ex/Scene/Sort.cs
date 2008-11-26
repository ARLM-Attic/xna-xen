using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xen.Ex.Scene
{
	/// <summary>
	/// <para>This class will draw a list of objects in sorted order, drawing them in either back-to-front or front-to-back order</para>
	/// <para>NOTE: Sorting order is based on the CullTests performed by the items. Items that do not perform a CullTest through an <see cref="ICuller"/> will not be sorted</para>
	/// <para>NOTE: The post culler used by this class is somewhat inefficient on the Xbox right now - and may slow rendering down when heavily CPU limited. (This will be improved in a future version)</para>
	/// </summary>
	public sealed class DepthDrawSorter : IDraw
	{
		struct Entry
		{
			public float addIndex;
			public IDraw item;
		}

		private DepthSortMode sortMode;
		private int itemCount = 0;
		private float addCount = 0;
		private Entry[] items = new Entry[32];
		private float[] depths = new float[32];

		//actually used as a post-culler here
		private BoundsCalculatingPreCuller postCuller = new BoundsCalculatingPreCuller();

		public DepthDrawSorter(DepthSortMode sortMode)
		{
			this.sortMode = sortMode;
		}

		/// <summary>
		/// Gets/Sets the sorting mode for this drawer
		/// </summary>
		public DepthSortMode SortMode
		{
			get { return sortMode; }
			set { sortMode = value; }
		}

		/// <summary>
		/// Add an item to the sorter
		/// </summary>
		/// <param name="item"></param>
		public void Add(IDraw item)
		{
			if (item == null)
				throw new ArgumentNullException();

			if (itemCount == items.Length)
			{
				Array.Resize(ref items, items.Length * 2);
				Array.Resize(ref depths, depths.Length * 2);
			}

			items[itemCount++] = new Entry() { item = item, addIndex = this.addCount++ };
		}

		/// <summary>
		/// Removes all items from the sorter
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < itemCount; i++)
				items[i].item = null;
			itemCount = 0;
		}

		/// <summary>
		/// <para>Removes an item from the sorter (Performs a linear search of the sorter to find the item)</para>
		/// </summary>
		/// <param name="item"></param>
		public bool Remove(IDraw item)
		{
			for (int i = 0; i < itemCount; i++)
			{
				if (items[i].item == item)
				{
					items[i].item = null;

					itemCount--;

					if (i != itemCount)
					{
						//swap in the end item
						items[i] = items[itemCount];
						items[itemCount].item = null;
					}

					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Draw the items in sorted order
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			if (itemCount == 0)
				return;

			ICuller culler = state;

			state.PushPostCuller(postCuller);

			postCuller.BeginPreCullItem(state);

			int index = 0;
			int visible = 0;
			int backIndex = itemCount - 1;
			int backIndexCulled = itemCount - 1;
			int unsortedItems = 0;

			Vector3 min, max, dif, camera, direction;

			state.Camera.GetCameraPosition(out camera);
			state.Camera.GetCameraViewDirection(out direction);

			for (int i = 0; i < itemCount; i++)
			{
				Entry item = items[index];

				postCuller.ResetPreCullItem();

				bool cullTest = item.item.CullTest(culler);

				if (cullTest && postCuller.TryGetBounds(out min, out max))
				{
					//keep item in the list
					items[index] = items[visible];

					dif.X = max.X - min.X;
					dif.Y = max.Y - min.Y;
					dif.Z = max.Z - min.Z;

					//centre of cull tests
					min.X += dif.X * 0.5f - camera.X;
					min.Y += dif.Y * 0.5f - camera.Y;
					min.Z += dif.Z * 0.5f - camera.Z;

					depths[visible] = direction.X * min.X + direction.Y * min.Y + direction.Z * min.Z -
						Math.Max(dif.X,Math.Max(dif.Y,dif.Z))*0.5f;

					items[visible] = item;

					visible++;
					index++;
				}
				else
				{
					//swap the back culled element to this one, don't increment index
					items[index] = items[backIndexCulled];
					items[backIndexCulled] = item;
					backIndexCulled--;

					if (cullTest)
					{
						//as the last step, put this item at the very back.
						items[backIndexCulled + 1] = items[backIndex];
						items[backIndex] = item;
						depths[backIndex] = item.addIndex;

						backIndex--;
						unsortedItems++;
					}
				}
			}

			state.PopPostCuller();

			if (unsortedItems > 0)
			{
				backIndex++;

				//due to the way the algorithm works, the unsorted list is usually ordered like so:
				//1,2,3,4,5,6,7,0

				//so put the last element first, and check if they are out of order

				bool outOfOrder = false;
				float lastD = this.depths[this.itemCount - 1];
				Entry lastE = this.items[this.itemCount - 1];

				for (int i = this.itemCount - 2; i >= backIndex; i--)
				{
					if (i != this.itemCount - 2)
						outOfOrder |= this.depths[i] > this.depths[i + 1];

					this.items[i+1] = this.items[i];
					this.depths[i+1] = this.depths[i];
				}
				this.depths[backIndex] = lastD;
				this.items[backIndex] = lastE;

				//draw the unsorted items in their add order (which was written to depths)
				//this sort won't be all that efficient
				if (outOfOrder)
					Array.Sort(this.depths, this.items, backIndex, unsortedItems);

				for (int i = 0; i < unsortedItems; i++)
					items[backIndex++].item.Draw(state);
			}

			if (visible > 0)
			{
				//if the frame hasn't change, the items should already be in sorted order,
				//so this sort should be very fast

				//test if the values are already sorted...
				float depth = this.depths[0];
				bool soted = true;
				for (int i = 1; i < visible; i++)
				{
					if (depths[i] < depth)
					{
						soted = false;
						break;
					}
					depth = depths[i];
				}

				if (!soted)
					Array.Sort(this.depths, this.items, 0, visible);

				if (sortMode == DepthSortMode.FrontToBack)
				{
					for (int i = 0; i < visible; i++)
						items[i].item.Draw(state);
				}
				else
				{
					for (int i = visible - 1; i >= 0; i--)
						items[i].item.Draw(state);
				}
			}
		}

		bool ICullable.CullTest(ICuller culler)
		{
			return itemCount > 0;
		}
	}

	/// <summary>
	/// Sorting mode for a <see cref="DepthDrawSorter"/>
	/// </summary>
	public enum DepthSortMode
	{
		/// <summary>
		/// <para>Items are drawn back first, with the closest items drawn last</para>
		/// <para>Use this mode for effects such as alpha blending, that can produce different results when drawn in different orders</para>
		/// </summary>
		BackToFront,
		/// <summary>
		/// <para>Items are drawn front first, with the furthest items drawn last</para>
		/// <para>Use this mode for reducing overdraw, to improve performance of high-fill rate objects</para>
		/// </summary>
		FrontToBack
	}
}
