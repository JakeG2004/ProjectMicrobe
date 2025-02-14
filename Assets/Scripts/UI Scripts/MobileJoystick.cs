using UnityEngine;
using UnityEngine.EventSystems;

// joystick ripped from 'Joystick Pack' asset.  Modified by Landon

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
	public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
	public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

	public float HandleRange {
		get { return handleRange; }
		set { handleRange = Mathf.Abs(value); }
	}
	public float DeadZone {
		get { return deadZone; }
		set { deadZone = Mathf.Abs(value); }
	}

	//public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }
	public enum AxisOptions { Both, Horizontal, Vertical }
	public bool SnapX { get { return snapX; } set { snapX = value; } }
	public bool SnapY { get { return snapY; } set { snapY = value; } }

	[SerializeField] float handleRange = 1;
	[SerializeField] float deadZone = 0;
	[SerializeField] AxisOptions axisOptions = AxisOptions.Both;
	[SerializeField] bool snapX = false;
	[SerializeField] bool snapY = false;

	[SerializeField] RectTransform background = null;
	[SerializeField] RectTransform handle = null;
	RectTransform baseRect = null;
	Canvas canvas;
	Camera cam;

	InputController ic;
	[Space(10)]
	[SerializeField] bool setMove = false;
	[SerializeField] bool setLook = false;

	private Vector2 input = Vector2.zero;

	void Start() {
		ic = GM.playerInput;

		HandleRange = handleRange;
		DeadZone = deadZone;
		baseRect = GetComponent<RectTransform>();
		canvas = GetComponentInParent<Canvas>();
		if (canvas == null)
			Debug.LogError("The Joystick is not placed inside a canvas");

		Vector2 center = new(0.5f, 0.5f);
		background.pivot = center;
		handle.anchorMin = center;
		handle.anchorMax = center;
		handle.pivot = center;
		handle.anchoredPosition = Vector2.zero;
	}

	void Update() {
		if (setMove) ic.mobileMoveInput = new(Horizontal, Vertical);
		if (setLook) ic.mobileLookInput = new(Horizontal, Vertical);
	}


	public void OnPointerDown(PointerEventData eventData) {
		OnDrag(eventData);
	}

	public void OnDrag(PointerEventData eventData) {
		cam = null;
		if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
			cam = canvas.worldCamera;

		Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
		Vector2 radius = background.sizeDelta / 2;
		input = (eventData.position - position) / (radius * canvas.scaleFactor);
		FormatInput();
		HandleInput(input.magnitude, input.normalized, radius, cam);
		handle.anchoredPosition = input * radius * handleRange;
	}

	void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam) {
		if (magnitude > deadZone) {
			if (magnitude > 1) input = normalised;
		}
		else input = Vector2.zero;
	}

	void FormatInput() {
		if (axisOptions == AxisOptions.Horizontal)
			input = new Vector2(input.x, 0f);
		else if (axisOptions == AxisOptions.Vertical)
			input = new Vector2(0f, input.y);
	}

	float SnapFloat(float value, AxisOptions snapAxis) {
		if (value == 0) return 0;

		if (axisOptions == AxisOptions.Both) {
			float angle = Vector2.Angle(input, Vector2.up);
			if (snapAxis == AxisOptions.Horizontal) {
				if (angle < 22.5f || angle > 157.5f) return 0;
				else return (value > 0) ? 1 : -1;
			}
			else if (snapAxis == AxisOptions.Vertical) {
				if (angle > 67.5f && angle < 112.5f) return 0;
				else return (value > 0) ? 1 : -1;
			}
			return value;
		}
		else {
			//note: value == 0 case already handled at the top
			if (value > 0) return 1;
			else return -1;
		}
	}

	public void OnPointerUp(PointerEventData eventData) {
		input = Vector2.zero;
		handle.anchoredPosition = Vector2.zero;
	}

	Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition) {
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out Vector2 localPoint)) {
			Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
			return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
		}
		return Vector2.zero;
	}
}