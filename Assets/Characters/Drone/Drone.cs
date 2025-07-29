using UnityEngine;

public class Drone : MonoBehaviour
{
    private Animator _anim;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetDroneDeployed(bool state)
    {
        _anim.SetBool("Flying", state);
    }
}