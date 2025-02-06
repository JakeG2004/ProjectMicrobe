using UnityEngine;

public class SimplePlayerRipple : MonoBehaviour {

    Transform player;
	ParticleSystem ripples;
	bool emit = false;

	void Start() {
		player = GM.player;
		ripples = GetComponent<ParticleSystem>();
	}

	void Update()  {
		Vector3 pos = player.position;

		if (emit) {
			if (pos.y > 0f || pos.y < -2f) {
				ripples.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				emit = false;
			}
		}
		else {
			if (pos.y < 0f && pos.y > -2f) {
				ripples.Play(true);
				emit = true;
			}
		}

		pos.y = 0;
		transform.position = pos;
    }
}