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
using CNBlogs.DataHelper.Helper;
using Windows.System;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PostReadingPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Post post = null;
        private string content = string.Empty;
        private string commentsCount = string.Empty;

        public Author Author { get; set; }


        public PostReadingPage()
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
            this.navigationHelper.OnNavigatedTo(e);
            var pageFile = string.Empty;
            if (!CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance.NightModeTheme)
            {
                pageFile = "ms-appx-web:///HTML/post_day.html";
                this.wv_WebContent.DefaultBackgroundColor = Windows.UI.Colors.White;
            }
            else
            {
                pageFile = "ms-appx-web:///HTML/post_night.html";
                this.wv_WebContent.DefaultBackgroundColor = Windows.UI.Colors.Black;
            }

            if (e.Parameter is Post)
            {
                this.post = e.Parameter as Post;
                this.Author = post.Author;
                this.commentsCount = post.CommentsCount;
                CNBlogs.DataHelper.CloudAPI.PostContentDS pcDS = new DataHelper.CloudAPI.PostContentDS(post.ID);
                if (await pcDS.LoadRemoteData())
                {
                    content = pcDS.Content;
                }

                UpdateUI();
                this.wv_WebContent.Navigate(new Uri(pageFile));
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
                var text = Windows.Data.Html.HtmlUtilities.ConvertToText(content);

                // fill post content using javascript
                await this.wv_WebContent.InvokeScriptAsync("setContent", new[] { content });
            }
            catch (Exception ex)
            {
                // invoke script may cause exception
                System.Diagnostics.Debug.WriteLine("exception when set post content", ex.Message);
            }

            FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.cmdBar);
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
        
        private void btn_Comment_Click(object sender, RoutedEventArgs e)
        {
            if (this.post != null)
            {
                this.Frame.Navigate(typeof(CommentReadingPage), this.post);
            }
        }

        private void btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
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
            if(this.post.Status == PostStatus.Favorite)
            {
                this.btn_Favorite.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.btn_UnFavorite.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                this.btn_Favorite.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.btn_UnFavorite.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void btn_Author_Click(object sender, RoutedEventArgs e)
        {
            if (this.post != null && !string.IsNullOrEmpty(this.post.BlogApp))
            {
                Blogger blogger = new Blogger()
                {
                    Avatar = this.post.Author.Avatar,
                    Name = this.post.Author.Name,
                    BlogApp = this.post.BlogApp,
                    Link = this.post.Link
                };

                this.Frame.Navigate(typeof(BloggerPage), blogger);
            }
        }

        private void btn_Source_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InAppWebViewPage), this.post.Link.Href);
        }

        private async void btn_Favorite_Click(object sender, RoutedEventArgs e)
        {
            await this.post.AsFavorite();
            this.UpdateUI();
        }

        private async void btn_UnFavorite_Click(object sender, RoutedEventArgs e)
        {
            await this.post.UnFavorite();
            this.UpdateUI();
        }
    }
}
