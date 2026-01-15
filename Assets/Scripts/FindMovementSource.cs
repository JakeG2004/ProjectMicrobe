using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class FindMovementSource : MonoBehaviour
{
    private Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        if (transform.position != lastPos)
        {
            UnityEngine.Debug.Log(
                $"Position changed to {transform.position}\n" +
                new StackTrace(true)
            );

            lastPos = transform.position;
        }
    }
}
