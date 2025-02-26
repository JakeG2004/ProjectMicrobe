// Modified from 2016 Unity built-in shader
// Overrites since path is the same
// Uses Toon Ramp and unity_ShadowColor.rgb

Shader "Hidden/TerrainEngine/Details/BillboardWavingDoublePass" {
    Properties {
        _WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
        _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
        _Cutoff ("Cutoff", float) = 0.5
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
    }

CGINCLUDE
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

struct v2f {
    float4 pos : SV_POSITION;
    fixed4 color : COLOR;
    float4 uv : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};
v2f BillboardVert (appdata_full v) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    WavingGrassBillboardVert (v);
    o.color = v.color;

    o.color.rgb *= ShadeVertexLights (v.vertex, v.normal);

    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord;
    return o;
}
ENDCG

    SubShader {
        Tags {
            "Queue" = "Geometry+200"
            "IgnoreProjector"="True"
            "RenderType"="GrassBillboard"
            "DisableBatching"="True"
        }
        Cull Off
        LOD 200
        ColorMask RGB

CGPROGRAM
#pragma surface surf ToonyColorsCustom vertex:WavingGrassBillboardVert addshadow exclude_path:deferred

sampler2D _MainTex, _Ramp;
fixed _Cutoff;

struct Input {
    float2 uv_MainTex;
    fixed4 color : COLOR;
};



//Custom SurfaceOutput
struct SurfaceOutputCustom {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	fixed Alpha;
};
inline half4 LightingToonyColorsCustom (SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) {
		s.Normal = normalize(s.Normal);
		fixed ndl = max(0, dot(s.Normal, lightDir)*0.5 + 0.5);
			
		fixed3 ramp = tex2D(_Ramp, fixed2(ndl,ndl));
	#if !(POINT) && !(SPOT)
		ramp *= atten;
	#endif
		ramp = lerp(unity_ShadowColor.rgb,fixed3(1,1,1),ramp);
		fixed4 c = fixed4(s.Albedo * _LightColor0.rgb * ramp,1);
	#if (POINT || SPOT)
		c.rgb *= atten;
	#endif
		return c;
}




void surf (Input IN, inout SurfaceOutputCustom o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
    o.Albedo = c.rgb;
    o.Alpha = c.a;
    clip (o.Alpha - _Cutoff);
    o.Alpha *= IN.color.a;
}

ENDCG
    }

    Fallback Off
}
