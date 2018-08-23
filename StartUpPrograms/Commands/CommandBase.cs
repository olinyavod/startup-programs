using System;
using System.Windows.Input;

namespace StartUpPrograms.Commands
{
	public abstract class CommandBase {
		static bool _defaultUseCommandManager = true;
		public static bool DefaultUseCommandManager { get { return _defaultUseCommandManager; } set { _defaultUseCommandManager = value; } }
	}

	public abstract class CommandBase<T> : CommandBase, ICommand<T> {
		protected Func<T, bool> CanExecuteMethod = null;
		protected bool UseCommandManager;
		event EventHandler _canExecuteChanged;

		public event EventHandler CanExecuteChanged {
			add {
				if(UseCommandManager) {
					CommandManagerHelper.Subscribe(value);
				} else {
					_canExecuteChanged += value;
				}
			}
			remove {
				if(UseCommandManager) {
					CommandManagerHelper.Unsubscribe(value);
				} else {
					_canExecuteChanged -= value;
				}
			}
		}

		protected CommandBase(bool? useCommandManager = null) {
			this.UseCommandManager = useCommandManager ?? DefaultUseCommandManager;
		}

		public virtual bool CanExecute(T parameter) {
			if(CanExecuteMethod == null) return true;
			return CanExecuteMethod(parameter);
		}
		public abstract void Execute(T parameter);

		public void RaiseCanExecuteChanged() {
			if(UseCommandManager) {
				CommandManagerHelper.InvalidateRequerySuggested();
			} else {
				OnCanExecuteChanged();
			}
		}
		protected virtual void OnCanExecuteChanged() {
			if(_canExecuteChanged != null)
				_canExecuteChanged(this, EventArgs.Empty);
		}
		bool ICommand.CanExecute(object parameter) {
			return CanExecute(GetGenericParameter(parameter, true));
		}
		void ICommand.Execute(object parameter) {
			Execute(GetGenericParameter(parameter));
		}
		static T GetGenericParameter(object parameter, bool suppressCastException = false) {
			parameter = (T) parameter;
			if(parameter == null || parameter is T) return (T)parameter;
			if(suppressCastException) return default(T);
			throw new InvalidCastException(string.Format("CommandParameter: Unable to cast object of type '{0}' to type '{1}'", parameter.GetType().FullName, typeof(T).FullName));
		}
	}
}