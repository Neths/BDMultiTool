using BDMultiTool.Core.Notification;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using BDMultiTool.Config;
using SimpleInjector;
using Size = System.Drawing.Size;

namespace BDMultiTool.Core.PInvoke
{
    public class WindowAttacher : IWindowAttacher
    {
        public IntPtr WindowHandle { get; private set; }
        public Size Size { get; private set; }
        public ScreenConfig Config { get; private set; }
        private WindowObserver _windowEventHook;
        private IOverlay _overlay;
        private readonly INotifier _notifier;
        private readonly Container _container;

        public WindowAttacher(INotifier notifier, Container container)
        {
            _notifier = notifier;
            _container = container;
        }

        public void Attach(IntPtr windowHandle)
        {
            if (windowHandle.Equals(IntPtr.Zero))
            {
                //_logger.Error("Make sure BDO isn't initially minimized and this application is running as admin.");
                //Debug.WriteLine("could not pinvoke!");
                MessageBox.Show("Make sure BDO isn't initially minimized and this application is running as admin.", "Could not attach to BDO", MessageBoxButton.OK, MessageBoxImage.Error);
                MyApp.exit();
            }
            else
            {
                WindowHandle = windowHandle;
                var r = GetWindowArea();
                Size = new Size
                {
                    Width = r.Width,
                    Height = r.Height
                };

                Config = ScreenConfig.LoadFromFile($"{Size.Width}x{Size.Height}");

                _overlay = new Overlay(_notifier, _container);

                _windowEventHook = new WindowObserver(windowHandle, ObservedWindowEvent);
                User32.SetForegroundWindow(windowHandle);

                _overlay.Update(r);
                _overlay.Topmost = true;
            }
        }

        private void ObservedWindowEvent(User32.WindowEventTypes windowEvent) {
            switch (windowEvent) {
                case User32.WindowEventTypes.EVENT_OBJECT_FOCUS: {
                        _overlay.Update(GetWindowArea());
                        _overlay.Show();
                        MyApp.minimized = false;
                    }
                    break;
                case User32.WindowEventTypes.EVENT_OBJECT_HIDE: {
                        MyApp.minimized = true;
                        _overlay.Hide();
                        _notifier.Notify("Info", "BDMT has been minimized!");
                    }
                    break;
                case User32.WindowEventTypes.EVENT_OBJECT_LOCATIONCHANGE: {
                        _overlay.Update(GetWindowArea());
                    }
                    break;
                case User32.WindowEventTypes.EVENT_OBJECT_SHOW: {

                    }
                    break;
                case User32.WindowEventTypes.EVENT_SYSTEM_FOREGROUND: {

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

        public static IntPtr GetHandleByWindowTitleBeginningWith(String title) {
            foreach (Process currentProcess in Process.GetProcesses()) {
                if(currentProcess.MainWindowTitle.StartsWith(title)) {
                    Debug.WriteLine("currentProcessWindow: " + currentProcess.MainWindowTitle);
                    return currentProcess.MainWindowHandle;
                }
            }

            return IntPtr.Zero;
        }

        public Rectangle GetWindowArea()
        {
            User32.Rect rectStructure;
            User32.GetWindowRect(WindowHandle, out rectStructure);

            return new Rectangle
            {
                X = rectStructure.Left,
                Y = rectStructure.Top,
                Width = rectStructure.Right - rectStructure.Left,
                Height = rectStructure.Bottom - rectStructure.Top
            };
        }





    }


}
