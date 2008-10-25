//CompilerOptions = NoPreShader, InternalClass, AvoidFlowControl, PoolShaderBytes, DefinePlatform, ParentNamespace
//ConstantOverride = vs0tci:vs0ci,vs0ti,vs0i;vs1tci:vs1ci,vs1ti,vs1i;vs3tci:vs3ci,vs3ti,vs3i;vs6tci:vs6ci,vs6ti,vs6i;	vs0nci:vs0ni;vs1nci:vs1ni;vs3nci:vs3ni;vs6nci:vs6ni;	vs0ps0tci:vs0ps0i,vs0ps0ti,vs0ps0ci;

#include <asm_vfetch>

//'#include <asm_vfetch>' imports the asm_vfetch macro for using vfetch on the xbox.
//see the shader used in tutorial 16 for more details

/*
//full list of extensions:
vs0tci:vs0ci,vs0ti,vs0i;
vs1tci:vs1ci,vs1ti,vs1i;
vs3tci:vs3ci,vs3ti,vs3i;
vs6tci:vs6ci,vs6ti,vs6i;

vs0nci:vs0ni;
vs1nci:vs1ni;
vs3nci:vs3ni;
vs6nci:vs6ni;

vs0ps0tci:vs0ps0i,vs0ps0ti,vs0ps0ci;
*/





//enforce register consistency so shaders can be shared
//most registers/samplers etc are enforced to keep registers consistent between shaders, so they can be merged.


#ifdef XBOX360
		
//bug? for some reason can't use register 8-11 on xbox
float4 viewPoint : VIEWPOINT : register(c12);
float4 ambient : register(c13);

//but can use it for vcount
int VertexCount : VERTEXCOUNT : register(c11);

#else

float4 viewPoint : VIEWPOINT : register(c8);
float4 ambient : register(c9);

#endif

float4x4 world : WORLD : register(c0);
float4x4 worldViewProj : WORLDVIEWPROJECTION : register(c4);
float4x4 viewProj : VIEWPROJECTION : register(c4);

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

#ifdef XBOX360
float4 v_lights[24] : register(c14); //max 6
#else
float4 v_lights[24] : register(c10); //max 6
#endif

float4 p_lights[16] : register(c0); //max 4


void DudVS(out float4 pos_out : POSITION)
{
	// use all the registers needed by the main shaders (having preshaders enabled would break this)
	pos_out = v_lights[23] + ambient + world._m30_m31_m32_m33 + worldViewProj._m30_m31_m32_m33;
	pos_out.xyz += viewPoint;
}


#ifndef XBOX360

void VS_instance(
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
					
			float4 worldX : POSITION12,
			float4 worldY : POSITION13,
			float4 worldZ : POSITION14,
			float4 worldW : POSITION15,
			
			uniform int vsLightCount,
			uniform bool useTextures,
			uniform bool useVertexColours,
			uniform bool outputNormals)
{
	float4x4 worldMatrix = float4x4(worldX,worldY,worldZ,worldW);
				   
	float3 worldPosition =	mul(pos_in,worldMatrix);  
	wpos = mul(float4(worldPosition,1),world);
	
	pos_out = mul(wpos,viewProj);
	
	tex = 0;
	if (useTextures)
		tex = tex0;
	colour = ambient;
	normal = normalize(mul(mul(normal,worldMatrix).xyz,(float3x3)world).xyz);
	
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

#else

void VS_instance(
			int index : INDEX,
			
			out float4 pos_out : POSITION,
			
			out float4 colour : TEXCOORD0,
			out float2 tex : TEXCOORD2,
			out float4 onormal : TEXCOORD3,
			out float4 wpos : TEXCOORD4,
			out float4 vcolour_out : TEXCOORD1,
			
			out float4 viewPointOut : TEXCOORD5,
			
			uniform int vsLightCount,
			uniform bool useTextures,
			uniform bool useVertexColours,
			uniform bool outputNormals)
{
    int vertexIndex = fmod(index + 0.5, VertexCount);
    int instanceIndex = (index + 0.5) / VertexCount;
    
	float4 pos_in;// : POSITION,
	float4 normal = 0;// : NORMAL,
	float4 vcolour = 0;// : COLOR,
	float4 tex0 = 0;// : TEXCOORD0,
	float4 worldX;// : POSITION12,
	float4 worldY;// : POSITION13,
	float4 worldZ;// : POSITION14,
	float4 worldW;// : POSITION15,
    
	asm_vfetch(pos_in,vertexIndex,position0);
	
	if (outputNormals)
	{
		asm_vfetch(normal,vertexIndex,normal0);
	}
	if (useVertexColours)
	{
		asm_vfetch(vcolour,vertexIndex,color0);
	}
	if (useTextures)
	{
		asm_vfetch(tex0,vertexIndex,texcoord0);
	}
	
	asm_vfetch(worldX,instanceIndex,position12);
	asm_vfetch(worldY,instanceIndex,position13);
	asm_vfetch(worldZ,instanceIndex,position14);
	asm_vfetch(worldW,instanceIndex,position15);
	
	float4x4 worldMatrix = float4x4(worldX,worldY,worldZ,worldW);
				   
	float3 worldPosition =	mul(pos_in,worldMatrix);  
	wpos = mul(float4(worldPosition,1),world);
	
	pos_out = mul(wpos,viewProj);
	
	tex = 0;
	if (useTextures)
		tex = tex0.xy;
	colour = ambient;
	normal.xyz = normalize(mul(mul(normal.xyz,worldMatrix).xyz,(float3x3)world).xyz);
	
	onormal = 0;
	if (outputNormals)
		onormal.xyz = normal.xyz;
	
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
		
		float diffuse = saturate(dot(lightDif,normal.xyz));
		float specular = saturate(dot(normalize(lightDif+viewNorm),normal.xyz));
		
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

#endif



#ifndef XBOX360

void VS_norm_instance(
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
			
			float4 worldX : POSITION12,
			float4 worldY : POSITION13,
			float4 worldZ : POSITION14,
			float4 worldW : POSITION15,

			out float4 viewPointOut : TEXCOORD7,
			
			uniform int vsLightCount,
			uniform bool useVertexColours)
{

	float4x4 worldMatrix = float4x4(worldX,worldY,worldZ,worldW);
				   		   
	float3 worldPosition =	mul(pos_in,worldMatrix);  
	wpos = float4(mul(float4(worldPosition,1),world).xyz,1);
	
	pos_out = mul(wpos,viewProj);
	
	tex = tex0;
	colour = ambient;
	normal = normalize(mul(mul(normal,(float3x3)worldMatrix).xyz,(float3x3)world).xyz);

	onormal = float4(normal,0);
	obinormal = float4(normalize(mul(mul(binorm,(float3x3)worldMatrix).xyz,(float3x3)world).xyz),0);
	otangent = float4(normalize(mul(mul(tangent,(float3x3)worldMatrix).xyz,(float3x3)world).xyz),0);
	
	
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

#else

void VS_norm_instance(
			int index : INDEX,	
			
			out float4 pos_out : POSITION,
			
			out float4 colour : TEXCOORD0,
			out float2 tex : TEXCOORD2,
			out float4 wpos : TEXCOORD3,
			out float4 vcolour_out : TEXCOORD1,
			
			out float4 onormal : TEXCOORD4,
			out float4 obinormal : TEXCOORD5,
			out float4 otangent : TEXCOORD6,
			
			out float4 viewPointOut : TEXCOORD7,
			
			uniform int vsLightCount,
			uniform bool useVertexColours)
{
    int vertexIndex = fmod(index + 0.5, VertexCount);
    int instanceIndex = (index + 0.5) / VertexCount;
    
	float4 pos_in;// : POSITION,
	float4 normal;// : NORMAL, // z
	float4 binorm;// : BINORMAL, // y
	float4 tangent;// : TANGENT, // x
	
	float4 vcolour = 0;// : COLOR,
	float4 tex0;// : TEXCOORD0,
	
	float4 worldX;// : POSITION12,
	float4 worldY;// : POSITION13,
	float4 worldZ;// : POSITION14,
	float4 worldW;// : POSITION15,

    
	asm_vfetch(pos_in,vertexIndex,position0);
	asm_vfetch(normal,vertexIndex,normal0);
	asm_vfetch(binorm,vertexIndex,binormal0);
	asm_vfetch(tangent,vertexIndex,tangent0);
	asm_vfetch(tex0,vertexIndex,texcoord0);
	
	if (useVertexColours)
	{
		asm_vfetch(vcolour,vertexIndex,color0);
	}
	
	asm_vfetch(worldX,instanceIndex,position12);
	asm_vfetch(worldY,instanceIndex,position13);
	asm_vfetch(worldZ,instanceIndex,position14);
	asm_vfetch(worldW,instanceIndex,position15);
	
	
	float4x4 worldMatrix = float4x4(worldX,worldY,worldZ,worldW);
				   		   
	float3 worldPosition =	mul(pos_in,worldMatrix);  
	wpos = float4(mul(float4(worldPosition,1),world).xyz,1);
	
	pos_out = mul(wpos,viewProj);
	
	tex = tex0.xy;
	colour = ambient;
	normal.xyz = normalize(mul(mul(normal.xyz,(float3x3)worldMatrix).xyz,(float3x3)world).xyz);

	onormal = float4(normal.xyz,0);
	obinormal = float4(normalize(mul(mul(binorm.xyz,(float3x3)worldMatrix).xyz,(float3x3)world).xyz),0);
	otangent = float4(normalize(mul(mul(tangent.xyz,(float3x3)worldMatrix).xyz,(float3x3)world).xyz),0);
	
	
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
		
		float diffuse = saturate(dot(lightDif,normal.xyz));
		float specular = saturate(dot(normalize(lightDif+viewNorm),normal.xyz));
		
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

#endif



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


technique vs0ps0i	{pass{
		VertexShader = compile VS_TARGET VS_instance(0,false,false,false);
		PixelShader = compile PS_TARGET PS0(false);
}}
technique vs0ps0ti	{pass{
		VertexShader = compile VS_TARGET VS_instance(0,true,false,false);
		PixelShader = compile PS_TARGET PS0(true);
}}
technique vs0ps0ci	{pass{
		VertexShader = compile VS_TARGET VS_instance(0,false,true,false);
		PixelShader = compile PS_TARGET PS0(false);
}}
technique vs0ps0tci	{pass{
		VertexShader = compile VS_TARGET VS_instance(0,true,true,false);
		PixelShader = compile PS_TARGET PS0(true);
}}



//---------------



///
///
///
///	Instancing VS
///
///
///




technique vs0i	{pass{
		VertexShader = compile VS_TARGET VS_instance(0,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs1i	{pass{
		VertexShader = compile VS_TARGET VS_instance(1,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs3i	{pass{
		VertexShader = compile VS_TARGET VS_instance(3,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs6i	{pass{
		VertexShader = compile VS_TARGET VS_instance(6,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}

/// textures

technique vs0ti	{pass{
		VertexShader = compile VS_TARGET VS_instance(0,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1ti	{pass{
		VertexShader = compile VS_TARGET VS_instance(1,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3ti	{pass{
		VertexShader = compile VS_TARGET VS_instance(3,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6ti	{pass{
		VertexShader = compile VS_TARGET VS_instance(6,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}


///
/// -- vertex colours
///

technique vs0ci	{pass{
		VertexShader = compile VS_TARGET VS_instance(0,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs1ci	{pass{
		VertexShader = compile VS_TARGET VS_instance(1,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs3ci	{pass{
		VertexShader = compile VS_TARGET VS_instance(3,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs6ci	{pass{
		VertexShader = compile VS_TARGET VS_instance(6,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}

/// textures

technique vs0tci	{pass{
		VertexShader = compile VS_TARGET VS_instance(0,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1tci	{pass{
		VertexShader = compile VS_TARGET VS_instance(1,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3tci	{pass{
		VertexShader = compile VS_TARGET VS_instance(3,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6tci	{pass{
		VertexShader = compile VS_TARGET VS_instance(6,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}



/// normals

technique vs0nci	{pass{
		VertexShader = compile VS_TARGET VS_norm_instance(0,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1nci	{pass{
		VertexShader = compile VS_TARGET VS_norm_instance(1,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3nci	{pass{
		VertexShader = compile VS_TARGET VS_norm_instance(3,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6nci	{pass{
		VertexShader = compile VS_TARGET VS_norm_instance(6,true);
		PixelShader = compile PS_TARGET DudPStex();
}}

technique vs0ni	{pass{
		VertexShader = compile VS_TARGET VS_norm_instance(0,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1ni	{pass{
		VertexShader = compile VS_TARGET VS_norm_instance(1,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3ni	{pass{
		VertexShader = compile VS_TARGET VS_norm_instance(3,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6ni	{pass{
		VertexShader = compile VS_TARGET VS_norm_instance(6,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
