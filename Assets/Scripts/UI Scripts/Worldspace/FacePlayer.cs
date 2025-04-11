using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private GameObject _cam;
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _cam = GameObject.FindGameObjectWithTag("MainCamera");
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_cam.transform);
    }

    public void SetAnimState(bool state)
    {
        _anim.SetBool("TextIsUp", state);
    }
}
