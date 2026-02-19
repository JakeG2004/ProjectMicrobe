/* =========================================================
   LToon Glass

   Stylized transparent glass shader with toon lighting.

   Features:
   • Toon ramp diffuse (per-light)
   • Hard cutoff specular (attenuated per-light)
   • Per-light rim lighting
   • Rim boosts alpha for soft edge transparency
   • View-based cubemap reflection (applied once via emission)
   • Dithered shadow caster for semi-transparent shadows

   Lighting Model:
   - surf() handles:
       * Base color
       * Alpha (base + rim boost)
       * Environment reflection (emission)
   - LightingToonGlass() handles:
       * Ramp diffuse
       * Specular
       * Rim (per-light, attenuated)

   Designed for Forward rendering with multiple lights.
   ========================================================= */


   Shader "Landon/Toon/Glass" {
       Properties {
            [NoScaleOffset] _Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
            _Color("Base Color (RGB), Transparency (A)", Color) = (0.2,0.4,0.5,0.5)
            _RimColor("Rim Glow (RGB), Threshold (A)", Color) = (0.05,0.1,0.13,0.8)
            _RimSoftness("Rim Softness", Range(0,0.3)) = 0.01
            _RimAlpha("Rim Alpha Boost", Range(0,1)) = 0.5
           _SpecColor("Specular Color (RGB), Intensity (A)", Color) = (1,1,1,1)
           _SpecThreshold("Spec Threshold", Range(0.9,1)) = 0.99
           _ReflectionIntensity("Reflection Intensity", Range(0,2)) = 1
           _ShadowAlpha("Shadow Transparency", Range(0,1)) = 0.5
       }
   
       SubShader {
           Tags {
               "Queue"="Transparent"
               "RenderType"="Transparent"
               "IgnoreProjector"="True"
           }
   
           Blend SrcAlpha OneMinusSrcAlpha
           ZWrite Off
   
           CGPROGRAM
           #pragma surface surf ToonGlass alpha:fade fullforwardshadows
           #include "LightingToonRamp.cginc"
   
           fixed4 _Color;
           fixed4 _RimColor;
           float _RimSoftness;
           float _RimAlpha;
           float  _SpecThreshold;
           float  _ReflectionIntensity;
   
           struct Input {
               float3 viewDir;
               float3 worldRefl;
               INTERNAL_DATA
           };
   
           void surf(Input IN, inout SurfaceOutputCustom o) {
                o.Normal = half3(0,0,1);
                o.Albedo = _Color.rgb;

                float3 V = normalize(IN.viewDir);
                float3 N = normalize(o.Normal);
                float rimFactor = 1.0 - saturate(dot(N, V));

                float rimMask = smoothstep(
                    _RimColor.a - _RimSoftness,
                    _RimColor.a + _RimSoftness,
                    rimFactor
                );

                // Alpha boosted by rim
                o.Alpha = saturate(_Color.a + rimMask * _RimAlpha);

                // --- Environment Reflection (ONCE, not per light) ---
                float3 worldRefl = WorldReflectionVector(IN, N);
                half3 reflCol = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl).rgb;

                o.Emission = reflCol * _ReflectionIntensity;
                o.Occlusion = 1;
            }

   
            inline half4 LightingToonGlass(SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) {
                // === Base Toon Ramp ===
                ToonLightData lightData = EvaluateToonLight(s, lightDir, atten);
            
                half4 c;
                c.rgb = s.Albedo * lightData.rampColor;
                c.a   = s.Alpha;
            
                float3 N = s.Normal;
                float3 V = normalize(viewDir);
                float3 L = normalize(lightDir);
                float3 H = normalize(L + V);
            
                // -------------------------------------------------
                // Hard Cutoff Specular (Physically Attenuated)
                // -------------------------------------------------
                float NdotH = saturate(dot(N, H));
                float specMask = step(_SpecThreshold, NdotH);
                specMask *= atten;
            
                c.rgb += _SpecColor.rgb * specMask * _SpecColor.a * 3;
            
                // -------------------------------------------------
                // Rim Lighting (Per Light)
                // -------------------------------------------------
                float rimFactor = 1.0 - saturate(dot(N, V));
            
                float rimMask = smoothstep(_RimColor.a - _RimSoftness, _RimColor.a + _RimSoftness, rimFactor);
            
                // Attenuated per-light rim
                rimMask *= atten;
            
                c.rgb += _RimColor.rgb * rimMask;
            
                return c;
            }
            
   
           ENDCG
   
           // ----------------------------------------------------
           // Dithered Shadow Caster
           // ----------------------------------------------------
           Pass {
               Name "ShadowCaster"
               Tags { "LightMode"="ShadowCaster" }
   
               ZWrite On
               ColorMask 0
   
               CGPROGRAM
               #pragma vertex vert
               #pragma fragment frag
               #include "UnityCG.cginc"
   
               float _ShadowAlpha;
   
               struct v2f {
                   V2F_SHADOW_CASTER;
               };
   
               v2f vert(appdata_base v) {
                   v2f o;
                   TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                   return o;
               }
   
               float Dither4x4(float2 pos) {
                   int x = (int)fmod(pos.x, 4);
                   int y = (int)fmod(pos.y, 4);
                   int index = x + y * 4;
   
                   const float dither[16] = {
                       0.0,   0.5,   0.125, 0.625,
                       0.75,  0.25,  0.875, 0.375,
                       0.1875,0.6875,0.0625,0.5625,
                       0.9375,0.4375,0.8125,0.3125
                   };
   
                   return dither[index];
               }
   
               float frag(v2f i) : SV_Target {
                   float alpha = _ShadowAlpha;
                   float dither = Dither4x4(i.pos.xy);
   
                   clip(alpha - dither);
   
                   SHADOW_CASTER_FRAGMENT(i)
               }
               ENDCG
           }
       }
   
       FallBack "Transparent/Diffuse"
   }
   