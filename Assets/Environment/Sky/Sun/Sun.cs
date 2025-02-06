using UnityEngine;

public class Sun : MonoBehaviour {

    Transform cam;
    float rotation = 0f;

    void Start() {
        cam = GM.cam;
    }

    void FixedUpdate() {
        transform.position = cam.position;

        rotation += Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(41f, rotation, 0f);
    }
}