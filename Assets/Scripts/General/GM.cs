using UnityEngine;

public class GM : MonoBehaviour {

    [HideInInspector] public static Transform player;
    [HideInInspector] public static Transform cam;
}

/* if I wanted this on an object in the scene for some reason
	[HideInInspector] public static GM gm;
	void InsureSingleGM() {
		if (gm != null) {
			GameObject.Destroy(this.gameObject);
			return;
		}
		gm = this;
	}
*/