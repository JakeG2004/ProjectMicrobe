using UnityEngine;
using UnityEngine.UI;

// put this script on the image object

public class ToggleImage : MonoBehaviour {

	Image image;
	public Sprite spriteA;
	public Sprite spriteB;
	bool first = true;

	void Start() {
		image = GetComponent<Image>();
	}

	public void toggleImage() {
		first = !first;
		image.sprite = first ? spriteA : spriteB; 
	}
}