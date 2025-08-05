using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    public KeyCode screenShotbutton;

    void Update()
    {
        if (Input.GetKeyDown(screenShotbutton))
        {
            ScreenCapture.CaptureScreenshot("screenshot.png");
            Debug.Log("Snap!");
        }
    }
}
