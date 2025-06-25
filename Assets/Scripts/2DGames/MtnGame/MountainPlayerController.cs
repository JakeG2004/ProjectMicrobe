// CoconutPlayerController.cs
// A script for managing the coconut game
// Author:  Jake Gendreau
// Date:    6/9/25

using UnityEngine;
using System.Collections;

public class MountainPlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _arrowObj;
    [SerializeField] private GameObject _prize;
    private bool _isDragging = false;
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _diff;
    private Rigidbody2D _rb;
    private bool _isGrounded = false;
    private Transform _curPlatform;
    private Vector3 _offset = Vector3.zero;
    private Transform _originalParent;
    private Vector3 _initPlayerPos;
    private SoundPlayer _sp;
    float _prevArrowScale = 0.0f;

    bool _canPlaySound = false;

    void Start()
    {
        _sp = GetComponent<SoundPlayer>();
        _initPlayerPos = transform.position;
        _originalParent = transform.parent;
        _rb = GetComponent<Rigidbody2D>();

        StartCoroutine(IPreventStartSounds());
    }

    private IEnumerator IPreventStartSounds()
    {
        while (!Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            yield return null;            
        }

        _canPlaySound = true;
    }

    void Update()
    {
        // Get the current mouse position as a vector2
        Vector3 curMousePos = Input.mousePosition;
        _diff = Vector3.Normalize(_startPos - curMousePos);

        if (_isDragging)
        {
            _arrowObj.SetActive(true);

            // Get the angle that the arrow should form with the player
            float angle = -1 * Mathf.Atan2(_diff.x, _diff.y) * Mathf.Rad2Deg;

            float arrowScale = Mathf.Clamp(Vector3.Distance(_startPos, curMousePos) * 0.01f, 0f, 3f) * 2f;

            // Set rotation, scale, position
            _arrowObj.transform.rotation = Quaternion.Euler(0, 0, angle);
            _arrowObj.GetComponent<SpriteRenderer>().size = new Vector2(1f, arrowScale);
            _arrowObj.transform.localPosition = new Vector3(_diff.x * (arrowScale / 2), _diff.y * (arrowScale / 2), 0f);

            if (_prevArrowScale < arrowScale)
            {
                _sp.PlayRapidSound(0);
            }

            _prevArrowScale = arrowScale;
        }

        else
        {
            _arrowObj.SetActive(false);
        }


        // Handle mouse controls
        if (Input.GetMouseButtonDown(0) && _isGrounded)
        {
            _isDragging = true;
            _startPos = curMousePos;
        }

        if (Input.GetMouseButtonUp(0) && _isGrounded && _canPlaySound)
        {
            _isDragging = false;
            _rb.velocity = _diff * 10.0f;
            _curPlatform = null;
            _prevArrowScale = 0.0f;

            _sp.PlaySound(1);
        }
    }

    public void ResetGame()
    {
        _isDragging = false;
        _canPlaySound = false;
        StartCoroutine(IPreventStartSounds());

        _rb.velocity = Vector2.zero;
        transform.SetParent(_originalParent);
        transform.position = _initPlayerPos;
        _prize.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            _isGrounded = true;
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            _curPlatform = col.gameObject.transform;

            transform.SetParent(_curPlatform);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            _isGrounded = false;
            _curPlatform = null;

            if (this.gameObject.activeInHierarchy)
            {
                StartCoroutine(DelayedUnparent());   
            }
        }
    }

    IEnumerator DelayedUnparent()
    {
        yield return null; // Wait for one frame
        transform.SetParent(_originalParent);
    }
}
