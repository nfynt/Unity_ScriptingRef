using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Markerless : MonoBehaviour {

	//Gyroscope
	private Gyroscope gyro;
	private GameObject cameraContainer;
	private Quaternion rotation;

	//Accelerometer
	private Vector3 accl;
	private Vector3 camPos;

	//Cam
	private WebCamTexture cam;
	public RawImage background;
	public AspectRatioFitter fit;

	private bool arReady = false;

	void Start()
	{
		if (!SystemInfo.supportsGyroscope) {
			Debug.Log ("Gyroscope doesn't supported");
			#if !UNITY_EDITOR
			return;
			#endif
		}

		for (int i = 0; i < WebCamTexture.devices.Length; i++) {
			if (!WebCamTexture.devices [i].isFrontFacing) {
				cam = new WebCamTexture (WebCamTexture.devices [i].name, Screen.width, Screen.height);
				break;
			}
		}

		if (cam == null) {
			Debug.Log ("This device doesn't have back camera");
			#if UNITY_EDITOR
			cam = new WebCamTexture (WebCamTexture.devices [0].name, Screen.width, Screen.height);
			#else
			return;
			#endif
		}

		cameraContainer = new GameObject ("CameraContainer");
		cameraContainer.transform.position = this.transform.position;
		transform.SetParent (cameraContainer.transform);

		gyro = Input.gyro;
		gyro.enabled = true;
		cameraContainer.transform.rotation = Quaternion.Euler (90f, 0, 0);
		rotation = new Quaternion (0, 0, 1, 0);

		cam.Play ();
		background.texture = cam;

		arReady = true;
	}



	void Update()
	{
		if (arReady) {
			float ratio = (float)cam.width / (float)cam.height;
			fit.aspectRatio = ratio;

			float scaleY = cam.videoVerticallyMirrored ? -1.0f : 1.0f;
			background.transform.localScale = new Vector3 (1f, scaleY, 1f);

			int orient = -cam.videoRotationAngle;
			background.transform.localEulerAngles = new Vector3 (0f, 0f, orient);

			transform.localRotation = gyro.attitude * rotation;


			//accl = Input.acceleration;
			//Debug.Log (accl);
		}
	}
}
