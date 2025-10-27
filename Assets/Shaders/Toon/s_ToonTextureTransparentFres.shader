// Basic Texture shader using ToonRamp
// Texture alpha is a mask
// Color alpha blends between mask giving Emission or Occlusion
// Color multiplies to Emission

Shader "Landon/Toon/Texture Transparent Fres" {
    Properties {
        [NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
        _MainTex ("Main Texture (RGB), NA (A)", 2D) = "white" {}
        _Color ("Glow Color (RGB), Transparency (A)", Color) = (1,1,1,1)
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
        fixed4 _Color;
        fixed4 _FresColor;

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

            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);

            o.Albedo = mainTex.rgb;
			//o.Albedo = fresnel;



			o.Alpha = saturate(_Color.a + fresnel * _Color.a);

			o.Emission = 0.5 * fresnelColor;

            //o.Alpha = normalize(mainTex.a * (_Color.a + fresnel) / 2);
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}