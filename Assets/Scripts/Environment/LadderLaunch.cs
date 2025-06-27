using UnityEngine;

public class LadderLaunch : MonoBehaviour {

	PlayerStates _ps;

	void Start() {
		_ps = PlayerMovementController.Instance.GetStates();
	}
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) _ps.isClimbing = true;
	}
	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) _ps.isClimbing = false;
	}
}



/*
void OnTriggerStay(Collider other) {
		if (other.CompareTag("Player")) ic.climbing = true;
	}
*/