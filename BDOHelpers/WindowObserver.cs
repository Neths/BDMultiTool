using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BDOHelpers {
    public class WindowObserver : IDisposable
    {
        private readonly IntPtr _windowHandle;
        private readonly IntPtr _eventHook;
        private readonly Action<int> _callback;
        private readonly WinEventProc _eventListener;


        internal enum SetWinEventHookFlags {
            WINEVENT_INCONTEXT = 4,
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_SKIPOWNTHREAD = 1
        }

        public WindowObserver(IntPtr windowHandle, Action<int> callback) {
            _windowHandle = windowHandle;
            _callback = callback;
            _eventListener = new WinEventProc(WindowEventCallback);
            int processId;
            var threadId = GetWindowThreadProcessId(windowHandle, out processId);

            _eventHook = SetWinEventHook(1, 0x7fffffff, IntPtr.Zero, _eventListener, processId, threadId, SetWinEventHookFlags.WINEVENT_OUTOFCONTEXT);
            Debug.WriteLine("hooked to window: " + _eventHook);
        }

        private void WindowEventCallback(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime) {
            if(hWnd == _windowHandle) {
                _callback(iEvent);
                Debug.WriteLine("Event on BDO window: " + iEvent.ToString("X4"));
            }

            Debug.WriteLine("Event: " + iEvent.ToString("X4"));
        }


        [DllImport("USER32.DLL")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        private delegate void WinEventProc(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime);

        [DllImport("USER32.DLL", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, SetWinEventHookFlags dwflags);

        [DllImport("USER32.DLL", SetLastError = true)]
        private static extern int UnhookWinEvent(IntPtr hWinEventHook);

        public void Dispose()
        {
            if (_eventHook != IntPtr.Zero)
            {
                UnhookWinEvent(_eventHook);
            }
        }
    }
}
