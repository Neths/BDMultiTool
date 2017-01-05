using BDMultiTool.Core;
using BDMultiTool.Core.Notification;
using BDMultiTool.Core.PInvoke;
using BDMultiTool.Persistence;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using BDMultiTool.Core.Hook;
using BDMultiTool.Engines;
using SimpleInjector;
using Application = System.Windows.Application;

namespace BDMultiTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class MyApp : Application
    {
        private readonly IWindowAttacher _windowAttacher;
        private readonly INotifier _notifier;
        private readonly Container _container;
        public const string version = "0.1";
        public static volatile bool appCoreIsInitialized = false;

        public static volatile bool minimized;

        public MyApp(IWindowAttacher windowAttacher, INotifier notifier, IScreenHelper screenHelper, Container container)
        {
            //var d = new DebugWindow(screenHelper);
            //d.Show();

            var t = new VideoTest();
            t.Show();

            _windowAttacher = windowAttacher;
            _notifier = notifier;
            _container = container;

            if (!Directory.Exists(BDMTConstants.WORKSPACE_NAME)) {
                Directory.CreateDirectory(BDMTConstants.WORKSPACE_NAME);
            }

            if( !File.Exists(BDMTConstants.WORKSPACE_PATH + BDMTConstants.NOTIFICATION_SOUND_FILE)) {
                File.WriteAllBytes(BDMTConstants.WORKSPACE_PATH + BDMTConstants.NOTIFICATION_SOUND_FILE, BDMultiTool.Properties.Resources.notifySound);
            }

            minimized = false;

            _windowAttacher.Attach(WindowAttacher.GetHandleByWindowTitleBeginningWith("BLACK DESERT"));

            appCoreIsInitialized = true;

            HookManager.KeyPress += HookManagerOnKeyPress;

            //letAllComponentsRegister();
        }

        private void HookManagerOnKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
            if(keyPressEventArgs.KeyChar != char.Parse("$"))
                return;

            var engines = _container.GetInstance<IEngine>();

            if(engines.Running)
                engines.Stop();
        }

        protected override void OnStartup(StartupEventArgs e) {
            CustomNotifyIcon.getInstance();
            _notifier.Notify("Info", "Welcome to BDMT v" + MyApp.version);

            base.OnStartup(e);
        }

        public static void exit() {
            CustomNotifyIcon.dispose();
            //PersistenceUnitThread.persistenceUnit.persist();
            Environment.Exit(0);
        }
    }
}
