using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Nfynt.UvcCamera
{
    public enum UVCCameraPixelFormat
    {
        PIXEL_FORMAT_RAW = 0,
        PIXEL_FORMAT_YUV = 1,
        PIXEL_FORMAT_RGB565 = 2,
        PIXEL_FORMAT_RGBX = 3,
        PIXEL_FORMAT_YUV420SP = 4,
        PIXEL_FORMAT_NV21 = 5      // = YVU420SemiPlanar
    }

    public class UVCCameraActivityInterface : MonoBehaviour
    {
        public bool DebugLog = true;
        protected AndroidJavaObject _uvcCameraActivity = null;

        [DllImport("uvc_camera_native_layer")]
        protected static extern System.IntPtr GetBuffer();

        // Use this for initialization
        void Awake()
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.nfynt.uvccamera.UVCCameraActivity");
            _uvcCameraActivity = jc.CallStatic<AndroidJavaObject>("GetInstance");
        }

        public void StartPreview(int previewWidth, int previewHeight, UVCCameraPixelFormat pixelFormat)
        {
            if( _uvcCameraActivity != null )
            {
                _uvcCameraActivity.Call("StartPreview", previewWidth, previewHeight, (int)pixelFormat);
            }
        }

        public void Connect(string deviceName)
        {
            if (_uvcCameraActivity != null)
            {
                _uvcCameraActivity.Call("Connect", deviceName );
            }
        }

        public bool GetIsConnected()
        {
            if( _uvcCameraActivity != null )
            {
                if (_uvcCameraActivity.Call<int>("GetIsConnected") == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void ChangePreviewFormat(UVCCameraPixelFormat pixelFormat)
        {
            if( _uvcCameraActivity != null )
            {
                _uvcCameraActivity.Call("ChangePreviewFormat", (int)pixelFormat);
            }
        }

        public bool GetIsPreviewing()
        {
            if (_uvcCameraActivity != null)
            {
                if (_uvcCameraActivity.Call<int>("GetIsPreviewing") == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void Disconnect()
        {
            if (_uvcCameraActivity != null)
            {
                _uvcCameraActivity.Call("Disconnect");
            }
        }

        public string[] GetDeviceList()
        {
            if (_uvcCameraActivity != null)
            {
                var objPtr = _uvcCameraActivity.Call<AndroidJavaObject>("GetDeviceList").GetRawObject();

                if (objPtr != System.IntPtr.Zero)
                    return AndroidJNIHelper.ConvertFromJNIArray<string[]>(objPtr);
            }

            return null;
        }

        public string[] GetSupportedResolutions()
        {
            if( _uvcCameraActivity != null && GetIsConnected())
            {
                var objPtr = _uvcCameraActivity.Call<AndroidJavaObject>("GetSupportedResolutions").GetRawObject();

                if( objPtr != System.IntPtr.Zero)
                    return AndroidJNIHelper.ConvertFromJNIArray<string[]>(objPtr);
            }

            return null;
        }

        public System.IntPtr GetBufferPtr()
        {
            if(_uvcCameraActivity != null)
            {
                return GetBuffer();
            }

            return System.IntPtr.Zero;
        }

        public void UpdateTexture()
        {
            if( _uvcCameraActivity != null )
            {
                _uvcCameraActivity.Call("UpdateTexture");
            }
        }

        public int GetTextureID()
        {
            if (_uvcCameraActivity != null)
            {
                return _uvcCameraActivity.Call<int>("GetTextureID"); 
            }

            return -1;
        }

        public void QueryDevices()
        {
            if( _uvcCameraActivity != null )
            {
                _uvcCameraActivity.Call("QueryDevices");
            }
        }

        public int GetFocus()
        {
            if( _uvcCameraActivity != null )
            {
                return _uvcCameraActivity.Call<int>("GetFocus");
            }

            return -1;
        }

        public void SetExposure(int exposure)
        {
            if( _uvcCameraActivity != null )
            {
                _uvcCameraActivity.Call("SetExposure", exposure);
            }
        }

        public int GetExposure()
        {
            if (_uvcCameraActivity != null)
            {
                return _uvcCameraActivity.Call<int>("GetExposure");
            }

            return -1;
        }

        public void SetExposureMode(int exposureMode)
        {
            if (_uvcCameraActivity != null)
            {
                _uvcCameraActivity.Call("SetExposureMode", exposureMode);
            }
        }

        public int GetExposureMode()
        {
            if (_uvcCameraActivity != null)
            {
                return _uvcCameraActivity.Call<int>("GetExposureMode");
            }

            return -1;
        }

        public void SetGamma(int gamma)
        {
            if (_uvcCameraActivity != null)
            {
                _uvcCameraActivity.Call("SetGamma", gamma);
            }
        }

        public int GetGamma()
        {
            if (_uvcCameraActivity != null)
            {
                return _uvcCameraActivity.Call<int>("GetGamma");
            }

            return 0;
        }

        void Log(string message)
        {
            if( DebugLog )
            {
                Debug.Log("UVCCameraActivityInterface -> " + message);
            }
        }
    }
}
