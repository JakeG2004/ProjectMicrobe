Shader "Landon/GlossyTransparentStandard" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _Glossiness("Glass Smoothness", Range(0,1)) = 0.5
        _Spec("Glass Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Blend One One // Additive
        // Blend OneMinusDstColor One // Soft additive

        Cull Back
        CGPROGRAM
        #pragma surface surf StandardSpecular //fullforwardshadows //alpha:premul
        #pragma target 3.0

        struct Input {
            float4 color : COLOR;
        };

        half _Glossiness, _Spec;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutputStandardSpecular o) {
            
            o.Albedo = _Color.rgb;
            o.Specular = _Spec;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
        //FallBack "Diffuse"
}


/*
     Properties {
        _Color("Color", Color) = (1,1,1,1)
        _Glossiness("Glass Smoothness", Range(0,1)) = 0.5
        _Metallic("Glass Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200


        Cull Back
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:premul
        #pragma target 3.0

        struct Input {
            float4 color : COLOR;
        };

        half _Glossiness, _Metallic;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutputStandard o) {
            
            o.Albedo = _Color.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
        FallBack "Diffuse"
}
*/