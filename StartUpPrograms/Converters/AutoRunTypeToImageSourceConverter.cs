using System;
using System.Globalization;
using System.Windows.Data;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Converters
{
	class AutoRunTypeToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var type = value as AutoRunType?;
			switch (type)
			{
				case AutoRunType.Registry:
					return "/StartUpPrograms;component/Images/RegEdit.png";
				case AutoRunType.Scheduler:
					return "/StartUpPrograms;component/Images/Schedule.png";
				case AutoRunType.StartMenu:
					return "/StartUpPrograms;component/Images/StartMenu.png";
				default:
					return string.Empty;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
