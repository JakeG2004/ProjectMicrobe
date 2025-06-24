Shader "Landon/Toon/Texture Glow" {
	Properties {
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		_Color("Color (RGB), ", Color) = (1,1,1,1)
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
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
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb;
			o.Alpha = 1;
			o.Occlusion = 1;
			o.Emission = (1 - mainTex.a) * 2 * _Color.rgb;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Texture"
}