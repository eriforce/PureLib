using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace PureLib.WPF {
    public abstract class SingleInstanceApp : Application {
        private static Mutex singleInstanceMutex;

        public SingleInstanceApp(Guid guid) {
            singleInstanceMutex = new Mutex(true, guid.ToString());
        }

        protected abstract void OnFirstStartup(StartupEventArgs e);

        protected abstract void OnNextStartup(StartupEventArgs e);

        protected sealed override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            if (singleInstanceMutex.WaitOne(TimeSpan.Zero, true)) {
                OnFirstStartup(e);
                singleInstanceMutex.ReleaseMutex();
            }
            else
                OnNextStartup(e);
        }
    }
}
