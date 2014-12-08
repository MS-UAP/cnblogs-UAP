using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace CNBlogs.ControlHelper
{
    class TimeCountDownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            DateTime dateTimeToConvert = (DateTime)value;
            string commentTimeString = "";
            TimeSpan commentTimeLag = System.DateTime.Now - dateTimeToConvert;
            if (dateTimeToConvert.Date == System.DateTime.Now.Date)
            {
                if (TimeSpan.FromHours(1) > commentTimeLag)
                {
                    commentTimeString = string.Format("{0}分钟前", commentTimeLag.Minutes);
                }
                else
                {
                    commentTimeString = string.Format("今天{0}:{1:d2}", dateTimeToConvert.Hour, dateTimeToConvert.Minute);
                }
            }
            else if (dateTimeToConvert.AddDays(1).Date == System.DateTime.Now.Date)
            {
                commentTimeString = string.Format("昨天{0}:{1:d2}", dateTimeToConvert.Hour, dateTimeToConvert.Minute);
            }
            else if (dateTimeToConvert.AddDays(2).Date == System.DateTime.Now.Date)
            {
                commentTimeString = string.Format("前天{0}:{1:d2}", dateTimeToConvert.Hour, dateTimeToConvert.Minute);
            }
            else
            {
                commentTimeString = dateTimeToConvert.ToString("yyyy-MM-dd");
            }
            return commentTimeString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            string toConvert = (string)value;
            return System.DateTime.Now;
        }
    }
}
