using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using UnityEngine.UI;
using System.IO;
using Shubham.PDF;

//using Image = System.Drawing;
//C:\Users\shubh\Downloads\Docs\HKUST_EDX_Certificate.pdf

public class PdfReader : MonoBehaviour {
	
	public int totalPageCnt = 0;
	public UnityEngine.UI.RawImage pdfSceneImg;
	public Text logTxt;

	private int currPageIndex = 0;
	private PdfToImage pti;
	//private List<Texture2D> pdfPages = new List<Texture2D> ();

	//C:\Users\shubh\Documents\Unity_Projects\OpenCVTest\Assets\StreamingAssets\cert.pdf

	public void InitialisePdf(InputField path)
	{
		//pdfPages.Clear ();
		totalPageCnt = 0;
		//Debug.Log(path.text);
		pti = new PdfToImage ();
		PdfToImage.logs += PdfLog;

//		Debug.Log (pti.Test());
		if (pti.ReadPdf (path.text, ref totalPageCnt))
			logTxt.text = "Page: (" + (currPageIndex + 1).ToString () + "/" + totalPageCnt.ToString () + ")";
		else
			logTxt.text = "unable to read";
		
		pdfSceneImg.texture = pti.GetDrawImageFromPdf(0);
		//pdfSceneImg.texture = pdfPages [0];
	}

	public Texture2D ConvertImageToTexture(System.Drawing.Image image)
	{
		Texture2D tex = new Texture2D (image.Width, image.Height);
		MemoryStream ms = new MemoryStream ();
		image.Save (ms, System.Drawing.Imaging.ImageFormat.Jpeg);
		
		ms.Seek (0, SeekOrigin.Begin);
		tex.LoadImage (ms.ToArray ());
		
		ms.Close ();
		ms = null;

		return tex;
	}

	public void ShowNextPage()
	{
		if (currPageIndex >= totalPageCnt-1)
			return;

		currPageIndex++;
		
		pdfSceneImg.texture = pti.GetDrawImageFromPdf(currPageIndex);
		logTxt.text = "Page: (" + (currPageIndex + 1).ToString () + "/" + totalPageCnt.ToString () + ")";
	}

	public void ShowPrevPage()
	{
		if (currPageIndex < 1)
			return;

		currPageIndex--;

		pdfSceneImg.texture = pti.GetDrawImageFromPdf(currPageIndex);
		logTxt.text = "Page: (" + (currPageIndex + 1).ToString () + "/" + totalPageCnt.ToString () + ")";
	}

	public void PdfLog(string msg)
	{
		Debug.Log ("[PDF]: " + msg);
	}
}
