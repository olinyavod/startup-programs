using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace StartUpPrograms.Commands
{
	public abstract class AsyncCommandBase<T> : CommandBase<T>, INotifyPropertyChanged
	{
		protected Func<T, Task> executeMethod = null;

		void Init(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod) {
			if(executeMethod == null && canExecuteMethod == null)
				throw new ArgumentNullException("executeMethod");
			this.executeMethod = executeMethod;
			this.CanExecuteMethod = canExecuteMethod;
		}

		public AsyncCommandBase(Func<T, Task> executeMethod)
			: this(executeMethod, null, null) {
		}
		public AsyncCommandBase(Func<T, Task> executeMethod, bool useCommandManager)
			: this(executeMethod, null, useCommandManager) {
		}
		public AsyncCommandBase(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod, bool? useCommandManager = null)
			: base(useCommandManager) {
			Init(executeMethod, canExecuteMethod);
		}

		event PropertyChangedEventHandler propertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}
		protected void RaisePropertyChanged(string propName) {
			if(propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}