using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class NearestPointOnSphereSurface : MonoBehaviour {

    public bool check = false;
    public bool testAng = false;
    public Transform playerTrans;
    public Collider surface;
    public Vector3 closestPoint;

    private void Update()
    {
        if (check)
            CalculatePosition();
        if(testAng)
        {
            Debug.Log(Mathf.Sin(90*Mathf.Deg2Rad));
            Debug.Log(Mathf.Asin(1)*Mathf.Rad2Deg);
            testAng = false;
        }
    }
    public float thetap, phip;
    public float theta, phi, rad;
    Vector3 tempV3;
    public float playerDist;
    
    void CalculatePosition()
    {
        playerDist = Vector3.Distance(playerTrans.position, surface.transform.position);
        if(playerDist==0)
        {
            return;
        }
        theta = Mathf.Rad2Deg * Mathf.Acos((playerTrans.position.z - surface.transform.position.z) / playerDist);
        phi = Mathf.Rad2Deg * Mathf.Acos((playerTrans.position.x - surface.transform.position.x) / (playerDist * Mathf.Sin(theta* Mathf.Deg2Rad)));
        thetap = theta;
        phip = phi;
        rad = surface.bounds.max.y - surface.transform.position.y;
        theta *= Mathf.Deg2Rad;
        phi *= Mathf.Deg2Rad;
        closestPoint = new Vector3(rad * Mathf.Sin(theta) * Mathf.Cos(phi), rad * Mathf.Sin(theta) * Mathf.Sin(phi), rad * Mathf.Cos(theta));
        if (playerTrans.position.y < surface.transform.position.y)
            closestPoint.y = -1 * closestPoint.y;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(closestPoint, 0.5f);
        Gizmos.DrawLine(surface.transform.position, playerTrans.position);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(playerTrans.position, 0.2f);
        Gizmos.DrawLine(surface.transform.position, new Vector3(playerTrans.position.x, 0, playerTrans.position.z));
        Gizmos.DrawLine(playerTrans.position, new Vector3(playerTrans.position.x, 0, playerTrans.position.z));
        //Gizmos.DrawLine(surface.bounds.max, surface.bounds.min);
    }
}
