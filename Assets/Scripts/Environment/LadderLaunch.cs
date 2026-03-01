using UnityEngine;

public class LadderLaunch : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerController.Instance.SetClimbPos(transform);
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerController.Instance.EndClimb();
		}
	}
}