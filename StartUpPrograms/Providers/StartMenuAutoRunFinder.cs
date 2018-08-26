using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	class StartMenuAutoRunFinder : IAutoRunFinder
	{
		private readonly IProgramItemFactory _factory;
		private bool _isStoped;

		public StartMenuAutoRunFinder(IProgramItemFactory factory)
		{
			_factory = factory;
		}

		public IEnumerable<ProgramItemViewModel> Run(Action<string> onChanged)
		{
			_isStoped = false;
			var directories = new[]
			{
				Environment.GetFolderPath(Environment.SpecialFolder.Startup),
				Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup)
			};
			foreach (var key in directories)
			{
				if (_isStoped)
					throw new OperationCanceledException();

				onChanged?.Invoke(string.Format(Properties.Resources.CurrentStatusMessage, key));
				
				foreach (var item in GetFromDirectory(key))
				{
					if(_isStoped)
						throw new OperationCanceledException();
					yield return item;
				}
			}
		}

		public void Stop()
		{
			_isStoped = true;
		}

		private IEnumerable<ProgramItemViewModel> GetFromDirectory(string path)
		{
			foreach (var file in Directory.EnumerateFiles(path)
				.Where(i => Path.GetFileName(i) != "desktop.ini"))
			{
				if (_isStoped)
					throw new OperationCanceledException();

				var filePath = file;
				var arguments = string.Empty;
				if (ShellHelper.IsShortcut(filePath))
				{
					var link = ShellHelper.GetPathAndArguments(filePath);
					filePath = link.Item1;
					arguments = link.Item2;
				}
				yield return _factory.Create(filePath, arguments, AutoRunType.StartMenu);
			}
		}
	}
}
