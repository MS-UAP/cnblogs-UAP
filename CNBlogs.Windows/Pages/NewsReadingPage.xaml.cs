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
using CNBlogs.DataHelper.Function;
using CNBlogs.DataHelper.CloudAPI;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CNBlogs.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class NewsReadingPage : FlatNavigationPage
    {
        public string Title { get; set; }

        private News news = null;
        private string content = string.Empty;
        private CommentsDS commentDS;
        private string commentsCount = "0";

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


        public NewsReadingPage()
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
            try
            {
                navigationHelper.OnNavigatedTo(e);

                if (e.NavigationMode == NavigationMode.New)
                {
                    Logger.LogAgent.GetInstance().WriteLog(this.GetType().ToString());
                }
                this.commentsCount = string.Empty;
                var pageFile = string.Empty;

                if (e.Parameter is News)
                {
                    this.news = e.Parameter as News;
                    this.Title = this.news.Title;
                    this.commentsCount = news.CommentsCount;
                    CNBlogs.DataHelper.CloudAPI.NewsContentDS ncDS = new DataHelper.CloudAPI.NewsContentDS(news.ID);
                    if (await ncDS.LoadRemoteData())
                    {
                        content = ncDS.News.Content;
                    }

                    pageFile = "ms-appx-web:///HTML/news_day.html";

                    this.commentDS = new CommentsDS(this.news.ID, "news");
                }
                else
                {
                    return;
                }

                this.DataContext = this;
                this.wv_Post.Navigate(new Uri(pageFile));
                this.commentDS.OnLoadMoreCompleted += commentDS_OnLoadMoreCompleted;

                this.lv_Comments.ItemsSource = commentDS;
                if (this.commentsCount.Trim('/') == "0")
                {
                    this.lv_Comments.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    this.tb_Message.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }

                //Set Visual States
                this.SetPageVisualStatus(RenderSize.Width);
                this.SetTitleFont(pageTitle.Text.Length);
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        async void commentDS_OnLoadMoreCompleted(int count)
        {
            try
            {
                if (this.commentDS != null)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                      {
                          char[] charsToTrim = { '*', ' ', '\'', '\\', '/' };
                          this.tb_CommentCount.Text = string.Format("{0}/{1}", this.commentDS.Count, this.commentsCount.Trim(charsToTrim));
                      });
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void wv_Post_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            try
            {
                pr_Loading.IsActive = false;
                await this.wv_Post.InvokeScriptAsync("setContent", new[] { content });
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }

        }

        private void btn_OriginalLink_Click(object sender, RoutedEventArgs e)
        {
            if (this.news != null)
            {
                wv_Post.Navigate(new Uri(this.news.Link.Href));
            }
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            SetPageVisualStatus(e);
        }

        Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        private void SetPageVisualStatus(SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1366)
            {
                Sb_CommentHiddenVIew.Begin();
                isCommentTemperarorilyShown = false;
            }
            else
            {
                Sb_CommentShownVIew.Begin();
                isCommentTemperarorilyShown = false;
            }
            CommentButton.Content = isCommentTemperarorilyShown ? loader.GetString("CommentTextblockText_Status2") : loader.GetString("CommentTextblockText_Status1");
        }

        private void SetPageVisualStatus(double pageWidth)
        {
            if (pageWidth < 1366)
            {
                Sb_CommentHiddenVIew.Begin();
                isCommentTemperarorilyShown = false;
            }
            else
            {
                Sb_CommentShownVIew.Begin();
                isCommentTemperarorilyShown = false;
            }
            CommentButton.Content = isCommentTemperarorilyShown ? loader.GetString("CommentTextblockText_Status2") : loader.GetString("CommentTextblockText_Status1");
        }


        private bool isCommentTemperarorilyShown = false;

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (isCommentTemperarorilyShown)
            {
                Sb_CommentHidden.Begin();
            }
            else
            {
                Sb_CommentTemperaroryShown.Begin();
            }
            isCommentTemperarorilyShown = !isCommentTemperarorilyShown;
            CommentButton.Content = isCommentTemperarorilyShown ? loader.GetString("CommentTextblockText_Status2") : loader.GetString("CommentTextblockText_Status1");
        }

        private void wv_Post_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isCommentTemperarorilyShown)
            {
                Sb_CommentHidden.Begin(); 
                isCommentTemperarorilyShown = !isCommentTemperarorilyShown;
                CommentButton.Content = isCommentTemperarorilyShown ? loader.GetString("CommentTextblockText_Status2") : loader.GetString("CommentTextblockText_Status1");
            }
        }

        private void SetTitleFont(int titleLength)
        {
            if (titleLength > 20)
                pageTitle.FontSize = 24;
            else if (titleLength > 40)
                pageTitle.FontSize = 20;
        }
    }
}
