using BDMultiTool.Core;
using BDMultiTool.Core.Notification;
using BDMultiTool.Core.PInvoke;
using BDMultiTool.Persistence;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace BDMultiTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class MyApp : Application
    {
        private readonly IWindowAttacher _windowAttacher;
        private readonly INotifier _notifier;
        public const string version = "0.1";
        public static volatile bool appCoreIsInitialized = false;

        public static volatile bool minimized;

        public MyApp(IWindowAttacher windowAttacher, INotifier notifier)
        {
            _windowAttacher = windowAttacher;
            _notifier = notifier;

            if (!Directory.Exists(BDMTConstants.WORKSPACE_NAME)) {
                Directory.CreateDirectory(BDMTConstants.WORKSPACE_NAME);
            }

            if( !File.Exists(BDMTConstants.WORKSPACE_PATH + BDMTConstants.NOTIFICATION_SOUND_FILE)) {
                File.WriteAllBytes(BDMTConstants.WORKSPACE_PATH + BDMTConstants.NOTIFICATION_SOUND_FILE, BDMultiTool.Properties.Resources.notifySound);
            }

            minimized = false;

            _windowAttacher.Attach(WindowAttacher.GetHandleByWindowTitleBeginningWith("Sans titre"));

            appCoreIsInitialized = true;

            letAllComponentsRegister();
        }


        private void letAllComponentsRegister() {
            Type multiToolType = typeof(MultiToolMarkUpThread);
            
            foreach (Assembly currentAssembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Type[] currentType = currentAssembly.GetTypes();

                foreach(Type currentInnerType in currentType) {
                    if (multiToolType.IsAssignableFrom(currentInnerType) && !currentInnerType.IsInterface) {
                        Activator.CreateInstance(currentInnerType);
                    }
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e) {
            CustomNotifyIcon.getInstance();
            _notifier.Notify("Info", "Welcome to BDMT v" + MyApp.version);

            base.OnStartup(e);
        }

        public static void exit() {
            CustomNotifyIcon.dispose();
            PersistenceUnitThread.persistenceUnit.persist();
            Environment.Exit(0);
        }
    }
}
