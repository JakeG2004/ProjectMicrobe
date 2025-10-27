// Basic Texture shader using ToonRamp
// Texture alpha is a mask
// Color alpha blends between mask giving Emission or Occlusion
// Color multiplies to Emission

Shader "Landon/Toon/Tint Mask/Transparent Fres" {
    Properties {
        [NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
        _MainTex ("Main Texture (RGB), NA (A)", 2D) = "white" {}
        _TintR ("Tint R (RGB), Transparency (A)", Color) = (.5,.5,.5,1)
		_TintG ("Tint G (RGB), Transparency (A)", Color) = (.5,.5,.5,1)
		_TintB ("Tint B (RGB), Transparency (A)", Color) = (.5,.5,.5,1)
        _FresColor ("Fresnel Color(RGB), Fresnel Angle (A)", Color) = (0,0.3,0.5,1)
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Lighting Off

        CGPROGRAM
        #pragma surface surf ToonRamp fullforwardshadows alpha:fade
        #include "LightingToonRamp.cginc"

        sampler2D _MainTex;
        fixed4 _TintR, _TintG, _TintB, _FresColor;

        struct Input {
            half2 uv_MainTex;
            half3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputCustom o) {
            half3 viewDir = normalize(IN.viewDir);

            // Fancy Fresnel
			half NdotV = saturate(dot(o.Normal, normalize(IN.viewDir)));
			half fresnel = pow(1 - NdotV, _FresColor.a * 5);
            fixed3 fresnelColor = _FresColor.rgb * fresnel;

            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

			o.Albedo = tex.r * _TintR.rgb + tex.g * _TintG.rgb + tex.b * _TintB.rgb;

			//o.Albedo = fresnel;

			fixed texOpacity = tex.r * _TintR.a + tex.g * _TintG.a + tex.b * _TintB.a;

			o.Alpha = saturate(texOpacity + fresnel * texOpacity);

			o.Emission = 0.5 * fresnelColor;

            //o.Alpha = normalize(mainTex.a * (_Color.a + fresnel) / 2);
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}