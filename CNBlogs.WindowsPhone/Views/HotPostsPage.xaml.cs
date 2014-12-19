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
using CNBlogs.DataHelper.CloudAPI;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Helper;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HotPostsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private TwoDaysTopViewPostsDS hotPostsDS;

        public HotPostsPage()
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
                this.LoadData();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void LoadData()
        {
            //FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, this.appbar);

            this.hotPostsDS = new DataHelper.CloudAPI.TwoDaysTopViewPostsDS();
            this.hotPostsDS.OnLoadMoreStarted += hotPostsDS_OnLoadMoreStarted;
            this.hotPostsDS.OnLoadMoreCompleted += hotPostsDS_OnLoadMoreCompleted;
            this.lv_HotPosts.ItemsSource = this.hotPostsDS;
            await this.hotPostsDS.LoadMoreItemsAsync(20);

            //FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.appbar);
        }

        async void hotPostsDS_OnLoadMoreCompleted(int count)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.control_PageTitle.ChangeSubTitleContent(this.hotPostsDS.Count.ToString());
                FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.appbar);
            });            
        }

        async void hotPostsDS_OnLoadMoreStarted(uint count)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, this.appbar);
            });          
        }

        private void PostControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PostControl postControl = sender as PostControl;
            if (postControl.GotoReadingPage)
            {
                Post post = postControl.DataContext as Post;
                this.Frame.Navigate(typeof(PostReadingPage), post);
            }
            else
            {
                this.lv_HotPosts.ScrollIntoView(sender, ScrollIntoViewAlignment.Leading);
            }
        }

        private void btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            //FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, this.appbar);

            await this.hotPostsDS.Refresh();

            //FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.appbar);
        }

        private void btn_Top_Click(object sender, RoutedEventArgs e)
        {
            FunctionHelper.Functions.ListViewScrollToTop(this.lv_HotPosts);
        }
    }
}
