using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StartUpPrograms.ViewModels;
using TaskScheduler;

namespace StartUpPrograms.Providers
{
	class SchedulerAutoRunFInder : IAutoRunFinder
	{
		private readonly CancellationTokenSource _tokenSource;
		private readonly IProgramItemFactory _factory;
		private Action<string> _onChangedStatus;

		public SchedulerAutoRunFInder(CancellationTokenSource tokenSource, IProgramItemFactory factory)
		{
			_tokenSource = tokenSource;
			_factory = factory;
		}

		public async Task LoadAsync(ICollection<ProgramItemViewModel> collection)
		{
			var scheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
			
			var items = new List<ProgramItemViewModel>();
			await Task.Factory.StartNew(() =>
			{
				var taskScheduler = new TaskScheduler.TaskScheduler();
				taskScheduler.Connect();
				if (taskScheduler.GetFolder(@"\") is ITaskFolder rootFolder)
					LoadFromFolder(rootFolder, items);
			}, _tokenSource.Token, TaskCreationOptions.RunContinuationsAsynchronously, scheduler);
			foreach (var item in items)
			{
				collection.Add(item);
			}


		}

		private void LoadFromFolder(ITaskFolder folder, ICollection<ProgramItemViewModel> collection)
		{
			var items = GetTasks(folder).ToArray();
			foreach (var item in items)
			{
				collection.Add(item);
			}

			foreach (ITaskFolder childFolder in folder.GetFolders(0))
			{
				LoadFromFolder(childFolder, collection);
			}
		}

		private IEnumerable<ProgramItemViewModel> GetTasks(ITaskFolder folder)
		{
			foreach (IRegisteredTask task in folder.GetTasks(0))
			{
				if(_tokenSource.IsCancellationRequested)
					yield break;
				if (task.Enabled)
				{
					foreach (var action in task.Definition.Actions.OfType<IExecAction>())
					{
						var file = Environment.ExpandEnvironmentVariables(action.Path);
						if (File.Exists(file))
							yield return _factory.Create(file, action.Arguments, AutoRunType.Scheduler);
					}
				}
			}
		}

		public void OnChangedStatus(Action<string> onChanged)
		{
			_onChangedStatus = onChanged;
		}
	}
}