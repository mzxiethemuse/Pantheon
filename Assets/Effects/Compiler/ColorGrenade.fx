sampler uImage0 : register(s0);
float4 ColorBright;
float4 ColorMedium;
float4 ColorDark;



// R=lightest, G=mid, B=low

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(uImage0, coords);
	if (color.r == 1) {
		color = ColorBright;
	} else if (color.g == 1) {
		color = ColorMedium;
	} else if (color.b == 1) {
		color = ColorDark;
	}
	return color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}