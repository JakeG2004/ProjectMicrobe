Shader "Landon/Toon/Color Cubemap" {
	Properties {
		_Color ("Color (RGB), ", Color) = (1,1,1,1)
		_Cube ("Cubemap", CUBE) = "" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"

		samplerCUBE _Cube;
		fixed4 _Color;

		struct Input {
			fixed4 color : COLOR;
			float3 worldRefl;
		};
		void surf (Input IN, inout SurfaceOutputCustom o) {
			o.Albedo = _Color.rgb;
			o.Emission = texCUBE (_Cube, IN.worldRefl).rgb;
		}
		ENDCG
	}
	Fallback "Diffuse"
}