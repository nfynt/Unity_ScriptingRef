using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollballCamera : MonoBehaviour
{
    public Transform CameraTransform;
    
    public float MaxPitch = 80f;
    
    public Vector2 CameraAngle;
    public Vector3 CameraPivot;
    public float MinCameraDistance = 0.01f;
    public float InputMultiplier = 1f;

    float CameraDistance = 10f;
    const float InputMultiplierRatio = 0.01f;

    private bool inputAllowed = true;

    void Start()
    {
        if(CameraTransform==null)
            CameraTransform = transform;

        //raise the camera above ground
        CameraAngle.y = -10f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!inputAllowed)
            return;

        if (Input.GetMouseButton(0))
        {
            CameraAngle.x = Mathf.Repeat(CameraAngle.x + Input.GetAxis("Mouse X") * InputMultiplier, 360f);
            CameraAngle.y = Mathf.Clamp(CameraAngle.y + Input.GetAxis("Mouse Y") * InputMultiplier, -MaxPitch, MaxPitch);
        }

        if (Input.GetMouseButton(2))
        {
            CameraPivot -= CameraTransform.up * ( Input.GetAxis( "Mouse Y" ) * InputMultiplier ) +
                           CameraTransform.right * ( Input.GetAxis( "Mouse X" ) * InputMultiplier );
        }

        UpdateTransform(CameraTransform);
    }

    private void UpdateTransform( Transform cameraTransform )
    {
        CameraDistance = Mathf.Min(
            CameraDistance - Input.mouseScrollDelta.y * InputMultiplier,
            InputMultiplier * (1f / InputMultiplierRatio));

        if (CameraDistance < 0f)
        {
            CameraPivot += cameraTransform.forward * -CameraDistance;
            CameraDistance = 0f;
        }

        cameraTransform.position = CameraPivot +
                                   Quaternion.AngleAxis(CameraAngle.x, Vector3.up) *
                                   Quaternion.AngleAxis(CameraAngle.y, Vector3.right) *
                                   new Vector3(0f, 0f, Mathf.Max(MinCameraDistance, CameraDistance));

        cameraTransform.LookAt(CameraPivot);

    }

    public void AllowInput( bool val )
    {
        inputAllowed = val;
    }

    public void ResetCamera()
    {
        CameraAngle = new Vector2( 0, -10f );
        CameraPivot = Vector3.zero;
        CameraDistance = 10f;
    }
}
