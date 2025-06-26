Shader "Landon/Toon/Tint Mask/Normal, AO, SSS" {
	Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_MainTex ("Tint Channels (RGB) SSS Mask (A)", 2D) = "white" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		_TintR ("Tint R (RGB)", Color) = (.5,.5,.5,1)
		_TintG ("Tint G (RGB)", Color) = (.5,.5,.5,1)
		_TintB ("Tint B (RGB)", Color) = (.5,.5,.5,1)
		_SSS("SSS Color (RGB), SSS Intensity (A)", Color) = (1,0,0,0.5)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom fullforwardshadows
		//#pragma target 2.0
		//#pragma glsl
		
		sampler2D _MainTex, _BumpMap, _Ramp;
		half4 _TintR, _TintG, _TintB, _SSS;
		
		struct Input {
			half2 uv_MainTex;
		};
		
		//Custom SurfaceOutput
		struct SurfaceOutputCustom {
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed Occlusion;
			fixed Alpha;
		};
		
		inline half4 LightingToonyColorsCustom (SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) {
			s.Normal = normalize(s.Normal);
			
			// Toon Ramp
			fixed ndl = max(0, dot(s.Normal, lightDir) * 0.5 + 0.5);
    
			fixed rampInput = smoothstep(0.3, 0.7, ndl);
			rampInput *= s.Occlusion;


			#if !defined(POINT) && !defined(SPOT) // directional cast shadows
					rampInput *= atten;
			#endif

			fixed3 ramp = tex2D(_Ramp, fixed2(rampInput, rampInput));
			ramp = lerp(unity_ShadowColor.rgb, _LightColor0.rgb, ramp);
    
			fixed4 c;
			c.rgb = s.Albedo * ramp + s.Emission;
    
			#if defined(POINT) || defined(SPOT)
					c.rgb *= atten;
			#endif
		
			// SSS Mask stored in Alpha
			c.rgb += (1.0 - atten) * _SSS.rgb * _SSS.a * s.Alpha;
		
			c.a = s.Alpha;
			return c;
		}
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.r * _TintR.rgb + tex.g * _TintG.rgb + tex.b * _TintB.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			//Store SSS in Alpha
			o.Alpha = tex.a;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Tint Mask/Normal"
}