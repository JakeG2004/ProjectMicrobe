Shader "Landon/Toon/Color Cube" {
	Properties {
		_Color ("Color (RGB), ", Color) = (1,1,1,1)
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"

		fixed4 _Color;

		struct Input {
			fixed4 color : COLOR;
			float3 worldRefl;
		};
		void surf (Input IN, inout SurfaceOutputCustom o) {
			o.Albedo = _Color.rgb;
			o.Emission = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, IN.worldRefl).rgb;
			o.Alpha = 1;
			o.Occlusion = 1;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Texture"
}

/* Selectable cubemep
	_Cube ("Cubemap", CUBE) = "" {}
	...
	samplerCUBE _Cube;
	...
	o.Emission = texCUBE (_Cube, IN.worldRefl).rgb;
*/