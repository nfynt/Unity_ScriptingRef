package com.nfynt.uvccamera;

import android.graphics.SurfaceTexture;
import android.hardware.usb.UsbConstants;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbInterface;
import android.opengl.GLES20;
import android.os.Bundle;
import android.util.Log;

import com.serenegiant.usb.IFrameCallback;
import com.serenegiant.usb.Size;
import com.serenegiant.usb.USBMonitor;

import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.List;

import com.serenegiant.usb.UVCCamera;
import com.serenegiant.usb.USBMonitor.OnDeviceConnectListener;
import com.serenegiant.usb.USBMonitor.UsbControlBlock;
import com.serenegiant.usb.IStatusCallback;
import com.unity3d.player.UnityPlayerActivity;

public class UVCCameraActivity extends UnityPlayerActivity
{
    protected  static UVCCameraActivity _instance = null;
    public static UVCCameraActivity GetInstance()
    {
        return _instance;
    }

    protected SurfaceTexture mRenderTexture ;
    protected  int mPreviewPixelFormat = UVCCamera.PIXEL_FORMAT_RAW;
    protected  int mPreviewWidth = UVCCamera.DEFAULT_PREVIEW_WIDTH;
    protected  int mPreviewHeight = UVCCamera.DEFAULT_PREVIEW_HEIGHT;
    protected  int mTextureID = -1;
    protected UVCCamera mUVCCamera = null;
    protected USBMonitor mUSBMonitor = null;
    protected OnDeviceConnectListener mOnDeviceConnectListener;
    protected Boolean mConnected = false;
    protected Boolean mIsPreviewing = false;

    //region ---JNI---
    static {
        System.loadLibrary("uvc_camera_native_layer");
    }
    native static void ProcessBuffer (ByteBuffer buffer, int width, int height, int size, int pixelFormat);

    public  UVCCameraActivity()
    {
        Log("Constructor called");
        _instance = this;
    }

    public void QueryDevices()
    {
        Log("QueryDevices");

        if( mUSBMonitor != null )
        {
            List<UsbDevice> devices = mUSBMonitor.getDeviceList();

            Log(devices.size() + " usb devices found!");
            for( UsbDevice device : devices )
            {
                Log("Device found = " + device.getDeviceName());
            }
        }
    }

    public  void ChangePreviewFormat(int pixelFormat)
    {
        mPreviewPixelFormat = pixelFormat;

        if( mUVCCamera != null )
        {
            mUVCCamera.setPreviewSize(mPreviewWidth, mPreviewHeight, mPreviewPixelFormat);
        }
    }

    public  void StartPreview(int width, int height, int pixelformat)
    {
        if( mUVCCamera != null && mConnected )
        {
            //
            mPreviewWidth = width;
            mPreviewHeight = height;
            mPreviewPixelFormat = pixelformat;

            //
            try
            {
                mUVCCamera.setPreviewSize(mPreviewWidth, mPreviewHeight, UVCCamera.DEFAULT_PREVIEW_MODE);
            } catch (final IllegalArgumentException e)
            {
                // fallback to YUV mode
                try
                {
                    Log("Fallback to YUV mode");

                    mPreviewPixelFormat = UVCCamera.DEFAULT_PREVIEW_MODE;
                    mUVCCamera.setPreviewSize(mPreviewWidth, mPreviewHeight, UVCCamera.DEFAULT_PREVIEW_MODE);
                } catch (final IllegalArgumentException e1)
                {
                    mUVCCamera.destroy();
                    mUVCCamera = null;

                    mTextureID = -1;
                    mConnected = false;
                    mIsPreviewing = false;
                }
            }

            if( mUVCCamera != null )
            {
                //
                mUVCCamera.setFrameCallback(new IFrameCallback() {
                    @Override
                    public void onFrame(ByteBuffer frame) {
                        // this is where the magic happens
                        ProcessBuffer(frame, mPreviewWidth, mPreviewHeight, frame.capacity(), mPreviewPixelFormat);
                    }
                }, mPreviewPixelFormat);

                Log("Starting preview " + mPreviewWidth + "x" + mPreviewHeight + " format : " + mPreviewPixelFormat);

                if (mTextureID == -1) {
                    int textures[] = new int[1];
                    GLES20.glGenTextures(1, textures, 0);
                    mTextureID = textures[0];

                    mRenderTexture = new SurfaceTexture(mTextureID);

                    Log("New Texture ID : " + mTextureID);
                }

                mConnected = true;


                mUVCCamera.setPreviewTexture(mRenderTexture);
                mUVCCamera.startPreview();

                //
                mIsPreviewing = true;
            }
        }
    }

    public void Disconnect()
    {
        if( mConnected )
        {
            if( mUVCCamera != null )
            {
                mUVCCamera.destroy();
                mUVCCamera = null;

                //  mRenderTexture.release();
                //  mRenderTexture = null;

                //  mTextureID = -1;
                mConnected = false;
                mIsPreviewing = false;
            }
        }
    }

    public int GetIsPreviewing()
    {
        if( mIsPreviewing )
            return 1;

        return 0;
    }

    public int GetIsConnected()
    {
        if( mConnected )
            return 1;

        return 0;
    }

    public String[] GetSupportedResolutions()
    {
        if( mConnected )
        {
            List<Size> sizes = mUVCCamera.getSupportedSizeList();
            String[] sizesString = new String[sizes.size()];

            int i = 0;
            for(Size size : sizes)
            {
                //     Log("Supported size : " + size.toString() + " found!");
                sizesString[i] = size.width + "x" + size.height;

                i++;
            }

            return sizesString;
        }

        return  null;
    }

    public String[] GetDeviceList()
    {
        Log("GetDeviceList ");

        if( mUSBMonitor != null )
        {
            List<UsbDevice> devices = mUSBMonitor.getDeviceList();
            List<UsbDevice> cameras = new ArrayList<UsbDevice>();

            for(UsbDevice device : devices )
            {
                for( int i = 0; i < device.getInterfaceCount(); i++ )
                {
                    UsbInterface iface = device.getInterface(i);

                    Log("UsbInterface device [" + device.getDeviceName() + "] found with interface [" + iface.getInterfaceClass() + " !" );

                    if(         iface.getInterfaceClass() == UsbConstants.USB_CLASS_VIDEO
                            ||  iface.getInterfaceClass() == UsbConstants.USB_CLASS_AUDIO
                            ||  iface.getInterfaceClass() == UsbConstants.USB_CLASS_MISC)
                    {
                        cameras.add(device);
                        break;
                    }
                }

            }

            String[] cameraList = new String[cameras.size()];

            int i = 0;
            for(UsbDevice camera : cameras )
            {
                cameraList[i] = camera.getDeviceName();
                i++;
            }

            return cameraList;
        }

        return null;
    }

    public void Connect(String deviceName)
    {
        //
        Log("Connect " + deviceName  + " " + mPreviewWidth + "x" + mPreviewHeight + " : " + mPreviewPixelFormat );

        if( mConnected )
        {
            Disconnect();
        }

        if( !mConnected )
        {
            if( mUSBMonitor != null )
            {
                List<UsbDevice> devices = mUSBMonitor.getDeviceList();

                for(int i = 0; i < devices.size() ; i++ )
                {
                    UsbDevice device = devices.get(i);
                    // Log("Checking " + deviceName + " against " + device.getDeviceName() );
                    if(device.getDeviceName().contains(deviceName))
                    {
                        Log("Request permission of device " + deviceName);
                        mUSBMonitor.requestPermission(device);

                        return;
                    }
                }
            }
        }
    }

    public int GetFocus()
    {
        Log("GetFocus called!");

        if( mUVCCamera != null )
        {
            return mUVCCamera.getFocus();
        }

        return  -1;
    }

    public  void SetFocus(int value)
    {
        Log("SetFocus " + value);

        if( mUVCCamera != null )
        {
            mUVCCamera.setFocus(value);
        }
    }

    protected void onCreate(Bundle savedInstanceState)
    {
        //
        Log("onCreate called!");

        // call UnityPlayerActivity.onCreate()
        super.onCreate(savedInstanceState);

        //
        mUSBMonitor = new USBMonitor(this, mDeviceConnectListener);
        mUSBMonitor.register();
    }

    public void Log(String log)
    {
        Log.d("Unity : ", "UVCCameraInterface -> " + log);
    }

    //
    protected OnDeviceConnectListener mDeviceConnectListener = new OnDeviceConnectListener()
    {
        @Override
        public void onAttach(final UsbDevice device)
        {
            Log("onAttach");

            if( device != null )
            {
                Log( "USB_DEVICE_ATTACHED " +device.getDeviceName());
            }
        }


        @Override
        public void onConnect(final UsbDevice device, final UsbControlBlock ctrlBlock, final boolean createNew)
        {
            Log("onConnect");

            if( device != null )
            {
                if (mUVCCamera != null)
                    mUVCCamera.destroy();

                mUVCCamera = new UVCCamera();

                runOnUiThread(new Runnable()
                {
                    @Override
                    public void run()
                    {
                        mUVCCamera.open(ctrlBlock);
                        mUVCCamera.setStatusCallback(new IStatusCallback() {
                            @Override
                            public void onStatus(final int statusClass, final int event, final int selector,
                                                 final int statusAttribute, final ByteBuffer data) {
                                new Runnable() {
                                    @Override
                                    public void run() {
                                    }
                                };
                            }
                        });
                        if (mRenderTexture != null) {
                            //    mRenderTexture.release();
                            //   mRenderTexture = null;
                        }

                        Log("Connected!");
                        mConnected = true;
                    }
                });
            }
        }


        @Override
        public void onDisconnect(final UsbDevice device, final UsbControlBlock ctrlBlock)
        {
            Log("onDisconnect");

            if( device != null )
            {
                // XXX you should check whether the coming device equal to camera device that currently using
                if (mUVCCamera != null)
                {
                    mUVCCamera.close();
                    if (mRenderTexture  != null)
                    {
                        //    mRenderTexture .release();
                        //    mRenderTexture  = null;
                    }

                    //        mTextureID = -1;
                    mConnected = false;
                    mIsPreviewing = false;
                }
            }

        }

        @Override
        public void onDettach(final UsbDevice device)
        {
            Log("onDettach");

            if( device != null )
            {
                Log( "USB_DEVICE_DETACHED : " + device.getDeviceName());
            }
        }

        @Override
        public void onCancel()
        {
            Log("onCancel");
        }
    };

    public  void SetExposure(int value)
    {
        if( mUVCCamera != null )
        {
            mUVCCamera.setExposure(value);
        }
    }

    public  int GetExposure()
    {
        if( mUVCCamera != null )
        {
            return  mUVCCamera.getExposure();
        }

        return 0;
    }

    public int GetExposureMode()
    {
        if( mUVCCamera != null )
        {
            return  mUVCCamera.getExposureMode();
        }

        return 0;
    }

    public void SetExposureMode(int value)
    {
        if( mUVCCamera != null )
        {
            mUVCCamera.setExposureMode(value);
        }
    }

    public  void SetGamma(int value)
    {
        if( mUVCCamera != null )
        {
            mUVCCamera.setGamma(value);
        }
    }

    public  int GetGamma()
    {
        if( mUVCCamera != null )
        {
            return  mUVCCamera.getGamma();
        }

        return 0;
    }
}
