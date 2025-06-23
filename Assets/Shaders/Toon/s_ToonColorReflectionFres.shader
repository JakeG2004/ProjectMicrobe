Shader "Landon/Toon/Color Reflection Fresnel" {
	Properties {
		_Color ("Color (RGB), Gloss (A)", Color) = (1,1,1,1)
		_FresColor ("Fresnel Color(RGB), Fresnel Angle (A)", Color) = (0,0.3,0.5,1)
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf ToonRamp fullforwardshadows
		#include "LightingToonRamp.cginc"
		
		fixed4 _Color, _FresColor;

		struct Input {
			fixed4 color : COLOR;
            half3 viewDir;
			float3 worldRefl;
		};
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			half fresnel = pow(1 - dot(normalize(IN.viewDir), normalize(IN.worldRefl)), _FresColor.a * 5) / 10;
			fixed3 fresnelColor = _FresColor.rgb * saturate(fresnel);

			o.Albedo = _Color.rgb;
			o.Emission = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, IN.worldRefl).rgb * _Color.a * 3 + fresnelColor;
		}
		ENDCG
	}
	Fallback "Diffuse"
}



/*  with cubemap input AND custom ramp light in code

Shader "Landon/Toon/Color Cubemap Fresnel" {
	Properties {
		_Color ("Color (RGB), Cube Gloss (A)", Color) = (1,1,1,1)
		_FresColor ("Fresnel Color(RGB), Fresnel Angle (A)", Color) = (0,0.3,0.5,1)
		_Cube ("Cubemap", CUBE) = "" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom fullforwardshadows
		//#pragma target 2.0
		//#pragma glsl
		
		sampler2D _Ramp;
		samplerCUBE _Cube;
		fixed4 _Color, _FresColor;

		struct Input {
			fixed4 color : COLOR;
            half3 viewDir;
			float3 worldRefl;
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

			half fresnel = pow(1 - dot(normalize(IN.viewDir), normalize(IN.worldRefl)), _FresColor.a * 5) / 10;  // for inverse option: _FresColor.a*10 - 5) / 10;
			fixed3 fresnelColor = _FresColor.rgb * saturate(fresnel);


			o.Albedo = _Color.rgb;
			// o.Emission = texCUBE (_Cube, IN.worldRefl).rgb;

			o.Emission = texCUBE(_Cube, IN.worldRefl).rgb * _Color.a * 3 + fresnelColor;
		}
		ENDCG
	}
	Fallback "Diffuse"
}   */