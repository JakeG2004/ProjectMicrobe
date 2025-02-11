using UnityEngine;

public class LadderLaunch : MonoBehaviour {

	InputController ic;

	void Start() {
		ic = GM.playerInput;
	}
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) ic.climbing = true;
	}
	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) ic.climbing = false;
	}
}



/*
void OnTriggerStay(Collider other) {
		if (other.CompareTag("Player")) ic.climbing = true;
	}
*/