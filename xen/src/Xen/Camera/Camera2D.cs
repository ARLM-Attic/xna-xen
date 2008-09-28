
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xen.Input.State;
using Xen.Input;
#endregion

namespace Xen.Camera
{
	/// <summary>
	/// Simple Camera with no projection. May be normalised with <see cref="Camera2D.UseNormalisedCoordinates"/> for a range of [0,1]
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public class Camera2D : ICamera
	{
		private BoundingFrustum frustum = new BoundingFrustum(Matrix.Identity);
		private readonly Plane[] frustumPlanes = new Plane[6];
		private Matrix cameraMatrix = Matrix.Identity, viewMatrix = Matrix.Identity;
		private bool normalised = true;
		private bool dirty = true;
		private int rtWidth, rtHeight;
		private float rtWidthf, rtHeightf;
		private Vector2 bottomLeft, topRight;
		private bool frustumDirty = true;
		private bool viewMatrixDirty = true;
		private int cameraMatrixIndex = 1;
		internal static int cameraMatrixBaseIndex = 2;
		private bool reverseBFC;

		/// <summary></summary>
		public Camera2D()
		{
		}
		/// <summary>
		/// </summary>
		/// <param name="useNormalisedCoordinates">see <see cref="UseNormalisedCoordinates"/></param>
		public Camera2D(bool useNormalisedCoordinates)
		{
			this.normalised = useNormalisedCoordinates;
		}

		void ICamera.GetCameraHorizontalVerticalFov(out Vector2 v)
		{
			v = new Vector2(1, 1);
		}

		bool ICamera.GetCameraHorizontalVerticalFov(ref Vector2 v, ref int changeIndex)
		{
			if (changeIndex != 1)
			{
				v.X = 1;
				v.Y = 1;
				changeIndex = 1;
				return true;
			}
			return false;
		}

		void ICamera.GetCameraHorizontalVerticalFovTangent(out Vector2 v)
		{
			v = new Vector2(1, 1);
		}

		bool ICamera.GetCameraHorizontalVerticalFovTangent(ref Vector2 v, ref int changeIndex)
		{
			if (changeIndex != 1)
			{
				v.X = 1;
				v.Y = 1;
				changeIndex = 1;
				return true;
			}
			return false;
		}

		void ICamera.GetCameraNearFarClip(out Vector2 v)
		{
			v = new Vector2(0, 1);
		}
		bool ICamera.GetCameraNearFarClip(ref Vector2 v, ref int changeIndex)
		{
			if (changeIndex != 1)
			{
				v.X = 0;
				v.Y = 1;
				changeIndex = 1;
				return true;
			}
			return false;
		}

		/// <summary>
		/// If true, the renderer will set the <see cref="Xen.Graphics.State.DepthColourCullState.CullMode"/> to the opposite value (unless set to None).
		/// </summary>
		public bool ReverseBackfaceCulling
		{
			get { return reverseBFC; }
			set { reverseBFC = value; }
		}


		bool ICamera.GetProjectionMatrix(ref Matrix matrix, ref Vector2 drawTargetSize, ref int changeIndex)
		{
			if (changeIndex != 1)
			{
				changeIndex = 1;
				matrix = Matrix.Identity;
				return true;
			}
			return false;
		}

		bool ICamera.GetViewMatrix(ref Matrix matrix, ref int changeIndex)
		{
			if (changeIndex != cameraMatrixIndex)
			{
				changeIndex = cameraMatrixIndex;
				((ICamera)this).GetViewMatrix(out matrix);
				return true;
			}
			return false;
		}

		bool ICamera.GetCameraMatrix(ref Matrix matrix, ref int changeIndex)
		{
			if (changeIndex != cameraMatrixIndex)
			{
				changeIndex = cameraMatrixIndex;
				((ICamera)this).GetCameraMatrix(out matrix);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Get the matrix for this camera (<see cref="GetCameraMatrix(out Matrix)"/> is the preferred method to use)
		/// </summary>
		public Matrix CameraMatrix
		{
			get
			{
				if (dirty)
					BuildView(); 

				return cameraMatrix;
			}
		}

		/// <summary>
		/// Get the culling planes for this camera
		/// </summary>
		/// <returns></returns>
		public Plane[] GetCullingPlanes()
		{
			if (dirty)
				BuildView();

			if (frustumDirty)
			{
				UpdateFrustum();
			}

			return frustumPlanes;
		}

		private void UpdateFrustum()
		{
			if (viewMatrixDirty)
			{
				Matrix.Invert(ref cameraMatrix, out viewMatrix);
				viewMatrixDirty = false;
			}
			frustum.Matrix = viewMatrix;

			frustumPlanes[0] = frustum.Near;
			frustumPlanes[1] = frustum.Far;
			frustumPlanes[2] = frustum.Left;
			frustumPlanes[3] = frustum.Right;
			frustumPlanes[4] = frustum.Bottom;
			frustumPlanes[5] = frustum.Top;

			frustumDirty = false;
		}


		bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
			return FrustumCull.BoxInFrustum(GetCullingPlanes(), ref min, ref max, ref world, 0);
		}
		bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
			return FrustumCull.BoxInFrustum(GetCullingPlanes(), ref min, ref max, ref world, 0);
		}

		bool ICullPrimitive.TestWorldSphere(float radius, Vector3 position)
		{
			return FrustumCull.SphereInFrustum(GetCullingPlanes(), radius, ref position);
		}
		bool ICullPrimitive.TestWorldSphere(float radius, ref Vector3 position)
		{
			return FrustumCull.SphereInFrustum(GetCullingPlanes(), radius, ref position);
		}

		
		ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
			return FrustumCull.BoxIntersectsFrustum(GetCullingPlanes(), ref min, ref max, ref world, 0);
		}
		ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
			return FrustumCull.BoxIntersectsFrustum(GetCullingPlanes(), ref min, ref max, ref world, 0);
		}

		ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, Vector3 position)
		{
			return FrustumCull.SphereIntersectsFrustum(GetCullingPlanes(), radius, ref position);
		}

		ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, ref Vector3 position)
		{
			return FrustumCull.SphereIntersectsFrustum(GetCullingPlanes(), radius, ref position);
		}

		bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max)
		{
			return FrustumCull.AABBInFrustum(GetCullingPlanes(), ref min, ref max);
		}
		bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max)
		{
			return FrustumCull.AABBInFrustum(GetCullingPlanes(), ref min, ref max);
		}
		ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max)
		{
			return FrustumCull.AABBIntersectsFrustum(GetCullingPlanes(), ref min, ref max);
		}
		ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max)
		{
			return FrustumCull.AABBIntersectsFrustum(GetCullingPlanes(), ref min, ref max);
		}

		/// <summary>
		/// <para>When true, the bottom left corner is 0,0, the top right is 1,1.</para>
		/// <para>When false, the bottom left corner is 0,0 and the top right is the Width/Height of the render target</para>
		/// </summary>
		public bool UseNormalisedCoordinates
		{
			get { return normalised; }
			set { if (normalised != value) { SetDirty(); normalised = value; } }
		}
		/// <summary>
		/// Overridable, get the size of the view window. Call <see cref="SetDirty"/> to have this method recalled
		/// </summary>
		/// <param name="bottomLeft"></param>
		/// <param name="topRight"></param>
		protected virtual void GetView(out Vector2 bottomLeft, out Vector2 topRight)
		{
			bottomLeft = new Vector2();

			if (normalised)
			{
				topRight = new Vector2(1, 1);
			}
			else
			{
				topRight = new Vector2(rtWidthf, rtHeightf);
			}
		}

		/// <summary>
		/// Call this method to dirty the internal state of the camera/view matrices
		/// </summary>
		protected void SetDirty()
		{
			dirty = true;
			cameraMatrixIndex = System.Threading.Interlocked.Increment(ref cameraMatrixBaseIndex);
		}

		void ICamera.GetProjectionMatrix(out Matrix matrix, ref Vector2 drawTargetSize)
		{
			matrix = Matrix.Identity;
		}

		/// <summary>
		/// Gets the current camera matrix
		/// </summary>
		/// <param name="matrix"></param>
		public void GetCameraMatrix(out Matrix matrix)
		{
			if (dirty)
				BuildView();

			matrix = cameraMatrix;
		}

		void ICamera.GetViewMatrix(out Matrix matrix)
		{
			if (dirty)
				BuildView();
			if (viewMatrixDirty)
			{
				Matrix.Invert(ref cameraMatrix, out viewMatrix);
				viewMatrixDirty = false;
			}
			matrix = viewMatrix;
		}

		/// <summary>
		/// Get the position of the camera
		/// </summary>
		/// <param name="viewPoint"></param>
		public void GetCameraPosition(out Vector3 viewPoint)
		{
#if XBOX360
			viewPoint = new Vector3();
#endif
			if (dirty)
				BuildView();
			viewPoint.X = cameraMatrix.M41;
			viewPoint.Y = cameraMatrix.M42;
			viewPoint.Z = cameraMatrix.M43;
		}
		/// <summary>
		/// Get the normalised view direction of the camera
		/// </summary>
		/// <param name="viewDirection"></param>
		public void GetCameraViewDirection(out Vector3 viewDirection)
		{
#if XBOX360
			viewDirection = new Vector3();
#endif
			if (dirty)
				BuildView();
			viewDirection.X = -cameraMatrix.M31;
			viewDirection.Y = -cameraMatrix.M32;
			viewDirection.Z = -cameraMatrix.M33;
		}

		bool ICamera.GetCameraPosition(ref Vector3 viewPoint, ref int changeIndex)
		{
			if (dirty)
				BuildView();
			if (changeIndex != cameraMatrixIndex)
			{
				viewPoint.X = cameraMatrix.M41;
				viewPoint.Y = cameraMatrix.M42;
				viewPoint.Z = cameraMatrix.M43;
				changeIndex = cameraMatrixIndex;
				return true;
			}
			return false;
		}

		bool ICamera.GetCameraViewDirection(ref Vector3 viewDirection, ref int changeIndex)
		{
			if (dirty)
				BuildView();
			if (changeIndex != cameraMatrixIndex)
			{
				viewDirection.X = -cameraMatrix.M31;
				viewDirection.Y = -cameraMatrix.M32;
				viewDirection.Z = -cameraMatrix.M33;
				changeIndex = cameraMatrixIndex;
				return true;
			}
			return false;
		}

		void BuildView()
		{
			Vector2 bl, tr;
			GetView(out bl, out tr);

			if (bottomLeft != bl || topRight != tr)
			{
				Matrix mat = Matrix.Identity;
				Matrix.CreateScale(0.5f, 0.5f, 1, out mat);
				mat.M41 = 0.5f;
				mat.M42 = 0.5f;

				if (tr.X != 1 || tr.Y != 1 ||
					bl.X != 0 || bl.Y != 0)
				{
					Matrix mat2;
					Matrix.CreateScale((tr.X - bl.X), (tr.Y - bl.Y), 1, out mat2);
					mat2.M41 = bl.X;
					mat2.M42 = bl.Y;

					Matrix.Multiply(ref mat, ref mat2, out cameraMatrix);
				}
				else
				{
					cameraMatrix = mat;
				}

				dirty = false;
				viewMatrixDirty = true;
				frustumDirty = true;

				topRight = tr;
				bottomLeft = bl;
			}
		}

		internal void Begin(DrawState state)
		{
			int w = state.DrawTarget.Width;
			int h = state.DrawTarget.Height;

			dirty |= w != rtWidth;
			dirty |= h != rtHeight;

			rtWidth = w;
			rtHeight = h;
			rtHeightf = (float)h;
			rtWidthf = (float)w;
		}
	}
}


