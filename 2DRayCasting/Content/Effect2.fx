#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float Seconds;
uint Milliseconds;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

//==============================================================================
// Vertex shader
//==============================================================================

//==============================================================================
// Pixel shader 
//==============================================================================


float4 PS(VertexShaderOutput input) : COLOR
{
	float2 coords = input.TextureCoordinates;

	float4 color = tex2D(SpriteTextureSampler, coords) * input.Color;
	float4 negativ = color * -1 + float4(1, 1, 1, 1);
	
	return color;
}

//==============================================================================
// Techniques
//==============================================================================
technique Tech0
{
	pass P0
	{
		//VertexShader = compile VS_SHADERMODEL VS();
		PixelShader = compile PS_SHADERMODEL PS();
	}
};