using UnityEngine;

public class LadderLaunch : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerMovementController.Instance.SetClimbPos(transform);
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerMovementController.Instance.SetClimbPos(null);
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