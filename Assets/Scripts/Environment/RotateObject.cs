using UnityEngine;

public class Rotator : MonoBehaviour {
	public float rotationSpeed = 10f;
	public Axis axis = Axis.Y;
	public enum Axis { X, Y, Z };

	void Update() {
		Vector3 vector = new Vector3((axis == Axis.X) ? 1 : 0, (axis == Axis.Y) ? 1 : 0, (axis == Axis.Z) ? 1 : 0);
		transform.Rotate(vector * rotationSpeed * 360f * Time.deltaTime, Space.World);
	}
}