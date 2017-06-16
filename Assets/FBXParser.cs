/**
 * Reading up custom FBX files
 * Shubham
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;

public class FBXParser : MonoBehaviour {

	public struct CreationTimeStamp{
		public int version;
		public int year,month,day;
		public int hour,minute,second,milisecond;
	};

	public struct FBXHeaderExtension{
		public int headerVersion;
		public int fbxVersion;
		public CreationTimeStamp timeStamp;
		public string creator;
	};

	struct Definition{
		public int modelCount;
		public int geometryCount;
		public int materialCount;
		public int poseCount;
	};

//	public struct ObjectProp{
//		public string objName;
//
//		public Vector3 objPosition;
//		public Vector3 ObjRotation;
//		public Vector3 objScale;
//
//		public List<Vector3> vertices;
//		public List<Vector3> normals;
//		public List<int> indices;		//trianlges: since the structure could also be quad {0,1,2,-4}
//		public List<int> triangles;		//need to convert the indices into list of triangles
//		public List<Vector2> uvs;
//	};

	public string fbxPath;
	public float scaleValue = 1.0f;
	public Material defaultMat;
	public FBXHeaderExtension header;
	public List<ObjectProp> meshList;
	//[SerializeField]
	//public ObjectProp objectProp;
	public int meshCount = 0;
	public int currMesh = -1;

	char[] trimChars = new char[]{' ', '\t', '\r', '\n'};

	void DebugHeader()
	{
		Debug.Log ("-------------------FBX INFO--------------------");
		//Debug.Log ("Header Version: " + header.fbxVersion.ToString ()+"\n Creation Time Stamp: "+ header.timeStamp.day.ToString());
		Debug.Log (String.Format ("Creator: {7}\nHeader Version: {0}\n Creation Time Stamp: [{1}/{2}/{3}::{4}:{5}:{6}]", header.fbxVersion.ToString (), header.timeStamp.day.ToString (),
			header.timeStamp.month.ToString (), header.timeStamp.year.ToString (), header.timeStamp.hour.ToString (), header.timeStamp.minute.ToString (), 
			header.timeStamp.second.ToString (), header.creator));		
	}

	void DebugData(int i)
	{
		Debug.Log ("Mesh: " + i.ToString ());
		Debug.Log ("----------Vertices-----------------------");
		foreach (Vector3 v in meshList[i].vertices) {
			Debug.Log (v);
		}
		Debug.Log ("----------Normals-----------------------");
		foreach (Vector3 v in meshList[i].normals) {
			Debug.Log (v);
		}
		Debug.Log ("----------Triangles-----------------------");
		foreach (int v in meshList[i].triangles) {
			Debug.Log (v);
		}
	}

	void Start()
	{
		
		//objectProp = new ObjectProp ();
		if (fbxPath != null) {
			meshList = new List<ObjectProp> ();
			ReadASCIIFbx (fbxPath);
			DebugHeader ();
			for (int i = 0; i < meshCount; i++) {
				DebugData (i);
				CreateMesh (i);
			}
		}

	}//end start func

	void ResetObjectProperties()
	{
//		objectProp.objName = "default";
//		objectProp.objPosition = Vector3.zero;
//		objectProp.ObjRotation = Vector3.zero;
//		objectProp.objScale = Vector3.one;
//
//		objectProp.vertices = new List<Vector3>();
//		objectProp.normals = new List<Vector3>();
//		objectProp.indices = new List<int>();
//		objectProp.triangles = new List<int>();
//		objectProp.uvs = new List<Vector2>();

//		objectProp.vertices = new List<Vector3> ();
//		objectProp.normals = new List<Vector3> ();
//		objectProp.indices = new List<int> ();
//		objectProp.triangles = new List<int> ();
//		objectProp.uvs = new List<Vector2> ();
	}

	void CreateMesh(int i)
	{
		//GameObject go = new GameObject ("Mesh" + (i).ToString ());
		GameObject go = new GameObject (meshList[i].objName);
		go.transform.SetParent (this.transform);

		MeshFilter mf = go.AddComponent<MeshFilter> ();
		go.AddComponent<MeshRenderer> ().material = defaultMat;

		Mesh mesh = new Mesh ();
		mesh.vertices = meshList[i].vertices.ToArray ();
		mesh.triangles = meshList[i].triangles.ToArray ();
		mesh.normals = meshList[i].normals.ToArray ();
		mesh.uv = meshList[i].uvs.ToArray ();

		mf.mesh = mesh;

		go.transform.localPosition = meshList [i].objPosition;
		go.transform.localRotation = Quaternion.Euler(meshList [i].ObjRotation);
		go.transform.localScale = meshList [i].objScale;
	}

	/**
	 * 1. C:\Users\METARVRSE\Desktop\FBX\Parse\test.fbx
	 * 2. C:\Users\METARVRSE\Desktop\FBX\Parse\plane.fbx
	 * 3. C:\Users\METARVRSE\Desktop\FBX\Parse\sphere_ascii.fbx
	 * */

	void ReadASCIIFbx(string path)
	{
		StreamReader sr = new StreamReader (path, Encoding.ASCII);

		//		stri;ng s = "random \t\n: { a: 23	.0}";
//		//int i = int.Parse (s);
//		s=s.Remove(0,s.IndexOf("a:")+2);
//		//if(s.Contains("Entr"))
//		Debug.Log(s);
//		string ns = s.TrimStart(trimChars);
//		Debug.Log (ns);
//		return;

		int lineCounter = 1;
		string currLine = sr.ReadLine ();

		while (currLine != null) {

			currLine = currLine.TrimStart (trimChars);

			if (currLine.Contains (";")) {
				lineCounter++;
				currLine = sr.ReadLine ();

			} else if (currLine.Contains ("FBXHeaderExtension")) {
				lineCounter++;
				ReadHeader (sr, sr.ReadLine(),ref lineCounter);
				//Debug.Log ("[Line "+lineCounter.ToString()+"]: " + currLine);

			} else if (currLine.Contains ("Definitions")) {
				lineCounter++;
				ReadDefinitions (sr, sr.ReadLine(),ref lineCounter);
				//Debug.Log ("[Line "+lineCounter.ToString()+"]: " + currLine);

			} else if (currLine.Contains ("Objects")) {
				lineCounter++;
				ReadObjectProperties (sr, sr.ReadLine(),ref lineCounter);
				//Debug.Log ("[Line "+lineCounter.ToString()+"]: " + currLine);
			}
//			} else if (currLine.Contains ("Relations")) {
//				ReadObjectRelations (sr, currLine,ref lineCounter);
//				//Debug.Log ("[Line "+lineCounter.ToString()+"]: " + currLine);
//
//			} else if (currLine.Contains ("Connections")) {
//				ReadObjectConnections (sr, currLine,ref lineCounter);
//				//Debug.Log ("[Line "+lineCounter.ToString()+"]: " + currLine);
//
//			} else {
//				//Debug.Log ("[Line "+lineCounter.ToString()+"]: " + currLine);
//
//			}
			lineCounter++;
			currLine = sr.ReadLine ();	
		}//end of while loop
	}//end ReadASCIIFbx

	void ReadHeader(StreamReader sr, string currLine,ref int lineCounter)
	{
		int loopCounter = 0;
		while (!currLine.Contains ("}") || loopCounter != 0) {

			if (currLine.Contains ("{"))
				loopCounter++;
			else if (currLine.Contains ("}")) {
				if (loopCounter == 0) {
					Debug.Log ("Some issue happened while reading Objects properties");
					return;
				} else
					loopCounter--;

				lineCounter++;
				currLine = sr.ReadLine ();
				//Debug.Log (currLine + "\t\t\t---" + loopCounter.ToString ());
				continue;
			} 


			if (currLine.Contains ("FBXHeaderVersion")) {
				currLine = currLine.Remove (0, currLine.IndexOf (':') + 1).TrimStart (trimChars) + " ";
				string num = "";
				int i = 0;
				while (Char.IsDigit (currLine [i])) {
					num += currLine [i].ToString ();
					i++;
				}
				header.fbxVersion = int.Parse (num);
			} else if (currLine.Contains ("Year")) {
				currLine = currLine.Remove (0, currLine.IndexOf (':') + 1).TrimStart (trimChars) + " ";
				string num = "";
				int i = 0;
				while (Char.IsDigit (currLine [i])) {
					num += currLine [i].ToString ();
					i++;
				}
				header.timeStamp.year = int.Parse (num);
			} else if (currLine.Contains ("Month")) {
				currLine = currLine.Remove (0, currLine.IndexOf (':') + 1).TrimStart (trimChars) + " ";
				string num = "";
				int i = 0;
				while (Char.IsDigit (currLine [i])) {
					num += currLine [i].ToString ();
					i++;
				}
				header.timeStamp.month = int.Parse (num);
			} else if (currLine.Contains ("Day")) {
				currLine = currLine.Remove (0, currLine.IndexOf (':') + 1).TrimStart (trimChars) + " ";
				string num = "";
				int i = 0;
				while (Char.IsDigit (currLine [i])) {
					num += currLine [i].ToString ();
					i++;
				}
				header.timeStamp.day = int.Parse (num);
			} else if (currLine.Contains ("Hour")) {
				currLine = currLine.Remove (0, currLine.IndexOf (':') + 1).TrimStart (trimChars) + " ";
				string num = "";
				int i = 0;
				while (Char.IsDigit (currLine [i])) {
					num += currLine [i].ToString ();
					i++;
				}
				header.timeStamp.hour = int.Parse (num);
			} else if (currLine.Contains ("Minute")) {
				currLine = currLine.Remove (0, currLine.IndexOf (':') + 1).TrimStart (trimChars) + " ";
				string num = "";
				int i = 0;
				while (Char.IsDigit (currLine [i])) {
					num += currLine [i].ToString ();
					i++;
				}
				header.timeStamp.minute = int.Parse (num);
			} else if (currLine.Contains ("Second")) {
				currLine = currLine.Remove (0, currLine.IndexOf (':') + 1).TrimStart (trimChars) + " ";
				string num = "";
				int i = 0;
				while (Char.IsDigit (currLine [i])) {
					num += currLine [i].ToString ();
					i++;
				}
				header.timeStamp.second = int.Parse (num);
			} else if (currLine.Contains ("Creator")) {
				currLine = currLine.Remove (0, currLine.IndexOf (':') + 1).TrimStart (trimChars);
				currLine = currLine.TrimEnd (trimChars);
				header.creator = currLine;
			}


			//Debug.Log ("[Hdr " + lineCounter.ToString () + "]: " + currLine);
			lineCounter++;
			currLine = sr.ReadLine ();
		}
	}

	void ReadDefinitions(StreamReader sr, string currLine, ref int lineCounter)
	{
		int loopCounter = 0;
		bool checkModelInfo = false;
		while (!currLine.Contains ("}") || loopCounter != 0) {

			if (currLine.Contains ("{"))
				loopCounter++;
			else if (currLine.Contains ("}")) {
				if (loopCounter == 0) {
					Debug.Log ("Some issue happened while reading Objects properties");
					return;
				} else {
					loopCounter--;
					if (checkModelInfo)
						checkModelInfo = false;
				}

				lineCounter++;
				currLine = sr.ReadLine ();
				//Debug.Log (currLine + "\t\t\t---" + loopCounter.ToString ());
				continue;
			} 
		

			if (currLine.Contains ("ObjectType") && currLine.Contains ("Mesh")) {
				checkModelInfo = true;
			}

			if (checkModelInfo) {
				if (currLine.Contains ("Count")) {
					currLine = currLine.Remove (0, currLine.IndexOf (':') + 1).TrimStart (trimChars) + " ";
					string num = "";
					int i = 0;
					while (Char.IsDigit (currLine [i])) {
						num += currLine [i].ToString ();
						i++;
					}
					meshCount = int.Parse (num);
				}
			}


			//Debug.Log ("[Def " + lineCounter.ToString () + "]: " + currLine);
			lineCounter++;
			currLine = sr.ReadLine ();
		}
	}

	void ReadObjectProperties(StreamReader sr, string currLine, ref int lineCounter)
	{
		int loopCounter = 0;
		bool readMesh = false;
		//ObjectProp objectProp;

		while (currLine!=null && (!currLine.Contains ("}") || loopCounter != 0)) {

			if (currLine.Contains ("{")) {
				loopCounter++;
				//Debug.Log ("[Obj " + lineCounter.ToString () + "]: " + currLine+"\n"+loopCounter.ToString());
			}
			else if (currLine.Contains ("}")) {
				if (loopCounter == 0) {
					Debug.Log ("Some issue happened while reading Objects properties");
					return;
				} else {
					loopCounter--;

					if (readMesh) {
						readMesh = false;
//						ObjectProp op = new ObjectProp ();
//						op.SetObjectProp (objectProp.objName, objectProp.objPosition, objectProp.ObjRotation, objectProp.objScale, objectProp.vertices, objectProp.normals,
//							objectProp.triangles, objectProp.indices, objectProp.uvs);
//						meshList.Add (op);
					}
					//Debug.Log ("[Obj " + lineCounter.ToString () + "]: " + currLine+"\n"+loopCounter.ToString());
				}
				
				lineCounter++;
				currLine = sr.ReadLine ();
				continue;
			}

			if (currLine.Contains ("Mesh:")) {
				//objectProp = new ObjectProp ();
				meshList.Add(new ObjectProp());
				currMesh++;
				readMesh = true;

				currLine = currLine.Remove (0, currLine.IndexOf ("*") + 2);

				string name = "";
				int i = 0;
				while (Char.IsLetter (currLine [i])) {
					name += currLine [i];
					i++;
				}

				meshList[currMesh].objName = name;
			}

			if (currLine.Contains ("Properties:")) {
				lineCounter++;
				currLine = sr.ReadLine ();
				ReadMeshProperties (sr, currLine, meshList [currMesh], ref lineCounter);
				loopCounter--;		//cause reading till the end of the block for properties
			}

			//Debug.Log ("[Obj " + lineCounter.ToString () + "]: " + currLine);

			if (currLine.Contains ("Vertices")) {
				//Debug.Log ("[Obj/Vert " + lineCounter.ToString () + "]: " + currLine+"\n"+loopCounter.ToString());
				while (!currLine.Contains ("a:")) {
					lineCounter++;
					currLine = sr.ReadLine ();
				}
				currLine = currLine.TrimStart (trimChars);
				int n=0;
				n=currLine.IndexOf ("a:");n+=2;
				//Debug.Log (n);
				//for trimming '		Vertices: *count{ 			a: '
				currLine = currLine.Remove (0, n);
				//Debug.Log ("trimmed vertices: " + currLine);
				currLine = currLine.TrimStart (trimChars);
				//Debug.Log ("trimmed vertices: " + currLine);
				ReadVertices (sr, currLine, meshList[currMesh]);
				//Debug.Log ("[Obj/Vert " + lineCounter.ToString () + "]: " + currLine+"\n"+loopCounter.ToString());
			}

			if (currLine.Contains ("PolygonVertexIndex")) {
				//Debug.Log ("[Obj/Ind " + lineCounter.ToString () + "]: " + currLine+"\n"+loopCounter.ToString());
				while (!currLine.Contains ("a:")) {
					lineCounter++;
					currLine = sr.ReadLine ();
				}
				currLine = currLine.TrimStart (trimChars);
				int n=0;
				n=currLine.IndexOf ("a:");n+=2;
				currLine = currLine.Remove (0, n);
				currLine = currLine.TrimStart (trimChars);

				ReadPolyQuad (sr, currLine, meshList[currMesh]);
				//Debug.Log ("[Obj/Ind " + lineCounter.ToString () + "]: " + currLine+"\n"+loopCounter.ToString());
			}

			if (currLine.Contains ("Normals")) {
				//Debug.Log ("[Obj/Norm " + lineCounter.ToString () + "]: " + currLine+"\n"+loopCounter.ToString());
				while (!currLine.Contains ("a:")) {
					lineCounter++;
					currLine = sr.ReadLine ();
				}
				currLine = currLine.TrimStart (trimChars);
				int n=0;
				n=currLine.IndexOf ("a:");n+=2;
				//Debug.Log (n);
				//for trimming '		Vertices: *count{ 			a: '
				currLine = currLine.Remove (0, n);
				currLine = currLine.TrimStart (trimChars);
				ReadNormals (sr, currLine, meshList[currMesh]);
				//Debug.Log ("[Obj/Norm " + lineCounter.ToString () + "]: " + currLine+"\n"+loopCounter.ToString());
			}

			lineCounter++;
			currLine = sr.ReadLine ();

		}
	}

	void ReadObjectRelations(StreamReader sr, string currLine, ref int lineCounter)
	{
		while (!currLine.Contains("}")) {

			Debug.Log ("[Rel "+lineCounter.ToString()+"]: " + currLine);
			lineCounter++;
			currLine = sr.ReadLine ();
		}
	}

	void ReadObjectConnections(StreamReader sr, string currLine, ref int lineCounter)
	{
		while (!currLine.Contains("}")) {

			Debug.Log ("[Conn "+lineCounter.ToString()+"]: " + currLine);
			lineCounter++;
			currLine = sr.ReadLine ();
		}
	}

	/// <summary>
	/// Reads the properties of Mesh.
	/// </summary>
	/// <param name="sr">loaded StreamReader.</param>
	/// <param name="line">currLine.</param>
	/// <param name="objectProp">Object property.</param>
	void ReadMeshProperties(StreamReader sr, string line, ObjectProp objectProp, ref int lineCounter)
	{
		while (!line.Contains ("}")) {
			
			if (line.Contains ("Lcl Translation")) {

				line = line.Remove (0, line.IndexOf ("A+") + 4);
				line = line.TrimEnd (trimChars);

				string name = "";
				double px, py, pz;
				px = py = pz = 0;
				bool readX, readY, readZ;
				readX = true;
				readY = readZ = false;
				for (int i = 0; i < line.Length; i++) {
					if (Char.IsDigit (line [i]) || line [i] == '-' || line [i] == '.') {
						name += line [i];
					} else if (line [i] == ',') {
						if (readX) {
							px = double.Parse (name);
							readX = false;
							readY = true;
						} else if (readY) {
							py = double.Parse (name);
							readY = false;
							readZ = true;
						}
						name = "";
					}
				}
				if (readZ) {
					pz = double.Parse (name);
					readZ = false;
				}
				objectProp.objPosition = new Vector3 ((float)px, (float)py, (float)pz);
			}//end translation
			else if (line.Contains ("Lcl Rotation")) {

				line = line.Remove (0, line.IndexOf ("A+") + 4);
				line = line.TrimEnd (trimChars);

				string name = "";
				double px, py, pz;
				px = py = pz = 0;
				bool readX, readY, readZ;
				readX = true;
				readY = readZ = false;
				for (int i = 0; i < line.Length; i++) {
					if (Char.IsDigit (line [i]) || line [i] == '-' || line [i] == '.') {
						name += line [i];
					} else if (line [i] == ',') {
						if (readX) {
							px = double.Parse (name);
							readX = false;
							readY = true;
						} else if (readY) {
							py = double.Parse (name);
							readY = false;
							readZ = true;
						}
						name = "";
					}
				}
				if (readZ) {
					pz = double.Parse (name);
					readZ = false;
				}
				objectProp.ObjRotation = new Vector3 ((float)px, (float)py, (float)pz);
			}//end rotation
			else if (line.Contains ("Lcl Scaling")) {

				line = line.Remove (0, line.IndexOf ("A+") + 4);
				line = line.TrimEnd (trimChars);

				string name = "";
				double px, py, pz;
				px = py = pz = 1;
				bool readX, readY, readZ;
				readX = true;
				readY = readZ = false;
				for (int i = 0; i < line.Length; i++) {
					if (Char.IsDigit (line [i]) || line [i] == '-' || line [i] == '.') {
						name += line [i];
					} else if (line [i] == ',') {
						if (readX) {
							px = double.Parse (name);
							readX = false;
							readY = true;
						} else if (readY) {
							py = double.Parse (name);
							readY = false;
							readZ = true;
						}
						name = "";
					}
				}
				if (readZ) {
					pz = double.Parse (name);
					readZ = false;
				}
				objectProp.objScale = new Vector3 ((float)px, (float)py, (float)pz);
			}//end scale

			lineCounter++;
			line = sr.ReadLine ();
		}//end while

//		Debug.Log (objectProp.objPosition);
//		Debug.Log (objectProp.ObjRotation);
//		Debug.Log (objectProp.objScale);

	}//end read property func

	/// <summary>
	/// Reads the vertices in Vector3 format and populates the ObjectProperties struct.
	/// </summary>
	/// <param name="sr">Stream reader that has loaded up the file.</param>
	/// <param name="line">Line that contains all the values.</param>
	void ReadVertices(StreamReader sr, string line, ObjectProp objectProp)
	{
		//currLine = sr.ReadLine ();
		string snum="";
		int num;
		Vector3 currVert;
		Double x, y, z;
		x = y = z = 0;
		bool readX = true, readY=false, readZ=false;
		foreach (char c in line) {
			if (Char.IsDigit (c) || c=='.' || c=='e' || c=='E')
				snum += c.ToString ();
			//Debug.Log (c);
			if (c == ',') {
				if (readX) {
					x = double.Parse (snum);
					readX = false;
					readY = true;
					//Debug.Log (x);
				} else if (readY) {
					y = double.Parse (snum);
					readY = false;
					readZ = true;
					//Debug.Log (y);
				} else {
					z = double.Parse (snum);
					currVert = new Vector3 ((float)x, (float)y, (float)z);
					//Debug.Log (currVert);
					objectProp.vertices.Add (currVert);
					readZ = false;
					readX = true;
					//Debug.Log (z);
					x = y = z = 0;
				}
				//Debug.Log (snum+"_____"+c.ToString());
				snum = "";
			} else if (c == ' ' || c== '\t' || c=='\n' || c=='\r') {
				z = double.Parse (snum);
				currVert = new Vector3 ((float)x, (float)y, (float)z);
				//Debug.Log (currVert);
				objectProp.vertices.Add (currVert);
				readX = true;
				x = y = z = 0;
				snum = "";
			} else if (c == '}')
				break;
		}

		if (readZ) {
			z = double.Parse (snum);
			currVert = new Vector3 ((float)x, (float)y, (float)z);
			//Debug.Log (currVert);
			objectProp.vertices.Add (currVert);
			readX = true;
			x = y = z = 0;
			snum = "";
		}

		Debug.Log (objectProp.vertices.Count.ToString() + " Vertices added");
		//objectProp.vertices.Add ();


	}

	/// <summary>
	/// Reads the poly quad from the ASCII file.
	/// </summary>
	/// <param name="sr">Stream reader that has loaded up the file.</param>
	/// <param name="line">the exact line that contains all the values.</param>
	void ReadPolyQuad(StreamReader sr, string line, ObjectProp objectProp)
	{
		string snum="";
		int num;
		int index;
		bool read = true;
		foreach (char c in line) {
			if (Char.IsDigit (c) || c=='-')
				snum += c.ToString ();
			//Debug.Log (c);
			if (c == ',') {
				index = int.Parse (snum);
				if (index < 0)
					index = index * (-1) - 1;
				objectProp.indices.Add (index);
				//Debug.Log (index);
				//Debug.Log (snum+"_____"+c.ToString());
				snum = "";
			} else if (c == ' ' || c== '\t' || c=='\n' || c=='\r') {
				index = int.Parse (snum);
				//FBX SPECIFIC WAY OF DEFINING END OF QUAD/POLY
				if (index < 0)
					index = index * (-1) - 1;
				//Debug.Log (index);
				objectProp.indices.Add (index);
				snum = "";
			} else if (c == '}')
				break;
		}

		if (snum!="") {
			index = int.Parse (snum);
			if (index < 0)
				index = index * (-1) - 1;
			//Debug.Log (index);
			objectProp.indices.Add (index);
			snum = "";
		}

		if (objectProp.indices.Count % 4 == 0) {
			SetTrianglesFromQuad (objectProp);
		} else {
			Debug.LogError ("[ShuLog]: Error reading quad poly's from file.");
		}
	}

	/// <summary>
	/// Sets the triangles from quad list which has been extracted from ReadPolyQuad method.
	/// </summary>
	void SetTrianglesFromQuad(ObjectProp objectProp)
	{
		Debug.Log ("Setting up triangle from quad list");
		int i=0;
		int a, b;
		a = b = 0;
		foreach (int ind in objectProp.indices) {
			if (i == 0) {
				a = ind;
				objectProp.triangles.Add (ind);
				i++;
			} else if (i == 1) {
				objectProp.triangles.Add (ind);
				i++;
			} else if (i == 2) {
				b = ind;
				objectProp.triangles.Add (ind);
				i++;
			}
			else {
				objectProp.triangles.Add (a);
				objectProp.triangles.Add (ind);
				objectProp.triangles.Add (b);
				i = 0;
			}
		}
	}

	/// <summary>
	/// Reads the normals.
	/// </summary>
	/// <param name="sr">Stream reader that has loaded up the file.</param>
	/// <param name="line">Line that contains all the values.</param>
	void ReadNormals(StreamReader sr, string line, ObjectProp objectProp)
	{
		string snum="";
		int num;
		Vector3 currNorm;
		Double x, y, z;
		x = y = z = 0;
		bool readX = true, readY=false, readZ=false;
		foreach (char c in line) {
			if (Char.IsDigit (c) || c=='.' || c=='e' || c=='E')
				snum += c.ToString ();
			//Debug.Log (c);
			if (c == ',') {
				if (readX) {
					x = double.Parse (snum);
					readX = false;
					readY = true;
					//Debug.Log (x);
				} else if (readY) {
					y = double.Parse (snum);
					readY = false;
					readZ = true;
					//Debug.Log (y);
				} else {
					z = double.Parse (snum);
					currNorm = new Vector3 ((float)x, (float)y, (float)z);
					//Debug.Log (currVert);
					objectProp.normals.Add (currNorm);
					readZ = false;
					readX = true;
					//Debug.Log (z);
					x = y = z = 0;
				}
				//Debug.Log (snum+"_____"+c.ToString());
				snum = "";
			} else if (c == ' ' || c== '\t' || c=='\n' || c=='\r') {
				z = double.Parse (snum);
				currNorm = new Vector3 ((float)x, (float)y, (float)z);
				//Debug.Log (currVert);
				objectProp.normals.Add (currNorm);
				readX = true;
				x = y = z = 0;
				snum = "";
			} else if (c == '}')
				break;
		}

		if (readZ) {
			z = double.Parse (snum);
			currNorm = new Vector3 ((float)x, (float)y, (float)z);
			//Debug.Log (currVert);
			objectProp.normals.Add (currNorm);
			readX = true;
			x = y = z = 0;
			snum = "";
		}

		Debug.Log (objectProp.normals.Count.ToString() + " normals added");
	}

}//end FBXParse class
