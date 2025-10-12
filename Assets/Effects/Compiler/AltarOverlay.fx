sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float uTime;

// framePos
float2 resolution;
float4 sourceRect;
float4 uColor;



// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float frameY = (coords.y * resolution.y - sourceRect.y) / sourceRect.w;

	float4 color2 = tex2D(uImage1, float2(coords.x, frameY - uTime) * 0.2);
	float4 color = tex2D(uImage0, coords);

	return uColor * color2.r * color.a;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}