using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	public interface IAutoRunFinder
	{
		Task LoadAsync(ICollection<ProgramItemViewModel> collection);

		void OnChangedStatus(Action<string> onChanged);
	}
}
