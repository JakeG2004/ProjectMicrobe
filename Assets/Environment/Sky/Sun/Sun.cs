using UnityEngine;

public class Sun : MonoBehaviour {

	[SerializeField] bool rotate = true;
    Transform cam;
    float rotation = 0f;

    void Start() {
        cam = GM.cam;
    }

    void FixedUpdate() {
        transform.position = cam.position;

		if(!rotate) return;
        rotation += Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(41f, rotation, 0f);
    }
}