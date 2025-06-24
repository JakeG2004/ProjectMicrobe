Shader "Landon/Toon/Texture" {
	Properties {
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"
		
		sampler2D _MainTex;
		struct Input {
			half2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = mainTex.rgb;
			o.Alpha = 1;
			o.Emission = 0;
			o.Occlusion = 1;
		}
		ENDCG
	}
	FallBack "Diffuse" // needed for shadows
}