using UnityEngine;

public class DoorScifi : MonoBehaviour {

	[SerializeField] AudioClip doorSound;

	[SerializeField] Transform[] doors;
	Vector3[] closePos = new Vector3[2];
	Vector3[] openPos = new Vector3[2];
	bool open = false;
	bool move = false;
	float openPercent = 0f;

	void Start() {
		if (doors.Length != 2) {
			Debug.Log(name + " should have two doors.");
			return;
		}
		for (int i = 1; i >= 0; i--) {
			closePos[i] = doors[i].position;
			openPos[i] = closePos[i] + doors[i].forward * 1.3f;
			//Debug.Log(doors[i].name + " start position: " + closePos[i].ToString());
		}
	}
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			open = true;
			move = true;
		}
	}
	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			open = false;
			move = true;
		}
	}
	void Update() {
		if (!move) return;
		openPercent = Mathf.Clamp01(openPercent + Time.deltaTime * (open ? 1f : -1f));
		for (int i = 1; i >= 0; i--) {
			doors[i].position = Vector3.Lerp(closePos[i], openPos[i], openPercent);
		}
		if (openPercent == 1 || openPercent == 0) move = false;
	}
}