sampler uImage0 : register(s0);
float uTime;
float alpha;
float4 colorA;
float4 colorB;
float2 resolution;
int colorlevels;

// this shader is like, 2 jnstructions away from the limit. don't ask me how it works. i don't know, honestly.
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords * 0.25 + float2(0, -uTime));
    
    float cr = (c.r - 0.5);
    float uTimeScaled = uTime * 0.25;
    
    float2 coords2 = saturate(
        coords + 
        float2(cr, cr * 0.2)
         * 0.3);
    float4 c2 = tex2D(uImage0, 
        coords2 * 0.25
         + float2(uTimeScaled, -uTimeScaled));
    float a2 =  
        sin(coords2.x * 3.14)
        * sin(1-coords2.y)
         * pow(sin(coords2.y), 0.5);
    float a = 1
        -(distance(coords2 * float2(1, 2-coords2.y), float2(0.5, 0.5)) * 2);
	float lerpA =  c2.r * min(a * 6, 0.4) * a2 * 10;

    float4 colorFinal = 5 * lerp(float4(0, 0 ,0 ,0), lerp(colorA, colorB, lerpA), lerpA);
    colorFinal.a = lerpA * alpha;
//    colorFinal = round(colorFinal * colorlevels) / (colorlevels * 1.0);
    return colorFinal;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}