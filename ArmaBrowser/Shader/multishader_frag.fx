//--------------------------------------------------------------------------------------
// 
// WPF ShaderEffect HLSL -- multishader_frag
//
//--------------------------------------------------------------------------------------

// to be set in main code
uniform float time = 0.0;

// blur steps
static const int gaussRadius = 13;
static float gaussFilter[gaussRadius] =
{
	0.000914514357099007,
	//0.00194255942708531,
	0.00386462479081985,
	//0.00720094664853653,
	0.012566695359556,
	//0.0205400651572557,
	0.0314435652401948,
	//0.045082810726942,
	0.0605395826381862,
	//0.0761407405573117,
	0.0896899963758627,
	//0.0989509919993277,
	0.102245813443645,
	//0.0989509919993277,
	0.0896899963758627,
	//0.0761407405573117,
	0.0605395826381862,
	//0.045082810726942,
	0.0314435652401948,
	//0.0205400651572557,
	0.012566695359556,
	//0.00720094664853653,
	0.00386462479081985,
	//0.00194255942708531,
	0.000914514357099007
}; 

/*
static const int gaussRadius = 25;
satic float gaussFilter[gaussRadius] =
	{
		0.000914514357099007,
		0.00194255942708531,
		0.00386462479081985,
		0.00720094664853653,
		0.012566695359556,
		0.0205400651572557,
		0.0314435652401948,
		0.045082810726942,
		0.0605395826381862,
		0.0761407405573117,
		0.0896899963758627,
		0.0989509919993277,
		0.102245813443645,
		0.0989509919993277,
		0.0896899963758627,
		0.0761407405573117,
		0.0605395826381862,
		0.045082810726942,
		0.0314435652401948,
		0.0205400651572557,
		0.012566695359556,
		0.00720094664853653,
		0.00386462479081985,
		0.00194255942708531,
		0.000914514357099007
	};*/

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------

// master brightness
float master : register(C0);


// texture size
float textureWidth : register(C1) = 512;
float textureHeight : register(C2) = 512;


//----------------------------------------------------------------------
// effects uniforms
// blur / glow
float direction_h : register(C3) = 0.7;
float direction_v : register(C4) = 0.7;
float blur_radius : register(C5) = 2.0;
float blur_intensity : register(C6) = 0.7;
float glow_intensity : register(C8) = 0.38;
// noise
float noise_intensity : register(C9) = 0.04;
// rgb shift
float rgbShift_offset : register(C10) = 0.005;
// interlace
uniform float interlace_intensity : register(C11) = 1.0;
// vignette
uniform float vignette_intensity : register(C12) = 2.2;

// screen pulsation
uniform float pulse_amplitude : register(C13) = 0.0;
uniform float pulse_freq : register(C14) = 0.0;

float4 screenSpace : SV_Position;

//----------------------------------------------------------------------
// global effects variables
//----------------------------------------------------------------------
// effects names
struct Effects{
	float3 blurredColor : color3;
	float noise;
	float3 rgbShift : color3;
	float3 interlace :color3;
};


//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D implicitInputSampler : register(S0);


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float makeRandom(float value)
{
	return frac(sin(value) * 10000.0);
}

float makeNoise(float2 value)
{
	float var = (value.x + value.y * 10000.0) * time;
	return makeRandom(var);
}

Effects makeEffects(float2 uv : TEXCOORD)
{
	Effects result;

	result.blurredColor = float3(0.0f, 0.0f, 0.0f);

		float2 uShift_h = float2(blur_radius / textureWidth, direction_h / textureHeight);
	//	float2 uShift_v = float2(direction_v / textureWidth, blur_radius / textureHeight);


	////----------------------------------------------------------------------
	//// blur / glow

	float2 texCoord;

	//// horizontal pass
	texCoord = uv - float(gaussRadius / 2) * uShift_h;

	for (int i = 0; i < gaussRadius; i++)
	{
		float3 p = tex2D(implicitInputSampler, texCoord);
			result.blurredColor += gaussFilter[i] * p.xyz;
		texCoord += uShift_h;
	}

	// vertical pass
	/*texCoord = uv - float(gaussRadius / 2) * uShift_v;

	for (int i = 0; i < gaussRadius; i++)
	{
		float3 p = tex2D(implicitInputSampler, texCoord);
			result.blurredColor += gaussFilter[i] * p.xyz;
		texCoord += uShift_v;
	}*/

	////----------------------------------------------------------------------
	//// rgb shift

	result.rgbShift = float3(0.0f, 0.0f, 0.0f);

	
	float2 rgbCoords;
	rgbCoords.x = uv.x;
	rgbCoords.y = uv.y -(rgbShift_offset / 1.5);

	float intensity = 0.34f;

	//rgbCoords.y -= rgbShift_offset / 1.5;

	//// R
	////rgbCoords.x -= rgbShift_offset / 2;
	////rgbShift.r = texture(implicitInputSampler, rgbCoords).r * intensity;

	// G
	rgbCoords.x -= rgbShift_offset / 2;
	result.rgbShift.g = tex2D(implicitInputSampler, rgbCoords).g * intensity;

	// B
	rgbCoords.x += rgbShift_offset;
	result.rgbShift.b = tex2D(implicitInputSampler, rgbCoords).b * intensity;

	////----------------------------------------------------------------------
	//// noise

	result.noise = makeNoise(uv) * noise_intensity;

	////----------------------------------------------------------------------
	// interlace

	float width1 = 4.0;
	float width2 = width1 / 2;

	if ((trunc(screenSpace.y) % width1) < width2)
		result.interlace = float3(0.0, 0.0, 0.0);
	else
		result.interlace = float3(1.0, 1.0, 1.0) * interlace_intensity;

	////----------------------------------------------------------------------

	return result;
}


float4 main(float2 texCoord : TEXCOORD) : COLOR
{
	Effects effect;
	effect.blurredColor = float3(0.0f, 0.0f, 0.0f);
	effect.rgbShift = float3(0.0f, 0.0f, 0.0f);
	//Effects effect = makeEffects(texCoord);
	
	float2 uShift_h = float2(blur_radius / textureWidth, direction_h / textureHeight);
	
	float4 color = tex2D(implicitInputSampler, texCoord);

	//// horizontal pass
	texCoord = texCoord - float(gaussRadius / 2) * uShift_h;

	for (int i = 0; i < gaussRadius; i++)
	{
		float3 p = tex2D(implicitInputSampler, texCoord);
			effect.blurredColor += gaussFilter[i] * p.xyz;
		texCoord += uShift_h;
	}

	/*// vertical pass
	texCoord = texCoord - float(gaussRadius / 2) * uShift_v;

	for (int i = 0; i < gaussRadius; i++)
	{
		float3 p = tex2D(implicitInputSampler, texCoord);
		effect.blurredColor += gaussFilter[i] * p.xyz;
		texCoord += uShift_v;
	}*/

	////////////////////////////////////////////////////////////////////////
	//// rgb Shift
	float intensity = 0.08f;
	float2 rgbCoords;
	rgbCoords.x = texCoord.x;
	rgbCoords.y = texCoord.y - (rgbShift_offset / 1.5);


	// G
	rgbCoords.x -= rgbShift_offset / 2;
	effect.rgbShift.g = tex2D(implicitInputSampler, rgbCoords).g * intensity;

	// B
	rgbCoords.x += rgbShift_offset;
	effect.rgbShift.b = tex2D(implicitInputSampler, rgbCoords).b * intensity;

	//
	//////////////////////////////////////////////////////////////////////////


	float3 blurredColor = effect.blurredColor * blur_intensity;
		// render blur
	//color = float4(color.r , color.g , color.b, 1.0);
	color = float4(blurredColor.r + color.r, blurredColor.g + color.g, blurredColor.b + color.b, 1.0);
	color = color*glow_intensity;

	// render rgb shift
	color.rgb -= effect.rgbShift;

	//// render noise
	//color.rgb -= effect.noise;
	//// render interlace
	//post.rgb -= effect.interlace;

	//// modulate brightness
	//post -= sin(time * pulse_freq) * pulse_amplitude;

	// render output (overall brightness controlled by master variable)
	color = color * master;
	return color;
}


