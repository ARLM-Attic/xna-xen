using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Xen.Camera;
using Xen.Graphics.Modifier;
using Microsoft.Xna.Framework;

namespace Xen
{
	using Xen.Graphics.State;

	sealed partial class DrawState
	{
		private DeviceRenderState internalState = new DeviceRenderState();
		private DeviceRenderStateContainer visibleState = new DeviceRenderStateContainer();
		private uint renderStateStackIndex = 0;
		private StateFlag internalStateDirty = StateFlag.All;
		private Xen.Graphics.IShader boundShader;
		private Type boundShaderType;
		private bool boundShaderStateDirty = true;
		private int boundShaderWorldIndex = -1, boundShaderProjectionIndex = -1, boundShaderViewIndex = -1;
		private bool boundShaderUsesVertexCount = false, boundShaderUsesWorldMatrix = false, boundShaderUsesProjectionMatrix = false, boundShaderUsesViewMatrix = false;
		private VStream[] vertexStreams;
		private VertexDeclaration vertexDecl;
		private IndexBuffer indexBuffer;
		private int bufferVertexCount, bufferVertexCountChangeIndex = 1;

#if XBOX360
		/// <summary>
		///<para>there is a bug in XNA with render target clearing on the 360</para>
		///<para>the screen doesn't obey the RenderTargetUsage option when you *only* render to the screen</para>
		///<para>so keep track if a texture has been rendered to yet</para>
		/// </summary>
		internal bool nonScreenRenderComplete;
#endif

		internal IndexBuffer IndexBuffer
		{
			get { return indexBuffer; }
			set 
			{
				if (indexBuffer != value)
				{
					indexBuffer = value;
					graphics.Indices = value;
				}
			}
		}

		internal VertexDeclaration VertexDeclaration
		{
			get 
			{
				return vertexDecl; 
			}
			set 
			{
				if (vertexDecl != value)
				{
					vertexDecl = value;
					graphics.VertexDeclaration = value;
				}
			}
		}


		struct VStream
		{
			public VertexBuffer vb;
			public int offset;
			public int stride;
		}

		private readonly DeviceRenderState[] renderStateStack = new DeviceRenderState[renderStackSize];


		/// <summary>
		/// Gets the current <see cref="DeviceRenderState"/>. Members of this instance can be directly modified. Always push/pop the render state when drawing. To set the entire render state, see <see cref="PushRenderState(ref DeviceRenderState)"/> or <see cref="SetRenderState"/>
		/// </summary>
		/// <remarks><para>It is highly recommended that changes to render state are doen through this instance, rather than directly through the GraphicsDevice.</para>
		/// <para>If render state is changed through the GraphicsDevice, then the internal state cache will become invalid, and must be reset with a call to <see cref="DirtyInternalRenderState"/>, with the appropriate flags set indicating what parts of the render state have changed</para></remarks>
		public DeviceRenderStateContainer RenderState
		{
			get
			{
#if DEBUG
				ValidateRenderState();
#endif
				return visibleState; 
			}
		}

		internal void SetStream(int index, VertexBuffer vb, int offsetInBytes, int stride)
		{
			if (vertexStreams[index].vb != vb ||
				offsetInBytes != vertexStreams[index].offset ||
				stride != vertexStreams[index].stride)
			{
				graphics.Vertices[index].SetSource(vb, offsetInBytes, stride);
				vertexStreams[index].vb = vb;
				vertexStreams[index].offset = offsetInBytes;
				vertexStreams[index].stride = stride;
			}
		}

		internal void EndFrameCleanup()
		{
#if XBOX360
			//if a dynamic buffer is bound during Present, then then XNA can think the buffer
			//is being used in two states during the frame, which is incompatible with tiling.
			//technically this is incorrect, but it's just XNA being cautious, and easy to fix...

			IndexBuffer = null;

			for (int i = 0; i < this.vertexStreams.Length; i++)
				this.SetStream(i, null, 0, 0);

#endif
			boundVertexShader = null;
			boundPixelShader = null;

			vertexShaderConstantsToBind = null;
			pixelShaderConstantsToBind = null;

			vertexShaderBooleanConstantsToBind = null;
			pixelShaderBooleanConstantsToBind = null;
		}

		internal void UnbindBuffer(VertexBuffer vb)
		{
			if (vertexStreams == null)
				return;
			for (int i = 0; i < vertexStreams.Length; i++)
			{
				if (vertexStreams[i].vb == null)
					return;
				if (vertexStreams[i].vb == vb)
					SetStream(i, null, 0, 0);
			}
		}
		internal void UnbindBuffer(IndexBuffer ib)
		{
			if (this.IndexBuffer == ib)
				this.IndexBuffer = null;
		}

		/// <summary>
		/// Saves the current render state onto the render state stack. Reset the state back with a call to <see cref="PopRenderState"/>
		/// </summary>
		/// <remarks><para>If you wish to modify the render state temporarily, then it is best to call PushRenderState() before making change, then call <see cref="PopRenderState"/> after rendering is complete.</para><para>This will be a lot more efficient than manually storing the states that are changed</para></remarks>
		public void PushRenderState()
		{
#if DEBUG
			ValidateRenderState();
			if (renderStateStackIndex == renderStateStack.Length)
				throw new StackOverflowException("Render State stack is too small. Set DrawState.RenderStackSize to a larger value");
#endif

			renderStateStack[renderStateStackIndex++] = this.visibleState.state;
		}

		/// <summary>
		/// Overwrites the current render state with the provided state. To Save the previous state, see <see cref="PushRenderState(ref DeviceRenderState)"/>
		/// </summary>
		/// <param name="state"></param>
		public void SetRenderState(ref DeviceRenderState state)
		{
#if DEBUG
			ValidateRenderState();
#endif
			this.visibleState.state = state;
		}

		/// <summary>
		/// Saves the current render state onto the render state stack, then copies the provided render state in. Reset the state back with a call to <see cref="PopRenderState"/>
		/// </summary>
		/// <remarks><para>If you wish to modify the render state temporarily, then it is best to call this method before making change, then call <see cref="PopRenderState"/> after rendering is complete to restore the previous state.</para><para>This will be a lot more efficient than manually storing the states that are changed</para></remarks>
		public void PushRenderState(ref DeviceRenderState newState)
		{
#if DEBUG
			ValidateRenderState();
#endif
			PushRenderState();
			this.visibleState.state = newState;
		}

		/// <summary>
		/// Restores the last <see cref="DeviceRenderState"/> saved by a call to <see cref="PushRenderState()"/>
		/// </summary>
		public void PopRenderState()
		{
#if DEBUG
			ValidateRenderState();
#endif
			this.visibleState.state = renderStateStack[checked(--renderStateStackIndex)];
		}

		/// <summary>
		/// <para>Use this method to apply all changes made to the render state directly to the GraphicsDevice, use only before performing manual rendering through the graphics device (or when using standard XNA components)</para>
		/// <para>This method is automatically called by all internal Xen classes</para>
		/// </summary>
		/// <remarks>
		/// <para>You only need to call this method when directly rendering a mesh using the GraphicsDevice.</para>
		/// <para>For making a direct call to <see cref="GraphicsDevice.DrawIndexedPrimitives"/>, see <see cref="DrawVertexBuffer"/>.</para>
		/// </remarks>
		public void ApplyRenderStateChanges()
		{
#if DEBUG
			ValidateRenderState();
#endif
			ApplyRenderStateChanges(0);
		}

		internal void ApplyRenderStateChanges(int vertexCount)
		{
#if DEBUG
			ValidateRenderState();
#endif
			if (this.bufferVertexCount != vertexCount)
			{
				this.bufferVertexCountChangeIndex++;
				this.bufferVertexCount = vertexCount;
			}


			if (internalStateDirty != StateFlag.None)
			{
				visibleState.state.ResetState(internalStateDirty, ref internalState, graphics, cameraInvertsCullMode);
				internalStateDirty = StateFlag.None;
			}

			visibleState.state.ApplyState(ref internalState, graphics, cameraInvertsCullMode			
#if DEBUG
				,this
#endif
				);

			if (boundShader != null &&
				(boundShaderStateDirty ||
				(boundShaderUsesVertexCount) ||
				(boundShaderUsesWorldMatrix && boundShaderWorldIndex != ms_World.index) ||
				(boundShaderUsesProjectionMatrix && boundShaderProjectionIndex != ms_Projection.index) ||
				(boundShaderUsesViewMatrix && boundShaderViewIndex != ms_View.index) ||
				(boundShader.HasChanged)))
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.ShaderRebindCount);
#endif
				boundShader.Bind(this);
			}


			if (vertexShaderToBind != boundVertexShader)
			{
				graphics.VertexShader = vertexShaderToBind;
				boundVertexShader = vertexShaderToBind;
			}

			if (pixelShaderToBind != boundPixelShader)
			{
				graphics.PixelShader = pixelShaderToBind;
				boundPixelShader = pixelShaderToBind;
			}

			if (pixelShaderConstantsToBind != null)
			{
				graphics.SetPixelShaderConstant(0, pixelShaderConstantsToBind);
				pixelShaderConstantsToBind = null;
			}
			if (pixelShaderBooleanConstantsToBind != null)
			{
				graphics.SetPixelShaderConstant(0, pixelShaderBooleanConstantsToBind);
				pixelShaderBooleanConstantsToBind = null;
			}

			if (vertexShaderConstantsToBind != null)
			{
				graphics.SetVertexShaderConstant(0, vertexShaderConstantsToBind);
				vertexShaderConstantsToBind = null;
#if XBOX360
				//must reassign the vertex shader on the xbox whenever the shader constants have changed...
				boundVertexShader = null;
#endif
			}
			if (vertexShaderBooleanConstantsToBind != null)
			{
				graphics.SetVertexShaderConstant(0, vertexShaderBooleanConstantsToBind);
				vertexShaderBooleanConstantsToBind = null;
#if XBOX360
				boundVertexShader = null;
#endif
			}
		}

		/// <summary>
		/// <para>Gets the graphics device. WARNING: Use with caution. A matching call to <see cref="EndGetGraphicsDevice()"/> must be made when direct graphics device access is complete</para>
		/// <para><paramref name="dirtyDeviceState"/> must be set correctly. Use <see cref="StateFlag.None"/> when using the device to create a resource. Use the appropriate combination of <see cref="StateFlag"/> flags when (for example) binding an XNA Effect object that changes render state.</para>
		/// <para>If using the <see cref="GraphicsDevice"/> to change render state, prefer using <see cref="DrawState.RenderState"/> where states are implemented.</para>
		/// </summary>
		/// <param name="dirtyDeviceState">Set to the values of <see cref="StateFlag"/> that reflect the render states you plan to manually change, or you are using an XNA object that changes render state itself</param>
		/// <remarks><para>Currently <see cref="EndGetGraphicsDevice()"/> has no effect, and calls will not be validated.</para>
		/// <para>Graphics device access is setup in such a way to allow future updates to perform multithreaded rendering. It is expected that during a Begin/End GetGraphicsDevice block, all such optimisations will need to be disabled.</para></remarks>
		/// <returns></returns>
		public Microsoft.Xna.Framework.Graphics.GraphicsDevice BeginGetGraphicsDevice(StateFlag dirtyDeviceState)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.BeginGetGraphicsDeviceCount);
#endif
			//may do state tracking reset in the future, if optimizations require it, hence internal method above
			if (dirtyDeviceState != StateFlag.None)
			{
				if (protectedState)
					throw new ArgumentException("StateFlag.None is the only supported parameter in the current context.");
				DirtyInternalRenderState(dirtyDeviceState);
			}
			return graphics;
		}


		/// <summary>
		/// See <see cref="BeginGetGraphicsDevice"/> for details
		/// </summary>
		public void EndGetGraphicsDevice()
		{
		}
		/// <summary>
		/// See <see cref="BeginGetGraphicsDevice"/> for details
		/// </summary>
		/// <param name="dirtyDeviceState"></param>
		public void EndGetGraphicsDevice(StateFlag dirtyDeviceState)
		{
			if (dirtyDeviceState != StateFlag.None)
			{
				if (protectedState)
					throw new ArgumentException("StateFlag.None is the only supported parameter in the current context.");
				DirtyInternalRenderState(dirtyDeviceState);
			}
		}

		/// <summary>
		/// <para>WARNING: Use with caution. Dirties internally tracked render state caches</para>
		/// <para>See documentation for <see cref="BeginGetGraphicsDevice"/>.</para>
		/// </summary>
		/// <param name="dirtyState"></param>
		public void DirtyInternalRenderState(StateFlag dirtyState)
		{
#if DEBUG
			System.Threading.Interlocked.Increment(ref application.currentFrame.DirtyRenderStateCount);
#endif
			ValidateProtected();

			if ((dirtyState & StateFlag.Shaders) != 0)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DirtyRenderShadersStateCount);
#endif
				boundShader = null;
				boundShaderType = null;
				boundShaderStateDirty = true;
			}

			if ((dirtyState & StateFlag.VerticesAndIndices) != 0)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DirtyRenderVerticesAndIndicesStateCount);
#endif
				for (int i = 0; i < vertexStreams.Length; i++)
				{
					vertexStreams[i].vb = null;
					vertexStreams[i].offset = -1;
					vertexStreams[i].stride = -1;
				}
				vertexDecl = null;
				indexBuffer = null;
			}

			if ((dirtyState & StateFlag.Textures) != 0)
			{
#if DEBUG
				System.Threading.Interlocked.Increment(ref application.currentFrame.DirtyRenderTexturesStateCount);
#endif
				boundShaderStateDirty = true;
				boundShader = null;
				for (int i = 0; i < psSamplerDirty.Length; i++)
					psSamplerDirty[i] = true;
				for (int i = 0; i < vsSamplerDirty.Length; i++)
					vsSamplerDirty[i] = true;

				for (int i = 0; i < psTextures.Length; i++)
					psTextures[i] = null;
				for (int i = 0; i < vsTextures.Length; i++)
					vsTextures[i] = null;

#if DEBUG
				for (int i = 0; i < psTexturesDEBUG.Length; i++)
					psTexturesDEBUG[i] = null;
				for (int i = 0; i < vsTexturesDEBUG.Length; i++)
					vsTexturesDEBUG[i] = null;
#endif
			}

			internalStateDirty |= dirtyState;
		}

		internal void ResetTextures()
		{
			for (int i = 0; i < psTextures.Length; i++)
			{
				if (psTextures[i] != null)
				{
					graphics.Textures[i] = null;
					psTextures[i] = null;
				}
			}
			for (int i = 0; i < vsTextures.Length; i++)
			{
				if (psTextures[i] != null)
				{
					graphics.VertexTextures[i] = null;
					vsTextures[i] = null;
				}
			}
#if DEBUG
			for (int i = 0; i < psTexturesDEBUG.Length; i++)
				psTexturesDEBUG[i] = null;
			for (int i = 0; i < vsTexturesDEBUG.Length; i++)
				vsTexturesDEBUG[i] = null;
#endif
			boundShader = null;
		}
	}
}