using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyEvent : MonoBehaviour
{
    public UnityEvent keyPressEvent;
    public KeyCode key = KeyCode.W;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(key))
        {
            keyPressEvent.Invoke();
        }
    }
}
