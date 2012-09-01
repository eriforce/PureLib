using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async void BusyWith(string content, Action action) {
            await BusyWithAsync(content, action);
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
            catch {
                throw;
            }
            finally {
                IsBusy = false;
            }
        }
    }
}
