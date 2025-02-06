Shader "Landon/Toon/Glow" {
	Properties {
		_Color ("Color (RGB), ", Color) = (1,1,1,1)
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom fullforwardshadows
		//#pragma target 2.0
		//#pragma glsl
		
		sampler2D _Ramp;
		fixed4 _Color;

		struct Input {
			fixed4 color : COLOR;
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
			o.Albedo = _Color.rgb;
			o.Emission = _Color.rgb * 2;
		}
		ENDCG
	}
	Fallback "Diffuse"
}