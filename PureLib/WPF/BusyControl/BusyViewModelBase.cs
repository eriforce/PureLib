using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using PureLib.WPF;

namespace PureLib.WPF.BusyControl {
    public abstract class BusyViewModelBase : ViewModelBase {
        private bool _isBusy;
        private string _busyContent;

        public bool IsBusy {
            get { return _isBusy; }
            set {
                _isBusy = value;
                RaiseChange(() => IsBusy);
            }
        }
        public string BusyContent {
            get { return _busyContent; }
            set {
                _busyContent = value;
                RaiseChange(() => BusyContent);
            }
        }

        public void BusyWith(string content, Action action) {
            BusyWith(content, () => { action(); return 0; });
        }

        public T BusyWith<T>(string content, Func<T> func) {
            BusyContent = content;
            IsBusy = true;
            try {
                DispatcherFrame frame = new DispatcherFrame();
                Task<T> task = Task.Run(func).ContinueWith(t => {
                    frame.Continue = false;
                    return t.Result;
                });
                Dispatcher.PushFrame(frame);
                return task.Result;
            }
            finally {
                IsBusy = false;
            }
        }
    }
}
