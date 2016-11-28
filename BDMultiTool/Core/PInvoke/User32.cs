using System;
using System.Runtime.InteropServices;

namespace BDMultiTool.Core.PInvoke
{
    public static class User32
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);
        [DllImport("USER32.DLL", SetLastError = true)]
        public static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, SetWinEventHookFlags dwflags);

        public enum SetWinEventHookFlags
        {
            WINEVENT_INCONTEXT = 4,
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_SKIPOWNTHREAD = 1
        }

        public delegate void WinEventProc(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime);

        public enum WindowEventTypes
        {
            EVENT_OBJECT_LOCATIONCHANGE = 0x800B,
            EVENT_SYSTEM_FOREGROUND = 0x0003,
            EVENT_OBJECT_FOCUS = 0x8005,
            EVENT_OBJECT_HIDE = 0x8003,
            EVENT_OBJECT_SHOW = 0x8002
        }
    }
}
