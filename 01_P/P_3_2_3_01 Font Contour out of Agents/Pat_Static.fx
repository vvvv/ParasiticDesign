// Screen Space Static Postprocessor
//
// Produces an analogue noise overlay similar to a film grain / TV static
//
// Original implementation and noise algorithm
// Pat 'Hawthorne' Shearon
// http://www.truevision3d.com/forums/printpage.html;topic=18745.0
//
// Optimized scanlines + noise version with intensity scaling
// Georg 'Leviathan' Steinrohder
//
// This version is provided under a Creative Commons Attribution 3.0 License
// http://creativecommons.org/licenses/by/3.0/ 
// 


//transforms
float4x4 tW: WORLD;        //the models world matrix
float4x4 tV: VIEW;         //view matrix as set via Renderer (EX9)
float4x4 tP: PROJECTION;   //projection matrix as set via Renderer (EX9)
float4x4 tWVP: WORLDVIEWPROJECTION;

//floats
//Global Variables
float Timer : TIME;

//noise effect intensity value (0 = no effect, 1 = full effect)
float fNintensity <String uiname="Noise Intensity"; float uimin = 0; float uimax = 1;> = 0.5;
//scanlines effect intensity value (0 = no effect, 1 = full effect)
float fSintensity <String uiname="Scanlines Intensity"; float uimin = 0; float uimax = 1;> = 0.1;
//scanlines effect count value (0 = no effect, 4096 = full effect)
int fScount <String uiname="Count of Scanlines";> = 2048;


//texture
texture Tex <string uiname="Texture";>;
sampler Texture = sampler_state    //sampler for doing the texture-lookup
{
    Texture   = (Tex);          //apply a texture to the sampler
    MipFilter = LINEAR;         //sampler states
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};


// Input and Output Semantics
float4x4 tTex: TEXTUREMATRIX <string uiname="Texture Transform";>;

struct VS_INPUT
{
   float4 position : POSITION;
   float2 texCoord : TEXCOORD;
};
struct VS_OUTPUT
{
   float4 position : POSITION;
   float2 texCoord : TEXCOORD;
};
#define PS_INPUT VS_OUTPUT

VS_OUTPUT vs_main(const VS_INPUT IN)
{
   VS_OUTPUT OUT;

   OUT.position = mul(IN.position, tWVP);
   OUT.texCoord = mul(IN.texCoord, tTex);

   return OUT;
}

// Pixel Shader Output
// Pixel Shader Output
float4 ps_main( float2 Tex : TEXCOORD0  ) : COLOR0
{
	// sample the source
	float4 cTextureScreen = tex2D( Texture, Tex.xy);

	// make some noise
	float x = Tex.x * Tex.y * Timer *  1000;
	x = fmod(x, 13) * fmod(x, 123);
	float dx = fmod(x, 0.01);

	// add noise
	float3 cResult = cTextureScreen.rgb + cTextureScreen.rgb * saturate(0.1f + dx.xxx * 100);

	// get us a sine and cosine
	float2 sc; sincos(Tex.y * fScount, sc.x, sc.y);

	// add scanlines
	cResult += cTextureScreen.rgb * float3(sc.x, sc.y, sc.x) * fSintensity;

	// interpolate between source and result by intensity
	cResult = lerp(cTextureScreen, cResult, saturate(fNintensity));

	// convert to grayscale if desired
	#ifdef MONOCHROME
		cResult.rgb = dot(cResult.rgb, float3(0.3, 0.59, 0.11));
	#endif

	// return with source alpha
	return float4(cResult, cTextureScreen.a);
}

//Technique Calls
technique PostProcess
{
   pass Pass_0
   {
      VertexShader = compile vs_1_1 vs_main();
      PixelShader = compile ps_2_0 ps_main();
   }
}
