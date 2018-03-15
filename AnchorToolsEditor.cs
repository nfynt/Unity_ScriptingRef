using UnityEditor;
using UnityEngine;

public class UIAnchors {

	[MenuItem("SUI/Set Anchors To Corner")]
	static void AnchorsToCorner()
	{
		foreach (Transform transform in Selection.transforms) {
			RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;

			if (t == null || pt == null) return;

			Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
				t.anchorMin.y + t.offsetMin.y / pt.rect.height);
			Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
				t.anchorMax.y + t.offsetMax.y / pt.rect.height);

			t.anchorMin = newAnchorsMin;
			t.anchorMax = newAnchorsMax;
			t.offsetMin = t.offsetMax = new Vector2(0, 0);
		}
	}
	
	[MenuItem("Nuspace/Build Settings/Android")]
	static void SetAndroidSettings()
	{
		UnityEditor.PlayerSettings.companyName = "Imaginate";
		UnityEditor.PlayerSettings.productName = "Nuspace";
		UnityEditor.PlayerSettings.runInBackground = true;
		UnityEditor.PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
		UnityEditor.PlayerSettings.SplashScreen.show = false;
		UnityEditor.PlayerSettings.applicationIdentifier = "com.imaginate.nuspace";
		UnityEditor.PlayerSettings.bundleVersion = "";
		UnityEditor.PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
		UnityEditor.PlayerSettings.SetScriptingBackend (BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
		UnityEditor.PlayerSettings.SetApiCompatibilityLevel (BuildTargetGroup.Android, ApiCompatibilityLevel.NET_2_0);
		UnityEditor.PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
		UnityEditor.PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.PreferExternal;
		UnityEditor.PlayerSettings.Android.forceSDCardPermission = true;
		UnityEditor.PlayerSettings.Android.androidTVCompatibility = false;
	}

}
