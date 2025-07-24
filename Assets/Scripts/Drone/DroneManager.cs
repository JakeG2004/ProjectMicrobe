// DroneManager.cs
// A script for managing drone transport
// Author:  Jake Gendreau
// Date:    7/18/25

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneManager : MonoBehaviour
{
    public static DroneManager Instance { get; private set; }

    [Header("Drone Objects")]
    [SerializeField] private NavMeshAgent _droneLead;
    [SerializeField] private GameObject _drone;
    [SerializeField] private GameObject _droneMountInteract;
    [SerializeField] private GameObject _droneDeliveryInteract;

    [Header("Start and End Transformations")]
    [SerializeField] private Transform _homeBase;

    private Queue<Flight> _flightQueue = new();
    private PlayerCarry _pc;
    private bool _isInFlight = false;
    private bool _playerPickedUpDelivery = false;
    private List<MicrobeSO> _curDelivery = new();

    // Settings
    private const float _CEILING_HEIGHT = 20f;
    private const float _Y_SMOOTHING = .75f;
    private const float _X_SMOOTHING = .9f;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        _pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCarry>();
    }

    public void ShipMicrobesToPlayer(List<MicrobeSO> microbes)
    {
        if (_isInFlight)
        {
            return;
        }

        _flightQueue.Clear();

        _flightQueue.Enqueue(new Flight(_pc.gameObject.transform.position, FlightType.DELIVERY));
        _flightQueue.Enqueue(new Flight(_homeBase.position, FlightType.RETURN));

        _curDelivery = microbes;
        _playerPickedUpDelivery = false;
        StartCoroutine(FlightManager());
    }

    // Constructs a flight path based start position and end position
    public void StartFlight(Vector3 curPlayerPos, Vector3 destinationPos)
    {
        if (_isInFlight)
        {
            StopAllCoroutines();
        }

        _flightQueue.Clear();

        _flightQueue.Enqueue(new Flight(curPlayerPos, FlightType.PICKUP));
        _flightQueue.Enqueue(new Flight(destinationPos, FlightType.DROPOFF));
        _flightQueue.Enqueue(new Flight(_homeBase.position, FlightType.RETURN));

        StartCoroutine(FlightManager());
    }

    public void GetOnDrone()
    {
        _pc.StartCarry(_drone);
        _droneMountInteract.SetActive(false);
    }

    private IEnumerator FlightManager()
    {
        // Early return if no queue
        if (_flightQueue.Count == 0)
        {
            yield break;
        }

        while (_flightQueue.Count > 0)
        {
            Flight curFlight = _flightQueue.Dequeue();

            _droneLead.destination = curFlight.pos;
            yield return StartCoroutine(FlyToDestination(curFlight.flightType));
        }
    }

    private IEnumerator FlyToDestination(FlightType flightType)
    {
        // Wait for pathfinding to finish
        while (_droneLead.pathPending)
        {
            yield return null;
        }

        _isInFlight = true;

        // Follow the drone during flight
        while (_droneLead.remainingDistance > 0.1f)
        {
            if (Vector3.Distance(_drone.transform.position, _droneLead.transform.position) > 60f)
            {
                _drone.transform.position = _droneLead.transform.position;
            }

            Vector3 targetRot = _droneLead.transform.eulerAngles;
            Vector3 targetPos = _droneLead.transform.position;

            // Check for ceiling above the drone lead
            // We get ceiling hit, LERP towards midway point of hit y and lead position y
            if (Physics.Raycast(targetPos + (transform.forward * 2f), Vector3.up, out RaycastHit hit, _CEILING_HEIGHT))
            {
                targetPos.y = Mathf.Lerp(_drone.transform.position.y, _droneLead.transform.position.y + ((hit.point.y - _droneLead.transform.position.y) / 2), _Y_SMOOTHING);
            }

            // No ceiling hit
            else
            {
                targetPos.y = Mathf.Lerp(_drone.transform.position.y, _droneLead.transform.position.y + _CEILING_HEIGHT, _Y_SMOOTHING);
            }

            // Smoothly interpolate the drone toward the target
            _drone.transform.position = Vector3.Lerp(_drone.transform.position, targetPos, _X_SMOOTHING * Time.deltaTime);
            _drone.transform.eulerAngles = Vector3.Lerp(_drone.transform.eulerAngles, targetRot, _X_SMOOTHING * Time.deltaTime);

            yield return null;
        }

        Vector3 landPos = _droneLead.transform.position + new Vector3(0, 2, 0);

        // Land the drone at the destination
        while (Vector3.Distance(_drone.transform.position, landPos) > 0.5f)
        {
            _drone.transform.position = Vector3.Lerp(_drone.transform.position, landPos, _Y_SMOOTHING * Time.deltaTime);
            yield return null;
        }

        yield return StartCoroutine(HandlePickupDropoff(flightType));
    }

    private IEnumerator HandlePickupDropoff(FlightType flightType)
    {
        switch (flightType)
        {
            // Player getting picked up by the drone
            case FlightType.PICKUP:
                _droneMountInteract.SetActive(true);
                PlayerStatesSO states = PlayerController.Instance.GetStates();
                while (!states.isBeingCarried)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                _droneMountInteract.SetActive(false);

                break;

            // Player getting dropped off by the drone
            case FlightType.DROPOFF:
                PlayerController.Instance.EndCarry();
                break;

            // Drone returns to the home base
            case FlightType.RETURN:
                _isInFlight = false;
                break;

            // Drone is delivering a package
            case FlightType.DELIVERY:
                _droneDeliveryInteract.SetActive(true);
                while (!_playerPickedUpDelivery)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                _playerPickedUpDelivery = true;
                _droneDeliveryInteract.SetActive(false);
                break;
        }
    }

    public void SetPlayerDeliveryStatus(bool state)
    {
        _playerPickedUpDelivery = state;    
    }

    public List<MicrobeSO> GetCurrentDelivery()
    {
        return _curDelivery;
    }

    public enum FlightType
    {
        PICKUP,
        DROPOFF,
        RETURN,
        DELIVERY,
    };

    private struct Flight
    {
        public FlightType flightType;
        public Vector3 pos;

        public Flight(Vector3 curPos, FlightType curFlightType)
        {
            pos = curPos;
            flightType = curFlightType;
        }
    }
}
