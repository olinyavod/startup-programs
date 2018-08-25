using StartUpPrograms.Providers;

namespace StartUpPrograms.ViewModels
{
	public class ProgramItemViewModel : ViewModelBase
	{
		public string Name
		{
			get => GetProperty(() => Name);
			set => SetProperty(() => Name, value);
		}

		public string Arguments
		{
			get => GetProperty(() => Arguments);
			set => SetProperty(() => Arguments, value);
		}

		public string FullFilePath
		{
			get => GetProperty(() => FullFilePath);
			set => SetProperty(() => FullFilePath, value);
		}

		public bool HasCertificate
		{
			get => GetProperty(() => HasCertificate);
			set => SetProperty(() => HasCertificate, value);
		}

		public string Company
		{
			get => GetProperty(() => Company);
			set => SetProperty(() => Company, value);
		}

		public AutoRunType AutoRunType
		{
			get => GetProperty(() => AutoRunType);
			set => SetProperty(() => AutoRunType, value);
		}

		public bool IsVerify
		{
			get => GetProperty(() => IsVerify);
			set => SetProperty(() => IsVerify, value);
		}

		public void Open()
		{
			ShellHelper.OpenExplorer(FullFilePath);
		}
	}
}