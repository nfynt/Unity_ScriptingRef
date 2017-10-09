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

	public static List<GameObject> objs;	//the objects that have been spawned
	public Vector3 goalPos=Vector3.zero;

	void Start()
	{
		objs = new List<GameObject> ();

		for (int i = 0; i < objCount; i++) {
			Vector3 pos = new Vector3 (Random.Range (-volumeRadius, volumeRadius), Random.Range (-volumeRadius, volumeRadius),
				              Random.Range (-volumeRadius, volumeRadius));
			GameObject go = Instantiate (flockPrefab, pos, Quaternion.identity);
			objs.Add (go);
			go.GetComponent<FlockObjectBehaviour> ().SetManager (this);
		}
	}

	void Update()
	{
		if (Random.Range (5, 50000) < 50) {
			goalPos = new Vector3(Random.Range (-volumeRadius, volumeRadius), Random.Range (-volumeRadius, volumeRadius),
				Random.Range (-volumeRadius, volumeRadius));
		}
	}
}
