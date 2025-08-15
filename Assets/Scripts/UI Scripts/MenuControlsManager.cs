using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControlsManager : MonoBehaviour
{
    private PlayerMovementController _mc;

    void Start()
    {
        _mc = PlayerMovementController.Instance;
    }

    public void SetControlState(bool _isActive)
    {
        _mc.SetMovementState(!_isActive);
        SetCameraTracking(!_isActive);

        if (NewInputController.Instance.GetCurrentInputDevice() == InputType.KeyboardMouse)
        {
            SetMouseState(_isActive);
        }
    }

    public void SetMouseState(bool _isActive)
    {
        if (_isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    public void SetCameraTracking(bool state)
    {
        Camera.main.gameObject.GetComponent<CameraController>().SetMouseTracking(state);
    }
}