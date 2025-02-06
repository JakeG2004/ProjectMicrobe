Shader "Landon/Toon/Normal/AO Glow Clip" {
	Properties {
		_MainTex ("Main Texture (RGB) AO (A)", 2D) = "white" {}
		[NoScaleOffset] _GlowTex ("Glow (RGB)", 2D) = "black" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom fullforwardshadows
		//#pragma target 2.0
		//#pragma glsl
		
		sampler2D _MainTex, _GlowTex, _BumpMap, _Ramp;
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

			//AO stored in alpha
			ramp *= s.Alpha;

			ramp = lerp(unity_ShadowColor.rgb,fixed3(1,1,1),ramp);
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp;
			c.a = 1;
		#if (POINT || SPOT)
			c.rgb *= atten;
		#endif

			return c;
		}
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 glowTex = tex2D(_GlowTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			//store AO in Alpha
			o.Alpha = mainTex.a;

			//store cutout in glowTexA
			clip(glowTex.a - 0.5);

			o.Emission = glowTex.rgb * 2;
		}
		ENDCG
	}
	Fallback "Diffuse"
}