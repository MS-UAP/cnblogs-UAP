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
using CNBlogs.DataHelper.CloudAPI;
using CNBlogs.DataHelper.Helper;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BloggerPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private AuthorPostsDS authorDS = null;
        private Blogger blogger = null;
        private ScrollViewer scrollViewer = null;
        private bool isAuthorShowOnTitle = false;

        // for binding author control
        public Author Author { get; set; }

        public BloggerPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            //this.NavigationCacheMode = NavigationCacheMode.Required;

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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            this.blogger = e.Parameter as Blogger;
            // for listview binding
            this.Author = new Author() { Avatar = this.blogger.Avatar, Name = this.blogger.Name };
            //TODO: here need author id
            Functions.ShowProgressBar(this.pb_Top);
            this.authorDS = new AuthorPostsDS(this.blogger.BlogApp);
            this.authorDS.OnLoadMoreStarted += authorDS_OnLoadMoreStarted;
            this.authorDS.OnLoadMoreCompleted += authorDS_OnLoadMoreCompleted;
            this.lv_AuthorPosts.ItemsSource = this.authorDS;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        void authorDS_OnLoadMoreStarted(uint count)
        {
            Functions.ShowProgressBar(this.pb_Top);
        }

        void authorDS_OnLoadMoreCompleted(int count)
        {
            Functions.HideProgressBar(this.pb_Top);
            this.sb_CountMoveOut.Begin();
        }

        void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (this.scrollViewer.VerticalOffset > 50)
            {
                if (!isAuthorShowOnTitle)
                {
                    this.sb_AuthorMoveUp.Begin();
                    isAuthorShowOnTitle = true;
                }
            }
            else
            {
                if (isAuthorShowOnTitle)
                {
                    this.sb_AuthorMoveDown.Begin();
                    isAuthorShowOnTitle = false;
                }
            }
        }

        private void PostControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PostControl postControl = sender as PostControl;
            if (postControl.GotoReadingPage)
            {
                Post post = postControl.DataContext as Post;
                this.Frame.Navigate(typeof(ReadingPage), post);
            }
        }

        private void lv_AuthorPosts_Loaded(object sender, RoutedEventArgs e)
        {
            this.scrollViewer = DataHelper.Helper.Functions.GetScrollViewer(this.lv_AuthorPosts);
            this.scrollViewer.ViewChanged += scrollViewer_ViewChanged;

        }

        private void sb_CountMoveOut_Completed(object sender, object e)
        {
            this.tb_PostCount.Text = string.Format("{0}/{1}", this.authorDS.Count, this.blogger.PostCount);
            this.sb_CountMoveIn.Begin();
        }

        private void btn_ScrollToTop_Click(object sender, RoutedEventArgs e)
        {
            var item0 = this.lv_AuthorPosts.Items[0];
            this.lv_AuthorPosts.ScrollIntoView(item0, ScrollIntoViewAlignment.Leading);
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Functions.ShowProgressBar(this.pb_Top);
            await this.authorDS.Refresh();
            Functions.HideProgressBar(this.pb_Top);
        }

        private void btn_Homepage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InAppWebViewPage), this.blogger.Link.Href);
        }

    }
}
