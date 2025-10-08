sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float4 uColor;
float uTime;
float uScanlineCount;



// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{	
	return uColor * (1 - abs(sin((coords.y * uScanlineCount) + uTime)));
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}