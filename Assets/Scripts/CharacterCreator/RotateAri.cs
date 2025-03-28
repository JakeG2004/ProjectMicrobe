using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAri : MonoBehaviour
{
    [SerializeField] private GameObject Ari;

    public void RotateAriByAmt(float amt)
    {
        Ari.transform.rotation = Quaternion.Euler(Ari.transform.rotation.eulerAngles.x, (amt * 360.0f) - 180f, Ari.transform.rotation.eulerAngles.z);
    }
}
