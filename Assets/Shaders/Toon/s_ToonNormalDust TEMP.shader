
Shader "Landon/Toon/Normal Dust TEMP" {
    Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
        _MainTex ("Base Texture", 2D) = "white" {}
        [NoScaleOffset] _BumpMap("Base Bump", 2D) = "bump" {}
		_ColorMain("Base Color (RGB)", Color) = (0.671,0.463,0.353,1)
        [NoScaleOffset] _TopTex  ("Top Texture", 2D) = "white" {}
        [NoScaleOffset] _TopBump ("Top Bump", 2D) = "bump" {}
		_ColorTop("Top Color (RGB)", Color) = (0.427,0.682,0.145,1)
        _TopScale("Top Texture Scale", Float) = 1
        _UpAxis("Up Axis (0=X,1=Y,2=Z)", Range(0,2)) = 1
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf ToonRamp fullforwardshadows vertex:vert

		#include "LightingToonRamp.cginc"


        sampler2D _MainTex, _BumpMap, _TopTex, _TopBump;
        float _TopScale, _TopStrength, _UpAxis;
		half4 _ColorMain, _ColorTop;

        struct Input {
			float2 uv_Ramp;
            float2 uv_MainTex;
            float3 normalOS; // reliable object-space normal from vertex shader
        };

        // Vertex function to calculate object-space normals
        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float3 worldNormal = UnityObjectToWorldNormal(v.normal);
            float3 objectNormal = mul((float3x3)unity_WorldToObject, worldNormal);
            o.normalOS = normalize(objectNormal);
        }

        void surf(Input IN, inout SurfaceOutputCustom o) {
			fixed ao = tex2D(_MainTex, IN.uv_Ramp).a; // unscaled uv space
            // Base texture and normal
            fixed4 baseCol = tex2D(_MainTex, IN.uv_MainTex);
            float3 baseNormal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

            // Up-facing mask
            float n = 0;
            if (_UpAxis < 0.5)      n = IN.normalOS.x;
            else if (_UpAxis < 1.5) n = IN.normalOS.y;
            else                     n = IN.normalOS.z;

            float upMask = pow(saturate(n - 0.1), 4); // offset, sharpness

            // Top texture and normal
            float2 topUV = IN.uv_MainTex * _TopScale;
            fixed4 topCol = tex2D(_TopTex, topUV);
            float3 topNormal = UnpackNormal(tex2D(_TopBump, topUV));

			// Noise breakup from top texture alpha
			float noise = lerp(1.0, topCol.a, 0.9);
			float blendMask = saturate(upMask * 3 * noise); // strength

            // Blend colors and normals
			fixed aoMult = (0.2 * ao + 0.8);
			fixed3 combinedTexture = lerp(baseCol.rgb, topCol.rgb, blendMask);
			fixed3 combinedColor = lerp(_ColorMain.rgb, _ColorTop.rgb, blendMask);
            o.Albedo = combinedTexture * combinedColor * aoMult;
            o.Normal = normalize(lerp(baseNormal, topNormal, blendMask));

			o.Occlusion = aoMult; // (ao * 0.5 + 0.5);
            o.Alpha = 1;
			// o.Emission = 1-ao;
        }
        ENDCG
    }
    Fallback "Landon/Toon/Texture"
}