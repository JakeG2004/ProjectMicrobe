using UnityEngine;

public class LadderLaunch : MonoBehaviour {

	Rigidbody player;
	AnimationController ac;

	void Start() {
		Transform playerTransform = GM.player;
		player = playerTransform.GetComponent<Rigidbody>();
		ac = playerTransform.GetComponent<AnimationController>();
	}
	void OnTriggerStay(Collider other) {
		if (other.tag == "Player") {
			player.velocity = new Vector3(player.velocity.x, 8f, player.velocity.z);
			if (ac.Submersed()) { // for exiting the water
				player.position += Vector3.up * 0.1f;
			}
		}
	}
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			ac.SetClimbing(true);
		}
	}
	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			ac.SetClimbing(false);
		}
	}
}