using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using PureLib.Properties;

namespace PureLib.WPF {
    public abstract class ViewModelBase : NotifyObject {
        private readonly Dispatcher _uiDispatcher;

        public Window View { get; set; }

        public ViewModelBase()
            : this(Dispatcher.CurrentDispatcher) {
        }

        public ViewModelBase(Dispatcher uiDispatcher) {
            _uiDispatcher = uiDispatcher;
        }

        public void RunOnUIThread(Action action) {
            if ((_uiDispatcher == null) || (Thread.CurrentThread == _uiDispatcher.Thread))
                action();
            else
                _uiDispatcher.BeginInvoke(action);
        }
    }
}
