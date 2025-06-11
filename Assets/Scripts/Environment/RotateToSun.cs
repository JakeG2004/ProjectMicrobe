using UnityEngine;

public class RotateToSun : MonoBehaviour {
	public Transform sun;

	void Update() {
		Vector3 sunDir = SquashVector(sun.forward);
		transform.rotation = Quaternion.LookRotation(sunDir, Vector3.up);
	}
	Vector3 SquashVector(Vector3 input) {
		return new Vector3(input.x, 0f, input.z);
	}
}