using System;
using System.Diagnostics;
using System.Windows.Input;

namespace PureLib.WPF.Command {
    public abstract class CommandBase : ICommand {
        protected readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public CommandBase(Predicate<object> canExecute) {
            _canExecute = canExecute;
        }

        public abstract void Execute(object parameter);

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) {
            return _canExecute == null || _canExecute(parameter);
        }

        public void RaiseCanExecuteChanged() {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
