using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	public interface IAutoRunFinder
	{
		IEnumerable<ProgramItemViewModel> Run();

		void Stop();

		void OnChangedStatus(Action<string> onChanged);
	}
}
