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
using CNBlogs.DataHelper.CloudAPI;
using CNBlogs.DataHelper.Helper;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CNBlogs.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class BloggerPage : Page
    {
        public Author Author { get; set; }

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private AuthorPostsDS authorDS = null;
        private Blogger blogger = null;
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


        public BloggerPage()
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            this.blogger = e.Parameter as Blogger;
            // for listview binding
            this.Author = new Author() { Avatar = this.blogger.Avatar, Name = this.blogger.Name };
            this.DataContext = this;

            //Functions.ShowProgressBar(this.pb_Top);
            this.authorDS = new AuthorPostsDS(this.blogger.BlogApp);
            //this.authorDS.OnLoadMoreStarted += authorDS_OnLoadMoreStarted;
            this.authorDS.OnLoadMoreCompleted += authorDS_OnLoadMoreCompleted;
            this.gv_AuthorPosts.ItemsSource = this.authorDS;
            this.gv_SimplePosts.ItemsSource = this.authorDS;
        }

        async void authorDS_OnLoadMoreCompleted(int count)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.tb_PostCount.Text = string.Format("加载/总数 : {0}/{1}",
                                this.authorDS.Count, this.authorDS.Count > this.authorDS.Feed.PostCount ? this.authorDS.Count : this.authorDS.Feed.PostCount);
                    });
            }
            catch (Exception ex)
            {
                this.tb_PostCount.Text = ex.Message;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void gv_AuthorPosts_ItemClick(object sender, ItemClickEventArgs e)
        {
            Post post = e.ClickedItem as Post;
            this.Frame.Navigate(typeof(ReadingPage), post);
        }

        private bool isZoomOutTapped = false;

        private void sz_AuthorPosts_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            e.DestinationItem.Item = e.SourceItem.Item;
        }

        private void sz_AuthorPosts_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (!e.IsSourceZoomedInView & isZoomOutTapped)
            {
                Post post = e.DestinationItem.Item as Post;
                isZoomOutTapped = false;
                this.Frame.Navigate(typeof(Pages.ReadingPage), post);
            }
        }

        private void gv_SimplePosts_Tapped(object sender, TappedRoutedEventArgs e)
        {
            isZoomOutTapped = true;
        }
    }
}
