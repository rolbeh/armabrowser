#version 130

// main shader variables
in vec2 TexCoord;
out vec4 FragColor;
vec4 post;

// global uniforms
uniform sampler2D uTexture;

// to be set in main code
uniform float time = 0.0;

// master brightness
uniform float master = 1.0;

//--------------------------------------------------------------------------------------------------------------------------

// texture size
uniform float textureWidth = 512;
uniform float textureHeight = 512;

//----------------------------------------------------------------------
// effects uniforms
// blur / glow
uniform float direction_h = 0.7;
uniform float direction_v = 0.7;
uniform float blur_radius = 2.0;
uniform float blur_intensity = 0.7;
uniform float glow_intensity = 0.38;
// noise
uniform float noise_intensity = 0.04;
// rgb shift
uniform float rgbShift_offset = 0.005;
// interlace
uniform float interlace_intensity = 1.0;
// vignette
uniform float vignette_intensity = 2.2;

// screen pulsation
uniform float pulse_amplitude = 0.0;
uniform float pulse_freq = 0.0;

//----------------------------------------------------------------------
// global effects variables

// blur distance
vec2 uShift_h = vec2(blur_radius / textureWidth, direction_h / textureHeight);
vec2 uShift_v = vec2(direction_v / textureWidth, blur_radius / textureHeight);
vec3 blurredColor = vec3(0.0);

// blur steps
const int gaussRadius = 25;
float gaussFilter[gaussRadius] = float[gaussRadius]
(
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
);
 
//----------------------------------------------------------------------
// effects names
float noise;
vec3 rgbShift;
vec3 interlace;

//----------------------------------------------------------------------
// shader functions

float makeRandom(float value) 
{
    return fract(sin(value) * 10000.0);
}

float makeNoise(vec2 value) 
{
    return makeRandom((value.x + value.y * 10000.0) * time);
}

void makeEffects()
{
	//----------------------------------------------------------------------
	// blur / glow

	vec2 texCoord;

	// horizontal pass
	texCoord = TexCoord - float(gaussRadius / 2) * uShift_h;

	for (int i = 0; i < gaussRadius; i ++) 
	{ 
		blurredColor += gaussFilter[i] * texture(uTexture, texCoord).xyz;
		texCoord += uShift_h;
	}

	// vertical pass
	texCoord = TexCoord - float(gaussRadius / 2) * uShift_v;

	for (int i = 0; i < gaussRadius; i ++) 
	{ 
		blurredColor += gaussFilter[i] * texture(uTexture, texCoord).xyz;
		texCoord += uShift_v;
	}

	//----------------------------------------------------------------------
	// rgb shift

	rgbShift = vec3(0);

	vec2 rgbCoords;
	rgbCoords.x = TexCoord.x;
	rgbCoords.y = TexCoord.y;

	float intensity = 0.34;

	rgbCoords.y -= rgbShift_offset / 1.5;

	// R
	//rgbCoords.x -= rgbShift_offset / 2;
	//rgbShift.r = texture(uTexture, rgbCoords).r * intensity;

	// G
	rgbCoords.x -= rgbShift_offset / 2;
	rgbShift.g = texture(uTexture, rgbCoords).g * intensity;

	// B
	rgbCoords.x += rgbShift_offset;
	rgbShift.b = texture(uTexture, rgbCoords).b * intensity;
	
	//----------------------------------------------------------------------
	// noise

	noise = makeNoise(TexCoord) * noise_intensity;

	//----------------------------------------------------------------------
	// interlace

	float width1 = 4.0;
	float width2 = width1 / 2;

	if (mod(trunc(gl_FragCoord.y), width1) < width2)
		interlace = vec3(0.0, 0.0, 0.0);
	else
		interlace = vec3(1.0, 1.0, 1.0) * interlace_intensity;

	//----------------------------------------------------------------------
}

//--------------------------------------------------------------------------------------------------------------------------

void main() 
{
	// call shader functions
	makeEffects();

	// render blur
	post = vec4(((blurredColor * blur_intensity) + texture(uTexture, TexCoord).xyz), 1.0) * glow_intensity; 

	// render rgb shift
	post.rgb -= rgbShift;
	// render noise
	post.rgb -= noise;
	// render interlace
	post.rgb -= interlace;

	// modulate brightness
	post -= sin(time * pulse_freq) * pulse_amplitude;
	
	// render output (overall brightness controlled by master variable)
	FragColor = post * master;


	// render post vignette
	float vignette;
	if (TexCoord.x < 0.5)
		vignette = (vignette_intensity * (TexCoord.x));
	else
		vignette = (vignette_intensity * (1 - TexCoord.x));

    // render output
	FragColor *= vignette;
	
}