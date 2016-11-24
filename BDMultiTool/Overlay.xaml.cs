using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BDMultiTool.Core.Notification;

namespace BDMultiTool {

    public interface IOverlay
    {
        bool Topmost { get; set; }
        void Update(Rect rect);
        void Show();
        void Hide();
        UserControl AddWindowToGrid(UserControl settingsWindow, string settings, bool noSavingFlagb);
    }

    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window, IOverlay
    {
        private readonly INotifier _notifier;
        private bool _menuVisible;
        private readonly UserControl _settingWindowHost;
        private readonly UserControl _macrosGalleryWindowHost;
        private readonly UserControl _macrosWindowHost;


        public Overlay(ISettingsWindow settingsWindow, INotifier notifier)
        {
            _notifier = notifier;
            Title = Guid.NewGuid().ToString();
            InitializeComponent();
            Background = null;
            _menuVisible = false;

            _settingWindowHost = AddWindowToGrid((UserControl)settingsWindow, "Settings", true);

            //mainMenu.ItemsSource = toto;
            var macroGallery = new MacroGallery();
            macroGallery.initialize();

            _macrosGalleryWindowHost = AddWindowToGrid(macroGallery, "Macros", false);
            _macrosWindowHost = AddWindowToGrid(new MacroAddControl(), "Create new macro", false);
        }

        private void mainMenu_Click(object sender, RoutedEventArgs e)
        {
            if(!_menuVisible) {
                ((Storyboard)FindResource("SlideIn")).Begin(MainMenu);
                _menuVisible = true;
            } else {
                ((Storyboard)FindResource("SlideOut")).Begin(MainMenu);
                _menuVisible = false;
            }
        }

        private void settingsMenu_Click(object sender, RoutedEventArgs e)
        {
            _settingWindowHost.Dispatcher.Invoke(() =>
            {
                _settingWindowHost.Visibility = Visibility.Visible;
            }); ;
        }

        private void macrosMenu_Click(object sender, RoutedEventArgs e)
        {
            _macrosGalleryWindowHost.Dispatcher.Invoke(() =>
            {
                _macrosGalleryWindowHost.Visibility = Visibility.Visible;
            });

            _macrosWindowHost.Dispatcher.Invoke(() =>
            {
                _macrosWindowHost.Visibility = Visibility.Visible;
            });
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            MyApp.exit();
        }

        private void RootGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_menuVisible)
                return;

            ((Storyboard)FindResource("SlideOut")).Begin(MainMenu);
            _menuVisible = false;
        }

        private void customButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(20, 250, 0, 0));
        }

        private void customButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        public void Update(Rect rect)
        {
            Width = rect.Width - 2;
            Height = rect.Height;

            Left = rect.X + 1;
            Top = rect.Y - 1;
        }

        public UserControl AddWindowToGrid(UserControl userControl, string title, bool noSavingFlag)
        {
            var currentInnerWindow = new MovableUserControl(RootGrid);
            currentInnerWindow.setTitle(title);
            currentInnerWindow.setGridContent(userControl);
            currentInnerWindow.Visibility = Visibility.Hidden;
            if (noSavingFlag)
            {
                currentInnerWindow.disableSaving();
            }
            RootGrid.Children.Add(currentInnerWindow);
            return currentInnerWindow;
        }
    }
}
