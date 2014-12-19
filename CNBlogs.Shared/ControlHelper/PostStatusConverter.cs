using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace CNBlogs.ControlHelper
{
    class PostStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            PostStatus ps = (PostStatus)value;
            switch(ps)
            {
                case PostStatus.Skip:
                    return " 朕无视";
                case PostStatus.Read:
                    return " 朕已阅";
                case PostStatus.Favorite:
                    return " 朕收藏";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }

    }
}
