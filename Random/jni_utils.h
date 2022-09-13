#ifndef NFYNT_SDK_JNI_UTILS_H_
#define NFYNT_SDK_JNI_UTILS_H_

#include <jni.h>

namespace nfynt::jni {

    void SetJavaVm(JavaVM* vm);
    // JavaVM* captured from JNI_OnLoad
    JavaVM* GetJavaVm();

    // Init Java class reference used by this module, using vm that's captued from JNI_OnLoad
    //void InitializeAndroid(JavaVM* vm, jobject ctx);
    void SetAppActivityAndContext(jobject activity, jobject context);
    
    jobject GetAppActivity();
    
    jobject GetAppContext();
    
    // Log Java exception and clear JNI flag in case of exception
    bool CheckExceptionInJava(JNIEnv* env);

    //Retrieves JNI environment. Return val: JNI_OK, JNI_DETACHED, or other in case of failure.
    void LoadJNIEnv(JavaVM* vm, JNIEnv** env);
    
    // Loads a class by it's class_name as a global reference
    jclass LoadJClass(JNIEnv* env, const char* class_name);
    
    static jobject getJNIContext(JNIEnv *env);
    
    // Throws runtime exception
    void ThrowJavaRuntimeException(JNIEnv* env, const char* msg);
    
    void FreeGlobalRefs();
}

#endif //ZAPPAR_SDK_JNI_UTILS_H_
