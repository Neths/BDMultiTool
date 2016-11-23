using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using BDOHelpers.UserControls;
using Mouse = InputManager.Mouse;

namespace BDOHelpers.Windows {
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window, IOverlay
    {
        private bool menuVisible;

        public Overlay()
        {
            this.Title = Guid.NewGuid().ToString();
            InitializeComponent();
            this.Background = null;
            menuVisible = false;
        }

        public MovableUserControl addWindowToGrid(UserControl userControl, String title, bool noSavingFlag)
        {
            MovableUserControl currentInnerWindow = new MovableUserControl(RootGrid);
            currentInnerWindow.setTitle(title);
            currentInnerWindow.setGridContent(userControl);
            currentInnerWindow.Visibility = Visibility.Hidden;
            if(noSavingFlag) {
                currentInnerWindow.disableSaving();
            }
            RootGrid.Children.Add(currentInnerWindow);
            return currentInnerWindow;
        }

        public MenuItem addMenuItemToMenu(String uri, String header)
        {
            MenuItem currentMenuItem = new MenuItem();
            currentMenuItem.Header = header;
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = new BitmapImage(new Uri(uri));
            currentMenuItem.Icon = image;
            mainMenu.Items.Insert(0, currentMenuItem);
            return currentMenuItem;
        }

        private void mainMenu_Click(object sender, RoutedEventArgs e)
        {
            if(!menuVisible) {
                ((Storyboard)FindResource("SlideIn")).Begin(mainMenu);
                menuVisible = true;
            } else {
                ((Storyboard)FindResource("SlideOut")).Begin(mainMenu);
                menuVisible = false;
            }

        }

        private void settingsMenu_Click(object sender, RoutedEventArgs e)
        {
            //SettingsThread.settings.showSettingsMenu();
            Mouse.MoveRelative(500,500);
            Thread.Sleep(50);
            Mouse.ButtonDown(Mouse.MouseKeys.Right);
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            //App.exit();
        }

        private void RootGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(menuVisible) {
                ((Storyboard)FindResource("SlideOut")).Begin(mainMenu);
                menuVisible = false;
            }
        }

        private void customButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(20, 250, 0, 0));
        }

        private void customButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
        }


        public void UpdateOverlay(Rect rect)
        {
            Width = rect.Width - 2;
            Height = rect.Height;

            Left = rect.X + 1;
            Top = rect.Y - 1;
        }
    }

    public interface IOverlay
    {
        bool Topmost { get; set; }
        void Show();
        void Hide();
        void UpdateOverlay(Rect rect);
    }
}
