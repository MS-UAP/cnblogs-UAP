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
    public sealed class AuthorControl : Control
    {
        
        public double NameFontSize
        {
            get { return (double)GetValue(NameFontSizeProperty); }
            set { SetValue(NameFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NameFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameFontSizeProperty =
            DependencyProperty.Register("NameFontSize", typeof(double), typeof(AuthorControl), new PropertyMetadata(20));
                

        public SolidColorBrush NameColor
        {
            get { return (SolidColorBrush)GetValue(NameColorProperty); }
            set { SetValue(NameColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NameColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameColorProperty =
            DependencyProperty.Register("NameColor", typeof(SolidColorBrush), typeof(AuthorControl), new PropertyMetadata(new SolidColorBrush(Windows.UI.Colors.Black)));

        public double AvatarHeight
        {
            get { return (double)GetValue(AvatarHeightProperty); }
            set { SetValue(AvatarHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AvatarHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvatarHeightProperty =
            DependencyProperty.Register("AvatarHeight", typeof(double), typeof(AuthorControl), new PropertyMetadata(20));
        
        public AuthorControl()
        {
            this.DefaultStyleKey = typeof(AuthorControl);
        }
    }
}
