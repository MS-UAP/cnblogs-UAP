using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CNBlogs.ControlHelper
{
    class FavoriteToVisibilityConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var num = int.Parse(value as string);

            if (parameter != null)
            {
                var reserve = System.Convert.ToBoolean(parameter);

                if (reserve)
                {
                    num = num == 0 ? 1 : 0;
                }
            }

            return num <8 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
