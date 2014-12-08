using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CNBlogs.ControlHelper
{
    class NewsControlTemplateSelector : DataTemplateSelector
    {
        public DataTemplate dtHasImage { get; set; }
        public DataTemplate dtNoImage { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            CNBlogs.DataHelper.DataModel.News news = item as CNBlogs.DataHelper.DataModel.News;
            if (string.IsNullOrEmpty(news.TopicIcon))
            {
                return dtNoImage;
            }
            return dtHasImage;
        }
    }
}
