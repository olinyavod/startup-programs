using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Commands
{
	public class AsyncCommand<T> : AsyncCommandBase<T>, IAsyncCommand {
		bool allowMultipleExecution = false;
		bool isExecuting = false;
		CancellationTokenSource cancellationTokenSource;
		bool shouldCancel = false;
		internal Task executeTask;
		DispatcherOperation completeTaskOperation;

		public bool AllowMultipleExecution {
			get { return allowMultipleExecution; }
			set { allowMultipleExecution = value; }
		}
		public bool IsExecuting {
			get { return isExecuting; }
			private set {
				if(isExecuting == value) return;
				isExecuting = value;
				RaisePropertyChanged(ViewModelBase.GetPropertyName(() => IsExecuting));
				OnIsExecutingChanged();
			}
		}
		public CancellationTokenSource CancellationTokenSource {
			get { return cancellationTokenSource; }
			private set {
				if(cancellationTokenSource == value) return;
				cancellationTokenSource = value;
				RaisePropertyChanged(ViewModelBase.GetPropertyName(() => CancellationTokenSource));
			}
		}
		[Obsolete("Use the IsCancellationRequested property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldCancel {
			get { return shouldCancel; }
			private set {
				if(shouldCancel == value) return;
				shouldCancel = value;
				RaisePropertyChanged(ViewModelBase.GetPropertyName(() => ShouldCancel));
			}
		}
		public bool IsCancellationRequested {
			get {
				if(CancellationTokenSource == null) return false;
				return CancellationTokenSource.IsCancellationRequested;
			}
		}
		public DelegateCommand CancelCommand { get; private set; }
		ICommand IAsyncCommand.CancelCommand { get { return CancelCommand; } }

		public AsyncCommand(Func<T, Task> executeMethod)
			: this(executeMethod, null, false, null) {
		}
		public AsyncCommand(Func<T, Task> executeMethod, bool useCommandManager)
			: this(executeMethod, null, false, useCommandManager) {
		}
		public AsyncCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod, bool? useCommandManager = null)
			: this(executeMethod, canExecuteMethod, false, useCommandManager) {
		}
		public AsyncCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod, bool allowMultipleExecution, bool? useCommandManager = null)
			: base(executeMethod, canExecuteMethod, useCommandManager) {
			CancelCommand = new DelegateCommand(Cancel, CanCancel, false);
			AllowMultipleExecution = allowMultipleExecution;
		}

		public override bool CanExecute(T parameter) {
			if(!AllowMultipleExecution && IsExecuting) return false;
			return base.CanExecute(parameter);
		}
		public override void Execute(T parameter) {
			if(!CanExecute(parameter))
				return;
			if(executeMethod == null) return;
			IsExecuting = true;
			Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
			CancellationTokenSource = new CancellationTokenSource();
			executeTask = executeMethod(parameter).ContinueWith(x => {
				completeTaskOperation = dispatcher.BeginInvoke(new Action(() => {
					IsExecuting = false;
					ShouldCancel = false;
					completeTaskOperation = null;
				}));
			});
		}
		public void Wait(TimeSpan timeout) {
			if(executeTask == null || !IsExecuting) return;
			executeTask.Wait(timeout);
			completeTaskOperation?.Wait(timeout);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void Cancel() {
			if(!CanCancel()) return;
			ShouldCancel = true;
			CancellationTokenSource.Cancel();
		}
		bool CanCancel() {
			return IsExecuting;
		}
		void OnIsExecutingChanged() {
			CancelCommand.RaiseCanExecuteChanged();
			RaiseCanExecuteChanged();
		}
	}

	public class AsyncCommand : AsyncCommand<object>, IAsyncCommand {
		public AsyncCommand(Func<Task> executeMethod)
			: this(executeMethod, null, false, null) {
		}
		public AsyncCommand(Func<Task> executeMethod, bool useCommandManager)
			: this(executeMethod, null, false, useCommandManager) {
		}
		public AsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod, bool? useCommandManager = null)
			: this(executeMethod, canExecuteMethod, false, useCommandManager) {
		}
		public AsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod, bool allowMultipleExecution, bool? useCommandManager = null)
			: base(
				executeMethod != null ? (Func<object, Task>)(o => executeMethod()) : null,
				canExecuteMethod != null ? (Func<object, bool>)(o => canExecuteMethod()) : null,
				allowMultipleExecution,
				useCommandManager) {
		}
	}
}