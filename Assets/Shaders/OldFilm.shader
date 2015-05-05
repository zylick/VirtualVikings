Shader "Custom/OldFilm"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			//Usual post processing setup
			ZTest Always Cull Off ZWrite Off
		  	Fog { Mode off }
	  	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert (appdata_img v)
			{
			    v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
			
			//float2 ImageSize;
			//float2 TexelSize;
			//float4 Colour;
			sampler2D _MainTex;
			
			float SepiaValue;
			float NoiseValue;
			float ScratchValue;
			float InnerVignetting;
			float OuterVignetting;
			float RandomValue;
			float TimeLapse;
			
			float3 Overlay (float3 src, float3 dst)
			{
			    // if (dst <= delta) then: 2 * src * dst
			    // if (dst > delta) then: 1 - 2 * (1 - dst) * (1 - src)
			    return float3((dst.x <= 0.5) ? (2.0 * src.x * dst.x) : (1.0 - 2.0 * (1.0 - dst.x) * (1.0 - src.x)),
			                (dst.y <= 0.5) ? (2.0 * src.y * dst.y) : (1.0 - 2.0 * (1.0 - dst.y) * (1.0 - src.y)),
			                (dst.z <= 0.5) ? (2.0 * src.z * dst.z) : (1.0 - 2.0 * (1.0 - dst.z) * (1.0 - src.z)));
			}
			
						/// <summary>
			/// 2D Noise by Ian McEwan, Ashima Arts.
			/// <summary>
			float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
			float2 mod289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
			float3 permute(float3 x) { return mod289(((x*34.0)+1.0)*x); }
			
			float snoise (float2 v)
			{
			    const float4 C = float4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
			                        0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
			                        -0.577350269189626, // -1.0 + 2.0 * C.x
			                        0.024390243902439); // 1.0 / 41.0
			
			    // First corner
			    float2 i  = floor(v + dot(v, C.yy) );
			    float2 x0 = v -   i + dot(i, C.xx);
			
			    // Other corners
			    float2 i1;
			    i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
			    float4 x12 = x0.xyxy + C.xxzz;
			    x12.xy -= i1;
			
			    // Permutations
			    i = mod289(i); // Avoid truncation effects in permutation
			    float3 p = permute( permute( i.y + float3(0.0, i1.y, 1.0 ))
			        + i.x + float3(0.0, i1.x, 1.0 ));
			
			    float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
			    m = m*m ;
			    m = m*m ;
			
			    // Gradients: 41 points uniformly over a line, mapped onto a diamond.
			    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
			
			    float3 x = 2.0 * frac(p * C.www) - 1.0;
			    float3 h = abs(x) - 0.5;
			    float3 ox = floor(x + 0.5);
			    float3 a0 = x - ox;
			
			    // Normalise gradients implicitly by scaling m
			    // Approximation of: m *= inversesqrt( a0*a0 + h*h );
			    m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
			
			    // Compute final noise value at P
			    float3 g;
			    g.x  = a0.x  * x0.x  + h.x  * x0.y;
			    g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			    return 130.0 * dot(m, g);
			}
	
			float4 frag( v2f v ) : COLOR
			{
			    // Sepia RGB value
			    float3 sepia = float3(112.0 / 255.0, 66.0 / 255.0, 20.0 / 255.0);
			
			    // Step 1: Convert to grayscale
			    float2 vUv = v.uv;
			    float3 colour = tex2D(_MainTex, v.uv).xyz;
			    float gray = (colour.x + colour.y + colour.z) / 3.0;
			    float3 grayscale = float3(gray);
			    
			    // Step 2: Appy sepia overlay
			    float3 finalColour = Overlay(sepia, grayscale);
			    
			    // Step 3: Lerp final sepia colour
			    finalColour = grayscale + SepiaValue * (finalColour - grayscale);
			    
			    // Step 4: Add noise
			    float noise = snoise(vUv * float2(1024.0 + RandomValue * 512.0, 1024.0 + RandomValue * 512.0)) * 0.5;
			    finalColour += noise * NoiseValue;
			    
			    // Optionally add noise as an overlay, simulating ISO on the camera
			    //vec3 noiseOverlay = Overlay(finalColour, vec3(noise));
			    //finalColour = finalColour + NoiseValue * (finalColour - noiseOverlay);
			    
			    // Step 5: Apply scratches
			    if ( RandomValue < ScratchValue )
			    {
			        // Pick a random spot to show scratches
			        float dist = 1.0 / ScratchValue;
			        float d = distance(vUv, float2(RandomValue * dist, RandomValue * dist));
			        if ( d < 0.4 )
			        {
			            // Generate the scratch
			            float xPeriod = 8.0;
			            float yPeriod = 1.0;
			            float pi = 3.141592;
			            float phase = TimeLapse;
			            float turbulence = snoise(vUv * 2.5);
			            float vScratch = 0.5 + (sin(((vUv.x * xPeriod + vUv.y * yPeriod + turbulence)) * pi + phase) * 0.5);
			            vScratch = clamp((vScratch * 10000.0) + 0.35, 0.0, 1.0);
			
			            finalColour.xyz *= vScratch;
			        }
			    }
			    
			    // Step 6: Apply vignetting
			    // Max distance from centre to corner is ~0.7. Scale that to 1.0.
			    float d = distance(float2(0.5, 0.5), vUv) * 1.414213;
			    float vignetting = clamp((OuterVignetting - d) / (OuterVignetting - InnerVignetting), 0.0, 1.0);
			    finalColour.xyz *= vignetting;
			    
			    // Apply colour
			    return float4(finalColour , 1.0);
			    //return float4(1.0 , 1.0 , 1.0 , 1.0);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}