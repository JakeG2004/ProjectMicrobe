Shader "Jake/TerrainBrownOverlay"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.36, 0.25, 0.20, 0.5) // brown
        _MaskTex ("Mask", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
        Pass
        {
            // This is a comment
            ZWrite Off
            ColorMask RGB
            Blend SrcAlpha One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _TintColor;
            sampler2D _MaskTex;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 projUV : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.projUV = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.projUV.xy / i.projUV.w;
                fixed alpha = tex2D(_MaskTex, uv).a;
                return fixed4(_TintColor.rgb, _TintColor.a * alpha);
            }
            ENDCG
        }
    }
}
