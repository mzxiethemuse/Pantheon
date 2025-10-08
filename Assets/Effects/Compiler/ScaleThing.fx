sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);



// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float2 centerCoordOffset = coords - float2(0.5, 0.5);
	float2 uv = (centerCoordOffset * float2(1, 2 + (sqrt(coords.x+0.1) * -1)));
	float4 color = (coords.x < 0.9) * (abs(uv.y) < 0.5) * tex2D(uImage0, clamp(float2(0.5, 0.5) + uv, float2(0, 0), float2(1, 1)));
	
	return color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}