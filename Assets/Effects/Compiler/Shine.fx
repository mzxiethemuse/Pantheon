sampler uImage0 : register(s0);
float uTime;



// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(uImage0, coords);
    float brightness = sin(6 * (coords.x + coords.y) + uTime);
    color += (float4(1, 1, 1, 1) * clamp(brightness, 0, 1) * 0.35 * color.a);
    return color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}