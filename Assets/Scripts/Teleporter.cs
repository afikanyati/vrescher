using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

	public GameObject TeleportMarker;
	public Transform OVRCameraRig;
	public Transform LocalAvatar;
	public float RayLength = 1000000f;
	public float PlayerHeight = 1.8f;
	private Vector2 ThumbstickPos = new Vector2 (0.0f, 0.0f);
	private bool TeleportOn = false;
	public OVRInput.Controller Controller = OVRInput.Controller.RTouch;
	private bool arrowOrientationUpdated = false;

	// Use this for initialization
	void Start () {
		OVRInput.Update ();
	}

	// Update is called once per frame
	void Update () {
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		ThumbstickPos = OVRInput.Get (OVRInput.Axis2D.SecondaryThumbstick);
		// Debug.Log (ThumbstickPos);
		// Debug.Log (Controller);

		if (TeleportOn && !LocalAvatar.GetComponent<AudioSource> ().isPlaying) {
			LocalAvatar.GetComponent<AudioSource> ().Play ();
		}

		if (Physics.Raycast (ray, out hit, RayLength)) {
			if (hit.collider.tag == "Ground"
				&& (Mathf.Abs(ThumbstickPos[0]) > 0.5f
					|| Mathf.Abs(ThumbstickPos[1]) > 0.5f)) {
				TeleportOn = true;
				if (!TeleportMarker.activeSelf) {
					TeleportMarker.SetActive (true);
				}
				TeleportMarker.transform.position = hit.point;
				TeleportMarker.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				// Vector3 posRTouch = OVRInput.GetLocalControllerPosition (OVRInput.Controller.RTouch);

				float angle = Mathf.Atan2 (ThumbstickPos [1], ThumbstickPos [0]) * Mathf.Rad2Deg;

				Vector3 vector = new Vector3 (0.0f, angle, 0.0f);
				GameObject TeleportMarkerArrow = GameObject.Find ("arrow");
				StartCoroutine(RotateObject(TeleportMarkerArrow, vector,   1));

			} else {
				TeleportMarker.SetActive (false);
			}
		} else {
			TeleportMarker.SetActive (false);
		}


		// Need to check where the user is looking to teleport him
		if (TeleportOn && OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) == Vector2.zero) {
			Vector3 markerPosition = TeleportMarker.transform.position;

			if (hit.normal.x == 1) {
				OVRCameraRig.position = new Vector3 (markerPosition.x + PlayerHeight, markerPosition.y, markerPosition.z);
				LocalAvatar.position = new Vector3 (markerPosition.x + PlayerHeight, markerPosition.y, markerPosition.z);
			} else if (hit.normal.x == -1) {
				OVRCameraRig.position = new Vector3 (markerPosition.x - PlayerHeight, markerPosition.y, markerPosition.z);
				LocalAvatar.position = new Vector3 (markerPosition.x - PlayerHeight, markerPosition.y, markerPosition.z);
			} else if (hit.normal.y == 1) {
				OVRCameraRig.position = new Vector3 (markerPosition.x, markerPosition.y + PlayerHeight, markerPosition.z);
				LocalAvatar.position = new Vector3 (markerPosition.x, markerPosition.y + PlayerHeight, markerPosition.z);
			} else if (hit.normal.y == -1) {
				OVRCameraRig.position = new Vector3 (markerPosition.x, markerPosition.y - PlayerHeight, markerPosition.z);
				LocalAvatar.position = new Vector3 (markerPosition.x, markerPosition.y - PlayerHeight, markerPosition.z);
			} else if (hit.normal.z == 1) {
				OVRCameraRig.position = new Vector3 (markerPosition.x, markerPosition.y, markerPosition.z + PlayerHeight);
				LocalAvatar.position = new Vector3 (markerPosition.x, markerPosition.y, markerPosition.z + PlayerHeight);
			} else if (hit.normal.z == -1) {
				OVRCameraRig.position = new Vector3 (markerPosition.x, markerPosition.y, markerPosition.z - PlayerHeight);
				LocalAvatar.position = new Vector3 (markerPosition.x, markerPosition.y, markerPosition.z - PlayerHeight);
			}
			LocalAvatar.GetComponent<AudioSource> ().Stop ();
			OVRCameraRig.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			LocalAvatar.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			OVRCameraRig.GetComponent<AudioSource> ().Play ();
			TeleportOn = false;
		}
	}

	IEnumerator RotateObject(GameObject asset, Vector3 byAngles, float inTime) {
		Quaternion fromAngle = asset.transform.rotation;
		Quaternion toAngle = Quaternion.Euler(asset.transform.eulerAngles + byAngles);
		for(var t = 0.0f; t < 1; t += Time.deltaTime/inTime) {
			asset.transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
			yield return null;
		}
	}

	private bool isBetween(float input, float min, float max) {
		if (input < max  && input > min) {
			return true;
		} else {
			return false;
		}
	}

	private bool Vector3InRange(Vector3 input, Vector3 checker, float offset)  {
		if (isBetween (input.x, checker.x - offset, checker.x + offset)
			&& isBetween (input.y, checker.y - offset, checker.y + offset)
			&& isBetween (input.z, checker.z - offset, checker.z + offset)) {
			return true;
		} else {
			return false;
		}
	}
}
