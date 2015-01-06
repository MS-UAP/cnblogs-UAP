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
using CNBlogs.DataHelper.Function;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CNBlogs.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class NewFavoritePage : FlatNavigationPage
    {

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


        public NewFavoritePage()
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
            //Fav posts
            this.Posts.Source = FavoritePostDS.Instance;
            this.Categories.Source = FavoriteCategoryDS.Instance.Items;
            this.Bloggers.Source = FavoriteAuthorDS.Instance.Items;
            await FavoriteCategoryDS.Instance.LoadData();
            await FavoriteAuthorDS.Instance.LoadData();
            await FavoritePostDS.Instance.LoadMoreItemsAsync(20);
            
            this.DataContext = FavoritePostDS.Instance;
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
            Frame.BackStack.Clear();
            await FavoriteCategoryDS.Instance.LoadData();
            await FavoriteAuthorDS.Instance.LoadData();
            await FavoritePostDS.Instance.Refresh();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void gv_FavoritePosts_ItemClick(object sender, ItemClickEventArgs e)
        {
            Post post = e.ClickedItem as Post;
            this.Frame.Navigate(typeof(PostReadingPage), post);
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TitleControl.DS_OnLoadMoreStarted(0);
                await FavoriteCategoryDS.Instance.LoadData();
                await FavoriteAuthorDS.Instance.LoadData();
                await FavoritePostDS.Instance.Refresh();
                TitleControl.DS_OnLoadMoreCompleted(0);
            }
            catch (Exception)
            {
                
            }
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

        private void gv_Bloggers_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void SubCategoryControl_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void btn_ScrollToTop_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
