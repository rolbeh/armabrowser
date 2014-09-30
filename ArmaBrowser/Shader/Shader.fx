//--------------------------------------------------------------------------------------
// 
// WPF ShaderEffect HLSL -- Shader
//
//--------------------------------------------------------------------------------------

static const float pi = 3.1415926535897932384626433832795028841971693993751f;
static const float pi2 = 3.1415926535897932384626433832795028841971693993751f *2.0f;

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------

float4 color1 : register(C0);
float  threshold1 : register(C1);

float4 color2 : register(C2);
float  threshold2 : register(C3);


//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D implicitInputSampler : register(S0);


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 main(float2 uv : TEXCOORD) : COLOR
{

	//float4 color = tex2D(implicitInputSampler, uv);
	//
	////float4 result = color;
	///*
	//if ((uv.y * 1.02f) > (threshold1  * 0.63) && uv.y < threshold1)
	//{
	//	color = color * color1;
	//}
	//*/
	//if ((uv.y) >(threshold2  * 0.63) && uv.y < (threshold2  * 1.93))
	//{
	//	color = color * color2;
	//}
	//
	return float4(0,1,0,1);
}


