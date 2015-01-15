using CNBlogs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Function;
using Windows.System;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewsReadingPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private News news = null;
        private string content = string.Empty;
        private string commentsCount = string.Empty;

        public Author Author { get; set; }


        public NewsReadingPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
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
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                Logger.LogAgent.GetInstance().WriteLog(this.GetType().ToString());
            }

            this.navigationHelper.OnNavigatedTo(e);
            var pageFile = string.Empty;
            if (!CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance.NightModeTheme)
            {
                pageFile = "ms-appx-web:///HTML/news_day.html#width={0}&height={1}";
                this.wv_WebContent.DefaultBackgroundColor = Windows.UI.Colors.White;
            }
            else
            {
                pageFile = "ms-appx-web:///HTML/news_night.html#width={0}&height={1}";
                this.wv_WebContent.DefaultBackgroundColor = Windows.UI.Colors.Black;
            }

            if (e.Parameter is News)
            {
                this.news = e.Parameter as News;
                this.commentsCount = news.CommentsCount;
                CNBlogs.DataHelper.CloudAPI.NewsContentDS ncDS = new DataHelper.CloudAPI.NewsContentDS(news.ID);
                if (await ncDS.LoadRemoteData())
                {
                    //this.wv_Post.NavigateToString(ncDS.News.Content);
                    // add title to news
                    content = "<h2 class='title'>" + this.news.Title + "</h2>" + ncDS.News.Content;
                }
                this.UpdateUI();
                string width = Window.Current.Bounds.Width.ToString();
                string height = Window.Current.Bounds.Height.ToString();

                this.wv_WebContent.Navigate(new Uri(string.Format(pageFile, width, height)));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private async void wv_WebContent_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            try
            {
                var newSize = CNBlogSettings.Instance.FontSize / 100 + 1;

                await this.wv_WebContent.InvokeScriptAsync("changeFontSize", new[] { newSize.ToString() });

                var text = Windows.Data.Html.HtmlUtilities.ConvertToText(content);

                // fill post content using javascript
                await this.wv_WebContent.InvokeScriptAsync("setContent", new[] { content });

            }
            catch (Exception ex)
            {
                // invoke script may cause exception
                System.Diagnostics.Debug.WriteLine("exception when set post content", ex.Message);
            }

            FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, null);
        }

        private void UpdateUI()
        {
            if (this.commentsCount == "0" || string.IsNullOrEmpty(this.commentsCount))
            {
                this.btn_Comment.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                this.btn_Comment.Label = string.Format("{0}条评论", this.commentsCount);
            }
        }


        private async void wv_WebContent_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            // in this page, we only need to load start page, and will using IE to load other links
            if (args.Uri.Scheme != "ms-appx-web")
            {
                args.Cancel = true;

                // using launcher to open links in IE
                await Launcher.LaunchUriAsync(args.Uri);
            }
        }

        #region command bar

        private void btn_Source_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InAppWebViewPage), this.news.Link.Href);
        }

        private void btn_Comment_Click(object sender, RoutedEventArgs e)
        {
            if (this.news != null)
            {
                this.Frame.Navigate(typeof(CommentReadingPage), this.news);
            }
        }

        private void btn_FontSize_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage), SettingPagePivotItemTags.FontSize);
        }

        #endregion
    }
}
