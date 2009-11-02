using System;
using System.Text;
using System.Collections.Generic;



using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xen.Ex.Graphics.Content;
using Xen.Ex.Material;
using Xen.Graphics.State;


/*
 * This sample extends from Tutorial_11 (Model Animation)
 * This sample demonstrates:
 * 
 * How certain Xen features can be used to implement a complex effect
 * 
 */
namespace Tutorials.Tutorial_25
{

	//
	// Please Note:
	//
	// This tutorial does not teach how shadow mapping works.
	// This tutorial shows how xen can be used to structure an effect such as shadow mapping.
	// Xen has no pre-built support for any shadow mapping effects.
	//
	// Shadow mapping requires an advanced understanding of many aspects of computer graphics
	// that goes well beyond the scope of these tutorials. There are many subtlties involved.
	// This tutorial does not attempt to explain any of these advanced concepts.
	//
	// Shadow mapping, in it's simplest form, is an extremely simple technique conceptually. 
	// The complications come from the understanding of these key aspects of computer graphics.
	//



	//
	// This tutorial relies heavily on Xen Draw Flags.
	// Draw Flags are intended to simplify managing a scene or object that is rendered multiple times.
	//
	// A Draw Flag can be any Structure or Enum type. The DrawState object stores a stack of DrawFlags,
	// which can be Pushed(), Popped() and set. Draw Flags are very fast to access.
	//
	// For example, the ModelInstance class has a method 'SetShaderOverride(..)'. This allows a model
	// to be displayed with a custom shader, using the ModelInstanceShaderProvider class.
	// However, if the scene has a large number of ModelInstance objects, and it is desired to draw
	// all of them with the same shader, it can be tricky or ugly to assign the shader provider for
	// every model, and then revert the setting when drawing is complete.
	//
	// For example, it may be desirable to render the scene with a custom shader that outputs a fog effect.
	// In such a case, traversing the scene, assigning the shader providers, drawing the scene, then finally
	// reverting the shader providers is not only tedious, but it's ugly and potentially error prone.
	//
	// This is where Draw Flags can significantly simplify the process. Draw flags are stored in the
	// DrawState, as a stack, so a Flag can be Pushed() before rendering the fog effect, then Popped()
	// once rendering is complete. The model can check the Flag when it draws (which is very fast), and 
	// adjust it's rendering logic accordingly.
	//
	// In the example above, the ModelInstance.ShaderProviderFlag structure can be used. This flag simply
	// stores a ShaderProvider instance. When rendering, the ModelInstance class will check the Flag, if it
	// is set, it will use the ShaderProvider stored in the flag, otherwise, it will use its own.
	//
	// Eg:
	//
	// state.PushDrawFlag(new ModelInstance.ShaderProviderFlag(...));
	//
	// scene.Draw(state);
	//
	// state.PopDrawFlag<ModelInstance.ShaderProviderFlag>();
	//
	// 
	// The MaterialLightCollection class also provides the 'LightCollectionFlag' structure, which is used
	// by ModelInstance and ModelBatch.
	//


	//here, a custom Enum is created, which will flag the rendering mode of the tutorial.
	//This will be used as a DrawFlag
	enum TutorialRenderMode
	{
		Default,
		DepthOutput,	// depth will be drawn into the shadow map
		DrawShadow		// the shadow effect will be drawn
	}


	//This class is a ShaderProvider, it overrides the shaders used by a ModelInstance.
	//This class will query the TutorialRenderMode, and bind the required shader.
	//
	//DepthOutput mode will draw the models using the Xen.Ex.Shaders.DepthOutRg shaders,
	//these shaders ouput linear depth to Red, and depth squared to Green.
	//
	sealed class ShadowOutputShaderProvider : Xen.Ex.Graphics.ModelInstanceShaderProvider
	{
		private Vector4[] animationBoneData;
		//set this value to true when bone data has changed
		private bool animationBoneDataDirty;

		//the BeginDraw() methods are the first to be called. (One of these two will be called)

		//without animation:
		public override void BeginDraw(DrawState state)
		{
			animationBoneData = null;
		}
		//with animation:
		public override void BeginDraw(DrawState state, Xen.Ex.Transform[] animationBoneHierarchy, Vector4[] animationBoneHierarchyMatrix4x3)
		{
			animationBoneData = animationBoneHierarchyMatrix4x3;
			//set to true, so bone data is only copied once.
			animationBoneDataDirty = true;
		}

		//this is called just before geometry is drawn,
		//return true to indicate the shader has been set
		public override bool BeginGeometryShaderOverride(DrawState state, GeometryData geometry, Xen.Ex.Material.MaterialLightCollection lights)
		{
			//query the draw flag, 
			switch (state.GetDrawFlag<TutorialRenderMode>())
			{
				case TutorialRenderMode.DrawShadow:
				{
					//bind the shadow rendering shader...
					if (animationBoneData == null)
					{
						Shader.ShadowShader shader = state.GetShader<Shader.ShadowShader>();

						shader.TextureMap = geometry.MaterialShader.TextureMap;
						shader.TextureSampler = geometry.MaterialShader.TextureMapSampler;

						shader.Bind(state);
					}
					else
					{
						//bind the animating shader,

						Shader.ShadowShaderBlend shader = state.GetShader<Shader.ShadowShaderBlend>();

						//set the blend matrix data
						if (animationBoneDataDirty)
						{
							//use the 'animationBoneDataDirty' bool so animation data is only copied once.
							//this could happen if a single model has many pieces of geometry.
							shader.SetBlendMatrices(animationBoneData);
							animationBoneDataDirty = false;
						}

						shader.TextureMap = geometry.MaterialShader.TextureMap;
						shader.TextureSampler = geometry.MaterialShader.TextureMapSampler;

						shader.Bind(state);
					}
					return true;//shader was assigned
				}

				case TutorialRenderMode.DepthOutput:
				{
					//determine if alpha test is being used (in this tutorial it won't be - but do it anyway...)
					bool alphaTest = state.RenderState.AlphaTest.Enabled;

					if (alphaTest)
					{
						//alpha test is only needed if a texture is set
						alphaTest &= geometry.MaterialShader.TextureMap != null;
					}


					if (alphaTest)
					{
						//bind a depth output shader that samples a texture for alpha (for alpha test compatibility)

						if (this.animationBoneData != null)
						{
							//the model is animated

							//get the shader
							Xen.Ex.Shaders.DepthOutRgTextureAlphaBlend shader = state.GetShader<Xen.Ex.Shaders.DepthOutRgTextureAlphaBlend>();

							//set animation data (it's possible this is called redundantly, so logic here could be improved)
							if (animationBoneDataDirty)
							{
								shader.SetBlendMatrices(this.animationBoneData);
								animationBoneDataDirty = false;
							}

							//set the texture
							shader.AlphaTexture = geometry.MaterialShader.TextureMap;
							shader.AlphaTextureSampler = geometry.MaterialShader.TextureMapSampler;

							//bind
							shader.Bind(state);
						}
						else
						{
							//get the shader
							Xen.Ex.Shaders.DepthOutRgTextureAlpha shader = state.GetShader<Xen.Ex.Shaders.DepthOutRgTextureAlpha>();

							//set the texture
							shader.AlphaTexture = geometry.MaterialShader.TextureMap;
							shader.AlphaTextureSampler = geometry.MaterialShader.TextureMapSampler;

							shader.Bind(state); // bind the basic depth out shader
						}
					}
					else
					{
						//bind a simple depth output shader

						if (this.animationBoneData != null)
						{
							//the model is animated
							Xen.Ex.Shaders.DepthOutRgBlend shader = state.GetShader<Xen.Ex.Shaders.DepthOutRgBlend>();

							//set animation data (it's possible this is called redundantly, so logic here could be improved)
							if (animationBoneDataDirty)
							{
								shader.SetBlendMatrices(this.animationBoneData);
								animationBoneDataDirty = false;
							}

							//bind
							shader.Bind(state);
						}
						else
							state.GetShader<Xen.Ex.Shaders.DepthOutRg>().Bind(state); // bind the basic depth out shader
					}
					return true;//shader was assigned
				}
			}

			//false, because no shader has been bound (will use the model material shader)
			return false;
		}

		//required by shader provider
		public override bool ProviderModifiesWorldMatrixInBeginDraw
		{
			get { return false; } // this class doesn't modify the world matrix in BeginDraw()
		}
	}


	//this class creates a very simple disk, which is drawn below the actors
	//this class queries the TutorialRenderMode draw flag as well
	class GroundDisk : IDraw, IContentOwner
	{
		private IVertices vertices;
		private MaterialShader material;

		public GroundDisk(ContentRegister content, float radius, MaterialLightCollection lights)
		{
			//build the disk
			VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[256];

			for (int i = 0; i < vertexData.Length; i++)
			{
				//a bunch of vertices, in a circle!
				float angle = (float)i / (float)vertexData.Length * MathHelper.TwoPi;
				Vector3 position = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle), 0);

				vertexData[i] = new VertexPositionNormalTexture(position * radius, new Vector3(0, 0, 1), new Vector2(position.X, position.Y));
			}
			this.vertices = new Vertices<VertexPositionNormalTexture>(vertexData);

			//create the material, and add to content
			this.material = new MaterialShader();
			this.material.Lights = lights;
			content.Add(this);
		}

		public void Draw(DrawState state)
		{
			//switch rendering mode based on the TutorialRenderMode flag
			switch (state.GetDrawFlag<TutorialRenderMode>())
			{
				case TutorialRenderMode.DepthOutput:
					//bind the depth output shader
					state.GetShader<Xen.Ex.Shaders.DepthOutRg>().Bind(state);
					break;
				case TutorialRenderMode.DrawShadow:
					//bind the shadow rendering shader
					Shader.ShadowShader shader = state.GetShader<Shader.ShadowShader>();
					shader.TextureMap = material.TextureMap;
					shader.TextureSampler = material.TextureMapSampler;
					shader.Bind(state);
					break;
				default:
					//no flag known specified
					material.Bind(state);
					break;
			}

			//draw the ground
			vertices.Draw(state, null, PrimitiveType.TriangleFan);
		}

		public bool CullTest(ICuller culler)
		{
			return true;
		}

		public void LoadContent(ContentRegister content, DrawState state, ContentManager manager)
		{
			material.TextureMap = manager.Load<Texture2D>(@"box");
		}

		public void UnloadContent(ContentRegister content, DrawState state)
		{
		}
	}

	//similar to the Tutorial 11 actor:
	//note, this class takes no special actions to deal with shadow rendering
	class Actor : IDraw, IContentOwner
	{
		private ModelInstance model;
		private Matrix worldMatrix;

		private AnimationController animationController;
		private AnimationInstance animation;

		public Actor(ContentRegister content, Vector3 position, float animationSpeed, int animationIndex)
		{
			Matrix.CreateRotationZ(1-(float)animationIndex, out this.worldMatrix);
			this.worldMatrix.Translation = position;

			model = new ModelInstance();
			this.animationController = model.GetAnimationController();

			content.Add(this);

			this.animation = this.animationController.PlayLoopingAnimation(animationIndex);
			this.animation.PlaybackSpeed = animationSpeed;
		}

		public void Draw(DrawState state)
		{
			state.PushWorldMatrixMultiply(ref this.worldMatrix);

			model.Draw(state);

			state.PopWorldMatrix();
		}

		public bool CullTest(ICuller culler)
		{
			return model.CullTest(culler, ref worldMatrix);
		}

		public void LoadContent(ContentRegister content, DrawState state, ContentManager manager)
		{
			//load the model data into the model instance
			model.ModelData = manager.Load<ModelData>(@"tiny_4anim");
		}

		public void UnloadContent(ContentRegister content, DrawState state)
		{
		}
	}


	// This is the object that is draws to the shadow map texture.
	// This class controls the rendering of the scene,
	// it sets up the draw flags that make the scene render Depth values
	class ShadowMapDrawer : IDraw
	{
		private ShadowOutputShaderProvider shaderProvider;
		private IDraw scene;

		public ShadowMapDrawer(IDraw scene, ShadowOutputShaderProvider shaderProvider)
		{
			this.scene = scene;
			this.shaderProvider = shaderProvider;
		}

		public void Draw(DrawState state)
		{
			//set the draw flags up, which will control rendering
			//this will make the models render depth
			state.PushDrawFlag(new ModelInstance.ShaderProviderFlag(this.shaderProvider));
			state.PushDrawFlag(TutorialRenderMode.DepthOutput);

			//draw the scene
			scene.Draw(state);

			//reset the flags
			state.PopDrawFlag<ModelInstance.ShaderProviderFlag>();
			state.PopDrawFlag<TutorialRenderMode>();
		}

		public bool CullTest(ICuller culler)
		{
			return true;
		}
	}

	//This class draws the shadow map into the scene.
	//It has the complex job of setting up the shadow shaders.
	class ShadowedSceneDrawer : IDraw
	{
		private ShadowOutputShaderProvider shaderProvider;
		private IDraw scene;
		private DrawTargetTexture2D shadowMapTarget;

		public ShadowedSceneDrawer(IDraw scene, ShadowOutputShaderProvider shaderProvider, DrawTargetTexture2D shadowMapTarget)
		{
			this.scene = scene;
			this.shaderProvider = shaderProvider;
			this.shadowMapTarget = shadowMapTarget;
		}

			
		public void Draw(DrawState state)
		{
			SetupShadowShader(state);

			//set render mode to shadow map
			state.PushDrawFlag(new ModelInstance.ShaderProviderFlag(this.shaderProvider));
			state.PushDrawFlag(TutorialRenderMode.DrawShadow);

			//Push the shadow map camera as a post-culler.
			//This way, anything not within the frustum of the shadow map
			//camera will not be drawn with the shadow shader
			state.PushPostCuller(this.shadowMapTarget.Camera);

			//set an additive blending mode
			state.PushRenderState();
			state.RenderState.AlphaBlend = AlphaBlendState.Additive;
			state.RenderState.DepthColourCull.DepthWriteEnabled = false;

			//draw the shadowed scene
			scene.Draw(state);

			state.PopRenderState();
			state.PopPostCuller();

			state.PopDrawFlag<TutorialRenderMode>();
			state.PopDrawFlag<ModelInstance.ShaderProviderFlag>();
		}


		private void SetupShadowShader(DrawState state)
		{
			ICamera shadowCamera = this.shadowMapTarget.Camera;

			//compute the view*projection matrix for the shadow map camera...

			Matrix view, projection, viewProjection;
			shadowCamera.GetViewMatrix(out view);
			shadowCamera.GetProjectionMatrix(out projection, this.shadowMapTarget.Size);

			Matrix.Multiply(ref view, ref projection, out viewProjection);

			//and the near clip, direction, etc
			Vector2 nearFarClip;
			Vector3 viewDirection, viewPoint;
			shadowCamera.GetCameraNearFarClip(out nearFarClip);
			shadowCamera.GetCameraViewDirection(out viewDirection);
			shadowCamera.GetCameraPosition(out viewPoint);


			//set the matrix and other constants in the shadow mapping shader instances
			Shader.ShadowShader shader = state.GetShader<Shader.ShadowShader>();
			Shader.ShadowShaderBlend shaderBlend = state.GetShader<Shader.ShadowShaderBlend>();

			//non-blending shader
			shader.ShadowMap = this.shadowMapTarget.GetTexture();
			shader.SetShadowMapProjection(ref viewProjection);

			shader.SetShadowCameraNearFar(ref nearFarClip);
			shader.SetShadowViewDirection(ref viewDirection);
			shader.SetShadowViewPoint(ref viewPoint);

			//setup the same constants for the blending shader
			shaderBlend.ShadowMap = this.shadowMapTarget.GetTexture();
			shaderBlend.SetShadowMapProjection(ref viewProjection);

			shaderBlend.SetShadowCameraNearFar(ref nearFarClip);
			shaderBlend.SetShadowViewDirection(ref viewDirection);
			shaderBlend.SetShadowViewPoint(ref viewPoint);
		}

		public bool CullTest(ICuller culler)
		{
			return true;
		}
	}



	[DisplayName(Name = "Tutorial 25: Shadow Mapping")]
	public class Tutorial : Application
	{
		//screen draw target
		private DrawTargetScreen drawToScreen;

		//a draw target which will have the shadow depth drawn
		private DrawTargetTexture2D drawShadowDepth;

		//a blur filter to blur the shadow depth texture
		private Xen.Ex.Filters.BlurFilter shadowDepthBlurFilter;
		
		//ambient lighting for the scene
		private MaterialLightCollection ambientLight;



		protected override void Initialise()
		{
			//setup ambient lighting
			this.ambientLight = new MaterialLightCollection();
			ambientLight.LightingEnabled = true;
			ambientLight.AmbientLightColour = new Vector3(0.25f, 0.25f, 0.25f);
			ambientLight.AddDirectionalLight(false, new Vector3(-1, -1, 0), new Vector3(2,2,2)); // add some backlighting

			//setup the shadow render camera
			Camera3D shadowCamera = new Camera3D();
			shadowCamera.LookAt(new Vector3(1, 1, 3), new Vector3(-15, 20, 20), new Vector3(0, 0, 1));

			//set the clip plane distances
			shadowCamera.Projection.FarClip = 40;
			shadowCamera.Projection.NearClip = 20;
			shadowCamera.Projection.FieldOfView *= 0.25f;


			//8bit is actually enough accuracy for this sample (given the limited range of the shadow)
			SurfaceFormat textureFormat = SurfaceFormat.Color;
			const int resolution = 512;

			//create the shadow map texture:
			drawShadowDepth = new DrawTargetTexture2D(shadowCamera, resolution, resolution, textureFormat, DepthFormat.Depth24);
			drawShadowDepth.ClearBuffer.ClearColour = Color.White;

			//for the shadow technique used, the shadow buffer is blurred.
			//this requires an intermediate render target on the PC
			DrawTargetTexture2D blurIntermediate = null;

#if !XBOX360
			//not required on the xbox if the render target is small enough to fit in EDRAM in one tile
			blurIntermediate = new DrawTargetTexture2D(shadowCamera,resolution,resolution,textureFormat);
#endif
			//create a blur filter
			shadowDepthBlurFilter = new Xen.Ex.Filters.BlurFilter(Xen.Ex.Filters.BlurFilterFormat.SevenSampleBlur, drawShadowDepth, blurIntermediate);

			//create the scene camera
			Camera3D camera = new Camera3D();
			camera.LookAt(new Vector3(0, 0, 3), new Vector3(10, 10, 6), new Vector3(0, 0, 1));
			camera.Projection.FieldOfView *= 0.55f;

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);
			drawToScreen.ClearBuffer.ClearColour = Color.Black;

			//the 'scene'
			//A DrawList from Tutorial 23 is used here, this stores the 'scene', 
			//which is just a set of actors and the ground
			Tutorials.Tutorial_23.DrawList scene = new Tutorials.Tutorial_23.DrawList();

			for (int x = 0; x < 2; x++)
			for (int y = 0; y < 2; y++)
			{
				//create the actor instances
				if (x != 0 || y != 0)
					scene.Add(new Actor(this.Content, new Vector3(x*6-3, y*6-3, 0), (x + y*2 + 1) * 0.1f, 4-x*2-y));
			}

			//add the ground
			GroundDisk ground = new GroundDisk(this.Content, 10, ambientLight);
			scene.Add(ground);


			//setup the draw targets...


			//create the shader provider
			ShadowOutputShaderProvider shadowOutputShaderProvider = new ShadowOutputShaderProvider();

			//add a ShadowMapDrawer to the shadow map texture
			drawShadowDepth.Add(new ShadowMapDrawer(scene, shadowOutputShaderProvider));

			//setup the scene to be drawn to the screen
			//draw the scene normally (no shadow, just ambient)
			drawToScreen.Add(scene);

			//then draw the scene with a shadow (blended on top)
			drawToScreen.Add(new ShadowedSceneDrawer(scene, shadowOutputShaderProvider, drawShadowDepth));

			//add a nice faded background
			Tutorial_20.BackgroundGradient background = new Tutorial_20.BackgroundGradient(Color.WhiteSmoke, Color.Black);
			background.DrawAtMaxZDepth = true;
			drawToScreen.Add(background);


			//create a textured element that will display the shadow map texture
			TexturedElement shadowDepthDisplay = new TexturedElement(drawShadowDepth, new Vector2(256, 256));
			shadowDepthDisplay.VerticalAlignment = VerticalAlignment.Top;
			this.drawToScreen.Add(shadowDepthDisplay);
		}

		protected override void Draw(DrawState state)
		{
			//draw the shadow map texture first,
			drawShadowDepth.Draw(state);

			//apply the blur filter to the shadow texture
			shadowDepthBlurFilter.Draw(state);


			//set a global light collection (that all models will use)
			state.PushDrawFlag(new MaterialLightCollection.LightCollectionFlag(ambientLight));

			//draw the scene to the screen
			drawToScreen.Draw(state);

			//reset the flag
			state.PopDrawFlag<MaterialLightCollection.LightCollectionFlag>();
		}

		protected override void SetupGraphicsDeviceManager(GraphicsDeviceManager graphics, ref RenderTargetUsage presentation)
		{
			if (graphics != null) // graphics is null when starting within a WinForms host
			{
				graphics.PreferredBackBufferWidth = 1280;
				graphics.PreferredBackBufferHeight = 720;
				graphics.PreferMultiSampling = true;
			}
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}
	}
}
