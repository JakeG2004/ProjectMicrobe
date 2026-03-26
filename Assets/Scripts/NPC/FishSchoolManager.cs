using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSchoolManager : MonoBehaviour
{
    [SerializeField] private PlayerStatesSO _playerStates;
    private Transform _playerTransform;
    bool _isTrackingPlayer = false;
    Boidy _boidy;

    void Start()
    {
        _boidy = gameObject.GetComponent<Boidy>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (_playerStates.submersion > 0.6f)
        {
            Vector3 newPos = new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z);
            if (!_isTrackingPlayer)
            {
                _boidy.ResetBoidPositions(newPos);
                _isTrackingPlayer = true;
            }
            
            transform.position = newPos;

            return;
        }

        _isTrackingPlayer = false;
    }
}
