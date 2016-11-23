using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using BDOHelpers.Windows;
using InputManager;

namespace BDOHelpers
{
    public class WindowAttacher
    {
        private IntPtr _windowHandle;
        private WindowObserver _windowEventHook;
        private readonly IOverlay _overlay;

        private const uint WM_KEYDOWN = 0x100;
        private const uint WM_KEYUP = 0x101;
        private const uint WM_SETTEXT = 0x000c;

        public WindowAttacher(IOverlay overlayWindow)
        {
            _overlay = overlayWindow;
        }

        private void observedWindowEvent(int windowEvent)
        {
            switch (windowEvent) {
                case WindowEventTypes.EVENT_OBJECT_FOCUS:
                    {
                        _overlay.UpdateOverlay(GetWindowArea());
                        _overlay.Show();
                        //App.minimized = false;
                    }
                    break;
                case WindowEventTypes.EVENT_OBJECT_HIDE:
                    {
                        //App.minimized = true;
                        _overlay.Hide();

                        //ToasterThread.toaster.popToast("Info", "BDMT has been minimized!");
                    }
                    break;
                case WindowEventTypes.EVENT_OBJECT_LOCATIONCHANGE:
                    {
                        _overlay.UpdateOverlay(GetWindowArea());
                    }
                    break;
                case WindowEventTypes.EVENT_OBJECT_SHOW:
                    {

                    }
                    break;
                case WindowEventTypes.EVENT_SYSTEM_FOREGROUND:
                    {

                    }
                    break;
                case 0:
                    {
                        _overlay.Hide();
                    }
                    break;
                default:
                    break;
            }
        }


        public void sendKeypress(System.Windows.Forms.Keys keyToSend) {
            User32.SetForegroundWindow(_windowHandle);
            Keyboard.KeyPress(keyToSend);
            Thread.Sleep(50);
        }

        public void sendKeyDown(System.Windows.Forms.Keys keyToSend) {
            User32.SetForegroundWindow(_windowHandle);
            Keyboard.KeyDown(keyToSend);
            Thread.Sleep(50);
        }

        public void sendKeyUp(System.Windows.Forms.Keys keyToSend) {
            User32.SetForegroundWindow(_windowHandle);
            Keyboard.KeyUp(keyToSend);
            Thread.Sleep(50);
        }

        public Rect GetWindowArea()
        {
            User32.RECT rectStructure;
            User32.GetWindowRect(_windowHandle, out rectStructure);

            return new Rect
            {
                Width = rectStructure.Right - rectStructure.Left,
                Height = rectStructure.Bottom - rectStructure.Top,
                X = rectStructure.Left,
                Y = rectStructure.Top
            };
        }

        public IntPtr GetHandleByWindowTitleBeginningWith(string title)
        {
            var singleOrDefault = Process.GetProcesses().ToList()
                .SingleOrDefault(p => p.MainWindowTitle.StartsWith(title));

            if(singleOrDefault == default (Process))
                return IntPtr.Zero;

            Debug.WriteLine("currentProcessWindow: " + singleOrDefault.MainWindowTitle);
            return singleOrDefault.MainWindowHandle;
        }

        public void Attach(IntPtr windowHandle)
        {
            if (windowHandle.Equals(IntPtr.Zero))
            {
                Debug.WriteLine("could not pinvoke!");
                MessageBox.Show("Make sure BDO isn't initially minimized and this application is running as admin.", "Could not attach to BDO", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(0);
            }

            _windowHandle = windowHandle;
            _windowEventHook = new WindowObserver(windowHandle, observedWindowEvent);
            User32.SetForegroundWindow(windowHandle);

            _overlay.UpdateOverlay(GetWindowArea());
            _overlay.Topmost = true;

            //ToasterThread.toaster.popToast("Info", "Welcome to BDMT v" + App.version);
        }
    }

    public class User32
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
