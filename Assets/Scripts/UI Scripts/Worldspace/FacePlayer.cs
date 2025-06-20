using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] private bool _faceOnStart = false;

    private GameObject _cam;
    private Animator _anim;
    private bool _isUp = false;

    // Start is called before the first frame update
    void Start()
    {
        InteractMaster.Instance.AddInteract(this);

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
        if (!_isUp)
        {
            return;
        }

        transform.LookAt(_cam.transform);
    }

    public void SetAnimState(bool state)
    {
        if (state == true)
        {
            InteractMaster.Instance?.DisableOtherInteracts(this);
        }
        
        _anim.SetBool("TextIsUp", state);
        _isUp = state;
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
