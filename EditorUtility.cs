using UnityEditor;
using UnityEngine;
//using System.IO;

/// <summary>
/// Editor utility: Shubham
/// Setting of app version and Input setting for every platform
/// </summary>
public class EditorUtility : EditorWindow{

	static string appVersion = "2.0.1";
	enum Platform{
	ANDROID, PC, VIVE, OCULUS, HOLOLENS, IOS	
	}
	static Platform buildPlatform;
	static bool overwriteInputManager=false;

	[MenuItem("Shubham/UI/Set Anchors To Corner")]
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

	[MenuItem("Shubham/Set Build Settings for/Android")]
	static void SetAndroidSettings()
	{
		appVersion = UnityEditor.PlayerSettings.bundleVersion;
		buildPlatform = Platform.ANDROID;
		GetWindow<EditorUtility> ("EditWindow");
	}

	[MenuItem("Shubham/Set Build Settings for/PC")]
	static void SetPCSettings()
	{
		appVersion = UnityEditor.PlayerSettings.bundleVersion;
		buildPlatform = Platform.PC;
		GetWindow<EditorUtility> ("EditWindow");
	}

	[MenuItem("Shubham/Set Build Settings for/Vive")]
	static void SetViveSettings()
	{
		appVersion = UnityEditor.PlayerSettings.bundleVersion;
		buildPlatform = Platform.VIVE;
		GetWindow<EditorUtility> ("EditWindow");
	}

	[MenuItem("Shubham/Set Build Settings for/Oculus")]
	static void SetOculusSettings()
	{
		appVersion = UnityEditor.PlayerSettings.bundleVersion;
		buildPlatform = Platform.OCULUS;
		GetWindow<EditorUtility> ("EditWindow");
	}

	[MenuItem("Shubham/Set Build Settings for/Hololens")]
	static void SetHololensSettings()
	{
		appVersion = UnityEditor.PlayerSettings.bundleVersion;
		buildPlatform = Platform.HOLOLENS;
		GetWindow<EditorUtility> ("EditWindow");
	}

	[MenuItem("Shubham/Set Build Settings for/IOS")]
	static void SetIOSSettings()
	{
		appVersion = UnityEditor.PlayerSettings.bundleVersion;
		buildPlatform = Platform.IOS;
		GetWindow<EditorUtility> ("EditWindow");
	}

	void OnGUI()
	{
		EditorGUILayout.LabelField ("Current app version: v." + appVersion, EditorStyles.wordWrappedLabel);
		appVersion = EditorGUI.TextField (new Rect (20, 30, 100, 20), appVersion);
		GUILayout.Space (50f);
		EditorGUILayout.LabelField ("Overwrite InputManager file in project settings");
		#if UNITY_STANDALONE
		overwriteInputManager = EditorGUI.Toggle (new Rect (300, 70, 100, 20), overwriteInputManager);
		#endif
		GUILayout.Space (50f);
		if (GUILayout.Button ("Go Ahead!")) {
			SetPlayerSettings ();
		}
	}

	void SetPlayerSettings()
	{
		string datap = Application.dataPath;
		string inpMgrLoc = datap.Substring (0, datap.LastIndexOf ('/')) + "/ProjectSettings";

		switch (buildPlatform) {
		case Platform.ANDROID:
			UnityEditor.PlayerSettings.companyName = "nfynt";
			UnityEditor.PlayerSettings.productName = "test";
			UnityEditor.PlayerSettings.runInBackground = true;
			//UnityEditor.PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
			UnityEditor.PlayerSettings.SplashScreen.show = false;
			UnityEditor.PlayerSettings.applicationIdentifier = "com.nfynt.test";
			if (appVersion.IndexOf ('.') < 0)
				UnityEditor.PlayerSettings.Android.bundleVersionCode = int.Parse (appVersion);
			else
				UnityEditor.PlayerSettings.Android.bundleVersionCode = int.Parse (appVersion.Substring (0, appVersion.IndexOf ('.')));
			
			UnityEditor.PlayerSettings.bundleVersion = appVersion;
			UnityEditor.PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
			UnityEditor.PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
			UnityEditor.PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
			UnityEditor.PlayerSettings.SetScriptingBackend (BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
			UnityEditor.PlayerSettings.SetApiCompatibilityLevel (BuildTargetGroup.Android, ApiCompatibilityLevel.NET_2_0);
			UnityEditor.PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
			UnityEditor.PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.ForceInternal;
			UnityEditor.PlayerSettings.Android.forceInternetPermission = true;
			UnityEditor.PlayerSettings.Android.forceSDCardPermission = false;
			UnityEditor.PlayerSettings.Android.androidTVCompatibility = false;
			UnityEditor.PlayerSettings.Android.androidIsGame = true;
			UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, "CROSS_PLATFORM_INPUT;MOBILE_INPUT");
			UnityEditor.PlayerSettings.virtualRealitySupported=true;
			Debug.Log("Player settings set for Android");
			break;

		case Platform.PC:
			UnityEditor.PlayerSettings.companyName = "nfynt";
			UnityEditor.PlayerSettings.productName = "test";
			UnityEditor.PlayerSettings.runInBackground = true;
			UnityEditor.PlayerSettings.SplashScreen.show = false;
			UnityEditor.PlayerSettings.applicationIdentifier = "com.nfynt.test";
			UnityEditor.PlayerSettings.bundleVersion = appVersion;
			//UnityEditor.PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
			UnityEditor.PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
			UnityEditor.PlayerSettings.SetScriptingBackend (BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
			UnityEditor.PlayerSettings.SetApiCompatibilityLevel (BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_2_0);
			UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Standalone, "CROSS_PLATFORM_INPUT");
			UnityEditor.PlayerSettings.virtualRealitySupported = false;
			Debug.Log ("Player settings set for PC");
			if (overwriteInputManager) {
				//File.Delete (inpMgrLoc);
				//File.WriteAllBytes (inpMgrLoc, File.ReadAllBytes (datap + "/Editor Default Resources/InputManager_PC.asset"));
				//Debug.Log ("Input Manager replaced successfully");
				UpdateInputManagerForPC();
			}
			break;

		case Platform.HOLOLENS:
			UnityEditor.PlayerSettings.companyName = "nfynt";
			UnityEditor.PlayerSettings.productName = "test";
			UnityEditor.PlayerSettings.runInBackground = true;
			UnityEditor.PlayerSettings.SplashScreen.show = false;
			UnityEditor.PlayerSettings.WSA.packageName = "test";
			//UnityEditor.PlayerSettings.WSA.packageVersion = appVersion;

			UnityEditor.PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
			UnityEditor.PlayerSettings.SetScriptingBackend (BuildTargetGroup.WSA, ScriptingImplementation.Mono2x);
			UnityEditor.PlayerSettings.SetApiCompatibilityLevel (BuildTargetGroup.WSA, ApiCompatibilityLevel.NET_2_0);
			UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.WSA, "MOBILE_INPUT");
			UnityEditor.PlayerSettings.WSA.SetCapability (PlayerSettings.WSACapability.VoipCall, true);
			UnityEditor.PlayerSettings.WSA.SetCapability (PlayerSettings.WSACapability.SpatialPerception, true);
			UnityEditor.PlayerSettings.WSA.SetCapability (PlayerSettings.WSACapability.InternetClient, true);
			UnityEditor.PlayerSettings.WSA.SetCapability (PlayerSettings.WSACapability.InternetClientServer, true);
			UnityEditor.PlayerSettings.WSA.SetCapability (PlayerSettings.WSACapability.MusicLibrary, true);
			UnityEditor.PlayerSettings.WSA.SetCapability (PlayerSettings.WSACapability.PicturesLibrary, true);
			UnityEditor.PlayerSettings.WSA.SetCapability (PlayerSettings.WSACapability.PrivateNetworkClientServer, true);
			UnityEditor.PlayerSettings.WSA.SetCapability (PlayerSettings.WSACapability.VideosLibrary, true);
			UnityEditor.PlayerSettings.WSA.SetCapability (PlayerSettings.WSACapability.WebCam, true);
			UnityEditor.PlayerSettings.virtualRealitySupported = true;
			Debug.Log("Player settings set for Hololens");
			break;

		case Platform.VIVE:
			UnityEditor.PlayerSettings.companyName = "nfynt";
			UnityEditor.PlayerSettings.productName = "test";
			UnityEditor.PlayerSettings.runInBackground = true;
			UnityEditor.PlayerSettings.SplashScreen.show = false;
			UnityEditor.PlayerSettings.applicationIdentifier = "com.nfynt.test";
			UnityEditor.PlayerSettings.bundleVersion = appVersion;
			//UnityEditor.PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
			UnityEditor.PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
			UnityEditor.PlayerSettings.SetScriptingBackend (BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
			UnityEditor.PlayerSettings.SetApiCompatibilityLevel (BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_2_0);
			UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Standalone, "CROSS_PLATFORM_INPUT;VIVE");
			UnityEditor.PlayerSettings.virtualRealitySupported = true;
			Debug.Log("Player settings set for Vive");
			if (overwriteInputManager) {
//				File.Delete (inpMgrLoc);
//				File.WriteAllBytes (inpMgrLoc, File.ReadAllBytes (datap + "/Editor Default Resources/InputManager_VIVE.assets"));
				UpdateInputManagerForVive();
			}
			break;

		case Platform.OCULUS:
			UnityEditor.PlayerSettings.companyName = "nfynt";
			UnityEditor.PlayerSettings.productName = "test";
			UnityEditor.PlayerSettings.runInBackground = true;
			UnityEditor.PlayerSettings.SplashScreen.show = false;
			UnityEditor.PlayerSettings.applicationIdentifier = "com.nfynt.test";
			UnityEditor.PlayerSettings.bundleVersion = appVersion;
			//UnityEditor.PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
			UnityEditor.PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
			UnityEditor.PlayerSettings.SetScriptingBackend (BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
			UnityEditor.PlayerSettings.SetApiCompatibilityLevel (BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_2_0);
			UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Standalone, "CROSS_PLATFORM_INPUT;OCULUS");
			UnityEditor.PlayerSettings.virtualRealitySupported = true;
			Debug.Log ("Player settings set for Oculus");
			if (overwriteInputManager) {
//				File.Delete (inpMgrLoc);
//				File.WriteAllBytes (inpMgrLoc, File.ReadAllBytes (datap + "/Editor Default Resources/InputManager_OCULUS.assets"));
				UpdateInputManagerForOculus();
			}
			break;

		case Platform.IOS:
			UnityEditor.PlayerSettings.companyName = "nfynt";
			UnityEditor.PlayerSettings.productName = "test";
			UnityEditor.PlayerSettings.runInBackground = true;
			//UnityEditor.PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
			UnityEditor.PlayerSettings.SplashScreen.show = false;
			UnityEditor.PlayerSettings.applicationIdentifier = "com.nfynt.test";
			if (appVersion.IndexOf ('.') < 0)
				UnityEditor.PlayerSettings.iOS.buildNumber = appVersion;
			else
				UnityEditor.PlayerSettings.iOS.buildNumber = appVersion.Substring (0, appVersion.IndexOf ('.'));
			
			UnityEditor.PlayerSettings.bundleVersion = appVersion;
			UnityEditor.PlayerSettings.iOS.targetOSVersionString = "9.0";
			UnityEditor.PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
			UnityEditor.PlayerSettings.SetScriptingBackend (BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
			UnityEditor.PlayerSettings.iOS.allowHTTPDownload = true;
			UnityEditor.PlayerSettings.SetArchitecture (BuildTargetGroup.iOS, 2);
			UnityEditor.PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
			UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, "CROSS_PLATFORM_INPUT;MOBILE_INPUT");
			UnityEditor.PlayerSettings.virtualRealitySupported=true;
			Debug.Log("Player settings set for IOS");
			break;
		}

		this.Close ();
	}//end player settings

	void UpdateInputManagerForVive()
	{
		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

		axesProperty.ClearArray ();
		serializedObject.ApplyModifiedProperties();
		//Keyboard Inputs
		AddAxis (new InputAxis () { 
			name = "Horizontal", 
			negativeButton = "left", 
			positiveButton = "right", 
			altNegativeButton = "a", 
			altPositiveButton = "d", 
			gravity = 3f, 
			dead = 0.001f,
			sensitivity = 3f, 
			snap = true,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Vertical", 
			negativeButton = "down", 
			positiveButton = "up", 
			altNegativeButton = "s", 
			altPositiveButton = "w", 
			gravity = 3f, 
			dead = 0.001f,
			sensitivity = 3f, 
			snap = true,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Fire1", 
			positiveButton = "left ctrl", 
			altPositiveButton = "mouse 0",
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Jump", 
			positiveButton = "space", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Submit", 
			positiveButton = "return", 
			altPositiveButton = "joystick button 0", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Submit",  
			positiveButton = "enter", 
			altPositiveButton = "space", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Cancel", 
			positiveButton = "escape", 
			altPositiveButton = "joystick button 1", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		//Mouse Inputs
		AddAxis (new InputAxis () { 
			name = "Mouse X",
			gravity = 0f, 
			dead = 0f,
			sensitivity = 0.1f, 
			snap = false,
			type = AxisType.MouseMovement, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Mouse Y",
			gravity = 0f, 
			dead = 0f,
			sensitivity = 0.1f, 
			snap = false,
			type = AxisType.MouseMovement, 
			axis = 2,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Mouse ScrollWheel",
			gravity = 0f, 
			dead = 0f,
			sensitivity = 0.1f, 
			snap = false,
			type = AxisType.MouseMovement, 
			axis = 3,
			joyNum = 0
		});
		//Joystick Inputs
		AddAxis (new InputAxis () { 
			name = "Horizontal",
			gravity = 0f, 
			dead = 0.19f,
			sensitivity = 1f, 
			snap = false,
			type = AxisType.JoystickAxis, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Horizontal",
			gravity = 0f, 
			dead = 0.19f,
			sensitivity = 1f, 
			snap = false,
			type = AxisType.JoystickAxis, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Vertical",
			gravity = 0f, 
			dead = 0.19f,
			sensitivity = 1f, 
			snap = false,
			invert = true,
			type = AxisType.JoystickAxis, 
			axis = 2,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Fire1", 
			positiveButton = "joystick button 0", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Jump", 
			positiveButton = "joystick button 3", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
	}

	void UpdateInputManagerForOculus()
	{
		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

		axesProperty.ClearArray ();
		serializedObject.ApplyModifiedProperties();
		//Keyboard Inputs
		AddAxis (new InputAxis () { 
			name = "Horizontal", 
			negativeButton = "left", 
			positiveButton = "right", 
			altNegativeButton = "a", 
			altPositiveButton = "d", 
			gravity = 3f, 
			dead = 0.001f,
			sensitivity = 3f, 
			snap = true,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Vertical", 
			negativeButton = "down", 
			positiveButton = "up", 
			altNegativeButton = "s", 
			altPositiveButton = "w", 
			gravity = 3f, 
			dead = 0.001f,
			sensitivity = 3f, 
			snap = true,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Fire1", 
			positiveButton = "left ctrl", 
			altPositiveButton = "mouse 0",
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Jump", 
			positiveButton = "space", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Submit",  
			positiveButton = "return", 
			altPositiveButton = "joystick button 0", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Submit", 
			positiveButton = "enter", 
			altPositiveButton = "space", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Cancel", 
			positiveButton = "escape", 
			altPositiveButton = "joystick button 1", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		//Mouse Inputs
		AddAxis (new InputAxis () { 
			name = "Mouse X",
			gravity = 0f, 
			dead = 0f,
			sensitivity = 0.1f, 
			snap = false,
			type = AxisType.MouseMovement, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Mouse Y",
			gravity = 0f, 
			dead = 0f,
			sensitivity = 0.1f, 
			snap = false,
			type = AxisType.MouseMovement, 
			axis = 2,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Mouse ScrollWheel",
			gravity = 0f, 
			dead = 0f,
			sensitivity = 0.1f, 
			snap = false,
			type = AxisType.MouseMovement, 
			axis = 3,
			joyNum = 0
		});
		//Joystick Inputs
		AddAxis (new InputAxis () { 
			name = "Horizontal",
			gravity = 0f, 
			dead = 0.19f,
			sensitivity = 1f, 
			snap = false,
			type = AxisType.JoystickAxis, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Vertical",
			gravity = 0f, 
			dead = 0.19f,
			sensitivity = 1f, 
			snap = false,
			invert = true,
			type = AxisType.JoystickAxis, 
			axis = 2,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Fire1", 
			positiveButton = "joystick button 0", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Jump", 
			positiveButton = "joystick button 3", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "RightHandIndexFinger", 
			positiveButton = "escape",
			altPositiveButton = "joystick button 1", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.JoystickAxis, 
			axis = 10,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "RightHandMiddleFinger", 
			positiveButton = "escape",
			altPositiveButton = "joystick button 1", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.JoystickAxis, 
			axis = 12,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "MoveForward", 
			positiveButton = "escape",
			altPositiveButton = "joystick button 1", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.JoystickAxis, 
			axis = 5,
			joyNum = 0
		});
	}

	void UpdateInputManagerForPC()
	{
		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

		axesProperty.ClearArray ();
		serializedObject.ApplyModifiedProperties();
		//Keyboard Inputs
		AddAxis (new InputAxis () { 
			name = "Horizontal", 
			negativeButton = "left", 
			positiveButton = "right", 
			altNegativeButton = "a", 
			altPositiveButton = "d", 
			gravity = 3f, 
			dead = 0.001f,
			sensitivity = 3f, 
			snap = true,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Vertical", 
			negativeButton = "down", 
			positiveButton = "up", 
			altNegativeButton = "s", 
			altPositiveButton = "w", 
			gravity = 3f, 
			dead = 0.001f,
			sensitivity = 3f, 
			snap = true,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Fire1", 
			positiveButton = "left ctrl", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Jump", 
			positiveButton = "space", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Submit",  
			positiveButton = "return", 
			altPositiveButton = "joystick button 0", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Submit", 
			positiveButton = "enter", 
			altPositiveButton = "space", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Cancel", 
			positiveButton = "escape", 
			altPositiveButton = "joystick button 1", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		//Mouse Inputs
		AddAxis (new InputAxis () { 
			name = "Mouse X",
			gravity = 0f, 
			dead = 0f,
			sensitivity = 0.1f, 
			snap = false,
			type = AxisType.MouseMovement, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Mouse Y",
			gravity = 0f, 
			dead = 0f,
			sensitivity = 0.1f, 
			snap = false,
			type = AxisType.MouseMovement, 
			axis = 2,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Mouse ScrollWheel",
			gravity = 0f, 
			dead = 0f,
			sensitivity = 0.1f, 
			snap = false,
			type = AxisType.MouseMovement, 
			axis = 3,
			joyNum = 0
		});
		//Joystick Inputs
		AddAxis (new InputAxis () { 
			name = "Horizontal",
			gravity = 0f, 
			dead = 0.19f,
			sensitivity = 1f, 
			snap = false,
			type = AxisType.JoystickAxis, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Horizontal",
			gravity = 0f, 
			dead = 0.19f,
			sensitivity = 1f, 
			snap = false,
			type = AxisType.JoystickAxis, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Vertical",
			gravity = 0f, 
			dead = 0.19f,
			sensitivity = 1f, 
			snap = false,
			invert = true,
			type = AxisType.JoystickAxis, 
			axis = 2,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Fire1", 
			positiveButton = "joystick button 0", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
		AddAxis (new InputAxis () { 
			name = "Jump", 
			positiveButton = "joystick button 3", 
			gravity = 1000f, 
			dead = 0.001f,
			sensitivity = 1000f, 
			snap = false,
			type = AxisType.KeyOrMouseButton, 
			axis = 1,
			joyNum = 0
		});
	}

	private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
	{
		SerializedProperty child = parent.Copy();
		child.Next(true);
		do
		{
			if (child.name == name) return child;
		}
		while (child.Next(false));
		return null;
	}

	private static bool AxisDefined(string axisName)
	{
		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

		axesProperty.Next(true);
		axesProperty.Next(true);
		while (axesProperty.Next(false))
		{
			SerializedProperty axis = axesProperty.Copy();
			axis.Next(true);
			if (axis.stringValue == axisName) return true;
		}
		return false;
	}

	public enum AxisType
	{
		KeyOrMouseButton = 0,
		MouseMovement = 1,
		JoystickAxis = 2
	};

	public class InputAxis
	{
		public string name;
		public string descriptiveName;
		public string descriptiveNegativeName;
		public string negativeButton;
		public string positiveButton;
		public string altNegativeButton;
		public string altPositiveButton;

		public float gravity;
		public float dead;
		public float sensitivity;

		public bool snap = false;
		public bool invert = false;

		public AxisType type;

		public int axis;
		public int joyNum;
	}

	private static void AddAxis(InputAxis axis)
	{
		//if (AxisDefined(axis.name)) return;

		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

		axesProperty.arraySize++;
		serializedObject.ApplyModifiedProperties();

		SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

		GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
		GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
		GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
		GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
		GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
		GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
		GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
		GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
		GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
		GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
		GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
		GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
		GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
		GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
		GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

		serializedObject.ApplyModifiedProperties();
	}

}
