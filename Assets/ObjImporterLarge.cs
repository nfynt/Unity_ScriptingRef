using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class ObjImporterLarge : MonoBehaviour {

	#region singleton
	// Singleton code
	// Static can be called from anywhere without having to make an instance
	private static ObjImporter _instance;

	// If called check if there is an instance, otherwise create it
	public static ObjImporter Instance
	{
		get { return _instance ?? (_instance = new ObjImporter()); }
	}
	#endregion

	private List<List<int>> triangles;
	private List<List<Vector3>> vertices;
	private List<List<Vector2>> uv;
	private List<List<Vector3>> normals;
	private List<List<Vect3Int>> faceData;
	private List<List<int>> intArray;

	private List<int> curr_triangles;
	private List<Vector3> curr_vertices;
	private List<Vector2> curr_uv;
	private List<Vector3> curr_normals;
	private List<Vect3Int> curr_faceData;
	private List<int> curr_intArray;

	private const int MIN_POW_10 = -16;
	private const int MAX_POW_10 = 16;
	private const int NUM_POWS_10 = MAX_POW_10 - MIN_POW_10 + 1;
	private static readonly float[] pow10 = GenerateLookupTable();

	private Mesh[] mesh;

	// Use this for initialization
	public Mesh[] ImportFile(string filePath)
	{
		triangles = new List<List<int>> ();
		vertices = new List<List<Vector3>> ();
		uv = new List<List<Vector2>> ();
		normals = new List<List<Vector3>> ();
		faceData = new List<List<Vect3Int>> ();
		intArray = new List<List<int>> ();

		curr_triangles = new List<int> ();
		curr_vertices = new List<Vector3> ();
		curr_uv = new List<Vector2> ();
		curr_normals = new List<Vector3> ();
		curr_faceData = new List<Vect3Int> ();
		curr_intArray = new List<int> ();


		LoadMeshData(filePath);

		mesh = new Mesh[faceData.Count];

		Debug.Log ("Number of meshes: "+ faceData.Count.ToString());

		for (int ii = 0; ii < faceData.Count; ii++) {

			Vector3[] newVerts = new Vector3[faceData[ii].Count];
			Vector2[] newUVs = new Vector2[faceData[ii].Count];
			Vector3[] newNormals = new Vector3[faceData[ii].Count];

			/* The following foreach loops through the facedata and assigns the appropriate vertex, uv, or normal
         * for the appropriate Unity mesh array.
         */
			for (int i = 0; i < faceData[ii].Count; i++) {
				newVerts [i] = vertices[ii] [faceData[ii] [i].x - 1];
				if (faceData[ii] [i].y >= 1)
					newUVs [i] = uv[ii] [faceData[ii] [i].y - 1];

				if (faceData[ii] [i].z >= 1)
					newNormals [i] = normals[ii] [faceData[ii] [i].z - 1];
			}
			Debug.Log (ii);
			mesh[ii].vertices = newVerts;
			mesh[ii].uv = newUVs;
			mesh[ii].normals = newNormals;
			mesh[ii].triangles = triangles[ii].ToArray ();

			mesh[ii].RecalculateBounds ();
		}

		return mesh;
	}

	private void LoadMeshData(string fileName)
	{

		StringBuilder sb = new StringBuilder ();
		string text = File.ReadAllText (fileName);
		int start = 0;
		string objectName = null;
		int faceDataCount = 0;

		bool vCont = false;
		int vCount = -1;

		StringBuilder sbFloat = new StringBuilder ();

		for (int i = 0; i < text.Length; i++) {
			if (text [i] == '\n') {
				sb.Remove (0, sb.Length);

				// Start +1 for whitespace '\n'
				sb.Append (text, start + 1, i - start);
				start = i;

				if (sb [0] == 'o' && sb [1] == ' ') {
					sbFloat.Remove (0, sbFloat.Length);
					int j = 2;
					while (j < sb.Length) {
						objectName += sb [j];
						j++;
					}
				} else if (sb [0] == 'v' && sb [1] == ' ') { // Vertices
					if (!vCont) {
						vCont = true;
						vCount++;

						if (curr_vertices.Count > 0) {
							vertices.Add (curr_vertices);
							uv.Add (curr_uv);
							normals.Add (curr_normals);
							faceData.Add (curr_faceData);
							intArray.Add (curr_intArray);
						}
						curr_vertices.Clear ();
						curr_uv.Clear ();
						curr_normals.Clear ();
						curr_faceData.Clear ();
						curr_intArray.Clear ();
					}

					int splitStart = 2;

					curr_vertices.Add (new Vector3 (GetFloat (sb, ref splitStart, ref sbFloat),
						GetFloat (sb, ref splitStart, ref sbFloat), GetFloat (sb, ref splitStart, ref sbFloat)));
				} else if (sb [0] == 'v' && sb [1] == 't' && sb [2] == ' ') { // UV
					if (vCont) {
						vCont = false;
					}
					int splitStart = 3;

					curr_uv.Add (new Vector2 (GetFloat (sb, ref splitStart, ref sbFloat),
						GetFloat (sb, ref splitStart, ref sbFloat)));
				} else if (sb [0] == 'v' && sb [1] == 'n' && sb [2] == ' ') { // Normals
					if (vCont) {
						vCont = false;
					}
					int splitStart = 3;

					curr_normals.Add (new Vector3 (GetFloat (sb, ref splitStart, ref sbFloat),
						GetFloat (sb, ref splitStart, ref sbFloat), GetFloat (sb, ref splitStart, ref sbFloat)));
				} else if (sb [0] == 'f' && sb [1] == ' ') {
					if (vCont) {
						vCont = false;
					}
					int splitStart = 2;

					int j = 1;
					curr_intArray.Clear ();
					int info = 0;
					// Add faceData, a face can contain multiple triangles, facedata is stored in following order vert, uv, normal. If uv or normal are / set it to a 0
					while (splitStart < sb.Length && char.IsDigit (sb [splitStart])) {
						curr_faceData.Add (new Vect3Int (GetInt (sb, ref splitStart, ref sbFloat),
							GetInt (sb, ref splitStart, ref sbFloat), GetInt (sb, ref splitStart, ref sbFloat)));
						j++;

						curr_intArray.Add (faceDataCount);
						faceDataCount++;
					}

					info += j;
					j = 1;
					while (j + 2 < info) { //Create triangles out of the face data.  There will generally be more than 1 triangle per face.
						curr_triangles.Add (curr_intArray [0]);
						curr_triangles.Add (curr_intArray [j]);
						curr_triangles.Add (curr_intArray [j + 1]);

						j++;
					}
				}
			}
		}

		vertices.Add (curr_vertices);
		uv.Add (curr_uv);
		normals.Add (curr_normals);
		faceData.Add (curr_faceData);
		intArray.Add (curr_intArray);
		curr_vertices.Clear ();
		curr_uv.Clear ();
		curr_normals.Clear ();
		curr_faceData.Clear ();
		curr_intArray.Clear ();
	}

	private float GetFloat(StringBuilder sb, ref int start, ref StringBuilder sbFloat)
	{
		sbFloat.Remove(0, sbFloat.Length);
		while (start < sb.Length &&
			(char.IsDigit(sb[start]) || sb[start] == '-' || sb[start] == '.'))
		{
			sbFloat.Append(sb[start]);
			start++;
		}
		start++;

		return ParseFloat(sbFloat);
	}

	private int GetInt(StringBuilder sb, ref int start, ref StringBuilder sbInt)
	{
		sbInt.Remove(0, sbInt.Length);
		while (start < sb.Length &&
			(char.IsDigit(sb[start])))
		{
			sbInt.Append(sb[start]);
			start++;
		}
		start++;

		return IntParseFast(sbInt);
	}


	private static float[] GenerateLookupTable()
	{
		var result = new float[(-MIN_POW_10 + MAX_POW_10) * 10];
		for (int i = 0; i < result.Length; i++)
			result[i] = (float)((i / NUM_POWS_10) *
				Mathf.Pow(10, i % NUM_POWS_10 + MIN_POW_10));
		return result;
	}

	private float ParseFloat(StringBuilder value)
	{
		float result = 0;
		bool negate = false;
		int len = value.Length;
		int decimalIndex = value.Length;
		for (int i = len - 1; i >= 0; i--)
			if (value[i] == '.')
			{ decimalIndex = i; break; }
		int offset = -MIN_POW_10 + decimalIndex;
		for (int i = 0; i < decimalIndex; i++)
			if (i != decimalIndex && value[i] != '-')
				result += pow10[(value[i] - '0') * NUM_POWS_10 + offset - i - 1];
			else if (value[i] == '-')
				negate = true;
		for (int i = decimalIndex + 1; i < len; i++)
			if (i != decimalIndex)
				result += pow10[(value[i] - '0') * NUM_POWS_10 + offset - i];
		if (negate)
			result = -result;
		return result;
	}

	private int IntParseFast(StringBuilder value)
	{
		// An optimized int parse method.
		int result = 0;
		for (int i = 0; i < value.Length; i++)
		{
			result = 10 * result + (value[i] - 48);
		}
		return result;
	}
}

public sealed class Vect3Int
{
	public int x { get; set; }
	public int y { get; set; }
	public int z { get; set; }

	public Vect3Int(){}

	public Vect3Int(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}
}
