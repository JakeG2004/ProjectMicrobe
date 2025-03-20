Shader "Landon/Sky/Skybox Rotation Fog" {
	Properties {
		[HideInInspector]_IsStandardPipeline("_IsStandardPipeline", Float) = 1
		[Gamma]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1)
		_Exposure("Exposure", Range( 0 , 8)) = 1
		[NoScaleOffset]_Tex("Cubemap (HDR)", CUBE) = "black" {}
		[Toggle(_ENABLEROTATION_ON)] _EnableRotation("Enable Rotation", Float) = 0
		[IntRange]_Rotation("Rotation", Range( 0 , 360)) = 0
		_RotationSpeed("Rotation Speed", Float) = 1
		[Toggle(_ENABLEFOG_ON)] _EnableFog("Enable Fog", Float) = 0
		_FogHeight("Fog Height", Range( 0 , 1)) = 1
		_FogSmoothness("Fog Smoothness", Range( 0.01 , 1)) = 0.01
		_FogFill("Fog Fill", Range( 0 , 1)) = 0.5
		[HideInInspector]_Tex_HDR("DecodeInstructions", Vector) = (0,0,0,0)
	}
	
	SubShader {
		Tags { "RenderType"="Background" "Queue"="Background" }
		LOD 100

		CGINCLUDE
		#pragma target 2.0
		ENDCG
		Blend Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		Pass {
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#pragma shader_feature _ENABLEFOG_ON
			#pragma shader_feature _ENABLEROTATION_ON


			struct appdata {
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};
			
			struct v2f {
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
			};

			uniform half _Advancedd;
			uniform half _Cubemapp;
			uniform half4 _Tex_HDR;
			uniform half _Rotationn;
			uniform half _Fogg;
			uniform half _SkyboxExtended;
			uniform half _IsStandardPipeline;
			uniform samplerCUBE _Tex;
			uniform half _Rotation;
			uniform half _RotationSpeed;
			uniform half4 _TintColor;
			uniform half _Exposure;
			uniform half _FogHeight;
			uniform half _FogSmoothness;
			uniform half _FogFill;
			inline half3 DecodeHDR1189( half4 Data ) {
				return DecodeHDR(Data, _Tex_HDR);
			}
			
			
			v2f vert ( appdata v ) {
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float lerpResult268 = lerp( 1.0 , ( unity_OrthoParams.y / unity_OrthoParams.x ) , unity_OrthoParams.w);
				half CAMERA_MODE300 = lerpResult268;
				float3 appendResult1129 = (float3(v.vertex.xyz.x , ( v.vertex.xyz.y * CAMERA_MODE300 ) , v.vertex.xyz.z));
				float3 normalizeResult1130 = normalize( appendResult1129 );
				float3 appendResult56 = (float3(cos( radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) ) , 0.0 , ( sin( radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) ) * -1.0 )));
				float3 appendResult266 = (float3(0.0 , CAMERA_MODE300 , 0.0));
				float3 appendResult58 = (float3(sin( radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) ) , 0.0 , cos( radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) )));
				float3 normalizeResult247 = normalize( v.vertex.xyz );
				#ifdef _ENABLEROTATION_ON
				float3 staticSwitch1164 = mul( float3x3(appendResult56, appendResult266, appendResult58), normalizeResult247 );
				#else
				float3 staticSwitch1164 = normalizeResult1130;
				#endif
				float3 vertexToFrag774 = staticSwitch1164;
				o.ase_texcoord.xyz = vertexToFrag774;
				
				o.ase_texcoord1 = v.vertex;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target {
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				float3 vertexToFrag774 = i.ase_texcoord.xyz;
				half4 Data1189 = texCUBE( _Tex, vertexToFrag774 );
				half3 localDecodeHDR1189 = DecodeHDR1189( Data1189 );
				half4 CUBEMAP222 = ( float4( localDecodeHDR1189 , 0.0 ) * unity_ColorSpaceDouble * _TintColor * _Exposure );
				float lerpResult678 = lerp( saturate( pow( (0.0 + (abs( i.ase_texcoord1.xyz.y ) - 0.0) * (1.0 - 0.0) / (_FogHeight - 0.0)) , ( 1.0 - _FogSmoothness ) ) ) , 0.0 , _FogFill);
				half FOG_MASK359 = lerpResult678;
				float4 lerpResult317 = lerp( unity_FogColor , CUBEMAP222 , FOG_MASK359);
				#ifdef _ENABLEFOG_ON
				float4 staticSwitch1179 = lerpResult317;
				#else
				float4 staticSwitch1179 = CUBEMAP222;
				#endif
				
				
				finalColor = staticSwitch1179;
				return finalColor;
			}
			ENDCG
		}
	}
	Fallback "Skybox/Cubemap"
}
