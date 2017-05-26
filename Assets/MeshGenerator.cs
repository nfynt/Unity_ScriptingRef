using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))] [ExecuteInEditMode]
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

	List<Vector3> vertices = new List<Vector3>();
	List<Vector3> trianlges = new List<Vector3>();
	List<Vector3> normals = new List<Vector3>();

	void Start()
	{
		meshFilter = GetComponent<MeshFilter> ();
		meshRend = GetComponent<MeshRenderer> ();

		Mesh mesh = new Mesh ();


		meshFilter.mesh = mesh;
		print ("running");
	}

	void SetPlane()
	{
		
	}
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
			script.widthOfPlane = EditorGUILayout.FloatField ("Width Of Plane", script.heightOfPlane, null);
		}
	}
}
#endif