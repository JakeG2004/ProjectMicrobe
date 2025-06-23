Shader "Landon/Toon/Texture, Clip" {
	Properties {
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
		Cull Off
		CGPROGRAM
		#pragma surface surf ToonRamp keepalpha fullforwardshadows addshadow 
		#include "LightingToonRamp.cginc"
		
		sampler2D _MainTex;
		fixed _Cutoff;

		struct Input {
			half2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = mainTex.rgb;
			clip(mainTex.a - _Cutoff);
		}
		ENDCG
	}
	Fallback "Diffuse"
}