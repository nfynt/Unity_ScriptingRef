using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    private void Update()
    {   
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            Debug.LogError(hit.transform.gameObject.name);
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 28;

        GUI.TextArea(new Rect(10, 10, 200, 100), "FPS: " + ((int)(1f / Time.deltaTime)).ToString(),style);

    }
}
