//This shader implements a simple projection, with an extra technique for Hardware instancing support

//This next line is a special include required for using the xbox vfetch assembly instruction.
//This is accessed through a macro defined in the include.
//Directly writing the raw assembly is not supported. Use the macro instead.
//This instruction is not available on the PC, any code using the macro must be within an #if block:
//#ifndef XBOX360
// pc shader
//#else
// xbox shader 
//#endif

#include <asm_vfetch>

//IMPORTANT NOTE!
//Due to XNA's limited ability to decode Xbox shaders on the PC, using the asm_vfetch
//feature may cause the compiled shader to behave in an unexpected way or fail to compile.
//This can happen for two reasons.
//The first reason is that the shader plugin has to pre-process the shader, working out
//the method declarations and techniques. This is complex, and it is highly likely there
//are bugs in this process. (as always, please report any bugs encountered!)
//The second problem is that the xbox compiled shader may rearrange it's registers.
//While this is somewhat rare, when it does happen it cannot be detected on the PC!
//If this occurs, mark your shader constants with the : register(c#) semantic to enforce
//their registers.


float4x4 worldViewProj : WORLDVIEWPROJECTION;

//instancing shader provides it's own world matrix
float4x4 viewProj : VIEWPROJECTION;


float4 colour : GLOBAL;

//--------------------------------------------------------------//
// vertex shader
//--------------------------------------------------------------//
void Tutorial16_VS(	
					float4 position			: POSITION, 
				out float4 positionOut		: POSITION)
{
	positionOut = mul(position,worldViewProj);
}


#ifndef XBOX360
//first, the PC instancing shader

//--------------------------------------------------------------//
// vertex shader, using instancing for the world matrix
// when using hardware instancing, the instance world matrix 
// is passed in {POSITION12,POSITION13,POSITION14,POSITION15}
//--------------------------------------------------------------//
void Tutorial16_VS_Instance(	
					float4 position			: POSITION, 
				out float4 positionOut		: POSITION,
				
					float4 worldX			: POSITION12,
					float4 worldY			: POSITION13,
					float4 worldZ			: POSITION14,
					float4 worldW			: POSITION15)
{
	//get the instance world matrix
	float4x4 instanceWorld = float4x4(worldX,worldY,worldZ,worldW);
	
	//note the world matrix is multiplied first, then the view projection
	
	positionOut = mul(mul(position,instanceWorld),viewProj);
}


#else

//now, the xbox version using vfetch

//the number of vertices used per instance
int VertexCount : VERTEXCOUNT;

//note that only the index and ouput position are specified.

void Tutorial16_VS_Instance(int index : INDEX,	
							out float4 positionOut		: POSITION)
{
	//work out the vertex and instance index.
	
    int vertexIndex = fmod(index + 0.5,VertexCount);
    int instanceIndex = (index + 0.5) / VertexCount;
    
	float4 position, worldX,worldY,worldZ,worldW;
	
	//fetch the vertex position
	asm_vfetch(position,vertexIndex,position0);
	
	//fetch the instance parts
	asm_vfetch(worldX,instanceIndex,position12);
	asm_vfetch(worldY,instanceIndex,position13);
	asm_vfetch(worldZ,instanceIndex,position14);
	asm_vfetch(worldW,instanceIndex,position15);

	float4x4 instanceWorld = float4x4(worldX,worldY,worldZ,worldW);
	
	positionOut = mul(mul(position,instanceWorld),viewProj);
}

#endif



//--------------------------------------------------------------//
// pixel shader, returns the global colour
//--------------------------------------------------------------//
float4 Tutorial16_PS() : COLOR 
{
	return colour;
}





//--------------------------------------------------------------//
// Technique that uses the shaders
//--------------------------------------------------------------//
technique Tutorial16
{
   pass
   {
		VertexShader = compile vs_2_0 Tutorial16_VS();
		PixelShader = compile ps_2_0 Tutorial16_PS();
   }
}
//--------------------------------------------------------------//
// Technique that uses the instancing version of the shaders
//--------------------------------------------------------------//
technique Tutorial16_Instance
{
   pass
   {
		VertexShader = compile vs_2_0 Tutorial16_VS_Instance();
		PixelShader = compile ps_2_0 Tutorial16_PS();
   }
}