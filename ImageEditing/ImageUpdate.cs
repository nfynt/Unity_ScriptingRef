using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageUpdate : MonoBehaviour {

	public Texture2D tex;
	public int brushSize = 15;
	[Range(0,1)]
	public float brushAlpha = 0f;

	Color32[] texPix;
	int width,height;


	void Start()
	{
		//Initialising image components
		GetComponent<RawImage> ().texture = tex;
		width = tex.width;
		height = tex.height;
		texPix = tex.GetPixels32 ();
		tex = new Texture2D (width, height, TextureFormat.ARGB32,false);
		tex.SetPixels32 (texPix);
	}

	public void UpdateSizeAndAlpha()
	{
		
	}

	void Update()
	{
		//Update this to touch for mobile devices
		if (Input.GetMouseButton (0)) {
			Vector2 pixelPos = Input.mousePosition;

			//Removing the touched pixels
			for (int i = -brushSize; i < brushSize; i++)
				for (int j = -brushSize; j < brushSize; j++) {
					Color col = tex.GetPixel (i + (int)pixelPos.x, j + (int)pixelPos.y);
					tex.SetPixel ((int)Mathf.Clamp (i + pixelPos.x, 0, width), (int)Mathf.Clamp (j + (int)pixelPos.y, 0, height), new Color (col.r, col.g, col.b, brushAlpha));
				}
			
			UpdateImage ();
		}
	}

	void UpdateImage()
	{
		//tex = new Texture2D (width, height, TextureFormat.ARGB32,false);
		//tex.SetPixels32 (texPix);
		//Applying the updates
		tex.Apply ();
		GetComponent<RawImage> ().texture = tex;
	}
}
