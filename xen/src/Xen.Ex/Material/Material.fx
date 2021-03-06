//CompilerOptions = NoPreShader, InternalClass, UseAsmToHlslXboxConverter, AvoidFlowControl, PoolShaderBytes, DefinePlatform, ParentNamespace, SkipConstantValidation
//ConstantOverride = vs0tc:vs0c,vs0t,vs0;vs1tc:vs1c,vs1t,vs1;vs3tc:vs3c,vs3t,vs3;vs6tc:vs6c,vs6t,vs6;	vs0nc:vs0n;vs1nc:vs1n;vs3nc:vs3n;vs6nc:vs6n;	ps0t:ps0;ps1t:ps1;ps2t:ps2;ps3t:ps3;ps4t:ps4;	ps0ts:ps0s;ps1ts:ps1s;ps2ts:ps2s;	vs0ps0tc:vs0ps0t,vs0ps0c,vs0ps0;

//The first line of this file (always the very first line) is a shader generator hack, to set the compiler options.
//NoPreShader:		preshader adds a lot here for a 1 instruction saving per light
//InternalClass:	makes the generated shaders internal, instead of public.
//AvoidFlowControl: because of a compiler bug for vs_2_0? the VS loops shouldn't use flow control anyway..
//PoolShaderBytes:	tells the plugin to pool duplicate shader byte arrays (in a separate class)
//DefinePlatform:	makes the compiler generate shaders for PC and Xbox separately (used due to an xbox bug and instancing)

//The second line indicates that certain sets of shaders share the same constants, and should 
//override a base shader class (so they don't duplicate the same constant code).
//The format is:
//base:extend,extend,extend;
//This is simply to keep the size of the material source file down, due to it's huge number of combinations.
//Note that the shader code isn't validated to confirm that the constants are the same

/*
//full list of extensions:

vs0tc:vs0c,vs0t,vs0;
vs1tc:vs1c,vs1t,vs1;
vs3tc:vs3c,vs3t,vs3;
vs6tc:vs6c,vs6t,vs6;

vs0nc:vs0n;
vs1nc:vs1n;
vs3nc:vs3n;
vs6nc:vs6n;

---

ps0t:ps0;
ps1t:ps1;
ps2t:ps2;
ps3t:ps3;
ps4t:ps4;

ps0ts:ps0s;
ps1ts:ps1s;
ps2ts:ps2s;

---

vs0ps0tc:vs0ps0t,vs0ps0c,vs0ps0;
*/




//this generates the most utterly ginormous source file :-) around 25,000 lines
//adds about 250kb to the .dll (but compresses really well! :)
//if only xna supported compile_fragment... :-(

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


void DudVS(out float4 pos_out : POSITION)
{
	// use all the registers needed by the main shaders (having preshaders enabled would break this)
	pos_out = v_lights[23] + ambient + world._m30_m31_m32_m33 + worldViewProj._m30_m31_m32_m33;
	pos_out.xyz += viewPoint;
}


void VS(
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
			
			uniform int vsLightCount,
			uniform bool useTextures,
			uniform bool useVertexColours,
			uniform bool outputNormals)
{
	viewPointOut = viewPoint;
	
	pos_out = mul(pos_in,worldViewProj);
	
	tex = 0;
	if (useTextures)
		tex = tex0;
	colour = ambient;
	normal = normalize(mul(normal,(float3x3)world).xyz);
	
	onormal = 0;
		
	if (outputNormals)
		onormal.xyz = normal;
	
	wpos = mul(pos_in,world);
	
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



void VS_norm(
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
			
			out float4 viewPointOut : TEXCOORD7,
			
			uniform int vsLightCount,
			uniform bool useVertexColours)
{
	pos_out = mul(pos_in,worldViewProj);
	
	tex = tex0;
	colour = ambient;
	normal = normalize(mul(normal,(float3x3)world).xyz);

	onormal = float4(normal,0);
	obinormal = float4(normalize(mul(binorm,(float3x3)world).xyz),0);
	otangent = float4(normalize(mul(tangent,(float3x3)world).xyz),0);
	
	
	wpos = mul(pos_in,world);
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



float4 PS(
			float4 vcolour : TEXCOORD1,
			float4 colour : TEXCOORD0,
			float2 tex : TEXCOORD2,
			float3 normal : TEXCOORD3,
			float3 wpos : TEXCOORD4,
			const uniform int psLightCount,
			const uniform bool useTextures) : COLOR
{
	
	
	normal = normalize(normal);
	
	float3 pixelColour = 0;
	
	for(int n=0; n<psLightCount; n++)
	{
		float4 lightPosition = p_lights[n*4];
		float4 attenuation = p_lights[n*4+3];
		float3 lightDif = lightPosition.xyz - wpos*lightPosition.w;
		float len2 = dot(lightDif,lightDif);
		float len = sqrt(len2);
		
		float diffuse = saturate(dot(lightDif,normal) / len);
		float falloff = diffuse / (attenuation.x + len * attenuation.y + len2 * attenuation.z);
		pixelColour += p_lights[n*4+2].xyz * falloff;
	}
	
	colour.xyz += pixelColour;
	
	float4 textureSample = 1;
	
	if (useTextures)
		textureSample = tex2D(CustomTextureSampler,tex);
	
	
	return textureSample * colour * vcolour;
}


float4 PS_specular(
			float4 vcolour : TEXCOORD1,
			float4 colour : TEXCOORD0,
			float2 tex : TEXCOORD2,
			float4 normal : TEXCOORD3,
			float4 wpos : TEXCOORD4,
			float4 viewPoint : TEXCOORD5,
			const uniform int psLightCount,
			const uniform bool useTextures) : COLOR
{
	
	normal.xyz = normalize(normal.xyz);
	float3 pixelColour = 0;
	float3 viewNorm = normalize(viewPoint.xyz - wpos.xyz);
	
	for(int n=0; n<psLightCount; n++)
	{
		float4 lightPosition = p_lights[n*4];
		float4 attenuation = p_lights[n*4+3];
		float3 lightDif = lightPosition.xyz - wpos.xyz*lightPosition.w;
		float len2 = dot(lightDif,lightDif);
		float len = sqrt(len2);
	
		lightDif /= len;
		
		float diffuse = saturate(dot(lightDif,normal.xyz));
		float specular = saturate(dot(normalize(lightDif+viewNorm),normal.xyz));
		
		float falloff = 1.0 / (attenuation.x + len * attenuation.y + len2 * attenuation.z);
		
		specular = pow(specular,p_lights[n*4+1].w);
		
		diffuse *= falloff;
		specular *= falloff;
		
		pixelColour += p_lights[n*4+1].xyz * specular + p_lights[n*4+2].xyz * diffuse;
	}
	
	colour.xyz += pixelColour;
	
	float4 textureSample = 1;
	
	if (useTextures)
		textureSample = tex2D(CustomTextureSampler,tex);
	
	return textureSample * colour * vcolour;
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



float4 PS_norm(
			float4 vcolour : TEXCOORD1,
			float4 colour : TEXCOORD0,
			float2 tex : TEXCOORD2,
			float3 wpos : TEXCOORD3,
			float3 normal : TEXCOORD4,
			float3 binorm : TEXCOORD5,
			float3 tangent : TEXCOORD6,
			const uniform int psLightCount) : COLOR
{
	float4 normalMap = tex2D(CustomNormalMapSampler,tex);
	float3x3 normalSpace = float3x3(tangent,binorm,normal);
	
	normal = normalize(mul(normalMap.xyz-0.5,normalSpace));
	
	float3 pixelColour = 0;
	
	for(int n=0; n<psLightCount; n++)
	{
		float4 lightPosition = p_lights[n*4];
		float4 attenuation = p_lights[n*4+3];
		float3 lightDif = lightPosition.xyz - wpos*lightPosition.w;
		float len2 = dot(lightDif,lightDif);
		float len = sqrt(len2);
		
		float diffuse = saturate(dot(lightDif,normal) / len);
		float falloff = diffuse / (attenuation.x + len * attenuation.y + len2 * attenuation.z);
		pixelColour += p_lights[n*4+2].xyz * falloff;
	}
	float4 textureSample = tex2D(CustomTextureSampler,tex);
	colour.xyz += pixelColour;
	
	return textureSample * colour * vcolour;
}


float4 PS_norm_specular(
			float4 vcolour : TEXCOORD1,
			float4 colour : TEXCOORD0,
			float2 tex : TEXCOORD2,
			float3 wpos : TEXCOORD3,
			float3 normal : TEXCOORD4,
			float3 binorm : TEXCOORD5,
			float3 tangent : TEXCOORD6,
			float3 viewPoint : TEXCOORD7,
			const uniform int psLightCount) : COLOR
{
	float4 normalMap = tex2D(CustomNormalMapSampler,tex);
	float3x3 normalSpace = float3x3(tangent,binorm,normal);
	
	normal = normalize(mul(normalMap.xyz-0.5,normalSpace));
	float3 viewNorm = normalize(viewPoint - wpos);
	
	float3 pixelColour = 0;
	
	for(int n=0; n<psLightCount; n++)
	{
		float4 lightPosition = p_lights[n*4];
		float4 attenuation = p_lights[n*4+3];
		float3 lightDif = lightPosition.xyz - wpos*lightPosition.w;
		float len2 = dot(lightDif,lightDif);
		float len = sqrt(len2);
	
		lightDif /= len;
		
		float diffuse = saturate(dot(lightDif,normal));
		float specular = saturate(dot(normalize(lightDif+viewNorm),normal));
		
		float falloff = 1.0 / (attenuation.x + len * attenuation.y + len2 * attenuation.z);
		
		specular = pow(specular,p_lights[n*4+1].w);
		
		diffuse *= falloff;
		specular *= falloff * normalMap.w;
		
		pixelColour += p_lights[n*4+1].xyz * specular + p_lights[n*4+2].xyz * diffuse;
	}
	float4 textureSample = tex2D(CustomTextureSampler,tex);
	colour.xyz += pixelColour;
	
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


//with exception of the really simple shaders...
technique vs0ps0	{pass{
		VertexShader = compile VS_TARGET VS(0,false,false,false);
		PixelShader = compile PS_TARGET PS0(false);
}}
technique vs0ps0t	{pass{
		VertexShader = compile VS_TARGET VS(0,true,false,false);
		PixelShader = compile PS_TARGET PS0(true);
}}
technique vs0ps0c	{pass{
		VertexShader = compile VS_TARGET VS(0,false,true,false);
		PixelShader = compile PS_TARGET PS0(false);
}}
technique vs0ps0tc	{pass{
		VertexShader = compile VS_TARGET VS(0,true,true,false);
		PixelShader = compile PS_TARGET PS0(true);
}}




//---------------

technique vs0	{pass{
		VertexShader = compile VS_TARGET VS(0,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs1	{pass{
		VertexShader = compile VS_TARGET VS(1,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs3	{pass{
		VertexShader = compile VS_TARGET VS(3,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs6	{pass{
		VertexShader = compile VS_TARGET VS(6,false,false,true);
		PixelShader = compile PS_TARGET DudPS();
}}

/// textures

technique vs0t	{pass{
		VertexShader = compile VS_TARGET VS(0,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1t	{pass{
		VertexShader = compile VS_TARGET VS(1,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3t	{pass{
		VertexShader = compile VS_TARGET VS(3,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6t	{pass{
		VertexShader = compile VS_TARGET VS(6,true,false,true);
		PixelShader = compile PS_TARGET DudPStex();
}}


///
/// -- vertex colours
///

technique vs0c	{pass{
		VertexShader = compile VS_TARGET VS(0,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs1c	{pass{
		VertexShader = compile VS_TARGET VS(1,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs3c	{pass{
		VertexShader = compile VS_TARGET VS(3,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}
technique vs6c	{pass{
		VertexShader = compile VS_TARGET VS(6,false,true,true);
		PixelShader = compile PS_TARGET DudPS();
}}

/// textures

technique vs0tc	{pass{
		VertexShader = compile VS_TARGET VS(0,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1tc	{pass{
		VertexShader = compile VS_TARGET VS(1,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3tc	{pass{
		VertexShader = compile VS_TARGET VS(3,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6tc	{pass{
		VertexShader = compile VS_TARGET VS(6,true,true,true);
		PixelShader = compile PS_TARGET DudPStex();
}}



/// normals

technique vs0nc	{pass{
		VertexShader = compile VS_TARGET VS_norm(0,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1nc	{pass{
		VertexShader = compile VS_TARGET VS_norm(1,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3nc	{pass{
		VertexShader = compile VS_TARGET VS_norm(3,true);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6nc	{pass{
		VertexShader = compile VS_TARGET VS_norm(6,true);
		PixelShader = compile PS_TARGET DudPStex();
}}

technique vs0n	{pass{
		VertexShader = compile VS_TARGET VS_norm(0,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs1n	{pass{
		VertexShader = compile VS_TARGET VS_norm(1,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs3n	{pass{
		VertexShader = compile VS_TARGET VS_norm(3,false);
		PixelShader = compile PS_TARGET DudPStex();
}}
technique vs6n	{pass{
		VertexShader = compile VS_TARGET VS_norm(6,false);
		PixelShader = compile PS_TARGET DudPStex();
}}






////////////////////////////////////////////////////////////////////////////////
///
///
///  pixel shaders
///
///


technique ps0	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(0,false);
}}
technique ps1	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(1,false);
}}
technique ps2	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(2,false);
}}
technique ps3	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(3,false);
}}
technique ps4	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(4,false);
}}


technique ps0t	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(0,true);
}}
technique ps1t	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(1,true);
}}
technique ps2t	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(2,true);
}}
technique ps3t	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(3,true);
}}
technique ps4t	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS(4,true);
}}


/// normals



technique ps1tn	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_norm(1);
}}
technique ps2tn	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_norm(2);
}}
technique ps3tn	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_norm(3);
}}
technique ps4tn	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_norm(4);
}}



technique ps1s	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_specular(1,false);
}}
technique ps2s	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_specular(2,false);
}}
technique ps1ts	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_specular(1,true);
}}
technique ps2ts	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_specular(2,true);
}}
technique ps1tns	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_norm_specular(1);
}}
technique ps2tns	{pass{
		VertexShader = compile VS_TARGET DudVS();
		PixelShader = compile PS_TARGET PS_norm_specular(2);
}}
