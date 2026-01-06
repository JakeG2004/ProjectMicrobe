Shader "Landon/UI/Tint" {
    Properties {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}

        _MaskA ("Tint Mask A (RGB)", 2D) = "black" {}
        _ColorAR ("Tint Color A R", Color) = (1,1,1,1)
        _ColorAG ("Tint Color A G", Color) = (1,1,1,1)
        _ColorAB ("Tint Color A B", Color) = (1,1,1,1)

		_MaskB ("Tint Mask B (RGB)", 2D) = "black" {}
		_ColorBR ("Tint Color B R", Color) = (1,1,1,1)
        _ColorBG ("Tint Color B G", Color) = (1,1,1,1)
        _ColorBB ("Tint Color B B", Color) = (1,1,1,1)

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex, _MaskA, _MaskB;
            float4 _MainTex_ST;

            float4 _ColorAR, _ColorAG, _ColorAB, _ColorBR, _ColorBG, _ColorBB;

            struct appdata {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {



				fixed4 baseCol = tex2D(_MainTex, i.uv) * i.color;
				fixed4 maskA   = tex2D(_MaskA, i.uv);
				fixed4 maskB   = tex2D(_MaskB, i.uv);

				fixed aEnable = maskA.a * (1 - maskB.a);
				fixed bEnable = maskB.a;

				// Accumulate color suppression
				fixed3 multColor = 0;

				// Mask A channels
				multColor += maskA.r * aEnable * (1 - _ColorAR.rgb);
				multColor += maskA.g * aEnable * (1 - _ColorAG.rgb);
				multColor += maskA.b * aEnable * (1 - _ColorAB.rgb);

				// Mask B channels
				multColor += maskB.r * bEnable * (1 - _ColorBR.rgb);
				multColor += maskB.g * bEnable * (1 - _ColorBG.rgb);
				multColor += maskB.b * bEnable * (1 - _ColorBB.rgb);

				// Apply
				baseCol.rgb *= 1 - saturate(multColor);



				/*

                fixed4 baseCol = tex2D(_MainTex, i.uv) * i.color;
                fixed4 maskA = tex2D(_MaskA, i.uv);
				fixed4 maskB = tex2D(_MaskB, i.uv);

				fixed3 multColor = saturate(maskA.r - (1 - maskA.a) - maskB.a) * (1 - _ColorAR.rgb);
				multColor += saturate(maskA.g - (1 - maskA.a) - maskB.a) * (1 - _ColorAG.rgb);
				multColor += saturate(maskA.b - (1 - maskA.a) - maskB.a) * (1 - _ColorAB.rgb);

				multColor += saturate(maskB.r - (1 - maskB.a)) * (1 - _ColorBR.rgb);
				multColor += saturate(maskB.g - (1 - maskB.a)) * (1 - _ColorBG.rgb);
				multColor += saturate(maskB.b - (1 - maskB.a)) * (1 - _ColorBB.rgb);

				baseCol.rgb *= 1 - multColor;

				*/
				


				/*
				fixed3 multColor = fixed3(1,1,1);
				multColor = lerp(multColor.rgb, _ColorC.rgb, mask.b);
				multColor = lerp(multColor.rgb, _ColorB.rgb, mask.g);
				multColor = lerp(multColor.rgb, _ColorA.rgb, mask.r);

				baseCol.rgb *= multColor;
				*/

               // baseCol.rgb *=
				//mask.r * _ColorA.rgb +
				//mask.g * _ColorB.rgb +
				//mask.b * _ColorC.rgb;




                return baseCol;
            }
            ENDCG
        }
    }
}