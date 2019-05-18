#include <jni.h>
#include <stdlib.h>
#include <android/log.h>

enum PixelFormats
{
	PIXEL_FORMAT_RAW,
	PIXEL_FORMAT_YUV,
	PIXEL_FORMAT_RGB565,
	PIXEL_FORMAT_RGBX,
	PIXEL_FORMAT_YUV420SP,
	PIXEL_FORMAT_NV21      // = YVU420SemiPlanar
};

typedef PixelFormats PixelFormat;
typedef unsigned char byte; //To make life easier

//#define CODEFLOW_DEBUG

static void* Buffer = NULL;

void ProcessBuffer(void* input, const int width, const int height, const size_t size);
void ProcessBufferRGBX(void* input, const int width, const int height, const size_t size);

extern "C" {
	JNIEXPORT void JNICALL Java_com_codeflow_uvccamera_UVCCameraActivity_ProcessBuffer(JNIEnv *, jobject, jobject buf, const int width, const int height, const size_t size, const PixelFormat pixelFormat);
}

JNIEXPORT void JNICALL Java_com_codeflow_uvccamera_UVCCameraActivity_ProcessBuffer(JNIEnv *env, jobject thisObj, jobject buf, const int width, const int height, const size_t size, const PixelFormat pixelFormat)
{
#ifdef CODEFLOW_DEBUG
	__android_log_print(ANDROID_LOG_VERBOSE, "Unity", "processbuffer");
#endif

	if( buf != NULL )
	{
		void* input = env->GetDirectBufferAddress(buf);
		ProcessBuffer(input, width, height, size);
	}
}

void ProcessBuffer(void* input, const int width, const int height, const size_t size)
{

#ifdef CODEFLOW_DEBUG
	__android_log_print(ANDROID_LOG_VERBOSE, "Unity", "ProcessBufferRGBX");
#endif

	if (Buffer == NULL)
	{
		Buffer = new byte[size];
	}

	memcpy(Buffer, input, size);

	// do stuff with buffer here
}

extern "C"
{
	// UNITY
	void* GetBuffer()
	{
#ifdef CODEFLOW_DEBUG
		__android_log_print(ANDROID_LOG_VERBOSE, "Unity", "GetBuffer");
#endif
		return Buffer;
	}
}
