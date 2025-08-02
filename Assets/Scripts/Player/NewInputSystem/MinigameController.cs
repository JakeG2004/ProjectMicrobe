using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    private NewInputController _controller;
    private BoolGameEventTrigger _2dBackTrigger;

    void Awake()
    {
        _2dBackTrigger = GetComponent<BoolGameEventTrigger>();
    }

    void Start()
    {
        _controller = NewInputController.Instance;
        BindControls();
    }

    void OnDisable()
    {
        UnbindControls();
    }

    public void UnlockTablet()
    {
        _controller.generalInput.UnlockTablet();
    }

    private void BindControls()
    {
        _controller.minigameInput.OnBackPressed += HandleMinigameBack;
    }

    private void UnbindControls()
    {
        if (_controller == null)
        {
            return;
        }

        _controller.minigameInput.OnBackPressed -= HandleMinigameBack;
    }

    private void HandleMinigameBack()
    {
        _2dBackTrigger.TriggerEvent(true);
    }
}
