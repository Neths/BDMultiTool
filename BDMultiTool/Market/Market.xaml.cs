using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BDMultiTool.Engines;

namespace BDMultiTool.Market
{
    /// <summary>
    /// Interaction logic for Market.xaml
    /// </summary>
    public partial class Market : UserControl, IMarketWindow
    {
        private readonly IEngine _engine;

        public Market(IEngine engine)
        {
            _engine = engine;
            InitializeComponent();
        }

        private void StartStopButton_OnClick(object sender, RoutedEventArgs e)
        {
            _engine.Start();
        }

        private void customButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 114, 23, 25));
        }

        private void customButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 249, 60, 64));
        }
    }

    public interface IMarketWindow
    {
    }
}
