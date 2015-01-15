using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace CNBlogs.ControlHelper
{
    class NightModeLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool bValue = (bool)value;
            if (bValue)
            {
                return FunctionHelper.Functions.LoadResourceString("DayModeText");
            }
            else
            {
                return FunctionHelper.Functions.LoadResourceString("NightModeText");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
