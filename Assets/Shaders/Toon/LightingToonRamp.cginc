#ifndef TOON_RAMP_INCLUDED
#define TOON_RAMP_INCLUDED

sampler2D _Ramp;

struct SurfaceOutputCustom {
    fixed3 Albedo;
    fixed3 Normal;
    fixed3 Emission;
    fixed Occlusion;
    fixed Alpha;
};

#include "UnityCG.cginc"
#include "AutoLight.cginc"

struct ToonLightData {
    fixed ndl;          // raw N·L
    fixed rampInput;    // shaped light factor (0–1)
    fixed3 rampColor;   // final blended light color
};


// =====================================================
// Core Stylized Light Evaluation
// =====================================================

inline ToonLightData EvaluateToonLight(SurfaceOutputCustom s, half3 lightDir, half atten) {
    ToonLightData o;

    s.Normal = normalize(s.Normal);

    o.ndl = dot(s.Normal, lightDir);

    // Light shaping
    o.rampInput = saturate(o.ndl * s.Occlusion);

    fixed3 ramp = tex2D(_Ramp, fixed2(o.rampInput, o.rampInput));

    #if (!POINT && !SPOT)
        ramp *= atten;
    #endif

    ramp = lerp(unity_ShadowColor.rgb, _LightColor0.rgb, ramp);

    #if (POINT || SPOT)
        ramp *= atten;
    #endif

    o.rampColor = ramp;

    return o;
}


// =====================================================
// Default Toon Lighting Model
// =====================================================

inline half4 LightingToonRamp(SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) {
    ToonLightData lightData = EvaluateToonLight(s, lightDir, atten);

    fixed4 c;
    c.rgb = s.Albedo * lightData.rampColor + s.Emission;
    c.a   = s.Alpha;

    return c;
}

#endif