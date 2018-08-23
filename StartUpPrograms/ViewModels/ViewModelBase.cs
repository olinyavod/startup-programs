using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime;

namespace StartUpPrograms.ViewModels
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public static string GetPropertyName<TProperty>(Expression<Func<TProperty>> property)
		{
			if (property.Body is MemberExpression me)
			{
				return me.Member.Name;
			}

			return null;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		
		private readonly Dictionary<string, object> _properties;

		protected ViewModelBase()
		{
			_properties = new Dictionary<string, object>();
		}
		
		protected virtual void RaisePropertyChanged(string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected TProperty GetProperty<TProperty>(string propertyName) => _properties.TryGetValue(propertyName, out var v)
				? (TProperty) v
				: default(TProperty);

		protected TProperty GetProperty<TProperty>(Expression<Func<TProperty>> property) => GetProperty<TProperty>(GetPropertyName(property));

		protected void SetProperty<TProperty>(string propertyName, TProperty value, Action<TProperty> onChanged = null)
		{
			var oldValue = GetProperty<TProperty>(propertyName);
			if (!Equals(oldValue, value))
			{
				if (_properties.ContainsKey(propertyName))
				{
					_properties[propertyName] = value;
				}
				else
				{
					_properties.Add(propertyName, value);
				}
				RaisePropertyChanged(propertyName);
				onChanged?.Invoke(oldValue);
			}
		}

		protected void SetProperty<TProperty>(Expression<Func<TProperty>> property, TProperty value, Action<TProperty> onChanged = null)
		{
			SetProperty(GetPropertyName(property), value, onChanged);
		}
	}
}
