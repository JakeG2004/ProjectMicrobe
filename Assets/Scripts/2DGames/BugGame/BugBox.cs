// BugBox.cs
// A script for managing the bugs in the box for the 2d minigame
// Author:  Jake Gendreau
// Date:    6/15/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugBox : MonoBehaviour
{
    [SerializeField] private GameObject _bugPrefab;
    [SerializeField] private float _numBugs = 10;
    
    [SerializeField] private Vector2 _box;

    void OnEnable()
    {
        DestroyBugs();
        SpawnBugs();
    }

    void DestroyBugs()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void SpawnBugs()
    {
        for(int i = 0; i < _numBugs; i++)
        {
            // Make the bug
            GameObject newBug = Instantiate(_bugPrefab, transform);

            // Set box bounds
            newBug.GetComponent<RandomBugMovement>().SetBounds(new Vector2(_box.x, _box.y));

            newBug.GetComponent<RandomBugMovement>().ResetBug();
        }
    }

    public void EndGame()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Draws the bounding box for the butgs based on the box vector
    void OnDrawGizmos()
    {
        Vector3[] points = new Vector3[8]
        {
            // Bottom left to upper left
            transform.position + new Vector3(-_box.x, -_box.y, 0),
            transform.position + new Vector3(-_box.x, _box.y, 0),

            // Upper left to upper right
            transform.position + new Vector3(-_box.x, _box.y, 0),
            transform.position + new Vector3(_box.x, _box.y, 0),

            // Upper right to bottom right
            transform.position + new Vector3(_box.x, _box.y, 0),
            transform.position + new Vector3(_box.x, -_box.y, 0),

            // Bottom right to bottom left
            transform.position + new Vector3(_box.x, -_box.y, 0),
            transform.position + new Vector3(-_box.x, -_box.y, 0),
        };

        Gizmos.color = Color.red;
        Gizmos.DrawLineList(points);
    }
}
