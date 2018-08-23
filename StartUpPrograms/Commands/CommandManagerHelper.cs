using System;
using System.Windows.Input;

namespace StartUpPrograms.Commands
{
	class CommandManagerHelper {
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		public static void Subscribe(EventHandler canExecuteChangedHandler) {
			CommandManager.RequerySuggested += canExecuteChangedHandler;
		}
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		public static void Unsubscribe(EventHandler canExecuteChangedHandler) {
			CommandManager.RequerySuggested -= canExecuteChangedHandler;
		}
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		public static void InvalidateRequerySuggested() {
			CommandManager.InvalidateRequerySuggested();
		}
	}
}