sampler uImage0 : register(s0);
float uTime;
float alpha;
float4 colorA;
float4 colorB;
float2 resolution;


// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords * 0.25 + float2(0, -uTime));
    float2 coords2 = clamp(coords + float2(c.r - 0.5, (c.r - 0.5) * 0.2) * 0.3, float2(0, 0), float2(1, 1));
    float4 c2 = tex2D(uImage0, coords2 * 0.25 + float2(uTime * 0.25, -uTime * 0.25));
    float a2 =  sin(coords2.x * 3.14) * sin(1-coords2.y) * pow(sin(coords2.y), 0.5);
    float a = 1- (distance(coords2 * float2(1, 2-coords2.y), float2(0.5, 0.5)) * 2);
	float cA = (c2.r + c2.g + c2.b) / 3;
	float lerpA =  cA * min(a * 6, 0.4) * a2 * 10;

    float4 colorFinal = 5 * lerp(float4(0,0 ,0 ,0), lerp(colorA, colorB, lerpA), lerpA);
    colorFinal.a = lerpA * alpha;
    return colorFinal;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}