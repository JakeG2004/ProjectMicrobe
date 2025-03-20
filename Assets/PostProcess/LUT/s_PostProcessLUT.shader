Shader "Landon/PostProcess/LUT" {
    Properties {
        _MainTex("Base (RGB)", 2D) = "" {}
        _LutTexA("LUT Texture A", 3D) = "" {}
        _LutTexB("LUT Texture B", 3D) = "" {}
        _LutBlendAmount("LUT Blend Amount", Range(0, 1)) = 0.5
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    // Vertex data structure
    struct appdata {
        fixed4 pos : POSITION;
        fixed2 uv : TEXCOORD0;
    };

    struct v2f {
        fixed4 pos : SV_POSITION;
        fixed2 uv : TEXCOORD0;
    };

    // Declare textures
    uniform sampler2D _MainTex;
    uniform sampler3D _LutTexA;
    uniform sampler3D _LutTexB;
    uniform fixed _LutBlendAmount;

    // Vertex shader
    v2f vert(appdata i) {
        v2f o;
        o.pos = UnityObjectToClipPos(i.pos);
        o.uv = i.uv;
        return o;
    }

    // Fragment shader
    fixed4 frag(v2f i) : SV_Target {
        // Sample the base texture
        fixed4 c = tex2D(_MainTex, i.uv);

        // Apply blended LUT effect
        fixed4 lutColorA = tex3D(_LutTexA, c.rgb * 0.9375 + 0.03125); // LUT A
        fixed4 lutColorB = tex3D(_LutTexB, c.rgb * 0.9375 + 0.03125); // LUT B

        // Blend the two LUT colors based on _LutBlendAmount
        c = lerp(lutColorA, lutColorB, _LutBlendAmount);

        return c;
    }

    ENDCG

    Subshader {
        Pass {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
    Fallback off
}