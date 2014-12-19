using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.CloudAPI;
using CNBlogs.DataHelper.Helper;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommentReadingPage : Page
    {
        private CommentsDS commentDS;
        private Common.NavigationHelper navigationHelper;
        private Post post = null;
        private News news = null;
        private string commentsCount = string.Empty;

        public CommentReadingPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new Common.NavigationHelper(this);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string typeValue = string.Empty;
            string id = string.Empty;

            if (e.Parameter is Post)
            {
                this.post = e.Parameter as Post;
                this.commentsCount = post.CommentsCount;
                this.commentDS = new CommentsDS(this.post.ID, "blog");
            }
            else if (e.Parameter is News)
            {
                this.news = e.Parameter as News;
                this.commentsCount = news.CommentsCount;
                this.commentDS = new CommentsDS(this.news.ID, "news");
            }
            this.commentDS.OnLoadMoreStarted += commentDS_OnLoadMoreStarted;
            this.commentDS.OnLoadMoreCompleted += commentDS_OnLoadMoreCompleted;
            this.lv_Comments.ItemsSource = commentDS;

        }

        async void commentDS_OnLoadMoreStarted(uint count)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, null);
            });
        }

        async void commentDS_OnLoadMoreCompleted(int count)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, null);
                this.sb_Hide.Begin();
            });
        }

        private void sb_Hide_Completed(object sender, object e)
        {
            var max = Functions.ParseInt(this.commentsCount);

            //check if max less than current, prevent showing 5/2
            if (this.commentDS.Count > max)
            {
                max = this.commentDS.Count;
            }

            this.tb_CommentCount.Text = string.Format("{0}/{1}", this.commentDS.Count, max);
            this.sb_Show.Begin();
        }

        private void btn_ScrollToTop_Click(object sender, RoutedEventArgs e)
        {
            var item0 = this.lv_Comments.Items[0];
            this.lv_Comments.ScrollIntoView(item0, ScrollIntoViewAlignment.Leading);
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            await this.commentDS.Refresh();
        }
    }
}
