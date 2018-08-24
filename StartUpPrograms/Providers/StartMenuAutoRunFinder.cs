using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	class StartMenuAutoRunFinder : IAutoRunFinder
	{
		private readonly CancellationTokenSource _tokenSource;
		private readonly IProgramItemFactory _factory;
		private Action<string> _onChanged;

		public StartMenuAutoRunFinder(CancellationTokenSource tokenSource, IProgramItemFactory factory)
		{
			_tokenSource = tokenSource;
			_factory = factory;
		}

		public async Task LoadAsync(ICollection<ProgramItemViewModel> collection)
		{
			var directories = new[]
			{
				Environment.GetFolderPath(Environment.SpecialFolder.Startup),
				Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup)
			};
			var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			

			foreach (var key in directories)
			{
				if (_tokenSource.IsCancellationRequested)
					return;
				_onChanged?.Invoke(string.Format(Properties.Resources.CurrentStatusMessage, key));
				var list = await Task.Factory.StartNew(x => GetFromDirectory(x.ToString()).ToArray(), key, _tokenSource.Token, TaskCreationOptions.RunContinuationsAsynchronously, scheduler);

				foreach (var item in list)
				{
					collection.Add(item);
				}
			}
		}

		private IEnumerable<ProgramItemViewModel> GetFromDirectory(string path)
		{
			foreach (var file in Directory.EnumerateFiles(path)
				.Where(i => Path.GetFileName(i) != "desktop.ini"))
			{
				if (_tokenSource.IsCancellationRequested)
					yield break;

				var filePath = file;
				var arguments = string.Empty;
				if (ShortcutHelper.IsShortcut(filePath))
				{
					var link = ShortcutHelper.GetPathAndArguments(filePath);
					filePath = link.Item1;
					arguments = link.Item2;
				}
				yield return _factory.Create(filePath, arguments, AutoRunType.StartMenu);
			}
		}

		public void OnChangedStatus(Action<string> onChanged)
		{
			_onChanged = onChanged;
		}
	}
}
