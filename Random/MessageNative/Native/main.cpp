#pragma once

#include <string>

#include "IUnityInterface.h"
#include "IUnityGraphics.h"

#include "Debug.hh"

// flag to check if this plugin has initialized.
bool m_hasInitialized = false;

static IUnityInterfaces* s_UnityInterfaces = nullptr;
static IUnityGraphics* s_Graphics = nullptr;
//static UnityGfxRenderer s_RendererType = kUnityGfxRendererNull;

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API LibInitialize()
{
    if (m_hasInitialized) return;
    m_hasInitialized = true;

    Debug::Initialize();
}


extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API LibFinalize()
{
    if (!m_hasInitialized) return;
    m_hasInitialized = false;

    Debug::Finalize();
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetDebugMode(Debug::Mode mode)
{
    Debug::SetMode(mode);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetLogFunc(Debug::DebugLogFuncPtr func)
{
    Debug::SetLogFunc(func);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetWarnFunc(Debug::DebugLogFuncPtr func)
{
    Debug::SetWarnFunc(func);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetErrorFunc(Debug::DebugLogFuncPtr func)
{
    Debug::SetErrorFunc(func);
}

void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType event)
{
    switch (event)
    {
    case kUnityGfxDeviceEventInitialize:
    {
        Debug::Log("kUnityGfxDeviceEventInitialize");
        break;
    }
    case kUnityGfxRendererD3D12:
    {
        Debug::Log("kUnityGfxRendererD3D12");
        break;
    }
    case kUnityGfxDeviceEventShutdown:
    {
        Debug::Log("kUnityGfxDeviceEventShutdown");
        break;
    }
    case kUnityGfxDeviceEventBeforeReset:
    {
        Debug::Log("kUnityGfxDeviceEventBeforeReset");
        break;
    }
    case kUnityGfxDeviceEventAfterReset:
    {
        Debug::Log("kUnityGfxDeviceEventAfterReset");
        break;
    }
    };
}

static void UNITY_INTERFACE_API OnRenderEvent(int eventID)
{
    Debug::Log("Graphics event: " + std::to_string(eventID));
}

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc()
{
    return OnRenderEvent;
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces * unityInterfaces)
{
    s_UnityInterfaces = unityInterfaces;
    s_Graphics = unityInterfaces->Get<IUnityGraphics>();

    s_Graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);

    OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
    LibInitialize();
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
    s_Graphics->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
    s_UnityInterfaces = nullptr;
    LibFinalize();
}