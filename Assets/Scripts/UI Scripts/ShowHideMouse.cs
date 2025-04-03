using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideMouse : MonoBehaviour
{
    private enum MouseStatus
    {
        Show,
        Hide
    }

    [SerializeField] private MouseStatus _mouseStatus = MouseStatus.Show;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayedDoMouse());
    }

    private IEnumerator DelayedDoMouse()
    {
        yield return null;
        DoMouse();
    }

    void DoMouse()
    {
        if(_mouseStatus == MouseStatus.Show)
        {
            // Lock cursor to center of screen and make it invisible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            return;
        }    

        // Lock cursor to center of screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
