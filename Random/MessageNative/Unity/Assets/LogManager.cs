using UnityEngine;

namespace Nfynt
{
    public class LogManager : MonoBehaviour
    {
        [SerializeField]
        private DebugMode m_DebugMode = DebugMode.UnityLog;

        public static event MessageLib.DebugLogDelegate onDebugLog = OnDebugLog;
        public static event MessageLib.DebugLogDelegate onDebugErr = OnDebugErr;
        [AOT.MonoPInvokeCallback(typeof(MessageLib.DebugLogDelegate))]
        private static void OnDebugLog(string msg) { Debug.Log(msg); }
        [AOT.MonoPInvokeCallback(typeof(MessageLib.DebugLogDelegate))]
        private static void OnDebugErr(string msg) { Debug.LogError(msg); }

        void Awake()
        {
            MessageLib.SetDebugMode(m_DebugMode);
            MessageLib.LibInitialize();
        }

        void OnApplicationQuit()
        {
            MessageLib.LibFinalize();
        }

        void OnEnable()
        {
            MessageLib.SetLogFunc(onDebugLog);
            MessageLib.SetErrorFunc(onDebugErr);
        }

        void OnDisable()
        {
            MessageLib.SetLogFunc(null);
            MessageLib.SetErrorFunc(null);
        }

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.P))
			{
                GL.IssuePluginEvent(MessageLib.GetRenderEventFunc(), 5);
			}
		}

		void OnGUI()
		{
            GUI.TextArea(new Rect(20, 20, 120, 60), "Press P for logs");
		}
	}
}