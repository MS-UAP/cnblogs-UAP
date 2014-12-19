using CNBlogs.Common;
using CNBlogs.DataHelper.CloudAPI;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Helper;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
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
            this.NavigationCacheMode = NavigationCacheMode.Required;

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
            if (e.NavigationMode == NavigationMode.New)
            {
                this.authorDS = null;
                this.DataContext = null;
                if (e.Parameter is Blogger)
                {
                    this.blogger = e.Parameter as Blogger;
                    this.Author = new Author() { Avatar = this.blogger.Avatar, Name = this.blogger.Name };
                    this.DataContext = this;
                    // for listview binding
                    this.authorDS = new AuthorPostsDS(this.blogger.BlogApp);
                }
                else if (e.Parameter is string)
                {
                    string blogapp = (string)e.Parameter;
                    this.authorDS = new AuthorPostsDS(blogapp);
                }
                this.authorDS.OnLoadMoreStarted += authorDS_OnLoadMoreStarted;
                this.authorDS.OnLoadMoreCompleted += authorDS_OnLoadMoreCompleted;
                this.lv_AuthorPosts.ItemsSource = this.authorDS;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        async void authorDS_OnLoadMoreStarted(uint count)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, this.cmdBar);
            });
        }

        async void authorDS_OnLoadMoreCompleted(int count)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.cmdBar);
                this.sb_CountMoveOut.Begin();
            });
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
                this.Frame.Navigate(typeof(PostReadingPage), post);
            }
        }

        private void lv_AuthorPosts_Loaded(object sender, RoutedEventArgs e)
        {
            this.scrollViewer = FunctionHelper.Functions.GetScrollViewer(this.lv_AuthorPosts);
            this.scrollViewer.ViewChanged += scrollViewer_ViewChanged;

        }

        private void sb_CountMoveOut_Completed(object sender, object e)
        {
            var max = this.authorDS.Feed.PostCount;

            //check if max less than current, prevent showing 5/2
            if (this.authorDS.Count > max)
            {
                max = this.authorDS.Count;
            }
            this.tb_PostCount.Text = string.Format("{0}/{1}", this.authorDS.Count, max);
            this.sb_CountMoveIn.Begin();
        }

        private void btn_ScrollToTop_Click(object sender, RoutedEventArgs e)
        {
            var item0 = this.lv_AuthorPosts.Items[0];
            this.lv_AuthorPosts.ScrollIntoView(item0, ScrollIntoViewAlignment.Leading);
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, this.cmdBar);
            await this.authorDS.Refresh();
            FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.cmdBar);
        }

        private void btn_Homepage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InAppWebViewPage), this.blogger.Link.Href);
        }

    }
}
