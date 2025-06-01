// CharacterDragRotate.cs
// A script for rotating the character by dragging them on the CCUI
// Author:  Jake Gendreau
// 6/1/25

using UnityEngine;

public class CharacterDragRotate : MonoBehaviour
{
    public float rotationSpeed = 35f;
    private bool isDragging = false;
    private Vector3 lastMousePosition;

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
            float rotationY = delta.x * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, -rotationY, Space.World);
            lastMousePosition = Input.mousePosition;
        }
    }
}
