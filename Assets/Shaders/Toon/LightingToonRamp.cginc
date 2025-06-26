#ifndef TOON_RAMP_INCLUDED
#define TOON_RAMP_INCLUDED

// Ramp texture used to define stylized lighting falloff
sampler2D _Ramp;

// Custom surface output structure, simplified for toon shading
struct SurfaceOutputCustom
{
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	fixed Occlusion;
	fixed Alpha;
};

// You still need AutoLight.cginc and UnityCG.cginc for other lighting-related macros
// like _LightColor0 and unity_ShadowColor, but NOT specifically for SHADOW_ATTENUATION
// when using it this way. However, it's good practice to keep them if you might use other features.
#include "UnityCG.cginc"
#include "AutoLight.cginc" // Keep this for _LightColor0, unity_ShadowColor, etc.

inline half4 LightingToonRamp(SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten)
{
    // Normalize the surface normal for consistent lighting calculations
	s.Normal = normalize(s.Normal);
    
    // Compute dot product of normal and light direction, remapped from [-1, 1] to [0, 1]
	fixed ndl = max(0, dot(s.Normal, lightDir) * 0.5 + 0.5);
    
    // Sharpen the ramp by applying an S-curve to compress midrange values
	fixed rampInput = smoothstep(0.3, 0.7, ndl);
    
    // Apply ambient occlusion before ramp sampling for stylized AO effect
	rampInput *= s.Occlusion;
    
    // Sample the ramp texture using rampInput as both U and V coordinates
	fixed3 ramp = tex2D(_Ramp, fixed2(rampInput, rampInput));
    
	
	// Apply the 'atten' for directional lights before changing ramp colors.
	// this is for shadow recieving from other objects.
	// This 'atten' will be 0 in shadowed areas and 1 in lit areas.
	#if (!POINT && !SPOT)
		ramp *= atten; // THIS IS THE KEY CHANGE. Use the passed 'atten' parameter.
	#endif
	
    // Blend between Unity’s shadow color (darkest) and the light color (brightest)
    // The 'atten' parameter already contains the shadow attenuation combined with light attenuation.
	ramp = lerp(unity_ShadowColor.rgb, _LightColor0.rgb, ramp);
	
	
	// attenuation for distance falloff
	#if (POINT || SPOT)
		ramp *= atten;
	#endif
	
	fixed4 c;
    // Modulate albedo by ramp lighting and add emission
	c.rgb = s.Albedo * ramp + s.Emission;
    
	// Preserve surface alpha
	c.a = s.Alpha;

	return c;
}
#endif





/*  backup

#ifndef TOON_RAMP_INCLUDED
#define TOON_RAMP_INCLUDED
sampler2D _Ramp;

struct SurfaceOutputCustom {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	fixed Occlusion;
};

inline half4 LightingToonRamp(SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) {
	s.Normal = normalize(s.Normal);
	fixed ndl = max(0, dot(s.Normal, lightDir) * 0.5 + 0.5);
			
	fixed3 ramp = tex2D(_Ramp, fixed2(ndl, ndl));
#if !(POINT) && !(SPOT)
		ramp *= atten;
#endif
	// multiply Ambient Occlusion
	ramp *= s.Occlusion;
	// use Colored Shadows
	ramp = lerp(unity_ShadowColor.rgb, fixed3(1, 1, 1), ramp);
	
	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ramp + s.Emission;
#if (POINT || SPOT)
		c.rgb *= atten;
#endif

    // Set alpha to fully opaque to avoid issues in deferred rendering or post-processing
	c.a = 1;

	return c;
}
#endif
*/