using UnityEngine;
using System.Collections;

namespace Nfynt
{
    public class AndroidTTS : MonoBehaviour
    {
#if UNITY_ANDROID
        private AndroidJavaObject tts;
        private AndroidJavaObject ttsListener;
        private bool isInitialized = false;

        IEnumerator Start()
        {
            if (Application.platform == RuntimePlatform.Android)
                yield return StartCoroutine(InitTTS());
        }

        IEnumerator InitTTS()
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                ttsListener = new AndroidJavaObject("com.unity3d.player.TTSListener", gameObject.name);
                tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity, ttsListener);
            }

            float timeout = 5f;
            float elapsedTime = 0f;

            while (!isInitialized && elapsedTime < timeout)
            {
                yield return new WaitForSeconds(0.1f);
                elapsedTime += 0.1f;
            }

            if (isInitialized)
            {
                Debug.Log("TTS initialized successfully");
                SetLanguage();
            }
            else
                Debug.LogError("TTS initialization timed out");
        }

        private void SetLanguage()
        {
            if (tts != null)
            {
                AndroidJavaClass localeClass = new AndroidJavaClass("java.util.Locale");
                AndroidJavaObject defaultLocale = localeClass.CallStatic<AndroidJavaObject>("getDefault");
                int result = tts.Call<int>("setLanguage", defaultLocale);
                if (result == -1 || result == -2)
                {
                    Debug.LogError("Failed to set language. Error code: " + result);
                }
            }
        }

        public void Speak(string text)
        {
            if (tts != null && isInitialized)
            {
                AndroidJavaObject hashMap = new AndroidJavaObject("java.util.HashMap");
                int result = tts.Call<int>("speak", text, 0, hashMap);
                if (result == -1)
                {
                    Debug.LogError("TTS speak failed");
                }
            }
            else
            {
                Debug.LogWarning("TTS not initialized.");
            }
        }

        public void Stop()
        {
            if (tts == null || !isInitialized) return;

            tts.Call<int>("stop");
        }

        public void SetPitch(float pitch)
        {
            if (tts == null || !isInitialized) return;
            
            tts.Call<int>("setPitch", pitch);
        }

        public void SetSpeechRate(float rate)
        {
            if (tts == null || !isInitialized) return;
            
            tts.Call<int>("setSpeechRate", rate);
        }

        void OnDestroy()
        {
            if (tts != null)
            {
                tts.Call("shutdown");
            }
        }

        // This method will be called from TTSListener.java
        void OnTTSInitialized(string message)
        {
            isInitialized = (message == "SUCCESS");
            Debug.Log("TTS initialization message: " + message);
        }
#endif
    }
}


/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 

*/