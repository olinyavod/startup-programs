using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using StartUpPrograms.Commands;
using StartUpPrograms.Providers;

namespace StartUpPrograms.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private readonly Dispatcher _mainDispatcher;
		private readonly IEnumerable<IAutoRunFinder> _finders;

		public ObservableCollection<ProgramItemViewModel> ItemsSource
		{
			get => GetProperty(() => ItemsSource);
			set => SetProperty(() => ItemsSource, value);
		}

		public string CurrentStatus
		{
			get => GetProperty(() => CurrentStatus);
			set => SetProperty(() => CurrentStatus, value);
		}

		public MainViewModel(Dispatcher mainDispatcher)
		{
			IProgramItemFactory factory = new ProgramItemFactory();
			_finders = new IAutoRunFinder[]
			{
				new RegistryAutoRunFinder(factory),
				new StartMenuAutoRunFinder(factory),
				new SchedulerAutoRunFInder(factory),
			};

			_mainDispatcher = mainDispatcher;

			CancelCommand = new DelegateCommand(OnCancel, OnCanCancel);
			RefreshListCommand = new AsyncCommand(OnRefreshList);
			OpenFolderCommand = new DelegateCommand<ProgramItemViewModel>(OnOpenFolder, OnCanOpenFolder);
		}

		public MainViewModel()
			:this(Dispatcher.CurrentDispatcher)
		{
			
		}

		private void OnChangesStatus(string status)
		{
			_mainDispatcher.BeginInvoke(new Action<string>(s => CurrentStatus = s), status);
		}

		public AsyncCommand RefreshListCommand { get; }

		private async Task OnRefreshList()
		{
			
			try
			{
				ItemsSource = new ObservableCollection<ProgramItemViewModel>();
				await Task.WhenAll(_finders.Select(RunRefresh));
				CurrentStatus = Properties.Resources.ScanComplited;
			}
			catch (OperationCanceledException)
			{
				CurrentStatus = Properties.Resources.Canceled;
			}
			catch (Exception ex)
			{
				//TODO: Logging
				CurrentStatus = $"{Properties.Resources.ScanError} ({ex.Message})";
			}
		}

		private void AddNewItems(IEnumerable<ProgramItemViewModel> newItems)
		{
			_mainDispatcher.BeginInvoke(new Action<IEnumerable<ProgramItemViewModel>>(items =>
				{
					foreach (var i in items)
					{
						ItemsSource.Add(i);
					}
				}),
				newItems);
		}

		private Task RunRefresh(IAutoRunFinder finder)
		{
			var tcs = new TaskCompletionSource<bool>();

			var thread = new Thread(state =>
			{
				try
				{
					var f = (IAutoRunFinder) state;
					var newItems = new List<ProgramItemViewModel>();
					foreach (var item in f.Run(OnChangesStatus))
					{
						newItems.Add(item);
						if (newItems.Count == 5)
						{
							AddNewItems(newItems.ToArray());
							newItems.Clear();
						}
					}

					AddNewItems(newItems);

					tcs.TrySetResult(true);
				}
				catch (OperationCanceledException)
				{
					tcs.TrySetCanceled();
				}
				catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}
			})
			{
				IsBackground = true
			};
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start(finder);

			return tcs.Task;
		}

		public ICommand CancelCommand { get; }

		private bool OnCanCancel()
		{
			return RefreshListCommand.IsExecuting;
		}

		private void OnCancel()
		{
			foreach (var finder in _finders)
			{
				finder.Stop();
			}
		}

		public ICommand OpenFolderCommand { get; }

		private bool OnCanOpenFolder(ProgramItemViewModel item)
		{
			return item != null;
		}

		private void OnOpenFolder(ProgramItemViewModel item)
		{
			item.Open();
		}
	}
}
