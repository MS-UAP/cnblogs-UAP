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
using Windows.UI.Xaml.Media.Animation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CNBlogs
{
    public sealed class FavoriteGroupControl : Control
    {


        //public string Count
        //{
        //    get { return (string)GetValue(CountProperty); }
        //    set { SetValue(CountProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CountProperty =
        //    DependencyProperty.Register("Count", typeof(string), typeof(FavoriteGroupControl), new PropertyMetadata("0"));

        

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(FavoriteGroupControl), new PropertyMetadata("Icon"));


        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(FavoriteGroupControl), new PropertyMetadata("Title"));


        public FavoriteGroupControl()
        {
            this.DefaultStyleKey = typeof(FavoriteGroupControl);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            Storyboard sb = this.GetTemplateChild("sb_Roll") as Storyboard;
            if (sb != null)
            {
                sb.Begin();
            }
        }
    }
}
