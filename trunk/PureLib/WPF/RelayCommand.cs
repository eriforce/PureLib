using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PureLib.WPF {
    public class RelayCommand : ICommand {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        private event EventHandler _canExecuteChanged;

        public event EventHandler CanExecuteChanged {
            add {
                _canExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }
            remove {
                _canExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        public RelayCommand(Action<object> execute)
            : this(execute, null) {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return (_canExecute == null) ? true : _canExecute(parameter);
        }

        public void Execute(object parameter) {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged() {
            if (_canExecute != null && _canExecuteChanged != null)
                _canExecuteChanged(this, EventArgs.Empty);
        }
    }
}
