Shader "Jake/TransparentStandardVertLit" {
	Properties{
		_TintR("_TintR", Color) = (1,1,1,1)
		_TintG("_TintB", Color) = (1,1,1,0)
		_TintB("_TintG", Color) = (0,0,0,0)
		_Shininess("Shininess", Range(0.1, 1)) = 0.7
	}

		SubShader{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			LOD 100

			Cull Off

			Alphatest Greater 0
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB

		// Non-lightmapped
		Pass {
			Tags { "LightMode" = "Vertex" }
			Material {
				Diffuse[_TintR]
				Ambient[_TintR]
				Shininess[_Shininess]
				Specular[_TintG]
				Emission[_TintB]
			}
			Lighting On
			SeparateSpecular On
			SetTexture[_MainTex] {
				Combine primary DOUBLE, primary
			}
		}
	}
}