sampler uImage0 : register(s0);

// framePos
float2 textureSize;
float4 staticColor;

// This is a shader. You are on your own with shaders. Compile shaders in an XNB project.

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{	
	float4 currentPixel = tex2D(uImage0, coords);
    float4 color = float4(0, 0, 0, 0);

	bool isUsingStatic = (staticColor.a != 0);
	if (currentPixel.a == 0)
	{
		
		float2 offsets[4];
	         
	    // Top row
	    offsets[0] = float2(-1, 0);
	    offsets[1] = float2(0, -1);
	    offsets[2] = float2(0, 1);
	    offsets[3] = float2(1, 0);
	    
	    
	    for(int i=0; i<4; ++i)
	        {
	            float2 off = offsets[i] / textureSize;
	            float4 neighboringColor = tex2D(uImage0, coords + off);
	            if (neighboringColor.a > 0) {
	                if (isUsingStatic) {
                        color = staticColor;
	                } else {
                        color = neighboringColor;
	                }
	            }
	        }
    }

	return color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}