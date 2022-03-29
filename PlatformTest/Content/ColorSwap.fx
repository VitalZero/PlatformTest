#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D InputSampler : register(s0);

int nColors;

texture xSourcePal;
sampler2D SourceSampler = sampler_state
{
	Texture = <xSourcePal>;
	Filter = POINT;
};

texture xTargetPal;
sampler2D TargetSampler = sampler_state
{
	Texture = <xTargetPal>;
	Filter = POINT;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 UV : TEXCOORD0;
};

// Pixel Shader
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 cIn = tex2D(InputSampler, input.UV) * input.Color;
	float4 cOut = cIn;

	for (int i = 0; i < nColors; i++)
	{
		float colorPos = (float)((float)i / (float)nColors);
		float4 tmpCol = tex2D(SourceSampler, float2(colorPos, 0));
				
		if (cIn.r == tmpCol.r && cIn.g == tmpCol.g && cIn.b == tmpCol.b)
		{
			cOut = tex2D(TargetSampler, float2(colorPos, 0));
		}
	}

	return cOut;
}

// Compile
technique Technique1
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}