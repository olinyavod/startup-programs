using System.Windows.Input;

namespace StartUpPrograms.Commands
{
	public interface IAsyncCommand : ICommand
	{
		ICommand CancelCommand { get; }
	}
}