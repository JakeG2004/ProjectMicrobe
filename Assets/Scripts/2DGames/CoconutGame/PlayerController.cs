using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;           // World Space Canvas
    [SerializeField] private GameObject _arrow;       // Child of player
    private RectTransform _canvasRect;
    private Rigidbody2D _rb;
    private bool _dragging = false;
    private Vector2 _startLocalPos;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _canvasRect = canvas.GetComponent<RectTransform>();
        _arrow.SetActive(false);
    }

    void Update()
    {
        Vector2 localPos;

        // Get mouse position relative to the World Space Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            Input.mousePosition,
            canvas.worldCamera,
            out localPos
        );

        if (Input.GetMouseButtonDown(0))
        {
            _dragging = true;
            _startLocalPos = localPos;
            _arrow.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
            Vector2 endPos = localPos;
            Vector2 velocity = (_startLocalPos - endPos) * 1.0f; // tune multiplier
            _rb.velocity = velocity;
            _arrow.SetActive(false);
        }

        if (_dragging)
        {
            Vector2 direction = localPos - (Vector2)transform.localPosition;
            float length = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            _arrow.transform.localPosition = direction * 0.5f;
            _arrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
            _arrow.transform.localScale = new Vector3(length, 0.2f, 1f);
        }
    }
}
