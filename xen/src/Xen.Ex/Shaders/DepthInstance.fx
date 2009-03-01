//CompilerOptions = ParentNamespace

#include <asm_vfetch>

float4x4 worldViewProj	: WORLDVIEWPROJECTION : register(c0);
float4x4 viewProj		: VIEWPROJECTION : register(c0);
float4x4 worldMatrix	: WORLD : register(c4);

float4 viewDirection	: VIEWDIRECTION : register(c8);
float4 viewPoint		: VIEWPOINT : register(c9);
float2 cameraNearFar	: CAMERANEARFAR : register(c10);

#ifdef XBOX360

int VertexCount			: VERTEXCOUNT : register(c11);

#endif

float4 blendMatrices[72*3] : register(c12);


texture2D AlphaTexture			: register(t0);
sampler2D AlphaTextureSampler	: register(s0) = sampler_state
{
	Texture = (AlphaTexture);
};



#ifdef XBOX360

void DepthOutVS_instance(
			int		index		: INDEX,
			
		out	float4	positionOut	: POSITION,
		out	float	depth		: TEXCOORD0)
{
    int vertexIndex = fmod(index + 0.5, VertexCount);
    int instanceIndex = (index + 0.5) / VertexCount;
    
    float4 positionIn;
	asm_vfetch(positionIn,vertexIndex,position0);
	
    float4 worldX,worldY,worldZ,worldW;
    
	asm_vfetch(worldX,instanceIndex,position12);
	asm_vfetch(worldY,instanceIndex,position13);
	asm_vfetch(worldZ,instanceIndex,position14);
	asm_vfetch(worldW,instanceIndex,position15);
	
	float4x4 instanceWorldMatrix = float4x4(worldX,worldY,worldZ,worldW);
	float4 worldPosition = mul(positionIn,instanceWorldMatrix);
				   
	positionOut = mul(worldPosition,viewProj);
	
	depth = dot(viewDirection.xyz,worldPosition.xyz - viewPoint.xyz);
	depth = (depth - cameraNearFar.x) / (cameraNearFar.y - cameraNearFar.x);
}


void DepthOutVS_instance_tex(
			int		index		: INDEX,
	
		out	float4	texCoordOut	: TEXCOORD1,
		
		out	float4	positionOut	: POSITION,
		out	float	depth		: TEXCOORD0)
{
    int vertexIndex = fmod(index + 0.5, VertexCount);
    int instanceIndex = (index + 0.5) / VertexCount;
    
    float4 positionIn;
	asm_vfetch(positionIn,vertexIndex,position0);
	asm_vfetch(texCoordOut,instanceIndex,texcoord0);
	
    float4 worldX,worldY,worldZ,worldW;
    
	asm_vfetch(worldX,instanceIndex,position12);
	asm_vfetch(worldY,instanceIndex,position13);
	asm_vfetch(worldZ,instanceIndex,position14);
	asm_vfetch(worldW,instanceIndex,position15);
	
	
	float4x4 instanceWorldMatrix = float4x4(worldX,worldY,worldZ,worldW);
	float4 worldPosition = mul(positionIn,instanceWorldMatrix);
				   
	positionOut = mul(worldPosition,viewProj);
	
	depth = dot(viewDirection.xyz,worldPosition.xyz - viewPoint.xyz);
	depth = (depth - cameraNearFar.x) / (cameraNearFar.y - cameraNearFar.x);
}

#else

void DepthOutVS_instance(
			float4	positionIn	: POSITION,
		out	float4	positionOut	: POSITION,
		out	float	depth		: TEXCOORD0,
			float4	worldX		: POSITION12,
			float4	worldY		: POSITION13,
			float4	worldZ		: POSITION14,
			float4	worldW		: POSITION15)
{
	float4x4 instanceWorldMatrix = float4x4(worldX,worldY,worldZ,worldW);
	float4 worldPosition = mul(positionIn,instanceWorldMatrix);
				   
	positionOut = mul(worldPosition,viewProj);
	
	depth = dot(viewDirection.xyz,worldPosition.xyz - viewPoint.xyz);
	depth = (depth - cameraNearFar.x) / (cameraNearFar.y - cameraNearFar.x);
}

void DepthOutVS_instance_tex(
			float4	positionIn	: POSITION,
		out	float4	positionOut	: POSITION,
		
			float2	texCoordIn	: TEXCOORD0,
		out	float2	texCoordOut	: TEXCOORD1,
		
		out	float	depth		: TEXCOORD0,
			float4	worldX		: POSITION12,
			float4	worldY		: POSITION13,
			float4	worldZ		: POSITION14,
			float4	worldW		: POSITION15)
{
	texCoordOut = texCoordIn;
	
	float4x4 instanceWorldMatrix = float4x4(worldX,worldY,worldZ,worldW);
	float4 worldPosition = mul(positionIn,instanceWorldMatrix);
				   
	positionOut = mul(worldPosition,viewProj);
	
	depth = dot(viewDirection.xyz,worldPosition.xyz - viewPoint.xyz);
	depth = (depth - cameraNearFar.x) / (cameraNearFar.y - cameraNearFar.x);
}

#endif


float4 DepthOutPS(float depth : TEXCOORD0) : COLOR0
{
	return float4(depth,depth*depth,0,1);
}

float4 DepthOutPSAlpha(float depth : TEXCOORD0, float2 texCoord : TEXCOORD1) : COLOR0
{
	return float4(depth,depth*depth,0,tex2D(AlphaTextureSampler,texCoord).a);
}



#ifdef XBOX360
#define VS_TARGET vs_3_0
#define PS_TARGET ps_3_0
#else
#define VS_TARGET vs_2_0
#define PS_TARGET ps_2_0
#endif


technique DepthOutRgInstance
{
	pass
	{
		VertexShader = compile VS_TARGET DepthOutVS_instance();
		PixelShader = compile PS_TARGET DepthOutPS();
	}
}

technique DepthOutRgTextureAlphaInstance
{
	pass
	{
		VertexShader = compile VS_TARGET DepthOutVS_instance_tex();
		PixelShader = compile PS_TARGET DepthOutPSAlpha();
	}
}