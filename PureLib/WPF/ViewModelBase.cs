using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PureLib.Properties;

namespace PureLib.WPF {
    public abstract class ViewModelBase : NotifyObject {
        private ICommand _closeCommand;

        protected Dispatcher UIDispatcher { get; private set; }

        public Window View { get; set; }
        public ICommand CloseCommand {
            get {
                if (_closeCommand == null)
                    _closeCommand = GetCloseCommand();
                return _closeCommand;
            }
        }

        public ViewModelBase()
            : this(Dispatcher.CurrentDispatcher) {
        }

        public ViewModelBase(Dispatcher uiDispatcher) {
            UIDispatcher = uiDispatcher;
        }

        public void RunOnUIThread(Action action, DispatcherPriority priority = DispatcherPriority.Normal) {
            if (UIDispatcher == null)
                action();
            else
                UIDispatcher.BeginInvoke(action, priority);
        }

        protected virtual RelayCommand GetCloseCommand() {
            return new RelayCommand(p => {
                if (View != null)
                    View.Close();
            });
        }
    }
}
