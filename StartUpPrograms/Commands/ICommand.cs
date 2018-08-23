using System.Windows.Input;

namespace StartUpPrograms.Commands
{
	public interface ICommand<T> : ICommand {
		void Execute(T param);
		bool CanExecute(T param);
	}
}