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
using CNBlogs.DataHelper.CloudAPI;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper;
using CNBlogs.DataHelper.CloudAPI;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace CNBlogs.Pages
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class NewsPage : FlatNavigationPage
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private NewsDS newsDs = null;

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

        public NewsPage()
        {
            this.InitializeComponent(); 
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            LoadData();
        }

        private void LoadData()
        {
            this.newsDs = new NewsDS();
            this.newsDs.OnLoadMoreStarted += TitleControl.DS_OnLoadMoreStarted;
            this.newsDs.OnLoadMoreCompleted += TitleControl.DS_OnLoadMoreCompleted;
            this.gv_News.ItemsSource = this.newsDs;
            this.gv_SimpleNews.ItemsSource = this.newsDs;
            this.DataContext = this.newsDs;
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
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void gv_News_ItemClick(object sender, ItemClickEventArgs e)
        {
            News news = e.ClickedItem as News;
            this.Frame.Navigate(typeof(ReadingPage), news);
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (this.newsDs != null)
            {
                TitleControl.DS_OnLoadMoreStarted(0);
                await this.newsDs.Refresh();
                TitleControl.DS_OnLoadMoreCompleted(0);
            }
        }

        private void btn_ScrollToTop_Click(object sender, RoutedEventArgs e)
        {
            FunctionHelper.Functions.GridViewScrollToTop(this.gv_News);
        }

        private bool isZoomOutTapped = false;

        private void sz_News_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            e.DestinationItem.Item = e.SourceItem.Item;
        }

        private void sz_News_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (!e.IsSourceZoomedInView & isZoomOutTapped)
            {
                Post post = e.DestinationItem.Item as Post;
                isZoomOutTapped = false;
                this.Frame.Navigate(typeof(Pages.ReadingPage), post);
            }
        }

        private void gv_SimpleNews_Tapped(object sender, TappedRoutedEventArgs e)
        {
            isZoomOutTapped = true;
        }
    }
}
