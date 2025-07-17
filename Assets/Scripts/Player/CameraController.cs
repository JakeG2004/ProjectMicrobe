using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

	#region variables
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
	#endregion


	void Awake()
	{
		cam = Camera.main ? Camera.main.transform : transform;
		GM.cam = cam;

		if (!character)
		{
			character = GameObject.FindGameObjectWithTag("Player").transform;
		}

		if (character)
		{
			lookPos = character.position + Vector3.up * lookPosYOffest;
		}
		filter = GetComponent<AudioLowPassFilter>();
	}

	void Update()
	{
		if (character == null || _states == null)
		{
			return;
		}

		SetLookPos();
		Zoom();

		if (_mouseTracking)
		{
			RotateCameraDirection();
		}

		PositionCamera();
		LowPassFilterIfSubmerged();
	}


	void SetLookPos()
	{
		Vector3 lookGoal = character.position + Vector3.up * lookPosYOffest;
		lookPos = Vector3.Lerp(lookPos, lookGoal, Time.deltaTime * 5f);
	}
	public void SetLookPosExternal()
	{
		lookPos = character.position + Vector3.up * lookPosYOffest;
		Debug.Log("Moving Camera Look Position!");
	}

	void Zoom()
	{
		zoomGoal = Mathf.Lerp(zoomBounds.x, zoomBounds.y, _states.zoom);
		CameraColision();
		zoom = Mathf.Lerp(zoom, Mathf.Min(zoomGoal, zoomCollision), Time.deltaTime * 5f);
	}
	void CameraColision()
	{
		if (Physics.Raycast(lookPos, directionSmooth, out RaycastHit hit, zoomGoal, ~mask))
		{
			zoomCollision = Mathf.Max(Vector3.Distance(lookPos, hit.point) - 0.1f, 0.8f);
		}
		else zoomCollision = zoomBounds.y;
	}
	void RotateCameraDirection()
	{
		angleVert = ClampAngle(angleVert - _states.look.y * _states.lookSensitivity / 2, angleVertBounds.x, angleVertBounds.y);
		angleHoz += _states.look.x * _states.lookSensitivity;
		// also turn camera when player moves to the side 
		angleHoz += _states.move.x * 1.5f;

		Vector3 directionHoz = Quaternion.AngleAxis(angleHoz, Vector3.up) * Vector3.forward;
		Vector3 directionHozLeft = Vector3.Cross(directionHoz, Vector3.up);
		directionGoal = Quaternion.AngleAxis(angleVert, directionHozLeft) * directionHoz;
		//Debug.DrawRay(lookPos, cameraDirection, Color.red);
	}
	void PositionCamera()
	{
		Vector3 posGoal = lookPos + directionGoal * zoom;
		Vector3 posSmooth = Vector3.Lerp(cam.position, posGoal, Time.deltaTime * 10f);
		directionSmooth = (posSmooth - lookPos).normalized;

		//Debug.DrawRay(lookPos, directionSmooth, Color.blue);
		cam.position = lookPos + directionSmooth * zoom;
		cam.LookAt(lookPos);
	}
	float ClampAngle(float angle, float min, float max)
	{
		do
		{
			if (angle < -360) angle += 360;
			if (angle > 360) angle -= 360;
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