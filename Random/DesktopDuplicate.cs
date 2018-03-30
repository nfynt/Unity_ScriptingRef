using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

public class ScreenCaptureBuffer : MonoBehaviour {

	public static ScreenCaptureBuffer Instance;

	public GameObject screenObj;
	public bool EnableCapture;

	private List<Texture2D> frameBuff = new List<Texture2D> ();
	private bool capturingScreen;
	private int bufferCounter;
	[HideInInspector]
	public Texture2D defaultTex;

	void Start()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy (this.gameObject);

		capturingScreen = EnableCapture = false;
		bufferCounter = 0;
		defaultTex = new Texture2D (UnityEngine.Screen.width, UnityEngine.Screen.height, TextureFormat.RGB24, false);
		frameBuff.Add (defaultTex);
		frameBuff.Add (defaultTex);
		frameBuff.Add (defaultTex);
		frameBuff.Add (defaultTex);
	}

	void Update()
	{
		if (!capturingScreen && EnableCapture)
			StartCoroutine (CaptureScreen ());
	}

	IEnumerator CaptureScreen()
	{
		capturingScreen = true;
		yield return new WaitForEndOfFrame ();
//		UnityEngine.Application.CaptureScreenshot (UnityEngine.Application.dataPath + "/cap.png");
//		yield return 1;
//		byte[] imgbyte = File.ReadAllBytes(UnityEngine.Application.dataPath + "/cap.png");
//		defaultTex.LoadImage (imgbyte);
//		//defaultTex.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
//		yield return 0;
		defaultTex = GetScreenTexture();
		yield return 1;
		screenObj.GetComponent<MeshRenderer>().material.mainTexture = defaultTex;
		//yield return new WaitForSeconds (0.8f);
		Debug.Log ("Captured a shot");
		capturingScreen = false;
	}

	/// <summary>
	/// Gets the screen texture from windows graphics.
	/// </summary>
	/// <returns>The screen texture.</returns>
	Texture2D GetScreenTexture()
	{
		Texture2D tex = new Texture2D (200, 300, TextureFormat.RGB24, false);
		Rectangle screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
		Bitmap target = new Bitmap (screenSize.Width, screenSize.Height);
		using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(target)) {
			g.CopyFromScreen (0, 0, 0, 0, new Size (screenSize.Width, screenSize.Height));
		}
		MemoryStream ms = new MemoryStream ();
		target.Save (ms, System.Drawing.Imaging.ImageFormat.Png);
		ms.Seek (0, SeekOrigin.Begin);

		tex.LoadImage (ms.ToArray ());

		return tex;
	}

	public void ResetScreen()
	{
		screenObj.GetComponent<MeshRenderer>().material.mainTexture = defaultTex;
	}
}
