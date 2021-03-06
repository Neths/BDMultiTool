﻿using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BDMultiTool.Engines;

namespace BDMultiTool.Fishing
{
    /// <summary>
    /// Interaction logic for FishingLog.xaml
    /// </summary>
    public partial class FishingLog : UserControl, IFishingWindow
    {
        private readonly IEngine _engine;

        public FishingLog(IEngine engine)
        {
            _engine = engine;
            InitializeComponent();
        }

        private void customButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 114, 23, 25));
        }

        private void customButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 249, 60, 64));
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            _engine.Start();
        }
    }
}
