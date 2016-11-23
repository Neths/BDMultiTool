using BDMultiTool.Core.Notification;
using BDMultiTool.Persistence;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BDMultiTool
{
    public interface ISettingsWindow
    {
        void ShowSettingsMenu();
    }

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : UserControl, ISettingsWindow
    {
        private readonly INotifier _notifier;

        public SettingsWindow(INotifier notifier)
        {
            _notifier = notifier;
            InitializeComponent();
        }

        private void resetWindowPositionButton_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show("Do you really want to reset the sub window's locations?", "Reset window location", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;
            PersistenceUnitThread.persistenceUnit.deleteByType(typeof(MovableUserControl).Name);
            _notifier.Notify("Info", "All windows have successfully been reset!");
        }

        private void customButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 114, 23, 25));
        }

        private void customButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 249, 60, 64));
        }

        public void ShowSettingsMenu()
        {
            //_ownParentWindow.Dispatcher.Invoke(() => {
            //    _ownParentWindow.Visibility = Visibility.Visible;
            //}); ;
        }
    }
}
