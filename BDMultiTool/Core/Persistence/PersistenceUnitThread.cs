﻿using BDMultiTool.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BDMultiTool.Persistence {
    class PersistenceUnitThread : MultiToolMarkUpThread {
        public static volatile bool keepWorking;
        public static volatile PersistenceUnit persistenceUnit;

        static PersistenceUnitThread() {
            keepWorking = true;
            persistenceUnit = new PersistenceUnit();
            ThreadManager.registerThread(new Thread(new PersistenceUnitThread().work));
        }

        public PersistenceUnitThread() {

        }

        public void work() {
            while (keepWorking) {
                if (MyApp.appCoreIsInitialized) {
                    persistenceUnit.persist();
                    Thread.Sleep(1000);
                } 

            }
        }
    }
}
