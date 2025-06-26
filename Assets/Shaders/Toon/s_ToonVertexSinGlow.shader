Shader "Landon/Toon/Vertex Sin, Glow" {
    Properties {
        [NoScaleOffset] _Ramp ("Ramp Texture (RGB)", 2D) = "gray" {}
        [MainTexture] _MainTex ("Albedo (RGB), Glow Mask (A)", 2D) = "white" {}
        _WindDirection ("Direction", Vector) = (1, 0, 0, 0)
        _WindStrength ("Strength", Range(0, 0.2)) = 0.025
        [NoScaleOffset] _WindMask ("Mask", 2D) = "white" {}
        _WindSpeed ("Speed", Range(0, 10)) = 2.5

        // Avoid compile error if the properties are ending with a drawer
        [HideInInspector] __dummy__ ("unused", Float) = 0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }

        CGINCLUDE
        #include "UnityCG.cginc"
        #include "UnityLightingCommon.cginc" // Needed for LightColor

        // Texture/Sampler abstraction
        #define TCP2_TEX2D_WITH_SAMPLER(tex) UNITY_DECLARE_TEX2D(tex)
        #define TCP2_TEX2D_NO_SAMPLER(tex) UNITY_DECLARE_TEX2D_NOSAMPLER(tex)
        #define TCP2_TEX2D_SAMPLE(tex, samplertex, coord) UNITY_SAMPLE_TEX2D_SAMPLER(tex, samplertex, coord)
        #define TCP2_TEX2D_SAMPLE_LOD(tex, samplertex, coord, lod) UNITY_SAMPLE_TEX2D_SAMPLER_LOD(tex, samplertex, coord, lod)

        // Shader Properties
        TCP2_TEX2D_WITH_SAMPLER(_WindMask);
        TCP2_TEX2D_WITH_SAMPLER(_MainTex);

        // Shader Properties (scalars and vectors)
        float _WindSpeed;
        float4 _WindDirection;
        float _WindStrength;
        float4 _MainTex_ST;
        fixed4 _HColor;
        fixed4 _SColor;
        sampler2D _Ramp;
        ENDCG

        // Main Surface Shader
        CGPROGRAM
        #pragma surface surf ToonyColorsCustom vertex:vertex_surface exclude_path:deferred exclude_path:prepass addshadow keepalpha nolightmap nolppv
        #pragma target 3.0

        // Structs
        struct appdata_tcp2 {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord0 : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
            float4 texcoord2 : TEXCOORD2;
            #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
                half4 tangent : TANGENT;
            #endif
            fixed4 vertexColor : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Input {
            float2 texcoord0;
        };

        // Custom SurfaceOutput
        struct SurfaceOutputCustom {
            half atten;
            half3 Albedo;
            half3 Normal;
            half3 Emission;
            half Specular;
            half Gloss;
			fixed Occlusion;
            half Alpha;
            Input input;

            // Shader Properties
            float3 __highlightColor;
            float3 __shadowColor;
            float __ambientIntensity;
        };

        // Vertex Function
        void vertex_surface(inout appdata_tcp2 v, out Input output) {
            UNITY_INITIALIZE_OUTPUT(Input, output);

            // Texture Coordinates
            output.texcoord0.xy = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;

            // Shader Properties Sampling
            float __windTimeOffset = v.vertexColor.g;
            float __windSpeed = _WindSpeed;
            float __windFrequency = 1.0;
            float4 __windSineScale2 = float4(2.3, 1.7, 1.4, 1.2);
            float __windSineStrength2 = 0.6;
            float3 __windDirection = _WindDirection.xyz;
            float3 __windMask = TCP2_TEX2D_SAMPLE_LOD(_WindMask, _WindMask, output.texcoord0.xy, 0).rgb;
            float __windStrength = _WindStrength;

            // Wind Animation
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float windTimeOffset = __windTimeOffset;
            float windSpeed = __windSpeed;
            float3 windFrequency = worldPos.xyz * __windFrequency;
            float windPhase = (_Time.y + windTimeOffset) * windSpeed;
            float3 windFactor = sin(windPhase + windFrequency);
            float4 windSin2scale = __windSineScale2;
            float windSin2strength = __windSineStrength2;
            windFactor += sin(windPhase.xxx * windSin2scale.www + windFrequency * windSin2scale.xyz) * windSin2strength;
            float3 windDir = normalize(__windDirection);
            float3 windMask = __windMask;
            worldPos.xyz += windDir * windFactor * windMask * __windStrength;
            v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
        }

        // Surface Function
        void surf(Input input, inout SurfaceOutputCustom output) {
            // Shader Properties Sampling
            float4 __albedo = TCP2_TEX2D_SAMPLE(_MainTex, _MainTex, input.texcoord0.xy).rgba;
            float4 __mainColor = float4(1, 1, 1, 1);
            float __alpha = __albedo.a * __mainColor.a;

            output.__highlightColor = fixed3(1, 1, 1);
            output.__shadowColor = unity_ShadowColor.rgb;
            output.__ambientIntensity = 1.0;

            output.input = input;

            output.Albedo = __albedo.rgb;
            output.Alpha = __alpha;

            output.Albedo *= __mainColor.rgb;

            output.Emission = (1 - __albedo.a) * 3 * __albedo.rgb;
        }

        // Lighting Function
        inline half4 LightingToonyColorsCustom(inout SurfaceOutputCustom surface, UnityGI gi) {
            half3 lightDir = gi.light.dir;
            #if defined(UNITY_PASS_FORWARDBASE)
                half3 lightColor = _LightColor0.rgb;
                half atten = surface.atten;
            #else
                half3 lightColor = _LightColor0.rgb;
                half atten = max(gi.light.color.r, max(gi.light.color.g, gi.light.color.b)) / max(_LightColor0.r, max(_LightColor0.g, _LightColor0.b));
            #endif

            half3 normal = normalize(surface.Normal);
            half ndl = dot(normal, lightDir);
            half3 ramp;

            // Define ramp threshold and smoothstep depending on context
            #define RAMP_TEXTURE _Ramp
            half2 rampUv = ndl.xx * 0.5 + 0.5;
			// Sharpen Ramp
			rampUv = smoothstep(0.3, 0.7, rampUv);
			// Apply AO
			rampUv *= surface.Occlusion;
			// Sample the ramp texture using rampUv as both U and V coordinates
            ramp = tex2D(RAMP_TEXTURE, rampUv).rgb;


            // Apply attenuation
            ramp *= atten;

            // Highlight/Shadow Colors
            ramp = lerp(surface.__shadowColor, surface.__highlightColor * lightColor.rgb, ramp);

            // Output color
            half4 color;
            color.rgb = surface.Albedo * ramp;
            color.a = surface.Alpha;

            // Apply indirect lighting (ambient)
            half occlusion = 1;
            #ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
                half3 ambient = gi.indirect.diffuse;
                ambient *= surface.Albedo * occlusion * surface.__ambientIntensity;
                color.rgb += ambient;
            #endif

            return color;
        }

        // GI Lighting
        void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom surface, UnityGIInput data, inout UnityGI gi) {
            half3 normal = surface.Normal;

            // GI without reflection probes
            gi = UnityGlobalIllumination(data, 1.0, normal);

            surface.atten = data.atten; // Transfer attenuation to lighting function
            gi.light.color = _LightColor0.rgb; // Remove attenuation
        }
        ENDCG
    }
    Fallback "Landon/Toon/Texture"
}