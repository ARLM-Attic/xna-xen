using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Xen.Graphics;
using Xen.Ex.Graphics.Content;
using Xen.Ex.Material;
using Xen.Ex.Compression;
using Xen.Threading;

namespace Xen.Ex.Graphics
{
	/// <summary>
	/// Controls animation streams and calulate transformed bone structures for a model
	/// </summary>
	public sealed class AnimationController : IUpdate, IDisposable, IAction
	{
		internal Transform[] transformedBones;
		private bool[] transformIdentity;
		private readonly List<AnimationStreamControl> animations;
		private ModelData modelData;
		private bool disposed, wasDrawn, wasSkipped;
		private readonly List<WeakReference> parents;
		private float deltaTime;
		private WaitCallback waitCallback;
		private int frameIndex, boundsFrameIndex;
		private bool isAsync;
		internal Vector3 boundsMin,boundsMax;

		internal AnimationController(ModelData model, UpdateManager manager, ModelInstance parent)
		{
			if (manager != null)
			{
				manager.Add(this);
				isAsync = true;

				parents = new List<WeakReference>();
				parents.Add(new WeakReference(parent));
			}
			animations = new List<AnimationStreamControl>();

			wasDrawn = true;

			if (model != null)
				SetModelData(model);
		}

		internal void SetModelData(ModelData model)
		{
			if (model != null && this.modelData == null)
			{
				if (model.skeleton == null)
					throw new InvalidOperationException("ModelData requires a skeleton to be animated");

				this.modelData = model;

				transformedBones = new Transform[model.Skeleton.BoneCount];
				transformIdentity = new bool[model.Skeleton.BoneCount];

				for (int i = 0; i < model.Skeleton.BoneCount; i++)
					transformedBones[i] = model.Skeleton.BoneLocalTransform[i];
			}
		}

		public bool IsDisposed
		{
			get { return disposed; }
		}

		internal ModelData ModelData
		{
			get { return modelData; }
		}

		public int AnimationCount
		{
			get
			{
				if (disposed)
					throw new ObjectDisposedException("this"); 
				return modelData.animations.Length;
			}
		}

		/// <summary>
		/// <para>Clears <i>all</i> global cached animations for this <see cref="ModelData"/>. The cache helps reduce allocation/garbage build up</para>
		/// <para>Note: This purge will effect all <see cref="AnimationController"/> instances for the current <see cref="ModelData"/> Content</para>
		/// </summary>
		public void PurgeAnimationStreamCaches()
		{
			if (modelData == null)
				throw new InvalidOperationException("ModelData == null");

			if (modelData.animations != null)
				for (int i = 0; i < modelData.animations.Length; i++)
					modelData.animations[i].ClearAnimationStreamCache();
		}

		public AnimationData GetAnimationData(int index)
		{
			if (disposed)
				throw new ObjectDisposedException("this");
			return modelData.animations[index];
		}
		/// <summary>
		/// <para>Gets an animation index by animation string name</para>
		/// <para>Performs a linear search of animations in the <see cref="ModelData"/></para>
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int AnimationIndex(string name)
		{
			if (disposed)
				throw new ObjectDisposedException("this");
			for (int i = 0; i < modelData.animations.Length; i++)
			{
				if (name == modelData.animations[i].Name)
					return i;
			}
			return -1;
		}

		/// <summary>
		/// <para>Gets the transformed bones for this animation controller (Bones are represented in bone world space, not model world space - see remarks for details).</para>
		/// <para>Note: For off-screen models, calling this method will force transform computation. For Async controllers, this method may cause the thread to wait for the animation processing to complete</para>
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// Bone transforms are in transformed bone space. To Get the exact world transform of a bone, multiply <see cref="SkeletonData.BoneWorldTransforms"/> by the transformed bones.
		/// </para>
		/// </remarks>
		public ReadOnlyArrayCollection<Transform> GetTransformedBones(DrawState state)
		{
			WaitForAsyncAnimation(state,state.FrameIndex,true);
			return new ReadOnlyArrayCollection<Transform>(transformedBones);
		}

		internal void AddParnet(object parent)
		{
			if (this.parents != null)
				this.parents.Add(new WeakReference(parent));
		}

		/// <summary>
		/// Plays an animation that loops continuously, returning a <see cref="AnimationInstance"/> structure
		/// </summary>
		/// <param name="animationIndex"></param>
		/// <returns></returns>
		public AnimationInstance PlayLoopingAnimation(int animationIndex)
		{
			return PlayLoopingAnimation(animationIndex, 0);
		}
		/// <summary>
		/// Plays an animation that loops continuously, returning a <see cref="AnimationInstance"/> structure
		/// </summary>
		/// <param name="animationIndex"></param>
		/// <returns></returns>
		/// <param name="fadeInTime">Time, in seconds, to fade the animation in</param>
		public AnimationInstance PlayLoopingAnimation(int animationIndex, float fadeInTime)
		{
			if (animationIndex == -1)
				throw new ArgumentException();
			if (modelData == null)
				throw new InvalidOperationException("ModelInstance.ModelData == null");
			if (disposed)
				throw new ObjectDisposedException("this");
			AnimationStreamControl control = this.modelData.animations[animationIndex].GetStream();
			control.Looping = true;
			control.fadeInTime = fadeInTime;
			control.fadeOutTime = 0;
			control.fadeOutStop = 0;
			animations.Add(control);
			return new AnimationInstance(control);
		}
		/// <summary>
		/// Plays an animation, returning a <see cref="AnimationInstance"/> structure
		/// </summary>
		/// <param name="animationIndex"></param>
		/// <returns></returns>
		public AnimationInstance PlayAnimation(int animationIndex)
		{
			return PlayAnimation(animationIndex, 0, 0);
		}
		/// <summary>
		/// Plays an animation, returning a <see cref="AnimationInstance"/> structure
		/// </summary>
		/// <param name="animationIndex"></param>
		/// <param name="fadeInTime">Time, in seconds, to fade the animation in</param>
		/// <param name="fadeOutTime">Time, in seconds, to fade the animation out</param>
		/// <returns></returns>
		public AnimationInstance PlayAnimation(int animationIndex, float fadeInTime, float fadeOutTime)
		{
			if (animationIndex == -1)
				throw new ArgumentException();
			if (fadeInTime < 0)
				throw new ArgumentException("fadeInTime");
			if (fadeOutTime < 0)
				throw new ArgumentException("fadeOutTime");
			if (modelData == null)
				throw new InvalidOperationException("ModelInstance.ModelData == null");
			if (disposed)
				throw new ObjectDisposedException("this");
			AnimationStreamControl control = this.modelData.animations[animationIndex].GetStream();
			control.Looping = false;
			control.fadeOutStop = 0;
			control.fadeInTime = fadeInTime;
			control.fadeOutTime = fadeOutTime;
			animations.Add(control);
			return new AnimationInstance(control);
		}

		UpdateFrequency IUpdate.Update(UpdateState state)
		{
			if (disposed)
				return UpdateFrequency.Terminate;

			if (!wasDrawn)
			{
				bool alive = false;
				foreach (WeakReference wr in this.parents)
				{
					if (wr.IsAlive)
					{
						alive = true;
						break;
					}
				}
				if (!alive)
					return UpdateFrequency.Terminate;
			}

			this.deltaTime = state.DeltaTimeSeconds;

			if (wasDrawn && modelData != null)
				this.waitCallback = state.Application.ThreadPool.QueueTask(this, null);
			else
				wasSkipped = true;
			wasDrawn = false;

			//using async here would be slower...
			return UpdateFrequency.OncePerFrame;
		}

		private void ComputeAnimationBounds()
		{
			boundsMin = this.modelData.StaticBounds.Minimum;
			boundsMax = this.modelData.StaticBounds.Maximum;

			for (int a = 0; a < animations.Count; a++)
			{
				AnimationStreamControl anim = animations[a];
				if (anim.enabled)
				{
					int animIndex = anim.animation.index;

					Vector3 value = modelData.animationStaticBounds[animIndex].Minimum;
					boundsMin.X += value.X * anim.weighting;
					boundsMin.Y += value.Y * anim.weighting;
					boundsMin.Z += value.Z * anim.weighting;

					value = modelData.animationStaticBounds[animIndex].Maximum;
					boundsMax.X += value.X * anim.weighting;
					boundsMax.Y += value.Y * anim.weighting;
					boundsMax.Z += value.Z * anim.weighting;
				}
			}
		}

		//computes the animation bounds for the mesh
		internal void ComputeMeshBounds(int meshIndex, out Vector3 boundsMin, out Vector3 boundsMax)
		{
			MeshData mesh = modelData.meshes[meshIndex];
			boundsMin = mesh.StaticBounds.Minimum;
			boundsMax = mesh.StaticBounds.Maximum;

			for (int a = 0; a < animations.Count; a++)
			{
				AnimationStreamControl anim = animations[a];
				if (anim.enabled)
				{
					int animIndex = anim.animation.index;

					Vector3 value = mesh.animationStaticBounds[animIndex].Minimum;
					boundsMin.X += value.X * anim.weighting;
					boundsMin.Y += value.Y * anim.weighting;
					boundsMin.Z += value.Z * anim.weighting;

					value = mesh.animationStaticBounds[animIndex].Maximum;
					boundsMax.X += value.X * anim.weighting;
					boundsMax.Y += value.Y * anim.weighting;
					boundsMax.Z += value.Z * anim.weighting;
				}
			}
		}
		//computes the animation bounds for geometry in a mesh
		internal void ComputeGeometryBounds(int meshIndex, int geometryIndex, out Vector3 boundsMin, out Vector3 boundsMax)
		{
			GeometryData geometry = modelData.meshes[meshIndex].geometry[geometryIndex];
			boundsMin = geometry.StaticBounds.Minimum;
			boundsMax = geometry.StaticBounds.Maximum;

			for (int a = 0; a < animations.Count; a++)
			{
				AnimationStreamControl anim = animations[a];
				if (anim.enabled)
				{
					int animIndex = anim.animation.index;

					Vector3 value = geometry.animationStaticBounds[animIndex].Minimum;
					boundsMin.X += value.X * anim.weighting;
					boundsMin.Y += value.Y * anim.weighting;
					boundsMin.Z += value.Z * anim.weighting;

					value = geometry.animationStaticBounds[animIndex].Maximum;
					boundsMax.X += value.X * anim.weighting;
					boundsMax.Y += value.Y * anim.weighting;
					boundsMax.Z += value.Z * anim.weighting;
				}
			}
		}

		private void ProcessAnimation()
		{
			float delta = this.deltaTime;

			Transform idenity = Transform.Identity;
			for (int i = 0; i < transformedBones.Length; i++)
				transformedBones[i] = idenity;
			for (int i = 0; i < transformIdentity.Length; i++)
				transformIdentity[i] = true;

			int index = 0;
			for (int a = 0; a < animations.Count; a++)
			{
				AnimationStreamControl anim = animations[a];
				if (anim.enabled == false)
					continue;

				if (!anim.Interpolate(delta))
				{
					anim.animation.CacheUnusedStream(anim);
					animations.RemoveAt(a);
					a--;
					continue;
				}

				AnimationStreamControl.AnimationChannel[] channels = anim.channels;

				if (index == 0)
				{
					for (int i = 0; i < channels.Length; i++)
					{
						int bi = channels[i].boneIndex;
						transformedBones[bi] = channels[i].lerpedTransform;
						transformIdentity[bi] = false;
					}
				}
				else
				{
					for (int i = 0; i < channels.Length; i++)
					{
						int bi = channels[i].boneIndex;
						if (transformIdentity[bi])
						{
							transformIdentity[bi] = false;
							transformedBones[bi] = channels[i].lerpedTransform;
						}
						else
							Transform.Multiply(ref transformedBones[bi], ref channels[i].lerpedTransform, out transformedBones[bi]);
					}
				}
				index++;
			}

			modelData.skeleton.TransformHierarchy(transformedBones);
		}

		internal void WaitForAsyncAnimation(IState state, int frameIndex, bool requiresBoneData)
		{
			if (modelData == null)
				return;

			if (state == null)
				throw new ArgumentNullException();

			this.wasDrawn |= requiresBoneData;

			if (frameIndex != this.boundsFrameIndex)
			{
				ComputeAnimationBounds();

				this.boundsFrameIndex = frameIndex;
			}
			if (requiresBoneData && frameIndex != this.frameIndex)
			{
				if (isAsync)
					this.waitCallback.WaitForCompletion();
				if (!isAsync || wasSkipped)
				{
					this.wasSkipped = false;
					this.deltaTime = state.DeltaTimeSeconds;
					this.ProcessAnimation();
				}
				this.frameIndex = frameIndex;
			}
		}

		public void Dispose()
		{
			foreach (AnimationStreamControl stream in this.animations)
				stream.animation.CacheUnusedStream(stream);
			this.animations.Clear();
			disposed = true;
		}

		void IAction.PerformAction(object data)
		{
			ProcessAnimation();
		}
	}

	/// <summary>
	/// Handle to an animation that is playing through an <see cref="AnimationController"/>
	/// </summary>
	public struct AnimationInstance
	{
		readonly private AnimationStreamControl control;
		readonly private int usageIndex;

		internal AnimationInstance(AnimationStreamControl control)
		{
			this.control = control;
			this.usageIndex = control.usageIndex;
		}

		/// <summary>
		/// Returns true if this animation instance exists
		/// </summary>
		public bool ValidAnimation { get { return control != null; } }
		/// <summary>
		/// True if the animation has finished playing, any changes made will have no effect if true.
		/// </summary>
		public bool AnimationFinished { get { return control == null || usageIndex != control.usageIndex; } }
		/// <summary>
		/// Playback speed multiplier. 1.0f is the default, for normal playback speed. Set to 0.5f for half speed, etc.
		/// </summary>
		public float PlaybackSpeed 
		{
			get { if (AnimationFinished) return 0; return control.AnimationSpeed; } 
			set { if (AnimationFinished) return; control.AnimationSpeed = value; } 
		}
		public string AnimationName { get { return control.animation.Name; } }
		/// <summary>
		/// Current playback time in the duration of this animation instance (seconds)
		/// </summary>
		public float Time { get { if (AnimationFinished) return AnimationDuration; return control.AnimationTime; } }
		/// <summary>
		/// <para>Duration of the animation, in seconds</para>
		/// <para>This value may be slightly larger if <see cref="LoopTransitionEnabled"/> is true</para>
		/// </summary>
		public float AnimationDuration 
		{
			get { if (AnimationFinished) return 0; return control.Duration(); }
		}
		/// <summary>
		/// <para>The weighting of this animation instance. 1.0f by default.</para
		/// <para>When set to 0.0, the animation will have no effect. If set to 0.5f, the animation will only have a 50 percent effect on rotations, translations, etc.</para>
		/// <para>Modify this value to fade animations in and out</para>
		/// </summary>
		public float Weighting
		{
			get { if (AnimationFinished) return 0; return control.weighting; }
			set { if (AnimationFinished) return; control.weighting = value; }
		}
		public bool Enabled
		{
			get { if (AnimationFinished) return false; return control.enabled; }
			set { if (AnimationFinished) return; control.enabled = value; }
		}
		public bool Looping
		{
			get { if (AnimationFinished) return false; return control.Looping; ; }
		}
		/// <summary>
		/// <para>If true, the animation will smoothly transition from the last frame to the first during a loop.</para>
		/// <para>Defaults to true for looping animations</para>
		/// </summary>
		public bool LoopTransitionEnabled
		{
			get { if (AnimationFinished) return false; return control.LoopTransition; }
			set { if (AnimationFinished || !control.Looping) return; control.LoopTransition = value; }
		}

		/// <summary>
		/// Seeks the animation to a new time. This may require decoding extra animation frames
		/// </summary>
		/// <param name="animationTime"></param>
		/// <remarks>True if the seek was successful</remarks>
		public bool SeekAnimation(float animationTime)
		{
			if (AnimationFinished)
				return false;
			control.Seek(animationTime);
			return true;
		}

		public bool StopAnimation()
		{
			return StopAnimation(0);
		}
		public bool StopAnimation(float fadeOutTime)
		{
			if (fadeOutTime < 0)
				throw new ArgumentException();

			if (AnimationFinished)
				return false;
			if (fadeOutTime > 0)
				control.fadeOutStop = 1.0f / fadeOutTime * control.weighting;
			else
				control.fadeOutStop = -1;
			control.usageIndex++;
			return true;
		}
	}

	internal sealed class AnimationStreamControl
	{
		internal struct AnimationChannel
		{
			public Transform storeTransform, lerpedTransform;
			public CompressedTransformReader frameReader;
			public byte[] sourceData;
			public int readIndex;
			public int boneIndex;
		}

		internal int storedFrameIndex, readFrameIndex;
		internal float currentFrameTime, previousFrameTime;
		internal readonly AnimationData animation;
		private float frameTimer, speed;
		internal readonly AnimationChannel[] channels;
		internal int usageIndex;
		internal float weighting;
		internal bool enabled;
		private bool looping = true, loopTransition = true, firstFrame = true;
		internal float fadeInTime, fadeOutTime, fadeOutStop;

		public bool Looping
		{
			get { return looping; }
			set { looping = value; }
		}
		public bool LoopTransition
		{
			get { return loopTransition; }
			set { loopTransition = value; }
		}


		public float AnimationSpeed
		{
			get { return speed; }
			set { if (value < 0) throw new ArgumentException("Negative values are not supported"); speed = value; }
		}

		public float AnimationTime
		{
			get { return frameTimer; } 
		}

		internal AnimationStreamControl(AnimationData animation)
		{
			speed = 1;
			weighting = 1;
			enabled = true;

			channels = new AnimationChannel[animation.BoneCount];

			this.animation = animation;

			Reset(true,false);
		}

		internal void Reset(bool newUsage, bool keepStored)
		{
			if (newUsage)
			{
				usageIndex++;
				firstFrame = true;
			}
			readFrameIndex = -1;
			currentFrameTime = 0;
			previousFrameTime = 0;
			storedFrameIndex = -1;

			for (int i = 0; i < channels.Length; i++)
			{
				if (!keepStored)
				{
					channels[i] = new AnimationChannel();
					channels[i].storeTransform = new Transform();
				}
				channels[i].frameReader = new CompressedTransformReader();
				channels[i].lerpedTransform = Transform.Identity;
				channels[i].sourceData = animation.GetBoneCompressedTransformData(i);
				channels[i].boneIndex = animation.BoneIndices[i];
				channels[i].readIndex = 0;
			}
		}

		internal void Seek(float time)
		{
			this.frameTimer = time;
		}

		internal bool Interpolate(float deltaTime)
		{
			if (!firstFrame)
			{
				if (speed == 0)
					return true;
				frameTimer += deltaTime * speed;
			}
			firstFrame = false;

			if (weighting == 0)
				return true;

			float frameTime = 0;
			float duration = Duration();
			
			if (duration != 0)
				frameTime = (float)(frameTimer - Math.Floor(frameTimer / duration) * duration);

			float scale = 1;

			if (fadeInTime != 0)
				scale *= Math.Max(0.0f, Math.Min(1.0f, frameTime / fadeInTime));
			if (fadeOutTime != 0)
				scale *= Math.Max(0.0f, Math.Min(1.0f, (duration - frameTime) / fadeOutTime));

			if (fadeOutStop != 0)
			{
				if (fadeOutStop == -1)
				{
					fadeOutStop = 0;
					return false;
				}
				float weight = this.weighting;
				this.weighting -= fadeOutStop * deltaTime;
				if ((this.weighting > 0) != (weight > 0))
				{
					fadeOutStop = 0;
					return false;
				}
			}

			while (true)
			{
				if (readFrameIndex == -1)
				{
					for (int i = 0; i < channels.Length; i++)
						channels[i].frameReader.MoveNext(channels[i].sourceData, ref channels[i].readIndex);
					readFrameIndex = 0;
					currentFrameTime = 0;
					previousFrameTime = 0;
				}

				if (frameTime <= currentFrameTime &&
					frameTime >= previousFrameTime)
					break;

				if (frameTime > currentFrameTime)
				{
					//move to next frame
					previousFrameTime = currentFrameTime;

					for (int i = 0; i < channels.Length; i++)
					{
						channels[i].storeTransform = channels[i].frameReader.value;
						channels[i].frameReader.MoveNext(channels[i].sourceData, ref channels[i].readIndex);
					}

					//special case, hit the end of the animation, need to interpolate to the start again
					if (readFrameIndex == animation.KeyFrameCount - 1 && (loopTransition && looping))
					{
						//reset now...
						this.Reset(false,true);

						for (int i = 0; i < channels.Length; i++)
							channels[i].frameReader.MoveNext(channels[i].sourceData, ref channels[i].readIndex);
						readFrameIndex = 0;
						currentFrameTime = duration;
						previousFrameTime = animation.Duration;
						continue;
					}
					else
					{
						readFrameIndex++;
						currentFrameTime = animation.KeyFrameTime[readFrameIndex];
					}
				}

				if (frameTime < previousFrameTime)
				{
					//need to reset
					if (!(loopTransition && looping))
					{
						//gone off the end of the animation
						this.Reset(false, false);

						//not looping? stop anim
						if (!looping)
						{
							this.frameTimer = 0;
							this.usageIndex++;
							return false;
						}
					}
					else
					{
						//special case, interpolating between the last and first frame
						if (readFrameIndex != 0)
						{
							//managed to skip over the entire interpolate frame, so reload
							this.Reset(false, true);

							for (int i = 0; i < channels.Length; i++)
								channels[i].frameReader.MoveNext(channels[i].sourceData, ref channels[i].readIndex);
							readFrameIndex = 0;
						}
						currentFrameTime = 0;
						previousFrameTime -= animation.Duration;
					}
				}
			}

			//interpolate
			float interp = 0;

			if (previousFrameTime != currentFrameTime)
				interp = (frameTime - previousFrameTime) / (currentFrameTime - previousFrameTime);

			if (interp == 1)
			{
				for (int i = 0; i < channels.Length; i++)
					channels[i].lerpedTransform = channels[i].storeTransform;
			}
			else if (interp == 0)
			{
				for (int i = 0; i < channels.Length; i++)
					channels[i].lerpedTransform = channels[i].frameReader.value;
			}
			else
			{
				//interpolate the keyframes
				for (int i = 0; i < channels.Length; i++)
				{
					Quaternion.Lerp(ref channels[i].storeTransform.Rotation, ref channels[i].frameReader.value.Rotation, interp, out channels[i].lerpedTransform.Rotation);
					Vector3.Lerp(ref channels[i].storeTransform.Translation, ref channels[i].frameReader.value.Translation, interp, out channels[i].lerpedTransform.Translation);
					channels[i].lerpedTransform.Scale = channels[i].frameReader.value.Scale * (1-interp) + channels[i].storeTransform.Scale * interp;
				}
			}

			scale *= weighting;

			//apply weighting
			if (scale != 1)
			{
				for (int i = 0; i < channels.Length; i++)
					channels[i].lerpedTransform.InterpolateToIdentity(scale);
			}

			return true;
		}

		internal float Duration()
		{
			float duration = animation.Duration;

			//special case, add a frame that interpolates from the end of the animation to the start
			if ((loopTransition && looping) && animation.KeyFrameCount > 0)
				duration += animation.Duration / (float)animation.KeyFrameCount;
			return duration;
		}
	}

	/// <summary>
	/// <para>Implement this abstract class to override shaders used by a <see cref="ModelInstance"/>.</para>
	/// <para>Return false in the <see cref="BeginGeometryShaderOverride"/> method if the model instance should use the default lighting shader</para>
	/// </summary>
	public abstract class ModelInstanceShaderProvider
	{
		/// <summary>
		/// <para>Called before drawing geometry</para>
		/// <para>Return true if the shader has been overridden</para>
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="lights"></param>
		/// <returns></returns>
		/// <remarks>If pushing the world matrix, make sure to pop it in <see cref="EndGeometry"/></remarks>
		/// <param name="state"></param>
		public virtual bool BeginGeometryShaderOverride(DrawState state, GeometryData geometry, MaterialLightCollection lights) { return false; }
		/// <summary>
		/// Called after drawing geometry
		/// </summary>
		/// <param name="state"></param>
		/// <param name="geometry"></param>
		public virtual void EndGeometry(DrawState state, GeometryData geometry) { }
		/// <summary>
		/// This method is called when animation transforms have been calculated, before drawing beings
		/// </summary>
		/// <param name="animationBoneHierarchy"></param>
		/// <param name="animationBoneHierarchyMatrix4x3">An array of Vector4 values storing the transform hierarchy in float4x3 format</param>
		/// <param name="state"></param>
		/// <remarks>If pushing the world matrix, make sure to pop it in <see cref="EndDraw"/>, and return true in <see cref="ProviderModifiesWorldMatrixInBeginDraw"/></remarks>
		public virtual void BeginDraw(DrawState state, Transform[] animationBoneHierarchy, Vector4[] animationBoneHierarchyMatrix4x3) { }
		/// <summary>
		/// Called before drawing the model instance begins, and no animation is active
		/// </summary>
		/// <param name="state"></param>
		public virtual void BeginDraw(DrawState state) { }
		/// <summary>
		/// Called after drawing the model
		/// </summary>
		/// <param name="state"></param>
		public virtual void EndDraw(DrawState state) { }
		/// <summary>
		/// Called before drawing a mesh
		/// </summary>
		/// <param name="mesh"></param>
		/// <remarks>If pushing the world matrix, make sure to pop it in <see cref="EndMesh"/></remarks>
		/// <param name="state"></param>
		public virtual void BeginMesh(DrawState state, MeshData mesh) { }
		/// <summary>
		/// Called after drawing a mesh
		/// </summary>
		/// <param name="state"></param>
		/// <param name="mesh"></param>
		public virtual void EndMesh(DrawState state, MeshData mesh) { }

		/// <summary>
		/// <para>Returns true if this provider modifies the world matrix in the <see cref="BeginDraw"/> method</para>
		/// <para>If true, the default CullTest will be skipped until BeginDraw() has been called</para>
		/// </summary>
		/// <remarks>Note: This property may be called frequenty, so should not run complex logic - it should simply return either true/false</remarks>
		public abstract bool ProviderModifiesWorldMatrixInBeginDraw { get;}
	}

	/// <summary>
	/// Draws <see cref="ModelData"/> loaded through the content pipeline
	/// </summary>
	/// <remarks>
	/// <para>
	/// When implementing shaders to draw the model, the following shader code example shows how to get the blend matrix when animation is enabled:
	/// </para>
	/// <para>
	/// <example>
	/// <code>
	/// //note: blendMatrices format is Matrix4x3 stored in Vector4s
	/// //in this example, there would be a maximum of 80 bones
	/// float4 blendMatrices[80*3];
	/// float4x4 worldViewProj : WORLDVIEWPROJECTION;
	/// 
	/// //This shader will be approximately 20 instructions
	/// 
	///	void vertexShader(
	///			float4	pos			: POSITION,
	///		out	float4	o_pos		: POSITION,
	///			float4	weights		: BLENDWEIGHT,
	///			int4	indices		: BLENDINDICES)
	///	{
	///		//transpose makes this easier to write (it gets optimised away by the shader compiler)
	///		float4x3 blendMat = transpose(float3x4(
	///						blendMatrices[indices.x*3+0] * weights.x + blendMatrices[indices.y*3+0] * weights.y + blendMatrices[indices.z*3+0] * weights.z + blendMatrices[indices.w*3+0] * weights.w,
	///						blendMatrices[indices.x*3+1] * weights.x + blendMatrices[indices.y*3+1] * weights.y + blendMatrices[indices.z*3+1] * weights.z + blendMatrices[indices.w*3+1] * weights.w,
	///						blendMatrices[indices.x*3+2] * weights.x + blendMatrices[indices.y*3+2] * weights.y + blendMatrices[indices.z*3+2] * weights.z + blendMatrices[indices.w*3+2] * weights.w
	///					   ));
	///
	///		float3 blendedPosition = mul(pos,blendMat).xyz;
	///		
	///		o_pos = mul(float4(blendedPosition,1),worldViewProj);
	///	}
	/// </code>
	/// </example>
	/// </para>
	/// </remarks>
	public sealed class ModelInstance : IDraw
	{
		private ModelData data;
		private AnimationController controller;
		private MaterialLightCollection lights;
		private MaterialAnimationTransformHierarchy hierarchy;
		private ModelInstanceShaderProvider shaderProvider;

		/// <summary>
		/// Construct the model instance. Setting the <see cref="ModelData"/> content is required before drawing
		/// </summary>
		public ModelInstance()
		{
		}

		/// <summary>
		/// Construct the model instance with existing model data
		/// </summary>
		/// <param name="sourceData"></param>
		public ModelInstance(ModelData sourceData)
		{
			this.data = sourceData;
		}

		/// <summary>
		/// Extend the <see cref="ModelInstanceShaderProvider"/> abstract class to override shaders or world matrices used by this model instance
		/// </summary>
		/// <param name="provider"></param>
		public void SetShaderOverride(ModelInstanceShaderProvider provider)
		{
			shaderProvider = provider;
		}

		/// <summary>
		/// <see cref="ModelData"/> used by this model instance. ModelData content must be assigned before the instance is drawn
		/// </summary>
		/// <remarks>ModelData may only be assigned once per instance</remarks>
		public ModelData ModelData
		{
			get { return data; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				if (this.data != null && this.data != value)
					throw new InvalidOperationException("MeshData may only be assigned once");
				this.data = value;
				if (controller != null)
					this.controller.SetModelData(value);
			}
		}

		/// <summary>
		/// Gets/Sets the lights collection used by any material shaders loaded with the model
		/// </summary>
		public MaterialLightCollection LightCollection
		{
			get { return lights; }
			set { lights = value; }
		}

		/// <summary>
		/// Gets/Creates an animation controller for this mesh instance
		/// </summary>
		/// <returns></returns>
		public AnimationController GetAnimationController()
		{
			if (data != null && data.skeleton == null)
				throw new InvalidOperationException("ModelData has no skeleton");
			if (controller == null)
				controller = new AnimationController(this.data, null, this);
			return controller;
		}

		/// <summary>
		/// <para>Gets/Creates an animation controller that runs as a thread task</para>
		/// <para>Async animations require adding to an <see cref="UpdateManager"/> because their processing is initalised at the end of the update loop</para>
		/// </summary>
		/// <param name="manager"></param>
		/// <returns></returns>
		public AnimationController GetAsyncAnimationController(UpdateManager manager)
		{
			if (manager == null)
				throw new ArgumentNullException();
			if (data == null)
				throw new InvalidOperationException("ModelData is null");
			if (data.skeleton == null)
				throw new InvalidOperationException("ModelData has no skeleton");
			if (controller == null)
				controller = new AnimationController(this.data, manager, this);
			return controller;
		}

		/// <summary>
		/// Share an existing animation controller with this model
		/// </summary>
		/// <param name="controller"></param>
		public void SetSharedAnimationController(AnimationController controller)
		{
			if (controller == null)
				throw new ArgumentNullException();
			if (this.controller == controller)
				return;
			if (this.controller != null)
				throw new InvalidOperationException("AnimationController already set");
			if (controller.ModelData != this.data || (this.data == null && controller.ModelData == null))
				throw new ArgumentException("ModelData mismatch");
			controller.AddParnet(this);
			this.controller = controller;
		}

		/// <summary>
		/// Draw the model. This class automatically assigns shaders when drawing
		/// </summary>
		/// <param name="state"></param>
		public void Draw(DrawState state)
		{
			if (data == null)
				return;

			if (controller != null)
			{
				controller.WaitForAsyncAnimation(state,state.FrameIndex,true);

				if (controller.IsDisposed)
					controller = null;
			}

			if (controller != null && hierarchy == null)
				hierarchy = new MaterialAnimationTransformHierarchy(data.skeleton);

			if (hierarchy != null)
				hierarchy.UpdateTransformHierarchy(controller.transformedBones);

			if (shaderProvider != null)
			{
				if (controller != null)
					shaderProvider.BeginDraw(state,controller.transformedBones, hierarchy.GetMatrixData());
				else
					shaderProvider.BeginDraw(state);
			}

			Vector3 boundsMin, boundsMax;

			ContainmentType cullModel;
			if (controller != null)
				cullModel = state.Culler.IntersectBox(ref controller.boundsMin, ref controller.boundsMax);
			else
				cullModel = state.Culler.IntersectBox(data.StaticBounds.Minimum, data.StaticBounds.Maximum);

			if (cullModel != ContainmentType.Disjoint)
			{
				for (int m = 0; m < data.meshes.Length; m++)
				{
					MeshData mesh = data.meshes[m];

					if (shaderProvider != null)
						shaderProvider.BeginMesh(state, mesh);

					ContainmentType cullMesh = cullModel;

					if (cullModel == ContainmentType.Intersects && data.meshes.Length > 1)
					{
						if (controller != null)
						{
							controller.ComputeMeshBounds(m, out boundsMin, out boundsMax);
							cullMesh = state.Culler.IntersectBox(ref boundsMin, ref boundsMax);
						}
						else
							cullMesh = state.Culler.IntersectBox(mesh.StaticBounds.Minimum, mesh.StaticBounds.Maximum);

					}

					if (cullMesh != ContainmentType.Disjoint)
					{
						for (int g = 0; g < mesh.geometry.Length; g++)
						{
							GeometryData geom = mesh.geometry[g];
							MaterialShader shader = geom.MaterialShader;

							if (shaderProvider != null && shaderProvider.BeginGeometryShaderOverride(state, geom, lights))
								shader = null;

							bool cullTest = true;

							if (cullMesh == ContainmentType.Intersects && mesh.geometry.Length > 1)
							{
								if (controller != null)
								{
									controller.ComputeGeometryBounds(m, g, out boundsMin, out boundsMax);
									cullTest = state.Culler.TestBox(ref boundsMin, ref boundsMax);
								}
								else
									cullTest = state.Culler.TestBox(geom.StaticBounds.Minimum, geom.StaticBounds.Maximum);
							}

							if (cullTest)
							{
								if (shader != null)
								{
									shader.AnimationTransforms = hierarchy;
									shader.Lights = this.lights;
									shader.Bind(state);
								}

								geom.Vertices.Draw(state, geom.Indices, PrimitiveType.TriangleList);
							}

							if (shaderProvider != null)
								shaderProvider.EndGeometry(state, geom);
						}
					}

					if (shaderProvider != null)
						shaderProvider.EndMesh(state, mesh);
				}
			}

			if (shaderProvider != null)
				shaderProvider.EndDraw(state);
		}

		/// <summary>
		/// FrustumCull test the model
		/// </summary>
		/// <param name="culler"></param>
		/// <returns></returns>
		public bool CullTest(ICuller culler)
		{
			if (data == null)
				return false;

			if (shaderProvider != null && shaderProvider.ProviderModifiesWorldMatrixInBeginDraw)
				return true;

			if (controller == null)
				return culler.TestBox(data.StaticBounds.Minimum, data.StaticBounds.Maximum);
			else
			{
				controller.WaitForAsyncAnimation(culler.GetState(), culler.FrameIndex,false);
				return culler.TestBox(ref controller.boundsMin, ref controller.boundsMax);
			}
		}
	}
}
