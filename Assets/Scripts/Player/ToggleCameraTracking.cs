using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCameraTracking : MonoBehaviour
{
    public void SetCameraTracking(bool state)
    {
        Camera.main.gameObject.GetComponent<CameraController>().SetMouseTracking(state);
    }

    public void DoToggleCameraTracking()
    {
        Camera.main.gameObject.GetComponent<CameraController>().ToggleMouseTracking();
    }
}
