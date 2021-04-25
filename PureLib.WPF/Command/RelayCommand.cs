using System;

namespace PureLib.WPF.Command {
    public class RelayCommand : CommandBase {
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute)
            : this(execute, null) {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(canExecute) {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public override void Execute(object parameter) {
            _execute(parameter);
        }
    }
}
