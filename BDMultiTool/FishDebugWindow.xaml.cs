using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Rectangle = System.Drawing.Rectangle;

namespace BDMultiTool
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        private readonly IScreenHelper _screenHelper;
        private static DebugWindow _instance;

        public DebugWindow(IScreenHelper screenHelper)
        {
            _screenHelper = screenHelper;
            InitializeComponent();

            _instance = this;
        }

        public static void SetImageStartFishingRaw(Bitmap img)
        {
            _instance.imgWaitStartFishingRaw.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate {
                _instance.imgWaitStartFishingRaw.Source = BitmapToImageSource(img);
            }));
        }

        public static void SetImageStartFishingFiltered(Bitmap img)
        {
            _instance.imgWaitStartFishingFilter.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate {
                _instance.imgWaitStartFishingFilter.Source = BitmapToImageSource(img);
            }));
        }

        public static void SetImageFishingGameRaw(Bitmap img)
        {
            _instance.imgFishingGameRaw.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate {
                _instance.imgFishingGameRaw.Source = BitmapToImageSource(img);
            }));
        }

        public static void SetImageFishingGameFiltered(Bitmap img)
        {
            _instance.imgFishingGameFilter.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate {
                _instance.imgFishingGameFilter.Source = BitmapToImageSource(img);
            }));
        }

        public static void SetPointStartFishingGame(bool state)
        {
            _instance.ptWaitFishingStart.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                _instance.ptWaitFishingStart.Fill = state
                    ? new SolidColorBrush(Colors.Green)
                    : new SolidColorBrush(Colors.Red);
            }));
        }

        public static void SetPointFishingGame(bool state)
        {
            _instance.ptFishingGame.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                _instance.ptFishingGame.Fill = state
                    ? new SolidColorBrush(Colors.Green)
                    : new SolidColorBrush(Colors.Red);
            }));
        }

        public static void SetPointFishingGauge(bool state)
        {
            _instance.ptFishingGauge.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                _instance.ptFishingGauge.Fill = state
                    ? new SolidColorBrush(Colors.Green)
                    : new SolidColorBrush(Colors.Red);
            }));
        }

        static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var img = _screenHelper.ScreenArea(new Rectangle {X = 800, Y = 130, Width = 400, Height = 300});

            imgWaitStartFishingRaw.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate {
                imgWaitStartFishingRaw.Source = BitmapToImageSource(img);
            }));
        }
    }
}
