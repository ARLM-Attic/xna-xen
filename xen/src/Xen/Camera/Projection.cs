using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xen.Camera
{
	/// <summary>
	/// Class storing a <see cref="Camera3D"/> projection matrix. Maybe a Perspective or Orthographic project
	/// </summary>
	/// <remarks>This class may be extended to implement more complex forms of projection</remarks>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public class Projection
	{
		private readonly BoundingFrustum frustum = new BoundingFrustum(Matrix.Identity);
		private readonly Plane[] frustumPlanes = new Plane[6];
		private float near = 1;
		private float far = 1000.0f;
		private float fov = MathHelper.PiOver2;
		private Matrix mat;
		private bool set = false;
		private float? aspect = null;
		private float computedAspect;
		private Vector4 region = new Vector4(0, 0, 1, 1);
		private bool orthographic;
		private bool frustumDirty = true;
		private int changeIndex = 1;
		private static int changeBaseIndex = 2;
		private bool isExtendedClass;
		private bool leftHandedProjection;
		private bool debugPauseCullPlaneUpdates;


		/// <summary>
		/// XNA projection matrices are right handed whereas DirectX is left handed, which can sometimes cause unexpected projection with rendered cubemaps. Set to true to use left handed projection.
		/// </summary>
		public bool UseLeftHandedProjection
		{
			get { return leftHandedProjection; }
			set { if (leftHandedProjection != value) { leftHandedProjection = value; set = false; changeIndex = System.Threading.Interlocked.Increment(ref changeBaseIndex); } }
		}

		/// <summary>
		/// <para>(Use for scene Debugging)</para>
		/// <para>When true, this Projections frustum cull planes will not be updated.</para>
		/// <para>This can be used to 'pause' frustum culling, while still allowing the camera to move. This allows visual debugging of off screen culling.</para>
		/// </summary>
		public bool PauseFrustumCullPlaneUpdates
		{
			get { return debugPauseCullPlaneUpdates; }
			set { if (value != debugPauseCullPlaneUpdates) { debugPauseCullPlaneUpdates = value; set = false; changeIndex = System.Threading.Interlocked.Increment(ref changeBaseIndex); } }
		}

		/// <summary>
		/// Get the current projection matrix, if it has changed (compared to changeIndex)
		/// </summary>
		/// <param name="matrix">output projection matrix</param>
		/// <param name="changeIndex">change indexer</param>
		/// <param name="drawTargetSize">size of the draw target (input)</param>
		/// <returns>true if the matrix has changed and was returned</returns>
		public bool GetProjectionMatrix(ref Matrix matrix, ref Vector2 drawTargetSize, ref int changeIndex)
		{
			if (!aspect.HasValue && !orthographic && drawTargetSize.Y != 0)
			{
				float value = drawTargetSize.X / drawTargetSize.Y;
				if (computedAspect != value)
				{
					computedAspect = value;
					changeIndex = this.changeIndex - 1;
				}
			}

			if (changeIndex != this.changeIndex)
			{
				changeIndex = this.changeIndex;
				GetProjectionMatrix(out matrix,ref drawTargetSize);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets/Sets if this projection uses an orhographic projection. Change the <see cref="Region"/> to modify the othrographic area
		/// </summary>
		public bool Orthographic
		{
			get { return orthographic; }
			set { if (orthographic != value) { orthographic = value; set = false; changeIndex = System.Threading.Interlocked.Increment(ref changeBaseIndex); } }
		}

		/// <summary>
		/// Fast copy projection settings from another projection object
		/// </summary>
		/// <param name="projection"></param>
		public void CopyFrom(Projection projection)
		{
			if (projection == null)
				throw new ArgumentNullException();

			this.aspect = projection.aspect;
			this.changeIndex = projection.changeIndex;
			this.computedAspect = projection.computedAspect;
			this.far = projection.far;
			this.fov = projection.fov;
			this.leftHandedProjection = projection.leftHandedProjection;
			this.mat = projection.mat;
			this.near = projection.near;
			this.orthographic = projection.orthographic;
			this.region = projection.region;

			this.set = false;
			this.changeIndex = System.Threading.Interlocked.Increment(ref changeBaseIndex);
		}

		internal BoundingFrustum GetFrustum(ref Matrix viewMatrix, bool viewChanged)
		{
			if (frustumDirty || !set || viewChanged)
			{
				if (!set)
				{
					SetProjection();
				}

				if (!debugPauseCullPlaneUpdates)
				{
					Matrix m;
					Matrix.Multiply(ref viewMatrix, ref mat, out m);
					frustum.Matrix = m;

					frustumPlanes[0] = frustum.Near;
					frustumPlanes[1] = frustum.Far;
					frustumPlanes[2] = frustum.Left;
					frustumPlanes[3] = frustum.Right;
					frustumPlanes[4] = frustum.Bottom;
					frustumPlanes[5] = frustum.Top;

					frustumDirty = false;
				}
			}

			return frustum; 
		}

		internal Plane[] GetFrustumPlanes(ref Matrix viewMatrix, bool viewChanged)
		{
			if (frustumDirty || !set || viewChanged)
			{
				if (!set)
				{
					SetProjection();
				}

				if (!debugPauseCullPlaneUpdates)
				{
					Matrix m;
					Matrix.Multiply(ref viewMatrix, ref mat, out m);
					frustum.Matrix = m;

					frustumPlanes[0] = frustum.Far;
					frustumPlanes[1] = frustum.Left;
					frustumPlanes[2] = frustum.Right;
					frustumPlanes[3] = frustum.Bottom;
					frustumPlanes[4] = frustum.Top;
					frustumPlanes[5] = frustum.Near;

					frustumDirty = false;
				}
			}

			return frustumPlanes;
		}

		/// <summary>
		/// Region the projection covers. Represented as TopLeft,BottomRight. The default value is 0,0,1,1
		/// </summary>
		public Vector4 Region
		{
			get { return region; }
			set { if (region != value) { region = value; set = false; changeIndex = System.Threading.Interlocked.Increment(ref changeBaseIndex); } }
		}

		/// <summary>
		/// Construct the default projection
		/// </summary>
		public Projection()
		{
			this.isExtendedClass = (this.GetType() != typeof(Projection));
		}
		/// <summary>
		/// Construct a projection
		/// </summary>
		/// <param name="fieldOfView">Field of view of the projection, in radians</param>
		/// <param name="nearPlane">Distance to the near clipping plane</param>
		/// <param name="farPlane">Distance to the far clipping plane</param>
		/// <param name="aspectRatio">Aspect ratio of the projection</param>
		public Projection(float fieldOfView, float nearPlane, float farPlane, float aspectRatio)
		{
			this.FieldOfView = fieldOfView;
			this.NearClip = nearPlane;
			this.FarClip = farPlane;
			this.Aspect = aspectRatio;
		}


		void SetProjection()
		{
			CalculateProjectionMatrix(out mat);

			if (isExtendedClass)
			{
				Vector4 zero = Vector4.UnitW;
				Vector4 one = new Vector4(0, 0, 1, 1);
				Vector4.Transform(ref one, ref mat, out one);
				Vector4.Transform(ref zero, ref mat, out zero);
				zero.Z = 0;

				if (zero != Vector4.Zero || one.X != 0 || one.Y != 0)
					throw new InvalidOperationException("Projection matix must not modify the X/Y of positions on the z-axis. A zero vector multiplication should also have an output W of zero");
			}

			if (region.X != 0 || region.Y != 0 || region.Z != 1 || region.W != 1)
			{
				mat *= Matrix.CreateTranslation(1 - (region.X + region.Z), (region.Y + region.W) - 1, 0) * Matrix.CreateScale(1.0f / (region.Z - region.X), 1.0f / (region.W - region.Y), 1);
			}

			frustumDirty = true;
			set = true;
		}

		/// <summary>
		/// Gets the projection matrix. <see cref="GetProjectionMatrix(out Matrix, ref Vector2)"/> is the preferred method
		/// </summary>
		public Matrix ProjectionMatrix
		{
			get 
			{
				if (!set)
				{
					SetProjection();
				}
				return mat; 
			}
		}

		/// <summary>
		/// Overrideable method to calcluate the projection matrix
		/// </summary>
		/// <param name="projection"></param>
		protected virtual void CalculateProjectionMatrix(out Matrix projection)
		{
			if (orthographic)
				Matrix.CreateOrthographic(1, 1, near, far, out projection);
			else
			{
				Matrix.CreatePerspectiveFieldOfView(fov, aspect.GetValueOrDefault(computedAspect), near, far, out projection);
				if (leftHandedProjection)
				{
					projection.M33 *= -1;
					projection.M34 *= -1;
				}
			}

			if (float.IsInfinity(projection.M11))
				throw new ArgumentException();
		}

		internal void GetProjectionMatrix(out Matrix matrix, ref Vector2 drawTargetSize)
		{
			if (!aspect.HasValue && !orthographic && drawTargetSize.Y != 0)
			{
				float value = drawTargetSize.X/drawTargetSize.Y;
				if (computedAspect != value)
				{
					computedAspect = value;
					set = false;
				}
			}

			if (!set) 
				SetProjection();

			matrix = mat;
		}
	
		/// <summary>
		/// Gets/Sets the field of view of the projection (in radians)
		/// </summary>
		public float FieldOfView
		{
			get { return fov; }
			set { if (fov != value) { fov = value; set = false; changeIndex = System.Threading.Interlocked.Increment(ref changeBaseIndex); } }
		}
		/// <summary>
		/// Gets the tangent of the vertical field of view of the projection
		/// </summary>
		/// <returns></returns>
		public float GetVerticalFovTangent()
		{
			return (float)Math.Tan(FieldOfView / 2);
		}
		/// <summary>
		/// Gets the tangent of the horizontal field of view of the projection
		/// </summary>
		/// <returns></returns>
		public float GetHorizontalFovTangent()
		{
			return (float)Math.Tan(FieldOfView / 2) * this.aspect.GetValueOrDefault(this.computedAspect);
		}
		internal float GetVerticalFov()
		{
			return FieldOfView;
		}
		internal float GetHorizontalFov()
		{
			return FieldOfView * this.aspect.GetValueOrDefault(this.computedAspect);
		}
	
		/// <summary>
		/// Gets/Sets the aspect ratio of the projection. (Specify 'null' to have the aspect ratio computed by the width and height of the draw target)
		/// </summary>
		/// <remarks>
		/// <para>An aspect ratio of 1 is a square projection</para>
		/// <para>Aspect ratios are most commonly set to the Width/Height of the current render target, this will be automatically calculated if <see cref="Aspect"/> is set to null.</para>
		/// <para>Aspect ratio has no effect on an orthographic projection</para>
		/// </remarks>
		public float? Aspect
		{
			get { return aspect; }
			set { if (aspect != value) { aspect = value; set = false; changeIndex = System.Threading.Interlocked.Increment(ref changeBaseIndex); } }
		}

		/// <summary>
		/// Gets/Sets the far clipping plane distance of the projection (See <see cref="NearClip"/> remarks for more details)
		/// </summary>
		/// <remarks><para>The far clipping plane represents how far geometry will be visible before it goes outside of the z buffers range</para></remarks>
		/// <seealso cref="NearClip"/>
		public float FarClip
		{
			get { return far; }
			set { if (far != value) { far = value; set = false; changeIndex = System.Threading.Interlocked.Increment(ref changeBaseIndex); } }
		}
	
		/// <summary>
		/// Gets/Sets the near clipping plane distance of the projection
		/// </summary>
		/// <remarks>
		/// <para>The near clip plane distance determines how close geometry can get to the camera before it goes outside of the range of the z buffer, and gets clipped
		/// </para>
		/// <para>Note, the z-buffer stores depth as non-linear values. A value close to the near clip plane will be closer to 0, and will be stored with greater accuracy</para>
		/// <para>Further values will have larger z values, and have lower precision.</para>
		/// <para>Consider:</para>
		/// <para>An apprximate measure of the accurate range of the depth buffer is FarClip / NearClip. The smaller the value, the more accurate.</para>
		/// <para>Therefore, to double z-buffer accuracy, you can either halve the FarClip or double the NearClip. Consider the implications of both if the NearClip is 0.01 and the FarClip is 10,000.</para>
		/// <para>In short, keep the NearClip value as large as you can. (default value is 1)</para>
		/// </remarks>
		public float NearClip
		{
			get { return near; }
			set { if (near != value) { near = value; set = false; changeIndex = System.Threading.Interlocked.Increment(ref changeBaseIndex); } }
		}

		
		internal bool GetCameraNearFarClip(ref Vector2 v, ref int changeIndex)
		{
			if (changeIndex != this.changeIndex)
			{
				changeIndex = this.changeIndex;
				v.X = near;
				v.Y = far;
				return true;
			}
			return false;
		}

		internal bool GetCameraHorizontalVerticalFovTangent(ref Vector2 v, ref int changeIndex)
		{
			if (changeIndex != this.changeIndex)
			{
				changeIndex = this.changeIndex;
				v.Y = GetVerticalFovTangent();
				v.X = v.Y * this.aspect.GetValueOrDefault(this.computedAspect);
				return true;
			}
			return false;
		}
		internal bool GetCameraHorizontalVerticalFov(ref Vector2 v, ref int changeIndex)
		{
			if (changeIndex != this.changeIndex)
			{
				changeIndex = this.changeIndex;
				v.Y = FieldOfView;
				v.X = FieldOfView * this.aspect.GetValueOrDefault(this.computedAspect);
				return true;
			}
			return false;
		}
	
	}
}
