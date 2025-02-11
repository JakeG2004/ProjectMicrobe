using UnityEngine;

public class CameraController : MonoBehaviour {

	#region variables
	Transform cam;
	[SerializeField] Transform character;
	[SerializeField] LayerMask mask;

	InputController ic;

	float zoom = 4f;
	float zoomGoal = 4f;
	float zoomCollision = 7f;
	Vector2 zoomBounds = new(1.5f, 7f);

	float angleVert = 30f;
	float angleHoz = 0f;
	Vector2 angleVertBounds = new(-40f, 70f);

	Vector3 lookPos;
	readonly float lookPosYOffest = 2f;
	
	Vector3 directionGoal;
	Vector3 directionSmooth;

	AudioLowPassFilter filter;
	#endregion


	void Awake() {
		cam = Camera.main? Camera.main.transform : transform;
		GM.cam = cam;
		lookPos = character.position + Vector3.up * lookPosYOffest;
		filter = GetComponent<AudioLowPassFilter>();
	}
	void Start() {
		ic = GM.playerInput;
	}

	void Update() {
		if (character == null) return;
		SetLookPos();
		Zoom();
		RotateCameraDirection();
		PositionCamera();
		LowPassFilterIfSubmerged();
	}
	

	void SetLookPos() {
		Vector3 lookGoal = character.position + Vector3.up * lookPosYOffest;
		lookPos = Vector3.Lerp(lookPos, lookGoal, Time.deltaTime * 5f);
	}
	public void SetLookPosExternal() {
		lookPos = character.position + Vector3.up * lookPosYOffest;
		Debug.Log("Moving Camera Look Position!");
	}

	void Zoom() {
		zoomGoal = Mathf.Lerp(zoomBounds.x, zoomBounds.y, ic.zoom);
		CameraColision();
		zoom = Mathf.Lerp(zoom, Mathf.Min(zoomGoal, zoomCollision), Time.deltaTime * 5f);
	}
	void CameraColision() {
		if (Physics.Raycast(lookPos, directionSmooth, out RaycastHit hit, zoomGoal, ~mask)) {
			zoomCollision = Mathf.Max(Vector3.Distance(lookPos, hit.point) - 0.1f,0.8f);
		}
		else zoomCollision = zoomBounds.y;
	}
	void RotateCameraDirection() {
		angleVert = ClampAngle(angleVert - ic.look.y, angleVertBounds.x, angleVertBounds.y);
		angleHoz += ic.look.x;
		// also turn camera when player moves to the side 
		angleHoz += Input.GetAxis("Horizontal") * 3f;
		
		Vector3 directionHoz = Quaternion.AngleAxis(angleHoz, Vector3.up) * Vector3.forward;
		Vector3 directionHozLeft = Vector3.Cross(directionHoz, Vector3.up);
		directionGoal = Quaternion.AngleAxis(angleVert, directionHozLeft) * directionHoz;
		//Debug.DrawRay(lookPos, cameraDirection, Color.red);
	}
	void PositionCamera() {
		Vector3 posGoal = lookPos + directionGoal * zoom;
		Vector3 posSmooth = Vector3.Lerp(cam.position, posGoal, Time.deltaTime * 10f);
		directionSmooth = (posSmooth - lookPos).normalized;

		//Debug.DrawRay(lookPos, directionSmooth, Color.blue);
		cam.position = lookPos + directionSmooth * zoom;
		cam.LookAt(lookPos);
	}
	float ClampAngle(float angle, float min, float max) {
		do {
			if (angle < -360) angle += 360;
			if (angle > 360) angle -= 360;
		} while (angle < -360 || angle > 360);
		return Mathf.Clamp(angle, min, max);
	}
	void LowPassFilterIfSubmerged() {
		if (cam.position.y < 0) filter.cutoffFrequency = 330f;
		else filter.cutoffFrequency = 21000f;
	}
}