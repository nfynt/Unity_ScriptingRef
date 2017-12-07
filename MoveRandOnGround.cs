using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRandOnGround : MonoBehaviour {

	public GameObject ground;
	public bool move = true;
	public float speed=1f;

	Vector3 minPoint;
	Vector3 maxPoint;
	Vector3 targetPoint;

	void Start()
	{
		ground = GameObject.FindWithTag ("ground");
		minPoint = ground.GetComponent<Collider> ().bounds.min;
		maxPoint = ground.GetComponent<Collider> ().bounds.max;
		//ChooseRandDirection ();
		InvokeRepeating ("ChooseRandDirection", 0.1f, 2f);

	}

	void FixedUpdate()
	{
		if ((targetPoint - transform.position).sqrMagnitude > 0.5f && ground!=null)
			transform.position = Vector3.Lerp (transform.position, targetPoint, Time.deltaTime * speed);
	}

	void ChooseRandDirection()
	{
		if (ground == null)
			return;
		
		targetPoint = new Vector3 (Random.Range (minPoint.x,maxPoint.x), transform.position.y, Random.Range (minPoint.z,maxPoint.z));

	}

	/// <summary>
	/// Vector1 is the less than Vector2.
	/// </summary>
	/// <returns><c>true</c>, if less than was vectored, <c>false</c> otherwise.</returns>
	/// <param name="v1">V1.</param>
	/// <param name="v2">V2.</param>
	bool VectorLessThan(Vector3 v1, Vector3 v2)
	{
		if (v1.sqrMagnitude < v2.sqrMagnitude)
			return true;
		return false;
	}
}
