using System;
using System.Collections.Generic;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	public interface IAutoRunFinder
	{
		IEnumerable<ProgramItemViewModel> Run(Action<string> onChanged);

		void Stop();
	}
}
