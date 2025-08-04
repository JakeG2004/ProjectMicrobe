// CharacterDragRotate.cs
// A script for rotating the character by dragging them on the CCUI
// Author:  Jake Gendreau
// 6/1/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDragRotate : MonoBehaviour
{
    public float rotationSpeed = 35f;
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    [SerializeField] private PlayerStatesSO _states;
    private MenuControlsManager _mcm;

    void Awake()
    {
        _mcm = GetComponent<MenuControlsManager>();
    }

    void Start()
    {
        NewInputController.Instance.SetMenuMode();
    }

    void OnMouseDown()
    {
        isDragging = true;
        lastMousePosition = Input.mousePosition;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Rotate(delta.x);
            lastMousePosition = Input.mousePosition;
        }

        RotateAriStick();
    }

    void Rotate(float delta)
    {
        float rotationY = delta * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, -rotationY, Space.World);
    }

    public void RotateAriStick()
    {
        float delta = _states.minigameMove.x * 5f;
        Rotate(delta);
    }
}
