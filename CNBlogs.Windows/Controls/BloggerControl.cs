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
    public sealed class BloggerControl : Control
    {

        public Visibility AttributionVisiable
        {
            get { return (Visibility)GetValue(AttributionVisiableProperty); }
            set { SetValue(AttributionVisiableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AttributionVisiable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttributionVisiableProperty =
            DependencyProperty.Register("AttributionVisiable", typeof(Visibility), typeof(BloggerControl), new PropertyMetadata(Visibility.Visible));

        public BloggerControl()
        {
            this.DefaultStyleKey = typeof(BloggerControl);
        }
    }
}
