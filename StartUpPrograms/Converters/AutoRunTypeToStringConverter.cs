using System;
using System.Globalization;
using System.Windows.Data;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Converters
{
	class AutoRunTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var type = value as AutoRunType?;
			switch (type)
			{
				case AutoRunType.Registry:
					return Properties.Resources.Registry;
				case AutoRunType.Scheduler:
					return Properties.Resources.Schedule;
				case AutoRunType.StartMenu:
					return Properties.Resources.StartMenu;
				default:
					return "";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}