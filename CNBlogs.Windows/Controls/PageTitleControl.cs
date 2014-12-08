using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CNBlogs
{
    public sealed class PageTitleControl : Control
    {


        public string TitleContent
        {
            get { return (string)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleContentProperty =
            DependencyProperty.Register("TitleContent", typeof(string), typeof(PageTitleControl), new PropertyMetadata("title"));

        
        public PageTitleControl()
        {
            this.DefaultStyleKey = typeof(PageTitleControl);
        }
    }
}
