Shader "Landon/Toon/Texture, AO, Glow Scroll" {
	Properties {
		_MainTex ("Texture (RGB) AO (A)", 2D) = "white" {}
		[NoScaleOffset] _Glow ("Glow Mask (R) Glow Pattern (G)", 2D) = "white" {}
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}

		[HDR] _Color ("Glow Color", Color) = (0.5,0.5,0.5,1.0)
		_Scroll ("Scroll Speed", Range(-5,5)) = 0.5
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows// noambient
		#include "LightingToonRamp.cginc"
		
		fixed4 _Color;
		fixed _Scroll;
		sampler2D _MainTex, _Glow;
		struct Input {
			half2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 glowTex = tex2D(_Glow, IN.uv_MainTex);

			fixed2 scroll = fixed2(0, frac(_Time.x*_Scroll));
			fixed3 glowScroll = tex2D(_Glow, IN.uv_MainTex - scroll);

			o.Albedo = mainTex.rgb;
			o.Occlusion = mainTex.a;
			o.Emission = glowTex.r * glowScroll.g * _Color;
			o.Alpha = 1;
		}
		ENDCG
	}
	 Fallback "Landon/Toon/Texture, AO"
}