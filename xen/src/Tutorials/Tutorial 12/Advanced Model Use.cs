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
using Xen.Ex;
using Xen.Graphics.State;


/*
 * 
 * This is an advanced tutorial.
 * 
 * 
 * This sample extends from Tutorial_11 (Model animation)
 * This sample demonstrates:
 * 
 * Using the embedded bounding box data stored in a model
 * Using the Graphics Device safely
 * 
 */
namespace Tutorials.Tutorial_12
{
	//In this tutorial, the bounding boxes of the model will be extracted, and drawn
	//This is an advanced technique
	//
	//The boxes will be drawn in wireframe mode. Wireframe isn't exposed by xen as a
	//render state, so the GraphicsDevice will need to be used.

	class Actor : IDraw, IContentOwner
	{
		private ModelInstance model;

		//NEW CODE
		//A list of bounding boxes
		private List<IDraw> boundingBoxes;

		public Actor(ContentRegister content)
		{
			//A ModelInstance can be created without any content...
			//However it cannot be used until the content is set

			model = new ModelInstance();

			content.Add(this);

			//play an animation
			model.GetAnimationController().PlayLoopingAnimation(3);

			//This method creates a Cube for each bone in the mesh
			BuildBoundingBoxGeometry();
		}

		public void Draw(DrawState state)
		{
			//ModelInstances automatically setup the default material shaders
			//Custom shaders can be used with model.SetShaderOverride(...)
			model.Draw(state);

			DrawBoundingBoxes(state);
		}

		//NEW CODE
		private void DrawBoundingBoxes(DrawState state)
		{
			//First, get the animated bone transforms of the model.
			//These transforms are in 'bone-space', not in world space.
			ReadOnlyArrayCollection<Transform> boneAnimationTransforms = model.GetAnimationController().GetTransformedBones(state);


			//Get a simple shader from Xen.Ex that fills a solid colour
			Xen.Ex.Shaders.FillSolidColour shader = state.GetShader<Xen.Ex.Shaders.FillSolidColour>();

			shader.FillColour = Color.White.ToVector4();

			shader.Bind(state);



			//push the render state...
			state.PushRenderState();

			//disable back face culling
			state.RenderState.DepthColourCull.CullMode = CullMode.None;


			//Get the graphics device.
			//This is the best way to get the graphics device, 
			//This tells xen that you intend to do something special with the device,
			//StateFlag.None tells xen that it does not need to dirty any of it's
			//internal state tracking buffers
			//
			//Future versions of xen are may render on multiple threads, 
			//Using BeginGetGraphicsDevice() and EndGetGraphicsDevice() will 
			//stay compatible in future versions
			GraphicsDevice device = state.BeginGetGraphicsDevice(StateFlag.None);

			//change the fill mode to wire frame (directly changing on the device)
			device.RenderState.FillMode = FillMode.WireFrame;

			//loop through all the geometry data in the model..
			//(note, the sample model has only 1 geometry instance)


			Xen.Ex.Graphics.Content.SkeletonData modelSkeleton = model.ModelData.Skeleton;
			Matrix matrix;
			int boxIndex = 0;

			foreach (Xen.Ex.Graphics.Content.MeshData meshData in model.ModelData.Meshes)
			{
				foreach (Xen.Ex.Graphics.Content.GeometryData geometry in meshData.Geometry)
				{
					//now loop through all bones used by this geometry

					for (int geometryBone = 0; geometryBone < geometry.BoneIndices.Length; geometryBone++)
					{
						//index of the bone (a piece of geometry may not use all the bones in the model)
						int boneIndex = geometry.BoneIndices[geometryBone];

						//get the base transform of the bone (the transform when not animated)
						Transform boneTransform = modelSkeleton.BoneWorldTransforms[boneIndex];

						//multiply the transform with the animation bone-local transform

						//it would be better to use Transform.Multiply() here to save data copying on the xbox
						boneTransform *= boneAnimationTransforms[boneIndex];

						//Get the transform as a matrix
						boneTransform.GetMatrix(out matrix);

						//push the matrix
						state.PushWorldMatrix(ref matrix);

						//draw the box
						if (boundingBoxes[boxIndex].CullTest(state))
							boundingBoxes[boxIndex].Draw(state);

						boxIndex++;

						//pop the world matrix
						state.PopWorldMatrix();
					}
				}
			}

			//Reset the fill mode
			device.RenderState.FillMode = FillMode.Solid;

			//end using the graphics device
			state.EndGetGraphicsDevice();

			//pop the render state
			state.PopRenderState();
		}

		//this method iterates through the geometry and creates the cubes used to display the bounding boxes
		//this is run when the tutorial starts
		private void BuildBoundingBoxGeometry()
		{
			boundingBoxes = new List<IDraw>(); 

			foreach (Xen.Ex.Graphics.Content.MeshData meshData in model.ModelData.Meshes)
			{
				foreach (Xen.Ex.Graphics.Content.GeometryData geometry in meshData.Geometry)
				{
					//now loop through all bones used by this geometry

					for (int geometryBone = 0; geometryBone < geometry.BoneIndices.Length; geometryBone++)
					{
						//index of the bone (a peice of geometry may not use all the bones in the model)
						int boneIndex = geometry.BoneIndices[geometryBone];

						//the bounds of the geometry for the given bone...
						Xen.Ex.Graphics.Content.GeometryBounds bounds = geometry.BoneLocalBounds[geometryBone];

						//create the cube
						boundingBoxes.Add(new Xen.Ex.Geometry.Cube(bounds.Minimum, bounds.Maximum));
					}
				}
			}
		}

		//from here everything is the same as the previous example

		public bool CullTest(ICuller culler)
		{
			return model.CullTest(culler);
		}


		public void LoadContent(ContentRegister content, DrawState state, ContentManager manager)
		{
			//load the model data into the model instance
			model.ModelData = manager.Load<Xen.Ex.Graphics.Content.ModelData>(@"tiny_4anim");
		}


		public void UnloadContent(ContentRegister content, DrawState state)
		{
		}
	}

	[DisplayName(Name = "Tutorial 12: Advanced Mesh Bounding Boxes")]
	public class Tutorial : Application
	{
		//screen draw target
		private DrawTargetScreen drawToScreen;

		protected override void Initialise()
		{
			Camera3D camera = new Camera3D();
			camera.LookAt(new Vector3(0, 0, 4), new Vector3(3, 4, 4), new Vector3(0, 0, 1));

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);

			//NEW CODE
			//create the actor instance
			drawToScreen.Add(new Actor(this.Content));
		}

		protected override void Draw(DrawState state)
		{
			drawToScreen.Draw(state);
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}
	}
}
