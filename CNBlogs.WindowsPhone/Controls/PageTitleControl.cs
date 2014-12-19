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
    public sealed class PageTitleControl : Control
    {


        public string TitleContent
        {
            get { return (string)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleContentProperty =
            DependencyProperty.Register("TitleContent", typeof(string), typeof(PageTitleControl), new PropertyMetadata("Default Title"));


        public string SubTitleContent
        {
            get { return (string)GetValue(SubTitleContentProperty); }
            set { SetValue(SubTitleContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubTitleContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubTitleContentProperty =
            DependencyProperty.Register("SubTitleContent", typeof(string), typeof(PageTitleControl), new PropertyMetadata(""));

                

        public PageTitleControl()
        {
            this.DefaultStyleKey = typeof(PageTitleControl);
        }

        private string newContent;

        public void ChangeSubTitleContent(string newContent, bool doAnimation = true)
        {
            this.newContent = newContent;
            Storyboard sbHide = this.GetTemplateChild("sb_Hide") as Storyboard;
            sbHide.Completed += sbHide_Completed;
            sbHide.Begin();
        }

        void sbHide_Completed(object sender, object e)
        {
            this.SubTitleContent = this.newContent;
            Storyboard sbShow = this.GetTemplateChild("sb_Show") as Storyboard;
            sbShow.Begin();
        }
    }
}
