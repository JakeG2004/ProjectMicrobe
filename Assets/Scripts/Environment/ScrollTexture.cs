using UnityEngine;

public class ScrollTexture : MonoBehaviour {
	public Vector2 scrollSpeed = new Vector2(0, -1f);
	Renderer rend;

	void Start() {
		rend = GetComponent<Renderer>();
	}
	void Update() {
		rend.material.SetTextureOffset("_MainTex", scrollSpeed * Time.time);
	}
}