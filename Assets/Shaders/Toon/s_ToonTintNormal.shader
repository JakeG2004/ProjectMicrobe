// Normal-Mapped TintMask shader using ToonRamp
// Texture alpha is Occlusion
// reducing color alphas gives Emission

Shader "Landon/Toon/Tint Mask/Normal" {
	Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_MainTex ("Main Texture (RGB), AO (A)", 2D) = "white" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		_TintR ("Tint R (RGB), Glow (-a)", Color) = (.5,.5,.5,1)
		_TintG ("Tint G (RGB), Glow (-a)", Color) = (.5,.5,.5,1)
		_TintB ("Tint B (RGB), Glow (-a)", Color) = (.5,.5,.5,1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"
		
		sampler2D _MainTex, _BumpMap;
		half4 _TintR, _TintG, _TintB;
		struct Input {
			half2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.r * _TintR.rgb + tex.g * _TintG.rgb + tex.b * _TintB.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Occlusion = tex.a;
			o.Alpha = 1;

			fixed3 r = tex.r * (1 - _TintR.a) * 2 * _TintR.rgb;
			fixed3 g = tex.g * (1 - _TintG.a) * 2 * _TintG.rgb;
			fixed3 b = tex.b * (1 - _TintB.a) * 2 * _TintB.rgb;
			o.Emission = r + g + b;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Texture"
}