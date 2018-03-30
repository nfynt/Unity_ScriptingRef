using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour {

	public GameObject flockPrefab;	//the prefab of object to be used in flock
	public int objCount = 10;
	public int volumeRadius = 5;
	public float neighbourRadius = 10f;
	public float objSpeed = 2f;
	public float objRotSpeed = 4f;
	public bool userRadius=true;
	public float radius=5f;
	private float height;
	public int pointDensity=5;

	public static List<GameObject> objs;	//the objects that have been spawned
	public List<Vector3> pathPoints = new List<Vector3>();
	[HideInInspector]
	public Vector3 goalPos=Vector3.zero;
	int currTarget=0;

	void Start()
	{
		objs = new List<GameObject> ();
		height = transform.position.y;

		if (userRadius) {
			pathPoints.Clear ();
			pointDensity *= 4;
			float theta = 0;
			float delta = 360 / pointDensity;
			for (int i = 0; i < pointDensity; i++) {
				pathPoints.Add (new Vector3 (transform.position.x + radius * Mathf.Cos (theta * Mathf.Deg2Rad), height, transform.position.z + radius * Mathf.Sin (theta * Mathf.Deg2Rad)));
				theta += delta;
			}

		}

		NextTarget ();

		for (int i = 0; i < objCount; i++) {
			Vector3 pos = transform.position + new Vector3 (Random.Range (-volumeRadius, volumeRadius), Random.Range (-volumeRadius, volumeRadius),
				Random.Range (-volumeRadius, volumeRadius));
			GameObject go = Instantiate (flockPrefab, pos, Quaternion.identity);
			objs.Add (go);
			go.GetComponent<FlockObjectBehaviour> ().SetManager (this);
		}
	}

	public void NextTarget()
	{
		if (currTarget > pathPoints.Count - 1) {
			currTarget = 0;
		}
		
		goalPos = pathPoints [currTarget];
		currTarget++;
	}


	void OnDrawGizmosSelected(){
		Vector3 currPos=transform.position;
		Gizmos.color = Color.red;
		foreach (Vector3 v in pathPoints) {
			Gizmos.DrawLine (currPos,v);
			currPos = v;
		}

		Gizmos.color = Color.green;
		Gizmos.DrawSphere (transform.position, volumeRadius);
	}

}
