Shader "Landon/Toon/Tint Mask/Normal, AO" {
	Properties {
		_MainTex ("Main Texture (RGB) AO (A)", 2D) = "white" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		_TintR("Tint R (RGB)", Color) = (.5,.5,.5,1)
		_TintG("Tint G (RGB)", Color) = (.5,.5,.5,1)
		_TintB("Tint B (RGB)", Color) = (.5,.5,.5,1)
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
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
			//store AO in Alpha
			o.Alpha = tex.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}