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
    public sealed class PostControl : Control
    {
        public Visibility AuthorVisiable
        {
            get { return (Visibility)GetValue(AuthorVisiableProperty); }
            set { SetValue(AuthorVisiableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AuthorVisiable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AuthorVisiableProperty =
            DependencyProperty.Register("AuthorVisiable", typeof(Visibility), typeof(PostControl), new PropertyMetadata(Visibility.Visible));
        

        private bool ShowSummary = false;
        public bool GotoReadingPage = false;

        public PostControl()
        {
            this.DefaultStyleKey = typeof(PostControl);
            this.DataContextChanged += PostControl_DataContextChanged;
            this.Width = CNBlogs.DataHelper.Helper.Functions.GetWindowsWidth();
        }

        void PostControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.UpdateUI();
        }

        protected override void OnApplyTemplate()
        {
            this.UpdateUI();
        }

        private void UpdateUI()
        {
            CNBlogs.DataHelper.DataModel.Post post = this.DataContext as CNBlogs.DataHelper.DataModel.Post;
            if (post == null)
            {
                return;
            }
            
            TextBlock tbSummary = this.GetTemplateChild("tb_Summary") as TextBlock;
            if (tbSummary == null)
                return;
            
            if (CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance.DefaultDisplaySummary)
            {
                this.ShowSummary = true;
                tbSummary.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                this.ShowSummary = false;
                tbSummary.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            base.OnApplyTemplate();
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            CNBlogs.DataHelper.DataModel.Post post = this.DataContext as CNBlogs.DataHelper.DataModel.Post;
            if (post == null)
            {
                return;
            }

            // click to go to reading page
            if (!CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance.ClickTitleForSummary)
            {
                this.GotoReadingPage = true;
                return;
            }
            else // click to show/hide summary
            {
                if (e.OriginalSource is Windows.UI.Xaml.Shapes.Rectangle)
                {
                    this.GotoReadingPage = false;
                    TextBlock tbSummary = this.GetTemplateChild("tb_Summary") as TextBlock;
                    if (this.ShowSummary)
                    {
                        this.ShowSummary = false;
                        tbSummary.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                    else
                    {
                        this.ShowSummary = true;
                        tbSummary.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        Storyboard sbSummary = this.GetTemplateChild("sb_Summary") as Storyboard;
                        sbSummary.Begin();
                    }
                }
                else
                {
                    TextBlock tbSource = e.OriginalSource as TextBlock;
                    if (tbSource.Name == "tb_Summary")
                    {
                        this.GotoReadingPage = true;
                        // don't navigate to target page here(in control), need do that in page's viewmodel (.cs)
                        return;
                    }
                }
            }

            base.OnTapped(e);

        }
    }
}
