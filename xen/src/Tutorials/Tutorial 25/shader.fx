//CompilerOptions = UseAsmToHlslXboxConverter


float4x4 worldViewProjection	: WORLDVIEWPROJECTION;
float4x4 viewProjection			: VIEWPROJECTION;

float4x4 shadowMapProjection;
float4x4 worldMatrix			: WORLD;

float3 shadowViewDirection;
float3 shadowViewPoint;
float2 shadowCameraNearFar;

float4 blendMatrices[71*3];


// --------------------------------
// --------------------------------
//
// This shader performs simple exponential shadow mapping
//
// --------------------------------
// --------------------------------

texture2D ShadowMap;
sampler2D ShadowSampler = sampler_state
{
	Texture = (ShadowMap);
    AddressU = Clamp;
    AddressV = Clamp;
};

texture2D TextureMap;
sampler2D TextureSampler = sampler_state
{
	Texture = (TextureMap);
    AddressU = Clamp;
    AddressV = Clamp;
};






void ShadowVS(
		float4	positionIn	: POSITION,
	out	float4	positionOut	: POSITION,
	
		float3	normalIn	: NORMAL,
	
		float2	texCoord	: TEXCOORD0,
	out	float2	texCoordOut	: TEXCOORD0,
	out	float4	shadowMap	: TEXCOORD1,
	out	float3	colour		: COLOR0
		)
{
	positionOut			= mul(positionIn, worldViewProjection);
	
	float4 worldPosition = mul(positionIn, worldMatrix);
	
	//shadow map projection
	shadowMap.xyz		= mul(worldPosition, shadowMapProjection).xyw;

	//linear depth from the camera is stored in shadowMap.w
	shadowMap.w			= dot(shadowViewDirection, worldPosition.xyz - shadowViewPoint);
	shadowMap.w			= (shadowMap.w - shadowCameraNearFar.x) / (shadowCameraNearFar.y - shadowCameraNearFar.x);
	shadowMap.w			-= 0.001f; // apply a small bias
	
	//basic lighting
	float3 normal		= mul(normalIn, worldMatrix);
	colour				= dot(normalize(normal), -shadowViewDirection);
	texCoordOut			= texCoord;
}

void ShadowVS_blend(
		float4	positionIn	: POSITION,
	out	float4	positionOut	: POSITION,
		float3	normalIn	: NORMAL,
					
		float4	weights		: BLENDWEIGHT,
		int4	indices		: BLENDINDICES,
	
		float2	texCoord	: TEXCOORD0,
	out	float2	texCoordOut	: TEXCOORD0,
	out	float4	shadowMap	: TEXCOORD1,
	out	float3	colour		: COLOR0
		)
{
	//this is a bit more complex, as animation blending is added to the equation
	
	float4x3 blendMatrix
				 = transpose(float3x4(
					blendMatrices[indices.x*3+0] * weights.x + blendMatrices[indices.y*3+0] * weights.y + blendMatrices[indices.z*3+0] * weights.z + blendMatrices[indices.w*3+0] * weights.w,
					blendMatrices[indices.x*3+1] * weights.x + blendMatrices[indices.y*3+1] * weights.y + blendMatrices[indices.z*3+1] * weights.z + blendMatrices[indices.w*3+1] * weights.w,
					blendMatrices[indices.x*3+2] * weights.x + blendMatrices[indices.y*3+2] * weights.y + blendMatrices[indices.z*3+2] * weights.z + blendMatrices[indices.w*3+2] * weights.w
				   ));
				   
	float4 blendPosition =	float4(mul(positionIn,blendMatrix).xyz,1); 
	float4 worldPosition = mul(blendPosition, worldMatrix);
	
	positionOut			= mul(worldPosition, viewProjection);
	
	shadowMap.xyz		= mul(worldPosition, shadowMapProjection).xyw;
	

	shadowMap.w			= dot(shadowViewDirection, worldPosition.xyz - shadowViewPoint);
	shadowMap.w			= (shadowMap.w - shadowCameraNearFar.x) / (shadowCameraNearFar.y - shadowCameraNearFar.x);
	shadowMap.w			-= 0.001f; // apply a small bias
	
	//basic lighting
	float3 normal		= mul(mul(normalIn,blendMatrix), worldMatrix);
	colour				= dot(normalize(normal), -shadowViewDirection);
	texCoordOut			= texCoord;
}


//the shadow pixel shader
float4 ShadowPS(float2 texCoord : TEXCOORD0, float4 shadowMapCoord : TEXCOORD1, float3 colour : COLOR0) : COLOR0
{
	//projected texture coordinate
	float2 lookupCoord = shadowMapCoord.xy / shadowMapCoord.z;
	float2 shadowCoord = lookupCoord * float2(0.5,-0.5) + 0.5;
	
	//sample the shadow map
	float occlusion = tex2D(ShadowSampler, shadowCoord).r;
	float depth = shadowMapCoord.w;
	
	//difference with real depth
	float difference = occlusion - depth;
	
	//shadow term
	float shadow = saturate(exp(difference * 100));
	
	//sample texture
	colour *= tex2D(TextureSampler, texCoord);
	colour *= shadow;
	
	return float4(colour * 1.5,1);
}




technique ShadowShader
{
   pass
   {
		VertexShader = compile vs_2_0 ShadowVS();
		PixelShader = compile ps_2_0 ShadowPS();
   }
}

technique ShadowShaderBlend
{
   pass
   {
		VertexShader = compile vs_2_0 ShadowVS_blend();
		PixelShader = compile ps_2_0 ShadowPS();
   }
}