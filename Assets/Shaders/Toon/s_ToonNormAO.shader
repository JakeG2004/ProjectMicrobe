Shader "Landon/Toon/Normal/AO" {
	Properties {
		_MainTex ("Main Texture (RGB) AO (A)", 2D) = "white" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"
		
		sampler2D _MainTex, _BumpMap;
		struct Input {
			half2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Alpha = 1;
			o.Emission = 0;
			o.Occlusion = mainTex.a;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Normal/Texture"
}