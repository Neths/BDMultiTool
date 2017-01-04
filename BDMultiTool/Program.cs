using System;
using BDMultiTool.Core;
using BDMultiTool.Core.Factory;
using BDMultiTool.Core.Notification;
using BDMultiTool.Core.PInvoke;
using BDMultiTool.Engines;
using BDMultiTool.Fishing;
using BDMultiTool.Market;
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
            //container.Register<IOverlay,Overlay>(Lifestyle.Singleton);
            container.Register<ILogger>(() => LogManager.GetLogger("debug"));
            container.Register<IWindowAttacher, WindowAttacher>(Lifestyle.Singleton);
            //container.Register<ISettingsWindow,SettingsWindow>();
            container.Register<INotifier, ToasterNotifier>(Lifestyle.Singleton);
            container.Register<ISoundNotifier, SoundNotification>(Lifestyle.Singleton);
            //container.Register<IMacroManager, MacroManager>();
            container.Register<IInputSender, InputSender>(Lifestyle.Singleton);
            container.Register<IGraphicFactory, GraphicsFactory>(Lifestyle.Singleton);
            container.Register<IScreenHelper, ScreenHelper>(Lifestyle.Singleton);
            //container.Register<IEngine,FishingEngine>();
            container.Register<IEngine, MarketEngine>(Lifestyle.Singleton);
            //container.Register<IFishingWindow,FishingLog>();
            container.Register<IMarketWindow, Market.Market>();
            container.Register<IRegonizeArea, RegonizeEngine>(Lifestyle.Singleton);

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
                var screenHelper = container.GetInstance<IScreenHelper>();

                var app = new MyApp(windowAttacher, notifier, screenHelper, container);
                app.Run();


            }
            catch (Exception ex)
            {
                //Log the exception and exit
            }
        }
    }
}
