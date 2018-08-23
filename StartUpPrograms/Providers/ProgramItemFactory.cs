using System.IO;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	class ProgramItemFactory : IProgramItemFactory
	{
		public ProgramItemViewModel Create(string filePath, string arguments, AutoRunType type)
		{
			return new ProgramItemViewModel
			{
				Arguments = arguments,
				AutoRunType = type,
				FullFilePath = filePath,
				Name = Path.GetFileNameWithoutExtension(filePath)
			};
		}
	}
}