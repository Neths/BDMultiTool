using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BDOHelpers.Windows;

namespace BDOHelpers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IOverlay _overlay;

        public App(IOverlay overlay)
        {
            _overlay = overlay;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var windowAttacher = new WindowAttacher(_overlay);
            windowAttacher.Attach(windowAttacher.GetHandleByWindowTitleBeginningWith("Sans titre"));
        }

        public void ExitApplication()
        {
            Environment.Exit(0);
        }
    }
}
