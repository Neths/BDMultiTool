using BDMultiTool.Core;
using System.Threading;

namespace BDMultiTool.Macros {
    class MacroManagerThread: MultiToolMarkUpThread{
        private readonly IMacroManager _macroManager;
        public static volatile bool keepWorking;
        //public static volatile MacroManager macroManager;

        public MacroManagerThread(IMacroManager macroManager)
        {
            keepWorking = true;
            _macroManager = macroManager;
            ThreadManager.registerThread(new Thread(work));
        }

        public void work() {
            while(keepWorking) {
                if(MyApp.appCoreIsInitialized && !MyApp.minimized) {
                    _macroManager.update();
                    Thread.Sleep(60);
                } else {
                    Thread.Sleep(300);
                }

            }
        }

    }
}
