using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace BDMultiTool.Core.Notification
{
    public interface INotifier
    {
        void Notify(string title, string text);
    }

    public class ToasterNotifier : INotifier
    {
        private readonly ISoundNotifier _soundNotifier;
        private const int BorderOffset = 10;
        private readonly long _showDuration = 5000;
        private bool _notified;
        private readonly Stopwatch _stopwatch;
        private readonly NotificationWindow _notificationWindow;

        public ToasterNotifier(ISoundNotifier soundNotifier)
        {
            _soundNotifier = soundNotifier;
            _notified = false;
            _stopwatch = new Stopwatch();
            _notificationWindow = new NotificationWindow();
            _notificationWindow.Left = SystemParameters.WorkArea.Right - _notificationWindow.Width - BorderOffset;
            _notificationWindow.Top = SystemParameters.WorkArea.Bottom - _notificationWindow.Height - BorderOffset;
            _notificationWindow.Height = 0;
            _notificationWindow.Show();
        }

        public void Update()
        {
            if (!_notified)
                return;

            if (_stopwatch.ElapsedMilliseconds < _showDuration)
                return;

            _notified = false;
            _stopwatch.Reset();
            _notificationWindow.Dispatcher.Invoke(() =>
            {
                ((Storyboard)_notificationWindow.FindResource("SlideOut")).Begin(_notificationWindow);
            });
        }

        public void Notify(string title, string text)
        {
            if (_notified)
            {
                _notificationWindow.Dispatcher.Invoke(() =>
                {
                    ((Storyboard)_notificationWindow.FindResource("SlideOut")).Begin(_notificationWindow);
                });
            }
            _notificationWindow.Dispatcher.Invoke(() =>
            {
                _notificationWindow.notificationTitle.Content = title;
                _notificationWindow.contentTextBox.Text = text;
                ((Storyboard)_notificationWindow.FindResource("SlideIn")).Begin(_notificationWindow);
            });

            _soundNotifier.PlayNotificationSound();

            _notified = true;
            _stopwatch.Restart();
        }
    }
}
