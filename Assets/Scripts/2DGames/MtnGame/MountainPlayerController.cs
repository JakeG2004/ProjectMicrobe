// CoconutPlayerController.cs
// A script for managing the coconut game
// Author:  Jake Gendreau
// Date:    6/9/25

using UnityEngine;
using System.Collections;

public class MountainPlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStatesSO _states;
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
    float _prevArrowScale = 0.0f;
    bool _canPlaySound = false;
    private Vector2 _camVector;
    private bool _jumpTrigger = false;
    public float rotationOff = 180f;
    private NewInputController _controller;

    void Start()
    {
        _initPlayerPos = transform.position;
        _originalParent = transform.parent;
        _rb = GetComponent<Rigidbody2D>();

        _controller = NewInputController.Instance;

        _controller.minigameInput.OnSelectPressed += HandleJumpDown;
        _controller.minigameInput.OnSelectCanceled += HandleJumpUp;

        StartCoroutine(IPreventStartSounds());
    }

    void OnDisable()
    {
        _controller.minigameInput.OnSelectPressed -= HandleJumpDown;
        _controller.minigameInput.OnSelectCanceled -= HandleJumpUp;
    }

    private void HandleJumpDown() => _jumpTrigger = true;
    private void HandleJumpUp() => _jumpTrigger = false;


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
        if (NewInputController.Instance.GetCurrentInputDevice() == InputType.KeyboardMouse)
        {
            MouseControls();
        }

        else
        {
            GamepadControls();
        }
    }

    void GamepadControls()
    {
        Vector2 invertedMoveVector = -1 * _states.minigameMove;
        float arrowScale = invertedMoveVector.magnitude * 5f;
        if (arrowScale > 4.5f)
        {
            arrowScale = 5;
        }

        if (arrowScale > 0.05f)
        {
            _arrowObj.SetActive(true);

            float angle = Mathf.Atan2(_states.minigameMove.y, _states.minigameMove.x) * Mathf.Rad2Deg;

            // Set rotation, scale, position
            _arrowObj.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
            _arrowObj.GetComponent<SpriteRenderer>().size = new Vector2(1f, -1 * arrowScale);
            _arrowObj.transform.localPosition = new Vector3(invertedMoveVector.x * (arrowScale / 2), invertedMoveVector.y * (arrowScale / 2), 0f);

            if (_prevArrowScale < arrowScale)
            {
                SoundManager.PlaySound(SoundType.EIGHT_BIT_BASS);
            }

            _prevArrowScale = arrowScale;
        }

        else
        {
            _arrowObj.SetActive(false);
        }

        // Handle launch
        if (_isGrounded && arrowScale > 0.05f && _jumpTrigger)
        {
            _isDragging = false;
            _rb.velocity = invertedMoveVector * 10.0f;
            _curPlatform = null;
            _prevArrowScale = 0.0f;

            SoundManager.PlaySound(SoundType.EIGHT_BIT_JUMP);
        }

        if (_jumpTrigger)
        {
            _jumpTrigger = false;
        }
    }

    void MouseControls()
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
                SoundManager.PlaySound(SoundType.EIGHT_BIT_BASS);
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

            //_sp.PlaySound(1);
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

    public void GotFruit()
    {
        NewInputController.Instance.SetMenuMode();
    }

    void OnEnable()
    {
        NewInputController.Instance.SetMinigameMode();
    }
}
