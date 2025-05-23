using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] private bool _faceOnStart = false;

    private GameObject _cam;
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _cam = GameObject.FindGameObjectWithTag("MainCamera");
        _anim = GetComponent<Animator>();

        if(_faceOnStart)
        {
            SetAnimState(true);
        }
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

    void OnEnable()
    {
        _anim = GetComponent<Animator>();
        if(_faceOnStart)
        {
            SetAnimState(true);
        }
    }
}
