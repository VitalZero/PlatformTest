﻿#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler2D InputSampler : register(s0);

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

	float4 c0 = tex2D(SourceSampler, float2(0, 0));
	float4 c1 = tex2D(SourceSampler, float2(0.25, 0));
	float4 c2 = tex2D(SourceSampler, float2(0.5, 0));
	float4 c3 = tex2D(SourceSampler, float2(0.75, 0));

	if (cIn.a == c0.a)
	{
		cOut = tex2D(TargetSampler, float2(0, 0));
	}
	else if (cIn.r == c1.r && cIn.g == c1.g && cIn.b == c1.b)
	{
		cOut = tex2D(TargetSampler, float2(0.25, 0));
	}
	else if (cIn.r == c2.r && cIn.g == c2.g && cIn.b == c2.b)
	{
		cOut = tex2D(TargetSampler, float2(0.5, 0));
	}
	else if (cIn.r == c3.r && cIn.g == c3.g && cIn.b == c3.b)
	{
		cOut = tex2D(TargetSampler, float2(0.75, 0));
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