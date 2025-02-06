Shader "Landon/Toon/Tint Mask/Normal, AO, Anisotropic" {
	Properties {
		_MainTex ("Main Texture (RGB) AO (A)", 2D) = "white" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		[NoScaleOffset] _AnisoTex("Anisotropic Direction (RGB) Mask (A)", 2D) = "bump" {}
		[Space]
		_TintR("Tint R (RGB)", Color) = (.5,.5,.5,1)
		_TintG("Tint G (RGB)", Color) = (.5,.5,.5,1)
		_TintB("Tint B (RGB)", Color) = (.5,.5,.5,1)
		[Space]
		_HighlightColor("Highlight Color A (RGB), ", Color) = (1,1,1,1)
		_Highlight("Highlight Offset (R), Gloss (G), Bright (B)", Color) = (1,.29,0,1)
		[Space]
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom fullforwardshadows
		//#pragma target 2.0
		//#pragma glsl
		
		sampler2D _MainTex, _BumpMap, _AnisoTex, _Ramp;
		fixed4 _TintR, _TintG, _TintB, _HighlightColor, _Highlight;

		struct Input {
			half2 uv_MainTex;
		};
		
		//Custom SurfaceOutput
		struct SurfaceOutputCustom {
			fixed3 Albedo;
			fixed3 Normal;
			fixed4 AnisoDir;
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




			half offset = _Highlight.r * 2 - 1;
			half gloss = _Highlight.g;
			half bright = _Highlight.b;

			fixed3 h = normalize(normalize(lightDir) + normalize(viewDir));
			float NdotL = saturate(dot(s.Normal, lightDir));
			fixed HdotA = dot(normalize(s.Normal + s.AnisoDir.rgb), h);
			float aniso = max(0, sin(radians((HdotA + offset + (0.1)) * 180)));

			half spec = saturate(dot(s.Normal, h));
			// blend between spec and aniso highlighting based on texture alpha.  and apply highlight  
			spec = saturate(pow(lerp(spec, aniso, s.AnisoDir.a), gloss * 128) * bright);

			// apply highlight color. and mask spec based on texture highlight
			c.rgb += saturate(_HighlightColor.rgb * spec * NdotL) * s.AnisoDir.a;




			return c;
		}
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.r * _TintR.rgb + tex.g * _TintG.rgb + tex.b * _TintB.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.AnisoDir = tex2D(_AnisoTex, IN.uv_MainTex);

			//store AO in Alpha
			o.Alpha = tex.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}