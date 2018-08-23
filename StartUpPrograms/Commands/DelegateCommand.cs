using System;

namespace StartUpPrograms.Commands
{
	public class DelegateCommand<T> : DelegateCommandBase<T> {
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null, null) {
        }
        public DelegateCommand(Action<T> executeMethod, bool useCommandManager)
            : this(executeMethod, null, useCommandManager) {
        }
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool? useCommandManager = null)
            : base(executeMethod, canExecuteMethod, useCommandManager) {
        }
        public override void Execute(T parameter) {
            if(!CanExecute(parameter))
                return;
            if(executeMethod == null) return;
            executeMethod(parameter);
        }
    }

    public class DelegateCommand : DelegateCommand<object> {
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null, null) {
        }
        public DelegateCommand(Action executeMethod, bool useCommandManager)
            : this(executeMethod, null, useCommandManager) {
        }
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod, bool? useCommandManager = null)
            : base(
                executeMethod != null ? (Action<object>)(o => executeMethod()) : null,
                canExecuteMethod != null ? (Func<object, bool>)(o => canExecuteMethod()) : null,
                useCommandManager) {
        }
    }
}
