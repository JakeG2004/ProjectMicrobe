// RandombugMovement.cs
// A script for randomly moving bugs on the screen
// Author:  Jake Gendreau
// Date:    6/15/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBugMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _maxTurn = 10.0f;
    [SerializeField] private float _rotationOffset = 90f;

    // x distance from center, y distance from center
    [SerializeField] private Vector2 _boxBounds;

    [SerializeField] private Color[] _colors = new Color[] {
        Color.blue,
        Color.cyan,
        Color.green,
        Color.magenta,
        Color.red,
        Color.yellow
    };

    void Update()
    {
        DoMovement();
        CheckBounds();
    }

    // Moves the bug based on a random rotation
    void DoMovement()
    {
        // Apply a random rotation around Z axis
        float randomTurn = Random.Range(-_maxTurn, _maxTurn);
        transform.rotation *= Quaternion.Euler(0, 0, randomTurn);

        // Get the current angle (in radians) from the Z rotation
        float angle = (transform.eulerAngles.z + _rotationOffset) * Mathf.Deg2Rad;

        // Resolve movement into x and y components
        float xComponent = _moveSpeed * Mathf.Cos(angle);
        float yComponent = _moveSpeed * Mathf.Sin(angle);

        // Apply movement scaled by deltaTime
        Vector3 movement = new Vector3(xComponent, yComponent, 0) * Time.deltaTime;

        transform.localPosition += movement;
    }

    public void ResetBug()
    {
        // Set its position randomly within the range 
        transform.localPosition = new Vector3(Random.Range(-_boxBounds.x, _boxBounds.x), Random.Range(-_boxBounds.y, _boxBounds.y), 0);
    
        // Set its rotation randomly
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

        // Set its color randomly
        GetComponent<SpriteRenderer>().color = _colors[Random.Range(0, 6)];
    }


    // Wrap the bug if it gets to the edge of the screen
    void CheckBounds()
    {
        Vector3 newPos = transform.localPosition;

        // Handle x bounds
        if(Mathf.Abs(newPos.x) >= _boxBounds.x)
        {
            newPos.x = -Mathf.Sign(newPos.x) * (_boxBounds.x - 0.1f);
        }

        if(Mathf.Abs(newPos.y) >= _boxBounds.y)
        {
            newPos.y = -Mathf.Sign(newPos.y) * (_boxBounds.y - 0.1f);
        }

        transform.localPosition = newPos;
    }

    public void SetBounds(Vector2 bounds)
    {
        _boxBounds = bounds;
    }
}
