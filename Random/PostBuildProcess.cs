using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class PostBuildProcess {
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        Debug.Log( pathToBuiltProject );
        File.Copy(sourceFile, Path.Combine(pathToBuiltProject, sourceFileName)); 
        }
}
