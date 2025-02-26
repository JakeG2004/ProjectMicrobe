using UnityEngine;

public class LightSwarmFollowPlayer : MonoBehaviour {

    Transform player;
	[SerializeField] Transform lightSwarm;
	[SerializeField] float targetHeight = 15f;
	Vector3 targetPos = Vector3.zero;
	Vector3 randomOffset = Vector3.zero;
	bool follow = false;

	void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Player")) follow = true;
	}
	void OnTriggerExit(Collider other) {
		if(other.CompareTag("Player")) follow = false;
	}
	void Start() {
		player = GM.player;
	}

	void Update()  {
		if(!follow) return;

		randomOffset = Vector3.ClampMagnitude(randomOffset + Random.insideUnitSphere / 2f, 1f);
		targetPos = new Vector3(player.position.x, (targetHeight + player.position.y) / 2f, player.position.z);
		//Debug.DrawLine(targetPos, targetPos + randomOffset, Color.red);

		lightSwarm.position = Vector3.Lerp(lightSwarm.position, targetPos + randomOffset, 3f * Time.deltaTime);
    }
}