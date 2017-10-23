using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Bundle/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "F:/Shubham_UnityProjects/AssetBundlesProjects/AssetBundlesPackages/";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.WSAPlayer);
    }
}
