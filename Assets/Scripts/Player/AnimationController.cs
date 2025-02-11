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
		ac.SetFloat("LookVert", ic.turn.y);
		ac.SetFloat("LookHoz", ic.turn.x);
		if (ic.triggerJump) {
			ac.SetTrigger("Jump");
			ic.triggerJump = false;
		}
		ac.SetBool("Climbing", ic.climbing);
		ac.SetBool("LongDrop", ic.longDrop);
		ac.SetBool("Grounded", ic.grounded);
		if (ic.grounded) {
			ic.longDrop = false;
		}
	}
}