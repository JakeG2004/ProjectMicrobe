using UnityEngine;

public class MobileInputButtons : MonoBehaviour {

	InputController ic;

	void Start() {
		ic = GM.playerInput;
	}
	void OnEnable() {
		ic.UseMobileControls(true);
	}
	void OnDisable() {
		ic.UseMobileControls(false);
	}
	public void ToggleSprint() {
		ic.sprintInvert = !ic.sprintInvert;
	}
	public void Jump() {
		ic.Jump(true);
	}
	public void StopJump() {
		ic.Jump(false);
	}
	public void ZoomIn() {
		ic.Zoom(true);
	}
	public void ZoomOut() {
		ic.Zoom(false);
	}
}