using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	class RegistryAutoRunFinder : IAutoRunFinder
	{
		private bool _isStoped;
		private readonly IProgramItemFactory _factory;
		private Action<string> _onChanged;

		public RegistryAutoRunFinder(IProgramItemFactory factory)
		{
			_factory = factory;
		}

		public IEnumerable<ProgramItemViewModel> Run()
		{
			_isStoped = false;
			var keys = new[]
			{
				Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"),
				Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce"),
				Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"),
				Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce")
			};
			try
			{
				foreach (var key in keys)
				{
					if (_isStoped)
						throw new OperationCanceledException();
					_onChanged?.Invoke(string.Format(Properties.Resources.CurrentStatusMessage, key));
					foreach (var item in GetFromRegistry(key))
					{
						yield return item;
					}
				}
			}
			finally
			{
				foreach (var key in keys)
				{
					key.Dispose();
				}
			}
		}

		public void Stop()
		{
			_isStoped = true;
		}

		private Tuple<string, string> GetPathAndArguments(string value)
		{
			if (File.Exists(value))
				return new Tuple<string, string>(Path.GetFullPath(value), string.Empty);

			var match = Regex.Match(value, "((\"(?<path>.+)\")|(?<path>[^\\s]+))\\s+(?<args>.*)");
			var path = match
				.Groups["path"]
				.Value;
			var args = match
				.Groups["args"]
				.Value.Trim();
			
			return new Tuple<string, string>(path, args);
		}

		private IEnumerable<ProgramItemViewModel> GetFromRegistry(RegistryKey key)
		{
			foreach (var name in key.GetValueNames())
			{
				if(_isStoped)
					throw new OperationCanceledException();
				
				var value = GetPathAndArguments(key.GetValue(name).ToString());
				var path = value.Item1;
				var args = value.Item2;
		
				if (File.Exists(path))
					yield return _factory.Create(path, args, AutoRunType.Registry);
			}
		}

		public void OnChangedStatus(Action<string> onChanged)
		{
			_onChanged = onChanged;

		}
	}
}