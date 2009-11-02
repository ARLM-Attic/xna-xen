using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Xen.Graphics.ShaderSystem.Constants;
using Xen.Graphics;
using Xen.Graphics.ShaderSystem;
using Xen.Graphics.State;

namespace Xen.Graphics
{
	/// <summary>
	/// Allows for dynamic use of the shader
	/// </summary>
	public interface IShader
	{
		/// <summary>
		/// Bind the shader - (this method is called by the application)
		/// </summary>
		/// <param name="state">Must not be null</param>
		void Bind(IShaderSystem state);
		/// <summary>
		/// True if a non-global has changed since this shader was last bound
		/// </summary>
		/// <returns></returns>
		bool HasChanged { get; }

		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, bool value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, ref Matrix value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, ref Vector4 value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, ref Vector3 value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, ref Vector2 value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, float value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, Matrix[] value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, Vector4[] value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, Vector3[] value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, Vector2[] value);
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		bool SetAttribute(IShaderSystem state, int name_uid, float[] value);
		/// <summary>
		/// Set a shader texture
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the texture name</param>
		/// <param name="texture">texture to set</param>
		/// <returns>true if the texture was set</returns>
		bool SetTexture(IShaderSystem state, int name_uid, Texture texture);
		/// <summary>
		/// Set a shader texture
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the texture name</param>
		/// <param name="texture">texture to set</param>
		/// <returns>true if the texture was set</returns>
		bool SetTexture(IShaderSystem state, int name_uid, Texture2D texture);
		/// <summary>
		/// Set a shader texture
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the texture name</param>
		/// <param name="texture">texture to set</param>
		/// <returns>true if the texture was set</returns>
		bool SetTexture(IShaderSystem state, int name_uid, Texture3D texture);
		/// <summary>
		/// Set a shader texture
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the texture name</param>
		/// <param name="texture">texture to set</param>
		/// <returns>true if the texture was set</returns>
		bool SetTexture(IShaderSystem state, int name_uid, TextureCube texture);
		/// <summary>
		/// Set a shader texture sampler state
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the sampler state name</param>
		/// <param name="sampler">sampler state to set</param>
		/// <returns>true if the sampler state was set</returns>
		bool SetSamplerState(IShaderSystem state, int name_uid, TextureSamplerState sampler);
		/// <summary>
		/// Get the number of vertex inputs required by the shader
		/// </summary>
		/// <returns>Returns the number of vertex inputs required by the shader</returns>
		int GetVertexInputCount();
		/// <summary>
		/// Gets a vertex input required by the shader, by index. <see cref="GetVertexInputCount"/> to get the number of inputs required.
		/// </summary>
		/// <param name="index">Index of the element</param>
		/// <param name="elementUsage">Gets the usage type of the vertex element (eg, <see cref="VertexElementUsage.Position"/>)</param>
		/// <param name="elementIndex">Gets the index of the vertex element (eg, there may be more than one <see cref="VertexElementUsage.TextureCoordinate"/>)</param>
		/// <remarks><para>Implementations should return elements in logical order, first all Position (0) elements in elementIndex order, all BlendWeights (1), etc.</para></remarks>
		void GetVertexInput(int index, out VertexElementUsage elementUsage, out int elementIndex);
	}
}

namespace Xen.Graphics.ShaderSystem
{
	/// <summary>
	/// Base shader class, provides empty implementations of the IShader methods, to reduce generated code
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough]
	public abstract class BaseShader : IShader
	{
		/// <summary>
		/// True if this shader instance owns it's own vertex and pixel shaders
		/// </summary>
		protected bool owner = true;

		internal void DisableShaderOwner()
		{
			owner = false;
		}

		/// <summary>
		/// Bind the shader - (this method is called by the application)
		/// </summary>
		/// <param name="state">Must not be null</param>
		public abstract void Bind(IShaderSystem state);
		/// <summary>
		/// True if a non-global has changed since this shader was last bound
		/// </summary>
		/// <returns></returns>
		protected abstract bool Changed();
		bool IShader.HasChanged { get { return Changed(); } }

		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, bool value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, ref Matrix value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, ref Vector4 value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, ref Vector3 value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, ref Vector2 value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, float value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, Matrix[] value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, Vector4[] value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, Vector3[] value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, Vector2[] value) { return false; }
		/// <summary>
		/// Set a shader attribute
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the attribute name</param>
		/// <param name="value">value to set</param>
		/// <returns>true if the value was set</returns>
		protected virtual bool SetAttribute(IShaderSystem state, int name_uid, float[] value) { return false; }
		/// <summary>
		/// Set a shader texture
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the texture name</param>
		/// <param name="texture">texture to set</param>
		/// <returns>true if the texture was set</returns>
		protected virtual bool SetTexture(IShaderSystem state, int name_uid, Texture texture) { return false; }
		/// <summary>
		/// Set a shader texture
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the texture name</param>
		/// <param name="texture">texture to set</param>
		/// <returns>true if the texture was set</returns>
		protected virtual bool SetTexture(IShaderSystem state, int name_uid, Texture2D texture) { return false; }
		/// <summary>
		/// Set a shader texture
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the texture name</param>
		/// <param name="texture">texture to set</param>
		/// <returns>true if the texture was set</returns>
		protected virtual bool SetTexture(IShaderSystem state, int name_uid, Texture3D texture) { return false; }
		/// <summary>
		/// Set a shader texture
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the texture name</param>
		/// <param name="texture">texture to set</param>
		/// <returns>true if the texture was set</returns>
		protected virtual bool SetTexture(IShaderSystem state, int name_uid, TextureCube texture) { return false; }
		/// <summary>
		/// Set a shader texture sampler state
		/// </summary>
		/// <param name="state"></param>
		/// <param name="name_uid">unique id of the sampler state name</param>
		/// <param name="sampler">sampler state to set</param>
		/// <returns>true if the sampler state was set</returns>
		protected virtual bool SetSamplerState(IShaderSystem state, int name_uid, TextureSamplerState sampler) { return false; }

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, bool value) { return SetAttribute(state,name_uid,value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, ref Matrix value) { return SetAttribute(state,name_uid,ref value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, ref Vector4 value) { return SetAttribute(state, name_uid, ref value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, ref Vector3 value) { return SetAttribute(state, name_uid, ref value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, ref Vector2 value) { return SetAttribute(state, name_uid, ref value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, float value) { return SetAttribute(state, name_uid, value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, Matrix[] value) { return SetAttribute(state, name_uid, value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, Vector4[] value) { return SetAttribute(state, name_uid, value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, Vector3[] value) { return SetAttribute(state, name_uid, value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, Vector2[] value) { return SetAttribute(state, name_uid, value); }
		bool IShader.SetAttribute(IShaderSystem state, int name_uid, float[] value) { return SetAttribute(state, name_uid, value); }
		bool IShader.SetTexture(IShaderSystem state, int name_uid, Texture texture) { return SetTexture(state,name_uid,texture); }
		bool IShader.SetTexture(IShaderSystem state, int name_uid, Texture2D texture) { return SetTexture(state, name_uid, texture); }
		bool IShader.SetTexture(IShaderSystem state, int name_uid, Texture3D texture) { return SetTexture(state, name_uid, texture); }
		bool IShader.SetTexture(IShaderSystem state, int name_uid, TextureCube texture) { return SetTexture(state, name_uid, texture); }
		bool IShader.SetSamplerState(IShaderSystem state, int name_uid, TextureSamplerState sampler) { return SetSamplerState(state, name_uid, sampler); }

		/// <summary>
		/// Get the number of vertex inputs required by the shader
		/// </summary>
		/// <returns>Returns the number of vertex inputs required by the shader</returns>
		internal protected abstract int GetVertexInputCount();
		/// <summary>
		/// Gets a vertex input required by the shader, by index. <see cref="GetVertexInputCount"/> to get the number of inputs required.
		/// </summary>
		/// <param name="index">Index of the element</param>
		/// <param name="elementUsage">Gets the usage type of the vertex element (eg, <see cref="VertexElementUsage.Position"/>)</param>
		/// <param name="elementIndex">Gets the index of the vertex element (eg, there may be more than one <see cref="VertexElementUsage.TextureCoordinate"/>)</param>
		internal protected abstract void GetVertexInput(int index, out VertexElementUsage elementUsage, out int elementIndex);


		/// <summary>
		/// Get the number of vertex inputs required by the shader
		/// </summary>
		/// <returns>Returns the number of vertex inputs required by the shader</returns>
		int IShader.GetVertexInputCount() { return GetVertexInputCount(); }
		/// <summary>
		/// Gets a vertex input required by the shader, by index. <see cref="GetVertexInputCount"/> to get the number of inputs required.
		/// </summary>
		/// <param name="index">Index of the element</param>
		/// <param name="elementUsage">Gets the usage type of the vertex element (eg, <see cref="VertexElementUsage.Position"/>)</param>
		/// <param name="elementIndex">Gets the index of the vertex element (eg, there may be more than one <see cref="VertexElementUsage.TextureCoordinate"/>)</param>
		void IShader.GetVertexInput(int index, out VertexElementUsage elementUsage, out int elementIndex)
		{
			GetVertexInput(index, out elementUsage, out elementIndex);
		}

		/// <summary>
		/// Preload shader resources
		/// </summary>
		/// <param name="state"></param>
		public void Warm(IShaderSystem state)
		{
			WarmShader(state);
		}
		/// <summary>
		/// Warm shader implementation
		/// </summary>
		/// <param name="state"></param>
		protected abstract void WarmShader(IShaderSystem state);

		/// <summary>
		/// Gets an array of integer values that give an approximate identification hash for the shader constants and their use by this shader
		/// </summary>
		/// <param name="ps">return pixel shader indices</param>
		/// <returns></returns>
		/// <remarks>Returned values can only be used to compare constant usage between shaders. It is possible to get the same values between shaders, however unlikely</remarks>
		protected abstract int[] GetShaderConstantHash(bool ps);

		/// <summary>
		/// <para>Attempts to merge two shader instances, sharing their vertex and pixel shaders into a single shader instance.</para>
		/// <para>This method requires that both shaders implement compatible shader constant logic (This requirement is only partially validated)</para>
		/// </summary>
		/// <param name="vsSource"></param>
		/// <param name="psSource"></param>
		/// <returns></returns>
		public static IShader Merge(BaseShader vsSource, BaseShader psSource)
		{
			if (psSource == null || vsSource == null)
				throw new ArgumentNullException();
			if (vsSource.GetType() == psSource.GetType())
				return vsSource;

			int[] vsc_vs = vsSource.GetShaderConstantHash(false);
			int[] psc_vs = psSource.GetShaderConstantHash(false);

			int[] vsc_ps = vsSource.GetShaderConstantHash(true);
			int[] psc_ps = psSource.GetShaderConstantHash(true);


			int len_vsc_vs = vsc_vs[vsc_vs.Length-1];
			int len_psc_vs = psc_vs[psc_vs.Length - 1];

			int len_vsc_ps = vsc_ps[vsc_ps.Length - 1];
			int len_psc_ps = psc_ps[psc_ps.Length - 1];

			//attempt to match vsSource to psSource by pixel shader...

			bool vsourceMatchByPs = true;

			if (len_psc_ps > len_vsc_ps || psc_ps.Length > vsc_ps.Length)
				vsourceMatchByPs = false; // vsource ps is shorter than psource ps, no match
			else
			{
				//match args
				int args = psc_ps.Length / 3;

				for (int i = 0; i < args; i++)
				{
					//format is {reg,count | rank << 16, namehash}

					int reg_ps = psc_ps[i * 3 + 0];
					int count_ps = psc_ps[i * 3 + 1] & 0xFF;
					int rank_ps = (psc_ps[i * 3 + 1] >> 16) & 0xFF;
					int hash_ps = psc_ps[i * 3 + 2];

					int reg_vs = vsc_ps[i * 3 + 0];
					int count_vs = vsc_ps[i * 3 + 1] & 0xFF;
					int rank_vs = (vsc_ps[i * 3 + 1] >> 16) & 0xFF;
					int hash_vs = vsc_ps[i * 3 + 2];

					if (reg_ps != reg_vs) // different start register
						vsourceMatchByPs = false;
					if (rank_ps != rank_vs) // different rank
						vsourceMatchByPs = false;
					if (count_ps > count_vs) // different count
						vsourceMatchByPs = false;
					if (hash_ps != hash_vs) // different name for the register
						vsourceMatchByPs = false;
				}
			}




			bool psourceMatchByVs = true;

			if (len_vsc_vs > len_psc_vs || vsc_vs.Length > psc_vs.Length)
				psourceMatchByVs = false; // psource vs is shorter than vsource vs, no match
			else
			{
				//match args
				int args = vsc_vs.Length / 3;

				for (int i = 0; i < args; i++)
				{
					//format is {reg,count | rank << 16, namehash}

					int reg_ps = psc_vs[i * 3 + 0];
					int count_ps = psc_vs[i * 3 + 1] & 0xFF;
					int rank_ps = (psc_vs[i * 3 + 1] >> 16) & 0xFF;
					int hash_ps = psc_vs[i * 3 + 2];

					int reg_vs = vsc_vs[i * 3 + 0];
					int count_vs = vsc_vs[i * 3 + 1] & 0xFF;
					int rank_vs = (vsc_vs[i * 3 + 1] >> 16) & 0xFF;
					int hash_vs = vsc_vs[i * 3 + 2];

					if (reg_ps != reg_vs) // different start register
						psourceMatchByVs = false;
					if (rank_ps != rank_vs) // different rank
						psourceMatchByVs = false;
					if (count_ps < count_vs) // different count
						psourceMatchByVs = false;
					if (hash_ps != hash_vs) // different name for the register
						psourceMatchByVs = false;
				}
			}


			if (!psourceMatchByVs && !vsourceMatchByPs)
				throw new InvalidOperationException("Shaders do not share compatible constant arrays");

			Type baseType = null;

			if (psourceMatchByVs)
			{
				baseType = psSource.GetType();
			}
			if (vsourceMatchByPs)
			{
				//vsSource has bigger constants, use it as the base
				baseType = vsSource.GetType();
			}

			return new MergedShader(baseType, vsSource, psSource);
		}
	}

	[System.Diagnostics.DebuggerStepThrough]
	sealed class MergedShader : IShader
	{
		private readonly BaseShader logicParent;
		private readonly BaseShader vsParent, psParent;
		private VertexShader vs;
		private PixelShader ps;
		private int deviceId;

		public MergedShader(Type baseType, BaseShader vs, BaseShader ps)
		{
			this.logicParent = (BaseShader)Activator.CreateInstance(baseType);
			this.logicParent.DisableShaderOwner();
			this.vsParent = vs;
			this.psParent = ps;
		}

		public void Bind(IShaderSystem state)
		{
			int id = state.DeviceUniqueIndex;
			if (id != deviceId)
			{
				deviceId = id;
				vsParent.Warm(state);
				psParent.Warm(state);
				PullShaders();
			}

			state.SetShaders(vs, ps);
			logicParent.Bind(state);
		}

		void PullShaders()
		{
			this.vs = (VertexShader)vsParent.GetType().GetField("vs", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
			this.ps = (PixelShader)psParent.GetType().GetField("ps", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
		}

		public bool HasChanged
		{
			get { return ((IShader)logicParent).HasChanged; }
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, bool value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, ref Matrix value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, ref value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, ref Vector4 value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, ref value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, ref Vector3 value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, ref value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, ref Vector2 value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, ref value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, float value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, Matrix[] value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, Vector4[] value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, Vector3[] value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, Vector2[] value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, value);
		}

		public bool SetAttribute(IShaderSystem state, int name_uid, float[] value)
		{
			return ((IShader)logicParent).SetAttribute(state, name_uid, value);
		}

		public bool SetTexture(IShaderSystem state, int name_uid, Texture texture)
		{
			return ((IShader)logicParent).SetTexture(state, name_uid, texture);
		}

		public bool SetTexture(IShaderSystem state, int name_uid, Texture2D texture)
		{
			return ((IShader)logicParent).SetTexture(state, name_uid, texture);
		}

		public bool SetTexture(IShaderSystem state, int name_uid, Texture3D texture)
		{
			return ((IShader)logicParent).SetTexture(state, name_uid, texture);
		}

		public bool SetTexture(IShaderSystem state, int name_uid, TextureCube texture)
		{
			return ((IShader)logicParent).SetTexture(state, name_uid, texture);
		}

		public bool SetSamplerState(IShaderSystem state, int name_uid, TextureSamplerState sampler)
		{
			return ((IShader)logicParent).SetSamplerState(state, name_uid, sampler);
		}


		public int GetVertexInputCount()
		{
			return logicParent.GetVertexInputCount();
		}

		public void GetVertexInput(int index, out VertexElementUsage elementUsage, out int elementIndex)
		{
			logicParent.GetVertexInput(index, out elementUsage, out elementIndex);
		}
	}

	/// <summary>
	/// Interface for setting shader constants with common semantic such as 'WORLDVIEWPROJECTION' and the special 'GLOBAL' semantic. All methods are called back by the shader
	/// </summary>
	public interface IShaderSystem
	{
		/// <summary>Will be called before calls to SetXXX()</summary>
		/// <returns>System-wide unique non-zero index for the current application graphics device (if this index changes, the shader will recreate itself automatically)</returns>
		/// <param name="pixelShaderSamplersUsed"></param>
		/// <param name="vertexShaderSamplersUsed"></param>
		/// <param name="instanceChanged"></param>
		/// <param name="shader"></param>
		/// <param name="typeChanged"></param>
		/// <remarks><para>if typeChanged is set to true, instanceChanged must also be true</para>
		/// <para>Implementation may always set true to both values if state tracking isn't supported</para></remarks>
		int Begin(IShader shader, int pixelShaderSamplersUsed, int vertexShaderSamplersUsed, out bool typeChanged, out bool instanceChanged);

		/// <summary>System-wide unique non-zero index for the current application graphics device (if this index changes, the shader will recreate itself automatically)</summary>
		/// <returns></returns>
		int DeviceUniqueIndex { get; }

		/// <summary>
		/// Create vertex and pixel shaders
		/// </summary>
		/// <param name="vertexShader"></param>
		/// <param name="pixelShader"></param>
		/// <param name="vShaderBytes"></param>
		/// <param name="pShaderBytes"></param>
		/// <param name="psInstructionCount">approximate</param>
		/// <param name="psPreShaderInstructionCount">approximate</param>
		/// <param name="vsInstructionCount">approximate</param>
		/// <param name="vsPreShaderInstructionCount">approximate</param>
		void CreateShaders(out VertexShader vertexShader, out PixelShader pixelShader, byte[] vShaderBytes, byte[] pShaderBytes, int vsInstructionCount, int psInstructionCount, int vsPreShaderInstructionCount, int psPreShaderInstructionCount);
		/// <summary>
		/// Set the vertex and pixel shaders
		/// </summary>
		/// <param name="vertexShader"></param>
		/// <param name="pixelShader"></param>
		void SetShaders(VertexShader vertexShader, PixelShader pixelShader);
		/// <summary>
		/// Set shader constants
		/// </summary>
		/// <param name="vertexShaderConstants">may be null</param>
		/// <param name="pixelShaderConstants">may be null</param>
		void SetShaderConstants(Vector4[] vertexShaderConstants, Vector4[] pixelShaderConstants);
		/// <summary>
		/// Set shader boolean constants
		/// </summary>
		/// <param name="vertexShaderBooleanConstants">may be null</param>
		/// <param name="pixelShaderBooleanConstants">may be null</param>
		void SetShaderBooleanConstants(bool[] vertexShaderBooleanConstants, bool[] pixelShaderBooleanConstants);

		//the following methods are auto-bound by the plugin,
		//ie : WORLDVIEWPROJECTION on a matrix will translate to SET+WORLDVIEWPROJECTION+MATRIX
		//which will auto reference the method below.
		//adding more methods in this format will automatically get used
		//make sure the type is matched correctly, otherwise the tool will fail
		//note: only standard types in Microsoft.Xna are supported (no custom structures)

		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldViewProjectionMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldViewProjectionInverseMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldViewProjectionTransposeMatrix(IValue<Matrix> value, ref int changeIndex);

		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldProjectionMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldProjectionInverseMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldProjectionTransposeMatrix(IValue<Matrix> value, ref int changeIndex);

		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewProjectionMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewProjectionInverseMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewProjectionTransposeMatrix(IValue<Matrix> value, ref int changeIndex);

		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldViewMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldViewInverseMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldViewTransposeMatrix(IValue<Matrix> value, ref int changeIndex);

		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldInverseMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWorldTransposeMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewInverseMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewTransposeMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetProjectionMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetProjectionInverseMatrix(IValue<Matrix> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetProjectionTransposeMatrix(IValue<Matrix> value, ref int changeIndex);

		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetCameraNearFarVector2(IValue<Vector2> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetCameraFovVector2(IValue<Vector2> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetCameraFovTangentVector2(IValue<Vector2> value, ref int changeIndex);

		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewDirectionVector3(IValue<Vector3> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewDirectionVector4(IValue<Vector4> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewPointVector3(IValue<Vector3> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetViewPointVector4(IValue<Vector4> value, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetWindowSizeVector2(IValue<Vector2> value, ref int changeIndex);

		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="changeIndex"></param>
		void SetVertexCountSingle(IValue<float> value, ref int changeIndex);

		/// <summary>
		/// Get the unique id for the name of a global type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		int GetGlobalUniqueID<T>(string name);
		/// <summary>
		/// Get the unique id for the name of an attibute, texture or sampler
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		int GetNameUniqueID(string name);
		/// <summary>Method used by a generated shader</summary><param name="array"></param><param name="index"></param><param name="gloabl_uid"></param><param name="changed"></param>
		void SetGlobal(bool[] array, int index, int gloabl_uid, ref bool changed);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IValue<Vector4> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IValue<Vector3> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IValue<Vector2> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IValue<Single> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IValue<Matrix> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IArray<Vector4> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IArray<Vector3> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IArray<Vector2> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IArray<Single> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="value"></param><param name="gloabl_uid"></param><param name="changeIndex"></param>
		void SetGlobal(IArray<Matrix> value, int gloabl_uid, ref int changeIndex);
		/// <summary>Method used by a generated shader</summary><param name="gloabl_uid"></param><returns></returns>
		Texture GetGlobalTexture(int gloabl_uid);
		/// <summary>Method used by a generated shader</summary><param name="gloabl_uid"></param><returns></returns>
		Texture2D GetGlobalTexture2D(int gloabl_uid);
		/// <summary>Method used by a generated shader</summary><param name="gloabl_uid"></param><returns></returns>
		Texture3D GetGlobalTexture3D(int gloabl_uid);
		/// <summary>Method used by a generated shader</summary><param name="gloabl_uid"></param><returns></returns>
		TextureCube GetGlobalTextureCube(int gloabl_uid);

		/// <summary>Method used by a generated shader</summary><param name="index"></param><param name="texture"></param><param name="state"></param>
		void SetPixelShaderSampler(int index, Texture texture, TextureSamplerState state);
		/// <summary>Method used by a generated shader</summary><param name="index"></param><param name="texture"></param><param name="state"></param>
		void SetVertexShaderSampler(int index, Texture texture, TextureSamplerState state);
		/*
		/// <summary>Set a global based on the unique identifier number for the constant name</summary><param name="value"></param><param name="name_uid"></param>
		void SetGlobal(IValue<Vector4> value, int name_uid);
		/// <summary>Set a global based on the unique identifier number for the constant name</summary><param name="value"></param><param name="name_uid"></param>
		void SetGlobal(IValue<Vector3> value, int name_uid);
		/// <summary>Set a global based on the unique identifier number for the constant name</summary><param name="value"></param><param name="name_uid"></param>
		void SetGlobal(IValue<Vector2> value, int name_uid);
		/// <summary>Set a global based on the unique identifier number for the constant name</summary><param name="value"></param><param name="name_uid"></param>
		void SetGlobal(IValue<Matrix> value, int name_uid);
		/// <summary>Set a global based on the unique identifier number for the constant name</summary><param name="value"></param><param name="name_uid"></param>
		void SetGlobal(IValue<float> value, int name_uid);
		/// <summary>Set a global based on the unique identifier number for the constant name</summary><param name="value"></param><param name="name_uid"></param><param name="expectedSize"></param>
		void SetGlobal(IValueArray value, int name_uid, int expectedSize);
		 */
	}

	namespace Constants
	{

		#region constants
		/// <summary>
		/// Simple wrapper class for a constant vector array
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class ConstantArray : IValue<Matrix>, IValue<Vector2>, IValue<Vector3>, IValue<Vector4>, IValue<float>
		{
			/// <summary>
			/// Common utilitiy methods used by shaders, including very simple runlength compression to help compiled shaders a tad.
			/// </summary>
			[System.Diagnostics.DebuggerStepThrough]
			public static class ArrayUtils
			{
				//	static Dictionary<string, int> ids = new Dictionary<string, int>();

				///// <summary>
				///// Gets a unique id from a string name. Used by constant and texture references
				///// </summary>
				///// <param name="name"></param>
				///// <returns></returns>
				//public static int GetUniqueID(string name)
				//{
				//    lock (ids)
				//    {
				//        int index;
				//        if (!ids.TryGetValue(name, out index))
				//        {
				//            index = ids.Count;
				//            ids.Add(name, index);
				//        }
				//        return index;
				//    }
				//}
				/// <summary>
				/// Decompress a byte array with very simple compression
				/// </summary>
				/// <param name="data"></param>
				/// <returns></returns>
				public static byte[] SimpleDecompress(params byte[] data)
				{
					List<byte> output = new List<byte>();

					int i = 0;
					while (i < data.Length)
					{
						if (data[i] > 127)
						{
							for (int n = 128; n < data[i]; n++)
								output.Add(data[i + 1]);
							i += 2;
						}
						else
						{
							int lenght = data[i++];
							for (int n = 0; n < lenght; n++)
								output.Add(data[i++]);
						}
					}
					return output.ToArray();
				}

				/// <summary>
				/// Compress a byte array with very simple compression
				/// </summary>
				/// <param name="data"></param>
				/// <returns></returns>
				public static byte[] SimpleCompress(byte[] data)
				{
					List<byte> output = new List<byte>();
					output.Add(0);
					int counter = 0;

					for (int i = 0; i < data.Length; )
					{
						int n = i + 1;
						while (n < data.Length)
						{
							if (data[i] != data[n] || n - i > 100)
								break;
							n++;
						}
						if (n == data.Length)
							n--;

						if (n - i > 2)
						{
							output.Add((byte)(n - i + 128));
							output.Add(data[i]);
							while (i < n)
							{
								i++;
							}
							counter = output.Count;
							output.Add(0);
						}
						else
						{
							if (i - counter > 100)
							{
								counter = output.Count;
								output.Add(0);
							}

							output[counter]++;
							output.Add(data[i++]);
						}
					}
					return output.ToArray();
				}
			}

			#region IValue get setter

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public IValue<Matrix> Matrix4Transpose(int index)
			{
				this.index = index;
				return this;
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public IValue<Matrix> Matrix3Transpose(int index)
			{
				if (maskedSetter == null)
					maskedSetter = new MatrixSetter(this);
				maskedSetter.matrixCopyMask = 3;
				this.index = index;
				return maskedSetter;
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public IValue<Matrix> Matrix2Transpose(int index)
			{
				if (maskedSetter == null)
					maskedSetter = new MatrixSetter(this);
				maskedSetter.matrixCopyMask = 1;
				this.index = index;
				return maskedSetter;
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public IValue<Matrix> Matrix1Transpose(int index)
			{
				if (maskedSetter == null)
					maskedSetter = new MatrixSetter(this);
				maskedSetter.matrixCopyMask = 0;
				this.index = index;
				return maskedSetter;
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public IValue<Vector4> Vector4(int index)
			{
				this.index = index;
				return this;
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public IValue<Vector3> Vector3(int index)
			{
				this.index = index;
				return this;
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public IValue<Vector2> Vector2(int index)
			{
				this.index = index;
				return this;
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public IValue<float> Single(int index)
			{
				this.index = index;
				return this;
			}

			[System.Diagnostics.DebuggerStepThrough]
			class MatrixSetter : IValue<Matrix>
			{
				private readonly ConstantArray parent;
				public int matrixCopyMask = 0xFF; 

				public MatrixSetter(ConstantArray parent)
				{
					this.parent = parent;
				}
				void IValue<Matrix>.Set(ref Matrix value)
				{
					bool copyRow2 = (matrixCopyMask & 1) == 1;
					bool copyRow3 = (matrixCopyMask & 2) == 2;
					bool copyRow4 = (matrixCopyMask & 4) == 4;

					parent.change = true;
					int index = parent.index;


					parent.array[index].X = value.M11;
					parent.array[index].Y = value.M21;
					parent.array[index].Z = value.M31;
					parent.array[index].W = value.M41;
					++index;
					if (copyRow2)
					{
						parent.array[index].X = value.M12;
						parent.array[index].Y = value.M22;
						parent.array[index].Z = value.M32;
						parent.array[index].W = value.M42;
						++index;
						if (copyRow3)
						{
							parent.array[index].X = value.M13;
							parent.array[index].Y = value.M23;
							parent.array[index].Z = value.M33;
							parent.array[index].W = value.M43;
							++index;
							if (copyRow4)
							{
								parent.array[index].X = value.M14;
								parent.array[index].Y = value.M24;
								parent.array[index].Z = value.M34;
								parent.array[index].W = value.M44;
							}
						}
					}
				}
			}

			#endregion

			#region interface set

			/// <summary></summary>
			/// <param name="value"></param>
			void IValue<Matrix>.Set(ref Matrix value)
			{
				change = true;
				int index = this.index;

				array[index].X = value.M11;
				array[index].Y = value.M21;
				array[index].Z = value.M31;
				array[index].W = value.M41;
				++index;

				array[index].X = value.M12;
				array[index].Y = value.M22;
				array[index].Z = value.M32;
				array[index].W = value.M42;
				++index;

				array[index].X = value.M13;
				array[index].Y = value.M23;
				array[index].Z = value.M33;
				array[index].W = value.M43;
				++index;

				array[index].X = value.M14;
				array[index].Y = value.M24;
				array[index].Z = value.M34;
				array[index].W = value.M44;
			}

			/// <summary></summary>
			/// <param name="value"></param>
			void IValue<Vector4>.Set(ref Vector4 value)
			{
				change = true;
				array[index] = value;
			}

			/// <summary></summary>
			/// <param name="value"></param>
			void IValue<Vector3>.Set(ref Vector3 value)
			{
				change = true;
				array[index].X = value.X;
				array[index].Y = value.Y;
				array[index].Z = value.Z;
			}

			/// <summary></summary>
			/// <param name="value"></param>
			void IValue<Vector2>.Set(ref Vector2 value)
			{
				change = true;
				array[index].X = value.X;
				array[index].Y = value.Y;
			}

			/// <summary></summary>
			/// <param name="value"></param>
			void IValue<float>.Set(ref float value)
			{
				change = true;
				array[index].X = value;
			}

			#endregion

			#region explicit set

			#region set mat


			/// <summary>Method used by a generated shader</summary>
			/// <param name="value"></param>
			/// <param name="index"></param>
			public void SetMatrix4Transpose(int index, ref Matrix value)
			{
				if (array[index].X != value.M11 ||
					array[index].Y != value.M21 ||
					array[index].Z != value.M31 ||
					array[index].W != value.M41)
				{
					array[index].X = value.M11;
					array[index].Y = value.M21;
					array[index].Z = value.M31;
					array[index].W = value.M41;
					change = true;
				}

				index++;
				if (array[index].X != value.M12 ||
					array[index].Y != value.M22 ||
					array[index].Z != value.M32 ||
					array[index].W != value.M42)
				{
					array[index].X = value.M12;
					array[index].Y = value.M22;
					array[index].Z = value.M32;
					array[index].W = value.M42;
					change = true;
				}

				index++;
				if (array[index].X != value.M13 ||
					array[index].Y != value.M23 ||
					array[index].Z != value.M33 ||
					array[index].W != value.M43)
				{
					array[index].X = value.M13;
					array[index].Y = value.M23;
					array[index].Z = value.M33;
					array[index].W = value.M43;
					change = true;
				}

				index++;
				if (array[index].X != value.M14 ||
					array[index].Y != value.M24 ||
					array[index].Z != value.M34 ||
					array[index].W != value.M44)
				{
					array[index].X = value.M14;
					array[index].Y = value.M24;
					array[index].Z = value.M34;
					array[index].W = value.M44;

					change = true;
				}
			}



			/// <summary>Method used by a generated shader</summary>
			/// <param name="value"></param>
			/// <param name="index"></param>
			public void SetMatrix3Transpose(int index, ref Matrix value)
			{
				if (array[index].X != value.M11 ||
					array[index].Y != value.M21 ||
					array[index].Z != value.M31 ||
					array[index].W != value.M41)
				{
					array[index].X = value.M11;
					array[index].Y = value.M21;
					array[index].Z = value.M31;
					array[index].W = value.M41;
					change = true;
				}

				index++;
				if (array[index].X != value.M12 ||
					array[index].Y != value.M22 ||
					array[index].Z != value.M32 ||
					array[index].W != value.M42)
				{
					array[index].X = value.M12;
					array[index].Y = value.M22;
					array[index].Z = value.M32;
					array[index].W = value.M42;
					change = true;
				}

				index++;
				if (array[index].X != value.M13 ||
					array[index].Y != value.M23 ||
					array[index].Z != value.M33 ||
					array[index].W != value.M43)
				{
					array[index].X = value.M13;
					array[index].Y = value.M23;
					array[index].Z = value.M33;
					array[index].W = value.M43;
					change = true;
				}
			}


			/// <summary>Method used by a generated shader</summary>
			/// <param name="value"></param>
			/// <param name="index"></param>
			public void SetMatrix2Transpose(int index, ref Matrix value)
			{
				if (array[index].X != value.M11 ||
					array[index].Y != value.M21 ||
					array[index].Z != value.M31 ||
					array[index].W != value.M41)
				{
					array[index].X = value.M11;
					array[index].Y = value.M21;
					array[index].Z = value.M31;
					array[index].W = value.M41;
					change = true;
				}

				index++;
				if (array[index].X != value.M12 ||
					array[index].Y != value.M22 ||
					array[index].Z != value.M32 ||
					array[index].W != value.M42)
				{
					array[index].X = value.M12;
					array[index].Y = value.M22;
					array[index].Z = value.M32;
					array[index].W = value.M42;
					change = true;
				}
			}

			/// <summary></summary>
			/// <param name="value"></param>
			/// <param name="index"></param>
			public void SetMatrix1Transpose(int index, ref Matrix value)
			{
				if (array[index].X != value.M11 ||
					array[index].Y != value.M21 ||
					array[index].Z != value.M31 ||
					array[index].W != value.M41)
				{
					array[index].X = value.M11;
					array[index].Y = value.M21;
					array[index].Z = value.M31;
					array[index].W = value.M41;
					
					this.change = true;
				}
			}

			#endregion

			/// <summary>Method used by a generated shader</summary>
			/// <param name="value"></param>
			/// <param name="index"></param>
			public void SetVector4(int index, ref Vector4 value)
			{
				if (array[index].X != value.X ||
					array[index].Y != value.Y ||
					array[index].Z != value.Z ||
					array[index].W != value.W)
				{
					change = true;
					array[index] = value;
				}
			}

			/// <summary>Method used by a generated shader</summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			public void SetVector3(int index, ref Vector3 value)
			{
				if (array[index].X != value.X ||
					array[index].Y != value.Y ||
					array[index].Z != value.Z)
				{
					change = true;
					array[index].X = value.X;
					array[index].Y = value.Y;
					array[index].Z = value.Z;
				}
			}

			/// <summary>Method used by a generated shader</summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			public void SetVector2(int index, ref Vector2 value)
			{
				if (array[index].X != value.X ||
					array[index].Y != value.Y)
				{
					change = true;
					array[index].X = value.X;
					array[index].Y = value.Y;
				}
			}

			/// <summary>Method used by a generated shader</summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			public void SetSingle(int index, ref float value)
			{
				if (array[index].X != value)
				{
					change = true;
					array[index].X = value;
				}
			}

			#endregion

			/// <summary>Main array of values</summary>
			public readonly Vector4[] array;
			/// <summary>True if a change has been written</summary>
			public bool change;
			private int index;
			private MatrixSetter maskedSetter;

			/// <summary></summary>
			/// <param name="length">number of constants</param>
			public ConstantArray(int length)
			{
				this.array = new Vector4[length];
			}


			/// <summary></summary>
			/// <param name="value"></param>
			/// <param name="reg"></param>
			public void Set(int reg, params float[] value)
			{
				change = true;
				int len = value.Length;
				int index = 0;
				for (int i = 0; i < value.Length / 4; i++)
				{
					array[reg].X = value[index++];
					array[reg].Y = value[index++];
					array[reg].Z = value[index++];
					array[reg].W = value[index++];
					reg++;
					len -= 4;
				}
				if (len > 0)
				{
					array[reg].X = value[index++];
				}
				if (len > 1)
					array[reg].Y = value[index++];
				if (len > 2)
					array[reg].Z = value[index++];
				if (len > 3)
					array[reg].W = value[index++];
			}
		}

		/// <summary>
		/// Interface to an array of shader values
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public interface IArray<T>
		{
			/// <summary>Gets/Sets an element in the array
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			T this[int index] { set; }
			/// <summary>
			/// Set an element in the array. Returns false if no value was set
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns>Returns false if no value was set</returns>
			bool SetValue(int index, ref T value);
			/// <summary>
			/// Sets all values in the array
			/// </summary>
			/// <param name="values"></param>
			void SetArray(T[] values);
			/// <summary>
			/// Sets all values in the array, from the given start index
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			void SetArray(T[] values, int startIndex);
			/// <summary>
			/// Gets the length of the array
			/// </summary>
			int Length { get; }
		}
		
		/// <summary>
		/// Class used by a generated shader
		/// </summary>
		/// <typeparam name="T"></typeparam>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class DualArray<T> : IArray<T>
		{
			private readonly IArray<T> a, b;
			/// <summary></summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			public DualArray(IArray<T> a, IArray<T> b)
			{
				if (a == null || b == null)
					throw new ArgumentNullException();
				this.a = a;
				this.b = b;
			}

			/// <summary>Method used by a generated shader</summary>
			public int Length { get { return Math.Max(a.Length, b.Length); } }

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public T this[int index]
			{
				set
				{
					a.SetValue(index, ref value);
					b.SetValue(index, ref value);
				}
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, ref T value)
			{
				return a.SetValue(index, ref value) | b.SetValue(index, ref value);
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="values"></param>
			public void SetArray(T[] values)
			{
				a.SetArray(values);
				b.SetArray(values);
			}
			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			public void SetArray(T[] values,int startIndex)
			{
				a.SetArray(values, startIndex);
				b.SetArray(values, startIndex);
			}
		}

		/// <summary>
		/// Array of Vector4 values
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class Vector4Array : IArray<Vector4>
		{
			private readonly ConstantArray array;
			private readonly int index;
			private readonly int length;

			/// <summary>
			/// Gets the length of the array
			/// </summary>
			public int Length { get { return length; } }

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="array"></param>
			/// <param name="index"></param>
			/// <param name="length"></param>
			public Vector4Array(ConstantArray array, int index, int length)
			{
				if (array == null) throw new ArgumentNullException();

				this.array = array;
				this.index = index;
				this.length = length;
			}
			/// <summary>
			/// Gets/Sets elements of the array
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Vector4 this[int index]
			{
				set
				{
					if (index >= length || index < 0)
						return;
					array.SetVector4(this.index+index, ref value);
				}
			}
			/// <summary>
			/// Set an element in the array
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, ref Vector4 value)
			{
				if (index >= length || index < 0)
					return false;
				array.SetVector4(this.index + index, ref value);
				return true;
			}
			/// <summary>
			/// Set values in the array
			/// </summary>
			/// <param name="values"></param>
			public void SetArray(Vector4[] values)
			{
				int index = this.index;
				for (int i = 0; i < this.length && i < values.Length; i++)
				{
					array.array[index] = values[i];
					index++;
				}
				array.change = true;
			}
			/// <summary>
			/// Set values in the array
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			public void SetArray(Vector4[] values,int startIndex)
			{
				int index = this.index;
				for (int i = startIndex; i < this.length + startIndex && i < values.Length; i++)
				{
					array.array[index] = values[i];
					index++;
				}
				array.change = true;
			}
		}

		/// <summary>
		/// Vector3 array
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class Vector3Array : IArray<Vector3>
		{
			private readonly ConstantArray array;
			private readonly int index;
			private readonly int length;

			/// <summary>
			/// Length of the array
			/// </summary>
			public int Length { get { return length; } }

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="array"></param>
			/// <param name="index"></param>
			/// <param name="length"></param>
			public Vector3Array(ConstantArray array, int index, int length)
			{
				if (array == null) throw new ArgumentNullException();

				this.array = array;
				this.index = index;
				this.length = length;
			}
			/// <summary>
			/// Gets/Sets elements in the array
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Vector3 this[int index]
			{
				set
				{
					if (index >= length || index < 0)
						return;
					array.SetVector3(this.index + index, ref value);
				}
			}
			/// <summary>
			/// Set an element in the array
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, ref Vector3 value)
			{
				if (index >= length || index < 0)
					return false;
				array.SetVector3(this.index + index, ref value);
				return true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			public void SetArray(Vector3[] values)
			{
				int index = this.index;
				for (int i = 0; i < this.length && i < values.Length; i++)
				{
					array.array[index].X = values[i].X;
					array.array[index].Y = values[i].Y;
					array.array[index].Z = values[i].Z;
					index++;
				}
				array.change = true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			public void SetArray(Vector3[] values,int startIndex)
			{
				int index = this.index;
				for (int i = startIndex; i < this.length + startIndex && i < values.Length; i++)
				{
					array.array[index].X = values[i].X;
					array.array[index].Y = values[i].Y;
					array.array[index].Z = values[i].Z;
					index++;
				}
				array.change = true;
			}
		}

		/// <summary>
		/// Array of vector2s
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class Vector2Array : IArray<Vector2>
		{
			private readonly ConstantArray array;
			private readonly int index;
			private readonly int length;

			/// <summary>
			/// Length of the array
			/// </summary>
			public int Length { get { return length; } }

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="array"></param>
			/// <param name="index"></param>
			/// <param name="length"></param>
			public Vector2Array(ConstantArray array, int index, int length)
			{
				if (array == null) throw new ArgumentNullException();

				this.array = array;
				this.index = index;
				this.length = length;
			}
			/// <summary>
			/// Gets/Sets elements of the array
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Vector2 this[int index]
			{
				set
				{
					if (index >= length || index < 0)
						return;
					array.SetVector2(this.index + index, ref value);
				}
			}
			/// <summary>
			/// Set elements of the array
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, ref Vector2 value)
			{
				if (index >= length || index < 0)
					return false;
				array.SetVector2(this.index + index, ref value);
				return true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			public void SetArray(Vector2[] values)
			{
				int index = this.index;
				for (int i = 0; i < this.length && i < values.Length; i++)
				{
					array.array[index].X = values[i].X;
					array.array[index].Y = values[i].Y;
					index++;
				}
				array.change = true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			public void SetArray(Vector2[] values,int startIndex)
			{
				int index = this.index;
				for (int i = startIndex; i < this.length + startIndex && i < values.Length; i++)
				{
					array.array[index].X = values[i].X;
					array.array[index].Y = values[i].Y;
					index++;
				}
				array.change = true;
			}
		}

		/// <summary>
		/// Float array
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class SingleArray : IArray<Single>
		{
			private readonly ConstantArray array;
			private readonly int index;
			private readonly int length;

			/// <summary>
			/// Length of the array
			/// </summary>
			public int Length { get { return length; } }

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="array"></param>
			/// <param name="index"></param>
			/// <param name="length"></param>
			public SingleArray(ConstantArray array, int index, int length)
			{
				if (array == null) throw new ArgumentNullException();

				this.array = array;
				this.index = index;
				this.length = length;
			}
			/// <summary>
			/// Gets/Sets elements in the array
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public float this[int index]
			{
				set
				{
					if (index >= length || index < 0)
						return;
					array.SetSingle(this.index + index, ref value);
				}
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, float value)
			{
				if (index >= length || index < 0)
					return false;
				array.SetSingle(this.index + index, ref value);
				return true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, ref float value)
			{
				if (index >= length || index < 0)
					return false;
				array.SetSingle(this.index + index, ref value);
				return true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			public void SetArray(float[] values)
			{
				int index = this.index;
				for (int i = 0; i < this.length && i < values.Length; i++)
				{
					array.array[index].X = values[i];
					index++;
				}
				array.change = true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			public void SetArray(float[] values, int startIndex)
			{
				int index = this.index;
				for (int i = startIndex; i < this.length + startIndex && i < values.Length; i++)
				{
					array.array[index].X = values[i];
					index++;
				}
				array.change = true;
			}
		}

		/// <summary>
		/// Array of matrix4
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class Matrix4Array : IArray<Matrix>
		{
			private readonly ConstantArray array;
			private readonly int index;
			private readonly int length;

			private const int size = 4;

			/// <summary>
			/// Length of the array
			/// </summary>
			public int Length { get { return (length + (size - 1)) / size; } }

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="array"></param>
			/// <param name="index"></param>
			/// <param name="length"></param>
			public Matrix4Array(ConstantArray array, int index, int length)
			{
				if (array == null) throw new ArgumentNullException();

				this.array = array;
				this.index = index;
				this.length = length;
			}
			/// <summary>
			/// Gets/Sets array elements
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Matrix this[int index]
			{
				set
				{
					SetValue(index, ref value);
				}
			}
			/// <summary>
			/// Set array element
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, ref Matrix value)
			{
				if (index * size >= length || index < 0)
					return false;
				switch (length - index * size)
				{
					case 1:
						array.SetMatrix1Transpose(this.index + (index * size), ref value);
						break;
					case 2:
						array.SetMatrix2Transpose(this.index + (index * size), ref value);
						break;
					case 3:
						array.SetMatrix3Transpose(this.index + (index * size), ref value);
						break;
					default:
						array.SetMatrix4Transpose(this.index + (index * size), ref value);
						break;
				}
				return true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			public void SetArray(Matrix[] values)
			{
				SetArray(values, 0);
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			public void SetArray(Matrix[] values, int startIndex)
			{
				int index = this.index;
				int max = this.index + Math.Min(this.length, (values.Length - startIndex) * size);
				if (index >= max)
					return;
				array.change = true;
				int i = startIndex;

				while (true)
				{
					array.array[index].X = values[i].M11;
					array.array[index].Y = values[i].M21;
					array.array[index].Z = values[i].M31;
					array.array[index].W = values[i].M41;
					if (++index == max)
						return;
					array.array[index].X = values[i].M12;
					array.array[index].Y = values[i].M22;
					array.array[index].Z = values[i].M32;
					array.array[index].W = values[i].M42;
					if (++index == max)
						return;
					array.array[index].X = values[i].M13;
					array.array[index].Y = values[i].M23;
					array.array[index].Z = values[i].M33;
					array.array[index].W = values[i].M43;
					if (++index == max)
						return;
					array.array[index].X = values[i].M14;
					array.array[index].Y = values[i].M24;
					array.array[index].Z = values[i].M34;
					array.array[index].W = values[i].M44;
					i++;
					if (++index == max)
						return;
				}
			}
		}


		/// <summary>
		/// Array of matrix4x3
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class Matrix3Array : IArray<Matrix>
		{
			private readonly ConstantArray array;
			private readonly int index;
			private readonly int length;

			private const int size = 3;

			/// <summary>
			/// Lenght of array
			/// </summary>
			public int Length { get { return (length + (size - 1)) / size; } }

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="array"></param>
			/// <param name="index"></param>
			/// <param name="length"></param>
			public Matrix3Array(ConstantArray array, int index, int length)
			{
				if (array == null) throw new ArgumentNullException();

				this.array = array;
				this.index = index;
				this.length = length;
			}
			/// <summary>
			/// Gets/Sets array element
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Matrix this[int index]
			{
				set
				{
					SetValue(index, ref value);
				}
			}
			/// <summary>
			/// Set array element
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, ref Matrix value)
			{
				if (index * size >= length || index < 0)
					return false;
				switch (length - index * size)
				{
					case 1:
						array.SetMatrix1Transpose(this.index + (index * size), ref value);
						break;
					case 2:
						array.SetMatrix2Transpose(this.index + (index * size), ref value);
						break;
					default:
						array.SetMatrix3Transpose(this.index + (index * size), ref value);
						break;
				}
				return true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			public void SetArray(Matrix[] values)
			{
				SetArray(values, 0);
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			public void SetArray(Matrix[] values, int startIndex)
			{
				int index = this.index;
				int max = this.index + Math.Min(this.length, (values.Length - startIndex) * size);
				if (index >= max)
					return;
				array.change = true;
				int i = startIndex;

				while (true)
				{
					array.array[index].X = values[i].M11;
					array.array[index].Y = values[i].M21;
					array.array[index].Z = values[i].M31;
					array.array[index].W = values[i].M41;
					if (++index == max)
						return;
					array.array[index].X = values[i].M12;
					array.array[index].Y = values[i].M22;
					array.array[index].Z = values[i].M32;
					array.array[index].W = values[i].M42;
					if (++index == max)
						return;
					array.array[index].X = values[i].M13;
					array.array[index].Y = values[i].M23;
					array.array[index].Z = values[i].M33;
					array.array[index].W = values[i].M43;
					i++;
					if (++index == max)
						return;
				}
			}
		}

		/// <summary>
		/// Array of matrix4x2
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class Matrix2Array : IArray<Matrix>
		{
			private readonly ConstantArray array;
			private readonly int index;
			private readonly int length;

			private const int size = 2;

			/// <summary>
			/// Array length
			/// </summary>
			public int Length { get { return (length + (size - 1)) / size; } }

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="array"></param>
			/// <param name="index"></param>
			/// <param name="length"></param>
			public Matrix2Array(ConstantArray array, int index, int length)
			{
				if (array == null) throw new ArgumentNullException();

				this.array = array;
				this.index = index;
				this.length = length;
			}
			/// <summary>
			/// Gets/Sets array elements
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Matrix this[int index]
			{
				set
				{
					SetValue(index, ref value);
				}
			}
			/// <summary>
			/// Set array element
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, ref Matrix value)
			{
				if (index * size >= length || index < 0)
					return false;
				switch (length - index * size)
				{
					case 1:
						array.SetMatrix1Transpose(this.index + (index * size), ref value);
						break;
					default:
						array.SetMatrix2Transpose(this.index + (index * size), ref value);
						break;
				}
				return true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			public void SetArray(Matrix[] values)
			{
				SetArray(values, 0);
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			public void SetArray(Matrix[] values, int startIndex)
			{
				int index = this.index;
				int max = this.index + Math.Min(this.length, (values.Length - startIndex) * size);
				if (index >= max)
					return;
				array.change = true;
				int i = startIndex;

				while (true)
				{
					array.array[index].X = values[i].M11;
					array.array[index].Y = values[i].M21;
					array.array[index].Z = values[i].M31;
					array.array[index].W = values[i].M41;
					if (++index == max)
						return;
					array.array[index].X = values[i].M12;
					array.array[index].Y = values[i].M22;
					array.array[index].Z = values[i].M32;
					array.array[index].W = values[i].M42;
					i++;
					if (++index == max)
						return;
				}
			}
		}

		/// <summary>
		/// Array of matrix4x1
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		public sealed class Matrix1Array : IArray<Matrix>
		{
			private readonly ConstantArray array;
			private readonly int index;
			private readonly int length;

			private const int size = 1;

			/// <summary>
			/// Array length
			/// </summary>
			public int Length { get { return (length + (size - 1)) / size; } }

			/// <summary>
			/// Method used by a generated shader
			/// </summary>
			/// <param name="array"></param>
			/// <param name="index"></param>
			/// <param name="length"></param>
			public Matrix1Array(ConstantArray array, int index, int length)
			{
				if (array == null) throw new ArgumentNullException();

				this.array = array;
				this.index = index;
				this.length = length;
			}
			/// <summary>
			/// Gets/Sets array elements
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public Matrix this[int index]
			{
				set
				{
					SetValue(index, ref value);
				}
			}
			/// <summary>
			/// Set array element
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			public bool SetValue(int index, ref Matrix value)
			{
				if (index * size >= length || index < 0)
					return false;

				array.SetMatrix1Transpose(this.index + (index * size), ref value);

				return true;
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			public void SetArray(Matrix[] values)
			{
				SetArray(values, 0);
			}
			/// <summary>
			/// Set array values
			/// </summary>
			/// <param name="values"></param>
			/// <param name="startIndex"></param>
			public void SetArray(Matrix[] values, int startIndex)
			{
				int index = this.index;
				int max = this.index + Math.Min(this.length, (values.Length - startIndex) * size);
				if (index >= max)
					return;
				array.change = true;
				int i = startIndex;

				while (true)
				{
					array.array[index].X = values[i].M11;
					array.array[index].Y = values[i].M21;
					array.array[index].Z = values[i].M31;
					array.array[index].W = values[i].M41;
					i++;
					if (++index == max)
						return;
				}
			}
		}





		#endregion

	}

	namespace Constants
	{

		/// <summary>
		/// Interface for setting a shader value by ref
		/// </summary>
		/// <typeparam name="T"><see cref="Single"/>, <see cref="Vector2"/>, <see cref="Vector3"/>, <see cref="Vector4"/> and <see cref="Matrix"/></typeparam>
		public interface IValue<T> where T : struct
		{
			/// <summary></summary><param name="value"></param>
			void Set(ref T value);
		}

		/// <summary>
		/// Interface for setting an array of shader value registers
		/// </summary>
		public interface IValueArray
		{
			/// <summary></summary><param name="values"></param>
			/// <remarks><para>It is recommended to not pass a copy of the register data into this method, but to simply pass the member reference.</para><para>The array data will not be modified.</para></remarks>
			void Set(float[] values);
		}

	}
}

namespace Xen.Graphics
{
	namespace State
	{
		/// <summary>
		/// Packed representation of common Texture Sampler states. 4 bytes
		/// </summary>
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 4)]
		[System.Diagnostics.DebuggerStepThrough]
		public struct TextureSamplerState
		{
			[System.Runtime.InteropServices.FieldOffset(0)]
			internal int mode;

			/*
			internal void ApplyState(SamplerState sampler, ref TextureSamplerState current)
			{
				if ((this.mode & 511) != (current.mode & 511))
				{
					if (AddressU != current.AddressU)
						sampler.AddressU = AddressU;

					if (AddressV != current.AddressV)
						sampler.AddressV = AddressV;

					if (AddressW != current.AddressW)
						sampler.AddressW = AddressW;
				}

				if ((this.mode & (63 << 9)) != (current.mode & (63 << 9)))
				{
					if (MinFilter != current.MinFilter)
						sampler.MinFilter = MinFilter;

					if (MagFilter != current.MagFilter)
						sampler.MagFilter = MagFilter;

					if (MipFilter != current.MipFilter)
						sampler.MipFilter = MipFilter;
				}

				if ((this.mode & (0xFFFF << 16)) != (current.mode & (0xFFFF << 16)))
				{
					if (MaxAnisotropy != current.MaxAnisotropy)
						sampler.MaxAnisotropy = MaxAnisotropy;

					if (MaxMipmapLevel != current.MaxMipmapLevel)
						sampler.MaxMipLevel = MaxMipmapLevel;
				}

				current = this;
			}
			*/

			internal void ResetState(SamplerState sampler, ref TextureSamplerState current)
			{
				sampler.AddressU = AddressU;
				sampler.AddressV = AddressV;
				sampler.AddressW = AddressW;
				sampler.MinFilter = MinFilter;
				sampler.MagFilter = MagFilter;
				sampler.MipFilter = MipFilter;
				sampler.MaxAnisotropy = MaxAnisotropy;
				sampler.MaxMipLevel = MaxMipmapLevel;

				current = this;
			}


			internal TextureSamplerState(TextureAddressMode uv, TextureFilter min, TextureFilter mag, TextureFilter mip, int maxAni)
			{
				mode = 0;
				this.AddressUV = uv;
				this.MinFilter = min;
				this.MagFilter = mag;
				this.MipFilter = mip;
				this.MaxAnisotropy = maxAni;
			}
			internal TextureSamplerState(int mode)
			{
				this.mode = mode;
			}

			/// <summary></summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <returns></returns>
			public static bool operator ==(TextureSamplerState a, TextureSamplerState b)
			{
				return a.mode == b.mode;
			}
			/// <summary></summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <returns></returns>
			public static bool operator !=(TextureSamplerState a, TextureSamplerState b)
			{
				return a.mode != b.mode;
			}
			/// <summary></summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public override bool Equals(object obj)
			{
				if (obj is TextureSamplerState)
					return ((TextureSamplerState)obj).mode == this.mode;
				return base.Equals(obj);
			}
			/// <summary>
			/// Gets the hash code for this sampler state. Returns the internal bitfield value
			/// </summary>
			/// <returns></returns>
			public override int GetHashCode()
			{
				return mode;
			}

			/// <summary>
			/// Cast this sampler to it's internal bitfield representation
			/// </summary>
			/// <param name="state"></param>
			/// <returns></returns>
			public static implicit operator int(TextureSamplerState state)
			{
				return state.mode;
			}
			/// <summary>
			/// Explicit case from an integer bitfield representation
			/// </summary>
			/// <param name="state"></param>
			/// <returns></returns>
			public static explicit operator TextureSamplerState(int state)
			{
				return new TextureSamplerState(state);
			}

			private static TextureSamplerState point = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Point, TextureFilter.Point, TextureFilter.Point, 0);
			private static TextureSamplerState bilinear = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Linear, TextureFilter.Linear, TextureFilter.Point, 0);
			private static TextureSamplerState trilinear = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Linear, TextureFilter.Linear, TextureFilter.Linear, 0);
			private static TextureSamplerState aniLow = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Anisotropic, TextureFilter.Linear, TextureFilter.Linear, 2);
			private static TextureSamplerState aniMed = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Anisotropic, TextureFilter.Linear, TextureFilter.Linear, 4);
			private static TextureSamplerState aniHigh = new TextureSamplerState(TextureAddressMode.Wrap, TextureFilter.Anisotropic, TextureFilter.Linear, TextureFilter.Linear, 8);

			/// <summary>
			/// Gets a texture sampler that applies simple pointer filtering
			/// </summary>
			public static TextureSamplerState PointFiltering { get { return point; } }
			/// <summary>
			/// Gets a texture sampler that applies bilinear filtering (linear UV filtering with point mipmap filtering)
			/// </summary>
			public static TextureSamplerState BilinearFiltering { get { return bilinear; } }
			/// <summary>
			/// Gets a texture sampler that applies trilinear filtering (linear UV filtering mipmap filtering)
			/// </summary>
			public static TextureSamplerState TrilinearFiltering { get { return trilinear; } }
			/// <summary>
			/// Gets a texture sampler that applies anisotropic filtering with a low max anisotropic value
			/// </summary>
			public static TextureSamplerState AnisotropicLowFiltering { get { return aniLow; } }
			/// <summary>
			/// Gets a texture sampler that applies anisotropic filtering with a medium max anisotropic value
			/// </summary>
			public static TextureSamplerState AnisotropicMediumFiltering { get { return aniMed; } }
			/// <summary>
			/// Gets a texture sampler that applies anisotropic filtering with a high max anisotropic value
			/// </summary>
			public static TextureSamplerState AnisotropicHighFiltering { get { return aniHigh; } }


			/// <summary>
			/// Allows setting of both the <see cref="AddressU"/> and <see cref="AddressV"/> coordinate address modes at the same time
			/// </summary>
			/// <remarks>The safest values to use are <see cref="TextureAddressMode.Wrap"/> for repeating textures, and <see cref="TextureAddressMode.Clamp"/>. Other options such as <see cref="TextureAddressMode.Border"/> may not be supported on all hardware.</remarks>
			public TextureAddressMode AddressUV
			{
				set
				{
					{
						const int offset = 0; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset);
					}
					{
						const int offset = 3; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset);
					}
				}
			}
			///// <summary>
			///// Allows setting of all three UVW coordinate address modes at the same time
			///// </summary>
			//public TextureAddressMode AddressUVW
			//{
			//    set
			//    {
			//        mode = (mode & (~(7 << 3)) | (~(7 << 0)) | (~(7 << 6))) | (((7 & ((int)value - 1)) << 0) | ((7 & ((int)value - 1)) << 3) | ((7 & ((int)value - 1)) << 6));
			//    }
			//}

			/// <summary>
			/// Controls texture address behaviour for the U coordinate (The U coordinate is the x-axis in texture coordinate space)
			/// </summary>
			/// <remarks>The safest values to use are <see cref="TextureAddressMode.Wrap"/> for repeating textures, and <see cref="TextureAddressMode.Clamp"/>. Other options such as <see cref="TextureAddressMode.Border"/> may not be supported on all hardware.</remarks>
			public TextureAddressMode AddressU
			{
				get { const int offset = 0; return (TextureAddressMode)(((mode >> offset) & 7) + 1); }
				set { const int offset = 0; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
			}
			/// <summary>
			/// Controls texture address behaviour for the V coordinate (The V coordinate is the y-axis in texture coordinate space)
			/// </summary>
			/// <remarks>The safest values to use are <see cref="TextureAddressMode.Wrap"/> for repeating textures, and <see cref="TextureAddressMode.Clamp"/>. Other options such as <see cref="TextureAddressMode.Border"/> may not be supported on all hardware.</remarks>
			public TextureAddressMode AddressV
			{
				get { const int offset = 3; return (TextureAddressMode)(((mode >> offset) & 7) + 1); }
				set { const int offset = 3; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
			}
			/// <summary>
			/// Controls texture address behaviour for the W coordinate (The W coordinate is the z-axis in texture coordinate space). This filtering mode only applies to 3D textures.
			/// </summary>
			/// <remarks>The safest values to use are <see cref="TextureAddressMode.Wrap"/> for repeating textures, and <see cref="TextureAddressMode.Clamp"/>. Other options such as <see cref="TextureAddressMode.Border"/> may not be supported on all hardware.</remarks>
			public TextureAddressMode AddressW
			{
				get { const int offset = 6; return (TextureAddressMode)(((mode >> offset) & 7) + 1); }
				set { const int offset = 6; mode = ((mode & ~(7 << offset)) | (7 & ((int)value - 1)) << offset); }
			}
			/// <summary>
			/// Controls texture filtering when the texture is displayed over a smaller area than itself (the texture is minified, or reduced in size)
			/// </summary>
			/// <remarks><para>TextureFilter.None, TextureFilter.Point, TextureFilter.Linear and TextureFilter.Anisotropic are supported.</para>
			/// <para>This filter would apply when a 512x512 texture is being displayed as 400x400 pixels in size</para></remarks>
			public TextureFilter MinFilter
			{
				get { const int offset = 9; return (TextureFilter)(((mode >> offset) & 3)); }
				set { const int offset = 9; mode = ((mode & ~(3 << offset)) | (3 & (Math.Min(3, (int)value))) << offset); }
			}
			/// <summary>
			/// Controls texture filtering when the texture is displayed over a larger area than itself (the texture is magnified, or enlarged)
			/// </summary>
			/// <remarks><para>TextureFilter.None, TextureFilter.Point, TextureFilter.Linear are supported.</para>
			/// <para>This filter would apply when a 512x512 texture is being displayed as 600x600 pixels in size</para>
			/// </remarks>
			public TextureFilter MagFilter
			{
				get { const int offset = 11; return (TextureFilter)(((mode >> offset) & 3)); }
				set { const int offset = 11; mode = ((mode & ~(3 << offset)) | (3 & (Math.Min(2, (int)value))) << offset); }
			}
			/// <summary>
			/// <para>Controls texture filtering that takes place between different mipmap levels.</para>
			/// <para>Set <see cref="TextureFilter.None"/> to disable mipmapping</para>
			/// <para>Set <see cref="TextureFilter.Point"/> in combination with MinFilter as TextureFilter.Linear for bilinear filtering (2 axis filtering). Samples in nearest miplevel will be displayed.</para>
			/// <para>Set <see cref="TextureFilter.Linear"/> in combination with MinFilter as TextureFilter.Linear for trilinear filtering (3 axis filtering). Samples in the two nearest mipmap levels will be interpolated between.</para>
			/// </summary>
			/// <remarks>Valid inputs are TextureFilter.None, TextureFilter.Point and TextureFilter.Linear</remarks>
			public TextureFilter MipFilter
			{
				get { const int offset = 13; return (TextureFilter)(((mode >> offset) & 3)); }
				set { const int offset = 13; mode = ((mode & ~(3 << offset)) | (3 & (Math.Min(2, (int)value))) << offset); }
			}
			/// <summary>
			/// Set the maximum number of samples used when <see cref="MinFilter"/> is set to <see cref="TextureFilter.Anisotropic"/> filtering. Range of [1-16], usually limited to values that are a power of two.
			/// </summary>
			public int MaxAnisotropy
			{
				get { const int offset = 16; return (((mode >> offset) & 15) + 1); }
				set { const int offset = 16; mode = ((mode & ~(15 << offset)) | (15 & (Math.Max(0, Math.Min(16, value) - 1))) << offset); }
			}
			/// <summary>
			/// Set the maximum mipmap level the video card will sample, where 0 is the largest map (and the default value). Set to 1 to prevent the highest mipmap level being sampled (this will effectivly half the resolution of the texture displayed).
			/// </summary>
			public int MaxMipmapLevel
			{
				get { const int offset = 20; return ((((mode >> offset)) & 255)); }
				set { const int offset = 20; mode = ((mode & ~(255 << offset)) | (255 & ((Math.Min(255, value)))) << offset); }
			}

			/*
			public void SetToTrilinearFiltering()
			{
				this.mode = trilinear.mode;
			}
			public void SetToBilinearFiltering()
			{
				this.mode = bilinear.mode;
			}
			public void SetToPointFiltering()
			{
				this.mode = point.mode;
			}
			public void SetToAnisotropicFilteringLow()
			{
				this.mode = aniLow.mode;
			}
			public void SetToAnisotropicFilteringMedium()
			{
				this.mode = aniMed.mode;
			}
			public void SetToAnisotropicFilteringHigh()
			{
				this.mode = aniHigh.mode;
			}
			*/
		}
	}
}
