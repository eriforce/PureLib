using System;
using System.Threading.Tasks;

namespace PureLib.WPF.Command {
    public class AsyncRelayCommand : CommandBase {
        private readonly Func<object, Task> _execute;

        public AsyncRelayCommand(Func<object, Task> execute)
            : this(execute, null) {
        }

        public AsyncRelayCommand(Func<object, Task> execute, Predicate<object> canExecute)
            : base(canExecute) {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public override async void Execute(object parameter) {
            await _execute(parameter);
        }
    }
}
