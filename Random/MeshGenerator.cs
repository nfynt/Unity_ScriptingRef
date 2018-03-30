using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] [RequireComponent(typeof(MeshFilter))] 
public class MeshGenerator : MonoBehaviour {

	public Material defaultMat;
	[HideInInspector]
	public bool isPlane;
	[HideInInspector]
	public float heightOfPlane;
	[HideInInspector]
	public float widthOfPlane;

	MeshFilter meshFilter;
	MeshRenderer meshRend;
	Mesh mesh;

//	List<Vector3> vertices = new List<Vector3>();
//	List<int> trianlges = new List<int>();
//	List<Vector3> normals = new List<Vector3>();
//	List<Vector2> uvs = new List<Vector2>();

	Vector3[] vertices = new Vector3[4];
	int[] triangles = new int[6];
	Vector3[] normals = new Vector3[4];
	Vector2[] uvs = new Vector2[4];

	void Update()
	{
		meshFilter = GetComponent<MeshFilter> ();
		meshRend = GetComponent<MeshRenderer> ();

		if (isPlane)
			SetPlane ();
		
		//meshFilter.mesh = mesh;
		Debug.Log ("ShuMesh: MeshGenerated!");
	}

	void SetPlane()
	{
		#region comment
		//Vertices
//		vertices.Add (new Vector3 (0, 0, 0));
//		vertices.Add (new Vector3 (widthOfPlane, 0, 0));
//		vertices.Add (new Vector3 (0, heightOfPlane, 0));
//		vertices.Add (new Vector3 (widthOfPlane, heightOfPlane, 0));
//
//		//Triangles
//		trianlges.Add(0);
//		trianlges.Add(2);
//		trianlges.Add(1);
//		trianlges.Add(2);
//		trianlges.Add(3);
//		trianlges.Add(1);
//
//		//Normals: only for displaying the object in game
//		normals.Add(-Vector3.forward);
//		normals.Add(-Vector3.forward);
//		normals.Add(-Vector3.forward);
//		normals.Add(-Vector3.forward);
//
//		//UVs: displaynig the texture
//		uvs.Add(new Vector2(0,0));
//		uvs.Add(new Vector2(1,0));
//		uvs.Add(new Vector2(0,1));
//		uvs.Add(new Vector2(1,1));
//
//		Mesh mesh = new Mesh();
//		mesh.triangles = trianlges.ToArray ();
//		mesh.vertices = vertices.ToArray ();
//		mesh.normals = normals.ToArray ();
//		mesh.uv = uvs.ToArray ();
		#endregion
		vertices[0] = new Vector3 (0, 0, 0);
		vertices[1] = new Vector3 (widthOfPlane, 0, 0);
		vertices[2] = new Vector3 (0, heightOfPlane, 0);
		vertices[3] = new Vector3 (widthOfPlane, heightOfPlane, 0);

		//Triangles
		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 2;
		triangles[4] = 3;
		triangles[5] = 1;

		//Normals: only for displaying the object in game
		normals[0] = -Vector3.forward;
		normals[1] = -Vector3.forward;
		normals[2] = -Vector3.forward;
		normals[3] = -Vector3.forward;

		//UVs: displaynig the texture
		uvs[0] = new Vector2(0,0);
		uvs[1] = new Vector2(1,0);
		uvs[2] = new Vector2(0,1);
		uvs[3] = new Vector2(1,1);

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uvs;
		meshFilter.mesh = mesh;

	}//end of setplane
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeshGenerator))]
public class MeshGenerator_Editor: Editor{

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		MeshGenerator script = (MeshGenerator)target;
		script.isPlane = EditorGUILayout.Toggle ("Is Plane", script.isPlane);

		if (script.isPlane) {
			script.heightOfPlane = EditorGUILayout.FloatField ("Height Of Plane", script.heightOfPlane, null);
			script.widthOfPlane = EditorGUILayout.FloatField ("Width Of Plane", script.widthOfPlane, null);
		}
	}
}
#endif