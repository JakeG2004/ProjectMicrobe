Shader "Landon/Toon/Tint Mask/AO" {
	Properties {
		_MainTex ("Tint Channels (RGB) AO (A)", 2D) = "white" {}
		_TintR ("Tint R (RGB)", Color) = (.5,.5,.5,1)
		_TintG ("Tint G (RGB)", Color) = (.5,.5,.5,1)
		_TintB ("Tint B (RGB)", Color) = (.5,.5,.5,1)
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"
		
		sampler2D _MainTex;
		half4 _TintR, _TintG, _TintB;

		struct Input {
			half2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.r * _TintR.rgb + tex.g * _TintG.rgb + tex.b * _TintB.rgb;
			o.Alpha = 1;
			o.Emission = 0;
			o.Occlusion = tex.a;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Texture"
}