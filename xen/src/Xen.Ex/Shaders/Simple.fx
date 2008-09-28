//CompilerOptions = ParentNamespace
float4x4 worldViewProj : WORLDVIEWPROJECTION;
float4x4 worldView : WORLDVIEW;
float4x4 viewMatrix : VIEW;
float4x4 viewProj : VIEWPROJECTION;
float4x4 world : WORLD;
float4 viewPoint : VIEWPOINT;
float4 viewDirection : VIEWDIRECTION;

float2 gCameraNearFar : CAMERANEARFAR;
float4 FillColour = 1;

texture2D GenericTexture : GLOBAL;
sampler2D GenericTextureSampler = sampler_state
{
	Texture = (GenericTexture);
};

texture2D CustomTexture;
sampler2D CustomTextureSampler = sampler_state
{
	Texture = (CustomTexture);
};

void SimpleVS(
	float4 pos : POSITION,
	out float4 o_pos : POSITION)
{
	o_pos = mul(pos,worldViewProj);
}

void SimpleVSTex(
	float4 pos : POSITION, float2 tex : TEXCOORD0,
	out float4 o_pos : POSITION,out float2 o_tex : TEXCOORD0)
{
	o_pos = mul(pos,worldViewProj);
	o_tex = tex;
}

void SimpleVSTexInstance(
	float4 sizePosition : POSITION12,
	float4 pos : POSITION, float2 tex : TEXCOORD0,
	out float4 o_pos : POSITION,out float2 o_tex : TEXCOORD0)
{
	pos.xy = pos.xy * sizePosition.xy + sizePosition.yz;
	o_pos = mul(pos,viewProj);
	o_tex = tex;
}

float4 SimplePSGenericTex(float2 tex : TEXCOORD0)   : COLOR 
{
	return tex2D(GenericTextureSampler,tex);
}
float4 SimplePSCustomTex(float2 tex : TEXCOORD0)   : COLOR 
{
	return tex2D(CustomTextureSampler,tex);
}
float4 SimplePSGenericColour()   : COLOR 
{
	return FillColour;
}

void SimpleVSCol(
	float4 pos : POSITION, float4 colour : COLOR0, out float4 colOut : COLOR0, out float4 o_pos : POSITION)
{
	o_pos = mul(pos,worldViewProj);
	colOut = colour;
}

float4 SimplePSCol(float4 colour : COLOR)   : COLOR 
{
	return colour;
}

void DepthOutVS(float4 pos : POSITION, out float4 o_pos : POSITION, out float3 out_position : TEXCOORD0)
{
	o_pos = mul(pos,worldViewProj);
	float4 worldPoint = mul(pos,world);
	float3 viewVector = (worldPoint.xyz - viewPoint.xyz);
	
	out_position = viewVector / gCameraNearFar.y;
}

float4 NonLinearDepthOutPS(float3 position : TEXCOORD0)   : COLOR0
{
	float depth = dot(position,position);
	return depth;
}
float4 LinearDepthOutPS(float3 position : TEXCOORD0)   : COLOR0
{
	float depth = length(position);
	return depth;
}

/*
technique FillGlobalGenericTexture
{
   pass
   {
		VertexShader = compile vs_2_0 SimpleVSTex();
		PixelShader = compile ps_2_0 SimplePSGenericTex();
   }
}
technique FillCustomTexture
{
   pass
   {
		VertexShader = compile vs_2_0 SimpleVSTex();
		PixelShader = compile ps_2_0 SimplePSCustomTex();
   }
}
*/

technique FillVertexColour
{
   pass
   {
		VertexShader = compile vs_2_0 SimpleVSCol();
		PixelShader = compile ps_2_0 SimplePSCol();
   }
}

technique FillSolidColour
{
   pass
   {
		VertexShader = compile vs_2_0 SimpleVS();
		PixelShader = compile ps_2_0 SimplePSGenericColour();
   }
}

technique NonLinearDepthOutput
{
   pass
   {
		VertexShader = compile vs_2_0 DepthOutVS();
		PixelShader = compile ps_2_0 NonLinearDepthOutPS();
   }
}



technique LinearDepthOutput
{
   pass
   {
		VertexShader = compile vs_2_0 DepthOutVS();
		PixelShader = compile ps_2_0 LinearDepthOutPS();
   }
}

