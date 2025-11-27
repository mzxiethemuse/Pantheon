sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 FilterMyShader(float2 coords : TEXCOORD0) : COLOR0
{
	float _kernel[9] = 
		{1, 2, 1,
		2, 4, 2,
		1, 2, 1};
	for(int i=0; i<9; ++i)
	{
        _kernel[i] /= 16.0;
	}
	
	float distance = 150.0 + (100 * -abs(sin(uTime)));
	float s = 1.0f / distance;

	float2 offsets[9];
	 
	// Top row
	offsets[0] = float2(-s, -s);
	offsets[1] = float2(0, -s);
	offsets[2] = float2(s, -s);
	 
	// Middle row
	offsets[3] = float2(-s, 0);
	offsets[4] = float2(0, 0);
	offsets[5] = float2(s, 0);
	 
	// Bottom row
	offsets[6] = float2(-s, s);
	offsets[7] = float2(0, s);
	offsets[8] = float2(s, s);

	float4 color = tex2D(uImage0, coords);
	float3 result = float3(0.0f, 0.0f, 0.0f);
	for(int i=0; i<9; ++i)
	{
		float2 off = offsets[i];
	    result += tex2D(uImage0, coords + offsets[i]).rgb * _kernel[i];
	}
	color.rgb = result;
    return color;
}

technique Technique1
{
    pass FilterMyShader
    {
        PixelShader = compile ps_2_0 FilterMyShader();
    }
}