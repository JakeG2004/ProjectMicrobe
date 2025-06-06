Shader "Landon/Toon/Terrain/4tile4Untile_GRASS_MASK"
{
    Properties
    {
        [TCP2HeaderHelp(Base)]
        [TCP2Separator]

        [TCP2Header(Ramp Shading)]
        _RampSmoothing ("Smoothing", Range(0.001,1)) = 0.5
        [TCP2Separator]
        [TCP2HeaderHelp(Terrain)]
        _HeightTransition ("Height Smoothing", Range(0, 1.0)) = 0.0
        [HideInInspector] TerrainMeta_maskMapTexture ("Mask Map", 2D) = "white" {}
        [Toggle(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)] _EnableInstancedPerPixelNormal("Enable Instanced per-pixel normal", Float) = 1.0
        [TCP2Separator]

        [TCP2Header(Custom Blending)]
        _CustomBlendFactor ("Custom Blend Factor (Layers 2 & 3 Influence)", Range(0, 1)) = 0.0
        [TCP2Color(HDR)] _GreenToBrownColor ("Green To Brown Color", Color) = (0.3,0.2,0.1,1) // NEW PROPERTY for remapping green
        [TCP2Separator]

        [TCP2TextureSingleLine] _NoTileNoiseTex ("Non-repeating Tiling Noise Texture", 2D) = "black" {}

        [HideInInspector] _Splat0 ("Layer 0 Albedo", 2D) = "gray" {}
        [HideInInspector] _Splat1 ("Layer 1 Albedo", 2D) = "gray" {}
        [HideInInspector] _Splat2 ("Layer 2 Albedo", 2D) = "gray" {}
        [HideInInspector] _Splat3 ("Layer 3 Albedo", 2D) = "gray" {}
        [HideInInspector] _Splat4 ("Layer 4 Albedo", 2D) = "gray" {}
        [HideInInspector] _Splat5 ("Layer 5 Albedo", 2D) = "gray" {}
        [HideInInspector] _Splat6 ("Layer 6 Albedo", 2D) = "gray" {}
        [HideInInspector] _Splat7 ("Layer 7 Albedo", 2D) = "gray" {}
        [HideInInspector] [NoScaleOffset] _Mask0 ("Layer 0 Mask", 2D) = "gray" {}
        [HideInInspector] [NoScaleOffset] _Mask1 ("Layer 1 Mask", 2D) = "gray" {}
        [HideInInspector] [NoScaleOffset] _Mask2 ("Layer 2 Mask", 2D) = "gray" {}
        [HideInInspector] [NoScaleOffset] _Mask3 ("Layer 3 Mask", 2D) = "gray" {}
        [HideInInspector] [NoScaleOffset] _Mask4 ("Layer 4 Mask", 2D) = "gray" {}
        [HideInInspector] [NoScaleOffset] _Mask5 ("Layer 5 Mask", 2D) = "gray" {}
        [HideInInspector] [NoScaleOffset] _Mask6 ("Layer 6 Mask", 2D) = "gray" {}
        [HideInInspector] [NoScaleOffset] _Mask7 ("Layer 7 Mask", 2D) = "gray" {}

        // Avoid compile error if the properties are ending with a drawer
        [HideInInspector] __dummy__ ("unused", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue"="Geometry-100"
            "TerrainCompatible"="True"
            "SplatCount"="8"
        }

        CGINCLUDE

        #include "UnityCG.cginc"
        #include "UnityLightingCommon.cginc"    // needed for LightColor

        // Texture/Sampler abstraction
        #define TCP2_TEX2D_WITH_SAMPLER(tex)                        UNITY_DECLARE_TEX2D(tex)
        #define TCP2_TEX2D_NO_SAMPLER(tex)                          UNITY_DECLARE_TEX2D_NOSAMPLER(tex)
        #define TCP2_TEX2D_SAMPLE(tex, samplertex, coord)           UNITY_SAMPLE_TEX2D_SAMPLER(tex, samplertex, coord)
        #define TCP2_TEX2D_SAMPLE_LOD(tex, samplertex, coord, lod)  UNITY_SAMPLE_TEX2D_SAMPLER_LOD(tex, samplertex, coord, lod)

        // Terrain

        //================================================================
        // Terrain Shader specific

        //----------------------------------------------------------------
        // Per-layer variables

        CBUFFER_START(_Terrain)
            float4 _Control_ST;
            float4 _Control_TexelSize;
            half _HeightTransition;
            half _DiffuseHasAlpha0, _DiffuseHasAlpha1, _DiffuseHasAlpha2, _DiffuseHasAlpha3;
            half _LayerHasMask0, _LayerHasMask1, _LayerHasMask2, _LayerHasMask3;
            // half4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;

            float4 _Control1_ST;
            float4 _Control1_TexelSize;
            half _DiffuseHasAlpha4, _DiffuseHasAlpha5, _DiffuseHasAlpha6, _DiffuseHasAlpha7;
            half _LayerHasMask4, _LayerHasMask5, _LayerHasMask6, _LayerHasMask7;
            // half4 _Splat4_ST, _Splat5_ST, _Splat6_ST, _Splat7_ST;

            #ifdef UNITY_INSTANCING_ENABLED
                float4 _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
                float4 _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
            #endif
            #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
            #endif
        CBUFFER_END

        //----------------------------------------------------------------
        // Terrain textures

        TCP2_TEX2D_WITH_SAMPLER(_Control);
        TCP2_TEX2D_WITH_SAMPLER(_Control1);

        #if defined(TERRAIN_BASE_PASS)
            TCP2_TEX2D_WITH_SAMPLER(_MainTex);
        #endif

        //----------------------------------------------------------------
        // Terrain Instancing

        #if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
            #define ENABLE_TERRAIN_PERPIXEL_NORMAL
        #endif

        #ifdef UNITY_INSTANCING_ENABLED
            TCP2_TEX2D_NO_SAMPLER(_TerrainHeightmapTexture);
            TCP2_TEX2D_WITH_SAMPLER(_TerrainNormalmapTexture);
        #endif

        UNITY_INSTANCING_BUFFER_START(Terrain)
            UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData)  // float4(xBase, yBase, skipScale, ~)
        UNITY_INSTANCING_BUFFER_END(Terrain)

        void TerrainInstancing(inout float4 positionOS, inout float3 normal, inout float2 uv)
        {
        #ifdef UNITY_INSTANCING_ENABLED
            float2 patchVertex = positionOS.xy;
            float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

            float2 sampleCoords = (patchVertex.xy + instanceData.xy) * instanceData.z; // (xy + float2(xBase,yBase)) * skipScale
            float height = UnpackHeightmap(_TerrainHeightmapTexture.Load(int3(sampleCoords, 0)));

            positionOS.xz = sampleCoords * _TerrainHeightmapScale.xz;
            positionOS.y = height * _TerrainHeightmapScale.y;

            #ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
                normal = float3(0, 1, 0);
            #else
                normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb * 2 - 1;
            #endif
            uv = sampleCoords * _TerrainHeightmapRecipSize.zw;
        #endif
        }

        void TerrainInstancing(inout float4 positionOS, inout float3 normal)
        {
            float2 uv = { 0, 0 };
            TerrainInstancing(positionOS, normal, uv);
        }

        //----------------------------------------------------------------
        // Terrain Holes

        #if defined(_ALPHATEST_ON)
            TCP2_TEX2D_WITH_SAMPLER(_TerrainHolesTexture);

            void ClipHoles(float2 uv)
            {
                float hole = TCP2_TEX2D_SAMPLE(_TerrainHolesTexture, _TerrainHolesTexture, uv).r;
                clip(hole == 0.0f ? -1 : 1);
            }
        #endif

        //----------------------------------------------------------------
        // Height-based blending

        void HeightBasedSplatModify_8_Layers(inout half4 splatControl, inout half4 splatControl1, in half4 splatHeight, in half4 splatHeight1)
        {
            // We multiply by the splat Control weights to get combined height
            splatHeight *= splatControl.rgba;
            splatHeight1 *= splatControl1.rgba;

            half maxHeight = max(splatHeight.r, max(splatHeight.g, max(splatHeight.b, splatHeight.a)));
            half maxHeight1 = max(splatHeight1.r, max(splatHeight1.g, max(splatHeight1.b, splatHeight1.a)));
            maxHeight = max(maxHeight, maxHeight1);

            // Ensure that the transition height is not zero.
            half transition = max(_HeightTransition, 1e-5);

            // This sets the highest splat to "transition", and everything else to a lower value relative to that
            // Then we clamp this to zero and normalize everything
            half4 weightedHeights = splatHeight + transition - maxHeight.xxxx;
            weightedHeights = max(0, weightedHeights);
            half4 weightedHeights1 = splatHeight1 + transition - maxHeight.xxxx;
            weightedHeights1 = max(0, weightedHeights1);

            // We need to add an epsilon here for active layers (hence the blendMask again)
            // so that at least a layer shows up if everything's too low.
            weightedHeights = (weightedHeights + 1e-6) * splatControl;
            weightedHeights1 = (weightedHeights1 + 1e-6) * splatControl1;

            // Normalize (and clamp to epsilon to keep from dividing by zero)
            half sumHeight = max(dot(weightedHeights, half4(1, 1, 1, 1)), 1e-6);
            half sumHeight1 = max(dot(weightedHeights1, half4(1, 1, 1, 1)), 1e-6);
            sumHeight = max(sumHeight, sumHeight1);
            splatControl = weightedHeights / sumHeight.xxxx;
            splatControl1 = weightedHeights1 / sumHeight.xxxx;
        }

        // Shader Properties
        sampler2D _Splat0;
        sampler2D _Splat1;
        sampler2D _Splat2;
        sampler2D _Splat3;
        TCP2_TEX2D_NO_SAMPLER(_Splat4);
        TCP2_TEX2D_NO_SAMPLER(_Splat5);
        TCP2_TEX2D_NO_SAMPLER(_Splat6);
        TCP2_TEX2D_NO_SAMPLER(_Splat7);
        TCP2_TEX2D_WITH_SAMPLER(_Mask0);
        TCP2_TEX2D_NO_SAMPLER(_Mask1);
        TCP2_TEX2D_NO_SAMPLER(_Mask2);
        TCP2_TEX2D_NO_SAMPLER(_Mask3);
        TCP2_TEX2D_WITH_SAMPLER(_Mask4);
        TCP2_TEX2D_NO_SAMPLER(_Mask5);
        TCP2_TEX2D_NO_SAMPLER(_Mask6);
        TCP2_TEX2D_NO_SAMPLER(_Mask7);

        // Shader Properties (ST properties for layers 0-3 are here, 4-7 are not present in original)
        float4 _Splat0_ST;
        float4 _Splat1_ST;
        float4 _Splat2_ST;
        float4 _Splat3_ST;
        float4 _Splat4_ST;
        float4 _Splat5_ST;
        float4 _Splat6_ST;
        float4 _Splat7_ST;

        float _RampSmoothing;
        float _CustomBlendFactor;
        fixed4 _GreenToBrownColor; // NEW DECLARATION

        // Non-repeating tiling
        sampler2D _NoTileNoiseTex;
        float4 _NoTileNoiseTex_TexelSize;

        // Non-repeating tiling texture fetch function
        float4 tex2D_noTile(sampler2D samp, in float2 uv)
        {
            float k = tex2D(_NoTileNoiseTex, (1/_NoTileNoiseTex_TexelSize.zw) * uv).a;
            float index = k*8.0;
            float i = floor(index);
            float f = frac(index);
            float2 offa = sin(float2(3.0,7.0)*(i+0.0));
            float2 offb = sin(float2(3.0,7.0)*(i+1.0));
            float2 dx = ddx(uv);
            float2 dy = ddy(uv);
            float4 cola = tex2Dgrad(samp, uv + offa, dx, dy);
            float4 colb = tex2Dgrad(samp, uv + offb, dx, dy);
            return lerp(cola, colb, smoothstep(0.2,0.8,f-0.1*dot(cola-colb, 1)));
        }

		half GetLuminance(half3 color)
		{
			return dot(color, half3(0.299, 0.587, 0.114)); // Standard NTSC luminance weights
		}

        ENDCG

        // Main Surface Shader

        CGPROGRAM

        #pragma surface surf ToonyColorsCustom vertex:vertex_surface exclude_path:deferred exclude_path:prepass keepalpha nolightmap nolppv addshadow

        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        #pragma target 3.0

        //================================================================
        // SHADER KEYWORDS

        #pragma shader_feature_local _TERRAIN_INSTANCED_PERPIXEL_NORMAL
        #pragma multi_compile_local_fragment __ _ALPHATEST_ON

        //================================================================
        // STRUCTS

        // Vertex input
        struct appdata_tcp2
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord0 : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
            float4 texcoord2 : TEXCOORD2;
            half4 tangent : TANGENT;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Input
        {
            float2 texcoord0;
        };

        //================================================================

        // Custom SurfaceOutput
        struct SurfaceOutputCustom
        {
            half atten;
            half3 Albedo;
            half3 Normal;
            half3 Emission;
            half Specular;
            half Gloss;
            half Alpha;

            Input input;

            half terrainWeight;
            half terrainWeight1;

            // Shader Properties
            float __rampThreshold;
            float __rampSmoothing;
            float3 __highlightColor;
            float3 __shadowColor;
            float __ambientIntensity;
        };

        //================================================================
        // VERTEX FUNCTION

        void vertex_surface(inout appdata_tcp2 v, out Input output)
        {
            UNITY_INITIALIZE_OUTPUT(Input, output);

            TerrainInstancing(v.vertex, v.normal, v.texcoord0.xy);
                v.tangent.xyz = cross(v.normal, float3(0,0,1));
                v.tangent.w = -1;

            // Texture Coordinates
            output.texcoord0 = v.texcoord0.xy;

        }

        //================================================================
        // SURFACE FUNCTION

		// Start of the surf function
		void surf(Input input, inout SurfaceOutputCustom output)
		{
			// Shader Properties Sampling
			float4 __layer0Mask = ( TCP2_TEX2D_SAMPLE(_Mask0, _Mask0, input.texcoord0.xy * _Splat0_ST.xy + _Splat0_ST.zw).rgba );
			float __layer0HeightSource = ( __layer0Mask.b );
			float __layer0HeightOffset = ( .0 );
			float4 __layer1Mask = ( TCP2_TEX2D_SAMPLE(_Mask1, _Mask0, input.texcoord0.xy * _Splat1_ST.xy + _Splat1_ST.zw).rgba );
			float __layer1HeightSource = ( __layer1Mask.b );
			float __layer1HeightOffset = ( .0 );
			float4 __layer2Mask = ( TCP2_TEX2D_SAMPLE(_Mask2, _Mask0, input.texcoord0.xy * _Splat2_ST.xy + _Splat2_ST.zw).rgba );
			float __layer2HeightSource = ( __layer2Mask.b );
			float __layer2HeightOffset = ( .0 );
			float4 __layer3Mask = ( TCP2_TEX2D_SAMPLE(_Mask3, _Mask0, input.texcoord0.xy * _Splat3_ST.xy + _Splat3_ST.zw).rgba );
			float __layer3HeightSource = ( __layer3Mask.b );
			float __layer3HeightOffset = ( .0 );
			float4 __layer4Mask = ( TCP2_TEX2D_SAMPLE(_Mask4, _Mask4, input.texcoord0.xy * _Splat4_ST.xy + _Splat4_ST.zw).rgba );
			float __layer4HeightSource = ( __layer4Mask.b );
			float __layer4HeightOffset = ( .0 );
			float4 __layer5Mask = ( TCP2_TEX2D_SAMPLE(_Mask5, _Mask4, input.texcoord0.xy * _Splat5_ST.xy + _Splat5_ST.zw).rgba );
			float __layer5HeightSource = ( __layer5Mask.b );
			float __layer5HeightOffset = ( .0 );
			float4 __layer6Mask = ( TCP2_TEX2D_SAMPLE(_Mask6, _Mask4, input.texcoord0.xy * _Splat6_ST.xy + _Splat6_ST.zw).rgba );
			float __layer6HeightSource = ( __layer6Mask.b );
			float __layer6HeightOffset = ( .0 );
			float4 __layer7Mask = ( TCP2_TEX2D_SAMPLE(_Mask7, _Mask4, input.texcoord0.xy * _Splat7_ST.xy + _Splat7_ST.zw).rgba );
			float __layer7HeightSource = ( __layer7Mask.b );
			float __layer7HeightOffset = ( .0 );

			// --- START OF CUSTOMIZATION: Capture original albedo and remap ---
			// Capture original albedo values for Layers 2 and 3 *before* remapping.
			float4 original_layer2_albedo = tex2D_noTile(_Splat2, input.texcoord0.xy * _Splat2_ST.xy + _Splat2_ST.zw).rgba;
			float4 original_layer3_albedo = tex2D_noTile(_Splat3, input.texcoord0.xy * _Splat3_ST.xy + _Splat3_ST.zw).rgba;

			// Define __layer0Albedo and __layer1Albedo (they are not remapped)
			float4 __layer0Albedo = ( tex2D_noTile(_Splat0, input.texcoord0.xy * _Splat0_ST.xy + _Splat0_ST.zw).rgba );
			float4 __layer1Albedo = ( tex2D_noTile(_Splat1, input.texcoord0.xy * _Splat1_ST.xy + _Splat1_ST.zw).rgba );

			// Define __layer2Albedo and __layer3Albedo *after* applying the green-to-brown remapping.
			// This uses the original luminance to maintain contrast.
			half greenStrengthL2 = original_layer2_albedo.g;
			half remap_L2_factor = saturate(greenStrengthL2 * _CustomBlendFactor * 2.0); // Adjust 2.0 multiplier for intensity
			half4 __layer2Albedo = lerp(original_layer2_albedo, _GreenToBrownColor * GetLuminance(original_layer2_albedo.rgb), remap_L2_factor);

			half greenStrengthL3 = original_layer3_albedo.g;
			half remap_L3_factor = saturate(greenStrengthL3 * _CustomBlendFactor * 2.0); // Adjust 2.0 multiplier for intensity
			half4 __layer3Albedo = lerp(original_layer3_albedo, _GreenToBrownColor * GetLuminance(original_layer3_albedo.rgb), remap_L3_factor);
			// --- END OF CUSTOMIZATION: Capture original albedo and remap ---

			// Define albedo for layers 4-7 as before
			float4 __layer4Albedo = ( TCP2_TEX2D_SAMPLE(_Splat4, _Mask4, input.texcoord0.xy * _Splat4_ST.xy + _Splat4_ST.zw).rgba );
			float4 __layer5Albedo = ( TCP2_TEX2D_SAMPLE(_Splat5, _Mask4, input.texcoord0.xy * _Splat5_ST.xy + _Splat5_ST.zw).rgba );
			float4 __layer6Albedo = ( TCP2_TEX2D_SAMPLE(_Splat6, _Mask4, input.texcoord0.xy * _Splat6_ST.xy + _Splat6_ST.zw).rgba );
			float4 __layer7Albedo = ( TCP2_TEX2D_SAMPLE(_Splat7, _Mask4, input.texcoord0.xy * _Splat7_ST.xy + _Splat7_ST.zw).rgba );

			float4 __mainColor = ( half4(1,1,1,1) );
			output.__rampThreshold = ( .5 );
			output.__rampSmoothing = ( _RampSmoothing );
			output.__highlightColor = ( half3(1,1,1) );
			output.__shadowColor = ( unity_ShadowColor.rgb );
			output.__ambientIntensity = ( 1.0 );

			output.input = input;

			// Terrain

			float2 terrainTexcoord0 = input.texcoord0.xy;

			#if defined(_ALPHATEST_ON)
				ClipHoles(terrainTexcoord0.xy);
			#endif

			#if defined(TERRAIN_BASE_PASS)

				half4 terrain_mixedDiffuse = TCP2_TEX2D_SAMPLE(_MainTex, _MainTex, terrainTexcoord0.xy).rgba;
				half3 normalTS = half3(0.0h, 0.0h, 1.0h);

			#else

				// Sample the splat control texture generated by the terrain
				float2 terrainSplatUV = (terrainTexcoord0.xy * (_Control_TexelSize.zw - 1.0f) + 0.5f) * _Control_TexelSize.xy;
				half4 terrain_splat_control_0_original = TCP2_TEX2D_SAMPLE(_Control, _Control, terrainSplatUV);
				terrainSplatUV = (terrainTexcoord0.xy * (_Control1_TexelSize.zw - 1.0f) + 0.5f) * _Control1_TexelSize.xy;
				half4 terrain_splat_control_1_original = TCP2_TEX2D_SAMPLE(_Control1, _Control1, terrainSplatUV);

				// Use a mutable copy for height-based blending.
				// NOTE: We no longer modify these based on _CustomBlendFactor to reduce weight.
				// The weight of Layers 2 and 3 will remain based on the original splat map and height blending.
				half4 terrain_splat_control_0_modified = terrain_splat_control_0_original;
				half4 terrain_splat_control_1_modified = terrain_splat_control_1_original;

				half height0 = __layer0HeightSource + __layer0HeightOffset;
				half height1 = __layer1HeightSource + __layer1HeightOffset;
				half height2 = __layer2HeightSource + __layer2HeightOffset;
				half height3 = __layer3HeightSource + __layer3HeightOffset;
				half height4 = __layer4HeightSource + __layer4HeightOffset;
				half height5 = __layer5HeightSource + __layer5HeightOffset;
				half height6 = __layer6HeightSource + __layer6HeightOffset;
				half height7 = __layer7HeightSource + __layer7HeightOffset;
				HeightBasedSplatModify_8_Layers(terrain_splat_control_0_modified, terrain_splat_control_1_modified, half4(height0, height1, height2, height3), half4(height4, height5, height6, height7));

				// --- CUSTOMIZATION: Removed old weight reduction logic ---
				// The previous lines that reduced terrain_splat_control_0_modified.b and .a
				// based on _CustomBlendFactor have been removed to prevent the "goes black" issue.
				// Also removed the associated re-normalization block here.
				// The remapping of green to brown is now done directly on __layer2Albedo and __layer3Albedo earlier.
				// --- END CUSTOMIZATION ---


				// Calculate overall weights and perform the texture blending using the MODIFIED controls
				half terrain_weight = dot(terrain_splat_control_0_modified, half4(1,1,1,1));
				half terrain_weight_1 = dot(terrain_splat_control_1_modified, half4(1,1,1,1));

				#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
					clip(terrain_weight == 0.0f ? -1 : 1);
					clip(terrain_weight_1 == 0.0f ? -1 : 1);
				#endif

				// Normalize weights before lighting (using modified controls)
				terrain_splat_control_0_modified /= (terrain_weight + terrain_weight_1 + 1e-3f);
				terrain_splat_control_1_modified /= (terrain_weight + terrain_weight_1 + 1e-3f);

			#endif // TERRAIN_BASE_PASS

			#if defined(INSTANCING_ON) && defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
				output.Normal = float3(0, 0, 1);
			#endif

			// Terrain normal, if using instancing and per-pixel normal map
			#if defined(UNITY_INSTANCING_ENABLED) && !defined(SHADER_API_D3D11_9X) && defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
				float2 terrainNormalCoords = (terrainTexcoord0.xy / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
				float3 geomNormal = normalize(TCP2_TEX2D_SAMPLE(_TerrainNormalmapTexture, _TerrainNormalmapTexture, terrainNormalCoords.xy).xyz * 2 - 1);

				output.Normal = geomNormal;
			#endif

			output.Albedo = half3(1,1,1);
			output.Alpha = 1;

			#if !defined(TERRAIN_BASE_PASS)
				// Sample textures that will be blended based on the MODIFIED terrain splat map.
				// Note: __layer2Albedo and __layer3Albedo now hold the remapped (green-to-brown) colors.
				half4 splat0 = __layer0Albedo;
				half4 splat1 = __layer1Albedo;
				half4 splat2 = __layer2Albedo; // This variable now contains the remapped color
				half4 splat3 = __layer3Albedo; // This variable now contains the remapped color
				half4 splat4 = __layer4Albedo;
				half4 splat5 = __layer5Albedo;
				half4 splat6 = __layer6Albedo;
				half4 splat7 = __layer7Albedo;

				// Redefine the macro to use the _modified splat controls
				#undef BLEND_TERRAIN_HALF4
				#define BLEND_TERRAIN_HALF4(outVariable, sourceVariable) \
					half4 outVariable = terrain_splat_control_0_modified.r * sourceVariable##0; \
					outVariable += terrain_splat_control_0_modified.g * sourceVariable##1; \
					outVariable += terrain_splat_control_0_modified.b * sourceVariable##2; \
					outVariable += terrain_splat_control_0_modified.a * sourceVariable##3; \
					outVariable += terrain_splat_control_1_modified.r * sourceVariable##4; \
					outVariable += terrain_splat_control_1_modified.g * sourceVariable##5; \
					outVariable += terrain_splat_control_1_modified.b * sourceVariable##6; \
					outVariable += terrain_splat_control_1_modified.a * sourceVariable##7;

				BLEND_TERRAIN_HALF4(terrain_mixedDiffuse, splat)

			#endif // !TERRAIN_BASE_PASS

			#if !defined(TERRAIN_BASE_PASS)
				output.terrainWeight = terrain_weight;
				output.terrainWeight1 = terrain_weight_1;
			#endif

			// Set the final Albedo for output
			output.Albedo = terrain_mixedDiffuse.rgb;
			output.Alpha = terrain_mixedDiffuse.a;

			// --- CUSTOMIZATION: Removed old final blend for dead grass color ---
			// The explicit lerp with _DeadGrassColor.rgb is no longer needed here,
			// as the remapping and contrast preservation happens directly on the layer albedos.
			// --- END CUSTOMIZATION ---

			output.Albedo *= __mainColor.rgb;

		}

        //================================================================
        // LIGHTING FUNCTION

        inline half4 LightingToonyColorsCustom(inout SurfaceOutputCustom surface, UnityGI gi)
        {

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

            #define     RAMP_THRESHOLD  surface.__rampThreshold
            #define     RAMP_SMOOTH     surface.__rampSmoothing
            ndl = saturate(ndl);
            ramp = smoothstep(RAMP_THRESHOLD - RAMP_SMOOTH*0.5, RAMP_THRESHOLD + RAMP_SMOOTH*0.5, ndl);

            ramp *= atten;

            #if !defined(UNITY_PASS_FORWARDBASE)
                ramp = lerp(half3(0,0,0), surface.__highlightColor, ramp);
            #else
                ramp = lerp(surface.__shadowColor, surface.__highlightColor, ramp);
            #endif

            half4 color;
            color.rgb = surface.Albedo * lightColor.rgb * ramp;
            color.a = surface.Alpha;

            half occlusion = 1;
            #ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
                half3 ambient = gi.indirect.diffuse;
                ambient *= surface.Albedo * occlusion * surface.__ambientIntensity;

                color.rgb += ambient;
            #endif

            #if !defined(TERRAIN_BASE_PASS)
                color.rgb *= saturate(surface.terrainWeight + surface.terrainWeight1);
            #endif

            return color;
        }

        void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom surface, UnityGIInput data, inout UnityGI gi)
        {
            half3 normal = surface.Normal;

            gi = UnityGlobalIllumination(data, 1.0, normal);

            surface.atten = data.atten;
            gi.light.color = _LightColor0.rgb;

        }

        ENDCG

        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
        UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
    }

    Dependency "BaseMapShader"    = "Hidden/Landon/Toony Colors Pro 2/Terrain8-BasePass"
    Dependency "BaseMapGenShader" = "Hidden/Landon/Toony Colors Pro 2/Terrain8-BaseGen"

    Fallback "Diffuse"
    //CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}
