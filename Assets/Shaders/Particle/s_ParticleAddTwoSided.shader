// Modified 2016 Mobile Additive Particle shader
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask
// + double brightness

Shader "Landon/Particles/Bright Add" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Overlay" "IgnoreProjector"="True"}

	//Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
	
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary DOUBLE
			}
		}
	}
}
}