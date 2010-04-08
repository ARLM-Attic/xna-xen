
//storage for the RGBM multiplier for images and also rendering
float2	RgbmImageRenderScale		: GLOBAL;

float3 DecodeRgbm(float4 rgbm, float MaxScale)
{
	return rgbm.rgb * (MaxScale * rgbm.a);
}


//helpers:
float3 DecodeRgbmImage(float4 rgbm)
{
	return DecodeRgbm(rgbm, RgbmImageRenderScale.x);
}
float3 DecodeRgbmRender(float4 rgbm)
{
	return DecodeRgbm(rgbm, RgbmImageRenderScale.y);
}


float4 EncodeRGBM(float3 rgb)
{
	float MaxScale	= RgbmImageRenderScale.y;

	float maxRGB	= max(rgb.x,max(rgb.g,rgb.b));
	float M			= maxRGB / MaxScale;

	M				= ceil(M * 255.0f) / 255.0;		// make sure to round up

	return saturate(float4(rgb / (M * MaxScale), M));
}
