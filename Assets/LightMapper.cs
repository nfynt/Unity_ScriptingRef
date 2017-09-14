using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMapper : MonoBehaviour {

	public int lightmapNum;
	public int modNum;
	public Texture2D tex;

	private LightmapData[] defData;

	void Start()
	{
		defData = LightmapSettings.lightmaps;
	}

	void Update()
	{
		if (Input.GetKeyUp (KeyCode.U))
			SetNewTexs ();
	}

	void SetNewTexs()
	{
		LightmapData[] lightmapData = new LightmapData[lightmapNum];

		for (int i = 0; i < lightmapNum; i++)
			lightmapData [i] = new LightmapData ();

		for (int i = 0; i < lightmapNum; i++) {
			Debug.Log ("Lightmap and path : Yourlightmapfolder/LightmapFar-" + i.ToString ()); 			
			if (i == modNum)
				lightmapData [i].lightmapColor = tex;
			else
				lightmapData [i].lightmapColor = defData [i].lightmapColor;
			//lightmapData [i].lightmapFar = Resources.Load ("LightmapsCZC/LightmapFar-" + i.ToString (), typeof(Texture2D)) as Texture2D;
		}
		LightmapSettings.lightmaps = lightmapData;
	}
}
