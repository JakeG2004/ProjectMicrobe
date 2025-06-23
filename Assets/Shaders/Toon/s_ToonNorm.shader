Shader "Landon/Toon/Normal/Texture" {
	Properties {
		_MainTex ("Main Texture (RGB)", 2D) = "gray" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Color("Color Mult (RGB)", Color) = (1,1,1,1)
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
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
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}
		ENDCG
	}
	Fallback "Diffuse"
}