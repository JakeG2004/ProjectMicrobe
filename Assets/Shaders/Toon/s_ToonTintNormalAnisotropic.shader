// Normal-Mapped TintMask shader using ToonRamp and Anisotropic Highlight
// Texture alpha is Occlusion
// reducing color alphas gives Emission
// AnisoTex.rgb is world space normal for highlight
// _AnisoTex.a masks where the highlight is applied
// HighlightColor controls the specular color
// Highlight controls the gloss and placement

Shader "Landon/Toon/Tint Mask/Normal, AO, Anisotropic" {
	Properties {
		[NoScaleOffset] _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		[Space]
		_MainTex ("Main Texture (RGB), AO (A)", 2D) = "white" {}
		[NoScaleOffset] _BumpMap("Bumpmap", 2D) = "bump" {}
		[NoScaleOffset] _AnisoTex("Anisotropic Direction (RGB), Mask (A)", 2D) = "bump" {}
		[Space]
		_TintR("Tint R (RGB), Glow (-a)", Color) = (.5,.5,.5,1)
		_TintG("Tint G (RGB), Glow (-a)", Color) = (.5,.5,.5,1)
		_TintB("Tint B (RGB), Glow (-a)", Color) = (.5,.5,.5,1)
		[Space]
		_HighlightColor("Highlight Color A (RGB), ", Color) = (1,1,1,1)
		_Highlight("Highlight Offset (R), Gloss (G), Bright (B)", Color) = (1,.29,0,1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom fullforwardshadows
		
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


			// Anisotropic Highlights
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

			
			// Preserve surface alpha
			c.a = s.Alpha;
			return c;
		}
		
		void surf (Input IN, inout SurfaceOutputCustom o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.r * _TintR.rgb + tex.g * _TintG.rgb + tex.b * _TintB.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.AnisoDir = tex2D(_AnisoTex, IN.uv_MainTex);
			o.Occlusion = tex.a;
			fixed3 r = tex.r * (1 - _TintR.a) * 2 * _TintR.rgb;
			fixed3 g = tex.g * (1 - _TintG.a) * 2 * _TintG.rgb;
			fixed3 b = tex.b * (1 - _TintB.a) * 2 * _TintB.rgb;
			o.Emission = r + g + b;
			o.Alpha = 1;
		}
		ENDCG
	}
	Fallback "Landon/Toon/Tint Mask/Normal"
}