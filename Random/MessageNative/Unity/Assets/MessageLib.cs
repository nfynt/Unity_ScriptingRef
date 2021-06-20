using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Nfynt
{
    public enum DebugMode
    {
        None = 0,
        File,
        UnityLog
    }

    public static class MessageLib
    {
        public const string pluginName = "message";

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DebugLogDelegate(string str);

        [DllImport(pluginName, EntryPoint = "LibInitialize")]
        public static extern void LibInitialize();
        [DllImport(pluginName, EntryPoint = "LibFinalize")]
        public static extern void LibFinalize();
        [DllImport(pluginName, EntryPoint = "SetDebugMode")]
        public static extern void SetDebugMode(DebugMode mode);
        [DllImport(pluginName, EntryPoint = "SetLogFunc")]
        public static extern void SetLogFunc(DebugLogDelegate func);
        [DllImport(pluginName, EntryPoint = "SetWarnFunc")]
        public static extern void SetWarnFunc(DebugLogDelegate func);
        [DllImport(pluginName, EntryPoint = "SetErrorFunc")]
        public static extern void SetErrorFunc(DebugLogDelegate func);

        [DllImport(pluginName)]
        public static extern IntPtr GetRenderEventFunc();
    }
}