using System;
using System.Collections.Generic;
using System.Text;
using Xen.Graphics;
using Xen.Graphics.ShaderSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xen.Graphics.State;

namespace Xen.Ex.Material
{
	sealed class ShaderBoundBones
	{
		public object boneSource;
		public int changeId = -1;
	}
	sealed class ShaderMerge
	{
		public const int MaxBones = 71;

		static int GetSourceId(int vs, int ps, bool tex, bool vertexColours, bool normalMapped, bool pixelSpecular, bool vertexBlending)
		{
			return (vs << 8) | (ps << 16) | ((tex | normalMapped) ? 1 : 0) | (vertexColours ? 2 : 0) | (normalMapped ? 4 : 0) | (pixelSpecular ? 8 : 0) | (vertexBlending ? 16 : 0);
		}
		readonly Dictionary<int, IShader> shaderCombinations = new Dictionary<int, IShader>();
		readonly Dictionary<int, ShaderBoundBones> shaderBoundBones = new Dictionary<int, ShaderBoundBones>();
		readonly Dictionary<string, IShader> shaderInstances = new Dictionary<string, IShader>();

		public static int MaxVertexLights
		{
			get { return vsSource.Length - 1; }
		}
		public static int MaxPixelLights
		{
			get { return psSource.Length - 1; }
		}
		public static int MaxPixelSpecularLights
		{
			get { return psSourceSpec.Length - 1; }
		}

		/////////////////////////////////////////////////////////////
		//type name of vertex shaders
		//keep the type name, so the class need not be loaded until it's actually needed

		static readonly string[] vsSource = new string[]
			{
				"Xen.Ex.Material.vs0",
				"Xen.Ex.Material.vs1",
				null,
				"Xen.Ex.Material.vs3",
				null,
				null,
				"Xen.Ex.Material.vs6"
			};
		static readonly string[] vsSourceTex = new string[]
			{
				"Xen.Ex.Material.vs0t",
				"Xen.Ex.Material.vs1t",
				null,
				"Xen.Ex.Material.vs3t",
				null,
				null,
				"Xen.Ex.Material.vs6t"
			};
		static readonly string[] vsSourceColours = new string[]
			{
				"Xen.Ex.Material.vs0c",
				"Xen.Ex.Material.vs1c",
				null,
				"Xen.Ex.Material.vs3c",
				null,
				null,
				"Xen.Ex.Material.vs6c"
			};
		static readonly string[] vsSourceTexColours = new string[]
			{
				"Xen.Ex.Material.vs0tc",
				"Xen.Ex.Material.vs1tc",
				null,
				"Xen.Ex.Material.vs3tc",
				null,
				null,
				"Xen.Ex.Material.vs6tc"
			};
		static readonly string[] vsSourceNormalsColours = new string[]
			{
				"Xen.Ex.Material.vs0nc",
				"Xen.Ex.Material.vs1nc",
				null,
				"Xen.Ex.Material.vs3nc",
				null,
				null,
				"Xen.Ex.Material.vs6nc"
			};
		static readonly string[] vsSourceNormals = new string[]
			{
				"Xen.Ex.Material.vs0n",
				"Xen.Ex.Material.vs1n",
				null,
				"Xen.Ex.Material.vs3n",
				null,
				null,
				"Xen.Ex.Material.vs6n"
			};

		/////////////////////////////////////////////////////////////
		//blend

		static readonly string[] vsSourceBlend = new string[]
			{
				"Xen.Ex.Material.vs0b",
				"Xen.Ex.Material.vs1b",
				null,
				"Xen.Ex.Material.vs3b",
				null,
				null,
				"Xen.Ex.Material.vs6b"
			};
		static readonly string[] vsSourceTexBlend = new string[]
			{
				"Xen.Ex.Material.vs0tb",
				"Xen.Ex.Material.vs1tb",
				null,
				"Xen.Ex.Material.vs3tb",
				null,
				null,
				"Xen.Ex.Material.vs6tb"
			};
		static readonly string[] vsSourceColoursBlend = new string[]
			{
				"Xen.Ex.Material.vs0cb",
				"Xen.Ex.Material.vs1cb",
				null,
				"Xen.Ex.Material.vs3cb",
				null,
				null,
				"Xen.Ex.Material.vs6cb"
			};
		static readonly string[] vsSourceTexColoursBlend = new string[]
			{
				"Xen.Ex.Material.vs0tcb",
				"Xen.Ex.Material.vs1tcb",
				null,
				"Xen.Ex.Material.vs3tcb",
				null,
				null,
				"Xen.Ex.Material.vs6tcb"
			};
		static readonly string[] vsSourceNormalsColoursBlend = new string[]
			{
				"Xen.Ex.Material.vs0ncb",
				"Xen.Ex.Material.vs1ncb",
				null,
				"Xen.Ex.Material.vs3ncb",
				null,
				null,
				"Xen.Ex.Material.vs6ncb"
			};
		static readonly string[] vsSourceNormalsBlend = new string[]
			{
				"Xen.Ex.Material.vs0nb",
				"Xen.Ex.Material.vs1nb",
				null,
				"Xen.Ex.Material.vs3nb",
				null,
				null,
				"Xen.Ex.Material.vs6nb"
			};

		/////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////////////
		//pixel shaders

		static readonly string[] psSource = new string[]
			{
				"Xen.Ex.Material.ps0",
				"Xen.Ex.Material.ps1",
				"Xen.Ex.Material.ps2",
				"Xen.Ex.Material.ps3",
				"Xen.Ex.Material.ps4",
			};
		static readonly string[] psSourceSpec = new string[]
			{
				"Xen.Ex.Material.ps0",
				"Xen.Ex.Material.ps1s",
				"Xen.Ex.Material.ps2s"
			};
		static readonly string[] psSourceTex = new string[]
			{
				"Xen.Ex.Material.ps0t",
				"Xen.Ex.Material.ps1t",
				"Xen.Ex.Material.ps2t",
				"Xen.Ex.Material.ps3t",
				"Xen.Ex.Material.ps4t"
			};
		static readonly string[] psSourceTexSpec = new string[]
			{
				"Xen.Ex.Material.ps0t",
				"Xen.Ex.Material.ps1ts",
				"Xen.Ex.Material.ps2ts"
			};
		static readonly string[] psSourceNormals = new string[]
			{
				"Xen.Ex.Material.ps0t",
				"Xen.Ex.Material.ps1tn",
				"Xen.Ex.Material.ps2tn",
				"Xen.Ex.Material.ps3tn",
				"Xen.Ex.Material.ps4tn"
			};
		static readonly string[] psSourceNormalsSpec = new string[]
			{
				"Xen.Ex.Material.ps0t",
				"Xen.Ex.Material.ps1tns",
				"Xen.Ex.Material.ps2tns"
			};

		public IShader GetShader(DrawState state, int vs, int ps, bool tex, bool vertexColours, bool normalMapped, bool pixelSpecular, bool vertexBlending, out ShaderBoundBones boundBones)
		{
			boundBones = null;

			tex |= normalMapped;
			int index = GetSourceId(vs, ps, tex, vertexColours, normalMapped, pixelSpecular, vertexBlending);
			IShader shader;
			lock (shaderCombinations)
			{
				if (!shaderCombinations.TryGetValue(index, out shader))
				{
					string[] vss, pss;
					if (vertexBlending)
					{
						if (tex)
						{
							if (normalMapped)
							{
								if (vertexColours)
									vss = vsSourceNormalsColoursBlend;
								else
									vss = vsSourceNormalsBlend;

								if (pixelSpecular)
									pss = psSourceNormalsSpec;
								else
									pss = psSourceNormals;
							}
							else
							{
								if (vertexColours)
									vss = vsSourceTexColoursBlend;
								else
									vss = vsSourceTexBlend;

								if (pixelSpecular)
									pss = psSourceTexSpec;
								else
									pss = psSourceTex;
							}
						}
						else
						{
							if (vertexColours)
								vss = vsSourceColoursBlend;
							else
								vss = vsSourceBlend;

							if (pixelSpecular)
								pss = psSourceSpec;
							else
								pss = psSource;
						}
					}
					else
					{
						if (tex)
						{
							if (normalMapped)
							{
								if (vertexColours)
									vss = vsSourceNormalsColours;
								else
									vss = vsSourceNormals;

								if (pixelSpecular)
									pss = psSourceNormalsSpec;
								else
									pss = psSourceNormals;
							}
							else
							{
								if (vertexColours)
									vss = vsSourceTexColours;
								else
									vss = vsSourceTex;

								if (pixelSpecular)
									pss = psSourceTexSpec;
								else
									pss = psSourceTex;
							}
						}
						else
						{
							if (vertexColours)
								vss = vsSourceColours;
							else
								vss = vsSource;

							if (pixelSpecular)
								pss = psSourceSpec;
							else
								pss = psSource;
						}
					}

					if (vs >= vss.Length)
						throw new InvalidOperationException(string.Format("MaterialShader supports a maximum of {0} vertex lights", vss.Length - 1));
					if (ps >= pss.Length)
					{
						if (pixelSpecular)
							throw new InvalidOperationException(string.Format("MaterialShader supports a maximum of {0} per-pixel lights when using per-pixel specular reflections", pss.Length - 1));
						else
							throw new InvalidOperationException(string.Format("MaterialShader supports a maximum of {0} per-pixel lights", pss.Length - 1));
					}

					//the instances aren't directly stored, this way they only get JITd when needed (because there are so many source instances)
					IShader vshader = GetInstance(state, vss[vs]);
					if (vshader == null)
						vshader = GetInstance(state, vss[vs + 1]);
					if (vshader == null)
						vshader = GetInstance(state, vss[vs + 2]);
					IShader pshader = GetInstance(state,pss[ps]);

					if (vs == 0 && ps == 0 && !vertexBlending)
					{
						if (tex)
						{
							if (vertexColours)
								shader = state.GetShader<Xen.Ex.Material.vs0ps0tc>();
							else
								shader = state.GetShader<Xen.Ex.Material.vs0ps0t>();
						}
						else
						{
							if (vertexColours)
								shader = state.GetShader<Xen.Ex.Material.vs0ps0c>();
							else
								shader = state.GetShader<Xen.Ex.Material.vs0ps0>();
						}
					}
					else
						shader = BaseShader.Merge((BaseShader)vshader, (BaseShader)pshader);

					if (vertexBlending)
					{
						boundBones = new ShaderBoundBones();
						shaderBoundBones.Add(index, boundBones);
					}

					shaderCombinations.Add(index, shader);
				}
				else
				{
					if (vertexBlending)
						boundBones = shaderBoundBones[index];
				}
			}
			return shader;
		}


		Type[] typeArray = new Type[1];
		Type[] nullArray = new Type[0];

		private IShader GetInstance(DrawState state, string p)
		{
			if (p == null)
				return null;
			IShader shader;

			if (!shaderInstances.TryGetValue(p, out shader))
			{
				lock (typeArray)
				{
					System.Reflection.Assembly asm = typeof(ShaderMerge).Assembly;

					Type type = asm.GetType(p);

					typeArray[0] = type;
					System.Reflection.MethodInfo method = typeof(DrawState).GetMethod("GetShader", nullArray);
					method = method.MakeGenericMethod(typeArray);
					shader = (IShader)method.Invoke(state, new object[0]);

					shaderInstances.Add(p, shader);
				}
			}
			return shader;
		}
	}
	class ShaderLight : IMaterialLight
	{
		internal Vector4 position;
		internal Vector4 specular;
		internal Vector4 colour;
		internal Vector4 attenuation;
		private bool perPixel, enabled = true;

		public bool PerPixelLighting
		{
			get { return perPixel; }
			internal set { perPixel = value; }
		}
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		public Vector3 Colour
		{
			get { return new Vector3(colour.X, colour.Y, colour.Z); }
			set
			{
				colour.X = value.X; colour.Y = value.Y; colour.Z = value.Z;
			}
		}

		public Vector3 SpecularColour
		{
			get { return new Vector3(specular.X, specular.Y, specular.Z); }
			set
			{
				specular.X = value.X; specular.Y = value.Y; specular.Z = value.Z;
			}
		}

		public float SpecularPowerScaler
		{
			get { return specular.W; }
			set { specular.W = value; }
		}
	}

	/// <summary>
	/// Interface to a light stored in a <see cref="MaterialLightCollection"/>
	/// </summary>
	public interface IMaterialLight
	{
		/// <summary>
		/// Gets/Sets the diffuse colour of the light (Vector3)
		/// </summary>
		Vector3 Colour
		{
			get;
			set;
		}
		/// <summary>
		/// Gets/Sets the specular colour of the light (Vector3)
		/// </summary>
		Vector3 SpecularColour
		{
			get;
			set;
		}
		/// <summary>
		/// Gets/Sets a scale factor for the material specular power (default is 1, no change)
		/// </summary>
		float SpecularPowerScaler
		{
			get;
			set;
		}
		/// <summary>
		/// Gets if this light is rendered per-pixel or per-vertex
		/// </summary>
		bool PerPixelLighting
		{
			get;
		}
		/// <summary>
		/// Gets/Sets if this light is enabled
		/// </summary>
		bool Enabled
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Interface to a point light stored in a <see cref="MaterialLightCollection"/>
	/// </summary>
	public interface IMaterialPointLight : IMaterialLight
	{
		/// <summary>
		/// Gets/Sets the position of the point light
		/// </summary>
		Vector3 Position { get; set; }
		/// <summary>
		/// Gets/Sets the light anttenuation (falloff) constant term
		/// </summary>
		/// <remarks>See <see cref="QuadraticAttenuation"/> for details</remarks>
		float ConstantAttenuation { get; set; }
		/// <summary>
		/// Gets/Sets the light anttenuation (falloff) linear term
		/// </summary>
		/// <remarks>See <see cref="QuadraticAttenuation"/> for details</remarks>
		float LinearAttenuation { get; set; }
		/// <summary>
		/// Gets/Sets the light anttenuation (falloff) squared linear term
		/// </summary>
		/// <remarks>
		/// <para>The attenuation of a light is calculated with the following equation:</para>
		/// <code>
		///		float len = ...;
		/// 
		///		float brightness = 1.0f / (CA + LA*len + QA*(len^2));
		/// </code>
		/// <para>Where <i>len</i> is the distance from the light source,
		/// <br/><i>CA</i> is the <see cref="ConstantAttenuation"/> term,
		/// <br/><i>LA</i> is the <see cref="LinearAttenuation"/> term,
		/// <br/><i>QA</i> is the <see cref="QuadraticAttenuation"/> term.</para>
		/// <para>
		/// For constant light with no falloff (brightness always == 1), set the constant term to 1, and linear/quadratic to 0.
		/// </para>
		/// <para>
		/// For lighting that starts at 10, and falls of in a inverse linear pattern with a half distance of 3 (ie, at 0 the brightness is 10.0 and at 3, the brightness is 5.0), set the contant term to 0.1, the linear term to (2-0.1) / 3. Putting these numbers into the equation should produce the correct results.
		/// </para>
		/// </remarks>
		float QuadraticAttenuation { get; set; }
	}
	/// <summary>
	/// Interface to a directional light stored in a <see cref="MaterialLightCollection"/>
	/// </summary>
	public interface IMaterialDirectionalLight : IMaterialLight
	{
		/// <summary>
		/// Gets/Sets the direction of the light
		/// </summary>
		Vector3 Direction { get; set; }
	}

	sealed class MaterialShaderPointLight : ShaderLight, IMaterialPointLight
	{
		public MaterialShaderPointLight(Vector3 position, Vector3 colour, float halfFalloffDistance)
		{
			this.Position = position;
			this.Colour = colour;
			this.SpecularColour = colour;
			this.SpecularPowerScaler = 1;
			this.position.W = 1;

			this.attenuation = new Vector4(1, halfFalloffDistance == 0 ? 0 : (1.0f / halfFalloffDistance), 0, 0);
		}


		public float ConstantAttenuation
		{
			get { return attenuation.X; }
			set { attenuation.X = value; }
		}

		public float LinearAttenuation
		{
			get { return attenuation.Y; }
			set { attenuation.Y = value; }
		}

		public float QuadraticAttenuation
		{
			get { return attenuation.Z; }
			set { attenuation.Z = value; }
		}

		public Vector3 Position
		{
			get { return new Vector3(position.X, position.Y, position.Z); }
			set
			{
				position.X = value.X; position.Y = value.Y; position.Z = value.Z;
			}
		}

	}
	sealed class MaterialShaderDirectionalLight : ShaderLight, IMaterialDirectionalLight
	{
		public MaterialShaderDirectionalLight(Vector3 direction, Vector3 colour)
		{
			this.Direction = direction;
			this.Colour = colour;
			this.SpecularColour = colour;
			this.SpecularPowerScaler = 1;
			this.position.W = 0;
			this.attenuation = new Vector4(1, 0, 0, 0);
		}
		public Vector3 Direction
		{
			get { return new Vector3(position.X, position.Y, position.Z); }
			set
			{
				position.X = value.X; position.Y = value.Y; position.Z = value.Z;
			}
		}

	}

	sealed class WhiteTexture : IContentOwner
	{
		static WhiteTexture instance;
		Texture2D texture;

		WhiteTexture(DrawState state)
		{
			state.Application.Content.Add(this);
		}

		public static Texture2D GetTexture(DrawState state)
		{
			if (instance == null)
				instance = new WhiteTexture(state);
			return instance.texture;
		}

		void IContentOwner.LoadContent(ContentRegister content, DrawState state, Microsoft.Xna.Framework.Content.ContentManager manager)
		{
			GraphicsDevice device = state.BeginGetGraphicsDevice(StateFlag.None);

			Color[] colours = new Color[]
			{
				Color.White,
				Color.White,
				Color.White,
				Color.White
			};

			texture = new Texture2D(device, 2, 2, 0, TextureUsage.None, SurfaceFormat.Color);
			texture.SetData(colours);

			state.EndGetGraphicsDevice();
		}

		void IContentOwner.UnloadContent(ContentRegister content, DrawState state)
		{
			texture.Dispose();
			texture = null;
		}
	}

	/// <summary>
	/// Stores a collection of lights used by one or more <see cref="MaterialShader"/> instances
	/// </summary>
	public sealed class MaterialLightCollection
	{
		internal readonly List<ShaderLight> vlights = new List<ShaderLight>(), plights = new List<ShaderLight>();
		internal Vector3 ambient = new Vector3(0, 0, 0);
		private bool enabled;

		/// <summary>
		/// Gets/Sets if lighting is enabled
		/// </summary>
		public bool LightingEnabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		/// <summary>
		/// Construct a shader lights collection
		/// </summary>
		public MaterialLightCollection()
		{
			this.enabled = true;
		}

		/// <summary>
		/// Construct a shader lights collection
		/// </summary>
		/// <param name="enabled"></param>
		public MaterialLightCollection(bool enabled)
		{
			this.enabled = enabled;
		}

		/// <summary>
		/// Gets the number of per-vertex lights
		/// </summary>
		public int VertexLightCount
		{
			get { return vlights.Count; }
		}
		/// <summary>
		/// Gets a per-vertex light by index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IMaterialLight GetVertexLight(int index)
		{
			return vlights[index];
		}

		/// <summary>
		/// Gets the number of per-pixel lights
		/// </summary>
		public int PixelLightCount
		{
			get { return plights.Count; }
		}
		/// <summary>
		/// Gets a per-pixel light by index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IMaterialLight GetPixelLight(int index)
		{
			return plights[index];
		}
		/// <summary>
		/// Gets/Sets the ambient light colour (Vector3)
		/// </summary>
		public Vector3 AmbientLightColour
		{
			get { return ambient; }
			set
			{
				ambient = value;
			}
		}
		/// <summary>
		/// Adds a point light source to the shader
		/// </summary>
		/// <param name="perPixelLighting">'True' to enable this light to be calculared per-pixel instead of per-vertex. Per-pixel lights are more expensive, and fewer can be used.</param>
		/// <param name="position">Position of the lightsource</param>
		/// <param name="halfFalloffDistance">The half falloff distance (the distance it takes the light to lose half its intensity)</param>
		/// <param name="colour">Colour of the lightsource</param>
		/// <returns></returns>
		public IMaterialPointLight AddPointLight(bool perPixelLighting, Vector3 position, float halfFalloffDistance, Vector3 colour)
		{
			IMaterialPointLight light = new MaterialShaderPointLight(position, colour, halfFalloffDistance);
			((ShaderLight)light).PerPixelLighting = perPixelLighting;
			(perPixelLighting ? this.plights : this.vlights).Add((ShaderLight)light);
			return light;
		}
		/// <summary>
		/// Adds a point light source to the shader
		/// </summary>
		/// <param name="perPixelLighting">'True' to enable this light to be calculared per-pixel instead of per-vertex. Per-pixel lights are more expensive, and fewer can be used.</param>
		/// <param name="position">Position of the lightsource</param>
		/// <param name="halfFalloffDistance">The half falloff distance (the distance it takes the light to lose half its intensity)</param>
		/// <param name="colour">Colour of the lightsource</param>
		/// <returns></returns>
		public IMaterialPointLight AddPointLight(bool perPixelLighting, Vector3 position, float halfFalloffDistance, Color colour)
		{
			IMaterialPointLight light = new MaterialShaderPointLight(position, colour.ToVector3(), halfFalloffDistance);
			((ShaderLight)light).PerPixelLighting = perPixelLighting;
			(perPixelLighting ? this.plights : this.vlights).Add((ShaderLight)light);
			return light;
		}

		/// <summary>
		/// Adds a directional (infinite) light source to the shader
		/// </summary>
		/// <param name="perPixelLighting">'True' to enable this light to be calculared per-pixel instead of per-vertex. Per-pixel lights are more expensive, and fewer can be used.</param>
		/// <param name="direction">Direction of the lightsource</param>
		/// <param name="colour">Colour of the lightsource</param>
		/// <returns></returns>
		public IMaterialDirectionalLight AddDirectionalLight(bool perPixelLighting, Vector3 direction, Vector3 colour)
		{
			IMaterialDirectionalLight light = new MaterialShaderDirectionalLight(direction, colour);
			((ShaderLight)light).PerPixelLighting = perPixelLighting;
			(perPixelLighting ? this.plights : this.vlights).Add((ShaderLight)light);
			return light;
		}
		/// <summary>
		/// Adds a directional (infinite) light source to the shader
		/// </summary>
		/// <param name="perPixelLighting">'True' to enable this light to be calculared per-pixel instead of per-vertex. Per-pixel lights are more expensive, and fewer can be used.</param>
		/// <param name="direction">Direction of the lightsource</param>
		/// <param name="colour">Colour of the lightsource</param>
		/// <returns></returns>
		public IMaterialDirectionalLight AddDirectionalLight(bool perPixelLighting, Vector3 direction, Color colour)
		{
			IMaterialDirectionalLight light = new MaterialShaderDirectionalLight(direction, colour.ToVector3());
			((ShaderLight)light).PerPixelLighting = perPixelLighting;
			(perPixelLighting ? this.plights : this.vlights).Add((ShaderLight)light);
			return light;
		}


		/// <summary>
		/// Adds a point light source to the shader
		/// </summary>
		/// <param name="perPixelLighting">'True' to enable this light to be calculared per-pixel instead of per-vertex. Per-pixel lights are more expensive, and fewer can be used.</param>
		/// <param name="position">Position of the lightsource</param>
		/// <param name="halfFalloffDistance">The half falloff distance (the distance it takes the light to lose half its intensity)</param>
		/// <param name="colour">Colour of the lightsource</param>
		/// <param name="specularColour">Specular colour of the lightsource (Specular is direct light reflection)</param>
		/// <returns></returns>
		public IMaterialPointLight AddPointLight(bool perPixelLighting, Vector3 position, float halfFalloffDistance, Vector3 colour, Vector3 specularColour)
		{
			IMaterialPointLight light = new MaterialShaderPointLight(position, colour, halfFalloffDistance);
			((ShaderLight)light).PerPixelLighting = perPixelLighting;
			(perPixelLighting ? this.plights : this.vlights).Add((ShaderLight)light);
			light.SpecularColour = specularColour;
			return light;
		}
		/// <summary>
		/// Adds a point light source to the shader
		/// </summary>
		/// <param name="perPixelLighting">'True' to enable this light to be calculared per-pixel instead of per-vertex. Per-pixel lights are more expensive, and fewer can be used.</param>
		/// <param name="position">Position of the lightsource</param>
		/// <param name="halfFalloffDistance">The half falloff distance (the distance it takes the light to lose half its intensity)</param>
		/// <param name="colour">Colour of the lightsource</param>
		/// <param name="specularColour">Specular colour of the lightsource (Specular is direct light reflection)</param>
		/// <returns></returns>
		public IMaterialPointLight AddPointLight(bool perPixelLighting, Vector3 position, float halfFalloffDistance, Color colour, Color specularColour)
		{
			IMaterialPointLight light = new MaterialShaderPointLight(position, colour.ToVector3(), halfFalloffDistance);
			((ShaderLight)light).PerPixelLighting = perPixelLighting;
			(perPixelLighting ? this.plights : this.vlights).Add((ShaderLight)light);
			light.SpecularColour = specularColour.ToVector3();
			return light;
		}

		/// <summary>
		/// Adds a directional (infinite) light source to the shader
		/// </summary>
		/// <param name="perPixelLighting">'True' to enable this light to be calculared per-pixel instead of per-vertex. Per-pixel lights are more expensive, and fewer can be used.</param>
		/// <param name="direction">Direction of the lightsource</param>
		/// <param name="colour">Colour of the lightsource</param>
		/// <param name="specularColour">Specular colour of the lightsource (Specular is direct light reflection)</param>
		/// <returns></returns>
		public IMaterialDirectionalLight AddDirectionalLight(bool perPixelLighting, Vector3 direction, Vector3 colour, Vector3 specularColour)
		{
			IMaterialDirectionalLight light = new MaterialShaderDirectionalLight(direction, colour);
			((ShaderLight)light).PerPixelLighting = perPixelLighting;
			(perPixelLighting ? this.plights : this.vlights).Add((ShaderLight)light);
			light.SpecularColour = specularColour;
			return light;
		}
		/// <summary>
		/// Adds a directional (infinite) light source to the shader
		/// </summary>
		/// <param name="direction">Direction of the lightsource</param>
		/// <param name="colour">Colour of the lightsource</param>
		/// <param name="specularColour">Specular colour of the lightsource (Specular is direct light reflection)</param>
		/// <param name="perPixelLighting">'True' to enable this light to be calculared per-pixel instead of per-vertex. Per-pixel lights are more expensive, and fewer can be used.</param>
		/// <returns></returns>
		public IMaterialDirectionalLight AddDirectionalLight(bool perPixelLighting, Vector3 direction, Color colour, Color specularColour)
		{
			IMaterialDirectionalLight light = new MaterialShaderDirectionalLight(direction, colour.ToVector3());
			((ShaderLight)light).PerPixelLighting = perPixelLighting;
			(perPixelLighting ? this.plights : this.vlights).Add((ShaderLight)light);
			light.SpecularColour = specularColour.ToVector3();
			return light;
		}

		/// <summary>
		/// Add an existing light to the shader
		/// </summary>
		/// <param name="light"></param>
		public void AddLight(IMaterialLight light)
		{
			(light.PerPixelLighting ? plights : vlights).Add((ShaderLight)light);
		}
		/// <summary>
		/// Remove a light from the shader
		/// </summary>
		/// <param name="light"></param>
		/// <returns></returns>
		public bool RemoveLight(IMaterialLight light)
		{
			if (light == null)
				return false;

			return (light.PerPixelLighting ? plights : vlights).Remove((ShaderLight)light);
		}
		/// <summary>
		/// Removes all lights from this shader
		/// </summary>
		public void RemoveAllLights()
		{
			this.vlights.Clear();
			this.plights.Clear();
		}
	}

	/// <summary>
	/// Stores a list of animation transforms, converting them to a format used by a <see cref="MaterialShader"/>
	/// </summary>
	public sealed class MaterialAnimationTransformHierarchy
	{
		private int changeIndex;
		private readonly Vector4[] matrixData;
		private bool enabled;
		private readonly Xen.Ex.Graphics.Content.SkeletonData skeleton;

		/// <summary>
		/// Construct the hierachy from a skeleton
		/// </summary>
		/// <param name="skeleton"></param>
		public MaterialAnimationTransformHierarchy(Xen.Ex.Graphics.Content.SkeletonData skeleton)
		{
			if (skeleton == null)
				throw new ArgumentNullException();
			if (skeleton.BoneCount > ShaderMerge.MaxBones)
				throw new InvalidOperationException(string.Format("The MaterialShader animation system supports a maximum of {0} bones", ShaderMerge.MaxBones));
			this.enabled = true;
			this.matrixData = new Vector4[skeleton.BoneCount*3];
			this.skeleton = skeleton;
		}
		/// <summary>
		/// Construct the hierachy from a skeleton
		/// </summary>
		/// <param name="skeleton"></param>
		/// <param name="hierarchy"></param>
		public MaterialAnimationTransformHierarchy(Xen.Ex.Graphics.Content.SkeletonData skeleton, Transform[] hierarchy) : this(skeleton)
		{
			UpdateTransformHierarchy(hierarchy);
		}

		/// <summary>
		/// Update the transform data
		/// </summary>
		/// <param name="hierarchy"></param>
		public void UpdateTransformHierarchy(Transform[] hierarchy)
		{
			if (hierarchy == null)
				throw new ArgumentNullException();
			if (hierarchy.Length != skeleton.BoneCount)
				throw new ArgumentException("hierarchy.Length");

			skeleton.GetBoneHierarchyAsMatrix4x3(hierarchy, matrixData);
			changeIndex++;
		}

		internal int ChangeIndex
		{
			get { return changeIndex; }
		}

		/// <summary>
		/// Gets/Sets if blending is enabled
		/// </summary>
		public bool BlendingEnabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		internal Vector4[] GetMatrixData()
		{
			return matrixData;
		}
	}



	/// <summary>
	/// <para>A shader that implements a simple lighting model that supporting a large number of point and directional lights.</para>
	/// <para>MaterialShader also allows for textures, normal maps and vertex colours to be used</para>
	/// </summary>
	public sealed class MaterialShader : IShader
	{
		sealed class AppMaterialID
		{
			public Application application;
			public ShaderMerge shaderMerge;
			public int v_lights_id, p_lights_id, ambient_id, CustomTexture_id, CustomTextureSampler_id, CustomNormalMap_id, CustomNormalMapSampler_id, blendMatrices_id;
		}

		private bool dirty = true;
		private IShader shader;
		private ShaderBoundBones shaderBoundBones;
		private AppMaterialID appId;
		private bool useVertexColour;
		private Texture2D texture, normalmap;
		private TextureSamplerState textureSampler = TextureSamplerState.BilinearFiltering, normalSampler = TextureSamplerState.BilinearFiltering;
		private bool perPixelSpecular;
		private float specularPower = 16;
		private Vector3 specularColour;
		private Vector3 diffuseColour = Vector3.One;
		private Vector3 emmissiveColour = Vector3.Zero;
		private MaterialLightCollection lightCollection;
		private short vsLightCount, psLightCount;
		private Vector4 emissive;
		private MaterialAnimationTransformHierarchy animationTransforms;

		/// <summary>
		/// Gets/Sets the animation transform object
		/// </summary>
		public MaterialAnimationTransformHierarchy AnimationTransforms
		{
			get { return animationTransforms; }
			set 
			{
				if (value != animationTransforms)
				{
					dirty |= (value == null) != (animationTransforms == null);
					animationTransforms = value;
				}
			}
		}


		/// <summary>
		/// Construct the shader with no lights (Lights collection will be null)
		/// </summary>
		public MaterialShader()
		{
		}

		/// <summary>
		/// Construct the shader with a set of lights
		/// </summary>
		/// <param name="lights"></param>
		public MaterialShader(MaterialLightCollection lights)
		{
			this.lightCollection = lights;
		}


		/// <summary>
		/// Gets/Sets the lights collection used by this shader. Setting to null or setting <see cref="MaterialLightCollection.LightingEnabled"/> to false will disable lighting
		/// </summary>
		public MaterialLightCollection Lights
		{
			get { return lightCollection; }
			set { lightCollection = value; }
		}

		/// <summary>
		/// Creates a MaterialShader that implements similar lighting characteristics to an XNA BasicEffect (The <see cref="Lights"/> collection will be populated if the basic effect has lighting enabled)
		/// </summary>
		/// <param name="effect"></param>
		/// <remarks><para>Not all features of the BasicEffect are implement by the created MaterialShader (eg Fog is not implemented)</para></remarks>
		/// <returns></returns>
		public static MaterialShader FromBasicEffect(BasicEffect effect)
		{
			if (effect == null)
				throw new ArgumentNullException();

			MaterialShader shader = new MaterialShader();
			shader.Alpha = effect.Alpha;
			shader.EmissiveColour = effect.EmissiveColor;
			shader.UseVertexColour = effect.VertexColorEnabled;
			shader.TextureMap = effect.TextureEnabled ? effect.Texture : null;
			shader.DiffuseColour = effect.DiffuseColor;
			shader.SpecularColour = effect.SpecularColor;
			shader.SpecularPower = effect.SpecularPower;

			if (effect.LightingEnabled)
			{
				shader.Lights = new MaterialLightCollection();
				shader.Lights.AmbientLightColour += effect.AmbientLightColor;

				Vector3 spec = new Vector3();
				if (effect.DirectionalLight0.Enabled)
				{
					shader.Lights.AddDirectionalLight(effect.PreferPerPixelLighting, effect.DirectionalLight0.Direction, effect.DirectionalLight0.DiffuseColor, effect.DirectionalLight0.SpecularColor);
					spec += effect.DirectionalLight0.SpecularColor;
				}
				if (effect.DirectionalLight1.Enabled)
				{
					shader.Lights.AddDirectionalLight(effect.PreferPerPixelLighting, effect.DirectionalLight1.Direction, effect.DirectionalLight1.DiffuseColor, effect.DirectionalLight1.SpecularColor);
					spec += effect.DirectionalLight1.SpecularColor;
				}
				if (effect.DirectionalLight2.Enabled)
				{
					shader.Lights.AddDirectionalLight(effect.PreferPerPixelLighting, effect.DirectionalLight2.Direction, effect.DirectionalLight2.DiffuseColor, effect.DirectionalLight2.SpecularColor);
					spec += effect.DirectionalLight2.SpecularColor;
				}
				shader.UsePerPixelSpecular = effect.PreferPerPixelLighting && spec.Length() > 0.005f;
			}

			return shader;
		}

		/// <summary>
		/// Gets/Sets the diffuse colour of this material, (Default value is White (1,1,1))
		/// </summary>
		public Vector3 DiffuseColour
		{
			get { return diffuseColour; }
			set { diffuseColour = value; }
		}


		/// <summary>
		/// Gets/Sets the specular colour of this material, (Default value is Black (0,0,0))
		/// </summary>
		public Vector3 SpecularColour
		{	
			get { return specularColour; }
			set { specularColour = value; }
		}


		/// <summary>
		/// Specular power of the material (larger values produce a more focused specular reflection. Values between 5 and 32 are common). Default value is 16. Note the default specular colour is black (no specular)
		/// </summary>
		/// <remarks><para>Each light source can also modify the specular power through a scaler (see <see cref="IMaterialLight.SpecularPowerScaler"/>)</para></remarks>
		public float SpecularPower
		{
			get { return specularPower; }
			set { specularPower = value; }
		}


		/// <summary>
		/// <para>Enables specular reflection for per-pixel lights (off by defualt)</para>
		/// <para>NOTE: Enabling specular reflections for per-pixel lights more than doubles the complexity of the lighting calculation, reducing the maximum supported number of per-pixel lights significantly (see <see cref="MaxPerPixelLights"/> and <see cref="MaxPerPixelSpecularLights"/>).</para>
		/// </summary>
		public bool UsePerPixelSpecular
		{
			get { return perPixelSpecular; }
			set
			{
				if (perPixelSpecular != value)
				{
					perPixelSpecular = value;
					dirty = true;
				}
			}
		}


		/// <summary>
		/// Gets the maximum number of per-vertex lights supported.
		/// </summary>
		public static int MaxPerVertexLights
		{
			get { return ShaderMerge.MaxVertexLights; }
		}
		/// <summary>
		/// Gets the maximum number of per-pixel lights supported.
		/// </summary>
		public static int MaxPerPixelLights
		{
			get { return ShaderMerge.MaxPixelLights; }
		}
		/// <summary>
		/// Gets the maximum number of per-pixel lights supported when <see cref="UsePerPixelSpecular"/> is set to true. Otherwise the maximum is <see cref="MaxPerPixelLights"/>
		/// </summary>
		public static int MaxPerPixelSpecularLights
		{
			get { return ShaderMerge.MaxPixelSpecularLights; }
		}

		/// <summary>
		/// Gets/Sets an optional texture map used during shading
		/// </summary>
		public Texture2D TextureMap
		{
			get { return texture; }
			set
			{
				if (texture == value)
					return;
				if ((texture == null) != (value == null))
					dirty = true;
				texture = value;
			}
		}
		/// <summary>
		/// Gets/Sets the texture sampler used for the optional texture map
		/// </summary>
		public TextureSamplerState TextureMapSampler
		{
			get { return textureSampler; }
			set { textureSampler = value; }
		}
		/// <summary>
		/// Gets/Sets the optional normal map used for lighting. Alpha of the normal map modulates specular reflection. Using a normal map requires the geometry has normals, tangents and binormals.
		/// </summary>
		public Texture2D NormalMap
		{
			get { return normalmap; }
			set
			{
				if (normalmap == value)
					return;
				if ((normalmap == null) != (value == null))
					dirty = true;
				normalmap = value;
			}
		}
		/// <summary>
		/// Gets/Sets the texture sampler used for the optional normal map
		/// </summary>
		public TextureSamplerState NormalMapSampler
		{
			get { return normalSampler; }
			set { normalSampler = value; }
		}

		/// <summary>
		/// Gets/Sets if this shader should use vertex colours from the geometry. All geometry drawn with this shader will require COLOR0 elements in their vertex geometry
		/// </summary>
		public bool UseVertexColour
		{
			get { return useVertexColour; }
			set 
			{
				if (value != useVertexColour)
				{
					dirty = true;
					useVertexColour = value;
				}
			}
		}

		/// <summary>
		/// Gets/Sets the base alpha value (default is 1)
		/// </summary>
		public float Alpha
		{
			get { return emissive.W; }
			set { emissive.W = value; }
		}

		/// <summary>
		/// Gets/Sets the emissive lighting value
		/// </summary>
		public Vector3 EmissiveColour
		{
			get { return new Vector3(emissive.X, emissive.Y, emissive.Z); }
			set
			{
				emissive.X = value.X;
				emissive.Y = value.Y;
				emissive.Z = value.Z;
			}
		}



		static Vector4[] ligthDataBufferVS = new Vector4[4 * 6];
		static Vector4[] ligthDataBufferPS = new Vector4[4 * 4];

		/// <summary>
		/// Bind the shader
		/// </summary>
		/// <remarks>The actual shader instance that is bound will differ depedning on the number of lights in use and the options selected (such as use vertex colour)</remarks>
		/// <param name="state"></param>
		public void Bind(IShaderSystem state)
		{
			DrawState _state = (DrawState)state;
			if (appId == null)
			{
				if (_state.UserValues.Contains(typeof(AppMaterialID).FullName))
					appId = (AppMaterialID)_state.UserValues[typeof(AppMaterialID).FullName];
				else
				{
					appId = new AppMaterialID();
					appId.application = _state.Application;
					appId.shaderMerge = new ShaderMerge();

					appId.v_lights_id = state.GetNameUniqueID("v_lights");
					appId.p_lights_id = state.GetNameUniqueID("p_lights");
					appId.ambient_id = state.GetNameUniqueID("ambient");
					appId.blendMatrices_id = state.GetNameUniqueID("blendMatrices");

					appId.CustomTexture_id = state.GetNameUniqueID("CustomTexture");
					appId.CustomTextureSampler_id = state.GetNameUniqueID("CustomTextureSampler");
					appId.CustomNormalMap_id = state.GetNameUniqueID("CustomNormalMap");
					appId.CustomNormalMapSampler_id = state.GetNameUniqueID("CustomNormalMapSampler");
					_state.UserValues.Add(typeof(AppMaterialID).FullName, appId);
				}
			}

#if DEBUG
			if (appId.application != _state.Application)
				throw new ArgumentException("MaterialShader is being used with multiple application instances");
#endif

			int vsLightCount = 0;
			int psLightCount = 0;
			Vector3 ambient = new Vector3(1,1,1);

			if (lightCollection != null &&
				lightCollection.LightingEnabled)
			{
				vsLightCount = lightCollection.vlights.Count;
				psLightCount = lightCollection.plights.Count;
				ambient = lightCollection.ambient * diffuseColour;


				if (vsLightCount > 0)
				{
					Vector4 v = new Vector4();
					int j = 0;
					for (int i = 0; i < lightCollection.vlights.Count; i++)
					{
						ShaderLight light = lightCollection.vlights[i];

						if (light.Enabled == false)
						{
							vsLightCount--;
							continue;
						}

						ligthDataBufferVS[j++] = light.position;

						v.X = light.specular.X * specularColour.X;
						v.Y = light.specular.Y * specularColour.Y;
						v.Z = light.specular.Z * specularColour.Z;
						v.W = light.specular.W * specularPower;

						ligthDataBufferVS[j++] = v;

						v.X = light.colour.X * diffuseColour.X;
						v.Y = light.colour.Y * diffuseColour.Y;
						v.Z = light.colour.Z * diffuseColour.Z;
						v.W = 0;

						ligthDataBufferVS[j++] = v;

						v.X = light.attenuation.X;
						v.Y = light.attenuation.Y;
						v.Z = light.attenuation.Z;

						ligthDataBufferVS[j++] = v;
					}
					//VS shaders calculate lights for 0,1,3,6
					//so 2,4 & 5 need to skip the following light (in 4's case, skip the two following)
					if (vsLightCount == 2 ||
						vsLightCount == 4 ||
						vsLightCount == 5)
					{
						ligthDataBufferVS[j++] = new Vector4();
						ligthDataBufferVS[j++] = new Vector4(0, 0, 0, 1);
						ligthDataBufferVS[j++] = new Vector4();
						ligthDataBufferVS[j++] = new Vector4(1, 0, 0, 0);
					}
					if (vsLightCount == 4)
					{
						ligthDataBufferVS[j++] = new Vector4();
						ligthDataBufferVS[j++] = new Vector4(0, 0, 0, 1);
						ligthDataBufferVS[j++] = new Vector4();
						ligthDataBufferVS[j++] = new Vector4(1, 0, 0, 0);
					}
				}

				if (psLightCount > 0)
				{
					Vector4 v = new Vector4();

					for (int i = 0, j = 0; i < lightCollection.plights.Count; i++)
					{
						ShaderLight light = lightCollection.plights[i];

						if (light.Enabled == false)
						{
							psLightCount--;
							continue;
						}

						ligthDataBufferPS[j++] = light.position;

						v.X = light.specular.X * specularColour.X;
						v.Y = light.specular.Y * specularColour.Y;
						v.Z = light.specular.Z * specularColour.Z;
						v.W = light.specular.W * specularPower;

						ligthDataBufferPS[j++] = v;

						v.X = light.colour.X * diffuseColour.X;
						v.Y = light.colour.Y * diffuseColour.Y;
						v.Z = light.colour.Z * diffuseColour.Z;
						v.W = 0;

						ligthDataBufferPS[j++] = v;

						v.X = light.attenuation.X;
						v.Y = light.attenuation.Y;
						v.Z = light.attenuation.Z;

						ligthDataBufferPS[j++] = v;
					}
				}
			}

			if (vsLightCount != this.vsLightCount ||
				psLightCount != this.psLightCount)
			{
				dirty = true;
				this.vsLightCount = (short)vsLightCount;
				this.psLightCount = (short)psLightCount;
			}

			if (animationTransforms != null && animationTransforms.BlendingEnabled != (this.shaderBoundBones != null))
				dirty = true;

			if (dirty)
			{
				this.shader = appId.shaderMerge.GetShader(
					_state,
					vsLightCount, 
					psLightCount, 
					this.texture != null, 
					this.useVertexColour, 
					this.normalmap != null, 
					this.perPixelSpecular, 
					animationTransforms != null && animationTransforms.BlendingEnabled, 
					out shaderBoundBones);

				dirty = false;
			}

			if (vsLightCount > 0)
				shader.SetAttribute(state, appId.v_lights_id, ligthDataBufferVS);
			if (psLightCount > 0)
				shader.SetAttribute(state, appId.p_lights_id, ligthDataBufferPS);


			if (animationTransforms != null && animationTransforms.BlendingEnabled)
			{
				if (shaderBoundBones.boneSource != animationTransforms ||
					shaderBoundBones.changeId != animationTransforms.ChangeIndex)
				{
					//copy the bones into the shader (they may already be there if this shader object is being reused)
					shaderBoundBones.changeId = animationTransforms.ChangeIndex;
					shaderBoundBones.boneSource = animationTransforms;

					shader.SetAttribute(state, appId.blendMatrices_id, animationTransforms.GetMatrixData());
				}
			}

			if (this.texture != null)
			{
				shader.SetTexture(state, appId.CustomTexture_id, texture);
				shader.SetSamplerState(state, appId.CustomTextureSampler_id, textureSampler);
			}
			if (this.normalmap != null)
			{
				if (texture == null)
				{
					shader.SetTexture(state, appId.CustomTexture_id, WhiteTexture.GetTexture(_state));
					shader.SetSamplerState(state, appId.CustomTextureSampler_id, TextureSamplerState.BilinearFiltering);
				}

				shader.SetTexture(state, appId.CustomNormalMap_id, normalmap);
				shader.SetSamplerState(state, appId.CustomNormalMapSampler_id, normalSampler);
			}

			Vector4 emmissive = this.emissive;
			emmissive.X += ambient.X;
			emmissive.Y += ambient.Y;
			emmissive.Z += ambient.Z;

			shader.SetAttribute(state, appId.ambient_id, ref emmissive);

			shader.Bind(state);
		}

		#region IShader

		bool IShader.HasChanged
		{
			get { return false; }
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, float[] value)
		{
			return false;
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, Vector2[] value)
		{
			return false;
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, Vector3[] value)
		{
			return false;
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, Vector4[] value)
		{
			return false;
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, Matrix[] value)
		{
			return false;
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, float value)
		{
			return false;
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, ref Vector2 value)
		{
			return false;
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, ref Vector3 value)
		{
			return false;
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, ref Vector4 value)
		{
			return false;
		}

		bool IShader.SetAttribute(IShaderSystem state, int name_uid, ref Matrix value)
		{
			return false;
		}

		bool IShader.SetSamplerState(IShaderSystem state, int name_uid, Xen.Graphics.State.TextureSamplerState sampler)
		{
			return false;
		}

		bool IShader.SetTexture(IShaderSystem state, int name_uid, TextureCube texture)
		{
			return false;
		}

		bool IShader.SetTexture(IShaderSystem state, int name_uid, Texture3D texture)
		{
			return false;
		}

		bool IShader.SetTexture(IShaderSystem state, int name_uid, Texture2D texture)
		{
			return false;
		}

		bool IShader.SetTexture(IShaderSystem state, int name_uid, Texture texture)
		{
			return false;
		}


		void IShader.GetVertexInput(int index, out VertexElementUsage elementUsage, out int elementIndex)
		{
			this.shader.GetVertexInput(index, out elementUsage, out elementIndex);
		}

		int IShader.GetVertexInputCount()
		{
			return this.shader.GetVertexInputCount();
		}
		#endregion

	}
}
