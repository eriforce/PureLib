using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using PureLib.Common;
using PureLib.Native;

namespace PureLib.WPF {
    public class SingleInstanceApp : Application, IDisposable {
        private Mutex _singleInstanceMutex;

        public SingleInstanceApp()
            : this(Process.GetCurrentProcess().MainModule.FileName.CreateHash<MD5CryptoServiceProvider>().ToHexString()) {
        }

        public SingleInstanceApp(string mutexName) {
            _singleInstanceMutex = new Mutex(true, mutexName);
        }

        protected virtual Process GetFirstProcess() {
            Process current = Process.GetCurrentProcess();
            return Process.GetProcessesByName(current.ProcessName).Single(
                p => (p.Id != current.Id) && (p.MainModule.FileName == current.MainModule.FileName));
        }

        protected virtual void OnFirstStartup(StartupEventArgs e) {
        }

        protected virtual void OnNextStartup(StartupEventArgs e, Process firstProcess) {
            NativeMethods.SetForegroundWindow(firstProcess.MainWindowHandle);
            Shutdown();
        }

        protected sealed override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            if (_singleInstanceMutex.WaitOne(TimeSpan.Zero, true)) {
                OnFirstStartup(e);
                _singleInstanceMutex.ReleaseMutex();
            }
            else
                OnNextStartup(e, GetFirstProcess());
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing)
                _singleInstanceMutex.Close();
        }
    }
}
