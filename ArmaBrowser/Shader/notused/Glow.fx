//--------------------------------------------------------------------------------------
// 
// WPF ShaderEffect HLSL -- Glow
//
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------

float Threshold : register(C0);
float glowFactor : register(C1);

//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D implicitInputSampler : register(S0);


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 main(float2 uv : TEXCOORD) : COLOR
{
    // This particular shader just multiplies the color at 
    // a point by the colorFilter shader constant.
	float4 color = tex2D(implicitInputSampler, uv);
	
	float intensity = (color.r + color.g + color.b) / 3.0f;

	if (intensity < Threshold)
		color = float4(color.r * glowFactor, color.g * glowFactor, color.b * glowFactor, color.a * glowFactor);
	else
		color * (1 / glowFactor);

	return color;
}


