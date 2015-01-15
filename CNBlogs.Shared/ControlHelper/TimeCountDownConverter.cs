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
            Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            DateTime dateTimeToConvert = (DateTime)value;
            string commentTimeString = "";
            TimeSpan commentTimeLag = System.DateTime.Now - dateTimeToConvert;
            if (dateTimeToConvert.Date == System.DateTime.Now.Date)
            {
                if (TimeSpan.FromHours(1) > commentTimeLag)
                {
                    if (commentTimeLag.Minutes <= 1)
                        commentTimeString = string.Format("{0}" + loader.GetString("TimeConverter_Recent_1"), commentTimeLag.Minutes);
                    else
                        commentTimeString = string.Format("{0}" + loader.GetString("TimeConverter_Recent"), commentTimeLag.Minutes);
                }
                else
                {
                    commentTimeString = string.Format(loader.GetString("TimeConverter_Today") + "{0}:{1:d2}", dateTimeToConvert.Hour, dateTimeToConvert.Minute);
                }
            }
            else if (dateTimeToConvert.AddDays(1).Date == System.DateTime.Now.Date)
            {
                commentTimeString = string.Format(loader.GetString("TimeConverter_Yesterday") + "{0}:{1:d2}", dateTimeToConvert.Hour, dateTimeToConvert.Minute);
            }
            //else if (dateTimeToConvert.AddDays(2).Date == System.DateTime.Now.Date)
            //{
            //    commentTimeString = string.Format(loader.GetString("TimeConverter_DayBeforeYesterday") + "{0}:{1:d2}", dateTimeToConvert.Hour, dateTimeToConvert.Minute);
            //}
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
