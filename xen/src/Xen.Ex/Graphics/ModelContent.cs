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

namespace Xen.Ex.Graphics.Content
{
	public struct GeometryBounds
	{
		public readonly Vector3 Minimum,Maximum;
		public readonly Vector3 RadiusCentre;
		public readonly float Radius;

#if DEBUG

		public GeometryBounds(Vector3 max, Vector3 min, float radius, Vector3 radiusCentre)
		{
			this.Maximum = max;
			this.Minimum = min;
			this.Radius = radius;
			this.RadiusCentre = radiusCentre;
		}

		public GeometryBounds Transform(ref Matrix transform)
		{
			Vector3 RadiusCentre = this.RadiusCentre;
			Vector3 Minimum = this.Minimum;
			Vector3 Maximum = this.Maximum;
			float Radius = this.Radius;

			Vector3.Transform(ref RadiusCentre, ref transform, out RadiusCentre);
			Vector3 min,max;
			Vector3.Transform(ref Minimum, ref transform, out min);
			max = min;

			Vector3 point = new Vector3();
			for (int x = 0; x < 2; x++)
			for (int y = 0; y < 2; y++)
			for (int z = 0; z < 2; z++)
			{
				point.X = x == 0 ? this.Minimum.X : this.Maximum.X;
				point.Y = y == 0 ? this.Minimum.Y : this.Maximum.Y;
				point.Z = z == 0 ? this.Minimum.Z : this.Maximum.Z;
				Vector3.Transform(ref point, ref transform, out point);

				Vector3.Min(ref min, ref point, out min);
				Vector3.Max(ref max, ref point, out max);
			}

			Minimum = min;
			Maximum = max;
			return new GeometryBounds(Maximum, Minimum, Radius, RadiusCentre);
		}

#endif

		public GeometryBounds(BinaryReader reader)
		{
			this.Maximum = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			this.Minimum = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			this.Radius = reader.ReadSingle();
			this.RadiusCentre = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		internal GeometryBounds(GeometryBounds[] bounds)
		{
			Maximum = new Vector3();
			Minimum = new Vector3();
			Vector3 v;

			if (bounds.Length != 0)
				this = bounds[0];
			for (int i = 1; i < bounds.Length; i++)
			{
				v = bounds[i].Maximum;
				Vector3.Max(ref v, ref this.Maximum, out Maximum);
				v = bounds[i].Minimum;
				Vector3.Min(ref v, ref this.Minimum, out Minimum);
			}
			this.RadiusCentre = Minimum + (Maximum - Minimum) * 0.5f;
			this.Radius = 0;
			for (int i = 0; i < bounds.Length; i++)
			{
				Vector3 point = new Vector3();
				for (int x = 0; x < 2; x++)
				for (int y = 0; y < 2; y++)
				for (int z = 0; z < 2; z++)
				{
					point.X = x == 0 ? bounds[i].Minimum.X : bounds[i].Maximum.X;
					point.Y = y == 0 ? bounds[i].Minimum.Y : bounds[i].Maximum.Y;
					point.Z = z == 0 ? bounds[i].Minimum.Z : bounds[i].Maximum.Z;
					this.Radius = Math.Max(this.Radius, (this.RadiusCentre - point).LengthSquared());
				}
			}
			this.Radius = (float)Math.Sqrt(this.Radius);
		}

#if DEBUG

		private GeometryBounds Add(GeometryBounds bound)
		{
			return new GeometryBounds(this.Maximum + bound.Maximum, this.Minimum + bound.Minimum, this.Radius + bound.Radius, this.RadiusCentre + bound.RadiusCentre);
		}
		internal GeometryBounds Difference(GeometryBounds bounds)
		{
			return new GeometryBounds(this.Maximum - bounds.Maximum, this.Minimum - bounds.Minimum, this.Radius - bounds.Radius, this.RadiusCentre - bounds.RadiusCentre);
		}

		internal GeometryBounds(GeometryBounds staticBound, GeometryBounds[] offsets)
		{
			GeometryBounds[] bounds = new GeometryBounds[offsets.Length];

			for (int i = 0; i < offsets.Length; i++)
				bounds[i] = staticBound.Add(offsets[i]);

			this = new GeometryBounds(bounds).Difference(staticBound);
		}
		public GeometryBounds Combine(ref GeometryBounds bound)
		{
			GeometryBounds previous = this;
			Vector3 v;
			Vector3 RadiusCentre = this.RadiusCentre;
			Vector3 Minimum = this.Minimum;
			Vector3 Maximum = this.Maximum;
			float Radius = this.Radius;


			v = bound.Maximum;
			Vector3.Max(ref v, ref Maximum, out Maximum);
			v = bound.Minimum;
			Vector3.Min(ref v, ref Minimum, out Minimum);

			RadiusCentre = Minimum + (Maximum - Minimum) * 0.5f;
			Radius = 0;

			Vector3 point = new Vector3();
			for (int x = 0; x < 2; x++)
			for (int y = 0; y < 2; y++)
			for (int z = 0; z < 2; z++)
			{
				point.X = x == 0 ? bound.Minimum.X : bound.Maximum.X;
				point.Y = y == 0 ? bound.Minimum.Y : bound.Maximum.Y;
				point.Z = z == 0 ? bound.Minimum.Z : bound.Maximum.Z;
				Radius = Math.Max(Radius, (RadiusCentre - point).LengthSquared());
			}
			for (int x = 0; x < 2; x++)
			for (int y = 0; y < 2; y++)
			for (int z = 0; z < 2; z++)
			{
				point.X = x == 0 ? previous.Minimum.X : previous.Maximum.X;
				point.Y = y == 0 ? previous.Minimum.Y : previous.Maximum.Y;
				point.Z = z == 0 ? previous.Minimum.Z : previous.Maximum.Z;
				Radius = Math.Max(Radius, (RadiusCentre - point).LengthSquared());
			}

			Radius = (float)Math.Sqrt(Radius);
			return new GeometryBounds(Maximum, Minimum, Radius, RadiusCentre);
		}
		internal void Write(BinaryWriter writer)
		{
			writer.Write(Maximum.X);
			writer.Write(Maximum.Y);
			writer.Write(Maximum.Z);

			writer.Write(Minimum.X);
			writer.Write(Minimum.Y);
			writer.Write(Minimum.Z);

			writer.Write(Radius);

			writer.Write(RadiusCentre.X);
			writer.Write(RadiusCentre.Y);
			writer.Write(RadiusCentre.Z);
		}

#endif
	}
	public class ModelData
	{
		private const int version = 1;
		readonly private string name;
		readonly internal MeshData[] meshes;
		readonly internal SkeletonData skeleton;
		readonly internal AnimationData[] animations;
		internal readonly GeometryBounds[] animationStaticBounds;

		/// <summary>
		/// Gets the bounding box for the non-animated model data
		/// </summary>
		public readonly GeometryBounds StaticBounds;

		internal class RuntimeReader : ContentTypeReader<ModelData>
		{
			protected override ModelData Read(ContentReader input, ModelData existingInstance)
			{
				if (existingInstance != null)
					existingInstance.UpdateTextures(input.ContentManager, Path.GetDirectoryName(input.AssetName));

				return existingInstance ?? new ModelData(input);
			}
		}

		internal static string RuntimeReaderType
		{
			get { return typeof(RuntimeReader).AssemblyQualifiedName; }
		}

#if DEBUG

		public ModelData(string name, MeshData[] meshes, SkeletonData skeleton, AnimationData[] animations)
		{
			this.name = name;
			this.meshes = meshes;
			this.skeleton = skeleton;
			this.animations = animations;

			GeometryBounds[] bounds = new GeometryBounds[meshes.Length];
			for (int i = 0; i < bounds.Length; i++)
				bounds[i] = meshes[i].StaticBounds;

			this.StaticBounds = new GeometryBounds(bounds);

			this.animationStaticBounds = new GeometryBounds[animations.Length];
			for (int i = 0; i < animations.Length; i++)
			{
				GeometryBounds[] children = new GeometryBounds[meshes.Length];
				for (int n = 0; n < meshes.Length; n++)
					children[n] = meshes[n].AnimationStaticBoundsOffset[i];
				this.animationStaticBounds[i] = new GeometryBounds(this.StaticBounds, children);
			}
		}

#endif

		internal void UpdateTextures(ContentManager manager, string baseDir)
		{
			foreach (MeshData mesh in this.meshes)
			{
				foreach (GeometryData geom in mesh.geometry)
				{
					geom.UpdateTextures(manager, baseDir);
				}
			}
		}

		public string Name { get { return name; } }
		public ReadOnlyArrayCollection<MeshData> Meshes { get { return new ReadOnlyArrayCollection<MeshData>(meshes); } }
		public ReadOnlyArrayCollection<AnimationData> Animations { get { return new ReadOnlyArrayCollection<AnimationData>(animations); } }
		public SkeletonData Skeleton { get { return skeleton; } }
		/// <summary>
		/// <para>Gets bounds offsets for each animation (assuming animation has a weighting of 1.0f)</para>
		/// <para>This offset is primarily used to adjust the bounding box of a model when an animation is playing</para>
		/// </summary>
		public ReadOnlyArrayCollection<GeometryBounds> AnimationStaticBoundsOffset
		{
			get { return new ReadOnlyArrayCollection<GeometryBounds>(animationStaticBounds); }
		}

		internal ModelData(ContentReader reader)
		{
			int fileVersion = reader.ReadInt32();
			if (version != fileVersion)
				throw new InvalidOperationException("Serialiezd ModelData version mismatch");
			this.name = string.Intern(reader.ReadString());
			int count = reader.ReadInt32();
			this.meshes = new MeshData[count];
			for (int i = 0; i < count; i++)
				this.meshes[i] = new MeshData(reader);
			bool hasSkel = reader.ReadBoolean();
			if (hasSkel)
				this.skeleton = new SkeletonData(reader);
			count = reader.ReadInt32();
			this.animations = new AnimationData[count];
			for (int i = 0; i < count; i++)
				this.animations[i] = new AnimationData(reader, i);

			GeometryBounds[] bounds = new GeometryBounds[meshes.Length];
			for (int i = 0; i < bounds.Length; i++)
				bounds[i] = meshes[i].StaticBounds;

			this.StaticBounds = new GeometryBounds(bounds);

			count = (int)reader.ReadInt16();
			animationStaticBounds = new GeometryBounds[count];
			for (int i = 0; i < animationStaticBounds.Length; i++)
				animationStaticBounds[i] = new GeometryBounds(reader);
		}

#if DEBUG

		internal void Write(BinaryWriter writer)
		{
			writer.Write(version);
			writer.Write(name ?? "");
			writer.Write(meshes.Length);
			foreach (MeshData mesh in meshes)
				mesh.Write(writer);
			writer.Write(skeleton != null);
			if (skeleton != null)
				skeleton.Write(writer);
			writer.Write(animations.Length);
			foreach (AnimationData animation in animations)
				animation.Write(writer);

			writer.Write((short)animationStaticBounds.Length);
			for (int i = 0; i < animationStaticBounds.Length; i++)
				animationStaticBounds[i].Write(writer);
		}

#endif
	}

	public class MeshData
	{
		readonly private string name;
		readonly internal GeometryData[] geometry;
		/// <summary>
		/// Gets the bounding box for the non-animated mesh data
		/// </summary>
		public readonly GeometryBounds StaticBounds;
		internal readonly GeometryBounds[] animationStaticBounds;

#if DEBUG

		public MeshData(string name, GeometryData[] data, AnimationData[] animations)
		{
			this.name = name;
			this.geometry = data;

			GeometryBounds[] bounds = new GeometryBounds[data.Length];
			for (int i = 0; i < bounds.Length; i++)
				bounds[i] = data[i].StaticBounds;

			this.StaticBounds = new GeometryBounds(bounds);

			this.animationStaticBounds = new GeometryBounds[animations.Length];
			for (int i = 0; i < animations.Length; i++)
			{
				GeometryBounds[] children = new GeometryBounds[data.Length];
				for (int n = 0; n < data.Length; n++)
					children[n] = data[n].AnimationStaticBoundsOffset[i];
				this.animationStaticBounds[i] = new GeometryBounds(this.StaticBounds, children);
			}
		}

#endif

		public ReadOnlyArrayCollection<GeometryData> Geometry
		{
			get { return new ReadOnlyArrayCollection<GeometryData>(geometry); }
		}
		public string Name { get { return name; } }

		/// <summary>
		/// <para>Gets bounds offsets for each animation (assuming animation has a weighting of 1.0f)</para>
		/// <para>This offset is primarily used to adjust the bounding box of a model when an animation is playing</para>
		/// </summary>
		public ReadOnlyArrayCollection<GeometryBounds> AnimationStaticBoundsOffset
		{
			get { return new ReadOnlyArrayCollection<GeometryBounds>(animationStaticBounds); }
		}

#if DEBUG

		internal void Write(BinaryWriter writer)
		{
			writer.Write(name ?? "");

			writer.Write(geometry.Length);
			for (int i = 0; i < geometry.Length; i++)
				geometry[i].Write(writer);

			writer.Write((short)animationStaticBounds.Length);
			for (int i = 0; i < animationStaticBounds.Length; i++)
				animationStaticBounds[i].Write(writer);
		}

#endif

		internal MeshData(ContentReader reader)
		{
			name = string.Intern(reader.ReadString());

			int count = reader.ReadInt32();
			this.geometry = new GeometryData[count];
			for (int i = 0; i < count; i++)
				this.geometry[i] = new GeometryData(reader);

			GeometryBounds[] bounds = new GeometryBounds[geometry.Length];
			for (int i = 0; i < bounds.Length; i++)
				bounds[i] = geometry[i].StaticBounds;

			this.StaticBounds = new GeometryBounds(bounds);

			count = (int)reader.ReadInt16();
			animationStaticBounds = new GeometryBounds[count];
			for (int i = 0; i < animationStaticBounds.Length; i++)
				animationStaticBounds[i] = new GeometryBounds(reader);
		}
	}

	public struct MaterialData
	{
		internal readonly float alpha, specularPower;
		internal readonly Vector3 diffuse, emissive, specular;
		internal readonly string textureFileName, normalMapFileName;
		internal readonly bool useVertexColour;
		private Texture2D textureMap, normalMap;

		public float Alpha { get { return alpha; } }
		public float SpecularPower { get { return specularPower; } }
		public Vector3 DiffuseColour { get { return diffuse; } }
		public Vector3 EmissiveColour { get { return emissive; } }
		public Vector3 SpecularColour { get { return specular; } }
		public bool UseVertexColour { get { return useVertexColour; } }
		public Texture2D Texture { get { return textureMap; } }
		public Texture2D NormalMap { get { return normalMap; } }

#if DEBUG

		public MaterialData(
			float alpha, float specularPower,
			Vector3 diffuse, Vector3 emissive, Vector3 specular,
			string texture, string normalMap,
			bool useVertexColour)
		{
			this.alpha = alpha;
			this.specularPower = specularPower;
			this.diffuse = diffuse;
			this.emissive = emissive;
			this.specular = specular;
			this.textureFileName = texture;
			this.normalMapFileName = normalMap;
			this.useVertexColour = useVertexColour;
			this.textureMap = null;
			this.normalMap = null;
		}

#endif

		internal MaterialData(ContentReader reader)
		{
			this.alpha = reader.ReadSingle();
			this.specularPower = reader.ReadSingle();
			this.diffuse = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			this.emissive = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			this.specular = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			this.textureFileName = string.Intern(reader.ReadString());
			this.normalMapFileName = string.Intern(reader.ReadString());
			this.useVertexColour = reader.ReadBoolean();

			this.textureMap = null;
			this.normalMap = null;

			UpdateTextures(reader.ContentManager, Path.GetDirectoryName(reader.AssetName));
		}

#if DEBUG

		public void Write(BinaryWriter writer)
		{
			writer.Write(alpha);
			writer.Write(specularPower);
			writer.Write(diffuse.X);
			writer.Write(diffuse.Y);
			writer.Write(diffuse.Z);

			writer.Write(emissive.X);
			writer.Write(emissive.Y);
			writer.Write(emissive.Z);

			writer.Write(specular.X);
			writer.Write(specular.Y);
			writer.Write(specular.Z);

			writer.Write(textureFileName);
			writer.Write(normalMapFileName);
			writer.Write(useVertexColour);
		}

#endif

		internal void UpdateTextures(ContentManager manager, string baseDir)
		{
			this.textureMap = null;
			this.normalMap = null;
			if (baseDir.Length > 0)
				baseDir += @"\";

			if (this.textureFileName != null && this.textureFileName.Length > 0)
				this.textureMap = manager.Load<Texture2D>(baseDir.Length == 0 ? this.textureFileName : baseDir + this.textureFileName);
			if (this.normalMapFileName != null && this.normalMapFileName.Length > 0)
				this.normalMap = manager.Load<Texture2D>(baseDir.Length == 0 ? this.normalMapFileName : baseDir + this.normalMapFileName);
		}
	}

	public class GeometryData
	{
		readonly private string name;
		readonly private VertexElement[] vertexElements;
		readonly private IVertices vertices;
		readonly private IIndices indices;
		readonly private MaterialData material;
		private MaterialShader shader;
		private readonly int[] boneIndices;
		private readonly GeometryBounds[] boneLocalBounds;
		/// <summary>
		/// Gets the bounding box for the non-animated geometry data
		/// </summary>
		public readonly GeometryBounds StaticBounds;
		internal readonly GeometryBounds[] animationStaticBounds;

#if DEBUG
		readonly private byte[] vertexData;
		readonly private int maxIndex;
		readonly private int[] indexData;

		public GeometryData(string name, VertexElement[] elements, byte[] data, int[] indexData, MaterialData material, SkeletonData skeleton, AnimationData[] animations, bool targetXbox)
		{
			this.name = name;
			this.vertexElements = elements;
			this.vertexData = data;
			this.vertices = null;
			this.indices = null;
			this.indexData = indexData;
			for (int i = 0; i < indexData.Length; i++)
				maxIndex = Math.Max(indexData[i], maxIndex);
			this.material = material;

			ComputeBoneBounds(skeleton, ExtractBounds(out this.StaticBounds, targetXbox), out boneIndices, out boneLocalBounds);

			if (animations != null)
			{
				this.animationStaticBounds = new GeometryBounds[animations.Length];
				ComputeAnimationBounds(skeleton, animations);
			}
			else
				animationStaticBounds = new GeometryBounds[0];
		}

		private void ComputeAnimationBounds(SkeletonData skeleton, AnimationData[] animations)
		{
			if (skeleton == null)
				return;
		
			Matrix[] worldBones = skeleton.BoneLocalMatrices.ToArray();
			Matrix[] transforms = new Matrix[skeleton.BoneCount];
			skeleton.TransformHierarchy(worldBones);

			for (int animIndex = 0; animIndex < animations.Length; animIndex++)
			{
				GeometryBounds bounds = new GeometryBounds();
				AnimationData anim = animations[animIndex];

				BinaryReader[] data = new BinaryReader[anim.BoneCount];
				CompressedTransformReader[] readers = new CompressedTransformReader[anim.BoneCount];


				for (int i = 0; i < anim.BoneCount; i++)
				{
					data[i] = new BinaryReader(anim.GetBoneCompressedTransformStream(i));
					readers[i] = new CompressedTransformReader();
				}

				for (int frame = 0; frame < anim.KeyFrameCount; frame++)
				{
					for (int i = 0; i < transforms.Length; i++)
						transforms[i] = Matrix.Identity;

					Transform t;
					for (int i = 0; i < anim.BoneCount; i++)
					{
						int bi = anim.BoneIndices[i];
						readers[i].MoveNext(data[i]);
						readers[i].GetTransform(out t);

						t.GetMatrix(out transforms[bi]);
					}
					skeleton.TransformHierarchy(transforms);

					for (int i = 0; i < transforms.Length; i++)
						Matrix.Multiply(ref worldBones[i], ref transforms[i], out transforms[i]);

					for (int i = 0; i < this.boneIndices.Length; i++)
					{
						int bi = this.boneIndices[i];

						GeometryBounds bound = this.boneLocalBounds[i];
						bound = bound.Transform(ref transforms[bi]);
						if (frame == 0 && i == 0)
							bounds = bound;
						else
							bounds = bounds.Combine(ref bound);
					}
				}

				animationStaticBounds[animIndex] = bounds.Difference(StaticBounds);
			}
		}

		#region compute bounds

		struct BlendedVertex
		{
			public Vector3 positon;
			public int i0,i1,i2,i3;
		}

		void ComputeBoneBounds(SkeletonData skeleton, BlendedVertex[] vertices, out int[] boneIndices, out GeometryBounds[] boneLocalBounds)
		{
			boneIndices = null;
			boneLocalBounds = null;

			if (skeleton == null)
				return;

			Matrix[] bones = (Matrix[])skeleton.boneLocalMatrices.Clone();
			skeleton.TransformHierarchy(bones);

			for (int i = 0; i < bones.Length; i++)
				Matrix.Invert(ref bones[i], out bones[i]);

			int bc = skeleton.BoneCount;
			List<Vector3>[] boneVertices = new List<Vector3>[skeleton.BoneCount];

			foreach (BlendedVertex v in vertices)
			{
				if (v.i0 >= 0 && v.i0 < bc) AddList(ref boneVertices[v.i0], v.positon, ref bones[v.i0]);
				if (v.i1 >= 0 && v.i1 < bc) AddList(ref boneVertices[v.i1], v.positon, ref bones[v.i1]);
				if (v.i2 >= 0 && v.i2 < bc) AddList(ref boneVertices[v.i2], v.positon, ref bones[v.i2]);
				if (v.i3 >= 0 && v.i3 < bc) AddList(ref boneVertices[v.i3], v.positon, ref bones[v.i3]);
			}

			List<int> indices = new List<int>();
			List<GeometryBounds> bounds = new List<GeometryBounds>();

			for (int i = 0; i < boneVertices.Length; i++)
			{
				List<Vector3> verts = boneVertices[i];
				if (verts == null ||
					verts.Count == 0)
					continue;

				Vector3 max = verts[0], min = verts[0];
				for (int v = 1; v < verts.Count; v++)
				{
					Vector3 vert = verts[v];
					Vector3.Max(ref max, ref vert, out max);
					Vector3.Min(ref min, ref vert, out min);
				}
				Vector3 centre = min + (max - min) * 0.5f;
				float radius = 0;
				for (int v = 0; v < verts.Count; v++)
				{
					radius = Math.Max(radius, (verts[v] - centre).Length());
				}

				indices.Add(i);
				bounds.Add(new GeometryBounds(max, min, radius, centre));
			}

			boneIndices = indices.ToArray();
			boneLocalBounds = bounds.ToArray();
		}
		void AddList(ref List<Vector3> list, Vector3 v, ref Matrix mat)
		{
			if (list == null)
				list = new List<Vector3>();
			Vector3.Transform(ref v, ref mat, out v);
			list.Add(v);
		}

		BlendedVertex[] ExtractBounds(out GeometryBounds staticBounds, bool targetXbox)
		{
			staticBounds = new GeometryBounds();

			VertexElementFormat format, blendFormat, weightFormat;

			int offset, blendOffset, weightOffset;
			int stride = VertexElementAttribute.CalculateVertexStride(this.vertexElements);
			if (!VertexElementAttribute.ExtractUsage(this.vertexElements, VertexElementUsage.Position, 0, out format, out offset))
				throw new ArgumentException("Geometry data vertex format has no position values");
			if (format != VertexElementFormat.Vector3 &&
				format != VertexElementFormat.Vector4)
				return null;


			bool hasBlending = VertexElementAttribute.ExtractUsage(this.vertexElements, VertexElementUsage.BlendIndices, 0, out blendFormat, out blendOffset) &&
				blendFormat == VertexElementFormat.Byte4;

			hasBlending &= VertexElementAttribute.ExtractUsage(this.vertexElements, VertexElementUsage.BlendWeight, 0, out weightFormat, out weightOffset) &&
				weightFormat == VertexElementFormat.Vector4;

			BlendedVertex[] verts = new BlendedVertex[this.vertexData.Length / stride];
			for (int i = 0; i < verts.Length; i++)
			{
				verts[i].i0 = -1;
				verts[i].i1 = -1;
				verts[i].i2 = -1;
				verts[i].i3 = -1;
			}

			Vector3 min = new Vector3(), max = new Vector3();
			float[] values = new float[4];
			BitCast cast = new BitCast();

			int index = offset;
			int blendIndex = blendOffset;
			int weightIndex = weightOffset;
			bool first = true;
			int v = 0;

			while (index < this.vertexData.Length - 12)
			{
				for (int i = 0; i < 12; )
				{
					if (targetXbox)
					{
						cast.Byte3 = this.vertexData[index + i++];
						cast.Byte2 = this.vertexData[index + i++];
						cast.Byte1 = this.vertexData[index + i++];
						cast.Byte0 = this.vertexData[index + i];
					}
					else
					{
						cast.Byte0 = this.vertexData[index + i++];
						cast.Byte1 = this.vertexData[index + i++];
						cast.Byte2 = this.vertexData[index + i++];
						cast.Byte3 = this.vertexData[index + i];
					}

					values[i++ / 4] = cast.Single;
				}
				Vector3 pos = new Vector3(values[0], values[1], values[2]);

				if (first)
				{
					first = false;
					min = pos;
					max = pos;
				}
				else
				{
					Vector3.Max(ref pos, ref max, out max);
					Vector3.Min(ref pos, ref min, out min);
				}

				if (hasBlending)
				{
					if (targetXbox)
					{
						verts[v].i3 = this.vertexData[blendOffset + 0];
						verts[v].i2 = this.vertexData[blendOffset + 1];
						verts[v].i1 = this.vertexData[blendOffset + 2];
						verts[v].i0 = this.vertexData[blendOffset + 3];
					}
					else
					{
						verts[v].i0 = this.vertexData[blendOffset + 0];
						verts[v].i1 = this.vertexData[blendOffset + 1];
						verts[v].i2 = this.vertexData[blendOffset + 2];
						verts[v].i3 = this.vertexData[blendOffset + 3];
					}

					for (int i = 0; i < 16; )
					{
						if (targetXbox)
						{
							cast.Byte3 = this.vertexData[weightOffset + i++];
							cast.Byte2 = this.vertexData[weightOffset + i++];
							cast.Byte1 = this.vertexData[weightOffset + i++];
							cast.Byte0 = this.vertexData[weightOffset + i];
						}
						else
						{
							cast.Byte0 = this.vertexData[weightOffset + i++];
							cast.Byte1 = this.vertexData[weightOffset + i++];
							cast.Byte2 = this.vertexData[weightOffset + i++];
							cast.Byte3 = this.vertexData[weightOffset + i];
						}

						values[i++ / 4] = cast.Single;
					}

					if (values[0] <= 0)
						verts[v].i0 = -1;
					if (values[1] <= 0)
						verts[v].i1 = -1;
					if (values[2] <= 0)
						verts[v].i2 = -1;
					if (values[3] <= 0)
						verts[v].i3 = -1;
				}

				verts[v++].positon = pos;

				index += stride;
				blendOffset += stride;
				weightOffset += stride;
			}

			Vector3 centre = min + (max - min) * 0.5f;
			float radius = 0;
			for (int i = 0; i < verts.Length; i++)
				radius = Math.Max(radius, (verts[i].positon - centre).LengthSquared());

			staticBounds = new GeometryBounds(max, min,(float)Math.Sqrt(radius),centre);
			return verts;
		}
		
		#endregion

#endif
		public void GetMaterial(out MaterialData material)
		{
			material = this.material;
		}

		/// <summary>
		/// <para>Gets bounds offsets for each animation (assuming animation has a weighting of 1.0f)</para>
		/// <para>This offset is primarily used to adjust the bounding box of a model when an animation is playing</para>
		/// </summary>
		public ReadOnlyArrayCollection<GeometryBounds> AnimationStaticBoundsOffset
		{
			get { return new ReadOnlyArrayCollection<GeometryBounds>(animationStaticBounds); }
		}

		/// <summary>
		/// Gets/Creates a <see cref="MaterialShader"/> instance matching the <see cref="MaterialData"/> of this geometry (see <see cref="GetMaterial"/>)
		/// </summary>
		public MaterialShader MaterialShader
		{
			get
			{
				if (shader == null)
					BuildMaterialShader();
				return shader;
			}
		}
		/// <summary>
		/// Gets the indices of the bones this geometry uses
		/// </summary>
		public ReadOnlyArrayCollection<int> BoneIndices { get { return new ReadOnlyArrayCollection<int>(boneIndices); } }
		/// <summary>
		/// <para>Gets the bone-local bounds for this geometry (based on <see cref="BoneIndices"/>)</para>
		/// <para>These bounds could be used as collision detection bounding boxes</para>
		/// </summary>
		public ReadOnlyArrayCollection<GeometryBounds> BoneLocalBounds { get { return new ReadOnlyArrayCollection<GeometryBounds>(boneLocalBounds); } }

		private void BuildMaterialShader()
		{
			if (shader == null)
				shader = new MaterialShader();
			shader.Alpha = material.alpha;
			shader.DiffuseColour = material.diffuse;
			shader.EmissiveColour = material.emissive;
			shader.SpecularColour = material.specular;
			shader.SpecularPower = material.specularPower;
			shader.UseVertexColour = material.useVertexColour;
			shader.TextureMap = material.Texture;
			shader.NormalMap = material.NormalMap;
			shader.UsePerPixelSpecular = 
				material.SpecularColour.Length() > 0 && 
				material.specularPower > 0 && 
				material.normalMapFileName != null && 
				material.normalMapFileName.Length > 0;
		}

		internal void UpdateTextures(ContentManager manager, string baseDir)
		{
			material.UpdateTextures(manager, baseDir);
			if (shader != null)
				BuildMaterialShader();
		}

		public IVertices Vertices
		{
			get { return vertices; }
		}
		public IIndices Indices
		{
			get { return indices; }
		}
		public string Name { get { return name; } }

#if DEBUG

		internal void Write(BinaryWriter writer)
		{
			writer.Write(name ?? "");
			writer.Write(vertexElements.Length);
			for (int i = 0; i < vertexElements.Length; i++)
			{
				Write(vertexElements[i], writer);
			}
			writer.Write(vertexData.Length);
			writer.Write(vertexData);

			byte bits = 8;
			if (maxIndex > byte.MaxValue)
				bits = 16;
			if (maxIndex > short.MaxValue)
				bits = 32;

			writer.Write(bits);
			writer.Write(this.indexData.Length);
			if (bits == 8)
				for (int i = 0; i < this.indexData.Length; i++)
					writer.Write((byte)this.indexData[i]);
			if (bits == 16)
				for (int i = 0; i < this.indexData.Length; i++)
					writer.Write((ushort)this.indexData[i]);
			if (bits == 32)
				for (int i = 0; i < this.indexData.Length; i++)
					writer.Write(this.indexData[i]);
			this.material.Write(writer);

			this.StaticBounds.Write(writer);

			if (boneIndices == null)
				writer.Write((short)0);
			else
			{
				writer.Write((short)boneIndices.Length);
				for (int i = 0; i < boneIndices.Length; i++)
					writer.Write((short)boneIndices[i]);
				for (int i = 0; i < boneLocalBounds.Length; i++)
					boneLocalBounds[i].Write(writer);
			}

			writer.Write((short)animationStaticBounds.Length);
			for (int i = 0; i < animationStaticBounds.Length; i++)
				animationStaticBounds[i].Write(writer);
		}

#endif

		internal GeometryData(ContentReader reader)
		{
			name = string.Intern(reader.ReadString());
			int count = reader.ReadInt32();
			this.vertexElements = new VertexElement[count];
			for (int i = 0; i < count; i++)
				this.vertexElements[i] = ReadElement(reader);
			count = reader.ReadInt32();
			byte[] data = reader.ReadBytes(count);
			this.vertices = Vertices<byte>.CreateRawDataVertices(data, vertexElements);

			byte bits = reader.ReadByte();
			count = reader.ReadInt32();

			if (bits == 8 || bits == 16)
			{
				ushort[] inds = new ushort[count];

				if (bits == 8)
				{
					for (int i = 0; i < count; i++)
						inds[i] = (ushort)reader.ReadByte();
				}
				else
				{
					for (int i = 0; i < count; i++)
						inds[i] = reader.ReadUInt16();
				}
				this.indices = new Indices<ushort>(inds);
			}
			if (bits == 32)
			{
				int[] inds = new int[count];
				for (int i = 0; i < count; i++)
					inds[i] = reader.ReadInt32();
				this.indices = new Indices<int>(inds);
			}

			Xen.Application.ApplicationProviderService app =
				(Xen.Application.ApplicationProviderService)reader.ContentManager.ServiceProvider.GetService(typeof(Xen.Application.ApplicationProviderService));

			if (app != null)
			{
				this.vertices.Warm(app.Application);
				this.indices.Warm(app.Application);
			}

			this.material = new MaterialData(reader);

			this.StaticBounds = new GeometryBounds(reader);

			count = (int)reader.ReadInt16();

			boneIndices = new int[count];
			boneLocalBounds = new GeometryBounds[count];

			for (int i = 0; i < count; i++)
				boneIndices[i] = (int)reader.ReadInt16();

			for (int i = 0; i < count; i++)
				boneLocalBounds[i] = new GeometryBounds(reader);

			count = (int)reader.ReadInt16();
			animationStaticBounds = new GeometryBounds[count];
			for (int i = 0; i < animationStaticBounds.Length; i++)
				animationStaticBounds[i] = new GeometryBounds(reader);
		}

		static internal void Write(VertexElement element, BinaryWriter writer)
		{
			writer.Write(element.Stream);
			writer.Write(element.Offset);
			writer.Write((byte)element.VertexElementFormat);
			writer.Write((byte)element.VertexElementMethod);
			writer.Write((byte)element.VertexElementUsage);
			writer.Write(element.UsageIndex);
		}

		static internal VertexElement ReadElement(ContentReader reader)
		{
			return new VertexElement(
				reader.ReadInt16(),
				reader.ReadInt16(),
				(VertexElementFormat)reader.ReadByte(),
				(VertexElementMethod)reader.ReadByte(),
				(VertexElementUsage)reader.ReadByte(),
				reader.ReadByte());
		}
	}

	public class AnimationData
	{
		private readonly string name;
		private readonly int[] boneIndices;
		private readonly float[] keyframeTimes;
		private readonly byte[][] keyframeChannels;
		private readonly float duration;
		internal readonly int index;
		private Stack<AnimationStreamControl> streamCache;

#if DEBUG

		public AnimationData(string name, int[] indices, KeyFrameData[] keyframes, float duration, float tollerance)
		{
			this.name = name;
			this.boneIndices = indices;
			this.duration = duration;

			this.keyframeTimes = new float[keyframes.Length];
			for (int i = 0; i < keyframes.Length; i++)
				keyframeTimes[i] = keyframes[i].Time;

			if (tollerance < 0)
				tollerance = 0;

			float keyframeTranslateRange = 0;

			for (int frame = 0; frame < keyframes.Length; frame++)
			{
				if (boneIndices.Length == 0)
					continue;
				Vector3 start = keyframes[frame].BoneTransforms[0].Translation;
				for (int i = 1; i < boneIndices.Length; i++)
				{
					float dist = (keyframes[frame].BoneTransforms[i].Translation - start).Length();
					keyframeTranslateRange = Math.Max(dist, keyframeTranslateRange);
				}
			}
			if (keyframeTranslateRange > 1)
				keyframeTranslateRange = (float)Math.Sqrt(keyframeTranslateRange);

			keyframeChannels = new byte[boneIndices.Length][];

			for (int i = 0; i < boneIndices.Length; i++)
			{
				MemoryStream ms = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(ms);
				CompressedTransformWriter compressor = new CompressedTransformWriter(Math.Min(0.5f,tollerance * 0.01f), keyframeTranslateRange * tollerance * 0.001f, Math.Min(0.5f,tollerance * 0.005f));

				for (int frame = 0; frame < keyframes.Length; frame++)
					compressor.Write(keyframes[frame].BoneTransforms[i], writer, false);
				compressor.EndWriting(writer);
				writer.Flush();

				keyframeChannels[i] = ms.ToArray();
			}
		}

#endif

		public int KeyFrameCount
		{
			get { return keyframeTimes.Length; }
		}
		public int AnimationIndex
		{
			get { return index; }
		}
		public int BoneCount
		{
			get { return boneIndices.Length; }
		}
		public ReadOnlyArrayCollection<int> BoneIndices
		{
			get { return new ReadOnlyArrayCollection<int>(boneIndices); }
		}
		public ReadOnlyArrayCollection<float> KeyFrameTime
		{
			get { return new ReadOnlyArrayCollection<float>(keyframeTimes); }
		}

		internal AnimationStreamControl GetStream()
		{
			lock (boneIndices)//private member, so saves a sync object
			{
				if (streamCache == null || streamCache.Count == 0)
					return new AnimationStreamControl(this);
				return streamCache.Pop();
			}
		}
		internal void CacheUnusedStream(AnimationStreamControl stream)
		{
			stream.Reset(true, false);
			lock (boneIndices)
			{
				if (streamCache == null)
					streamCache = new Stack<AnimationStreamControl>();
				streamCache.Push(stream);
			}
		}
		/// <summary>
		/// <para>As animations are played, their playback streams are cached to reduce garbage collection and allocation.</para>
		/// <para>Call this method to clear the animation stream cache</para>
		/// </summary>
		public void ClearAnimationStreamCache()
		{
			streamCache.Clear();
		}
		/// <summary>
		/// Read bone transform data using a <see cref="CompressedTransformReader"/>
		/// </summary>
		/// <param name="boneId"></param>
		/// <returns></returns>
		public Stream GetBoneCompressedTransformStream(int boneIndex)
		{
			return new MemoryStream(keyframeChannels[boneIndex], false);
		}

		internal byte[] GetBoneCompressedTransformData(int boneIndex)
		{
			return keyframeChannels[boneIndex];
		}

		public float Duration
		{
			get { return duration; }
		}

		public string Name { get { return name; } }

#if DEBUG

		internal void Write(BinaryWriter writer)
		{
			writer.Write(name ?? "");
			writer.Write(duration);
			writer.Write(boneIndices.Length);
			for (int i = 0; i < boneIndices.Length; i++)
				writer.Write((short)boneIndices[i]);

			writer.Write(keyframeTimes.Length);
			for (int i = 0; i < keyframeTimes.Length; i++)
				writer.Write(keyframeTimes[i]);

			for (int i = 0; i < this.keyframeChannels.Length; i++)
			{
				writer.Write(this.keyframeChannels[i].Length);
				writer.Write(this.keyframeChannels[i]);
			}
		}

#endif

		internal AnimationData(ContentReader reader, int index)
		{
			this.index = index;

			name = string.Intern(reader.ReadString());
			duration = reader.ReadSingle();

			int count = reader.ReadInt32();

			this.keyframeChannels = new byte[count][];
			this.boneIndices = new int[count];

			for (int i = 0; i < count; i++)
			{
				this.boneIndices[i] = (int)reader.ReadInt16();
			}

			count = reader.ReadInt32();
			this.keyframeTimes = new float[count];

			for (int i = 0; i < count; i++)
				this.keyframeTimes[i] = reader.ReadSingle();

			for (int i = 0; i < this.keyframeChannels.Length; i++)
			{
				count = reader.ReadInt32();
				keyframeChannels[i] = reader.ReadBytes(count);
			}
		}

	}

#if DEBUG

	/// <summary>
	/// DEBUG ONLY
	/// </summary>
	public class KeyFrameData
	{
		readonly float time;
		readonly Transform[] transforms;
		
		public KeyFrameData(float time, Transform[] transforms)
		{
			this.time = time;
			this.transforms = transforms;
		}
		public KeyFrameData(float time, Matrix[] transforms)
		{
			this.time = time;
			this.transforms = new Transform[transforms.Length];
			for (int i = 0; i < transforms.Length; i++)
				this.transforms[i] = new Transform(ref transforms[i]);
		}


		public ReadOnlyArrayCollection<Transform> BoneTransforms
		{
			get { return new ReadOnlyArrayCollection<Transform>(transforms); }
		}

		public float Time { get { return time; } }
	}

#endif

	public class SkeletonData
	{
		internal readonly Transform[] boneLocalTransforms, boneWorldTransforms;
		private readonly BoneData[] boneData;
		private readonly int[] hierarchy;

#if DEBUG
		internal readonly Matrix[] boneLocalMatrices;

		public SkeletonData(Matrix[] boneTransforms, BoneData[] bones)
		{
			this.boneLocalMatrices = boneTransforms;
			this.boneLocalTransforms = new Transform[boneTransforms.Length];
			for (int i = 0; i < this.boneLocalTransforms.Length; i++)
				this.boneLocalTransforms[i] = new Transform(ref boneTransforms[i]);
			this.boneData = bones;

			hierarchy = new int[boneData.Length * 2];
			CreateHierarchy();

			boneWorldTransforms = BoneLocalTransform.ToArray();
			TransformHierarchy(boneWorldTransforms);
		}

#endif

		/// <summary>
		/// Transforms a hierarchy of local bone transforms into world space bone transforms
		/// </summary>
		/// <param name="transforms"></param>
		public void TransformHierarchy(Transform[] transforms)
		{
			for (int i = 0; i < hierarchy.Length; i+=2)
			{
				int parent = hierarchy[i];
				int index = hierarchy[i+1];

				if (parent != -1)
					Transform.Multiply(ref transforms[index], ref transforms[parent], out transforms[index]);
			}
		}
#if DEBUG
		/// <summary>
		/// Transforms a hierarchy of local bone transforms into world space bone transforms
		/// </summary>
		/// <param name="transforms"></param>
		public void TransformHierarchy(Matrix[] transforms)
		{
			for (int i = 0; i < hierarchy.Length; i += 2)
			{
				int parent = hierarchy[i];
				int index = hierarchy[i + 1];

				if (parent != -1)
					Matrix.Multiply(ref transforms[index], ref transforms[parent], out transforms[index]);
			}
		}
#endif

		/// <summary>
		/// Applies the inverse of the <see cref="TransformHierarchy"/> method. (This operation is considerably slower and should not be performed at runtime)
		/// </summary>
		/// <param name="transforms"></param>
		public void TransformHierarchyInverse(Transform[] transforms)
		{
			for (int i = hierarchy.Length - 2; i >= 0; i -= 2)
			{
				int parent = hierarchy[i];
				int index = hierarchy[i + 1];

				if (parent != -1)
				{
					Matrix matrix;
					transforms[parent].GetMatrix(out matrix);
					Matrix.Invert(ref matrix, out matrix);
					Transform transform = new Transform(ref matrix);

					Transform.Multiply(ref transforms[index], ref transform, out transforms[index]);
				}
			}
		}
#if DEBUG
		/// <summary>
		/// Applies the inverse of the <see cref="TransformHierarchy"/> method. (This operation is considerably slower and should not be performed at runtime)
		/// </summary>
		/// <param name="transforms"></param>
		public void TransformHierarchyInverse(Matrix[] transforms)
		{
			for (int i = hierarchy.Length - 2; i >= 0; i -= 2)
			{
				int parent = hierarchy[i];
				int index = hierarchy[i + 1];

				if (parent != -1)
				{
					Matrix matrix;
					Matrix.Invert(ref transforms[parent], out matrix);
					Matrix.Multiply(ref transforms[index], ref matrix, out transforms[index]);
				}
			}
		}
#endif

		private void CreateHierarchy()
		{
			for (int i = 0; i < hierarchy.Length; i++)
				hierarchy[i] = -1;

			int index = 0;
			FillChildren(ref index,0,-1);
		}

		private void FillChildren(ref int start, int index, int parent)
		{
			hierarchy[start * 2 + 0] = parent;
			hierarchy[start * 2 + 1] = index;
			start++;

			for (int i = 0; i < boneData[index].Children.Length; i++)
				FillChildren(ref start, boneData[index].Children[i], index);
		}

		public int BoneCount
		{
			get { return boneData.Length; }
		}
		public ReadOnlyArrayCollection<Transform> BoneLocalTransform
		{
			get { return new ReadOnlyArrayCollection<Transform>(boneLocalTransforms); }
		}
		public ReadOnlyArrayCollection<Transform> BoneWorldTransforms
		{
			get { return new ReadOnlyArrayCollection<Transform>(boneWorldTransforms); }
		}
#if DEBUG
		/// <summary>
		/// This value will be null at runtime, and is only used at content build time
		/// </summary>
		public ReadOnlyArrayCollection<Matrix> BoneLocalMatrices
		{
			get { return new ReadOnlyArrayCollection<Matrix>(boneLocalMatrices); }
		}
#endif
		public ReadOnlyArrayCollection<BoneData> BoneData
		{
			get { return new ReadOnlyArrayCollection<BoneData>(boneData); }
		}

		internal Vector4[] GetBoneHierarchyAsMatrix4x3(Transform[] source, Vector4[] transforms)
		{
			if (source == null)
				throw new ArgumentNullException();

			for (int i = 0, j = 0; i < source.Length; i++)
			{
				float xx = source[i].Rotation.X * source[i].Rotation.X;
				float yy = source[i].Rotation.Y * source[i].Rotation.Y;
				float zz = source[i].Rotation.Z * source[i].Rotation.Z;
				float xy = source[i].Rotation.X * source[i].Rotation.Y;
				float zw = source[i].Rotation.Z * source[i].Rotation.W;
				float zx = source[i].Rotation.Z * source[i].Rotation.X;
				float yw = source[i].Rotation.Y * source[i].Rotation.W;
				float yz = source[i].Rotation.Y * source[i].Rotation.Z;
				float xw = source[i].Rotation.X * source[i].Rotation.W;

				if (source[i].Scale == 1)
				{
					transforms[j].X = 1f - (2f * (yy + zz));//x
					transforms[j].Y = 2f * (xy - zw);//y
					transforms[j].Z = 2f * (zx + yw);//z
					transforms[j].W = source[i].Translation.X;
					j++;
					transforms[j].X = 2f * (xy + zw);//x
					transforms[j].Y = 1f - (2f * (zz + xx));//y
					transforms[j].Z = 2f * (yz - xw);//z
					transforms[j].W = source[i].Translation.Y;
					j++;
					transforms[j].X = 2f * (zx - yw);//x
					transforms[j].Y = 2f * (yz + xw);//y
					transforms[j].Z = 1f - (2f * (yy + xx));//z
					transforms[j].W = source[i].Translation.Z;
					j++;
				}
				else
				{
					transforms[j].X = (1f - (2f * (yy + zz))) * source[i].Scale;//x
					transforms[j].Y = (2f * (xy - zw)) * source[i].Scale;//y
					transforms[j].Z = (2f * (zx + yw)) * source[i].Scale;//z
					transforms[j].W = source[i].Translation.X;
					j++;
					transforms[j].X = (2f * (xy + zw)) * source[i].Scale;//x
					transforms[j].Y = (1f - (2f * (zz + xx))) * source[i].Scale;//y
					transforms[j].Z = (2f * (yz - xw)) * source[i].Scale;//z
					transforms[j].W = source[i].Translation.Y;
					j++;
					transforms[j].X = (2f * (zx - yw)) * source[i].Scale;//x
					transforms[j].Y = (2f * (yz + xw)) * source[i].Scale;//y
					transforms[j].Z = (1f - (2f * (yy + xx))) * source[i].Scale;//z
					transforms[j].W = source[i].Translation.Z;
					j++;
				}
			}
			return transforms;
		}

#if DEBUG
		internal void Write(BinaryWriter writer)
		{
			writer.Write(boneLocalTransforms.Length);
			for (int i = 0; i < boneLocalTransforms.Length; i++)
			{
				boneLocalTransforms[i].Write(writer);
				boneData[i].Write(writer);
			}
		}
#endif

		internal SkeletonData(ContentReader reader)
		{
			int count = reader.ReadInt32();

			this.boneData = new BoneData[count];
			this.boneLocalTransforms = new Transform[count];

			for (int i = 0; i < count; i++)
			{
				this.boneLocalTransforms[i] = new Transform(reader);
				this.boneData[i] = new BoneData(reader);
			}

			hierarchy = new int[boneData.Length * 2];
			CreateHierarchy();

			boneWorldTransforms = BoneLocalTransform.ToArray();
			TransformHierarchy(boneWorldTransforms);
		}
	}

	public struct BoneData
	{
		private readonly string name;
		private readonly int boneIndex;
		private readonly int[] children;
		private readonly int parent;

#if DEBUG

		public BoneData(string name, int boneIndex, int parent, int[] children)
		{
			this.name = name;
			this.boneIndex = boneIndex;
			this.children = children;
			this.parent = parent;
		}

#endif

		public string Name
		{
			get { return name; }
		}
		public int Index
		{
			get { return boneIndex; }
		}
		public int Parent
		{
			get { return parent; }
		}

		public ReadOnlyArrayCollection<int> Children
		{
			get { return new ReadOnlyArrayCollection<int>(children); }
		}

#if DEBUG

		internal void Write(BinaryWriter writer)
		{
			if (name == null ||
				children == null)
			{
				//null bone.. skip
				writer.Write(false);
				return;
			}
			writer.Write(true);
			writer.Write(name);
			writer.Write((short)boneIndex);
			writer.Write((short)parent);
			writer.Write((short)children.Length);
			for (int i = 0; i < children.Length; i++)
				writer.Write((short)children[i]);
		}

#endif

		internal BoneData(ContentReader reader)
		{
			if (reader.ReadBoolean() == false)
			{
				this.name = null;
				this.boneIndex = -1;
				this.children = null;
				this.parent = -1;
				return;
			}

			this.name = string.Intern(reader.ReadString());
			this.boneIndex = (int)reader.ReadInt16();
			this.parent = (int)reader.ReadInt16();
			int count = (int)reader.ReadInt16();

			this.children = new int[count];
			for (int i = 0; i < count; i++)
				this.children[i] = (int)reader.ReadInt16();
		}
	}
}
