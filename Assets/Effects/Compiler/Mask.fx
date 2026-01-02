sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 maskSize;
float2 textureSize;
float2 screenPosition;

// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(uImage1, ((coords * maskSize) + screenPosition) / textureSize);
	float4 mask = tex2D(uImage0, coords);
	return color * (mask > float4(0, 0, 0, 0));
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}