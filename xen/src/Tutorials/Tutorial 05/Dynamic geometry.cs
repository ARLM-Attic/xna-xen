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
 * This sample modifies Tutorial_04 (Custom geometry)
 * This sample demonstrates:
 * 
 * using a dynamic vertex buffer
 * 
 * see the 'NEW CODE' comments for code that has changed in this tutorial
 * 
 */
namespace Tutorials.Tutorial_05
{
	//A custom vertex structure, storing a position and normal.
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



	//a class that draws a quad using dynamic custom vertices.
	class DynamicQuadGeometry : IDraw
	{
		//vertex and index buffers
		private IVertices vertices;
		private IIndices indices;

		//NEW CODE
		//local copy of the source vertex data
		//Dynamic vertices/indices are created in the same way as normal vertices/indices,
		//To change the data, edit the source array then tell the buffer it needs updating
		//This differs from XNA, where you copy the changed data manually.
		//The advantage is you do not need to worry about the XNA ContentLost situation
		CustomVertex[] vertexData;


		//setup and create the vertices/indices
		public DynamicQuadGeometry()
		{
			//create the array of custom vertices, to form a quad
			vertexData = new CustomVertex[4]
			{
				new CustomVertex(new Vector3(-1,-1,0), Vector3.UnitZ), // bottom left
				new CustomVertex(new Vector3(-1, 1,0), Vector3.UnitZ), // top left
				new CustomVertex(new Vector3( 1,-1,0), Vector3.UnitZ), // bottom right
				new CustomVertex(new Vector3( 1, 1,0), Vector3.UnitZ), // top right
			};

			//create the buffers
			this.vertices = new Vertices<CustomVertex>(vertexData);
			this.indices = new Indices<ushort>(0, 1, 2, 1, 3, 2); //shortcut using params[] supporting constructor

			//NEW CODE
			//Set the resource usage of the vertices to Dynamic
			//This can only be set before the vertices are first used
			this.vertices.ResourceUsage = ResourceUsage.Dynamic;
		}


		//NEW CODE
		//writes dynamic data changes to the source vertex data (in this case, applying a simple sin wave)
		void OffsetVertices(float time)
		{
			float indexF = 0;//avoid casting index to a float (float to int hurts on the xbox! although here it's not going to do much :)

			//set the Z coordinate of each vertex to a sin wave
			for (int index = 0; index < vertexData.Length; index++)
			{
				//offset in the Z-axis (towards the camera)
				vertexData[index].position.Z = (float)Math.Sin(time + indexF);

				indexF++;
			}
		}


		//draw the quad
		public void Draw(DrawState state)
		{
			//NEW CODE
			//apply changes to the source vertex data
			OffsetVertices(state.TotalTimeSeconds);

			//NEW CODE
			//tell the vertices that the entire source buffer is has changed and needs updating
			this.vertices.SetDirty();

			//draw as usual
			this.vertices.Draw(state, this.indices, PrimitiveType.TriangleList);
		}

		public bool CullTest(ICuller culler)
		{
			//cull test with an approximate bounding box... (a bit of a hack :)
			return culler.TestBox(new Vector3(-1, -1, -1), new Vector3(1, 1, 1));
		}
	}

	//the rest of the code in this file is identical to tutorial 04


	//a simple class that draws the quad,
	//mostly unchanged from 'SphereDrawer' in tutorial 02
	class GeometryDrawer : IDraw
	{
		private DynamicQuadGeometry geometry;
		private Matrix worldMatrix;
		private IShader shader;

		public GeometryDrawer(Vector3 position)
		{
			//create the quad
			geometry = new DynamicQuadGeometry();

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
	[DisplayName(Name = "Tutorial 05: Dynamic Geometry")]
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
