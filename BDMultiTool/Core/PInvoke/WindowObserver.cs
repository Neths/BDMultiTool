using System;
using System.Diagnostics;

namespace BDMultiTool.Core.PInvoke {
    class WindowObserver {
        private static IntPtr windowHandle;
        private static IntPtr eventHook;
        private static Action<User32.WindowEventTypes> callback;
        private static User32.WinEventProc eventListener;

        public WindowObserver(IntPtr windowHandle, Action<User32.WindowEventTypes> callback) {
            WindowObserver.windowHandle = windowHandle;
            WindowObserver.callback = callback;
            eventListener = new User32.WinEventProc(WindowEventCallback);
            int processId;
            int threadId = User32.GetWindowThreadProcessId(windowHandle, out processId);

            eventHook = User32.SetWinEventHook(1, 0x7fffffff, IntPtr.Zero, eventListener, processId, threadId, User32.SetWinEventHookFlags.WINEVENT_OUTOFCONTEXT);
            Debug.WriteLine("hooked to window: " + eventHook);
        }

        private static void WindowEventCallback(IntPtr hWinEventHook, User32.WindowEventTypes iEvent, IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime) {
            if(hWnd == WindowObserver.windowHandle) {
                callback(iEvent);
                //Debug.WriteLine("Event on BDO window: " + iEvent.ToString("X4"));
            }

            //Debug.WriteLine("Event: " + iEvent.ToString("X4"));
        }
    }
}
