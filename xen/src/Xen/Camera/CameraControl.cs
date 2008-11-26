using System;
using System.Collections.Generic;
using System.Text;
using Xen.Graphics.ShaderSystem;
using Microsoft.Xna.Framework;
using Xen.Camera;
using Xen.Graphics.State;
using Xen.Graphics.ShaderSystem.Constants;

namespace Xen.Camera
{
	/*
	 * This file implements DrawState shader support for camera and world matrix related matrices, cullers and other camera ops
	 * 
	 */
	enum MatrixOp
	{
		Multiply,Inverse,Transpose
	}

#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	abstract class MatrixSource 
	{
		public Matrix value = Matrix.Identity;
		public int index = 1;

		public abstract void UpdateValue(int frame);

		public bool Changed(ref int index)
		{
			bool changed = index != this.index;
			index = this.index;
			return changed;
		}
	}

#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	sealed class ViewProjectionProvider : MatrixSource
	{
		ICamera cam;
		bool proj;
		int camIndex;
#if DEBUG
		DrawState state;
#endif

		public ViewProjectionProvider(bool projection, DrawState state)
		{
			this.proj = projection;
#if DEBUG
			this.state = state;
#endif
		}

		public override sealed void UpdateValue(int frame)
		{
		}

		public void SetConstant(IValue<Matrix> constant, int frame)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixValueSetCount);
#endif
			constant.Set(ref value);
		}
		public void SetProjectionCamera(ICamera value, ref Vector2 drawTargetSize)
		{
			if (value != cam)
			{
				cam = value;
				camIndex = 0;
			}
			if (cam != null)
			{
				if (cam.GetProjectionMatrix(ref this.value,ref drawTargetSize, ref this.camIndex))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixProjectionChangedCount);
#endif
					index++;
				}
			}
		}
		public void SetViewCamera(ICamera value)
		{
			if (value != cam)
			{
				cam = value;
				camIndex = 0;
			}
			if (cam != null)
			{
				if (cam.GetViewMatrix(ref this.value, ref this.camIndex))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixViewChangedCount);
#endif
					index++;
				}
			}
		}
	}

#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	sealed class WorldStackProvider : MatrixSource
	{
		private const bool TestMatrixEquality = false;

		private readonly Matrix[] stack;
		private readonly int[] stackIndex;
		private readonly bool[] stackIdentity;
		private readonly float[] stackApproxSize;
		private int highpoint = 1;
		private uint top;
		internal float approxScale = 1;
		internal bool isApproxNorm = true;
		internal bool detectScale = false;
		private bool isIdentity = true;
#if DEBUG
		private DrawState state;
#endif

		public WorldStackProvider(int stackSize, DrawState state)
		{
#if DEBUG
			this.state = state;
#endif
			stack = new Matrix[stackSize];
			stackIndex = new int[stackSize];
			stackApproxSize = new float[stackSize];
			stackIdentity = new bool[stackSize];
			for (int i = 0; i < stackApproxSize.Length; i++)
				stackApproxSize[i] = 1;
		}

		public void SetConstant(IValue<Matrix> constant, int frame)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixValueSetCount);
#endif
			constant.Set(ref value);
		}

		public uint MatrixStackTop { get { return top; } }
		public bool IsIdentity { get { return isIdentity; } }

		public void Set(ref Matrix matrix)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixSetWorldMatrixCount);
#endif
			if (top == 0)
			{
				throw new InvalidOperationException("World matrix at the bottom of the stack must stay an Identity Matrix, Please use PushWorldMatrix()");
			}
#if TestMatrixEquality
			if (AppState.MatrixNotEqual(ref this.value, ref matrix))//prevents recalcuating shader constants later if not changed now
#endif
			{
				this.value = matrix;
				if (detectScale)
				{
					AppState.ApproxMatrixScale(ref this.value, out this.approxScale);
					this.isApproxNorm = approxScale > 0.99995f && approxScale < 1.00005f;
				}
				index = ++highpoint;
				isIdentity = false;
			}
		}

		public void Push(ref Matrix matrix)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixPushWorldMatrixCount);
#endif
			if (top == 0)
			{
#if TestMatrixEquality
				if (AppState.MatrixIsNotIdentiy(ref matrix))//prevents recalcuating shader constants later if not changed now
#endif
				{
					this.value = matrix;
					if (detectScale)
					{
						AppState.ApproxMatrixScale(ref this.value, out this.approxScale);
						this.isApproxNorm = approxScale > 0.99995f && approxScale < 1.00005f;
					}
					index = ++highpoint;
					isIdentity = false;
				}
			}
			else
			{
				if (index != stackIndex[top])
				{
					stack[top] = matrix;
					stackIndex[top] = index;
					stackIdentity[top] = isIdentity;
					stackApproxSize[top] = approxScale;
				}
				
#if TestMatrixEquality
				if (AppState.MatrixNotEqual(ref this.value, ref matrix))//prevents recalcuating shader constants later if not changed now
#endif
				{
					this.value = matrix;
					if (detectScale)
					{
						AppState.ApproxMatrixScale(ref this.value, out this.approxScale);
						this.isApproxNorm = approxScale > 0.99995f && approxScale < 1.00005f;
					}
					index = ++highpoint;
					isIdentity = false;
				}
			}

			top++;
		}

		public void PushTrans(ref Vector3 translate)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixPushTranslateWorldMatrixCount);
#endif
			if (top == 0)
			{
				if (translate.X != 0 || translate.Y != 0 || translate.Z != 0)
				{
					this.value.M41 = translate.X;
					this.value.M42 = translate.Y;
					this.value.M43 = translate.Z;

					approxScale = 1;
					isApproxNorm = true;
					isIdentity = false;

					index = ++highpoint;
				}
			}
			else
			{
				if (index != stackIndex[top])
				{
					stack[top] = value;
					stackIndex[top] = index;
					stackIdentity[top] = isIdentity;
					stackApproxSize[top] = approxScale;
				}
				
#if TestMatrixEquality
				if (
					this.value.M11 != 1 ||
					this.value.M12 != 0 ||
					this.value.M13 != 0 ||
					this.value.M14 != 0 ||

					this.value.M21 != 0 ||
					this.value.M22 != 1 ||
					this.value.M23 != 0 ||
					this.value.M24 != 0 ||

					this.value.M31 != 0 ||
					this.value.M32 != 0 ||
					this.value.M33 != 1 ||
					this.value.M34 != 0 ||

					this.value.M11 != translate.X ||
					this.value.M12 != translate.Y ||
					this.value.M13 != translate.Z ||
					this.value.M14 != 1

					)//prevents recalcuating shader constants later if not changed now
#endif
				{
					this.value.M11 = 1;
					this.value.M12 = 0;
					this.value.M13 = 0;
					this.value.M14 = 0;

					this.value.M21 = 0;
					this.value.M22 = 1;
					this.value.M23 = 0;
					this.value.M24 = 0;

					this.value.M31 = 0;
					this.value.M32 = 0;
					this.value.M33 = 1;
					this.value.M34 = 0;

					this.value.M41 = translate.X;
					this.value.M42 = translate.Y;
					this.value.M43 = translate.Z;
					this.value.M44 = 1;

					approxScale = 1;
					isApproxNorm = true;

					index = ++highpoint;
					isIdentity = false;
				}
			}

			top++;
		}

		public void PushMultTrans(ref Vector3 translate)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixPushMultiplyTranslateWorldMatrixCount);
#endif
			if (top == 0)
			{
				if (
					/*this.value.M11 != 1 ||
					this.value.M12 != 0 ||
					this.value.M13 != 0 ||
					this.value.M14 != 0 ||

					this.value.M21 != 0 ||
					this.value.M22 != 1 ||
					this.value.M23 != 0 ||
					this.value.M24 != 0 ||

					this.value.M31 != 0 ||
					this.value.M32 != 0 ||
					this.value.M33 != 1 ||
					this.value.M34 != 0 ||

					this.value.M11 != translate.X ||
					this.value.M12 != translate.Y ||
					this.value.M13 != translate.Z ||
					this.value.M14 != 1
					*/
					translate.X != 0 || translate.Y != 0 || translate.Z != 0
					)//prevents recalcuating shader constants later if not changed now
				{
					//this.value.M11 = 1;
					//this.value.M12 = 0;
					//this.value.M13 = 0;
					//this.value.M14 = 0;

					//this.value.M21 = 0;
					//this.value.M22 = 1;
					//this.value.M23 = 0;
					//this.value.M24 = 0;

					//this.value.M31 = 0;
					//this.value.M32 = 0;
					//this.value.M33 = 1;
					//this.value.M34 = 0;

					this.value.M41 = translate.X;
					this.value.M42 = translate.Y;
					this.value.M43 = translate.Z;
					//this.value.M44 = 1;

					approxScale = 1;
					isApproxNorm = true;
					isIdentity = false;

					index = ++highpoint;
				}
			}
			else
			{
				if (index != stackIndex[top])
				{
					stack[top] = this.value;
					stackIndex[top] = index;
					stackIdentity[top] = isIdentity;
					stackApproxSize[top] = approxScale;
				}

				if (translate.X != 0 || translate.Y != 0 || translate.Z != 0)
				{
					if (isIdentity)
					{
						this.value.M41 = translate.X;
						this.value.M42 = translate.Y;
						this.value.M43 = translate.Z;
					}
					else
					{
						this.value.M41 += translate.X * this.value.M11 +
											translate.Y * this.value.M21 +
											translate.Z * this.value.M31;

						this.value.M42 += translate.X * this.value.M12 +
											translate.Y * this.value.M22 +
											translate.Z * this.value.M32;

						this.value.M43 += translate.X * this.value.M13 +
											translate.Y * this.value.M23 +
											translate.Z * this.value.M33;
					}
					isIdentity = false;
					index = ++highpoint;
				}
			}
			top++;
		}

		public void PushMult(ref Matrix matrix)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixPushMultiplyWorldMatrixCount);
#endif
			if (top == 0)
			{
#if TestMatrixEquality
				if (AppState.MatrixIsNotIdentiy(ref matrix)) //prevents recalcuating shader constants later if not changed now
#endif
				{
					this.value = matrix;
					if (detectScale)
					{
						AppState.ApproxMatrixScale(ref this.value, out this.approxScale);
						this.isApproxNorm = approxScale > 0.99995f && approxScale < 1.00005f;
					}
					index = ++highpoint;
					isIdentity = false;
				}
			}
			else
			{
				if (index != stackIndex[top])
				{
					stack[top] = this.value;
					stackIndex[top] = index;
					stackIdentity[top] = isIdentity;
					stackApproxSize[top] = approxScale;
				}

#if TestMatrixEquality
				if (AppState.MatrixIsNotIdentiy(ref matrix))
#endif
				{
					if (isIdentity)
						this.value = matrix;
					else
					{
#if DEBUG
						System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixMultiplyCalculateCount);
#endif
						Matrix.Multiply(ref matrix, ref this.value, out this.value);
					}
					if (detectScale)
					{
						AppState.ApproxMatrixScale(ref this.value, out this.approxScale);
						this.isApproxNorm = approxScale > 0.99995f && approxScale < 1.00005f;
					}
					index = ++highpoint;
					isIdentity = false;
				}
			}
			top++;
		}

		public void Pop()
		{
			if (checked(--top) != 0)
			{
				if (index != stackIndex[top])
				{
					value = stack[top];
					index = stackIndex[top];
					isIdentity = stackIdentity[top];
					approxScale = stackApproxSize[top];
					isApproxNorm = approxScale > 0.99995f && approxScale < 1.00005f;
				}
			}
			else
			{
				index = 1;
				isIdentity = true;
				approxScale = 1;
				isApproxNorm = true;

				this.value.M11 = 1;
				this.value.M12 = 0;
				this.value.M13 = 0;
				this.value.M14 = 0;
				this.value.M21 = 0;
				this.value.M22 = 1;
				this.value.M23 = 0;
				this.value.M24 = 0;
				this.value.M31 = 0;
				this.value.M32 = 0;
				this.value.M33 = 1;
				this.value.M34 = 0;
				this.value.M41 = 0;
				this.value.M42 = 0;
				this.value.M43 = 0;
				this.value.M44 = 1;
			}
		}

		public override sealed void UpdateValue(int frame)
		{
		}
	}


#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	sealed class MatrixCalc : MatrixSource
	{
		private MatrixSource provider;
		private int providerIndex;
		private MatrixOp op;
		private MatrixSource source;
		private int sourceIndex;
		private int frame;
#if DEBUG
		DrawState state;
#endif

		public MatrixCalc(MatrixOp op, MatrixSource provider, MatrixSource source, DrawState state)
		{
#if DEBUG
			this.state = state;
#endif
			this.op = op;
			this.provider = provider;
			this.source = source;
		}

		public override sealed void UpdateValue(int frame)
		{
			if (frame != this.frame)
			{
				this.frame = frame;
				provider.UpdateValue(frame);

				if (op == MatrixOp.Multiply)
				{
					source.UpdateValue(frame);

					if (provider.Changed(ref providerIndex) ||
						source.Changed(ref sourceIndex))
					{
#if DEBUG
						System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixMultiplyCalculateCount);
#endif
						Matrix.Multiply(ref provider.value, ref source.value, out value);
						index++;
					}
				}
				else
				{
					if (provider.Changed(ref providerIndex))
					{
						if (op == MatrixOp.Transpose)
						{
#if DEBUG
							System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixTransposeCalculateCount);
#endif
							Matrix.Transpose(ref provider.value, out this.value);
							index++;
						}
						else
						{
							//invert
#if DEBUG
							System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixInverseCalculateCount);
#endif
							Matrix.Invert(ref provider.value, out this.value);
							index++;
						}
					}
				}
			}
		}

		public void SetConstant(IValue<Matrix> constant, int frame, ref int changeId)
		{
			UpdateValue(frame);
			if (changeId != index)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref state.Application.currentFrame.ShaderConstantMatrixValueSetCount);
#endif
				constant.Set(ref value);
				changeId = index;
			}
		}
	}

}
namespace Xen
{

	sealed partial class DrawState : IShaderSystem, ICuller
	{
		private static int renderStackSize = 128;
		private static bool renderStackSizeUsed = false;

		private int frame;
		internal readonly WorldStackProvider worldMatrix;
		private readonly ViewProjectionProvider viewMatrix;
		private readonly ViewProjectionProvider projectionMatrix;

		private readonly ICullPrimitive[] preCullers = new ICullPrimitive[RenderStackSize], postCullers = new ICullPrimitive[RenderStackSize];
		private int preCullerCount = 0, postCullerCount = 0;

		/// <summary>
		/// Sets the size of the world matrix and render state stacks (default is 128)
		/// </summary>
		public static int RenderStackSize
		{
			get { return renderStackSize; }
			set 
			{
				if (renderStackSizeUsed)
					throw new ArgumentException("This value can only be set prior to application startup");
				renderStackSize = value; 
			}
		}

		/// <summary>
		/// Auto detect scaled matrices in the world matrix stack. Default to FALSE. Corrects frustum culling logic when scaling, but causes a slight performance loss.
		/// </summary>
		public bool WorldMatrixDetectScale
		{
			get 
			{ 	
#if DEBUG
				ValidateProtected(); 
#endif
				return worldMatrix.detectScale; 
			}
			set 
			{
#if DEBUG
				ValidateProtected(); 
#endif
				worldMatrix.detectScale = value; 
			}
		}


		/// <summary>
		/// Pushes a culler onto the pre-culling stack (pre-culling cull tests occurs <i>before</i> the default onscreen Culler). Fast cull operations are usually added here
		/// </summary>
		/// <param name="culler"></param>
		public void PushPreCuller(ICullPrimitive culler)
		{
			if (culler == null)
				throw new ArgumentNullException();
			preCullers[preCullerCount++] = culler;
		}
		/// <summary>
		/// Removes the culler on the top of the pre-culling stack
		/// </summary>
		public void PopPreCuller()
		{
			if (preCullerCount == 0)
				throw new InvalidOperationException("Stack is empty");
			preCullers[--preCullerCount] = null;
		}

		/// <summary>
		/// Pushes a culler onto the post-culling stack (post-culling cull tests  occurs <i>after</i> the default onscreen Culler). More expensive culling operations are usually added here
		/// </summary>
		/// <param name="culler"></param>
		public void PushPostCuller(ICullPrimitive culler)
		{
			if (culler == null)
				throw new ArgumentNullException();
			postCullers[postCullerCount++] = culler;
		}
		/// <summary>
		/// Removes the culler on the top of the post-culling stack
		/// </summary>
		public void PopPostCuller()
		{
			if (postCullerCount == 0)
				throw new InvalidOperationException("Stack is empty");
			postCullers[--postCullerCount] = null;
		}

		IState ICuller.GetState()
		{
			return this;
		}

		#region ICuller tests

		bool ICuller.TestBox(Vector3 min, Vector3 max)
		{
			if (worldMatrix.IsIdentity)
				return ((ICuller)this).TestWorldBox(ref min, ref max);
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif
			for (int i = preCullerCount-1; i >=0; i--)
			{
				if (!preCullers[i].TestWorldBox(ref min, ref max, ref worldMatrix.value))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.BoxInFrustum(camera.GetCullingPlanes(), ref min, ref max, ref worldMatrix.value, worldMatrix.approxScale))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldBox(ref min, ref max, ref worldMatrix.value))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return false;
				}
			}
			return true;
		}
		bool ICuller.TestBox(ref Vector3 min, ref Vector3 max)
		{
			if (worldMatrix.IsIdentity)
				return ((ICuller)this).TestWorldBox(ref min, ref max);
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldBox(ref min, ref max, ref worldMatrix.value))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.BoxInFrustum(camera.GetCullingPlanes(), ref min, ref max, ref worldMatrix.value, worldMatrix.approxScale))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldBox(ref min, ref max, ref worldMatrix.value))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return false;
				}
			}

			return true;
		}


		bool ICuller.TestBox(Vector3 min, Vector3 max, ref Matrix localMatrix)
		{
			if (worldMatrix.IsIdentity)
				return ((ICuller)this).TestWorldBox(ref min, ref max, ref localMatrix);

			Matrix matrix;
			Matrix.Multiply(ref localMatrix, ref worldMatrix.value, out matrix);
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldBox(ref min, ref max, ref matrix))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.BoxInFrustum(camera.GetCullingPlanes(), ref min, ref max, ref matrix, this.worldMatrix.detectScale ? 0 : 1))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldBox(ref min, ref max, ref matrix))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return false;
				}
			}
			return true;
		}
		bool ICuller.TestBox(ref Vector3 min, ref Vector3 max, ref Matrix localMatrix)
		{
			if (worldMatrix.IsIdentity)
				return ((ICuller)this).TestWorldBox(ref min, ref max, ref localMatrix);

			Matrix matrix;
			Matrix.Multiply(ref localMatrix, ref worldMatrix.value, out matrix);
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldBox(ref min, ref max, ref matrix))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.BoxInFrustum(camera.GetCullingPlanes(), ref min, ref max, ref matrix, worldMatrix.detectScale ? 0 : 1))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldBox(ref min, ref max, ref matrix))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return false;
				}
			}

			return true;
		}


		bool ICuller.TestSphere(float radius)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif
			Vector3 pos;
#if XBOX360
			pos = new Vector3();
#endif
			pos.X = worldMatrix.value.M41;
			pos.Y = worldMatrix.value.M42;
			pos.Z = worldMatrix.value.M43;

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldSphere(radius, ref pos))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.SphereInFrustum(camera.GetCullingPlanes(), radius * worldMatrix.approxScale, ref pos))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldSphere(radius, ref pos))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePostCulledCount);
#endif
					return false;
				}
			}

			return true;
		}
		bool ICuller.TestSphere(float radius, Vector3 position)
		{
			return ((ICuller)this).TestSphere(radius, ref position);
		}
		bool ICuller.TestSphere(float radius, ref Vector3 position)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif
			Vector3 pos;
			if (worldMatrix.IsIdentity)
				pos = position;
			else
				Vector3.Transform(ref position, ref worldMatrix.value, out pos);

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldSphere(radius, ref pos))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.SphereInFrustum(camera.GetCullingPlanes(), radius * worldMatrix.approxScale, ref pos))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldSphere(radius, ref pos))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePostCulledCount);
#endif
					return false;
				}
			}

			return true;
		}



		ContainmentType ICuller.IntersectBox(Vector3 min, Vector3 max)
		{
			if (worldMatrix.IsIdentity)
				return ((ICuller)this).IntersectWorldBox(ref min, ref max);

			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldBox(ref min, ref max, ref worldMatrix.value);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.BoxIntersectsFrustum(camera.GetCullingPlanes(), ref min, ref max, ref worldMatrix.value, worldMatrix.approxScale);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldBox(ref min, ref max, ref worldMatrix.value);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}
			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}
		ContainmentType ICuller.IntersectBox(ref Vector3 min, ref Vector3 max)
		{
			if (worldMatrix.IsIdentity)
				return ((ICuller)this).IntersectWorldBox(ref min, ref max);

			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldBox(ref min, ref max, ref worldMatrix.value);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.BoxIntersectsFrustum(camera.GetCullingPlanes(), ref min, ref max, ref worldMatrix.value, worldMatrix.approxScale);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldBox(ref min, ref max, ref worldMatrix.value);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}


		ContainmentType ICuller.IntersectBox(Vector3 min, Vector3 max, ref Matrix localMatrix)
		{
			if (worldMatrix.IsIdentity)
				return ((ICuller)this).IntersectWorldBox(ref min, ref max, ref localMatrix);

			Matrix matrix;
			Matrix.Multiply(ref localMatrix, ref worldMatrix.value, out matrix);

			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldBox(ref min, ref max, ref matrix);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.BoxIntersectsFrustum(camera.GetCullingPlanes(), ref min, ref max, ref matrix, worldMatrix.detectScale ? 0 : 1);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldBox(ref min, ref max, ref matrix);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}
			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}
		ContainmentType ICuller.IntersectBox(ref Vector3 min, ref Vector3 max, ref Matrix localMatrix)
		{
			if (worldMatrix.IsIdentity)
				return ((ICuller)this).IntersectWorldBox(ref min, ref max, ref localMatrix);

			Matrix matrix;
			Matrix.Multiply(ref localMatrix, ref worldMatrix.value, out matrix);

			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldBox(ref min, ref max, ref matrix);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.BoxIntersectsFrustum(camera.GetCullingPlanes(), ref min, ref max, ref matrix, worldMatrix.detectScale ? 0 : 1);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldBox(ref min, ref max, ref matrix);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}

		ContainmentType ICuller.IntersectSphere(float radius)
		{
			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif
			Vector3 pos;
#if XBOX360
			pos = new Vector3();
#endif
			pos.X = worldMatrix.value.M41;
			pos.Y = worldMatrix.value.M42;
			pos.Z = worldMatrix.value.M43;

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldSphere(radius, ref pos);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.SphereIntersectsFrustum(camera.GetCullingPlanes(), radius * worldMatrix.approxScale, ref pos);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldSphere(radius, ref pos);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}
		ContainmentType ICuller.IntersectSphere(float radius, Vector3 position)
		{
			return ((ICuller)this).IntersectSphere(radius, ref position);
		}
		ContainmentType ICuller.IntersectSphere(float radius, ref Vector3 position)
		{
			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCount);
#endif
#if DEBUG
			if (camera == null)
				throw new ArgumentNullException("DrawState.Camera == null");
#endif
			Vector3 pos;
			Vector3.Transform(ref position, ref worldMatrix.value, out pos);

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldSphere(radius, ref pos);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.SphereIntersectsFrustum(camera.GetCullingPlanes(), radius * worldMatrix.approxScale, ref pos);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldSphere(radius, ref pos);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}


		#endregion

		/// <summary>
		/// Gets the approximate scale factor of the current set world matrix. Requires that <see cref="WorldMatrixDetectScale"/> is set to true
		/// </summary>
		public float WorldMatrixApproximateScale
		{
			get 
			{	
#if DEBUG
				ValidateProtected(); 
#endif
				if (!worldMatrix.detectScale) 
					throw new InvalidOperationException("WorldMatrixDetectScale must be true"); 
				return worldMatrix.approxScale; 
			}
		}
		/// <summary>
		/// Returns true if the current world matrix scale is approximately 1. Requires that <see cref="WorldMatrixDetectScale"/> is set to true
		/// </summary>
		public bool WorldMatrixApproximateIsNormalised
		{
			get 
			{ 
#if DEBUG
				ValidateProtected(); 
#endif
				if (!worldMatrix.detectScale) 
					throw new InvalidOperationException("WorldMatrixDetectScale must be true"); 
				return worldMatrix.isApproxNorm; 
			}
		}

		/// <summary>
		/// Gets an interface to the current Culler (Note the DrawState class implicitly casts to <see cref="ICuller"/>, making this property redundant)
		/// </summary>
		public ICuller Culler
		{
			get 
			{ 
#if DEBUG 
				ValidateProtected(); 
#endif
				return this; 
			}
		}

		#region cull primitives

		bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldBox(ref min, ref max, ref world))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.BoxInFrustum(camera.GetCullingPlanes(), ref min, ref max, ref world, 0))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldBox(ref min, ref max, ref world))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return false;
				}
			}
			return true;
		}
		bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldBox(ref min, ref max, ref world))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.BoxInFrustum(camera.GetCullingPlanes(), ref min, ref max, ref world, 0))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldBox(ref min, ref max, ref world))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return false;
				}
			}
			return true;
		}

		bool ICullPrimitive.TestWorldBox(Vector3 min, Vector3 max)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldBox(ref min, ref max))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.AABBInFrustum(camera.GetCullingPlanes(), ref min, ref max))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldBox(ref min, ref max))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return false;
				}
			}
			return true;
		}
		bool ICullPrimitive.TestWorldBox(ref Vector3 min, ref Vector3 max)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldBox(ref min, ref max))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.AABBInFrustum(camera.GetCullingPlanes(), ref min, ref max))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldBox(ref min, ref max))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return false;
				}
			}
			return true;
		}

		bool ICullPrimitive.TestWorldSphere(float radius, Vector3 position)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCount);
#endif

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldSphere(radius, ref position))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.SphereInFrustum(camera.GetCullingPlanes(),radius,ref position))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldSphere(radius, ref position))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePostCulledCount);
#endif
					return false;
				}
			}
			return true;
		}
		bool ICullPrimitive.TestWorldSphere(float radius, ref Vector3 position)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				if (!preCullers[i].TestWorldSphere(radius, ref position))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePreCulledCount);
#endif
					return false;
				}
			}

			if (!FrustumCull.SphereInFrustum(camera.GetCullingPlanes(), radius, ref position))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCulledCount);
#endif
				return false;
			}

			for (int i = 0; i < postCullerCount; i++)
			{
				if (!postCullers[i].TestWorldSphere(radius, ref position))
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePostCulledCount);
#endif
					return false;
				}
			}
			return true;
		}


		ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max, ref Matrix world)
		{
			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldBox(ref min, ref max, ref world);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}


			type = FrustumCull.BoxIntersectsFrustum(camera.GetCullingPlanes(), ref min, ref max, ref world, 0);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldBox(ref min, ref max, ref world);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}
			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}
		ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max, ref Matrix world)
		{
			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldBox(ref min, ref max, ref world);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.BoxIntersectsFrustum(camera.GetCullingPlanes(), ref min, ref max, ref world, 0);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldBox(ref min, ref max, ref world);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}
			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldBox(Vector3 min, Vector3 max)
		{
			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldBox(ref min, ref max);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.AABBIntersectsFrustum(camera.GetCullingPlanes(), ref min, ref max);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldBox(ref min, ref max);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}
			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}
		ContainmentType ICullPrimitive.IntersectWorldBox(ref Vector3 min, ref Vector3 max)
		{
			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldBox(ref min, ref max);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.AABBIntersectsFrustum(camera.GetCullingPlanes(), ref min, ref max);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldBox(ref min, ref max);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestBoxPostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}
			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}

		ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, Vector3 position)
		{
			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCount);
#endif

			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldSphere(radius, ref position);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.SphereIntersectsFrustum(camera.GetCullingPlanes(),radius,ref position);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldSphere(radius, ref position);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}
			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}
		ContainmentType ICullPrimitive.IntersectWorldSphere(float radius, ref Vector3 position)
		{
			ContainmentType type; bool intersect = false;
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCount);
#endif
			for (int i = preCullerCount - 1; i >= 0; i--)
			{
				type = preCullers[i].IntersectWorldSphere(radius, ref position);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePreCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}

			type = FrustumCull.SphereIntersectsFrustum(camera.GetCullingPlanes(), radius, ref position);
			if (type == ContainmentType.Disjoint)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSphereCulledCount);
#endif
				return type;
			}
			if (type == ContainmentType.Intersects)
				intersect = true;

			for (int i = 0; i < postCullerCount; i++)
			{
				type = postCullers[i].IntersectWorldSphere(radius, ref position);
				if (type == ContainmentType.Disjoint)
				{
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.DefaultCullerTestSpherePostCulledCount);
#endif
					return type;
				}
				if (type == ContainmentType.Intersects)
					intersect = true;
			}
			return intersect ? ContainmentType.Intersects : ContainmentType.Contains;
		}

		#endregion

		void ICuller.GetWorldPosition(out Vector3 position)
		{
#if XBOX360
			position = new Vector3();
#endif
			position.X = worldMatrix.value.M41;
			position.Y = worldMatrix.value.M42;
			position.Z = worldMatrix.value.M43;
		}

		#region providers
		MatrixCalc ms_Projection_Inverse;
		ViewProjectionProvider ms_Projection;
		MatrixCalc ms_Projection_Transpose;
		//MatrixSource _ViewDirectionVector3;
		MatrixCalc ms_View_Inverse;
		ViewProjectionProvider ms_View;
		//MatrixSource _ViewPointVector3;
		MatrixCalc ms_ViewProjection_Inverse;
		MatrixCalc ms_ViewProjection;
		MatrixCalc ms_ViewProjection_Transpose;
		MatrixCalc ms_View_Transpose;
		//MatrixSource _WindowSizeVector2;
		MatrixCalc ms_World_Inverse;
		WorldStackProvider ms_World;
		MatrixCalc ms_WorldProjection_Inverse;
		MatrixCalc ms_WorldProjection;
		MatrixCalc ms_WorldProjection_Transpose;
		MatrixCalc ms_World_Transpose;
		MatrixCalc ms_WorldView_Inverse;
		MatrixCalc ms_WorldView;
		MatrixCalc ms_WorldViewProjection_Inverse;
		MatrixCalc ms_WorldViewProjection;
		MatrixCalc ms_WorldViewProjection_Transpose;
		MatrixCalc ms_WorldView_Transpose;

		const int psCount = 16, vsCount = 4;

		readonly Microsoft.Xna.Framework.Graphics.Texture[] 
			psTextures = new Microsoft.Xna.Framework.Graphics.Texture[psCount],
			vsTextures = new Microsoft.Xna.Framework.Graphics.Texture[vsCount];

#if DEBUG
		readonly Microsoft.Xna.Framework.Graphics.Texture[]
			psTexturesDEBUG = new Microsoft.Xna.Framework.Graphics.Texture[psCount],
			vsTexturesDEBUG = new Microsoft.Xna.Framework.Graphics.Texture[vsCount];
#endif

		readonly TextureSamplerState[] 
			psSamplers = new TextureSamplerState[psCount], 
			vsSamplers = new TextureSamplerState[vsCount];
		readonly bool[] 
			psSamplerDirty = new bool[psCount],
			vsSamplerDirty = new bool[vsCount];

		readonly Dictionary<string, int> uniqueNameIndex = new Dictionary<string, int>();

		internal void InitShaderCommon()
		{
			for (int i = 0; i < psCount; i++)
			{
				psSamplers[i] = TextureSamplerState.BilinearFiltering;
				psSamplerDirty[i] = true;
			}

			for (int i = 0; i < vsCount; i++)
			{
				vsSamplers[i] = TextureSamplerState.PointFiltering;
				vsSamplerDirty[i] = true;
			}

			renderStackSizeUsed = true;

			ms_Projection = projectionMatrix;
			ms_View = viewMatrix;
			ms_World = worldMatrix;

			ms_Projection_Inverse = Inverse(projectionMatrix);
			ms_Projection_Transpose = Transpose(projectionMatrix);
				
			ms_View_Transpose = Transpose(viewMatrix);
			ms_View_Inverse = Inverse(viewMatrix);
				
			ms_World_Inverse = Inverse(worldMatrix);
			ms_World_Transpose = Transpose(worldMatrix);
			
			ms_ViewProjection = Mult(viewMatrix,projectionMatrix);
			ms_WorldProjection = Mult(worldMatrix,projectionMatrix);
			ms_WorldView = Mult(worldMatrix, viewMatrix);

				
			ms_ViewProjection_Inverse = Inverse(ms_ViewProjection);
			ms_ViewProjection_Transpose = Transpose(ms_ViewProjection);
			ms_WorldProjection_Inverse = Inverse(ms_WorldProjection);
			ms_WorldProjection_Transpose = Transpose(ms_WorldProjection);
			ms_WorldView_Transpose = Transpose(ms_WorldView);
			ms_WorldView_Inverse = Inverse(ms_WorldView);

			ms_WorldViewProjection = Mult(worldMatrix, ms_ViewProjection);

			ms_WorldViewProjection_Inverse = Inverse(ms_WorldViewProjection);
			ms_WorldViewProjection_Transpose = Transpose(ms_WorldViewProjection);

			/*

		_ViewDirectionVector3
		_ViewPointVector3
		_WindowSizeVector2
			 * 
			 
			 */
		}

		MatrixCalc Inverse(MatrixSource provider)
		{
			return new MatrixCalc(MatrixOp.Inverse, provider, null, this);
		}
		MatrixCalc Transpose(MatrixSource provider)
		{
			return new MatrixCalc(MatrixOp.Transpose, provider, null, this);
		}
		MatrixCalc Mult(MatrixSource provider, MatrixSource source)
		{
			return new MatrixCalc(MatrixOp.Multiply, provider, source, this);
		}

		#endregion

		/// <summary>
		/// Using <see cref="GetWorldMatrix(out Matrix)"/> is the preferred method for getting the current world matrix
		/// </summary>
		public Matrix WorldMatrix
		{
			get 
			{
#if DEBUG
				ValidateProtected(); 
#endif
				return worldMatrix.value;
			}
		}
		/// <summary>
		/// Gets the translation of the current world matrix at the top of the world matrix stack
		/// </summary>
		public Vector3 WorldTranslate
		{
			get
			{
#if DEBUG
				ValidateProtected();
#endif
				return new Vector3(worldMatrix.value.M41, worldMatrix.value.M42, worldMatrix.value.M43);
			}
		}
		/// <summary>
		/// Gets the current world matrix at the top of the world matrix stack
		/// </summary>
		/// <param name="matrix"></param>
		public void GetWorldMatrix(out Matrix matrix)
		{
#if DEBUG
			ValidateProtected();
#endif
#if XBOX360
			matrix = new Matrix();
#endif
			matrix = worldMatrix.value;
		}
		/// <summary>
		/// Gets the translation of the current world matrix at the top of the world matrix stack
		/// </summary>
		/// <param name="translate"></param>
		public void GetWorldTranslate(out Vector3 translate)
		{
#if DEBUG
			ValidateProtected();
#endif
#if XBOX360
			translate = new Vector3();
#endif
			translate.X = worldMatrix.value.M41;
			translate.Y = worldMatrix.value.M42;
			translate.Z = worldMatrix.value.M43;
		}


		internal void GetStackHeight(out ushort worldHeight, out ushort stateHeight, out ushort cameraHeight, out ushort preCullerCount, out ushort postCullerCount)
		{
			worldHeight = (ushort)worldMatrix.MatrixStackTop;
			stateHeight = (ushort)renderStateStackIndex;
			cameraHeight = (ushort)cameraStack.Count;
			postCullerCount = (ushort)this.postCullerCount;
			preCullerCount = (ushort)this.preCullerCount;
		}

		internal void ValidateStackHeight(ushort worldHeight, ushort stateHeight, ushort cameraHeight, ushort preCullerCount, ushort postCullerCount)
		{
			if (worldMatrix.MatrixStackTop != worldHeight ||
				renderStateStackIndex != stateHeight ||
				cameraStack.Count != cameraHeight ||
				preCullerCount != this.preCullerCount ||
				postCullerCount != this.postCullerCount)
			{
				string str = "World matrix, camera or render state stack corruption detected during method call";
#if !XBOX360
				System.Diagnostics.StackFrame[] stack = new System.Diagnostics.StackTrace(false).GetFrames();
				for (int i = 0; i < stack.Length - 1; i++)
				{
					if (stack[i].GetMethod() == System.Reflection.MethodInfo.GetCurrentMethod())
						throw new InvalidOperationException(str + " (" + stack[i+1].GetMethod().DeclaringType +" :: "+ stack[i+1].GetMethod() + ")");
				}
#endif
				throw new InvalidOperationException(str);
			}
		}

		/// <summary>
		/// Pushes the matrix on to the top of the current rendering world matrix stack, multiplying with the current matrix at the top of the stack.
		/// </summary>
		/// <param name="matrix"></param>
		public void PushWorldMatrixMultiply(ref Matrix matrix)
		{
#if DEBUG
			ValidateProtected();
#endif
			worldMatrix.PushMult(ref matrix);
		}

		/// <summary>
		/// Optimised equivalent of calling <see cref="PushWorldMatrixMultiply"/>(<see cref="Microsoft.Xna.Framework.Matrix.CreateTranslation(Vector3)"/>(translate))
		/// </summary>
		/// <param name="translate"></param>
		public void PushWorldTranslateMultiply(ref Vector3 translate)
		{
#if DEBUG
			ValidateProtected();
#endif
			worldMatrix.PushMultTrans(ref translate);
		}
		/// <summary>
		/// Optimised equivalent of calling <see cref="PushWorldMatrixMultiply"/>(<see cref="Microsoft.Xna.Framework.Matrix.CreateTranslation(Vector3)"/>(translate))
		/// </summary>
		/// <param name="translate"></param>
		public void PushWorldTranslateMultiply(Vector3 translate)
		{
#if DEBUG
			ValidateProtected();
#endif
			worldMatrix.PushMultTrans(ref translate);
		}

		/// <summary>
		/// Copies the matrix on the top of the rendering world matrix stack into a <see cref="Xen.Graphics.StreamFrequency.InstanceMatrix"/>
		/// </summary>
		/// <param name="matrix"></param>
		public void GetWorldMatrix(ref Xen.Graphics.StreamFrequency.InstanceMatrix matrix)
		{
#if DEBUG
			ValidateProtected();
#endif
			matrix.Set(ref worldMatrix.value);
		}

		/// <summary>
		/// Pushes the matrix on to the top of the current rendering world matrix stack
		/// </summary>
		/// <param name="matrix"></param>
		public void PushWorldMatrix(ref Matrix matrix)
		{
#if DEBUG
			ValidateProtected();
#endif
			worldMatrix.Push(ref matrix);
		}
		/// <summary>
		/// Sets the top of the current rendering world matrix stack to the matrix
		/// </summary>
		/// <param name="matrix"></param>
		public void SetWorldMatrix(ref Matrix matrix)
		{
#if DEBUG
			ValidateProtected();
#endif
			worldMatrix.Set(ref matrix);
		}
		/// <summary>
		/// Optimised equivalent of calling <see cref="PushWorldMatrix"/>(<see cref="Microsoft.Xna.Framework.Matrix.CreateTranslation(Vector3)"/>(translate))
		/// </summary>
		/// <param name="translate"></param>
		public void PushWorldTranslate(ref Vector3 translate)
		{
#if DEBUG
			ValidateProtected();
#endif
			worldMatrix.PushTrans(ref translate);
		}
		/// <summary>
		/// Optimised equivalent of calling <see cref="PushWorldMatrix"/>(<see cref="Microsoft.Xna.Framework.Matrix.CreateTranslation(Vector3)"/>(translate))
		/// </summary>
		/// <param name="translate"></param>
		public void PushWorldTranslate(Vector3 translate)
		{
#if DEBUG
			ValidateProtected();
#endif
			worldMatrix.PushTrans(ref translate);
		}
		/// <summary>
		/// Pops the top of the rendering world matrix stack, Restoring the matrix saved with <see cref="PushWorldMatrix"/>
		/// </summary>
		public void PopWorldMatrix()
		{
#if DEBUG
			ValidateProtected();
#endif
			worldMatrix.Pop();
		}

		private ICamera camera;
		private bool cameraInvertsCullMode;
		private Stack<ICamera> cameraStack = new Stack<ICamera>();

		/// <summary>
		/// <para>Gets the Camera currently being used to draw the scene</para>
		/// <para>If <see cref="Camera"/> is null, then the current context lacks a camera (eg, currently no <see cref="DrawTarget"/> is being drawn to)</para>
		/// </summary>
		public ICamera Camera
		{
			get 
			{ 
#if DEBUG
				ValidateProtected();
#endif
				return this.camera; 
			}
		}

		private void SetCamera(ICamera value, ref Vector2 targetSize)
		{
			this.camera = value;

			if (value != null)
				BeginCamera(value, ref targetSize);
		}

		/// <summary>
		/// Push a new Camera on to the top of the camera stack. All rendering from this call onwards will use the new camera. Restore the previous camera with a call to <see cref="PopCamera"/>
		/// </summary>
		/// <param name="camera"></param>
		public void PushCamera(ICamera camera)
		{
#if DEBUG
			ValidateProtected();
#endif
			if (target == null)
				throw new InvalidOperationException("Camera is being set without an active draw target. Call PushCamera(camera, drawTargetDimmensions) to specify the desired draw target size (Cameras may require the draw target size to computer the aspect ratio)");

			if (camera == null)
				throw new ArgumentNullException();
			if (camera is Camera2D)
				(camera as Camera2D).Begin(this);

			cameraStack.Push(this.Camera);

			Vector2 v = new Vector2();
			SetCamera(camera, ref v);
		}

		/// <summary>
		/// Push a new Camera on to the top of the camera stack. All rendering from this call onwards will use the new camera. Restore the previous camera with a call to <see cref="PopCamera"/>
		/// </summary>
		/// <param name="drawTargetDimmensions">Manually specify the size of draw target to be used by the camera (used for cameras that automatically computer the aspect ratio)</param>
		/// <param name="camera"></param>
		public void PushCamera(ICamera camera, Vector2 drawTargetDimmensions)
		{
#if DEBUG
			ValidateProtected();
#endif

			if (camera == null)
				throw new ArgumentNullException();
			if (camera is Camera2D)
				(camera as Camera2D).Begin(this);

			cameraStack.Push(this.Camera);

			SetCamera(camera, ref drawTargetDimmensions);
		}
		/// <summary>
		/// Restores the last Camera stored with a call to <see cref="PushCamera(ICamera)"/>
		/// </summary>
		public void PopCamera()
		{
#if DEBUG
			ValidateProtected();
#endif

			Vector2 v = new Vector2();
			this.SetCamera(cameraStack.Pop(), ref v);
		}

		int GetIndex<T>(ref ShaderGlobal<T>[] array, string name, Dictionary<string,int> dict)
		{
			int index;
			if (!dict.TryGetValue(name, out index))
			{
				if (dict.Count == array.Length)
				{
					//resize up
					index = array.Length;
					ShaderGlobal<T>[] newArray = new ShaderGlobal<T>[array.Length * 2];
					for (int i = 0; i < array.Length; i++)
						newArray[i] = array[i];
					newArray[index] = new ShaderGlobal<T>();
					array = newArray;
				}
				else
				{
					index = dict.Count;
					array[index] = new ShaderGlobal<T>();
				}

				dict.Add(name, index);
			}
			return index;
		}

		int GetIndexTexture<T>(ref T[] array, ref int[] frameArray, string name, Dictionary<string, int> dict) where T : Microsoft.Xna.Framework.Graphics.Texture
		{
			int index;
			if (!dict.TryGetValue(name, out index))
			{
				if (dict.Count == array.Length)
				{
					//resize up
					index = array.Length;
					T[] newArray = new T[array.Length * 2];
					int[] newFrameArray = new int[array.Length * 2];
					for (int i = 0; i < array.Length; i++)
					{
						newArray[i] = array[i];
						newFrameArray[i] = frameArray[i];
					}
					frameArray = newFrameArray;
					array = newArray;
				}
				else
					index = dict.Count;

				dict.Add(name, index);
			}
			return index;
		}

		int IShaderSystem.GetGlobalUniqueID<T>(string name)
		{
			Type type = typeof(T);

			if (type == typeof(Matrix))
				return GetIndex(ref matrixGlobals, name, matrixGlobalLookup);

			if (type == typeof(Vector4))
				return GetIndex(ref v4Globals, name, v4GlobalLookup);

			if (type == typeof(Vector3))
				return GetIndex(ref v3Globals, name, v3GlobalLookup);

			if (type == typeof(Vector2))
				return GetIndex(ref v2Globals, name, v2GlobalLookup);

			if (type == typeof(float))
				return GetIndex(ref singleGlobals, name, singleGlobalLookup);


			if (type == typeof(Microsoft.Xna.Framework.Graphics.Texture))
				return GetIndexTexture(ref textureGlobals, ref textureGlobalsFrame, name, textureGlobalLookup);

			if (type == typeof(Microsoft.Xna.Framework.Graphics.Texture2D))
				return GetIndexTexture(ref texture2DGlobals, ref texture2DGlobalsFrame, name, texture2DGlobalLookup);

			if (type == typeof(Microsoft.Xna.Framework.Graphics.Texture3D))
				return GetIndexTexture(ref texture3DGlobals, ref texture3DGlobalsFrame, name, texture3DGlobalLookup);

			if (type == typeof(Microsoft.Xna.Framework.Graphics.TextureCube))
				return GetIndexTexture(ref textureCubeGlobals, ref textureCubeGlobalsFrame, name, textureCubeGlobalLookup);

			throw new NotImplementedException();
		}

        int IShaderSystem.GetNameUniqueID(string name)
        {
            return GetShaderAttributeNameUniqueID(name);
        }

		/// <summary>
		/// Gets the unique ID index for a non-global shader attribute. For use in a call to IShader.SetAttribute, IShader.SetTexture or IShader.SetSamplerState"/>
		/// </summary>
		/// <param name="name">case sensitive name of the shader attribute</param>
		/// <returns>globally unique index of the attribute name</returns>
		public int GetShaderAttributeNameUniqueID(string name)
		{
			int index;
			if (!uniqueNameIndex.TryGetValue(name, out index))
			{
				index = uniqueNameIndex.Count;
				uniqueNameIndex.Add(name, index);
			}
			return index;
		}

		#region set global, non array types

		void IShaderSystem.SetGlobal(IValue<Matrix> value, int uid, ref int changeId)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantMatrixGlobalAssignCount);
#endif

			ShaderGlobal<Matrix> global = matrixGlobals[uid];
			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value, uid, matrixGlobalLookup);
#endif
				value.Set(ref global.value);
				changeId = global.id;
			}
		}
		void IShaderSystem.SetGlobal(IValue<Vector4> value, int uid, ref int changeId)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantVectorGlobalAssignCount);
#endif

			ShaderGlobal<Vector4> global = v4Globals[uid];
			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value, uid, v4GlobalLookup);
#endif
				value.Set(ref global.value);
				changeId = global.id;
			}
		}
		void IShaderSystem.SetGlobal(IValue<Vector3> value, int uid, ref int changeId)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantVectorGlobalAssignCount);
#endif

			ShaderGlobal<Vector3> global = v3Globals[uid];
			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value, uid, v3GlobalLookup);
#endif
				value.Set(ref global.value);
				changeId = global.id;
			}
		}
		void IShaderSystem.SetGlobal(IValue<Vector2> value, int uid, ref int changeId)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantVectorGlobalAssignCount);
#endif

			ShaderGlobal<Vector2> global = v2Globals[uid];
			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value,uid, v2GlobalLookup);
#endif
				value.Set(ref global.value);
				changeId = global.id;
			}
		}
		void IShaderSystem.SetGlobal(IValue<float> value, int uid, ref int changeId)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantSingleGlobalAssignCount);
#endif

			ShaderGlobal<float> global = singleGlobals[uid];
			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value,uid,singleGlobalLookup);
#endif
				value.Set(ref global.value);
				changeId = global.id;
			}
		}

		#endregion


		#region set global, non array types

		void IShaderSystem.SetGlobal(IArray<Matrix> value, int uid, ref int changeId)
		{
			ShaderGlobal<Matrix[]> global = matrixArrayGlobals[uid];

#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantArrayGlobalAssignCount);
			application.currentFrame.ShaderConstantArrayGlobalAssignTotalBytes += global.value.Length * 64;
#endif

			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value, uid, matrixArrayGlobalLookup);
#endif
				value.SetArray(global.value);
				changeId = global.id;
			}
		}
		void IShaderSystem.SetGlobal(IArray<Vector4> value, int uid, ref int changeId)
		{
			ShaderGlobal<Vector4[]> global = v4ArrayGlobals[uid];

#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantArrayGlobalAssignCount);
			application.currentFrame.ShaderConstantArrayGlobalAssignTotalBytes += global.value.Length * 16;
#endif
			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value, uid, v4ArrayGlobalLookup);
#endif
				value.SetArray(global.value);
				changeId = global.id;
			}
		}
		void IShaderSystem.SetGlobal(IArray<Vector3> value, int uid, ref int changeId)
		{
			ShaderGlobal<Vector3[]> global = v3ArrayGlobals[uid];

#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantArrayGlobalAssignCount);
			application.currentFrame.ShaderConstantArrayGlobalAssignTotalBytes += global.value.Length * 12;
#endif
			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value, uid, v3ArrayGlobalLookup);
#endif
				value.SetArray(global.value);
				changeId = global.id;
			}
		}
		void IShaderSystem.SetGlobal(IArray<Vector2> value, int uid, ref int changeId)
		{
			ShaderGlobal<Vector2[]> global = v2ArrayGlobals[uid];

#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantArrayGlobalAssignCount);
			application.currentFrame.ShaderConstantArrayGlobalAssignTotalBytes += global.value.Length * 8;
#endif
			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value, uid, v2ArrayGlobalLookup);
#endif
				value.SetArray(global.value);
				changeId = global.id;
			}
		}
		void IShaderSystem.SetGlobal(IArray<float> value, int uid, ref int changeId)
		{
			ShaderGlobal<float[]> global = singleArrayGlobals[uid];

#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantArrayGlobalAssignCount);
			application.currentFrame.ShaderConstantArrayGlobalAssignTotalBytes += global.value.Length * 4;
#endif
			global.frame = frame;
			if (global.id != changeId)
			{
#if DEBUG
				if (global.id == 0)
					ValidateGlobalAccess(value, uid, singleArrayGlobalLookup);
#endif
				value.SetArray(global.value);
				changeId = global.id;
			}
		}

		#endregion


		Microsoft.Xna.Framework.Graphics.Texture IShaderSystem.GetGlobalTexture(int uid)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantTextureGlobalAssignCount);

			if (textureGlobals[uid] == null)
				ValidateGlobalTextureAccess<Microsoft.Xna.Framework.Graphics.Texture>(uid, textureGlobalLookup);
#endif
			textureGlobalsFrame[uid] = frame;
			return textureGlobals[uid];
		}
		Microsoft.Xna.Framework.Graphics.Texture2D IShaderSystem.GetGlobalTexture2D(int uid)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantTextureGlobalAssignCount);

			if (texture2DGlobals[uid] == null)
				ValidateGlobalTextureAccess<Microsoft.Xna.Framework.Graphics.Texture2D>(uid, texture2DGlobalLookup);
#endif
			texture2DGlobalsFrame[uid] = frame;
			return texture2DGlobals[uid];
		}
		Microsoft.Xna.Framework.Graphics.Texture3D IShaderSystem.GetGlobalTexture3D(int uid)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantTextureGlobalAssignCount);

			if (texture3DGlobals[uid] == null)
				ValidateGlobalTextureAccess<Microsoft.Xna.Framework.Graphics.Texture3D>(uid, texture3DGlobalLookup);
#endif
			texture3DGlobalsFrame[uid] = frame;
			return texture3DGlobals[uid];
		}
		Microsoft.Xna.Framework.Graphics.TextureCube IShaderSystem.GetGlobalTextureCube(int uid)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderConstantTextureGlobalAssignCount);

			if (textureCubeGlobals[uid] == null)
				ValidateGlobalTextureAccess<Microsoft.Xna.Framework.Graphics.TextureCube>(uid, textureCubeGlobalLookup);
#endif
			textureCubeGlobalsFrame[uid] = frame;
			return textureCubeGlobals[uid];
		}


		void ValidateGlobalAccess<T>(IValue<T> value, int uid, Dictionary<string, int> lookup) where T : struct
		{
			string name = "value";
			foreach (KeyValuePair<string,int> kvp in lookup)
			{
				if (kvp.Value == uid)
					name = kvp.Key;
			}
			throw new InvalidOperationException(string.Format("Shader is accessing uninitalised global value {0} \'{1}\'",typeof(T).Name,name));
		}

		void ValidateGlobalAccess<T>(IArray<T> value, int uid, Dictionary<string, int> lookup) where T : struct
		{
			string name = "value";
			foreach (KeyValuePair<string, int> kvp in lookup)
			{
				if (kvp.Value == uid)
					name = kvp.Key;
			}
			throw new InvalidOperationException(string.Format("Shader is accessing uninitalised global value {0}[{2}] \'{1}\'", typeof(T).Name, name,value.Length));
		}
		void ValidateGlobalTextureAccess<T>(int uid, Dictionary<string, int> lookup)
		{
			string name = "value";
			foreach (KeyValuePair<string,int> kvp in lookup)
			{
				if (kvp.Value == uid)
					name = kvp.Key;
			}
			throw new InvalidOperationException(string.Format("Shader is accessing uninitalised global texture {0} \'{1}\'",typeof(T).Name,name));
		}


		/// <summary>
		/// Set the global matrix array value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Matrix[] value)
		{
			if (value == null)
				throw new ArgumentNullException();
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalSetCount);
			application.currentFrame.ShaderArrayGlobalSetBytesTotal += value.Length * 64;
#endif
			
			ShaderGlobal<Matrix[]> global = matrixArrayGlobals[GetIndex(ref matrixArrayGlobals, name, matrixArrayGlobalLookup)];

			global.value = value;
			global.id++;
			if (global.frame == frame)
				boundShaderStateDirty = true;
		}
		/// <summary>
		/// Set the global vector array value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Vector4[] value)
		{
			if (value == null)
				throw new ArgumentNullException();
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalSetCount);
			application.currentFrame.ShaderArrayGlobalSetBytesTotal += value.Length * 16;
#endif

			ShaderGlobal<Vector4[]> global = v4ArrayGlobals[GetIndex(ref v4ArrayGlobals, name, v4ArrayGlobalLookup)];

			global.value = value;
			global.id++;
			if (global.frame == frame)
				boundShaderStateDirty = true;
		}
		/// <summary>
		/// Set the global vector array value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Vector3[] value)
		{
			if (value == null)
				throw new ArgumentNullException();
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalSetCount);
			application.currentFrame.ShaderArrayGlobalSetBytesTotal += value.Length * 12;
#endif

			ShaderGlobal<Vector3[]> global = v3ArrayGlobals[GetIndex(ref v3ArrayGlobals, name, v3ArrayGlobalLookup)];

			global.value = value;
			global.id++;
			if (global.frame == frame)
				boundShaderStateDirty = true;
		}
		/// <summary>
		/// Set the global vector array value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Vector2[] value)
		{
			if (value == null)
				throw new ArgumentNullException();
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalSetCount);
			application.currentFrame.ShaderArrayGlobalSetBytesTotal += value.Length * 8;
#endif

			ShaderGlobal<Vector2[]> global = v2ArrayGlobals[GetIndex(ref v2ArrayGlobals, name, v2ArrayGlobalLookup)];

			global.value = value;
			global.id++;
			if (global.frame == frame)
				boundShaderStateDirty = true;
		}
		/// <summary>
		/// Set the global float array value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, float[] value)
		{
			if (value == null)
				throw new ArgumentNullException();
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalSetCount);
			application.currentFrame.ShaderArrayGlobalSetBytesTotal += value.Length * 4;
#endif

			ShaderGlobal<float[]> global = singleArrayGlobals[GetIndex(ref singleArrayGlobals, name, singleArrayGlobalLookup)];

			global.value = value;
			global.id++;
			if (global.frame == frame)
				boundShaderStateDirty = true;
		}




		/// <summary>
		/// Set the global matrix value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, ref Matrix value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderMatrixGlobalSetCount);
#endif

			ShaderGlobal<Matrix> global = matrixGlobals[GetIndex(ref matrixGlobals, name, matrixGlobalLookup)];
			if (global.id == 0 ||
				AppState.MatrixNotEqual(ref value, ref global.value))
			{
				global.value = value;
				global.id++;
				if (global.frame == frame)
					boundShaderStateDirty = true;
			}
		}


		/// <summary>
		/// Get the shader global array by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Matrix[] value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalGetCount);
#endif
			value = null;
			int index;
			if (matrixArrayGlobalLookup.TryGetValue(name, out index))
			{
				value = matrixArrayGlobals[index].value;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Get the shader global array by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Vector4[] value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalGetCount);
#endif
			value = null;
			int index;
			if (v4ArrayGlobalLookup.TryGetValue(name, out index))
			{
				value = v4ArrayGlobals[index].value;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Get the shader global array by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Vector3[] value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalGetCount);
#endif
			value = null;
			int index;
			if (v3ArrayGlobalLookup.TryGetValue(name, out index))
			{
				value = v3ArrayGlobals[index].value;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Get the shader global array by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Vector2[] value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalGetCount);
#endif
			value = null;
			int index;
			if (v2ArrayGlobalLookup.TryGetValue(name, out index))
			{
				value = v2ArrayGlobals[index].value;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Get the shader global array by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out float[] value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderArrayGlobalGetCount);
#endif
			value = null;
			int index;
			if (singleArrayGlobalLookup.TryGetValue(name, out index))
			{
				value = singleArrayGlobals[index].value;
				return true;
			}
			return false;
		}



		/// <summary>
		/// Get the shader global matrix by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Matrix value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderMatrixGlobalGetCount);
#endif
			int index;
			if (matrixGlobalLookup.TryGetValue(name, out index))
			{
				value = matrixGlobals[index].value;
				return true;
			}
			value = default(Matrix);
			return false;
		}

		/// <summary>
		/// Set the global vector4 value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, ref Vector4 value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderVectorGlobalSetCount);
#endif
			ShaderGlobal<Vector4> global = v4Globals[GetIndex(ref v4Globals, name, v4GlobalLookup)];
			if (global.id == 0 || 
				(value.X != global.value.X ||
				 value.Y != global.value.Y ||
				 value.Z != global.value.Z ||
				 value.W != global.value.W))
			{
				global.value = value;
				global.id++;
				if (global.frame == frame)
					boundShaderStateDirty = true;
			}
		}

		/// <summary>
		/// Get the shader global vector4 by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Vector4 value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderVectorGlobalGetCount);
#endif
			int index;
			if (v4GlobalLookup.TryGetValue(name, out index))
			{
				value = v4Globals[index].value;
				return true;
			}
			value = default(Vector4);
			return false;
		}

		/// <summary>
		/// Set the global vector3 value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, ref Vector3 value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderVectorGlobalSetCount);
#endif
			ShaderGlobal<Vector3> global = v3Globals[GetIndex(ref v3Globals, name, v3GlobalLookup)];
			if (global.id == 0 ||
				(value.X != global.value.X ||
				 value.Y != global.value.Y ||
				 value.Z != global.value.Z))
			{
				global.value = value;
				global.id++;
				if (global.frame == frame)
					boundShaderStateDirty = true;
			}
		}

		/// <summary>
		/// Get the shader global vector3 by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Vector3 value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderVectorGlobalGetCount);
#endif
			int index;
			if (v3GlobalLookup.TryGetValue(name, out index))
			{
				value = v3Globals[index].value;
				return true;
			}
			value = default(Vector3);
			return false;
		}

		/// <summary>
		/// Set the global vector2 value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, ref Vector2 value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderVectorGlobalSetCount);
#endif
			ShaderGlobal<Vector2> global = v2Globals[GetIndex(ref v2Globals, name, v2GlobalLookup)];
			if (global.id == 0 ||
				(value.X != global.value.X ||
				 value.Y != global.value.Y))
			{
				global.value = value;
				global.id++;
				if (global.frame == frame)
					boundShaderStateDirty = true;
			}
		}

		/// <summary>
		/// Get the shader global vector2 by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Vector2 value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderVectorGlobalGetCount);
#endif
			int index;
			if (v2GlobalLookup.TryGetValue(name, out index))
			{
				value = v2Globals[index].value;
				return true;
			}
			value = default(Vector2);
			return false;
		}

		///// <summary>
		///// Set the global matrix value used by shaders
		///// </summary>
		///// <param name="name">name of the value (case sensitive)</param>
		///// <param name="value">value to assign the shader global</param>
		//public void SetShaderGlobal(string name, Matrix value)
		//{
		//    SetShaderGlobal(name,ref value);
		//}
		/// <summary>
		/// Set the global Vector4 value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Vector4 value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderVectorGlobalSetCount);
#endif
			SetShaderGlobal(name,ref value);
		}
		/// <summary>
		/// Set the global vector3 value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Vector3 value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderVectorGlobalSetCount);
#endif
			SetShaderGlobal(name,ref value);
		}
		/// <summary>
		/// Set the global vector2 value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Vector2 value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderVectorGlobalSetCount);
#endif
			SetShaderGlobal(name, ref value);
		}

		/// <summary>
		/// Set the global float value used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, float value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderSingleGlobalSetCount);
#endif
			ShaderGlobal<float> global = singleGlobals[GetIndex(ref singleGlobals, name, singleGlobalLookup)];
			if (global.id == 0 ||
				value != global.value)
			{
				global.value = value;
				global.id++;
				if (global.frame == frame)
					boundShaderStateDirty = true;
			}
		}

		/// <summary>
		/// Get the shader global float by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out float value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderSingleGlobalGetCount);
#endif
			int index;
			if (singleGlobalLookup.TryGetValue(name, out index))
			{
				value = singleGlobals[index].value;
				return true;
			}
			value = default(float);
			return false;
		}


		/// <summary>
		/// Set the global texture used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Microsoft.Xna.Framework.Graphics.Texture value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderTextureGlobalSetCount);
#endif
			int i = GetIndexTexture(ref textureGlobals, ref textureGlobalsFrame, name, textureGlobalLookup);
			textureGlobals[i] = value;
			if (textureGlobalsFrame[i] == frame)
				boundShaderStateDirty = true;
		}

		/// <summary>
		/// Get the shader global texture by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Microsoft.Xna.Framework.Graphics.Texture value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderTextureGlobalGetCount);
#endif
			int index;
			if (textureGlobalLookup.TryGetValue(name, out index))
			{
				value = textureGlobals[index];
				return true;
			}
			value = null;
			return false;
		}

		/// <summary>
		/// Set the global texture (2D) used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Microsoft.Xna.Framework.Graphics.Texture2D value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderTextureGlobalSetCount);
#endif
			int i = GetIndexTexture(ref texture2DGlobals, ref texture2DGlobalsFrame, name, texture2DGlobalLookup);
			texture2DGlobals[i] = value;
			if (texture2DGlobalsFrame[i] == frame)
				boundShaderStateDirty = true;
		}

		/// <summary>
		/// Get the shader global texture (2D) by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Microsoft.Xna.Framework.Graphics.Texture2D value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderTextureGlobalGetCount);
#endif
			int index;
			if (texture2DGlobalLookup.TryGetValue(name, out index))
			{
				value = texture2DGlobals[index];
				return true;
			}
			value = null;
			return false;
		}

		/// <summary>
		/// Set the global texture (3D) used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Microsoft.Xna.Framework.Graphics.Texture3D value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderTextureGlobalSetCount);
#endif
			int i = GetIndexTexture(ref texture3DGlobals, ref texture3DGlobalsFrame, name, texture3DGlobalLookup);
			texture3DGlobals[i] = value;
			if (texture3DGlobalsFrame[i] == frame)
				boundShaderStateDirty = true;
		}

		/// <summary>
		/// Get the shader global texture (3D) by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Microsoft.Xna.Framework.Graphics.Texture3D value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderTextureGlobalGetCount);
#endif
			int index;
			if (texture3DGlobalLookup.TryGetValue(name, out index))
			{
				value = texture3DGlobals[index];
				return true;
			}
			value = null;
			return false;
		}

		/// <summary>
		/// Set the global texture (cubemap) used by shaders
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">value to assign the shader global</param>
		public void SetShaderGlobal(string name, Microsoft.Xna.Framework.Graphics.TextureCube value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderTextureGlobalSetCount);
#endif
			int i = GetIndexTexture(ref textureCubeGlobals, ref textureCubeGlobalsFrame, name, textureCubeGlobalLookup);
			textureCubeGlobals[i] = value;
			if (textureCubeGlobalsFrame[i] == frame)
				boundShaderStateDirty = true;
		}

		/// <summary>
		/// Get the shader global texture (cubemap) by name. Returns true if the value exists
		/// </summary>
		/// <param name="name">name of the value (case sensitive)</param>
		/// <param name="value">output value to be assigned the shader global value (if it exists)</param>
		/// <remarks>True if the value exists, and has been ouput</remarks>
		public bool GetShaderGlobal(string name, out Microsoft.Xna.Framework.Graphics.TextureCube value)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderTextureGlobalGetCount);
#endif
			int index;
			if (textureCubeGlobalLookup.TryGetValue(name, out index))
			{
				value = textureCubeGlobals[index];
				return true;
			}
			value = null;
			return false;
		}

		private ShaderGlobal<Matrix>[] matrixGlobals = new ShaderGlobal<Matrix>[8];
		private ShaderGlobal<Vector4>[] v4Globals = new ShaderGlobal<Vector4>[8];
		private ShaderGlobal<Vector3>[] v3Globals = new ShaderGlobal<Vector3>[8];
		private ShaderGlobal<Vector2>[] v2Globals = new ShaderGlobal<Vector2>[8];
		private ShaderGlobal<float>[] singleGlobals = new ShaderGlobal<float>[8];

		private ShaderGlobal<Matrix[]>[] matrixArrayGlobals = new ShaderGlobal<Matrix[]>[8];
		private ShaderGlobal<Vector4[]>[] v4ArrayGlobals = new ShaderGlobal<Vector4[]>[8];
		private ShaderGlobal<Vector3[]>[] v3ArrayGlobals = new ShaderGlobal<Vector3[]>[8];
		private ShaderGlobal<Vector2[]>[] v2ArrayGlobals = new ShaderGlobal<Vector2[]>[8];
		private ShaderGlobal<float[]>[] singleArrayGlobals = new ShaderGlobal<float[]>[8];

		private Microsoft.Xna.Framework.Graphics.Texture[] textureGlobals = new Microsoft.Xna.Framework.Graphics.Texture[8];
		private Microsoft.Xna.Framework.Graphics.Texture2D[] texture2DGlobals = new Microsoft.Xna.Framework.Graphics.Texture2D[8];
		private Microsoft.Xna.Framework.Graphics.Texture3D[] texture3DGlobals = new Microsoft.Xna.Framework.Graphics.Texture3D[8];
		private Microsoft.Xna.Framework.Graphics.TextureCube[] textureCubeGlobals = new Microsoft.Xna.Framework.Graphics.TextureCube[8];

		private int[] textureGlobalsFrame = new int[8];
		private int[] texture2DGlobalsFrame = new int[8];
		private int[] texture3DGlobalsFrame = new int[8];
		private int[] textureCubeGlobalsFrame = new int[8];

		private readonly Dictionary<string, int>
			matrixGlobalLookup = new Dictionary<string, int>(),
			v4GlobalLookup = new Dictionary<string, int>(),
			v3GlobalLookup = new Dictionary<string, int>(),
			v2GlobalLookup = new Dictionary<string, int>(),
			singleGlobalLookup = new Dictionary<string, int>(),

			matrixArrayGlobalLookup = new Dictionary<string, int>(),
			v4ArrayGlobalLookup = new Dictionary<string, int>(),
			v3ArrayGlobalLookup = new Dictionary<string, int>(),
			v2ArrayGlobalLookup = new Dictionary<string, int>(),
			singleArrayGlobalLookup = new Dictionary<string, int>(),

			textureGlobalLookup = new Dictionary<string, int>(),
			texture2DGlobalLookup = new Dictionary<string, int>(),
			texture3DGlobalLookup = new Dictionary<string, int>(),
			textureCubeGlobalLookup = new Dictionary<string, int>();

		sealed class ShaderGlobal<T>
		{
			public T value;
			public int id;
			public int frame;
		}

		void IShaderSystem.SetPixelShaderSampler(int index, Microsoft.Xna.Framework.Graphics.Texture texture, Xen.Graphics.State.TextureSamplerState state)
		{
			TextureSamplerState sampler = (TextureSamplerState)state;
			Microsoft.Xna.Framework.Graphics.SamplerState samplerState = graphics.SamplerStates[index];

			if (psSamplerDirty[index])
			{
				TextureSamplerStateInternal.ResetState(samplerState, sampler);
				psSamplers[index] = sampler;
			//	sampler.ResetState(samplerState, ref psSamplers[index]);

				graphics.Textures[index] = texture;
				psTextures[index] = texture;

				psSamplerDirty[index] = false;
			}
			else
			{
				if (psSamplers[index] != sampler)
				{
				//	sampler.ApplyState(samplerState, ref psSamplers[index]);
					TextureSamplerStateInternal.ApplyState(sampler, samplerState, ref psSamplers[index]
#if DEBUG
					,this
#endif
						);
				}
			
				if (texture != psTextures[index])
				{
					graphics.Textures[index] = texture;
					psTextures[index] = texture;
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.TextureUnitTextureChanged);
#endif
				}
			}
#if DEBUG
			psTexturesDEBUG[index] = texture;
#endif
		}
		void IShaderSystem.SetVertexShaderSampler(int index, Microsoft.Xna.Framework.Graphics.Texture texture, Xen.Graphics.State.TextureSamplerState state)
		{
			TextureSamplerState sampler = (TextureSamplerState)state;
			Microsoft.Xna.Framework.Graphics.SamplerState samplerState = graphics.VertexSamplerStates[index];

			if (vsSamplerDirty[index])
			{
			//	sampler.ResetState(samplerState, ref vsSamplers[index]);
				TextureSamplerStateInternal.ResetState(samplerState,sampler);
				vsSamplers[index] = sampler;
				
				graphics.VertexTextures[index] = texture;
				vsTextures[index] = texture;

				vsSamplerDirty[index] = false;
			}
			else
			{
				if (vsSamplers[index] != sampler)
				{
					TextureSamplerStateInternal.ApplyState(sampler, samplerState, ref vsSamplers[index]
#if DEBUG
			, this
#endif
			);
				//	sampler.ApplyState(samplerState, ref vsSamplers[index]);
				}
			
				if (texture != vsTextures[index])
				{
					graphics.VertexTextures[index] = texture;
					vsTextures[index] = texture;
#if DEBUG
					System.Threading.Interlocked.Increment(ref application.currentFrame.VertexTextureUnitTextureChanged);
#endif
				}
			}
#if DEBUG
			vsTexturesDEBUG[index] = texture;
#endif
		}

#if DEBUG
		internal void CalcBoundTextures()
		{
			System.Threading.Interlocked.Increment(ref application.currentFrame.BoundTextureCountTotalSamples);

			for (int i = 0; i < psTexturesDEBUG.Length; i++)
			{
				if (psTexturesDEBUG[i] != null)
					System.Threading.Interlocked.Increment(ref application.currentFrame.BoundTextureCount);
			}
			for (int i = 0; i < vsTexturesDEBUG.Length; i++)
			{
				if (vsTexturesDEBUG[i] != null)
					System.Threading.Interlocked.Increment(ref application.currentFrame.BoundVertexTextureCount);
			}
		}
#endif

		internal void BeginCamera(ICamera camera, ref Vector2 targetSize)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.SetCameraCount);
#endif
			targetSize = target != null ? target.Size : targetSize;

			if (targetSize.X == 0 || targetSize.Y == 0)
			{
				throw new ArgumentException("When pushing a camera, a render target size must be specified if a draw target isn't currently active");
			}

			projectionMatrix.SetProjectionCamera(camera, ref targetSize);
			viewMatrix.SetViewCamera(camera);

			if (cameraInvertsCullMode != camera.ReverseBackfaceCulling)
			{
				//swap the internal render state cull flag, to force the change to be applied
				switch (this.internalState.DepthColourCull.CullMode)
				{
					case Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace:
						this.internalState.DepthColourCull.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace;
						break;
					case Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace:
						this.internalState.DepthColourCull.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace;
						break;
				}

				cameraInvertsCullMode = camera.ReverseBackfaceCulling;
			}
		}

		int IShaderSystem.DeviceUniqueIndex
		{
			get
			{
				return application.graphicsId;
			}
		}

		int IShaderSystem.Begin(Xen.Graphics.IShader shader, int psSamplerMax, int vsSamplerMax, out bool typeChanged, out bool instanceChanged)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderBindCount);

			if (camera == null)
			{
				throw new InvalidOperationException("Shaders cannot be bound with a null camera");
			}
#endif

#if DEBUG
			ValidateProtected();
#endif

			unchecked { frame++; }

			boundShaderStateDirty = false;

			boundShaderWorldIndex = worldMatrix.index;
			boundShaderProjectionIndex = projectionMatrix.index;
			boundShaderViewIndex = viewMatrix.index;

			boundShaderUsesWorldMatrix = false;
			boundShaderUsesProjectionMatrix = false;
			boundShaderUsesViewMatrix = false;
			boundShaderUsesVertexCount = false;


			if (this.boundShader == shader)
			{
				instanceChanged = false;
				typeChanged = false;
			}
			else
			{
				Type type = shader.GetType();
				typeChanged = boundShaderType != type;
				instanceChanged = true;
				boundShader = shader;
				boundShaderType = type;
			}

#if DEBUG
			if (typeChanged)
			{
				for (int i = 0; i < psTexturesDEBUG.Length; i++)
					psTexturesDEBUG[i] = null;
				for (int i = 0; i < vsTexturesDEBUG.Length; i++)
					vsTexturesDEBUG[i] = null;
			}
#endif

			return application.graphicsId;
		}

		private Dictionary<byte[], Microsoft.Xna.Framework.Graphics.VertexShader> cachedVShaders = new Dictionary<byte[], Microsoft.Xna.Framework.Graphics.VertexShader>();
		private Dictionary<byte[], Microsoft.Xna.Framework.Graphics.PixelShader> cachedPShaders = new Dictionary<byte[], Microsoft.Xna.Framework.Graphics.PixelShader>();

		void IShaderSystem.CreateShaders(
			out Microsoft.Xna.Framework.Graphics.VertexShader vertexShader, 
			out Microsoft.Xna.Framework.Graphics.PixelShader pixelShader, 
			byte[] vShaderBytes, 
			byte[] pShaderBytes,
			int vsCount,int psCount,
			int vsPreCount, int psPreCount)
		{
			if (!cachedVShaders.TryGetValue(vShaderBytes, out vertexShader))
			{
				vertexShader = new Microsoft.Xna.Framework.Graphics.VertexShader(graphics, vShaderBytes);
				cachedVShaders.Add(vShaderBytes, vertexShader);
			}
			if (!cachedPShaders.TryGetValue(pShaderBytes, out pixelShader))
			{
				pixelShader = new Microsoft.Xna.Framework.Graphics.PixelShader(graphics, pShaderBytes);
				cachedPShaders.Add(pShaderBytes, pixelShader);
			}
		
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.BinaryShadersCreated);
			vertexShader.Tag = new int[] { vsCount, vsPreCount };
			pixelShader.Tag = new int[] { psCount, psPreCount };
#endif
		}
		void IShaderSystem.SetShaders(
			Microsoft.Xna.Framework.Graphics.VertexShader vertexShader, 
			Microsoft.Xna.Framework.Graphics.PixelShader pixelShader)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.BinaryShadersSet);


			int[] arr = vertexShader.Tag as int[];
			if (arr != null && arr.Length == 2)
			{
				System.Threading.Interlocked.Increment(ref application.currentFrame.VertexShaderBoundWithKnownInstructionsCount);
				this.application.currentFrame.VertexShaderApproximateInstructionsTotal += arr[0];
				if (arr[1] > 0)
				{
					System.Threading.Interlocked.Increment(ref application.currentFrame.PixelShaderBoundWithPreShaderCount);
					this.application.currentFrame.VertexShaderApproximatePreshaderInstructionsTotal += arr[1];
				}
			}

			arr = pixelShader.Tag as int[];
			if (arr != null && arr.Length == 2)
			{
				System.Threading.Interlocked.Increment(ref application.currentFrame.PixelShaderBoundWithKnownInstructionsCount);
				this.application.currentFrame.PixelShaderApproximateInstructionsTotal += arr[0];
				if (arr[1] > 0)
				{
					System.Threading.Interlocked.Increment(ref application.currentFrame.PixelShaderBoundWithPreShaderCount);
					this.application.currentFrame.PixelShaderApproximatePreshaderInstructionsTotal += arr[1];
				}
			}
#endif
			graphics.VertexShader = vertexShader;
			graphics.PixelShader = pixelShader;
			
#if XBOX360
			boundVertexShader = vertexShader;
			boundPixelShader = pixelShader;
			shadersAssigned = true;
#endif
		}

#if XBOX360
		//there is a bug with XNA on the 360 that sometimes occurs...
		//if you change VS constants, but not PS constants, then the 
		//the pixel shader can need reassigning, even though it's still set.
		//The same may be true for VS.
		private bool shadersAssigned;
		private bool vertexConstantsSet, pixelConstantsSet;
		
		Microsoft.Xna.Framework.Graphics.VertexShader boundVertexShader;
		Microsoft.Xna.Framework.Graphics.PixelShader boundPixelShader;
#endif

		void IShaderSystem.SetShaderConstants(
			Vector4[] vertexShaderConstants, 
			Vector4[] pixelShaderConstants)
		{
#if DEBUG
			if (vertexShaderConstants != null)
			{
				this.application.currentFrame.VertexShaderConstantBytesSetTotalCount += vertexShaderConstants.Length * 16;
				System.Threading.Interlocked.Increment(ref application.currentFrame.VertexShaderConstantBytesSetCount);
			}
			if (pixelShaderConstants != null)
			{
				this.application.currentFrame.PixelShaderConstantBytesSetTotalCount += pixelShaderConstants.Length * 16;
				System.Threading.Interlocked.Increment(ref application.currentFrame.PixelShaderConstantBytesSetCount);
			}
#endif
#if XBOX360

			if (vertexShaderConstants != null)
			{
				vertexConstantsSet = true;
				graphics.SetVertexShaderConstant(0, vertexShaderConstants);
			}
			if (pixelShaderConstants != null)
			{
				pixelConstantsSet = true;
				graphics.SetPixelShaderConstant(0, pixelShaderConstants);
			}
			
#else
			if (vertexShaderConstants != null)
				graphics.SetVertexShaderConstant(0, vertexShaderConstants);
			if (pixelShaderConstants != null)
				graphics.SetPixelShaderConstant(0, pixelShaderConstants);
			
#endif
		}

		#region deferred rendering logic

		private DeferredDrawCall[] deferredDrawList = new DeferredDrawCall[0];
		private int[] deferredDrawListSortOrder;
		private int deferredDrawListCount;
		private bool currentDrawingDeferredCalls;
		private DeferredDrawCallComparer deferredDrawCallComparerInstance = new DeferredDrawCallComparer();

		private struct DeferredDrawCall
		{
			public Matrix stack;
			public IDraw call;
			public DeviceRenderState state;
		}

		private sealed class DeferredDrawCallComparer : IComparer<int>
		{
			public DeferredDrawCall[] calls;
			public IComparer<IDraw> deferredDrawListSorter;
			public int Compare(int x, int y)
			{
				return deferredDrawListSorter.Compare(calls[x].call, calls[y].call);
			}
		}

		/// <summary>
		/// Defer a draw call, to be called at a later time. Render state and world matrix is also stored.
		/// </summary>
		/// <param name="call">Draw call to make at a later time</param>
		/// <remarks>
		/// <para>Deferred draw calls can be run by calling <see cref="RunDeferredDrawCalls()"/>, or will be run before the current <see cref="DrawTarget"/> completes drawing</para>
		/// <para>The call will be ignored if the <see cref="ICullable.CullTest"/> fails</para>
		/// </remarks>
		public void DeferDrawCall(IDraw call)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DeferredDrawCallsMade);
#endif
#if DEBUG
			ValidateProtected();
#endif

			if (call.CullTest(this))
			{
				if (currentDrawingDeferredCalls)
					throw new InvalidOperationException("Recursive deferred rendering is not supported");

				if (deferredDrawListCount == deferredDrawList.Length)
				{
					Array.Resize(ref deferredDrawList, Math.Max(32, deferredDrawList.Length * 2));
				}

				DeferCall(call, ref deferredDrawList[deferredDrawListCount++]);
			}
#if DEBUG
			else
				System.Threading.Interlocked.Increment(ref application.currentFrame.DeferredDrawCallsCulled);
#endif
		}

		void DeferCall(IDraw call, ref DeferredDrawCall store)
		{
			store.call = call;
			store.state = this.visibleState.state;
			GetWorldMatrix(out store.stack);
		}

		/// <summary>
		/// Run all deferred draw calls (made with <see cref="DeferDrawCall"/>).
		/// </summary>
		/// <seealso cref="DeferDrawCall"/>
		public void RunDeferredDrawCalls()
		{
#if DEBUG
			ValidateProtected();
#endif

			DeviceRenderState state = visibleState.state;
			int count = deferredDrawListCount;
			deferredDrawListCount = 0;
			currentDrawingDeferredCalls = true;

			if (count > 0)
			{
				Matrix mat = Matrix.Identity;
				PushWorldMatrix(ref mat);
			}

			for (int i = 0; i < count; i++)
			{
				SetWorldMatrix(ref deferredDrawList[i].stack);
				visibleState.state = deferredDrawList[i].state;
				deferredDrawList[i].call.Draw(this);

				deferredDrawList[i].call = null;
			}

			if (count > 0)
				PopWorldMatrix();

			currentDrawingDeferredCalls = false;
			visibleState.state = state;
		}

		/// <summary>
		/// Run all deferred draw calls (made with <see cref="DeferDrawCall"/>).
		/// </summary>
		/// <param name="sorter"></param>
		/// <seealso cref="DeferDrawCall"/>
		public void RunDeferredDrawCalls(IComparer<IDraw> sorter)
		{
#if DEBUG
			ValidateProtected();
#endif

			if (deferredDrawListCount == 0)
				return;

			deferredDrawCallComparerInstance.deferredDrawListSorter = sorter;
			deferredDrawCallComparerInstance.calls = this.deferredDrawList;

			if (deferredDrawListSortOrder == null || deferredDrawListCount > deferredDrawListSortOrder.Length)
			{
				int size = 2;
				while (deferredDrawListCount > size)
					size *= 2;
				deferredDrawListSortOrder = new int[size];
			}

			for (int i = 0; i < deferredDrawListCount; i++)
				deferredDrawListSortOrder[i] = i;

			Array.Sort<int>(deferredDrawListSortOrder, 0, deferredDrawListCount, deferredDrawCallComparerInstance);

			deferredDrawCallComparerInstance.calls = null;
			deferredDrawCallComparerInstance.deferredDrawListSorter = null;

			currentDrawingDeferredCalls = true;
			DeviceRenderState state = visibleState.state;
			int count = deferredDrawListCount;
			deferredDrawListCount = 0;
			if (count > 0)
			{
				Matrix mat = Matrix.Identity;
				PushWorldMatrix(ref mat);
			}

			for (int i = 0; i < count; i++)
			{
				int index = deferredDrawListSortOrder[i];

				SetWorldMatrix(ref deferredDrawList[index].stack);
				visibleState.state = deferredDrawList[index].state;
				deferredDrawList[index].call.Draw(this);

				deferredDrawList[index].call = null;
			}

			if (count > 0)
				PopWorldMatrix();

			visibleState.state = state;
			currentDrawingDeferredCalls = false;
		}


		#endregion


		#region IShaderSystem Members




		void IShaderSystem.SetProjectionInverseMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesProjectionMatrix = true;
			ms_Projection_Inverse.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetProjectionMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesProjectionMatrix = true;
			if (ci != ms_Projection.index)
			{
				ms_Projection.SetConstant(value, frame); 
				ci = ms_Projection.index; 
			}
		}

		void IShaderSystem.SetProjectionTransposeMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesProjectionMatrix = true;
			ms_Projection_Transpose.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetViewDirectionVector3(IValue<Vector3> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			Vector3 v = new Vector3();
			if (camera.GetCameraViewDirection(ref v, ref ci))
				value.Set(ref v);
		}

		void IShaderSystem.SetViewDirectionVector4(IValue<Vector4> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			Vector3 v = new Vector3();
			if (camera.GetCameraViewDirection(ref v, ref ci))
			{
				Vector4 v4 = new Vector4(v.X, v.Y, v.Z, 0);
				value.Set(ref v4);
			}
		}

		void IShaderSystem.SetCameraFovVector2(IValue<Vector2> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			Vector2 v = new Vector2();
			if (camera.GetCameraHorizontalVerticalFov(ref v, ref ci))
				value.Set(ref v);
		}
		void IShaderSystem.SetCameraFovTangentVector2(IValue<Vector2> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			Vector2 v = new Vector2();
			if (camera.GetCameraHorizontalVerticalFovTangent(ref v, ref ci))
				value.Set(ref v);
		}

		void IShaderSystem.SetCameraNearFarVector2(IValue<Vector2> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			Vector2 v = new Vector2();
			if (camera.GetCameraNearFarClip(ref v, ref ci))
				value.Set(ref v);
		}

		void IShaderSystem.SetViewInverseMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			ms_View_Inverse.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetViewMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			if (ci != ms_View.index)
			{
				ms_View.SetConstant(value, frame);
				ci = ms_View.index;
			}
		}

		void IShaderSystem.SetViewPointVector3(IValue<Vector3> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			Vector3 v = new Vector3();
			if (camera.GetCameraPosition(ref v, ref ci))
				value.Set(ref v);
		}
		void IShaderSystem.SetViewPointVector4(IValue<Vector4> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			Vector3 v = new Vector3();
			if (camera.GetCameraPosition(ref v, ref ci))
			{
				Vector4 v4 = new Vector4(v.X, v.Y, v.Z, 0);
				value.Set(ref v4);
			}
		}

		void IShaderSystem.SetViewProjectionInverseMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			boundShaderUsesProjectionMatrix = true;
			ms_ViewProjection_Inverse.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetViewProjectionMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesProjectionMatrix = true;
			boundShaderUsesViewMatrix = true;
			ms_ViewProjection.SetConstant(value, frame, ref ci); 
		}

		void IShaderSystem.SetViewProjectionTransposeMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			boundShaderUsesProjectionMatrix = true;
			ms_ViewProjection_Transpose.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetViewTransposeMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			ms_View_Transpose.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWindowSizeVector2(IValue<Vector2> value, ref int ci)
		{
			Vector2 v;
			if (this.target.GetWidthHeightAsVector(out v, ref ci))
				value.Set(ref v);
		}

		void IShaderSystem.SetWorldInverseMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesWorldMatrix = true;
			ms_World_Inverse.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesWorldMatrix = true;
			if (ci != ms_World.index)
			{
				ms_World.SetConstant(value, frame);
				ci = ms_World.index;
			}
		}

		void IShaderSystem.SetWorldProjectionInverseMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesWorldMatrix = true;
			boundShaderUsesProjectionMatrix = true;
			ms_WorldProjection_Inverse.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldProjectionMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesWorldMatrix = true;
			boundShaderUsesProjectionMatrix = true;
			ms_WorldProjection.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldProjectionTransposeMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesWorldMatrix = true;
			boundShaderUsesProjectionMatrix = true;
			ms_WorldProjection_Transpose.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldTransposeMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesWorldMatrix = true;
			ms_World_Transpose.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldViewInverseMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			boundShaderUsesWorldMatrix = true;
			ms_WorldView_Inverse.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldViewMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesWorldMatrix = true;
			boundShaderUsesViewMatrix = true;
			ms_WorldView.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldViewProjectionInverseMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			boundShaderUsesProjectionMatrix = true;
			boundShaderUsesWorldMatrix = true;
			ms_WorldViewProjection_Inverse.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldViewProjectionMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			boundShaderUsesProjectionMatrix = true;
			boundShaderUsesWorldMatrix = true;
			ms_WorldViewProjection.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldViewProjectionTransposeMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			boundShaderUsesProjectionMatrix = true;
			boundShaderUsesWorldMatrix = true;
			ms_WorldViewProjection_Transpose.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetWorldViewTransposeMatrix(IValue<Matrix> value, ref int ci)
		{
			boundShaderUsesViewMatrix = true;
			boundShaderUsesWorldMatrix = true;
			ms_WorldView_Transpose.SetConstant(value, frame, ref ci);
		}

		void IShaderSystem.SetVertexCountSingle(IValue<float> value, ref int ci)
		{
			if (ci != bufferVertexCountChangeIndex)
			{
				float count = (float)(bufferVertexCount);
				value.Set(ref count);
				ci = bufferVertexCountChangeIndex;
			}
			boundShaderUsesVertexCount = true;
		}

		#endregion


		#region Project / UnProject

		/// <summary>
		/// <para>Projects a position in 3D space into screen pixel coordinates</para>
		/// <para>Returns false if the projected point is behind the camera</para>
		/// </summary>
		/// <param name="position">3D position to project into screen coordinates</param>
		/// <param name="screenCoordinate">screen coordinates of the projected position</param>
		/// <returns>True if the projected position is in front of the camera</returns>
		public bool ProjectToScreen(ref Vector3 position, out Vector2 screenCoordinate)
		{
			Vector3 pos;
			Vector3.Transform(ref position, ref worldMatrix.value, out pos);

			Vector4 worldPositionW = new Vector4(pos, 1.0f);

			this.ms_ViewProjection.UpdateValue(this.frame);

			Vector2 drawTargetSize;
			int index = -1;
			DrawTarget.GetWidthHeightAsVector(out drawTargetSize,ref index);

			Vector4.Transform(ref worldPositionW, ref this.ms_ViewProjection.value, out worldPositionW);

			if (worldPositionW.W != 0)
				worldPositionW.W = 1.0f / worldPositionW.W;

			screenCoordinate = new Vector2(worldPositionW.X * worldPositionW.W,worldPositionW.Y * worldPositionW.W);

			screenCoordinate.X = drawTargetSize.X * (screenCoordinate.X * 0.5f + 0.5f);
			screenCoordinate.Y = drawTargetSize.Y * (screenCoordinate.Y * 0.5f + 0.5f);

			return worldPositionW.Z > 0;
		}

		/// <summary>
		/// Projects a position in screen coordinates into a 3D position in world space
		/// </summary>
		/// <param name="screenPosition">Position in screen space to project into world space</param>
		/// <param name="projectDepth">Depth to project from the camera position</param>
		/// <param name="position">projected position</param>
		public void ProjectFromScreen(ref Vector2 screenPosition, float projectDepth, out Vector3 position)
		{
			this.ms_ViewProjection_Inverse.UpdateValue(this.frame);

			Vector2 drawTargetSize;
			int index = -1;
			DrawTarget.GetWidthHeightAsVector(out drawTargetSize, ref index);

			Vector4 coordinate = new Vector4(0,0,0.5f,1);
			if (drawTargetSize.X != 0)
				coordinate.X = ((screenPosition.X / drawTargetSize.X) - 0.5f) * 2;
			if (drawTargetSize.Y != 0)
				coordinate.Y = ((screenPosition.Y / drawTargetSize.Y) - 0.5f) * 2;

			Vector4.Transform(ref coordinate, ref ms_ViewProjection_Inverse.value, out coordinate);

			if (coordinate.W != 0)
			{
				coordinate.W = 1.0f / coordinate.W;
				coordinate.X *= coordinate.W;
				coordinate.Y *= coordinate.W;
				coordinate.Z *= coordinate.W;
				coordinate.W = 1;
			}

			Vector3 cameraPos;
			camera.GetCameraPosition(out cameraPos);

			Vector3 difference = new Vector3();
			difference.X = coordinate.X - cameraPos.X;
			difference.Y = coordinate.Y - cameraPos.Y;
			difference.Z = coordinate.Z - cameraPos.Z;

			if (difference.X != 0 || difference.Y != 0 || difference.Y != 0)
				difference.Normalize();

			difference.X *= projectDepth;
			difference.Y *= projectDepth;
			difference.Z *= projectDepth;

			position = new Vector3();
			position.X = difference.X + cameraPos.X;
			position.Y = difference.Y + cameraPos.Y;
			position.Z = difference.Z + cameraPos.Z;
		}

		#endregion
	}
}
