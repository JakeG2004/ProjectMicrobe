Shader "Landon/Toon/Texture, Clip" {
	Properties {
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
		Cull Off
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom keepalpha fullforwardshadows addshadow 
		//#pragma target 2.0
		//#pragma glsl
		
		sampler2D _MainTex, _Ramp;
		fixed _Cutoff;

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
			return c;
		}
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb;
			
			clip(mainTex.a - _Cutoff);

			//o.Alpha = mainTex.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}