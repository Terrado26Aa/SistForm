using System;
using Microsoft.Maui.Controls;

namespace Forms.Converters
{
    public class RoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string role)
            {
                return role == "Admin" ? Colors.Red : Colors.Green;
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
