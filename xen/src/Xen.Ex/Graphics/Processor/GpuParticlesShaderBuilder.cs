using System;
using System.Collections.Generic;
using System.Text;
using Xen.Graphics.State;
using Xen.Graphics;
using Xen.Camera;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Xen.Ex.Graphics.Display;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Xen.Ex.Graphics.Content;

namespace Xen.Ex.Graphics.Processor
{
#if DEBUG && !XBOX360

	//this class needs to be here in order to hot load the particle system at runtime
	//it generates pixel shaders for particle systems.
	//the vertex shaders are consistent for all particle systems, and are embedded in GpuParticleData
	static class GpuParticleShaderBuilder
	{
		#region shader strings

		const string BasePixelShader =
@"
float4 globals[4]	: register(c0);
// stepSize (delta time), current step, noise base XY
float4 constants	: register(c4);
// current the max time step, and two random offsets (very small values)
float4 constants2	: register(c5);

sampler2D RandSampler				: register(s0);

_#ifdef TEXTURE_PARTICLE_METHOD
sampler2D PositionSizeSampler		: register(s1);
sampler2D VelocityRotationSampler	: register(s2);
_#endif

float2 random(float x, float y)
{
	return tex2D(RandSampler,float2(x,y)) + constants2.xy;
}

float rand(float from, float to, float randX, float randY)
{
	return from + (to - from) * random(randX,randY).x;
}
float rand(float to, float randX, float randY)
{
	return to * random(randX,randY).x;
}
float rand_smooth(float from, float to, float randX, float randY)
{
	return from + (to - from) * random(randX,randY).y;
}
float rand_smooth(float to, float randX, float randY)
{
	return to * random(randX,randY).y;
}

void PS_Method(
			float4 texRandIndex,
			float4 lifeIndex,
			float4 defaultPosition,
			float4 defaultVelocity,
			float4 defaultColour,
			float4 defaultUserData,
			out float4 posOut,
			out float4 velOut,
			out float4 colOut,
			out float4 userOut)
{
	float4 _positionSize			= defaultPosition;
	float4 _velocityRotation		= defaultVelocity;
	float4 _user					= defaultUserData;
	float4 _colour					= defaultColour;
	float4 _life					= lifeIndex;

	float2 index = texRandIndex.xy;
	float2 randIndex = texRandIndex.zw + constants.zw;

_#ifdef TEXTURE_PARTICLE_METHOD

	_positionSize			= tex2D(PositionSizeSampler,index);
	_velocityRotation		= tex2D(VelocityRotationSampler,index);

_#ifdef USER_COLOUR_TEX
	_colour				= tex2D(ColourSampler,index);
_#endif

_#ifdef USER_USER_TEX
	_user				= tex2D(UserSampler,index);
_#endif

_#ifdef USER_LIFE_TEX
	_life.xy			= tex2D(LifeSampler,index).xy;
_#endif

_#endif

	//most of this gets compiled out, unless used by the shader in the user block

	float local0=0,local1=0,local2=0,local3=0;
	float delta_time = constants2.z;

	float global0	= globals[0].x,  global1  = globals[0].y,  global2  = globals[0].z,  global3  = globals[0].w;
	float global4	= globals[1].x,  global5  = globals[1].y,  global6  = globals[1].z,  global7  = globals[1].w;
	float global8	= globals[2].x,  global9  = globals[2].y,  global10 = globals[2].z,  global11 = globals[2].w;
	float global12	= globals[3].x,  global13 = globals[3].y,  global14 = globals[3].z,  global15 = globals[3].w;

	float3 position	= _positionSize.xyz;
	float size		= _positionSize.w;

	float3 velocity	= _velocityRotation.xyz;
	float rotation	= _velocityRotation.w;

	float red		= _colour.r;
	float green		= _colour.g;
	float blue		= _colour.b;
	float alpha		= _colour.a;

	float life		= _life.x;
	float age		= constants.y - _life.y;
	
	if (age < 0)
		age			+= constants.x;

	float user0		= _user.x;
	float user1		= _user.y;
	float user2		= _user.z;
	float user3		= _user.w;

_#ifdef ADD_VELOCITY

	position += velocity * delta_time;

_#endif

//Begin user code

%%USER_CODE%%
//End user code

_#ifdef WRITE_POS_TO_USER

	user1 = position.x;
	user2 = position.y;
	user3 = position.z;

	position = 0;

_#endif

	posOut = float4(position,size);
	velOut = float4(velocity,rotation);
	colOut = float4(red,green,blue,alpha);
	userOut = float4(user0,user1,user2,user3);
}
";
	

		private const string replaceMarker = @"%%USER_CODE%%";

		private static Dictionary<string, char> opTypes;

		#endregion

		public enum LogicType
		{
			Frame,
			FrameMove,
			Once,
			OnceClone,
		}


		public static ParticleSystemCompiledShaderData BuildGpuLogicPixelShader(IEnumerable<ParticleSystemLogicStep> steps, LogicType logicType, bool useUserValues, bool useColours, bool storeLifeData, TargetPlatform targetPlatform, bool useUserDataPositionBuffer)
		{
			if (steps == null)
				throw new ArgumentNullException();

			StringBuilder output = new StringBuilder();
			Random random = new Random();

			foreach (ParticleSystemLogicStep step in steps)
				BuildStep(step, output, 1, random);

			string psCode = GpuParticleShaderBuilder.BasePixelShader.Replace(GpuParticleShaderBuilder.replaceMarker, output.ToString()).Replace("_#", "#");

			//build the shader header and main method.

			string headerPS = "";
			string methodHeaderPS = "";
			string methodPS = "void PS(float4 texRandIndex : TEXCOORD0";

			if (logicType != LogicType.Frame)
				methodPS += ", float4 lifeIndex : TEXCOORD1";
			else
				methodHeaderPS += "float4 lifeIndex = 0;";

			if (logicType == LogicType.Once)
			{
				methodPS += ", float4 defaultPosition : TEXCOORD2";
				methodPS += ", float4 defaultVelocity : TEXCOORD3";
				methodPS += ", float4 defaultColour   : TEXCOORD4";
				methodPS += ", float4 defaultUserData : TEXCOORD5";
			}
			else
			{
				methodHeaderPS += "float4 defaultPosition = 0, defaultVelocity = 0;";
				methodHeaderPS += "float4 defaultColour = 1, defaultUserData = 0;";
			}

			int colIndex = 2;

			methodPS += ", out float4 posOut : COLOR0, out float4 velOut : COLOR1";


			if (useColours)
				methodPS += string.Format(", out float4 colOut : COLOR{0}", colIndex++);
			else
				methodHeaderPS += "float4 colOut = 1;";

			if (useUserValues)
				methodPS += string.Format(", out float4 userOut : COLOR{0}", colIndex++);
			else
				methodHeaderPS += "float4 userOut = 0;";

			methodPS += ")\n{\n\t" + methodHeaderPS;

			methodPS += "\n\t" + @"PS_Method(texRandIndex,  
				lifeIndex, defaultPosition, defaultVelocity, defaultColour, defaultUserData, 
				posOut, velOut, colOut, userOut);";
			methodPS += "\n}";

			int colourIndex = -1, userIndex = -1, lifeIndex = -1;

			if (logicType != LogicType.Once)
			{
				int samplerIndex = 3;
				headerPS = Environment.NewLine + "#define TEXTURE_PARTICLE_METHOD" + Environment.NewLine;
				if (useColours)
				{
					colourIndex = samplerIndex;
					headerPS += Environment.NewLine + "#define USER_COLOUR_TEX" + Environment.NewLine;
					headerPS += Environment.NewLine + string.Format(@"sampler2D ColourSampler : register(s{0});", samplerIndex++) + Environment.NewLine;
				}
				if (useUserValues)
				{
					userIndex = samplerIndex;
					headerPS += Environment.NewLine + "#define USER_USER_TEX" + Environment.NewLine;
					headerPS += Environment.NewLine + string.Format(@"sampler2D UserSampler : register(s{0});", samplerIndex++) + Environment.NewLine;
				}
				if (storeLifeData && logicType == LogicType.Frame)
				{
					lifeIndex = samplerIndex;
					headerPS += Environment.NewLine + "#define USER_LIFE_TEX" + Environment.NewLine;
					headerPS += Environment.NewLine + string.Format(@"sampler2D LifeSampler : register(s{0});", samplerIndex++) + Environment.NewLine;
				}

				if (logicType == LogicType.Frame || logicType == LogicType.FrameMove)
					headerPS += Environment.NewLine + "#define ADD_VELOCITY" + Environment.NewLine;
			}

			if ((logicType == LogicType.OnceClone || logicType == LogicType.Once) &&
				useUserDataPositionBuffer)
			{
				headerPS += Environment.NewLine + "#define WRITE_POS_TO_USER" + Environment.NewLine;
			}

			psCode = headerPS + Environment.NewLine + psCode + Environment.NewLine + methodPS;


			CompiledShader pscs = ShaderCompiler.CompileFromSource(psCode, null, null, CompilerOptions.None, "PS", ShaderProfile.PS_3_0, targetPlatform);

			if (!pscs.Success) 
				throw new InvalidOperationException("GPU Particle System Pixel Shader failed to compile:" + Environment.NewLine + pscs.ErrorsAndWarnings);

			return new ParticleSystemCompiledShaderData(pscs.GetShaderCode(), colourIndex, userIndex, lifeIndex);
		}


		private static void BuildStep(ParticleSystemLogicStep step, StringBuilder output, int depth, Random random)
		{
			for (int i = 0; i < depth; i++)
				output.Append('\t');


			if (opTypes == null)
			{
				opTypes = new Dictionary<string, char>();
				opTypes.Add("div", '/');
				opTypes.Add("add", '+');
				opTypes.Add("sub", '-');
				opTypes.Add("mul", '*');
			}
			if (step.children == null)
			{
				//this is a simple function
				switch (step.type)
				{
					case "set":
						output.Append(step.target);
						output.Append(" = ");
						output.Append(step.arg0);
						output.AppendLine(";");
						return;
					case "madd":
						output.Append(step.target);
						output.Append(" += ");
						output.Append(step.arg0);
						output.Append(" * ");
						output.Append(step.arg1);
						output.AppendLine(";");
						return;
					case "add":
					case "sub":
					case "mul":
					case "div":
						output.Append(step.target);
						output.Append(' ');
						if (step.arg1 == null)
						{
							output.Append(opTypes[step.type]);
							output.Append("= ");
							output.Append(step.arg0);
						}
						else
						{
							output.Append("= ");
							output.Append(step.arg0);
							output.Append(' ');
							output.Append(opTypes[step.type]);
							output.Append(' ');
							output.Append(step.arg1);
						}
						output.AppendLine(";");
						return;
					default:
						//function
						output.Append(step.target);
						output.Append(" = ");
						output.Append(step.type);
						output.Append('(');
						output.Append(step.arg0);
						if (step.arg1 != null)
						{
							output.Append(',');
							output.Append(step.arg1);
						}
						if (step.type.StartsWith("rand"))
						{
							//add two static-random numbers onto the end of the function call
							output.Append(", randIndex.x + ");
							output.Append(random.NextDouble());
							output.Append(", randIndex.y + ");
							output.Append(random.NextDouble());
						}
						output.AppendLine(");");
						return;
				}
			}
			else
			{
				//branching operations
				switch (step.type)
				{
					case "loop":

						//for loop
						output.Append("for (int i");
						output.Append(depth);
						output.Append("; i");
						output.Append(depth);
						output.Append(" < ");
						output.Append(step.arg0);
						output.Append("; i");
						output.Append(depth);
						output.AppendLine("++)");
						// {
						for (int i = 0; i < depth; i++)
							output.Append('\t');
						output.AppendLine("{");

						//children
						for (int i = 0; i < step.children.Length; i++)
							BuildStep(step.children[i], output, depth + 1, random);

						// }
						for (int i = 0; i < depth; i++)
							output.Append('\t');
						output.AppendLine("}");

						return;

					case "if_equal":
					case "if_notequal":
					case "if_lessequal":
					case "if_greaterequal":
					case "if_less":
					case "if_greater":

						//if (
						output.Append("if (");
						output.Append(step.arg0);
						output.Append(' ');

						//operator
						if (step.type == "if_equal") output.Append("==");
						if (step.type == "if_notequal") output.Append("!=");
						if (step.type == "if_lessequal") output.Append("<=");
						if (step.type == "if_greaterequal") output.Append(">=");
						if (step.type == "if_less") output.Append('<');
						if (step.type == "if_greater") output.Append('>');

						output.Append(' ');
						output.Append(step.arg1);
						output.AppendLine(")");

						// {
						for (int i = 0; i < depth; i++)
							output.Append('\t');
						output.AppendLine("{");

						//children
						for (int i = 0; i < step.children.Length; i++)
							BuildStep(step.children[i], output, depth + 1,random);

						// }
						for (int i = 0; i < depth; i++)
							output.Append('\t');
						output.AppendLine("}");

						return;
					}	
			}
		}
	}

#endif
}
