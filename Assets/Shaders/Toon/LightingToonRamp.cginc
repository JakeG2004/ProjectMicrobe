#ifndef TOON_RAMP_INCLUDED
#define TOON_RAMP_INCLUDED

sampler2D _Ramp;

struct SurfaceOutputCustom {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	fixed Alpha;
};

inline half4 LightingToonRamp(SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) {
	s.Normal = normalize(s.Normal);
	fixed ndl = max(0, dot(s.Normal, lightDir) * 0.5 + 0.5);
			
	fixed3 ramp = tex2D(_Ramp, fixed2(ndl, ndl));
#if !(POINT) && !(SPOT)
		ramp *= atten;
#endif

	//AO stored in alpha
	ramp *= s.Alpha;

	ramp = lerp(unity_ShadowColor.rgb, fixed3(1, 1, 1), ramp);
	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ramp;
	c.a = 1;
#if (POINT || SPOT)
		c.rgb *= atten;
#endif

	return c;
}

#endif