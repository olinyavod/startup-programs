using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	public interface IProgramItemFactory
	{
		ProgramItemViewModel Create(string filePath, string argiments, AutoRunType type);
	}
}
