using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MainViewModel _mainViewModel;

		public MainWindow()
		{
			InitializeComponent();
			_mainViewModel = new MainViewModel();

			DataContext = _mainViewModel;

			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var command = _mainViewModel.RefreshListCommand;
			if (command.CanExecute(null))
				command.Execute(null);
		}
	}
}
