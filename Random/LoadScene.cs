using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

    AssetBundle bundle;

    private void Start()
    {
        bundle = AssetBundle.LoadFromFile("F:/Shubham_UnityProjects/AssetBundlesProjects/AssetBundlesPackages/testnu");
        string[] scenePath = bundle.GetAllScenePaths();
        Debug.Log(scenePath.Length);
        Debug.Log(scenePath[0]); // -> "Assets/scene.unity"
        SceneManager.LoadScene(scenePath[0]);
        //Application.LoadLevel(scenePath[0]);
    }
}
