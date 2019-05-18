#!/bin/sh
echo ""
echo "Compiling NativeCode.c..."
$ANDROID_NDK_ROOT/ndk-build.cmd NDK_PROJECT_PATH=. NDK_APPLICATION_MK=Application.mk $*
mv libs/armeabi/libuvc_camera_native_layer.so ..

echo ""
echo "Cleaning up / removing build folders..."  #optional..
rm -rf libs
rm -rf obj

sleep 10

echo ""
echo "Done!"
