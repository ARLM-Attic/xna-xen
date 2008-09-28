using System;
using System.Collections.Generic;
using System.Text;



using Xen;
using Xen.Camera;
using Xen.Graphics;
using Xen.Ex.Geometry;
using Xen.Ex.Material;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


/*
 * This sample modifies Tutorial_02 (Draw sphere)
 * This sample demonstrates:
 * 
 * creating a custom vertex and index buffer
 * 
 * see the 'NEW CODE' comments for code that has changed in this tutorial
 * 
 */
namespace Tutorials.Tutorial_04
{
	//NEW CODE
	//A custom vertex structure, storing a position and normal.
	//(An XNA vertex structure could also be used)
	struct CustomVertex
	{
		public Vector3 position;
		public Vector3 normal;


		//constructor
		public CustomVertex(Vector3 position, Vector3 normal)
		{
			this.position = position;
			this.normal = normal;
		}
	}


	//NEW CODE
	//a class that draws a quad using custom vertices.
	class QuadGeometry : IDraw
	{
		//vertex and index buffers
		private IVertices vertices;
		private IIndices indices;


		//setup and create the vertices/indices
		public QuadGeometry()
		{
			//create an array of custom vertices to form a quad
			CustomVertex[] verts = new CustomVertex[]
			{
				new CustomVertex(new Vector3(-1,-1,0), Vector3.UnitZ), // bottom left
				new CustomVertex(new Vector3(-1, 1,0), Vector3.UnitZ), // top left
				new CustomVertex(new Vector3( 1,-1,0), Vector3.UnitZ), // bottom right
				new CustomVertex(new Vector3( 1, 1,0), Vector3.UnitZ), // top right
			};

			//create the indices array
			ushort[] inds = new ushort[]
			{
				0,1,2, // first triangle	(bottom left -> top left -> bottom right)
				1,3,2  // second triangle	(top left -> top right -> bottom right)
			};

			//create the vertices/indices objects
			this.vertices = new Vertices<CustomVertex>(verts);
			this.indices = new Indices<ushort>(inds);
		}

		//draw the quad
		public void Draw(DrawState state)
		{
			//draw the vertices as triangle list, with the indices
			this.vertices.Draw(state, this.indices, PrimitiveType.TriangleList);
		}

		public bool CullTest(ICuller culler)
		{
			//cull test with an approximate bounding box...
			return culler.TestBox(new Vector3(-1, -1, 0), new Vector3(1, 1, 0));
		}
	}
	


	//a simple class that draws the quad,
	//mostly unchanged from 'SphereDrawer' in tutorial 02
	class GeometryDrawer : IDraw
	{
		private QuadGeometry geometry;
		private Matrix worldMatrix;
		private IShader shader;

		public GeometryDrawer(Vector3 position)
		{
			//create the quad
			geometry = new QuadGeometry();

			//setup the world matrix
			worldMatrix = Matrix.CreateTranslation(position);

			//create a basic lighting shader with some nice looking lighting
			MaterialShader material = new MaterialShader();
			Vector3 lightDirection = new Vector3(0.5f,1,-0.5f); //a dramatic direction
			material.Lights = new MaterialLightCollection();
			material.Lights.AddDirectionalLight(false, lightDirection, Color.Gray);//two light sources
			material.Lights.AddDirectionalLight(false, -lightDirection, Color.DarkSlateBlue);
			material.SpecularColour = Color.LightYellow.ToVector3();//with a nice sheen

			this.shader = material;
		}

		public void Draw(DrawState state)
		{
			//push the world matrix, multiplying by the current matrix if there is one
			state.PushWorldMatrixMultiply(ref worldMatrix);

			//cull test the geometry
			if (geometry.CullTest(state))
			{
				//bind the shader
				shader.Bind(state);

				//draw the custom geometry
				geometry.Draw(state);
			}

			//always pop the matrix afterwards
			state.PopWorldMatrix();
		}

		//always draw.. don't cull yet
		public bool CullTest(ICuller culler)
		{
			return true;
		}
	}


	//this class hasn't changed...

	//a application that draws geometry in the middle of the screen
	[DisplayName(Name = "Tutorial 04: Custom Geometry")]
	public class Tutorial : Application
	{
		//a DrawTargetScreen is a draw target that draws items directly to the screen.
		//in this case it will only draw a GeometryDrawer
		private DrawTargetScreen drawToScreen;

		protected override void Initialise()
		{
			//draw targets usually need a camera.
			Camera3D camera = new Camera3D();
			//look at the geometry, which will be at 0,0,0
			camera.LookAt(Vector3.Zero, new Vector3(0, 0, 4), Vector3.UnitY);

			//create the draw target.
			drawToScreen = new DrawTargetScreen(this, camera);

			//create the geometry
			GeometryDrawer geometry = new GeometryDrawer(Vector3.Zero);

			//add it to be drawn to the screen
			drawToScreen.Add(geometry);
		}

		//main application draw method
		protected override void Draw(DrawState state)
		{
			//draw to the screen.
			drawToScreen.Draw(state);
		}

		protected override void Update(UpdateState state)
		{
			if (state.PlayerInput[PlayerIndex.One].InputState.Buttons.Back.OnPressed)
				this.Shutdown();
		}
	}
}
