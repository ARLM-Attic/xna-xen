
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
	/// Interface to a Camera
	/// </summary>
	public interface ICamera : ICullPrimitive
	{
		/// <summary>
		/// Gets the view matrix for this camera
		/// </summary>
		/// <remarks>The view matrix is the <see cref="Matrix.Invert(Matrix)">inverse</see> of the <see cref="GetCameraMatrix(out Matrix)">camera matrix</see></remarks>
		/// <param name="matrix"></param>
		void GetViewMatrix(out Matrix matrix);
		/// <summary>
		/// Gets the matrix for this camera's position/rotation
		/// </summary>
		/// <remarks>The <see cref="GetViewMatrix(out Matrix)">view matrix</see> is the <see cref="Matrix.Invert(Matrix)">inverse</see> of the camera matrix</remarks>
		/// <param name="matrix"></param>
		void GetCameraMatrix(out Matrix matrix);

		/// <summary>
		/// If true, the renderer will set the <see cref="Xen.Graphics.State.DepthColourCullState.CullMode"/> to the opposite value (unless set to None).
		/// </summary>
		bool ReverseBackfaceCulling { get;}

		/// <summary>
		/// Gets the projection matrix for this camera, with a change index. Returns true if the matrix has changed
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="changeIndex"></param>
		/// <param name="drawTargetSize">Size of the current draw target (some projections may automatically calculate aspect ratio based on the size of the draw target)</param>
		/// <returns>true if the matrix has changed</returns>
		bool GetProjectionMatrix(ref Matrix matrix, ref Vector2 drawTargetSize, ref int changeIndex);
		/// <summary>
		/// Gets the projection matrix for this camera
		/// </summary>
		/// <param name="drawTargetSize">Size of the current draw target (some projections may automatically calculate aspect ratio based on the size of the draw target)</param>
		/// <param name="matrix"></param>
		void GetProjectionMatrix(out Matrix matrix, ref Vector2 drawTargetSize);
		/// <summary>
		/// Gets the view matrix for this camera, with a change index. Returns true if the matrix has changed
		/// </summary>
		/// <remarks>The view matrix is the <see cref="Matrix.Invert(Matrix)">inverse</see> of the <see cref="GetCameraMatrix(out Matrix)">camera matrix</see></remarks>
		/// <param name="matrix"></param>
		/// <param name="changeIndex"></param>
		/// <returns>true if the matrix has changed</returns>
		bool GetViewMatrix(ref Matrix matrix, ref int changeIndex);
		/// <summary>
		/// Gets the matrix for this camera's position/rotation, with a change index. Returns true if the matrix has changed
		/// </summary>
		/// <remarks>The <see cref="GetViewMatrix(out Matrix)">view matrix</see> is the <see cref="Matrix.Invert(Matrix)">inverse</see> of the camera matrix</remarks>
		/// <param name="matrix"></param>
		/// <param name="changeIndex"></param>
		/// <returns>true if the matrix has changed</returns>
		bool GetCameraMatrix(ref Matrix matrix, ref int changeIndex);

		/// <summary>
		/// Gets the current position of the camera
		/// </summary>
		/// <param name="viewPoint"></param>
		void GetCameraPosition(out Vector3 viewPoint);
		/// <summary>
		/// Gets the normalised view direction of the camera
		/// </summary>
		/// <param name="viewDirection"></param>
		void GetCameraViewDirection(out Vector3 viewDirection);

		/// <summary>
		/// Gets the position of the camera, if it has changed according to the changeIndex
		/// </summary>
		/// <param name="viewPoint"></param>
		/// <param name="changeIndex"></param>
		/// <returns></returns>
		bool GetCameraPosition(ref Vector3 viewPoint, ref int changeIndex);
		/// <summary>
		/// Gets the normalised view direction of the camera, if it has changed according to the changeIndex
		/// </summary>
		/// <param name="viewDirection"></param>
		/// <param name="changeIndex"></param>
		/// <returns></returns>
		bool GetCameraViewDirection(ref Vector3 viewDirection, ref int changeIndex);

		/// <summary>
		/// Gets the near/far clip plane distances of the camera as a vector
		/// </summary>
		/// <param name="nearFarClip"></param>
		void GetCameraNearFarClip(out Vector2 nearFarClip);
		/// <summary>
		/// Gets the near/far clip plane distances of the camera as a vector, if it has changed according to the changeIndex
		/// </summary>
		/// <param name="changeIndex"></param>
		/// <param name="nearFarClip"></param>
		bool GetCameraNearFarClip(ref Vector2 nearFarClip, ref int changeIndex);

		/// <summary>
		/// Gets the horizontal/vertical field of view of the camera as a vector
		/// </summary>
		/// <param name="hvFov"></param>
		void GetCameraHorizontalVerticalFov(out Vector2 hvFov);
		/// <summary>
		/// Gets the horizontal/vertical field of view of the camera as a vector, if it has changed according to the changeIndex
		/// </summary>
		/// <param name="changeIndex"></param>
		/// <param name="hvFov"></param>
		bool GetCameraHorizontalVerticalFov(ref Vector2 hvFov, ref int changeIndex);
		/// <summary>
		/// Gets the tangent of the horizontal/vertical field of view of the camera as a vector
		/// </summary>
		/// <param name="hvFovTan"></param>
		void GetCameraHorizontalVerticalFovTangent(out Vector2 hvFovTan);
		/// <summary>
		/// Gets the tangent of the horizontal/vertical field of view of the camera as a vector, if it has changed according to the changeIndex
		/// </summary>
		/// <param name="changeIndex"></param>
		/// <param name="hvFovTan"></param>
		bool GetCameraHorizontalVerticalFovTangent(ref Vector2 hvFovTan, ref int changeIndex);

		/// <summary>
		/// Get the culling planes for this camera
		/// </summary>
		/// <returns></returns>
		Plane[] GetCullingPlanes();
	}
}


