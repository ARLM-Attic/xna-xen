
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
	/// Represents a 3D camera
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public class Camera3D : ICamera
	{
		private Projection proj;
		private Matrix camMatrix = Matrix.Identity;
		private Matrix viewMatrix = Matrix.Identity;
		private bool camMatChanged = true;
		private bool viewMatDirty = true;
		private int camMatIndex = 1;
		private bool reverseBFC;

		/// <summary>
		/// Construct a camera with the given projection, located at the given matrix
		/// </summary>
		/// <param name="projection"></param>
		/// <param name="cameraMatrix"></param>
		public Camera3D(Projection projection, Matrix cameraMatrix)
		{
			this.proj = projection;
			this.camMatrix = cameraMatrix;
		}
		/// <summary>
		/// Construct a camera with the given projection, using an identity matrix for it's position
		/// </summary>
		/// <param name="projection"></param>
		public Camera3D(Projection projection)
		{
			this.proj = projection;
			this.camMatrix = Matrix.Identity;
		}
		/// <summary>
		/// Construct a camera with a default camera projection, using an identity matrix for it's position
		/// </summary>
		public Camera3D()
		{
			this.proj = new Projection();
			this.camMatrix = Matrix.Identity;
		}


		/// <summary>
		/// Sets the <see cref="CameraMatrix"/> to a matrix that will make the camera look at a target
		/// </summary>
		/// <param name="cameraPosition"></param>
		/// <param name="lookAtTarget"></param>
		/// <param name="upVector"></param>
		/// <remarks>
		/// <para>Using <see cref="Matrix.CreateLookAt(Vector3,Vector3,Vector3)"/> is not recommended because it creats a View matrix, so it cannot be used for non-camera matrices. The <see cref="CameraMatrix"/> of a camera is the Inverse (<see cref="Matrix.Invert(Matrix)"/>) of the View Matrix (<see cref="ICamera.GetViewMatrix(out Matrix)"/>), so trying to set the camera matrix using Matrix.CreateLookAt will produce highly unexpected results.
		/// </para></remarks>
		public void LookAt(ref Vector3 lookAtTarget, ref Vector3 cameraPosition, ref Vector3 upVector)
		{
			Vector3 dir = cameraPosition - lookAtTarget;
			if (dir.LengthSquared() == 0)
				throw new ArgumentException("target and position are the same");
			dir.Normalize();
			Vector3 xaxis;

			Vector3.Cross(ref upVector, ref dir, out xaxis);
			xaxis.Normalize();

			Vector3.Cross(ref dir, ref xaxis, out upVector);

			this.camMatrix.M11 = xaxis.X;
			this.camMatrix.M12 = xaxis.Y;
			this.camMatrix.M13 = xaxis.Z;
			this.camMatrix.M14 = 0;

			this.camMatrix.M21 = upVector.X;
			this.camMatrix.M22 = upVector.Y;
			this.camMatrix.M23 = upVector.Z;
			this.camMatrix.M24 = 0;

			this.camMatrix.M31 = dir.X;
			this.camMatrix.M32 = dir.Y;
			this.camMatrix.M33 = dir.Z;
			this.camMatrix.M34 = 0;

			this.camMatrix.M41 = cameraPosition.X;
			this.camMatrix.M42 = cameraPosition.Y;
			this.camMatrix.M43 = cameraPosition.Z;
			this.camMatrix.M44 = 1;

			camMatChanged = true;
			viewMatDirty = true;
			camMatIndex = System.Threading.Interlocked.Increment(ref Camera2D.cameraMatrixBaseIndex);
		}

		/// <summary>
		/// Sets the <see cref="CameraMatrix"/> to a matrix that will make the camera look at a target
		/// </summary>
		/// <param name="cameraPosition"></param>
		/// <param name="lookAtTarget"></param>
		/// <param name="upVector"></param>
		public void LookAt(Vector3 lookAtTarget, Vector3 cameraPosition, Vector3 upVector)
		{
			LookAt(ref lookAtTarget, ref cameraPosition, ref upVector);
		}

		void ICamera.GetCameraHorizontalVerticalFov(out Vector2 v)
		{
			v = new Vector2(proj.GetHorizontalFov(), proj.GetVerticalFov());
		}

		bool ICamera.GetCameraHorizontalVerticalFov(ref Vector2 v, ref int changeIndex)
		{
			return proj.GetCameraHorizontalVerticalFov(ref v, ref changeIndex);
		}

		void ICamera.GetCameraHorizontalVerticalFovTangent(out Vector2 v)
		{
			v = new Vector2(proj.GetHorizontalFovTangent(), proj.GetVerticalFovTangent());
		}

		bool ICamera.GetCameraHorizontalVerticalFovTangent(ref Vector2 v, ref int changeIndex)
		{
			return proj.GetCameraHorizontalVerticalFovTangent(ref v, ref changeIndex);
		}

		void ICamera.GetCameraNearFarClip(out Vector2 v)
		{
			v = new Vector2(proj.NearClip, proj.FarClip);
		}
		bool ICamera.GetCameraNearFarClip(ref Vector2 v, ref int changeIndex)
		{
			return proj.GetCameraNearFarClip(ref v, ref changeIndex);
		}


		/// <summary>
		/// If true, the renderer will set the <see cref="Xen.Graphics.State.DepthColourCullState.CullMode"/> to the opposite value (unless set to None).
		/// </summary>
		public bool ReverseBackfaceCulling
		{
			get { return reverseBFC; }
			set { reverseBFC = value; }
		}

		bool ICamera.ReverseBackfaceCulling { get { return reverseBFC ^ this.proj.UseLeftHandedProjection; } }

		/// <summary>
		/// Get the cameras projection
		/// </summary>
		public Projection Projection
		{
			get { return proj; }
		}

		bool ICamera.GetProjectionMatrix(ref Matrix matrix, ref Vector2 drawTargetSize, ref int changeIndex)
		{
			return this.proj.GetProjectionMatrix(ref matrix, ref drawTargetSize, ref changeIndex);
		}

		bool ICamera.GetViewMatrix(ref Matrix matrix, ref int changeIndex)
		{
			if (changeIndex != camMatIndex)
			{
				changeIndex = camMatIndex;
				((ICamera)this).GetViewMatrix(out matrix);
				return true;
			}
			return false;
		}

		bool ICamera.GetCameraMatrix(ref Matrix matrix, ref int changeIndex)
		{
			if (changeIndex != camMatIndex)
			{
				changeIndex = camMatIndex;
				((ICamera)this).GetCameraMatrix(out matrix);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets/Sets the camera matrix. The preferred methods to use are <see cref="GetCameraMatrix(out Matrix)"/> and <see cref="SetCameraMatrix(ref Matrix)"/>
		/// </summary>
		public Matrix CameraMatrix
		{
			get { return camMatrix; }
			set { SetCameraMatrix(ref value); }
		}

		/// <summary>
		/// Sets the camera matrix
		/// </summary>
		/// <param name="value"></param>
		public void SetCameraMatrix(ref Matrix value)
		{
			if (AppState.MatrixNotEqual(ref camMatrix, ref value)) 
			{
				camMatrix = value; 
				camMatChanged = true; 
				viewMatDirty = true;
				camMatIndex = System.Threading.Interlocked.Increment(ref Camera2D.cameraMatrixBaseIndex);
			}
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
		/// Get the cull planes for this camera
		/// </summary>
		/// <returns></returns>
		public Plane[] GetCullingPlanes()
		{
			if (viewMatDirty)
			{
				Matrix.Invert(ref camMatrix, out viewMatrix);
				viewMatDirty = false;
			}
			Plane[] planes = proj.GetFrustumPlanes(ref viewMatrix, camMatChanged);
			camMatChanged = false;
			return planes;
		}

		void ICamera.GetProjectionMatrix(out Matrix matrix, ref Vector2 drawTargetSize)
		{
			this.proj.GetProjectionMatrix(out matrix, ref drawTargetSize);
		}
		/// <summary>
		/// Get the matrix for this camera
		/// </summary>
		/// <param name="matrix"></param>
		public void GetCameraMatrix(out Matrix matrix)
		{
			matrix = camMatrix;
		}
		/// <summary>
		/// Get the position of this camera
		/// </summary>
		/// <param name="viewPoint"></param>
		public void GetCameraPosition(out Vector3 viewPoint)
		{
			viewPoint = new Vector3(camMatrix.M41, camMatrix.M42, camMatrix.M43);
		}
		/// <summary>
		/// Get the view direction of this camera
		/// </summary>
		/// <param name="viewDirection"></param>
		public void GetCameraViewDirection(out Vector3 viewDirection)
		{
			viewDirection = new Vector3(-camMatrix.M31, -camMatrix.M32, -camMatrix.M33);
		}


		bool ICamera.GetCameraPosition(ref Vector3 viewPoint, ref int changeIndex)
		{
			if (changeIndex != camMatIndex)
			{
				viewPoint.X = camMatrix.M41;
				viewPoint.Y = camMatrix.M42;
				viewPoint.Z = camMatrix.M43;
				changeIndex = camMatIndex;
				return true;
			}
			return false;
		}

		bool ICamera.GetCameraViewDirection(ref Vector3 viewDirection, ref int changeIndex)
		{
			if (changeIndex != camMatIndex)
			{
				viewDirection.X = -camMatrix.M31;
				viewDirection.Y = -camMatrix.M32;
				viewDirection.Z = -camMatrix.M33;
				changeIndex = camMatIndex;
				return true;
			}
			return false;
		}

		void ICamera.GetViewMatrix(out Matrix matrix)
		{
			if (viewMatDirty)
			{
				Matrix.Invert(ref camMatrix, out viewMatrix);
				viewMatDirty = false;
			}
			matrix = viewMatrix;
		}

		/// <summary>
		/// Gets/Sets the position of this camera
		/// </summary>
		public Vector3 Position
		{
			get { return new Vector3(camMatrix.M41,camMatrix.M42,camMatrix.M43); }
			set 
			{ 
				if (camMatrix.M41 != value.X || camMatrix.M42 != value.Y || camMatrix.M43 != value.Z) 
				{
					camMatrix.M41 = value.X;
					camMatrix.M42 = value.Y;
					camMatrix.M43 = value.Z;
					
					camMatChanged = true; viewMatDirty = true;

					camMatIndex = System.Threading.Interlocked.Increment(ref Camera2D.cameraMatrixBaseIndex);
				} 
			}
		}
	}
}


