//CompilerOptions = NoPreShader, InternalClass, UseAsmToHlslXboxConverter, AvoidFlowControl, PoolShaderBytes, DefinePlatform, ParentNamespace
//ConstantOverride = vs0tcb:vs0b,vs0tb,vs0cb;vs1tcb:vs1b,vs1tb,vs1cb;vs3tcb:vs3b,vs3tb,vs3cb;vs6tcb:vs6b,vs6tb,vs6cb;	vs0ncb:vs0nb;vs1ncb:vs1nb;vs3ncb:vs3nb;vs6ncb:vs6nb;	vs0ps0tcb:vs0ps0b,vs0ps0cb,vs0ps0tb;

/*
//full list of extensions:

vs0tcb:vs0b,vs0tb,vs0cb;
vs1tcb:vs1b,vs1tb,vs1cb;
vs3tcb:vs3b,vs3tb,vs3cb;
vs6tcb:vs6b,vs6tb,vs6cb;

vs0ncb:vs0nb;
vs1ncb:vs1nb;
vs3ncb:vs3nb;
vs6ncb:vs6nb;

vs0ps0tcb:vs0ps0b,vs0ps0cb,vs0ps0tb;
*/



//enforce register consistency so shaders can be shared
//most registers/samplers etc are enforced to keep registers consistent between shaders, so they can be merged.


float4x4 world : WORLD : register(c0);
float4x4 worldViewProj : WORLDVIEWPROJECTION : register(c4);
float4x4 viewProj : VIEWPROJECTION : register(c4);

float4 viewPoint : VIEWPOINT : register(c8);
float4 ambient : register(c9);


texture2D CustomTexture : register(t0);
sampler2D CustomTextureSampler : register(s0) = sampler_state
{
	Texture = (CustomTexture);
};
texture2D CustomNormalMap : register(t1);
sampler2D CustomNormalMapSampler : register(s1) = sampler_state
{
	Texture = (CustomNormalMap);
};


//format is position,specular,diffuse,attenuation

float4 v_lights[24] : register(c10); //max 6
float4 p_lights[16] : register(c0); //max 4

//blend matrices, stored at the end of the VS, max is 71. stored as float4x3

float4 blendMatrices[72*3] : register(c34);



void DudVS(out float4 pos_out : POSITION)
{
	// use all the registers needed by the main shaders (having preshaders enabled would break this)
	pos_out = v_lights[23] + ambient + world._m30_m31_m32_m33 + worldViewProj._m30_m31_m32_m33;
	pos_out.xyz += viewPoint;
}




void VS_blend(
			float4 pos_in : POSITION,
			float3 normal : NORMAL,
			out float4 pos_out : POSITION,
			
			float4 vcolour : COLOR,
			float2 tex0 : TEXCOORD0,
			
			out float4 colour : TEXCOORD0,
			out float2 tex : TEXCOORD2,
			out float4 onormal : TEXCOORD3,
			out float4 wpos : TEXCOORD4,
			out float4 vcolour_out : TEXCOORD1,
			
			out float4 viewPointOut : TEXCOORD5,
					
			float4 weights : BLENDWEIGHT,
			int4 indices : BLENDINDICES,
			
			uniform int vsLightCount,
			uniform bool useTextures,
			uniform bool useVertexColours,
			uniform bool outputNormals)
{

	float4x3 blendMatrix
				 = transpose(float3x4(
					blendMatrices[indices.x*3+0] * weights.x + blendMatrices[indices.y*3+0] * weights.y + blendMatrices[indices.z*3+0] * weights.z + blendMatrices[indices.w*3+0] * weights.w,
					blendMatrices[indices.x*3+1] * weights.x + blendMatrices[indices.y*3+1] * weights.y + blendMatrices[indices.z*3+1] * weights.z + blendMatrices[indices.w*3+1] * weights.w,
					blendMatrices[indices.x*3+2] * weights.x + blendMatrices[indices.y*3+2] * weights.y + blendMatrices[indices.z*3+2] * weights.z + blendMatrices[indices.w*3+2] * weights.w
				   ));
				   
	float3 worldPosition =	mul(pos_in,blendMatrix);  
	wpos = mul(float4(worldPosition,1),world);
	
	pos_out = mul(wpos,viewProj);
	
	tex = 0;
	if (useTextures)
		tex = tex0;
	colour = ambient;
	normal = normalize(mul(mul(normal,blendMatrix).xyz,(float3x3)world).xyz);
	
	onormal = 0;
	if (outputNormals)
		onormal.xyz = normal;
	
	viewPointOut = viewPoint;
	float3 viewNorm = normalize(viewPoint.xyz - wpos.xyz);
	float3 vertexColours = 0;
	
	for(int n=0; n<vsLightCount; n++)
	{
		float4 lightPosition = v_lights[n*4];
		float4 specularColour = v_lights[n*4+1];
		float3 diffuseColour = v_lights[n*4+2].xyz;
		float4 attenuation = v_lights[n*4+3];
		
		float3 lightDif = lightPosition.xyz - wpos.xyz*lightPosition.w;
		float len2 = dot(lightDif,lightDif);
		float len = sqrt(len2);
		
		lightDif /= len;
		
		float diffuse = saturate(dot(lightDif,normal));
		float specular = saturate(dot(normalize(lightDif+viewNorm),normal));
		
		float falloff = 1.0 / (attenuation.x + len * attenuation.y + len2 * attenuation.z);
		
		specular = pow(specular,specularColour.w);
		
		diffuse *= falloff;
		specular *= falloff;
		
		vertexColours += specularColour.xyz * specular + diffuseColour * diffuse;
		
	}
	
	colour.xyz += vertexColours;
	
	vcolour_out = 1;
	if (useVertexColours)
	{
		vcolour_out = vcolour;
	}
}


void VS_norm_blend(
			float4 pos_in : POSITION,
			float3 normal : NORMAL, // z
			float3 binorm : BINORMAL, // y
			float3 tangent : TANGENT, // x
			
			out float4 pos_out : POSITION,
			
			float4 vcolour : COLOR,
			float2 tex0 : TEXCOORD0,
			
			out float4 colour : TEXCOORD0,
			out float2 tex : TEXCOORD2,
			out float4 wpos : TEXCOORD3,
			out float4 vcolour_out : TEXCOORD1,
			
			out float4 onormal : TEXCOORD4,
			out float4 obinormal : TEXCOORD5,
			out float4 otangent : TEXCOORD6,
			
			float4 weights : BLENDWEIGHT,
			float4 indices : BLENDINDICES,

			out float4 viewPointOut : TEXCOORD7,
			
			uniform int vsLightCount,
			uniform bool useVertexColours)
{

	float4x3 blendMatrix
				 = transpose(float3x4(
					blendMatrices[indices.x*3+0] * weights.x + blendMatrices[indices.y*3+0] * weights.y + blendMatrices[indices.z*3+0] * weights.z + blendMatrices[indices.w*3+0] * weights.w,
					blendMatrices[indices.x*3+1] * weights.x + blendMatrices[indices.y*3+1] * weights.y + blendMatrices[indices.z*3+1] * weights.z + blendMatrices[indices.w*3+1] * weights.w,
					blendMatrices[indices.x*3+2] * weights.x + blendMatrices[indices.y*3+2] * weights.y + blendMatrices[indices.z*3+2] * weights.z + blendMatrices[indices.w*3+2] * weights.w
				   ));
				   		   
	float3 worldPosition =	mul(pos_in,blendMatrix);  
	wpos = float4(mul(float4(worldPosition,1),world).xyz,1);
	
	pos_out = mul(wpos,viewProj);
	
	tex = tex0;
	colour = ambient;
	normal = normalize(mul(mul(normal,(float3x3)blendMatrix).xyz,(float3x3)world).xyz);

	onormal = float4(normal,0);
	obinormal = float4(normalize(mul(mul(binorm,(float3x3)blendMatrix).xyz,(float3x3)world).xyz),0);
	otangent = float4(normalize(mul(mul(tangent,(float3x3)blendMatrix).xyz,(float3x3)world).xyz),0);
	
	
	viewPointOut = viewPoint;
	float3 viewNorm = normalize(viewPoint.xyz - wpos);
	
	float3 vertexColours = 0;
	for(int n=0; n<vsLightCount; n++)
	{
		float4 lightPosition = v_lights[n*4];
		float4 specularColour = v_lights[n*4+1];
		float3 diffuseColour = v_lights[n*4+2].xyz;
		float4 attenuation = v_lights[n*4+3];
		
		float3 lightDif = lightPosition.xyz - wpos*lightPosition.w;
		float len2 = dot(lightDif,lightDif);
		float len = sqrt(len2);
		
		lightDif /= len;
		
		float diffuse = saturate(dot(lightDif,normal));
		float specular = saturate(dot(normalize(lightDif+viewNorm),normal));
		
		float falloff = 1.0 / (attenuation.x + len * attenuation.y + len2 * attenuation.z);
		
		specular = pow(specular,specularColour.w);
		
		diffuse *= falloff;
		specular *= falloff;
		
		vertexColours += specularColour.xyz * specular + diffuseColour * diffuse;
		
	}
	colour.xyz += vertexColours;
	
	vcolour_out = 1;
	if (useVertexColours)
	{
		vcolour_out = vcolour;
	}
}




float4 PS0(
			float4 vcolour : TEXCOORD1,
			float4 colour : TEXCOORD0,
			float2 tex : TEXCOORD2,
			const uniform bool useTextures) : COLOR
{	
	float4 textureSample = 1;
	
	if (useTextures)
		textureSample = tex2D(CustomTextureSampler,tex);
	
	return textureSample * colour * vcolour;
}



float4 DudPS() : COLOR
{
	return p_lights[15];
}

float4 DudPStex() : COLOR
{
	return tex2D(CustomTextureSampler,p_lights[15].xy) * tex2D(CustomNormalMapSampler,p_lights[15].zy);
}


//various shaders used by the material shader system.
//these shaders are merged, so the litterally 1,000s of combinations don't need to be created
//'dud' shaders are used in place of the full equivalents (the duds still access the same constants/samples/etc)


#ifdef XBOX360
//sm3 saves the odd instruction
#define VS_TARGET vs_3_0
#define PS_TARGET ps_3_0
#else
#define VS_TARGET vs_2_0
#define PS_TARGET ps_2_0
#endif



technique vs0ps0b	{pass{
		VertexShader = compile VS_TARGET VS_blend(0,false,false,false);
		PixelShader = compile PS_TARGET PS0(false);
}}
technique vs0ps0tb	{pass{
		VertexShader = compile VS_TARGET VS_blend(0,true,false,false);
		PixelShader = compile PS_TARGET PS0(true);
}}
technique vs0ps0cb	{pass{
		VertexShader = compile VS_TARGET VS_blend(0,false,true,false);
		PixelShader = compile PS_TARGET PS0(false);
}}
technique vs0ps0tcb	{pass{
		VertexShader = compile VS_TARGET VS_blend(0,true,true,false);
		PixelShader = compile PS_TARGET PS0(true);
}}

///
///
///
///	Blending VS
///
///
///










technique vs0b	{pass{
		VertexShader = compile VS_TARGET VS_blend(0,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs1b	{pass{
		VertexShader = compile VS_TARGET VS_blend(1,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs3b	{pass{
		VertexShader = compile VS_TARGET VS_blend(3,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs6b	{pass{
		VertexShader = compile VS_TARGET VS_blend(6,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}

/// textures

technique vs0tb	{pass{
		VertexShader = compile VS_TARGET VS_blend(0,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1tb	{pass{
		VertexShader = compile VS_TARGET VS_blend(1,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3tb	{pass{
		VertexShader = compile VS_TARGET VS_blend(3,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6tb	{pass{
		VertexShader = compile VS_TARGET VS_blend(6,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}


///
/// -- vertex colours
///

technique vs0cb	{pass{
		VertexShader = compile VS_TARGET VS_blend(0,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs1cb	{pass{
		VertexShader = compile VS_TARGET VS_blend(1,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs3cb	{pass{
		VertexShader = compile VS_TARGET VS_blend(3,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs6cb	{pass{
		VertexShader = compile VS_TARGET VS_blend(6,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}

/// textures

technique vs0tcb	{pass{
		VertexShader = compile VS_TARGET VS_blend(0,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1tcb	{pass{
		VertexShader = compile VS_TARGET VS_blend(1,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3tcb	{pass{
		VertexShader = compile VS_TARGET VS_blend(3,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6tcb	{pass{
		VertexShader = compile VS_TARGET VS_blend(6,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}



/// normals

technique vs0ncb	{pass{
		VertexShader = compile VS_TARGET VS_norm_blend(0,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1ncb	{pass{
		VertexShader = compile VS_TARGET VS_norm_blend(1,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3ncb	{pass{
		VertexShader = compile VS_TARGET VS_norm_blend(3,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6ncb	{pass{
		VertexShader = compile VS_TARGET VS_norm_blend(6,true);
		PixelShader = compile PS_TARGET DudPStex();
}}

technique vs0nb	{pass{
		VertexShader = compile VS_TARGET VS_norm_blend(0,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1nb	{pass{
		VertexShader = compile VS_TARGET VS_norm_blend(1,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3nb	{pass{
		VertexShader = compile VS_TARGET VS_norm_blend(3,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6nb	{pass{
		VertexShader = compile VS_TARGET VS_norm_blend(6,false);
		PixelShader = compile PS_TARGET DudPStex();
}}


