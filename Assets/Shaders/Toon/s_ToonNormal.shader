// Normal-mapped Texture shader using ToonRamp
// Texture alpha is a mask
// Color alpha blends between mask giving Emission or Occlusion
// Color multiplies to Emission

Shader "Landon/Toon/Normal" {
	Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_MainTex ("Main Texture (RGB), Mask (A)", 2D) = "gray" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		_Color("Glow Color (RGB), Emission or AO (A)", Color) = (1,1,1,1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"
		
		sampler2D _MainTex, _BumpMap;
		half4 _Color;

		struct Input {
			half2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = mainTex.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Occlusion = lerp(mainTex.a, fixed3(1,1,1), 1 - _Color.a);
			o.Emission = (1 - mainTex.a) * (1 - _Color.a) * 2 * _Color.rgb;
			o.Alpha = 1;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Texture"
}