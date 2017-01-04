using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BDMultiTool.Core.Notification;
using BDMultiTool.Fishing;
using BDMultiTool.Market;
using SimpleInjector;
using Color = System.Windows.Media.Color;

namespace BDMultiTool {

    public interface IOverlay
    {
        bool Topmost { get; set; }
        void Update(Rectangle rect);
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
        private readonly Container _container;
//        private readonly IMacroManager _macroManager;
        private bool _menuVisible;
        //private readonly UserControl _settingWindowHost;
        //private readonly UserControl _macrosGalleryWindowHost;
        //private readonly UserControl _macrosWindowHost;
        //private readonly UserControl _fishingWindowHost;
        private readonly UserControl _marketWindowHost;


        public Overlay(INotifier notifier, Container container)
        {
            _notifier = notifier;
            _container = container;
            //_macroManager = macroManager;
            Title = Guid.NewGuid().ToString();
            InitializeComponent();
            Background = null;
            _menuVisible = false;

            var marketWindow = container.GetInstance<IMarketWindow>();

            //_settingWindowHost = AddWindowToGrid((UserControl)settingsWindow, "Settings", true);

            //mainMenu.ItemsSource = toto;
            //var macroGallery = new MacroGallery();
            //macroGallery.initialize();

            //_macrosGalleryWindowHost = AddWindowToGrid(macroGallery, "Macros", false);
            ////_macrosWindowHost = AddWindowToGrid((UserControl)macroManager, "Create new macro", false);
            //_fishingWindowHost = AddWindowToGrid((UserControl)fishingWindow, "Fishing", false);
            _marketWindowHost = AddWindowToGrid((UserControl)marketWindow, "Market", false);
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
            //_settingWindowHost.Dispatcher.Invoke(() =>
            //{
            //    _settingWindowHost.Visibility = Visibility.Visible;
            //}); ;
        }

        private void macrosMenu_Click(object sender, RoutedEventArgs e)
        {
            //_macrosGalleryWindowHost.Dispatcher.Invoke(() =>
            //{
            //    _macrosGalleryWindowHost.Visibility = Visibility.Visible;
            //});

            //_macrosWindowHost.Dispatcher.Invoke(() =>
            //{
            //    _macrosWindowHost.Visibility = Visibility.Visible;
            //});
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

        public void Update(Rectangle rect)
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

        private void FishMenu_OnClick(object sender, RoutedEventArgs e)
        {
            //_fishingWindowHost.Dispatcher.Invoke(() =>
            //{
            //    _fishingWindowHost.Visibility = Visibility.Visible;
            //});
        }

        private void MarketMenu_OnClick(object sender, RoutedEventArgs e)
        {
            _marketWindowHost.Dispatcher.Invoke(() =>
            {
                _marketWindowHost.Visibility = Visibility.Visible;
            });
        }
    }

    public interface IFishingWindow
    {
    }
}
