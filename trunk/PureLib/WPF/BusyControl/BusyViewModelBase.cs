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
            DispatcherFrame frame = new DispatcherFrame();
            T result = default(T);
            try {
                Task<T> task = Task.Run(func).ContinueWith(t => {
                    frame.Continue = false;
                    return t.Result;
                });
                Dispatcher.PushFrame(frame);
                result = task.Result;
            }
            finally {
                IsBusy = false;
            }
            return result;
        }

        public async Task BusyWithAsync(string content, Action action) {
            await BusyWithAsync(content, () => { action(); return 0; });
        }

        public async Task<T> BusyWithAsync<T>(string content, Func<T> func) {
            BusyContent = content;
            IsBusy = true;
            try {
                return await Task.Run(func);
            }
            finally {
                IsBusy = false;
            }
        }
    }
}
