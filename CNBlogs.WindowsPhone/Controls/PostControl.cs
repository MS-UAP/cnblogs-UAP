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
using CNBlogs.DataHelper.Helper;
// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CNBlogs
{
    public sealed class PostControl : Control
    {
        public Visibility AttributionVisible
        {
            get { return (Visibility)GetValue(AttributionVisibleProperty); }
            set { SetValue(AttributionVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AttributionVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttributionVisibleProperty =
            DependencyProperty.Register("AttributionVisible", typeof(Visibility), typeof(PostControl), new PropertyMetadata(Visibility.Visible));
       
        public Visibility AuthorVisible
        {
            get { return (Visibility)GetValue(AuthorVisibleProperty); }
            set { SetValue(AuthorVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AuthorVisiable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AuthorVisibleProperty =
            DependencyProperty.Register("AuthorVisible", typeof(Visibility), typeof(PostControl), new PropertyMetadata(Visibility.Visible));


        private bool showSummary = false;
        public bool GotoReadingPage = false;
        private TextBlock tbSummary, tbStatus;
        private AttributionControl controlAtt;

        public PostControl()
        {
            this.DefaultStyleKey = typeof(PostControl);
            this.DataContextChanged += PostControl_DataContextChanged;
        }

        void PostControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.UpdateUI(false);
        }

        protected override void OnApplyTemplate()
        {
            this.UpdateUI(false);
        }

        private void GetSummaryControl()
        {
            if (this.tbSummary == null)
            {
                this.tbSummary = this.GetTemplateChild("tb_Summary") as TextBlock;
            }
        }

        private void GetStatusControl()
        {
            if (this.tbStatus == null)
            {
                this.tbStatus = this.GetTemplateChild("tb_Status") as TextBlock;
            }
        }

        private void GetAttributionControl()
        {
            if (this.controlAtt == null)
            {
                this.controlAtt = this.GetTemplateChild("control_Attribution") as AttributionControl;
            }
        }

        private void UpdateUI(bool showAnimation = true)
        {
            CNBlogs.DataHelper.DataModel.Post post = this.DataContext as CNBlogs.DataHelper.DataModel.Post;
            if (post == null)
            {
                return;
            }

            switch (post.Status)
            {
                case DataHelper.DataModel.PostStatus.None:
                    this.ShowSummary(showAnimation);
                    //this.HideStatus();
                    break;

                case DataHelper.DataModel.PostStatus.Skip:
                    this.HideSummary();
                    //this.ShowStatus(" 朕无视");
                    break;

                case DataHelper.DataModel.PostStatus.Read:
                    this.HideSummary();
                    //this.ShowStatus(" 朕已阅");
                    break;

                case DataHelper.DataModel.PostStatus.Favorite:
                    this.HideSummary();
                    //this.ShowStatus(" 朕收藏");
                    break;

                default:
                    post.Status = DataHelper.DataModel.PostStatus.None;
                    this.ShowSummary(showAnimation);
                    //this.HideStatus();
                    break;
            }

            base.OnApplyTemplate();
        }

        private void ShowSummary(bool showAnimation)
        {
            this.showSummary = true;
            this.GetSummaryControl();
            if (this.tbSummary != null)
            {
                this.tbSummary.Visibility = Windows.UI.Xaml.Visibility.Visible;
                if (showAnimation)
                {
                    Storyboard sbSummary = this.GetTemplateChild("sb_Summary") as Storyboard;
                    sbSummary.Begin();
                }
            }
            this.GetAttributionControl();
            if (this.controlAtt != null)
            {
                this.controlAtt.Visibility = this.AttributionVisible;
            }
        }

        private void HideSummary()
        {
            this.showSummary = false;
            this.GetSummaryControl();
            if (this.tbSummary != null)
            {
                this.tbSummary.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            this.GetAttributionControl();
            if (this.controlAtt != null)
            {
                this.controlAtt.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
        /*
        private void ShowStatus(string text)
        {
            this.GetStatusControl();
            if (this.tbStatus != null)
            {
                this.tbStatus.Text = text;
                this.tbStatus.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private void HideStatus()
        {
            this.GetStatusControl();
            if (this.tbStatus != null)
            {
                this.tbStatus.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
        */
        /// <summary>
        /// if user click title, control will collapse summary, set status = Skip, next time: show title only
        /// if user click summary, goto reading page, set status = Read, next time: show title only
        /// if user click favorite in reading page, set status = Favorite, next time: show title only
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            CNBlogs.DataHelper.DataModel.Post post = this.DataContext as CNBlogs.DataHelper.DataModel.Post;
            if (post == null)
            {
                return;
            }

            // click on the title
            if (e.OriginalSource is Windows.UI.Xaml.Shapes.Rectangle)
            {
                this.GotoReadingPage = false;
                if (this.showSummary) // show summary
                {
                    this.HideSummary();
                    this.SetNewStatus(post, DataHelper.DataModel.PostStatus.Skip);
                }
                else // hide summary
                {
                    this.ShowSummary(true);
                }
            }
            else // click on the summary
            {
                TextBlock tbSource = e.OriginalSource as TextBlock;
                if (tbSource.Name == "tb_Summary")
                {
                    // don't navigate to target page here(in control), need do that in page's viewmodel (.cs)
                    this.GotoReadingPage = true;
                    this.SetNewStatus(post, DataHelper.DataModel.PostStatus.Read);
                }
            }

            base.OnTapped(e);
        }

        private void SetNewStatus(CNBlogs.DataHelper.DataModel.Post post, CNBlogs.DataHelper.DataModel.PostStatus newStatus, bool updateUI = true)
        {
            if (post.Status != newStatus)
            {
                if (post.Status <= newStatus)
                {
                    post.Status = newStatus;
                    DataHelper.DataModel.CNBlogSettings.Instance.SaveBlogStatus(post);
                }
                if (updateUI)
                {
                    this.UpdateUI();
                }
            }
        }
    }
}
