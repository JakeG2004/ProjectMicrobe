Shader "Landon/Toon/Texture, AO" {
	Properties {
		_MainTex ("Main Texture (RGB) AO (A)", 2D) = "white" {}
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
			//store AO in Alpha
			o.Alpha = mainTex.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}