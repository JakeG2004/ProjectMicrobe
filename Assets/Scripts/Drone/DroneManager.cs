// DroneManager.cs
// A script for managing drone transport
// Author:  Jake Gendreau
// Date:    7/18/25

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DroneManager : MonoBehaviour
{
    [Header("Drone Objects")]
    [SerializeField] private NavMeshAgent _droneLead;
    [SerializeField] private GameObject _drone;

    [Header("Start and End Transformations")]
    [SerializeField] private Transform _homeBase;
    [SerializeField] private Transform _dest;

    private Transform _player;

    // Settings
    private const float _CEILING_HEIGHT = 20f;
    private const float _SMOOTHING = .75f;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        CallDrone(_dest.position, true);
    }

    public void CallDrone(Vector3 pos, bool isReturningToShip)
    {
        Debug.Log("Calling drone...");

        _droneLead.destination = pos;

        StartCoroutine(TrackDrone(isReturningToShip));
    }

    public void ReturnToBase()
    {
        if (Vector3.Distance(_drone.transform.position, _homeBase.transform.position + new Vector3(0, 2, 0)) < 1f)
        {
            return;
        }

        Debug.Log("Returning to base...");
        _droneLead.destination = _homeBase.position;
        StartCoroutine(TrackDrone(false));
    }

    private IEnumerator TrackDrone(bool isReturningToShip)
    {
        // Wait for pathfinding to finish
        while (_droneLead.pathPending)
        {
            yield return null;
        }

        // Follow the drone during flight
        while (_droneLead.remainingDistance > 0.1f)
        {
            Vector3 targetPos = _droneLead.transform.position;

            // Check for ceiling above the drone lead
            // We get ceiling hit, LERP towards midway point of hit y and lead position y
            if (Physics.Raycast(targetPos + (transform.forward * 2f), Vector3.up, out RaycastHit hit, _CEILING_HEIGHT))
            {
                targetPos.y = Mathf.Lerp(_drone.transform.position.y, _droneLead.transform.position.y + ((hit.point.y - _droneLead.transform.position.y) / 2), _SMOOTHING);
            }

            // No ceiling hit
            else
            {
                targetPos.y = Mathf.Lerp(_drone.transform.position.y, _droneLead.transform.position.y + _CEILING_HEIGHT, _SMOOTHING);
            }

            // Smoothly interpolate the drone toward the target
            _drone.transform.position = Vector3.Lerp(_drone.transform.position, targetPos, _SMOOTHING * Time.deltaTime);

            yield return null;
        }

        Vector3 landPos = _droneLead.transform.position + new Vector3(0, 2, 0);

        // Land the drone at the destination
        while (Vector3.Distance(_drone.transform.position, landPos) > 0.5f)
        {
            _drone.transform.position = Vector3.Lerp(_drone.transform.position, landPos, _SMOOTHING * Time.deltaTime);
            yield return null;
        }

        // Player is coming back to the ship
        if (isReturningToShip)
        {
            // Wait until the player is being carried
            PlayerStatesSO states = PlayerController.Instance.GetStates();
            while (!states.isBeingCarried)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        // Player is getting dropped off at a pylon
        else
        {
            PlayerController.Instance.EndCarry();

            yield return new WaitForSeconds(0.5f);
        }

        ReturnToBase();
    }
}
