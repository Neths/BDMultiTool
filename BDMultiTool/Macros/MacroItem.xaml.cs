﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BDMultiTool.Macros {
    /// <summary>
    /// Interaction logic for MacroItem.xaml
    /// </summary>
    public partial class MacroItem : UserControl {
        private readonly IMacroManager _macroManager;

        public MacroItem(IMacroManager macroManager)
        {
            _macroManager = macroManager;
            InitializeComponent();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e) {
            _macroManager.removeMacroByName((this.DataContext as MacroItemModel).MacroName);

        }

        private void resetButton_Click(object sender, RoutedEventArgs e) {
            _macroManager.getMacroByName((this.DataContext as MacroItemModel).MacroName).ResetAll();
            (this.DataContext as MacroItemModel).Paused = true;
            (this.DataContext as MacroItemModel).NotPaused = false;
        }

        private void playButton_Click(object sender, RoutedEventArgs e) {
            _macroManager.getMacroByName((this.DataContext as MacroItemModel).MacroName).Resume();
            if(!_macroManager.getMacroByName((this.DataContext as MacroItemModel).MacroName).Paused) {
                (this.DataContext as MacroItemModel).Paused = false;
                (this.DataContext as MacroItemModel).NotPaused = true;
            }

        }

        private void pauseButton_Click(object sender, RoutedEventArgs e) {
            _macroManager.getMacroByName((this.DataContext as MacroItemModel).MacroName).Pause();
            if(_macroManager.getMacroByName((this.DataContext as MacroItemModel).MacroName).Paused) {
                (this.DataContext as MacroItemModel).Paused = true;
                (this.DataContext as MacroItemModel).NotPaused = false;
            }

        }

        private void customButton_MouseEnter(object sender, MouseEventArgs e) {
            ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(80, 230, 230, 230));
        }

        private void customButton_MouseLeave(object sender, MouseEventArgs e) {
            ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void addModeBackground_MouseUp(object sender, MouseButtonEventArgs e) {
            //MacroManagerThread.macroManager.showCreateMacroMenu();
        }

        public void AddModeActive(bool setActive) {
            if(setActive) {
                addModeBackground.Visibility = Visibility.Visible;
                addModeForeground.Visibility = Visibility.Visible;
            } else {
                addModeBackground.Visibility = Visibility.Hidden;
                addModeForeground.Visibility = Visibility.Hidden;
            }
        }
    }
}
