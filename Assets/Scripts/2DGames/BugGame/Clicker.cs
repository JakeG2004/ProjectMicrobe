using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    /*private int _numBugsClicked = 0;
    private SoundPlayer _sp;

    void Start()
    {
        _sp = GetComponent<SoundPlayer>();
    }*/

    /*
        public void Click()
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(ITurnOffClicker());
        }

        private IEnumerator ITurnOffClicker()
        {
            yield return new WaitForSeconds(0.1f);
            GetComponent<IntGameEventTrigger>().TriggerEvent(_numBugsClicked);
            _numBugsClicked = 0;
            gameObject.SetActive(false);
        }*/

    [SerializeField] private float _moveSpeed = 10.0f;
    private PlayerInputActions _pia;
    private Rigidbody2D _rb;
    private Vector2 _moveVector;
    private SoundPlayer _sp;

    void Awake()
    {
        _sp = GetComponent<SoundPlayer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _pia = NewInputController.Instance.GetPlayerInputActions();

        // Movement lambdas
        _pia.Minigames.Move.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
        _pia.Minigames.Move.canceled += ctx => _moveVector = Vector2.zero;
    }

    void Update()
    {
        _rb.velocity = _moveVector * _moveSpeed;

        Vector3 newTransform = transform.localPosition;

        if (Mathf.Abs(transform.localPosition.x) > 2.6f)
        {
            newTransform.x = Mathf.Sign(transform.localPosition.x) * 2.6f;
        }

        if (transform.localPosition.y > -7.3f)
        {
            newTransform.y = -7.3f;
        }

        if (transform.localPosition.y < -12.7f)
        {
            newTransform.y = -12.7f;
        }

        transform.localPosition = newTransform;
    }

    void OnEnable()
    {
        NewInputController.Instance.SetMinigameMode();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        col.gameObject.GetComponent<RandomBugMovement>().ResetBug();
        GetComponent<IntGameEventTrigger>().TriggerEvent(1);

        if (_sp == null)
        {
            Debug.Log("test");
        }
        _sp.PlayRapidSound(0);
    }

    public void Click()
    {

    }
}
