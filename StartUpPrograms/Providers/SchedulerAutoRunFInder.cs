using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using StartUpPrograms.ViewModels;
using TaskScheduler;

namespace StartUpPrograms.Providers
{
	class SchedulerAutoRunFInder : IAutoRunFinder
	{
		private readonly IProgramItemFactory _factory;
		private Action<string> _onChangedStatus;
		private bool _isStoped;

		public SchedulerAutoRunFInder(IProgramItemFactory factory)
		{
			_factory = factory;
		}

		public IEnumerable<ProgramItemViewModel> Run()
		{
			_isStoped = false;
			var taskScheduler = new TaskScheduler.TaskScheduler();
				taskScheduler.Connect();
				if (taskScheduler.GetFolder(@"\") is ITaskFolder rootFolder)
					foreach (var item in LoadFromFolder(rootFolder))
					{
						if(_isStoped)
							throw new OperationCanceledException();

						yield return item;
					}
		}

		public void Stop()
		{
			_isStoped = true;
		}

		private IEnumerable<ProgramItemViewModel> LoadFromFolder(ITaskFolder folder)
		{
			_onChangedStatus?.Invoke(string.Format(Properties.Resources.CurrentStatusMessage, folder.Path));
			foreach (var item in GetTasks(folder))
			{
				if (_isStoped)
					throw new OperationCanceledException();

				yield return item;
			}

			foreach (ITaskFolder childFolder in folder.GetFolders(0))
			{
				foreach (var item in LoadFromFolder(childFolder))
				{
					if (_isStoped)
						throw new OperationCanceledException();
					yield return item;
				}
			}
		}

		private IEnumerable<ProgramItemViewModel> GetTasks(ITaskFolder folder)
		{
			foreach (IRegisteredTask task in folder.GetTasks(0))
			{
				if(_isStoped)
					throw new OperationCanceledException();
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