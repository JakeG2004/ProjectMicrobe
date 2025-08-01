using UnityEngine;

public class PlayerSoundController : MonoBehaviour {

	// footsteps by getting foot groundedness (as used in foot placement) from animator
	Animator anim;
	float footL = 0f;
	float footR = 0f;
	bool groundedL = false;
	bool groundedR = false;
	[SerializeField] float impact = 0f;

    void Start() {
		anim = GetComponent<Animator>();
    }

    void Update() {
		footL = anim.GetFloat("IKWeightLeft");
		footR = anim.GetFloat("IKWeightRight");
		impact = Mathf.Lerp(impact, anim.GetBool("Grounded") ? 0f : 1f, 3f * Time.deltaTime);

		// if foot hasn't been on the ground and it is now, play sound
		if (!groundedL) {
			groundedL = footL > 0.01f;
			if (groundedL) RandomSoundBasedOnImpact(impact);
		}
		else groundedL = footL > 0.01f;

		if (!groundedR) {
			groundedR = footR > 0.01f;
			if (groundedR) RandomSoundBasedOnImpact(impact);
		}
		else groundedR = footR > 0.01f;
	}
	void RandomSoundBasedOnImpact(float impactStrength) {
		if (impactStrength > 0.5f) {
			SoundManager.PlaySound(SoundType.FOOTSTEP_HARD);
			//Debug.DrawRay(anim.GetIKPosition(AvatarIKGoal.LeftFoot), Vector3.up, Color.red, 0.5f);
		}
		else {
			SoundManager.PlaySound(SoundType.FOOTSTEP_SOFT);
			//Debug.DrawRay(anim.GetIKPosition(AvatarIKGoal.LeftFoot), Vector3.up, Color.yellow, 0.5f);
		}
	}
}