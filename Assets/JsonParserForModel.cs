using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class JsonParserForModel : MonoBehaviour {

	public string filePath;
	public float scaleValue = 1.0f;
	public Material defaultMat;
	public int meshCount = 0;
	public int currMesh = -1;

	private Data2File df;
	private List<List<int>> meshTri;
	private List<List<Vector3>> meshVertices;
	private List<List<Vector3>> meshNormals;
	void Start()
	{
		if (filePath != null) {
			//Debug.Log (Time.time);
			string textStr = File.ReadAllText(filePath);
			df = new Data2File ();
			JsonConvert.PopulateObject (textStr, df);
			meshTri = new List<List<int>> ();
			meshVertices = new List<List<Vector3>> ();
			meshNormals = new List<List<Vector3>> ();
			for (int i = 0; i < df.ObjDefinitions.Obj.Count; i++) {
				SetMeshProps(i);
				CreateMesh (i);
			}
			//Debug.Log (df.Objs.MeshInfo[0].Name);
			Debug.Log ("File loading finished");
			//Debug.Log (Time.time);
		}
	}

	void CreateMesh(int i)
	{
		Debug.Log ("Creating mesh no: " + i.ToString ());
		//GameObject go = new GameObject ("Mesh" + (i).ToString ());
		GameObject go = new GameObject (df.Objs.MeshInfo[i].Name);
		go.transform.SetParent (this.transform);

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		go.AddComponent<MeshRenderer> ().material = defaultMat;

		Mesh mesh = new Mesh ();
		mesh.vertices = meshVertices[i].ToArray ();
		mesh.triangles = meshTri[i].ToArray ();//SaveLog (i);
		mesh.normals = meshNormals[i].ToArray ();
		//mesh.uv = df.objMesh.meshes[i].uvs.ToArray ();

		mf.mesh = mesh;

		go.transform.localPosition = new Vector3((float)df.Objs.MeshInfo[i].LclTranslation[0],(float)df.Objs.MeshInfo[i].LclTranslation[1],(float)df.Objs.MeshInfo[i].LclTranslation[2]);
		go.transform.localRotation = Quaternion.Euler (new Vector3((float)df.Objs.MeshInfo[i].LclRotation[0],(float)df.Objs.MeshInfo[i].LclRotation[1],(float)df.Objs.MeshInfo[i].LclRotation[2]));
		go.transform.localScale = new Vector3((float)df.Objs.MeshInfo[i].LclScale[0],(float)df.Objs.MeshInfo[i].LclScale[1],(float)df.Objs.MeshInfo[i].LclScale[2]) * scaleValue;
	}

	void SaveLog(int i)
	{
		if (i == 0)
			return;
		//File.CreateText (Application.dataPath + "\\tri.txt");
		string ss = "";
		foreach (int v in meshTri[i]) {
			ss += v.ToString () + "\n";
		}
		File.WriteAllText (Application.dataPath+"\\tri.txt",ss);
	}

	void SetMeshProps(int c)
	{
		Debug.Log ("Setting up triangle from quad list");
		int i=0;
		int a, b;
		a = b = 0;
		List<int> tri = new List<int> ();
		List<Vector3> vert = new List<Vector3> ();
		List<Vector3> norm = new List<Vector3> ();
		int temp=0;
		foreach (int ind in df.Objs.MeshInfo[c].PolygonVertexIndex) {
			temp = ind;
			if (temp < 0)
				temp = (Mathf.Abs (ind) - 1);
			if (i == 0) {
				a = temp;
				tri.Add (temp);
				i++;
			} else if (i == 1) {
				tri.Add (temp);
				i++;
			} else if (i == 2) {
				b = temp;
				tri.Add (temp);
				i++;
			}
			else {
				tri.Add (a);
				tri.Add (temp);
				tri.Add (b);
				i = 0;
			}
		}

		for (int j = 0; j < df.Objs.MeshInfo [c].Vertices.Count; j+=3) {
			vert.Add (new Vector3 ((float)df.Objs.MeshInfo [c].Vertices [j], (float)df.Objs.MeshInfo [c].Vertices [j+1], (float)df.Objs.MeshInfo [c].Vertices [j+2]));
			norm.Add (new Vector3 ((float)df.Objs.MeshInfo [c].Normals [j], (float)df.Objs.MeshInfo [c].Normals [j+1], (float)df.Objs.MeshInfo [c].Normals [j+2]));
		}

		meshVertices.Add (vert);
		meshNormals.Add (norm);
		meshTri.Add (tri);
		Debug.Log (meshTri [c].Count);
	}



}//end NewtonParser


public class Data2File
{
	public HeaderExtension HeaderExtension { get; set; }
	public ObjDefinitions ObjDefinitions { get; set; }
	public Objs Objs { get; set; }
}

public class CreationTimeStamp
{
	public int Year { get; set; }
	public int Month { get; set; }
	public int Day { get; set; }
	public int Hour { get; set; }
	public int Minute { get; set; }
	public int Second { get; set; }
}

public class HeaderExtension
{
	public int Version { get; set; }
	public CreationTimeStamp CreationTimeStamp { get; set; }
	public string Creator { get; set; }
}

public class Obj
{
	public string Type { get; set; }
	public int Count { get; set; }
}

public class ObjDefinitions
{
	public Obj Obj { get; set; }
}

public class MeshInfo
{
	public string Name { get; set; }
	public List<double> LclTranslation { get; set; }
	public List<double> LclRotation { get; set; }
	public List<double> LclScale { get; set; }
	public List<double> Vertices { get; set; }
	public List<int> PolygonVertexIndex { get; set; }
	public List<double> Normals { get; set; }
}

public class Objs
{
	public List<MeshInfo> MeshInfo { get; set; }
}
