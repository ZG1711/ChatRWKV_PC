using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatRWKV_PC.Commands
{
    public class BtnCommand : ICommand
    {
        private Action<object?> action;

        private Func<object?, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public BtnCommand(Action<object?> action, Func<object?, bool>? canExecute = null)
        {
            this.action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null || _canExecute == null)
                return true;
            else 
                return _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            action(parameter);
        }
    }
}
