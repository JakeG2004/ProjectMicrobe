using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplePlayerRipple : MonoBehaviour
{

	Transform player;
	ParticleSystem ripples;
	bool emit = false;

	Vector3 pos = Vector3.zero;
	float playerSpeed = 0f;
	AudioSource rippleSounds;
	AudioSource splashSounds;
	[SerializeField] AudioClip[] entrySounds;

	// rippleSounds responsible for entry / exit
	// splashSounds responsible for splashing sounds

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		ripples = GetComponent<ParticleSystem>();
		rippleSounds = GetComponent<AudioSource>();
		splashSounds = transform.GetChild(0).GetComponent<AudioSource>();
	}

	void Update()
	{
		playerSpeed = (pos - player.position).magnitude;
		//Debug.Log("Player speed: " + playerSpeed.ToString("0.##"));
		pos = player.position;

		if (emit)
		{
			//control volume of splash sound based on player speed
			splashSounds.volume = Mathf.Clamp01(playerSpeed * 3f);

			// stop if exit conditions are met
			if (pos.y > 0f || pos.y < -3f)
			{
				ripples.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				emit = false;

				splashSounds.loop = false;
				rippleSounds.loop = false;

				StopAllCoroutines();
				StartCoroutine(IFadeOutSounds());

				rippleSounds.PlayOneShot(entrySounds[Random.Range(0, entrySounds.Length)]);
			}
		}
		else
		{
			// start if entry conditions are met
			if (pos.y < 0f && pos.y > -3f)
			{
				ripples.Play(true);
				emit = true;

				splashSounds.Play();
				rippleSounds.Play();

				splashSounds.loop = true;
				rippleSounds.loop = true;

				rippleSounds.PlayOneShot(entrySounds[Random.Range(0, entrySounds.Length)]);
			}
		}

		transform.position = new Vector3(pos.x, 0f, pos.z);
	}

	private IEnumerator IFadeOutSounds()
	{
		float curTime = 0.0f;

		while (curTime < 0.5f)
		{
			curTime += Time.deltaTime;
			float ratio = 1 - (curTime / 0.1f);

			splashSounds.volume = ratio;
			rippleSounds.volume = ratio;

			yield return null;
		}

		splashSounds.Pause();
		rippleSounds.Pause();
	}
}