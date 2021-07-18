// Adapted version of: https://github.com/EnoxSoftware/WebGLFileUploader

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Nfynt
{

    [Serializable]
    public class UploadedFileInfo
    {
        public string name = "";
        public string type = "";
        public long size = 0;
        public int lastModified = 0;
        public string filePath = "";
        public bool isSuccess = false;
        public int errorCode = 0;
    }

    public enum ERROR_CODE : int
    {
        NONE = 0,
        NOT_FOUND_ERR = 1,
        SECURITY_ERR = 2,
        ABORT_ERR = 3,
        NOT_READABLE_ERRF = 4,
        ENCODING_ERR = 5,
        FS_IO_ERRO = 6,
        NOT_ALLOWED_FILENAME = 7
    }

    public enum OS_NAME { iOS, Mac, Android, Windows, ChromeOS, FireFoxOS, UNKNOWN };

    [Serializable]
    class FileUploadResult
    {
        public UploadedFileInfo[] files = new UploadedFileInfo[] { };
    }

    public static class WebGLFileUploadManager
    {
        /// <summary>
        /// Occurs when on file uploaded.
        /// </summary>
        public static event Action<UploadedFileInfo[]> onFileUploaded;

        public static void SetDescription(string description)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLFileUploadManager.Unity_FileUploadManager_SetDescription (description);
#endif
        }

        public static void SetAllowedFileName(string filenameReg)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLFileUploadManager.Unity_FileUploadManager_SetAllowedFileName (filenameReg);
#endif
        }
        public static void Dispose()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLFileUploadManager.Unity_FileUploadManager_Dispose();
#endif
        }


        /// <summary>
        /// Popup the file upload dialog UI.
        /// </summary>
        /// <returns><c>true</c>, if dialog was popuped, <c>false</c> otherwise.</returns>
        /// <param name="titleText">Title text.</param>
        /// <param name="uploadBtnText">Upload button text.</param>
        /// <param name="cancelBtnText">Cancel button text.</param>
        public static bool PopupDialog(string titleText = "", string uploadBtnText = "", string cancelBtnText = "")
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Screen.fullScreen)
            {
                if( Unity_FileUploadManager_IsRunningOnEdgeBrowser() ){
                    Screen.fullScreen = false;
                }else{
                    Unity_FileUploadManager_HideUnityScreenIfHtmlOverlayCant();
                }
            }

            WebGLFileUploadManager.Unity_FileUploadManager_SetCallback(WebGLFileUploadManager.Callback);
            bool success = WebGLFileUploadManager.Unity_FileUploadManager_PopupDialog(titleText, uploadBtnText, cancelBtnText);
            
            return success;
#else
            return false;
#endif
        }

        public static bool IsMobile
        {
            get
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                return Unity_FileUploadManager_IsMobile();
#else
                return false;
#endif
            }
        }

        public static OS_NAME GetOS
        {
            get
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                string osName = Unity_FileUploadManager_GetOS();

#else
                string osName = Enum.GetName(typeof(RuntimePlatform),Application.platform);
#endif
                OS_NAME platform;

                switch ( osName )
                {
                    case "iOS":
                    case "IPhonePlayer":
                        platform = OS_NAME.iOS;

                        break;

                    case "Mac":
                    case "OSXPlayer":
                        platform = OS_NAME.Mac;

                        break;

                    case "Android":
                        platform = OS_NAME.Android;

                        break;

                    case "Windows":
                    case "WindowsPlayer":
                        platform = OS_NAME.Windows;

                        break;

                    case "Chrome OS":
                        platform = OS_NAME.ChromeOS;

                        break;

                    case "FireFox OS":
                        platform = OS_NAME.FireFoxOS;

                        break;

                    default:
                        platform = OS_NAME.UNKNOWN;

                        break;
                }

                return platform;
            }
        }

#if UNITY_WEBGL && !UNITY_EDITOR

        [DllImport ("__Internal")]
        private static extern bool Unity_FileUploadManager_PopupDialog (string title, string uploadBtnText, string cancelBtnText);

        [DllImport ("__Internal")]
        private static extern void Unity_FileUploadManager_SetCallback (Action<string> callback);

        [AOT.MonoPInvokeCallback (typeof(Action<string>))]
        private static void Callback (string fileUploadDataJSON)
        {
            Debug.Log ("Callback called " + fileUploadDataJSON);

            if(onFileUploaded == null) {
                Debug.Log ("onFileUploaded == null");
                return;
            }

            UploadedFileInfo[] files;
            if (!string.IsNullOrEmpty(fileUploadDataJSON)) {
                files = JsonUtility.FromJson<FileUploadResult>(fileUploadDataJSON).files;
                onFileUploaded.Invoke (files);
            } else {
                files = new UploadedFileInfo[0]{};
                onFileUploaded.Invoke (files);
            }
                
            //Debug.Log (onFileUploaded);
        }

        [DllImport ("__Internal")]
        private static extern void Unity_FileUploadManager_SetDescription (string str);

        [DllImport("__Internal")]
        private static extern void Unity_FileUploadManager_Dispose();

        [DllImport ("__Internal")]
        private static extern void Unity_FileUploadManager_SetAllowedFileName (string str);

        [DllImport ("__Internal")]
        private static extern bool Unity_FileUploadManager_IsPopupDialog ();

        [DllImport ("__Internal")]
        private static extern string Unity_FileUploadManager_GetOS ();

        [DllImport ("__Internal")]
        private static extern bool Unity_FileUploadManager_IsMobile ();

        [DllImport ("__Internal")]
        private static extern string Unity_FileUploadManager_GetUserAgent ();

        [DllImport("__Internal")]
        private static extern void Unity_FileUploadManager_HideUnityScreenIfHtmlOverlayCant();

        [DllImport("__Internal")]
        private static extern bool Unity_FileUploadManager_IsRunningOnEdgeBrowser();
#endif

    }

}