// Normal-mapped Texture shader using ToonRamp
// Texture alpha is a mask
// Color alpha blends between mask giving Emission or Occlusion
// Color multiplies to Emission

Shader "Landon/Toon/Normal Mult" {
	Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_MainTex ("Main Texture (RGB), AO Mask (A)", 2D) = "gray" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		_Color("Mult Color (RGB), AO (A)", Color) = (1,1,1,1)
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
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Occlusion = lerp(mainTex.a, fixed3(1,1,1), 1 - _Color.a);
			o.Emission = 0;
			o.Alpha = 1;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Texture"
}