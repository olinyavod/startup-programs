using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	class RegistryAutoRunFinder : IAutoRunFinder
	{
		private readonly CancellationTokenSource _tokenSource;
		private readonly IProgramItemFactory _factory;
		private Action<string> _onChanged;

		public RegistryAutoRunFinder(CancellationTokenSource tokenSource, IProgramItemFactory factory)
		{
			_tokenSource = tokenSource;
			_factory = factory;
		}

		public async Task LoadAsync(ICollection<ProgramItemViewModel> collection)
		{
			var keys = new[]
			{
				Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"),
				Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce"),
				Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"),
				Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\RunOnce")
			};

			foreach (var key in keys)
			{
				if(_tokenSource.IsCancellationRequested)
					return;
				_onChanged?.Invoke(string.Format(Properties.Resources.CurrentStatusMessage, key));
				var list = await Task.Run(() => GetFromRegistry(key).ToArray(), _tokenSource.Token);
				key.Close();
				foreach (var item in list)
				{
					collection.Add(item);
				}
			}
		}

		private Tuple<string, string> GetPathAndArguments(string value)
		{
			var invalidPathChars = Path.GetInvalidPathChars();
			
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
				if(_tokenSource.IsCancellationRequested)
					yield break;
				
				var value = GetPathAndArguments(key.GetValue(name).ToString());
				var path = value.Item1;
				var args = value.Item2;
		
				
				if (File.Exists(path))
					yield return _factory.Create(path, args, AutoRunType.Registry);
				else
				{

				}
			}
		}

		public void OnChangedStatus(Action<string> onChanged)
		{
			_onChanged = onChanged;

		}
	}
}