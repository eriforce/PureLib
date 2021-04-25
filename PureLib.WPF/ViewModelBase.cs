using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PureLib.Common;
using PureLib.WPF.Command;

namespace PureLib.WPF {
    public abstract class ViewModelBase : NotifyObject {
        private ICommand _closeCommand;

        protected Dispatcher UiDispatcher { get; private set; }

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
            UiDispatcher = uiDispatcher;
        }

        public void RunOnUIThread(Action action, DispatcherPriority priority = DispatcherPriority.Normal) {
            if (UiDispatcher == null)
                action();
            else
                UiDispatcher.BeginInvoke(action, priority);
        }

        protected virtual RelayCommand GetCloseCommand() {
            return new RelayCommand(p => View?.Close());
        }
    }
}
