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
using CNBlogs.DataHelper.Helper;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CNBlogs.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CategoryPage : FlatNavigationPage
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private List<Category> listCategory = null;
        private CategoryPostDS categoryDS = null;

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


        public CategoryPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.LoadData();
        }

        private async void LoadData()
        {
            this.listCategory = await PostHelper.GetCategories();
            Categories.Source = this.listCategory;
            this.TitleControl.SetTopProgressBarVisibility(false);
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
            this.LoadData();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (sz_FavoritePosts.IsZoomedInViewActive)
            {
                if (this.categoryDS != null)
                {
                    TitleControl.DS_OnLoadMoreStarted(0);
                    await this.categoryDS.Refresh();
                    TitleControl.DS_OnLoadMoreCompleted(0);
                }
            }
        }

        private bool isZoomOutTapped = false;

        private void sz_FavoritePosts_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView)
                this.DataContext = null;
            e.DestinationItem.Item = e.SourceItem.Item;
        }

        private async void sz_FavoritePosts_ViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (!e.IsSourceZoomedInView & isZoomOutTapped)
            {                

                Category category = e.DestinationItem.Item as Category;
                this.categoryDS = new CategoryPostDS(category);
                this.categoryDS.OnLoadMoreStarted += TitleControl.DS_OnLoadMoreStarted;
                this.categoryDS.OnLoadMoreCompleted += TitleControl.DS_OnLoadMoreCompleted;
                gv_CategoryPosts.ItemsSource = this.categoryDS;
                this.DataContext = categoryDS;

                //change the topic
                this.TitleControl.TitleContent = "全站 - 分类 - " + category.Name;

                //get data
                await this.categoryDS.LoadMoreItemsAsync(20);

                //ensure is tapped
                isZoomOutTapped = false;
            }
        }

        private void gv_CategoryPosts_ItemClick(object sender, ItemClickEventArgs e)
        {
            Post post = e.ClickedItem as Post;
            this.Frame.Navigate(typeof(ReadingPage), post);
        }

        private void btn_ScrollToTop_Click(object sender, RoutedEventArgs e)
        {
            CNBlogs.DataHelper.Helper.Functions.GridViewScrollToTop(this.gv_CategoryPosts);
        }

        private void gv_Category_Tapped(object sender, TappedRoutedEventArgs e)
        {
            isZoomOutTapped = true;
        }
    }
}
