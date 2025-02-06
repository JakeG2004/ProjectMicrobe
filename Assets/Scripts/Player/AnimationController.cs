using UnityEngine;

public class AnimationController : MonoBehaviour {

    Animator ac;
    InputController ic;

    void Start() {
        ac = GetComponent<Animator>();
        ic = GM.playerInput;
    }
    void FixedUpdate() {

		ac.SetFloat("Move", ic.move.magnitude);
		ac.SetFloat("Submersion", ic.submersion);
		ac.SetFloat("LookVert", ic.lookVert);
		ac.SetFloat("LookHoz", ic.lookHoz);

		if (ic.triggerJump) {
			ac.SetTrigger("Jump");
			ic.triggerJump = false;
		}
		
		ac.SetBool("LongDrop", ic.longDrop);
		ac.SetBool("Grounded", ic.grounded);
		if (ic.grounded) {
			ic.longDrop = false;
		}
	}
	public void SetClimbing(bool value) {
		ac.SetBool("Climbing", value);
	}
	public bool Submersed() {
		return ic.submersion > 0f;
	}
}