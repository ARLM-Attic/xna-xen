//CompilerOptions = ParentNamespace

float4x4 worldViewProj	: WORLDVIEWPROJECTION : register(c0);
float4x4 viewProj		: VIEWPROJECTION : register(c0);
float4x4 worldMatrix	: WORLD : register(c4);

float3 viewDirection	: VIEWDIRECTION : register(c8);
float3 viewPoint		: VIEWPOINT : register(c9);
float2 cameraNearFar	: CAMERANEARFAR : register(c10);
float4 blendMatrices[72*3] : register(c12);


texture2D AlphaTexture			: register(t0);
sampler2D AlphaTextureSampler	: register(s0) = sampler_state
{
	Texture = (AlphaTexture);
};


void DepthOutVS(
			float4	positionIn	: POSITION,
		out	float4	positionOut	: POSITION,
		out	float	depth		: TEXCOORD0)
{
	positionOut = mul(positionIn,worldViewProj);
	
	float3 worldPoint = mul(positionIn,worldMatrix).xyz - viewPoint.xyz;
	
	depth = dot(viewDirection.xyz, worldPoint);
	depth = (depth - cameraNearFar.x) / (cameraNearFar.y - cameraNearFar.x);
}

void DepthOutVS_blend(
			float4	positionIn	: POSITION,
		out	float4	positionOut	: POSITION,
		out	float	depth		: TEXCOORD0,
					
			float4	weights		: BLENDWEIGHT,
			int4	indices		: BLENDINDICES)
{
	float4x3 blendMatrix
				 = transpose(float3x4(
					blendMatrices[indices.x*3+0] * weights.x + blendMatrices[indices.y*3+0] * weights.y + blendMatrices[indices.z*3+0] * weights.z + blendMatrices[indices.w*3+0] * weights.w,
					blendMatrices[indices.x*3+1] * weights.x + blendMatrices[indices.y*3+1] * weights.y + blendMatrices[indices.z*3+1] * weights.z + blendMatrices[indices.w*3+1] * weights.w,
					blendMatrices[indices.x*3+2] * weights.x + blendMatrices[indices.y*3+2] * weights.y + blendMatrices[indices.z*3+2] * weights.z + blendMatrices[indices.w*3+2] * weights.w
				   ));
				   
	float4 blendPosition =	float4(mul(positionIn,blendMatrix).xyz,1); 
	
	positionOut = mul(blendPosition,worldViewProj);
	
	float3 worldPoint = mul(blendPosition,worldMatrix).xyz - viewPoint.xyz;
	
	depth = dot(viewDirection.xyz,worldPoint);
	depth = (depth - cameraNearFar.x) / (cameraNearFar.y - cameraNearFar.x);
}


void DepthOutVS_texCoord(
			float4	positionIn	: POSITION,
		out	float4	positionOut	: POSITION,
		
			float2	texCoordIn	: TEXCOORD0,
		out	float2	texCoordOut	: TEXCOORD1,
		
		out	float	depth		: TEXCOORD0)
{
	texCoordOut = texCoordIn;
	
	positionOut = mul(positionIn,worldViewProj);
	
	float3 worldPoint = mul(positionIn,worldMatrix).xyz - viewPoint.xyz;
	
	depth = dot(viewDirection.xyz, worldPoint);
	depth = (depth - cameraNearFar.x) / (cameraNearFar.y - cameraNearFar.x);
}

void DepthOutVS_blend_texCoord(
			float4	positionIn	: POSITION,
		out	float4	positionOut	: POSITION,
		out	float	depth		: TEXCOORD0,
		
			float2	texCoordIn	: TEXCOORD0,
		out	float2	texCoordOut	: TEXCOORD1,
					
			float4	weights		: BLENDWEIGHT,
			int4	indices		: BLENDINDICES)
{
	texCoordOut = texCoordIn;
	
	float4x3 blendMatrix
				 = transpose(float3x4(
					blendMatrices[indices.x*3+0] * weights.x + blendMatrices[indices.y*3+0] * weights.y + blendMatrices[indices.z*3+0] * weights.z + blendMatrices[indices.w*3+0] * weights.w,
					blendMatrices[indices.x*3+1] * weights.x + blendMatrices[indices.y*3+1] * weights.y + blendMatrices[indices.z*3+1] * weights.z + blendMatrices[indices.w*3+1] * weights.w,
					blendMatrices[indices.x*3+2] * weights.x + blendMatrices[indices.y*3+2] * weights.y + blendMatrices[indices.z*3+2] * weights.z + blendMatrices[indices.w*3+2] * weights.w
				   ));
				   
	float4 blendPosition =	float4(mul(positionIn,blendMatrix).xyz,1); 
	
	positionOut = mul(blendPosition,worldViewProj);
	
	float3 worldPoint = mul(blendPosition,worldMatrix).xyz - viewPoint.xyz;
	
	depth = dot(viewDirection.xyz,worldPoint);
	depth = (depth - cameraNearFar.x) / (cameraNearFar.y - cameraNearFar.x);
}





float4 DepthOutPS(float depth : TEXCOORD0) : COLOR0
{
	return float4(depth,depth*depth,0,1);
}

float4 DepthOutPSAlpha(float depth : TEXCOORD0, float2 texCoord : TEXCOORD1) : COLOR0
{
	return float4(depth,depth*depth,0,tex2D(AlphaTextureSampler,texCoord).a);
}


technique DepthOutRg
{
	pass
	{
		VertexShader = compile vs_2_0 DepthOutVS();
		PixelShader = compile ps_2_0 DepthOutPS();
	}
}

technique DepthOutRgBlend
{
	pass
	{
		VertexShader = compile vs_2_0 DepthOutVS_blend();
		PixelShader = compile ps_2_0 DepthOutPS();
	}
}



technique DepthOutRgTextureAlpha
{
	pass
	{
		VertexShader = compile vs_2_0 DepthOutVS_texCoord();
		PixelShader = compile ps_2_0 DepthOutPSAlpha();
	}
}

technique DepthOutRgTextureAlphaBlend
{
	pass
	{
		VertexShader = compile vs_2_0 DepthOutVS_blend_texCoord();
		PixelShader = compile ps_2_0 DepthOutPSAlpha();
	}
}
