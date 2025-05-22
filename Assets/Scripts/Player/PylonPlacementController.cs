// PylonPlacementController.cs
// A script for managing pylon placement by the player
// Author:  Jake Gendreau
// Date:    5/22/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonPlacementController : MonoBehaviour
{
    [SerializeField] private float _forwardRaycastDist = 5.0f;
    [Serializefield] private float _downwardRaycastDist = 5.0f;

    private GameObject _mainCamGO;
    private Transform _player;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamGO = Camera.main.gameObject;
        _player = GameObject.FindGameObjectByTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit _horizontalHit;
        if(Physics.Raycast(_player.position, transform.TransformDirection(Vector3.forward), out hit, _forwardRaycastDist))
        {
            
        }
    }
}
