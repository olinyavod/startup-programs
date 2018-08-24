using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Win32;
using StartUpPrograms.Commands;
using StartUpPrograms.Providers;

namespace StartUpPrograms.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private Dispatcher _mainDispatcher;

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
			_mainDispatcher = mainDispatcher;
		}

		public MainViewModel()
			:this(Dispatcher.CurrentDispatcher)
		{
			RefreshListCommand = new AsyncCommand(OnRefreshList);
		}

		public AsyncCommand RefreshListCommand { get; }

		private async Task OnRefreshList()
		{
			var factory = new ProgramItemFactory();
			var finders = new IAutoRunFinder[]
			{
				new RegistryAutoRunFinder(RefreshListCommand.CancellationTokenSource, factory),
				new StartMenuAutoRunFinder(RefreshListCommand.CancellationTokenSource, factory), 
			};
			try
			{
				ItemsSource = new ObservableCollection<ProgramItemViewModel>();
				await Task.WhenAll(finders.Select(i =>
				{
					i.OnChangedStatus(s => CurrentStatus = s);
					return i.LoadAsync(ItemsSource);
				}));

				CurrentStatus = Properties.Resources.ScanComplited;
			}
			catch (OperationCanceledException)
			{
				CurrentStatus = Properties.Resources.Canceled;
			}
			catch (Exception ex)
			{
				//TODO: Logging
				CurrentStatus = Properties.Resources.ScanError;
			}
		}

		
	}
}
