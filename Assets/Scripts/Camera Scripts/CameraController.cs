using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject target;
	public float heightOffset = 0.58f;
	public float distance = 12.0f;
	public float offsetFromWall = 0.1f;
	public float minDistance = 0.6f;
	public float maxDistance = 20f;
	public float xSpeed = 200f;
	public float ySpeed = 200f;
	public float yMinLimit = -80f;
	public float yMaxLimit = 80f;
	public float zoomSpeed = 40f;
	public float autoRotationSpeed = 3.0f;
	public float autoZoomSpeed = 5.0f;
	public LayerMask collisionLayer = -1;
	public bool alwaysRotateTorearOfTarget = false;
	public bool allowMouseInputX = true;
	public bool allowMouseInputY = true;

	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private float currentDistance;
	private float desiredDistance;
	private float correctDistance;
	private bool rotateBehind = false;
	private bool mouseSideButton = false;



	// Use this for initialization
	void Start () {

		Vector3 angles = transform.eulerAngles;
		xDeg = angles.x;
		yDeg = angles.y;
		currentDistance = distance;
		desiredDistance = distance;
		correctDistance = distance;

		if(alwaysRotateTorearOfTarget) {
			rotateBehind = true;
		}
	}

	void LateUpdate() {

		// Auto-move button pressed
		if(Input.GetButton("Toggle Move")) {
			mouseSideButton = !mouseSideButton;
		}

		// Player moved or otherwise interrupted the auto-move
		if ((mouseSideButton) && (Input.GetAxis("Vertical") != 0) || (Input.GetButton("Jump")) || (Input.GetMouseButton(0)) && (Input.GetMouseButton(1))) {
			mouseSideButton = false;
		}

		// If either mouse buttons are down, let the mouse control the camera
		if(GUIUtility.hotControl == 0) {
			if(Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
				//Check to see if mouse input is allowed on axis
				if(allowMouseInputX) {
					xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
				} else {
					RotateBehindTarget();
				}
					//Check to see if mouse input is allowed on axis
					if(allowMouseInputY) {
						yDeg += Input.GetAxis("Mouse Y") * xSpeed * 0.02f;
					} //else {
						//RotateBehindTarget();
					//}

					// Interrupt rotating behind if mouse wants control.
					if(!alwaysRotateTorearOfTarget) {
						rotateBehind = false;
					}
			}  else if((Input.GetAxis("Vertical") != 0) || (Input.GetAxis("Horizontal") != 0) ||  (rotateBehind) || (mouseSideButton)) {
				RotateBehindTarget();
			}
		}

		// Ensure the pitch is  within the camera contraints
		yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

		// Set the camera rotation
		Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);

		// Calculate the desired distance
		desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * Mathf.Abs (desiredDistance);

		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
		correctDistance = desiredDistance;

		// Calculate desired camera position
		Vector3 vTargetOffset = new Vector3(0, -heightOffset, 0);
		Vector3 position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

		// Check for collision using the target's desired registration point
		RaycastHit collisionHit;
		Vector3 trueTargetPosition = new Vector3(target.transform.position.x, target.transform.position.y + heightOffset, target.transform.position.z);

		// If there is a collision, correct the camera position
		bool isCorrected = false;
		if(Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayer)) {
			// Calculate the distance from the orignal estimated position to the collsion location.
			correctDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
			isCorrected = true;
		}

		// Smooth the transition by "lerping" the distance if it was corrected
		if(!isCorrected || correctDistance > currentDistance) {
			currentDistance = Mathf.Lerp(currentDistance, correctDistance, Time.deltaTime * autoZoomSpeed);
		} else {
			currentDistance = correctDistance;
		}

		// Keep within the limits
		currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
		// Recalculate position on the new currentDistance
		position = target.transform.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);

		// Finally, set the rotation and position of the camera
		transform.rotation = rotation;
		transform.position = position;
	}

	private void RotateBehindTarget() {

		float targetRotationAngle = target.transform.eulerAngles.y;
		float currentRotationAngle = transform.eulerAngles.y;
		xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, autoRotationSpeed * Time.deltaTime);

		// Stop rotating behind if its not completed
		if(targetRotationAngle == currentRotationAngle) {
			if(!alwaysRotateTorearOfTarget) {
				rotateBehind = false;
			}
		} else {
			rotateBehind = true;
		}
	}


	private float ClampAngle(float angle, float min, float max) {

		if(angle < -360f) {
			angle += 360f;
		}

		if (angle > 360f) {
			angle -= 360f;
		}

		return Mathf.Clamp(angle, min, max);
	}
	
}
