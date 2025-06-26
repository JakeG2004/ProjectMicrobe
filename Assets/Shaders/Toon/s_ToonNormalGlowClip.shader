// Normal-mapped Texture shader using ToonRamp
// GlowTex.rgb sets glow color
// Color.a Sets glow strength
// GlowTex.a is a clipping mask
// Color.rgb multiplies to Albedo
// Texture alpha is Occlusion

Shader "Landon/Toon/Normal Glow Clip" {
	Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_MainTex ("Main Texture (RGB), AO (A)", 2D) = "white" {}
		[NoScaleOffset] _GlowTex ("Glow Color (RGB), Clip (A)", 2D) = "black" {}
		[NoScaleOffset] _BumpMap ("Bumpmap", 2D) = "bump" {}
		_Color("Color Mult (RGB), Glow Strength (A)", Color) = (1,1,1,1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="AlphaTest" }
		//Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" "IgnoreProjector"="True" }
		Cull Off
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows addshadow
		#include "LightingToonRamp.cginc"
		
		sampler2D _MainTex, _GlowTex, _BumpMap;
		half4 _Color;
		struct Input {
			half2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 glowTex = tex2D(_GlowTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Emission = glowTex.rgb * _Color.a;
			o.Occlusion = mainTex.a;

			o.Alpha = glowTex.a;
			clip(glowTex.a - 0.5);
		}
		ENDCG
	}
	Fallback "Landon/Toon/Texture"
}