using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Diagnostics;

using Debug=UnityEngine.Debug;

//using Image = System.Drawing;
//C:\Users\shubh\Downloads\Docs\HKUST_EDX_Certificate.pdf

public class PdfReader : MonoBehaviour {

	public bool isPdf = false;
	public int totalPageCnt = 0;
	public UnityEngine.UI.RawImage pdfSceneImg;
	public Text logTxt;
	public bool loadingFile;

	private string officeFilePath;
	private List<Texture2D> renderedImages = new List<Texture2D> ();
	private int currPageIndex;
	//C:\Users\shubh\Documents\Unity_Projects\OpenCVTest\Assets\StreamingAssets\cert.pdf
	//C:\Users\shubh\Documents\Dev_Sources\PdfFilesTest

	void Start()
	{
		//ShuOffice.logs += this.FileLog;
	}

	public void InitialiseFile(InputField path)
	{

		if (isPdf)
			InitialisePdf (path);
		else
			InitialiseXlsx (path);
			//StartCoroutine (InitialiseXlsx (path));
	}

	void InitialisePdf(InputField path)
	{
		loadingFile = true;
		officeFilePath = path.text;

		try
		{
			string destPath = Path.Combine (Path.GetDirectoryName (officeFilePath), Path.GetFileNameWithoutExtension (officeFilePath));
			Process myProcess = new Process ();
			myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			myProcess.StartInfo.FileName = Application.dataPath + "/Plugins/Driver.exe";
			Debug.Log(":= "+Application.dataPath+"/Plugins/Driver.exe "+officeFilePath+" "+destPath+" 3");
			myProcess.StartInfo.Arguments = "\""+officeFilePath + "\" \"" + destPath + "\" 3";
			myProcess.EnableRaisingEvents = false;

			myProcess.Start ();

			myProcess.WaitForExit ();

			Debug.Log("[PDF] Finished processing: "+myProcess.ExitCode);

			renderedImages = new List<Texture2D>();
			totalPageCnt = 0;

			foreach(string s in Directory.GetFiles(destPath))
			{
				totalPageCnt++;
				Texture2D tex = new Texture2D(10,10);
				tex.LoadImage(File.ReadAllBytes(s));
				tex.Apply();
				renderedImages.Add(tex);
			}

			if (renderedImages.Count > 0) {
				pdfSceneImg.texture = renderedImages [0];
				currPageIndex = 0;
				Directory.Delete(destPath,true);
			}
		}
		catch(Exception ex) {
			UnityEngine.Debug.Log ("[PDF] Exception: " + ex.Message);
		}

	}

	//IEnumerator InitialiseXlsx(InputField path)
	void InitialiseXlsx(InputField path)
	{
		loadingFile = true;
		officeFilePath = path.text;

		try
		{
			string destPath = Path.Combine (Path.GetDirectoryName (officeFilePath), Path.GetFileNameWithoutExtension (officeFilePath));
			Process myProcess = new Process ();
			myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			myProcess.StartInfo.FileName = Application.dataPath + "/Plugins/Driver.exe";
			Debug.Log(":= "+Application.dataPath+"/Plugins/Driver.exe "+officeFilePath+" "+destPath+" 2");
			myProcess.StartInfo.Arguments = "\""+officeFilePath + "\" \"" + destPath + "\" 2";
			myProcess.EnableRaisingEvents = false;

			myProcess.Start ();

			myProcess.WaitForExit ();

			Debug.Log("[PDF] Finished processing: "+myProcess.ExitCode);

			renderedImages = new List<Texture2D>();
			totalPageCnt = 0;

			foreach(string s in Directory.GetFiles(destPath))
			{
				totalPageCnt++;
				Texture2D tex = new Texture2D(10,10);
				tex.LoadImage(File.ReadAllBytes(s));
				tex.Apply();
				renderedImages.Add(tex);
			}

			if (renderedImages.Count > 0) {
				pdfSceneImg.texture = renderedImages [0];
				currPageIndex = 0;
				Directory.Delete(destPath,true);
			}
		}
		catch(Exception ex) {
			UnityEngine.Debug.Log ("[XLSX] Exception: " + ex.Message);
		}
	}

	void InitialiseWordDoc(InputField path)
	{
		loadingFile = true;
		officeFilePath = path.text;

		try
		{
			string destPath = Path.Combine (Path.GetDirectoryName (officeFilePath), Path.GetFileNameWithoutExtension (officeFilePath));
			Process myProcess = new Process ();
			myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			myProcess.StartInfo.FileName = Application.dataPath + "/Plugins/Driver.exe";
			Debug.Log(":= "+Application.dataPath+"/Plugins/Driver.exe "+officeFilePath+" "+destPath+" 1");
			myProcess.StartInfo.Arguments = "\""+officeFilePath + "\" \"" + destPath + "\" 1";
			myProcess.EnableRaisingEvents = false;

			myProcess.Start ();

			myProcess.WaitForExit ();

			Debug.Log("[PDF] Finished processing: "+myProcess.ExitCode);

			renderedImages = new List<Texture2D>();
			totalPageCnt = 0;

			foreach(string s in Directory.GetFiles(destPath))
			{
				totalPageCnt++;
				Texture2D tex = new Texture2D(10,10);
				tex.LoadImage(File.ReadAllBytes(s));
				tex.Apply();
				renderedImages.Add(tex);
			}

			if (renderedImages.Count > 0) {
				pdfSceneImg.texture = renderedImages [0];
				currPageIndex = 0;
				Directory.Delete(destPath,true);
			}
		}
		catch(Exception ex) {
			UnityEngine.Debug.Log ("[PDF] Exception: " + ex.Message);
		}
	}


	public void ShowNextPage()
	{
		if (currPageIndex >= totalPageCnt-1)
			return;

		currPageIndex++;

			pdfSceneImg.texture = renderedImages [currPageIndex];

		logTxt.text = "Page: (" + (currPageIndex + 1).ToString () + "/" + totalPageCnt.ToString () + ")";
	}

	public void ShowPrevPage()
	{
		if (currPageIndex < 1)
			return;

		currPageIndex--;


		pdfSceneImg.texture = renderedImages [currPageIndex];

		logTxt.text = "Page: (" + (currPageIndex + 1).ToString () + "/" + totalPageCnt.ToString () + ")";
	}



}
