using System;
using BDMultiTool.Core;
using BDMultiTool.Core.Factory;
using BDMultiTool.Core.Notification;
using BDMultiTool.Core.PInvoke;
using BDMultiTool.Engines;
using BDMultiTool.Fishing;
using BDMultiTool.Macros;
using NLog;
using NLog.Config;
using NLog.Targets;
using SimpleInjector;

namespace BDMultiTool
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var container = Bootstrap();

            // Any additional other configuration, e.g. of your desired MVVM toolkit.

            RunApplication(container);
        }

        private static Container Bootstrap()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            LogManager.Configuration = config;

            // Create the container as usual.
            var container = new Container();

            //container.Register<IServiceProvider>(() => container);
            container.Register<IOverlay,Overlay>();
            container.Register<ILogger>(() => LogManager.GetLogger("debug"));
            container.Register<IWindowAttacher, WindowAttacher>();
            container.Register<ISettingsWindow,SettingsWindow>();
            container.Register<INotifier, ToasterNotifier>();
            container.Register<ISoundNotifier, SoundNotification>();
            container.Register<IMacroManager, MacroManager>();
            container.Register<IInputSender, InputSender>();
            container.Register<IGraphicFactory, GraphicsFactory>();
            container.Register<IScreenHelper, ScreenHelper>();
            container.Register<IEngine,FishingEngine>();
            container.Register<IFishingWindow,FishingLog>();
            container.Register<IRegonizeArea, RegonizeEngine>();

            // Register your types, for instance:
            //container.Register<IQueryProcessor, QueryProcessor>(Lifestyle.Singleton);
            //container.Register<IUserContext, WpfUserContext>();

            container.Verify();

            return container;
        }

        private static void RunApplication(Container container)
        {
            try
            {
                var windowAttacher = container.GetInstance<IWindowAttacher>();
                var notifier = container.GetInstance<INotifier>();

                var app = new MyApp(windowAttacher, notifier);
                app.Run();
            }
            catch (Exception ex)
            {
                //Log the exception and exit
            }
        }
    }
}
