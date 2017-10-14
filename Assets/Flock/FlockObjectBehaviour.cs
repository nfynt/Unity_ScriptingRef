using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockObjectBehaviour : MonoBehaviour {

	float speed = 0.5f;
	float rotationSpeed =  4.0f;

	Vector3 averageHeading;
	Vector3 averagePosition;
	float neighbourDist = 2f;
	FlockManager manager;

	void Start()
	{
		speed = Random.Range (speed, 3 * speed);
		if (GetComponent<Animator> () != null) {
			GetComponent<Animator> ().enabled = false;
			//GetComponent<Animator> ().SetBool ("wait", true);
			Invoke ("StartAnim", Random.Range (0.1f, 1f));
		}
	}

	void StartAnim()
	{
		GetComponent<Animator> ().enabled=true;
	}

	public void SetManager(FlockManager mgr)
	{
		manager = mgr;
		neighbourDist = mgr.neighbourRadius;
		speed = mgr.objSpeed;
		rotationSpeed = mgr.objRotSpeed;
	}

	void Update()
	{
		if(Random.Range(0,5) < 1)
			Check();
		transform.Translate(new Vector3(0,0,Time.deltaTime * speed));
	}

	void Check ()
	{
		GameObject[] gos = FlockManager.objs.ToArray ();

		Vector3 vCenter = Vector3.zero;
		Vector3 vAvoid = Vector3.zero;
		float gSpeed = 0.1f;

		Vector3 goalPos = manager.goalPos;

		float dist;

		int groupSize = 0;

		foreach (GameObject g in gos) {
			if (g != this.gameObject) {
				dist = Vector3.Distance (g.transform.position, this.transform.position);
				if (dist <= neighbourDist) {
					vCenter += g.transform.position;
					groupSize++;

					if (dist < 1f) {
						vAvoid += this.transform.position - g.transform.position;
					}

					gSpeed += g.GetComponent<FlockObjectBehaviour> ().speed;

				}
			}
		}

		if (groupSize > 0) {
			vCenter = vCenter / groupSize + (goalPos - this.transform.position);
			speed = gSpeed / groupSize;

			Vector3 dir = (vCenter + vAvoid) - transform.position;
			if (dir != Vector3.zero)
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (dir), rotationSpeed * Time.deltaTime);
		}

		if ((goalPos - this.transform.position).sqrMagnitude < 2f)
			manager.NextTarget ();

		SetManager (manager);
	}
}
