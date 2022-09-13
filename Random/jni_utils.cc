#include "jni_utils.h"

#include "util/is_arg_null.h"
#include "util/logging.h"
#include "util/check.h"

extern "C"
{
    jint JNI_OnLoad(JavaVM *vm, void *reserved)
    {
        nfynt::jni::SetJavaVm(vm);
        return JNI_VERSION_1_6;
    }
}

namespace nfynt::jni
{
    namespace
    {
        jclass runtime_exception_class_;
        JavaVM* m_cachedVm;
        jobject m_cachedGlobalActivity;
        jobject m_cachedGlobalContext;

        void LoadJNIResources(JNIEnv *env)
        {
            runtime_exception_class_ = nfynt::jni::LoadJClass(env, "java/lang/RuntimeException");
        }

        void ResetGlobalCaches()
        {
            if(m_cachedVm == nullptr) return;

            NFYNT_LOGI("JNI resetting global references");
            JNIEnv* env=nullptr;
            LoadJNIEnv(GetJavaVm(),&env);

            env->DeleteGlobalRef(m_cachedGlobalActivity);
            env->DeleteGlobalRef(m_cachedGlobalContext);
            env->DeleteGlobalRef(runtime_exception_class_);
        }

        void InitializeUnityPlayerActivityAndContext(JavaVM* vm)
        {
            NFYNT_LOGI("Caching unity player activity and context");

            JNIEnv* env=nullptr;
            LoadJNIEnv(vm,&env);

            jclass  unityPlayer = LoadJClass(env,"com/unity3d/player/UnityPlayer");
            CheckExceptionInJava(env);
            jfieldID fid = env->GetStaticFieldID(unityPlayer,"currentActivity","Landroid/app/Activity;");
            CheckExceptionInJava(env);
            jobject activity = env->GetStaticObjectField(unityPlayer,fid);
            CheckExceptionInJava(env);
            jobject context = getJNIContext(env);
            CheckExceptionInJava(env);

            SetAppActivityAndContext(activity,context);
        }

    } // anonymous namespace

    static jobject getJNIContext(JNIEnv *env) {
        CHECK(env!= nullptr);

        jclass activityThreadCls = env->FindClass("android/app/ActivityThread");
        CheckExceptionInJava(env);
        jmethodID currentActivityThread = env->GetStaticMethodID(activityThreadCls,"currentActivityThread", "()Landroid/app/ActivityThread;");
        jobject activityThreadObj = env->CallStaticObjectMethod(activityThreadCls, currentActivityThread);
        CheckExceptionInJava(env);

        jmethodID getApplication = env->GetMethodID(activityThreadCls, "getApplication", "()Landroid/app/Application;");
        jobject context = env->CallObjectMethod(activityThreadObj, getApplication);
        CheckExceptionInJava(env);
        return context;
    }

    void SetJavaVm(JavaVM* vm)
    {
        if(!NFYNT_IS_ARG_NULL(vm)) {
            m_cachedVm = vm;
            InitializeUnityPlayerActivityAndContext(vm);
        }else{
            NFYNT_LOGE("nfynt::jni Nullptr passed for JavaVM*");
        }
    }

    JavaVM* GetJavaVm()
    {
        return m_cachedVm;
    }

    bool CheckExceptionInJava(JNIEnv *env)
    {
        const bool exception_occ = env->ExceptionOccurred();
        if (exception_occ)
        {
            env->ExceptionDescribe();
            env->ExceptionClear();
        }
        return exception_occ;
    }

    void LoadJNIEnv(JavaVM *vm, JNIEnv **env)
    {
        switch (vm->GetEnv(reinterpret_cast<void **>(env), JNI_VERSION_1_6))
        {
        case JNI_OK:
            break;
        case JNI_EDETACHED:
            if (vm->AttachCurrentThread(env, nullptr) != 0)
            {
                *env = nullptr;
            }
            break;
        default:
            *env = nullptr;
            break;
        }
    }

    // Finds java class associated with env
    jclass LoadJClass(JNIEnv *env, const char *class_name)
    {
        jclass local = env->FindClass(class_name);
        CheckExceptionInJava(env);
        return static_cast<jclass>(env->NewGlobalRef(local));
    }

    void ThrowJavaRuntimeException(JNIEnv *env, const char *msg)
    {
        ZAPPAR_LOGE("Throw Java RuntimeException: %s", msg);
        env->ThrowNew(runtime_exception_class_, msg);
    }

    void SetAppActivityAndContext(jobject activity, jobject context)
    {
        JNIEnv* env=nullptr;
        LoadJNIEnv(GetJavaVm(),&env);

        if(!env->IsSameObject(activity,m_cachedGlobalActivity)) {
            env->DeleteGlobalRef(m_cachedGlobalActivity);
            m_cachedGlobalActivity = env->NewGlobalRef(activity);
        }
        if(!env->IsSameObject(context,m_cachedGlobalContext)) {
            env->DeleteGlobalRef(m_cachedGlobalContext);
            m_cachedGlobalContext = env->NewGlobalRef(context);
        }
    }

    jobject GetAppActivity()
    {
        NFYNT_LOGI("nfynt::jni  GetAppActivity");
        CHECK(m_cachedGlobalActivity != NULL);
        return m_cachedGlobalActivity;
    }
    jobject GetAppContext()
    {
        NFYNT_LOGI("nfynt::jni  GetAppContext");
        CHECK(m_cachedGlobalContext != NULL);
        return m_cachedGlobalContext;
    }

    void FreeGlobalRefs()
    {
        ResetGlobalCaches();
    }

} // namespace nfynt::jni
