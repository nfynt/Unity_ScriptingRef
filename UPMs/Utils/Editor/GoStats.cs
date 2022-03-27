using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace nfynt.utils.editor
{
	public class GOStats
	{
		[MenuItem("GameObject/NFYNT/Mesh Filter stats", false, 10)]
		public static void PrintMeshFilterStats()
		{
			MeshFilter mf = Selection.activeGameObject.GetComponent<MeshFilter>();
			if (mf == null)
			{
				Logger.Error("No mesh filter found!");
				return;
			}

			Mesh m = mf.sharedMesh;
			string msg = "{0}: (verts: {1}\ttris: {2}\tsub-meshes: {3})";
			Logger.Log(string.Format(msg, m.name, m.vertexCount, m.triangles.Length, m.subMeshCount));
		}
	}
}


/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 

*/