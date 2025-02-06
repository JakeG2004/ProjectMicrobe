Shader "Landon/Toon/Tint Mask/SSS" {
	Properties {
		_MainTex ("Tint Channels (RGB) SSS Mask (A)", 2D) = "white" {}
		_TintR ("Tint R (RGB)", Color) = (.5,.5,.5,1)
		_TintG ("Tint G (RGB)", Color) = (.5,.5,.5,1)
		_TintB ("Tint B (RGB)", Color) = (.5,.5,.5,1)
		
		_SSS("SSS Color (RGB), SSS Intensity (A)", Color) = (1,0,0,0.5)

		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom fullforwardshadows
		//#pragma target 2.0
		//#pragma glsl
		
		sampler2D _MainTex, _Ramp;
		half4 _TintR, _TintG, _TintB, _SSS;
		
		struct Input {
			half2 uv_MainTex;
		};
		
		//Custom SurfaceOutput
		struct SurfaceOutputCustom {
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
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
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp;
			c.a = s.Alpha;
		#if (POINT || SPOT)
			c.rgb *= atten;
		#endif
		
		
		
		
		
		// SSS ........
		// SSS Mask stored in Alpha
		
		c.rgb += (1.0 - atten) * _SSS.rgb * _SSS.a * s.Alpha;
		
		
		
		
		
			return c;
		}
		
		
		
		
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.r * _TintR.rgb + tex.g * _TintG.rgb + tex.b * _TintB.rgb;
			
			
			//Store SSS in Alpha
			o.Alpha = tex.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}