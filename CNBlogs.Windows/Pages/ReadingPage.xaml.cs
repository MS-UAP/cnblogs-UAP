using CNBlogs.Common;
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
using CNBlogs.DataHelper.Helper;
using CNBlogs.DataHelper.CloudAPI;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CNBlogs.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ReadingPage : Page
    {
        public Author Author { get; set; }
        public string Title { get; set; }

        private Post post = null;
        private News news = null;
        private string content = string.Empty;
        private CommentsDS commentDS;
        private string commentsCount = string.Empty;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public ReadingPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            string commentsCount = string.Empty;
            var pageFile = string.Empty;

            if (e.Parameter is Post)
            {
                this.post = e.Parameter as Post;
                this.Title = this.post.Title;
                this.Author = post.Author;
                commentsCount = post.CommentsCount;
                CNBlogs.DataHelper.CloudAPI.PostContentDS pcDS = new DataHelper.CloudAPI.PostContentDS(post.ID);
                if (await pcDS.LoadRemoteData())
                {
                    content = pcDS.Content;
                }

                pageFile = "ms-appx-web:///HTML/post.html";
                currentBlogger.DataContext = this.post.Author;
                this.commentDS = new CommentsDS(this.post.ID, "blog");
            }
            else if (e.Parameter is News)
            {
                this.news = e.Parameter as News;
                this.Title = this.news.Title;
                commentsCount = news.CommentsCount;
                CNBlogs.DataHelper.CloudAPI.NewsContentDS ncDS = new DataHelper.CloudAPI.NewsContentDS(news.ID);
                if (await ncDS.LoadRemoteData())
                {
                    content = ncDS.News.Content;
                }

                pageFile = "ms-appx-web:///HTML/news.html";

                this.commentDS = new CommentsDS(this.news.ID, "news");

            }
            this.DataContext = this;
            this.wv_Post.Navigate(new Uri(pageFile));
            this.lv_Comments.ItemsSource = commentDS;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void wv_Post_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            await this.wv_Post.InvokeScriptAsync("setContent", new[] { content });
        }
    }
}
