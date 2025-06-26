// Simple color shader using ToonRamp
// reducing color alpha gives Emission

Shader "Landon/Toon/Color" {
	Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_Color ("Color (RGB), Glow (-a)", Color) = (1,1,1,1)
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
			o.Emission = (1 - _Color.a) * 2 * _Color.rgb;
			o.Occlusion = 1;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "VertexLit" // needed for shadows
}