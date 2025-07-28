using UnityEngine;

// handles the sound and visuals of the drone.  the player motion and animations is all done in player scripts.

public class Drone : MonoBehaviour {

    Transform player;    
    InputController ic;
    Animator ac;
    bool flying = false;
    Vector2 smoothMoveDir = Vector2.zero;
    AudioSource sfx;
    Rigidbody rb;

    void Start() {
        player = GM.player;
        ic = GM.playerInput;
        ac = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
        rb = GM.player.GetComponent<Rigidbody>();
    }
    void Update() {
        if (flying != ic.flying)
            FlyingToggled();
        if (!flying)
            return;
        
        transform.position = player.position;
        transform.rotation = player.rotation;

        smoothMoveDir = Vector2.Lerp(smoothMoveDir, ic.move, 10f * Time.deltaTime);
        ac.SetFloat("Pitch", Mathf.Abs(smoothMoveDir.y));
        ac.SetFloat("Bank", smoothMoveDir.x);
        
        sfx.volume = Mathf.Clamp01(rb.velocity.magnitude / 50f + 0.2f);
        //Debug.Log(rb.velocity.magnitude);
    }
    void FlyingToggled() {
        flying = !flying;
        ac.SetBool("Flying", flying);
        if (flying) sfx.Play();
        else sfx.Stop();
    }
}