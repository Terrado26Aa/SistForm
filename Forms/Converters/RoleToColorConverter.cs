using System;
using Microsoft.Maui.Controls;

namespace Forms.Converters
{
    public class RoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
            if (value is string role)
            {
                if (role == "Admin")
                    return isDark ? Color.FromArgb("#FF6B6B") : Colors.Red;
                return isDark ? Color.FromArgb("#6BCB6B") : Colors.Green;
            }
            return isDark ? Color.FromArgb("#888888") : Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
