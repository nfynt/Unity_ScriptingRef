//#define USE_OPENCV

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


using Rect = UnityEngine.Rect;


namespace Nfynt.UvcCamera
{
    [RequireComponent(typeof(UVCCameraActivityInterface))]
    public class UVCCameraActivityInterfaceGUI : MonoBehaviour
    {
        public UVCCameraPixelFormat PixelFormat = UVCCameraPixelFormat.PIXEL_FORMAT_RGBX;
        public int Width = 1280;
        public int Height = 720;

        public RawImage TargetRenderer1;

        protected UVCCameraActivityInterface _interface;
        protected List<string> _devices = new List<string>();
        protected Texture2D _outputTexture = null;

        protected byte[] _byteArray = null;

        protected bool _doTextureConversion = false;

        // Use this for initialization
        void Start()
        {
            _interface = GetComponent<UVCCameraActivityInterface>();

            Query();
            InvokeRepeating("UpdateOutputTexture", 0.1f, 0.1f);
        }

        void Query()
        {
            _devices.Clear();
            var deviceList = _interface.GetDeviceList();

            if(deviceList != null )
            {
                for (int i = 0; i < deviceList.Length; i++)
                {
                    _devices.Add(deviceList[i]);
                }
            }

        }

        void UpdateOutputTexture()
        {
            bool isPreviewing = _interface.GetIsPreviewing();

            if(isPreviewing)
            {
                var bufferPointer = _interface.GetBufferPtr();

                if (bufferPointer != System.IntPtr.Zero)
                {

                    if (_doTextureConversion)
                    {
                        if (_outputTexture == null)
                        {
                            _outputTexture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
							// TargetRenderer1.material.mainTexture = _outputTexture;
							TargetRenderer1.texture = _outputTexture;
                        }

                        _outputTexture.LoadRawTextureData(bufferPointer, Width * Height * 4);
                        _outputTexture.Apply();
                    }
                }
            }
        }

        // Update is called once per frame
        void OnGUI()
        {
            int offset = 20;
            if (GUI.Button(new Rect(10, offset, 250, 90), "Query"))
            {
                Query();
            }
            offset += 120;

            for (int i = 0; i < _devices.Count; i++)
            {
                if (GUI.Button(new Rect(10, offset, 250, 90), _devices[i]))
                {
                    _interface.Connect(
                        deviceName: _devices[i]);
                }

                offset += 120;
            }
			

            if (!_doTextureConversion)
            {
                if (GUI.Button(new Rect(10, offset, 250, 90), "Enable Tex Conv"))
                {
                    _doTextureConversion = true;
                }
                offset += 120;
            }
            else
            {
                if (GUI.Button(new Rect(10, offset, 250, 90), "Disable Tex Conv"))
                {
                    _doTextureConversion = false;
                }
                offset += 120;
            }

            bool isConnected = _interface.GetIsConnected();

            if (isConnected)
            {
                if (GUI.Button(new Rect(10, offset, 250, 90), "Disconnect"))
                {
                    _interface.Disconnect();
                }
                offset += 120;

                bool isPreviewing = _interface.GetIsPreviewing();
                bool changed = false;

                if (GUI.Button(new Rect(10, offset, 250, 90), "StartPreview"))
                {
                    _interface.StartPreview(Width, Height, PixelFormat);
                    changed = true;
                }
                offset += 120;

				//int offset2 = 10;

				//if (GUI.Button(new Rect(220, offset2, 200, 50), UVCCameraPixelFormat.PIXEL_FORMAT_NV21.ToString()))
				//{
				//    PixelFormat = UVCCameraPixelFormat.PIXEL_FORMAT_NV21;
				//    changed = true;
				//}
				//offset2 += 60;

				//if (GUI.Button(new Rect(220, offset2, 200, 50), UVCCameraPixelFormat.PIXEL_FORMAT_RAW.ToString()))
				//{
				//    PixelFormat = UVCCameraPixelFormat.PIXEL_FORMAT_RAW;
				//    changed = true;
				//}
				//offset2 += 60;

				//if (GUI.Button(new Rect(220, offset2, 200, 50), UVCCameraPixelFormat.PIXEL_FORMAT_RGB565.ToString()))
				//{
				//    PixelFormat = UVCCameraPixelFormat.PIXEL_FORMAT_RGB565;
				//    changed = true;
				//}
				//offset2 += 60;

				//if (GUI.Button(new Rect(220, offset2, 200, 50), UVCCameraPixelFormat.PIXEL_FORMAT_RGBX.ToString()))
				//{
				//    PixelFormat = UVCCameraPixelFormat.PIXEL_FORMAT_RGBX;
				//    changed = true;
				//}
				//offset2 += 60;

				//if (GUI.Button(new Rect(220, offset2, 200, 50), UVCCameraPixelFormat.PIXEL_FORMAT_YUV.ToString()))
				//{
				//    PixelFormat = UVCCameraPixelFormat.PIXEL_FORMAT_YUV;
				//    changed = true;
				//}
				//offset2 += 60;

				//if (GUI.Button(new Rect(220, offset2, 200, 50), UVCCameraPixelFormat.PIXEL_FORMAT_YUV420SP.ToString()))
				//{
				//    PixelFormat = UVCCameraPixelFormat.PIXEL_FORMAT_YUV420SP;
				//    changed = true;
				//}
				//offset2 += 60;

				//
				if (isPreviewing && changed)
				{
					_interface.ChangePreviewFormat(PixelFormat);
				}

				//if (!isPreviewing)
				//{ 
				//    // resolutions  
				//    var resolutions = _interface.GetSupportedResolutions();
				//    int offset3 = 10;

				//    if (resolutions != null)
				//    {
				//        foreach (var resolution in resolutions)
				//        {
				//            if (GUI.Button(new Rect(430, offset3, 200, 50), resolution))
				//            {
				//                string[] res = resolution.ToString().Split('x');

				//                Width = int.Parse(res[0]);
				//                Height = int.Parse(res[1]);

				//                if (_outputTexture != null)
				//                {
				//                    _outputTexture = null;
				//                }
				//            }
				//            offset3 += 60;
				//        }
				//    }
				//}
			}
        }
    }

}
