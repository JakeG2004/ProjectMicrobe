Shader "Landon/Toon/Normal/AO Glow Clip" {
	Properties {
		_MainTex ("Main Texture (RGB) AO (A)", 2D) = "white" {}
		[NoScaleOffset] _GlowTex ("Glow (RGB)", 2D) = "black" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" "IgnoreProjector"="True" }
		Cull Off
		CGPROGRAM
		#pragma surface surf ToonRamp alpha:clip fullforwardshadows addshadow
		#include "LightingToonRamp.cginc"
		
		sampler2D _MainTex, _GlowTex, _BumpMap;
		struct Input {
			half2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 glowTex = tex2D(_GlowTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Occlusion = mainTex.a;
			o.Emission = glowTex.rgb * 1.5;
			o.Alpha = glowTex.a;
			clip(glowTex.a - 0.5);
		}
		ENDCG
	}
	Fallback "Landon/Toon/Normal/Texture"
}