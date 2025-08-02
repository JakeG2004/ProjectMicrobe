using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    [SerializeField] private PlayerStatesSO _states;
    [SerializeField] private float _moveSpeed = 10.0f;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _rb.velocity = _states.minigameMove * _moveSpeed;

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

        SoundManager.PlaySound(SoundType.EIGHT_BIT_COLLECTED);
    }

    public void Click()
    {

    }
}
