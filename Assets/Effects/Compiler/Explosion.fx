sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float4 Color;
float Intensity;
float Radius;
float TotalTime;
float uTime;



// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float distance = sqrt(
    	pow(
    	0.5 - coords.x, 2) + 
    	pow(0.5 - coords.y, 2)
    	
    ) * 1;
    float4 c = Color * pow(2 * distance, Intensity) * (distance < 0.5);
    c.a *= (Color.a / 255);
    return c * 5 * (1 - (uTime / TotalTime));
}

technique Technique1
{
	pass Circle
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}