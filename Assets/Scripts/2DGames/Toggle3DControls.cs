using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle3DControls : MonoBehaviour
{
    private ToggleCameraTracking _tct;
    private ShowHideMouse _shm;

    // Start is called before the first frame update
    void Start()
    {
        _tct = GetComponent<ToggleCameraTracking>();
        _shm = GetComponent<ShowHideMouse>();
    }

    public void Set3DControls(bool state)
    {
        _tct.SetCameraTracking(state);
        MovementController.instance.SetMovementState(state);
        _shm.SetState(!state);
    }
}
