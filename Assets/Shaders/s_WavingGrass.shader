
// Modified from 2016 Unity built-in shader
// Overrites since path is the same
// Uses Toon Ramp and unity_ShadowColor.rgb

Shader "Hidden/TerrainEngine/Details/WavingDoublePass" {
Properties {
    _WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
    _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
    _WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
    _Cutoff ("Cutoff", float) = 0.5
	_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
}

SubShader {
    Tags {
        "Queue" = "Geometry+200"
        "IgnoreProjector"="True"
        "RenderType"="Grass"
        "DisableBatching"="True"
    }
    Cull Off
    LOD 200
    ColorMask RGB

CGPROGRAM
#pragma surface surf ToonyColorsCustom vertex:WavingGrassVert addshadow exclude_path:deferred
//#pragma surface surf ToonyColorsCustom vertex:WavingGrassVert fullforwardshadows exclude_path:deferred
#include "TerrainEngine.cginc"

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


// void surf (Input IN, inout SurfaceOutput o) {

void surf (Input IN, inout SurfaceOutputCustom o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
    o.Albedo = c.rgb;
    o.Alpha = c.a;
    clip (o.Alpha - _Cutoff);
    o.Alpha *= IN.color.a;
}
ENDCG
}

    SubShader {
        Tags {
            "Queue" = "Geometry+200"
            "IgnoreProjector"="True"
            "RenderType"="Grass"
        }
        Cull Off
        LOD 200
        ColorMask RGB

        Pass {
            Tags { "LightMode" = "Vertex" }
            Material {
                Diffuse (1,1,1,1)
                Ambient (1,1,1,1)
            }
            Lighting On
            ColorMaterial AmbientAndDiffuse
            AlphaTest Greater [_Cutoff]
            SetTexture [_MainTex] { combine texture * primary DOUBLE, texture }
        }
        Pass {
            Tags { "LightMode" = "VertexLMRGBM" }
            AlphaTest Greater [_Cutoff]
            BindChannels {
                Bind "Vertex", vertex
                Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
                Bind "texcoord", texcoord1 // main uses 1st uv
            }
            SetTexture [unity_Lightmap] {
                matrix [unity_LightmapMatrix]
                combine texture * texture alpha DOUBLE
            }
            SetTexture [_MainTex] { combine texture * previous QUAD, texture }
        }
    }

    Fallback Off
}
