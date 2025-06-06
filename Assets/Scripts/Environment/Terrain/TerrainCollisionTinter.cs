// TerrainCollisionTinter.cs
// A script to use projectors to change the color of terrain
// Author:  Jake Gendreau
// Date:    6/6/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCollisionTinter : MonoBehaviour
{
    [SerializeField] private GameObject _projectorPrefab;

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.GetComponent<Terrain>())
        {
            ContactPoint contact = col.contacts[0];

            GameObject projector = Instantiate(_projectorPrefab, contact.point + Vector3.up * 5f, Quaternion.Euler(90, 0, 0));
        }
    }
}
