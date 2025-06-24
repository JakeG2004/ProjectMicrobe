Shader "Landon/Toon/Glow" {
	Properties {
		_Color ("Color (RGB), ", Color) = (1,1,1,1)
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"

		fixed4 _Color;

		struct Input {
			fixed4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutputCustom o) {
			o.Albedo = _Color.rgb;
			o.Emission = _Color.rgb * 2;
			o.Alpha = 1;
			o.Occlusion = 1;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Color"
}