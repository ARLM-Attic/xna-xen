//CompilerOptions = InternalClass, ParentNamespace, NoPreShader, UseAsmToHlslXboxConverter, DefinePlatform



// ------------------------------------
//
// See Billboard.fx for details on how to implement a particle drawer shader
//
// ------------------------------------



float4x4 worldViewProj : WORLDVIEWPROJECTION : register(c0);
float3 textureSizeOffset : register(c4);
float2 velocityScale : register(c5);

//this shader is a modified copy of VelocityBillBoardDrawer.fx
//see BillboardDrawer.fx for a description of how to write a particle drawer shader

#ifndef XBOX360

//used for CPU particles
float4 positionData[80] : register(c6);
float4 velocityData[80] : register(c86);
float4 colourData[80] : register(c166);

#endif

texture2D PositionTexture;
sampler2D PositionSampler = sampler_state
{
	Texture = (PositionTexture);
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = POINT;
    AddressU = WRAP;
    AddressV = WRAP;
};
texture2D ColourTexture;
sampler2D ColourSampler = sampler_state
{
	Texture = (ColourTexture);
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = POINT;
    AddressU = WRAP;
    AddressV = WRAP;
};
texture2D VelocityTexture;
sampler2D VelocitySampler = sampler_state
{
	Texture = (VelocityTexture);
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = POINT;
    AddressU = WRAP;
    AddressV = WRAP;
};
texture2D UserTexture;
sampler2D UserSampler = sampler_state
{
	Texture = (UserTexture);
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = POINT;
    AddressU = WRAP;
    AddressV = WRAP;
};

void ParticleVS_GpuTex(
						float4 position : POSITION,
					out float4 positionOut : POSITION,
					out float4 colourOut : TEXCOORD1,
				uniform bool	useColour,				// note the uniform constant passed in, indicates if colours are sampled
				uniform bool	useUserPositionOffset)
{
	position.x += textureSizeOffset.z;

	float index = position.x * textureSizeOffset.x;
	
	float2 samplePosition = float2(fmod(index,1),(floor(index)) * textureSizeOffset.y);
	samplePosition    += 0.5 * textureSizeOffset.xy;
	samplePosition.y  = 1 - samplePosition.y;
	
	float4 texCoord = float4(samplePosition,0,0);

	float4 positionSize = tex2Dlod(PositionSampler,texCoord);
	float4 velocityRotation = tex2Dlod(VelocitySampler,texCoord);
	
	if (useUserPositionOffset)
	{
		positionSize.xyz += tex2Dlod(UserSampler,texCoord).yzw;
	}
	
	positionOut = float4(0,0,0,1);
	positionOut.xy = positionSize.xy;
	
	float size = positionSize.w;
	
	colourOut = 1;
	if (useColour)
		colourOut = tex2Dlod(ColourSampler,texCoord);
	
	float scale = velocityScale.x + velocityRotation.w * velocityScale.y;
	
	float2 offset = velocityRotation.xy * position.y * scale;
	
	positionOut.xy = (positionOut.xy + offset * size);
	
	positionOut = mul(positionOut,worldViewProj);
}




#ifndef XBOX360


void ParticleVS_Cpu(
						float4 position : POSITION,
					out float4 positionOut : POSITION,
					out float4 colourBase : TEXCOORD1,
						uniform bool useColour)
{
	float4 positionSize = positionData[position.x];
	float4 velocityRotation = velocityData[position.x];
	
	float size = positionSize.w;
	
	colourBase = 1;
	if (useColour)
		colourBase = (colourData[position.x]);

	float length = position.y;

	position = float4(0,0,0,1);
	position.xy = positionSize.xy;
	
	float scale = velocityScale.x + velocityRotation.w * velocityScale.y;
	float2 offset = length * velocityRotation.xy * scale;
	
	position.xy = (position.xy + offset * size);
	positionOut = position;
	
	positionOut = mul(positionOut,worldViewProj);
}

#endif

float4 ParticlePS(float4 colour : TEXCOORD1) : COLOR0
{
	colour.a = saturate(colour.a);
	colour.rgb *= colour.a;
	return colour;
}




technique DrawVelocityParticles_LinesGpuTex
{
   pass { VertexShader = compile vs_3_0 ParticleVS_GpuTex(false,false); PixelShader = compile ps_3_0 ParticlePS(); }
}


technique DrawVelocityParticlesColour_LinesGpuTex
{
   pass { VertexShader = compile vs_3_0 ParticleVS_GpuTex(true,false); PixelShader = compile ps_3_0 ParticlePS(); }
}

//user variantes

technique DrawVelocityParticles_LinesGpuTex_UserOffset
{
   pass { VertexShader = compile vs_3_0 ParticleVS_GpuTex(false,true); PixelShader = compile ps_3_0 ParticlePS(); }
}
technique DrawVelocityParticlesColour_LinesGpuTex_UserOffset
{
   pass { VertexShader = compile vs_3_0 ParticleVS_GpuTex(true,true); PixelShader = compile ps_3_0 ParticlePS(); }
}


#ifndef XBOX360



technique DrawVelocityParticles_LinesCpu
{
   pass { VertexShader = compile vs_2_0 ParticleVS_Cpu(false); PixelShader = compile ps_2_0 ParticlePS(); }
}


technique DrawVelocityParticlesColour_LinesCpu
{
   pass { VertexShader = compile vs_2_0 ParticleVS_Cpu(true); PixelShader = compile ps_2_0 ParticlePS(); }
}

#endif