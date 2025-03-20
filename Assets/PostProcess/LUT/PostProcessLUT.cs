using UnityEngine;

[ExecuteInEditMode]
public class PostProcessLUT : MonoBehaviour {
	[Range(0, 1)] float LutBlendAmount = 0.0f; // Blending between two LUT textures
	[SerializeField] Texture2D SourceLutA = null;  // First LUT texture
	[SerializeField] Texture2D SourceLutB = null;  // Second LUT texture

	Texture3D convertedLutA = null;
	Texture3D convertedLutB = null;
	Texture2D previousLutA = null;
	Texture2D previousLutB = null;
	[SerializeField] Material material;

	Transform cam;

	void Start() {
		// Initialize LUT conversion if both LUTs are set
		if(SourceLutA != null) {
			Convert3D(SourceLutA, ref convertedLutA);
		}
		if(SourceLutB != null) {
			Convert3D(SourceLutB, ref convertedLutB);
		}
		cam = GM.cam? GM.cam : Camera.main.transform;
	}

	void Update() {
		// Update the LUT textures if they've changed
		if(SourceLutA != previousLutA) {
			previousLutA = SourceLutA;
			Convert3D(SourceLutA, ref convertedLutA);
		}
		if(SourceLutB != previousLutB) {
			previousLutB = SourceLutB;
			Convert3D(SourceLutB, ref convertedLutB);
		}


		// Blend between LUTs based on camera height
		if(!cam) return;
		LutBlendAmount = Mathf.Clamp01(0.5f - cam.position.y);
	}

	void OnDestroy() {
		// Clean up the 3D LUT textures when the script is destroyed
		if(convertedLutA != null) DestroyImmediate(convertedLutA);
		if(convertedLutB != null) DestroyImmediate(convertedLutB);
	}

	void Convert3D(Texture2D lutTex, ref Texture3D convertedLut) {
		// Convert the 2D LUT texture to a 3D LUT texture for use in the shader
		var color = lutTex.GetPixels();
		var newCol = new Color[color.Length];

		for(int i = 0; i < 16; i++) {
			for(int j = 0; j < 16; j++) {
				for(int k = 0; k < 16; k++) {
					int val = 16 - j - 1;
					newCol[i + (j * 16) + (k * 256)] = color[k * 16 + i + val * 256];
				}
			}
		}

		// Create and apply the 3D LUT texture
		if(convertedLut != null) {
			DestroyImmediate(convertedLut);
		}
		convertedLut = new Texture3D(16, 16, 16, TextureFormat.ARGB32, false);
		convertedLut.SetPixels(newCol);
		convertedLut.Apply();
		convertedLut.wrapMode = TextureWrapMode.Clamp;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if(convertedLutA != null && convertedLutB != null) {
			// Set the blended LUT texture based on the LutBlendAmount slider
			material.SetFloat("_LutBlendAmount", LutBlendAmount);
			material.SetTexture("_LutTexA", convertedLutA);
			material.SetTexture("_LutTexB", convertedLutB);
		}
		// Apply the material (with LUT effect) to the destination
		Graphics.Blit(source, destination, material);
	}
}