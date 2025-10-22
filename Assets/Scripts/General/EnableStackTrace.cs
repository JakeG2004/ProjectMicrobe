using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableStackTrace : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log(gameObject.name + " Enabled!");
    }

    void OnDisable()
    {
        Debug.Log(gameObject.name + " Disabled!");
    }
}
