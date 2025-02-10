using UnityEngine;

public class CameraController : MonoBehaviour {

	#region variables
	Transform cam;
	[SerializeField] Transform character;
	[SerializeField] LayerMask mask;
	bool enableCollision = true;

	float zoom = 4f;
	float zoomGoal = 4f;
	float zoomCollision = 7f;
	float zoomDamping = 5f;
	float zoomSensitivity = 0.5f;
	Vector2 zoomBounds = new Vector2(1.5f, 7f);

	Vector2 mouseSensitivity = new Vector2(5f, 5f);
	float keySensitivity = 3f;
	float angleVert = 30f;
	float angleHoz = 0f;
	Vector2 angleVertBounds = new Vector2(-40f, 70f);

	Vector3 lookPos;
	Vector3 lookGoal;
	bool lookSmooth = true;
	float lookPosDamping = 5f;
	float lookPosYOffest = 2f;
	
	float posDamping = 10f;
	Vector3 posGoal;
	Vector3 posSmooth;
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
	void Update() {
		if (character == null) return;
		SetLookPos();
		Zoom();
		RotateCameraDirection();
		PositionCamera();
		LowPassFilterIfSubmerged();
	}
	

	void SetLookPos() {
		lookGoal = character.position + Vector3.up * lookPosYOffest;
		lookPos = lookSmooth? Vector3.Lerp(lookPos, lookGoal, Time.deltaTime * lookPosDamping): lookGoal;
	}
	public void SetLookPosExternal() {
		lookPos = character.position + Vector3.up * lookPosYOffest;
		Debug.Log("Moving Camera Look Position!");
	}

	void Zoom() {
		zoomGoal += Input.GetAxis("Zoom") * zoomSensitivity;
		zoomGoal += Input.mouseScrollDelta.y * zoomSensitivity;
		//Debug.Log("Zoom += " + Input.GetAxis("Zoom"));


		zoomGoal = Mathf.Clamp(zoomGoal, zoomBounds.x, zoomBounds.y);
		cameraColision();
		zoom = Mathf.Lerp(zoom, Mathf.Min(zoomGoal, zoomCollision), Time.deltaTime * zoomDamping);
	}
	void cameraColision() {
		if (!enableCollision) return;
		RaycastHit hit;
		if (Physics.Raycast(lookPos, directionSmooth, out hit, zoomGoal, ~mask)) {
			zoomCollision = Mathf.Max(Vector3.Distance(lookPos, hit.point) - 0.1f,0.8f);
		}
		else zoomCollision = zoomBounds.y;
	}
	void RotateCameraDirection() {
		angleVert = ClampAngle(angleVert - Input.GetAxis("Mouse Y") * mouseSensitivity.y, angleVertBounds.x, angleVertBounds.y);

		angleHoz += Input.GetAxis("Mouse X") * mouseSensitivity.x;
		
		//if(Settings.useTurning) 
		angleHoz += Input.GetAxis("Horizontal") * keySensitivity;
		

		Vector3 directionHoz = Quaternion.AngleAxis(angleHoz, Vector3.up) * Vector3.forward;
		Vector3 directionHozLeft = Vector3.Cross(directionHoz, Vector3.up);
		directionGoal = Quaternion.AngleAxis(angleVert, directionHozLeft) * directionHoz;
		//Debug.DrawRay(lookPos, cameraDirection, Color.red);
	}
	void PositionCamera() {
		posGoal = lookPos + directionGoal * zoom;
		posSmooth = Vector3.Lerp(cam.position, posGoal, Time.deltaTime * posDamping);
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