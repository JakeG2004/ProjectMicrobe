using UnityEngine;

public class LadderLaunch : MonoBehaviour
{

	PlayerStates _ps;

	void Start()
	{
		_ps = PlayerMovementController.Instance.GetStates();
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerMovementController.Instance.SetClimbPos(transform);
			_ps.isClimbing = true;
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			_ps.isClimbing = false;
		}
	}

	/*private void OnDrawGizmos()
	{
		// Set color
		Gizmos.color = Color.red;

		Vector3 forwardPosition = transform.position + transform.up * 5.0f;

		// Draw thje sphere
		Gizmos.DrawSphere(forwardPosition, 1.0f);
	}*/
}