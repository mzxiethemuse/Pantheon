sampler uImage0 : register(s0);
float kernel[9];
float distance;


// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float _kernel[9] = kernel;
	for(int i=0; i<9; ++i)
	{
        _kernel[i] /= 16.0;
	}

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
		float3 px = tex2D(uImage0, coords + offsets[i]).rgb;
		if (
			((px.r + px.g + px.b) / 3) > 0
		) {
		    result += px * _kernel[i];
		}
	}
	color.rgb = result;
    return color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}