using UnityEngine;

public class SimplePlayerRipple : MonoBehaviour {

    Transform player;
	ParticleSystem ripples;
	bool emit = false;

	Vector3 pos = Vector3.zero;
	float playerSpeed = 0f;
	AudioSource rippleSounds;
	AudioSource splashSounds;
	[SerializeField] AudioClip[] entrySounds;

	void Start() {
		player = GM.player;
		ripples = GetComponent<ParticleSystem>();
		rippleSounds = GetComponent<AudioSource>();
		splashSounds = transform.GetChild(0).GetComponent<AudioSource>();
	}

	void Update()  {
		playerSpeed = (pos - player.position).magnitude;
		//Debug.Log("Player speed: " + playerSpeed.ToString("0.##"));
		pos = player.position;

		if (emit) {
			//control volume of splash sound based on player speed
			splashSounds.volume = Mathf.Clamp01(playerSpeed * 3f);

			// stop if exit conditions are met
			if (pos.y > 0f || pos.y < -2f) {
				ripples.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				emit = false;
				rippleSounds.Pause();
				splashSounds.Pause();
				rippleSounds.PlayOneShot(entrySounds[Random.Range(0, entrySounds.Length)]);
			}
		}
		else {
			// start if entry conditions are met
			if (pos.y < 0f && pos.y > -2f) {
				ripples.Play(true);
				emit = true;
				rippleSounds.Play();
				splashSounds.Play();
				rippleSounds.PlayOneShot(entrySounds[Random.Range(0, entrySounds.Length)]);
			}
		}

		transform.position = new Vector3(pos.x,0f,pos.z);
    }
}