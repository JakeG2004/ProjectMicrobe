using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObjectiveObjectByTag : MonoBehaviour
{
    public void SetObject(string tag)
    {
        Transform newObj = GameObject.FindGameObjectWithTag(tag).transform;
        GetComponent<Objective>().SetObject(newObj);
    }
}
