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
using CNBlogs.DataHelper;
using CNBlogs.DataHelper.CloudAPI;
using Windows.UI.Xaml.Media.Animation;
using CNBlogs.Common;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : FlatNavigationPage
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private LatestPostsDS recentDS = null;

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

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            LoadData();
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
            if (e.NavigationMode == NavigationMode.New)
            {
                Logger.LogAgent.GetInstance().WriteLog(this.GetType().ToString());
            } 
            Frame.BackStack.Clear();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

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


        private void LoadData()
        {
            this.recentDS = new LatestPostsDS();
            this.recentDS.DataRequestError += TitleControl.DS_DataRequestError;
            this.recentDS.OnLoadMoreStarted += TitleControl.DS_OnLoadMoreStarted;
            this.recentDS.OnLoadMoreCompleted += TitleControl.DS_OnLoadMoreCompleted;
            this.gv_HomePosts.ItemsSource = recentDS;
            this.gv_SimplePosts.ItemsSource = recentDS;
            this.DataContext = this.recentDS;
        }

        private void gv_HomePosts_ItemClick(object sender, ItemClickEventArgs e)
        {
                Post post = e.ClickedItem as Post;
                this.Frame.Navigate(typeof(Pages.PostReadingPage), post);
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (this.recentDS != null)
            {
                TitleControl.DS_OnLoadMoreStarted(0);
                await this.recentDS.Refresh();
                TitleControl.DS_OnLoadMoreCompleted(0);
            }
        }

        private void btn_ScrollToTop_Click(object sender, RoutedEventArgs e)
        {
            if (sz_HomePosts.IsZoomedInViewActive)
                FunctionHelper.Functions.GridViewScrollToTop(this.gv_HomePosts);
            else
                FunctionHelper.Functions.GridViewScrollToTop(this.gv_SimplePosts);
        }

        private bool isZoomOutTapped = false;

        private void sz_HomePosts_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            e.DestinationItem.Item = e.SourceItem.Item;
        }

        private void sz_HomePosts_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (!e.IsSourceZoomedInView & isZoomOutTapped)
            {
                Post post = e.DestinationItem.Item as Post;
                isZoomOutTapped = false;
                this.Frame.Navigate(typeof(Pages.PostReadingPage), post);
                sz_HomePosts.ToggleActiveView();
            }
        }

        private void gv_SimplePosts_Tapped(object sender, TappedRoutedEventArgs e)
        {
            isZoomOutTapped = true;
        }

        private void btn_ZoomChange_Click(object sender, RoutedEventArgs e)
        {
            sz_HomePosts.ToggleActiveView();
        }

        private void PostControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                PostControl postControl = sender as PostControl;
                postControl.ShowStoryBoard();
            }
            catch (Exception)
            {
            }
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.TopAppBar != null)
            {
                this.TopAppBar.IsOpen = true;
            }
            if (this.BottomAppBar != null)
            {
                this.BottomAppBar.IsOpen = true;
            }
        }
    }
}
