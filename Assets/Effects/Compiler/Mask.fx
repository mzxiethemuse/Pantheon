sampler uImage0 : register(s0);
sampler uImage1 : register(s1);



// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(uImage0, coords);
	float4 mask = tex2D(uImage1, coords);
	return color * (mask > float4(0, 0, 0, 0));
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}