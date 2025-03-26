using UnityEngine;

public class CCAnimationController : MonoBehaviour {

    public Animator ac;

    void Start() {
        ac = GetComponent<Animator>();
    }
}