// Slightly modified 2016 Mobile VertexLit Blended Particle shader
// - no AlphaTest
// - no ColorMask
// + double colorBrightness

Shader "Landon/Particles/VertexLit" {
	Properties{
		_EmisColor("Emissive Color", Color) = (.2,.2,.2,0)
		_MainTex("Particle Texture", 2D) = "white" {}
	}

		Category{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off ZWrite Off Fog { Color(0,0,0,0) }

			Lighting On
			Material { Emission[_EmisColor] }
			ColorMaterial AmbientAndDiffuse

			SubShader {
				Pass {
					SetTexture[_MainTex] {
						combine texture * primary DOUBLE
					}
				}
			}
	}
}