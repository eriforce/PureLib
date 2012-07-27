using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using PureLib.Native;

namespace PureLib.WPF {
    public class SingleInstanceApp : Application {
        private Mutex singleInstanceMutex;

        public SingleInstanceApp(Guid guid) {
            singleInstanceMutex = new Mutex(true, guid.ToString());
        }

        protected virtual void OnFirstStartup(StartupEventArgs e) {
        }

        protected virtual void OnNextStartup(StartupEventArgs e, Process firstProcess) {
            NativeMethods.SetForegroundWindow(firstProcess.MainWindowHandle);
            Shutdown();
        }

        protected sealed override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            if (singleInstanceMutex.WaitOne(TimeSpan.Zero, true)) {
                OnFirstStartup(e);
                singleInstanceMutex.ReleaseMutex();
            }
            else {
                Process current = Process.GetCurrentProcess();
                OnNextStartup(e, Process.GetProcessesByName(current.ProcessName).Single(p => p.Id != current.Id));
            }
        }
    }
}
