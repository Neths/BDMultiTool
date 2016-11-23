using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;

namespace BDOHelpers
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
            // Create the container as usual.
            var container = new Container();

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
                var app = new App();
                app.Run();
            }
            catch (Exception ex)
            {
                //Log the exception and exit
            }
        }
    }
}
