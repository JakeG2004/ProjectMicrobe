using UnityEngine;
using UnityEngine.UI;

/* Quick simple script only for setting UI image colors... for now */
// this goes on the toggle...

[RequireComponent(typeof(Toggle))]
public class UIImage : MonoBehaviour {

	[SerializeField] Image lockImage; // set in start depending on toggle object interactability
	[SerializeField] Image[] uiImages;
	[SerializeField] Color[] colors;

    void Start() {
		lockImage.enabled = !GetComponent<Toggle>().interactable;

		for(int i = 0; i < uiImages.Length; i++) {
			uiImages[i].color = colors[i];
		}
	}
}