using BDMultiTool.Core.Notification;
using InputManager;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using NLog;

namespace BDMultiTool.Core.PInvoke
{
    public class WindowAttacher : IWindowAttacher
    {
        private IntPtr _windowHandle;
        private WindowObserver _windowEventHook;
        private readonly IOverlay _overlay;
        private readonly INotifier _notifier;
        private readonly ILogger _logger;
        private const uint WmKeydown = 0x100;
        private const uint WmKeyup = 0x101;
        private const uint WmSettext = 0x000c;

        public WindowAttacher(IOverlay overlay,INotifier notifier,  ILogger logger)
        {
            _logger = logger;
            _overlay = overlay;
            _notifier = notifier;
        }

        public void Attach(IntPtr windowHandle)
        {
            if (windowHandle.Equals(IntPtr.Zero))
            {
                //_logger.Error("Make sure BDO isn't initially minimized and this application is running as admin.");
                //Debug.WriteLine("could not pinvoke!");
                //System.Windows.MessageBox.Show("Make sure BDO isn't initially minimized and this application is running as admin.", "Could not attach to BDO", MessageBoxButton.OK, MessageBoxImage.Error);
                MyApp.exit();
            }
            else
            {
                _windowHandle = windowHandle;

                _windowEventHook = new WindowObserver(windowHandle, ObservedWindowEvent);
                User32.SetForegroundWindow(windowHandle);

                _overlay.Update(GetWindowArea());
                _overlay.Topmost = true;
            }
        }

        private void ObservedWindowEvent(int windowEvent) {
            switch (windowEvent) {
                case WindowEventTypes.EVENT_OBJECT_FOCUS: {
                        _overlay.Update(GetWindowArea());
                        _overlay.Show();
                        MyApp.minimized = false;
                    }
                    break;
                case WindowEventTypes.EVENT_OBJECT_HIDE: {
                        MyApp.minimized = true;
                        _overlay.Hide();
                        _notifier.Notify("Info", "BDMT has been minimized!");
                    }
                    break;
                case WindowEventTypes.EVENT_OBJECT_LOCATIONCHANGE: {
                        _overlay.Update(GetWindowArea());
                    }
                    break;
                case WindowEventTypes.EVENT_OBJECT_SHOW: {

                    }
                    break;
                case WindowEventTypes.EVENT_SYSTEM_FOREGROUND: {

                    }
                    break;
                case 0: {
                        _overlay.Hide();
                    }
                    break;
                default:
                    break;
            }
        }


        public void SendKeypress(System.Windows.Forms.Keys keyToSend) {
            User32.SetForegroundWindow(_windowHandle);
            Keyboard.KeyPress(keyToSend);
            Thread.Sleep(50);
        }

        public void SendKeyDown(System.Windows.Forms.Keys keyToSend) {
            User32.SetForegroundWindow(_windowHandle);
            Keyboard.KeyDown(keyToSend);
            Thread.Sleep(50);
        }

        public void SendKeyUp(System.Windows.Forms.Keys keyToSend) {
            User32.SetForegroundWindow(_windowHandle);
            Keyboard.KeyUp(keyToSend);
            Thread.Sleep(50);
        }

        public static IntPtr GetHandleByWindowTitleBeginningWith(String title) {
            foreach (Process currentProcess in Process.GetProcesses()) {
                if(currentProcess.MainWindowTitle.StartsWith(title)) {
                    Debug.WriteLine("currentProcessWindow: " + currentProcess.MainWindowTitle);
                    return currentProcess.MainWindowHandle;
                }
            }

            return IntPtr.Zero;
        }

        public Rect GetWindowArea()
        {
            User32.Rect rectStructure;
            User32.GetWindowRect(_windowHandle, out rectStructure);

            return new Rect
            {
                X = rectStructure.Left,
                Y = rectStructure.Top,
                Width = rectStructure.Right - rectStructure.Left,
                Height = rectStructure.Bottom - rectStructure.Top
            };
        }





    }


}
