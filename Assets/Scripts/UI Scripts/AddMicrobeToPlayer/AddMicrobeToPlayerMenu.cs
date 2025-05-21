using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddMicrobeToPlayerMenu : MonoBehaviour
{
    public static AddMicrobeToPlayerMenu Instance { get; private set; }
    [SerializeField] private GameObject _menuPanel;
    private bool _isActive = false;

    private BoolGameEventTrigger _menuStateTracker;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }

        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _menuPanel.SetActive(false);
        _menuStateTracker = GetComponent<BoolGameEventTrigger>();
    }

    public void ToggleState()
    {
        _isActive = !_isActive;
        _menuPanel.gameObject.SetActive(_isActive);

        // Set UI Control state
        GetComponent<ToggleCameraTracking>()?.SetCameraTracking(!_isActive);
        MovementController.instance.SetMovementState(!_isActive);
        GetComponent<ShowHideMouse>()?.SetState(_isActive);

        // Set UI Menu Tracker State
        _menuStateTracker.TriggerEvent(_isActive);
    }
}
