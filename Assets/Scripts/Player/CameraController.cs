using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	Transform cam;
	[SerializeField] Transform character;
	[SerializeField] LayerMask mask;
	[SerializeField] private PlayerStatesSO _states;

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

	private bool _aboveWater = false;

	[SerializeField] private bool _mouseTracking = true;

	void Awake()
	{
		cam = Camera.main ? Camera.main.transform : transform;

		if (!character)
		{
			character = GameObject.FindGameObjectWithTag("Player").transform;
		}

		filter = GetComponent<AudioLowPassFilter>();
	}

	void Start()
	{
		lookPos = character.position + Vector3.up * lookPosYOffest;
	}

	void LateUpdate()
	{
		if (character == null || _states == null)
		{
			return;
		}

		CalculateLookGoal();
		Zoom();

		RotateCameraDirection();

		PositionCamera();
		LowPassFilterIfSubmerged();
	}


	// Calculates the target look position for the camera
	void CalculateLookGoal()
	{
		Vector3 lookGoal = character.position + Vector3.up * lookPosYOffest;
		lookPos = Vector3.Lerp(lookPos, lookGoal, Time.unscaledDeltaTime * 10f);
	}

	// Calculates how zoomed the camera should be
	void Zoom()
	{
		// Restrict the zoom to the specified limits
		zoomGoal = Mathf.Lerp(zoomBounds.x, zoomBounds.y, _states.zoom);

		// Calculate camera collision
		CameraColision();

		// Move the zoom towards a specified point, the min of zoom goal or zoom collision
		zoom = Mathf.Lerp(zoom, Mathf.Min(zoomGoal, zoomCollision), Time.unscaledDeltaTime * 10f);
	}

	// Handles the zoom when the camera is colliding
	void CameraColision()
	{
		// Use a spherecast to check where the camera will collide, and set it to be in front of that collision
		if (Physics.SphereCast(lookPos, 0.3f, directionSmooth, out RaycastHit hit, zoomGoal, ~mask))
		{
			zoomCollision = Mathf.Max(Vector3.Distance(lookPos, hit.point) - 0.1f, 0.8f);
		}

		// Otherwise, the zoom collision is the furtheset zoom
		else zoomCollision = zoomBounds.y;
	}

	// Handles manual rotation from the player
	void RotateCameraDirection()
	{
		// Modify the look vector so that mouse movement feels more snappy at default settings
		Vector2 modifiedLook = _states.look * (NewInputController.Instance.GetCurrentInputDevice() == InputType.KeyboardMouse ? 1.5f : 1f);
		
		angleVert = ClampAngle(angleVert - modifiedLook.y * _states.movementVals.lookSensitivity / 2, angleVertBounds.x, angleVertBounds.y);
		angleHoz += modifiedLook.x * _states.movementVals.lookSensitivity;
		// also turn camera when player moves to the side 
		angleHoz += _states.move.x * 1.5f;

		Vector3 directionHoz = Quaternion.AngleAxis(angleHoz, Vector3.up) * Vector3.forward;
		Vector3 directionHozLeft = Vector3.Cross(directionHoz, Vector3.up);
		directionGoal = Quaternion.AngleAxis(angleVert, directionHozLeft) * directionHoz;
		//Debug.DrawRay(lookPos, cameraDirection, Color.red);
	}

	// Positions the camera in 3d space
	void PositionCamera()
	{
		Vector3 posGoal = lookPos + directionGoal * zoom;

		// Smoothly move towards the target position
		Vector3 posSmooth = Vector3.Lerp(cam.position, posGoal, Time.deltaTime * 10f);
		directionSmooth = (posSmooth - lookPos).normalized;

		directionSmooth = Vector3.Slerp(directionSmooth, directionGoal, Time.deltaTime * 10f);

		//Debug.DrawRay(lookPos, directionSmooth, Color.blue);
		cam.position = lookPos + directionSmooth * zoom;
		cam.LookAt(lookPos);
	}
	float ClampAngle(float angle, float min, float max)
	{
		do
		{
			if (angle < -360)
			{
				angle += 360;
			}

			if (angle > 360)
			{
				angle -= 360;
			}
		} while (angle < -360 || angle > 360);

		return Mathf.Clamp(angle, min, max);
	}
	void LowPassFilterIfSubmerged()
	{
		if (cam.position.y < 0 && _aboveWater)
		{
			_aboveWater = false;
			StartCoroutine(ChangeLowPassOverTime(330f));
		}
		else if(cam.position.y >= 0 && !_aboveWater)
		{
			_aboveWater = true;
			StartCoroutine(ChangeLowPassOverTime(10000f));
		}
	}

	public void SetMouseTracking(bool state)
	{
		_mouseTracking = state;
	}

	public void ToggleMouseTracking()
	{
		_mouseTracking = !_mouseTracking;
	}

	// Smoothly transition between different cutoff amounts utilizing the natural smoothing of LERPing with a current value
	private IEnumerator ChangeLowPassOverTime(float targetFreq)
	{
		float elapsedTime = 0.0f;
		float transitionTime = 0.5f;

		while (elapsedTime < transitionTime)
		{
			float step = elapsedTime / transitionTime;
			elapsedTime += Time.deltaTime;
			filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, targetFreq, step);
			yield return null;
		}

		filter.cutoffFrequency = targetFreq;
	}
}