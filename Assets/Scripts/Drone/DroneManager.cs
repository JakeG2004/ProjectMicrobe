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
    private Transform _player;
    private bool _isInFlight = false;
    private bool _playerPickedUpDelivery = false;
    private MicrobeDelivery _curDelivery = new();
    private LoopingSoundHandle _loop;
    private Vector3 _velocity;

    // Settings
    private const float _CEILING_HEIGHT = 20f;
    private const float _Y_SMOOTHING = .3f;
    private const float _X_SMOOTHING = .6f;

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
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _loop = SoundManager.PlayLoopingSoundWithIntroAndOutro(SoundType.DRONE_TAKEOFF, SoundType.DRONE_FLIGHT, SoundType.DRONE_LANDING, _drone.transform, 1f);
        _loop.SetPitch(0.6f);
    }

    public void ShipMicrobesToPlayer(MicrobeDelivery delivery)
    {
        if (_isInFlight)
        {
            StopAllCoroutines();
        }

        _flightQueue.Clear();

        _flightQueue.Enqueue(new Flight(_player.position, FlightType.DELIVERY));
        _flightQueue.Enqueue(new Flight(_homeBase.position, FlightType.RETURN));

        _curDelivery = delivery;
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
        //_pc.StartCarry(_drone);
        _droneMountInteract.SetActive(false);
    }

    private void ManageDroneSound()
    {
        float verticalSpeed = _velocity.y;
        Vector3 horizontalVel = new Vector3(_velocity.x, 0, _velocity.z);
        float horizontalSpeed = horizontalVel.magnitude;

        float horizontalPitch = Mathf.InverseLerp(0f, _droneLead.speed, _droneLead.velocity.magnitude);  // 0 to 1
        float verticalPitch = Mathf.InverseLerp(-_droneLead.speed, _droneLead.speed, _droneLead.velocity.magnitude); // -1 to 1

        float basePitch = 0.65f;
        float hWeight = 0.35f;
        float vWeight = 0.5f;

        // The vertical contribution should not overshoot — keep it symmetric
        float combinedPitch = basePitch + (horizontalPitch * hWeight) + (Mathf.Abs(verticalPitch) * vWeight);
        combinedPitch = Mathf.Clamp(combinedPitch, 0.4f, 1f); // optional

        _loop.SetPitch(combinedPitch);
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
        _loop.IsLerpingPitch(true);

        Vector2 droneLeadXZ = Vector2.zero;
        Vector2 droneXZ = Vector2.zero;

        // Follow the drone during flightf dsa
        while (_droneLead.remainingDistance > 0.1f || Vector2.Distance(droneLeadXZ, droneXZ) > 10f)
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

            // Determine tilt from velocity
            Vector3 localVelocity = _drone.transform.InverseTransformDirection(_droneLead.velocity);

            // Max tilt in degrees
            float maxTilt = 15f;

            // Pitch: tilt forward/backward based on forward velocity (negative so forward = nose down)
            float pitch = -Mathf.Clamp(-localVelocity.z, -_droneLead.speed, _droneLead.speed) / _droneLead.speed * maxTilt;

            // Roll: tilt side-to-side based on sideways velocity
            float roll = Mathf.Clamp(-localVelocity.x, -_droneLead.speed, _droneLead.speed) / _droneLead.speed * maxTilt;

            // Keep yaw from the lead
            float yaw = _droneLead.transform.eulerAngles.y;

            // Smooth tilt application
            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
            _drone.transform.rotation = Quaternion.Lerp(_drone.transform.rotation, targetRotation, _X_SMOOTHING * Time.deltaTime);


            _velocity = _droneLead.velocity;
            ManageDroneSound();

            droneLeadXZ = new Vector2(_droneLead.transform.position.x, _droneLead.transform.position.z);
            droneXZ = new Vector2(_drone.transform.position.x, _drone.transform.position.z);

            yield return null;
        }

        Vector3 landPos = _droneLead.transform.position + new Vector3(0, 2, 0);
        Vector3 initialPos = _drone.transform.position;
        Quaternion initialRotation = _drone.transform.rotation;
        Quaternion destRotation = Quaternion.Euler(0, 0, 0);

        float elapsedTime = 0f;
        float duration = 2f; // seconds to land

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // ease in/out
            Vector3 newPos = Vector3.Lerp(initialPos, landPos, t);

            _drone.transform.rotation = Quaternion.Lerp(initialRotation, destRotation, t);

            _velocity = newPos - _drone.transform.position;
            _drone.transform.position = newPos;

            ManageDroneSound();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it lands exactly
        _drone.transform.position = landPos;
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
                //PlayerController.Instance.EndCarry();
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

    public MicrobeDelivery GetCurrentDelivery()
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
