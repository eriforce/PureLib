using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using PureLib.Properties;

namespace PureLib.WPF {
    public abstract class ViewModelBase : NotifyObject {
        private readonly Dispatcher uiDispatcher;

        public Window View { get; set; }

        public ViewModelBase()
            : this(Dispatcher.CurrentDispatcher) {
        }

        public ViewModelBase(Dispatcher uiDispatcher) {
            this.uiDispatcher = uiDispatcher;
        }

        public void RunOnUIThread(Action code) {
            uiDispatcher.BeginInvoke(code);
        }

        protected void RunInBackground(DoWorkEventHandler work, RunWorkerCompletedEventHandler onCompleted = null) {
            if (uiDispatcher == null)
                work.Invoke(null, new DoWorkEventArgs(null));
            else {
                using (BackgroundWorker worker = new BackgroundWorker()) {
                    worker.DoWork += (s, e) => {
                        work(s, e);
                    };
                    if (onCompleted != null)
                        worker.RunWorkerCompleted += onCompleted;
                    worker.RunWorkerAsync();
                }
            }
        }
    }
}
