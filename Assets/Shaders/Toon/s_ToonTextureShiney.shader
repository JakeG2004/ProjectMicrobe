// Basic Texture shader using ToonRamp
// Texture alpha is a mask
// Color alpha blends between mask giving Emission or Occlusion
// Color multiplies to Emission

Shader "Landon/Toon/Texture Shiney" {
	Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_MainTex ("Main Texture (RGB), Shiney Mask (A)", 2D) = "white" {}
		_Color ("Reflection Color (RGB), Gloss (A)", Color) = (1,1,1,1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"
		
		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			half2 uv_MainTex;
			float3 worldRefl;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = mainTex.rgb;
			o.Occlusion = 1;
			o.Emission = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, IN.worldRefl).rgb * _Color.a * _Color.rgb * 3 * (1 - mainTex.a);
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse" // needed for shadows
}