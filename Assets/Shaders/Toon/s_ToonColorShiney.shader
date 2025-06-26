// color shader using ToonRamp fresnel and cubemap
// color alpha gives Cubemap Emission
// fresColor alpha gives Fresnel Angle

Shader "Landon/Toon/Color Shiney" {
	Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_Color ("Color (RGB), Gloss (A)", Color) = (1,1,1,1)
		_FresColor ("Fresnel Color(RGB), Fresnel Angle (A)", Color) = (0,0.3,0.5,1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"
		
		fixed4 _Color, _FresColor;

		struct Input {
			fixed4 color : COLOR;
            half3 viewDir;
			float3 worldRefl;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			half fresnel = pow(1 - dot(normalize(IN.viewDir), normalize(IN.worldRefl)), _FresColor.a * 5) / 10;
			fixed3 fresnelColor = _FresColor.rgb * saturate(fresnel);

			o.Albedo = _Color.rgb;
			o.Emission = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, IN.worldRefl).rgb * _Color.a * 3 + fresnelColor;
			o.Alpha = 1;
			o.Occlusion = 1;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Color"
}