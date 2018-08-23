using System;

namespace StartUpPrograms.Commands
{
	public abstract class DelegateCommandBase<T> : CommandBase<T> {
		protected Action<T> executeMethod = null;

		void Init(Action<T> executeMethod, Func<T, bool> canExecuteMethod) {
			if(executeMethod == null && canExecuteMethod == null)
				throw new ArgumentNullException("executeMethod");
			this.executeMethod = executeMethod;
			this.CanExecuteMethod = canExecuteMethod;
		}
		public DelegateCommandBase(Action<T> executeMethod)
			: this(executeMethod, null, null) {
		}
		public DelegateCommandBase(Action<T> executeMethod, bool useCommandManager)
			: this(executeMethod, null, useCommandManager) {
		}
		public DelegateCommandBase(Action<T> executeMethod, Func<T, bool> canExecuteMethod, bool? useCommandManager = null)
			: base(useCommandManager) {
			Init(executeMethod, canExecuteMethod);
		}
	}
}