using CNBlogs.DataHelper.CloudAPI;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Helper;
using System;
using System.ComponentModel;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        private Common.NavigationHelper navigationHelper;
        private LatestPostsDS recentDS;
        private TenDaysTopLikePostsDS topLikeDS;
        private TwoDaysTopViewPostsDS topViewDS;
        private RecommendBloggerDS topBloggerDS;
        private NewsDS newsDs;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.DataContext = this;
            CreateDictionaryData();
            LoadData();
        }

        private async void LoadData()
        {
            // hide splash screen after 2 seconds
            ThreadPoolTimer.CreateTimer(SplashTimeOut, new TimeSpan(0, 0, 2));

            this.recentDS = new LatestPostsDS();
            this.recentDS.DataRequestError += recentDS_DataRequestError;
            this.recentDS.OnLoadMoreCompleted += recentDS_OnLoadMoreCompleted;
            this.lv_HomePosts.ItemsSource = this.recentDS;
            this.tb_HomeTag.DataContext = this.recentDS;

            this.topViewDS = new DataHelper.CloudAPI.TwoDaysTopViewPostsDS();
            this.topViewDS.DataRequestError += recentDS_DataRequestError;
            this.lv_HotPosts.ItemsSource = this.topViewDS;
            this.tb_HotTag.DataContext = this.topViewDS;

            this.topLikeDS = new TenDaysTopLikePostsDS();
            this.topLikeDS.DataRequestError += recentDS_DataRequestError;
            this.lv_BestPosts.ItemsSource = this.topLikeDS;
            this.tb_BestTag.DataContext = this.topLikeDS;

            this.topBloggerDS = new RecommendBloggerDS();
            this.topBloggerDS.DataRequestError += recentDS_DataRequestError;
            this.lv_Bloggers.ItemsSource = this.topBloggerDS.Bloggers;
            await topBloggerDS.LoadRemoteData();

            this.newsDs = new NewsDS();
            this.newsDs.DataRequestError += recentDS_DataRequestError;
            this.lv_News.ItemsSource = this.newsDs;
            this.tb_NewsTag.DataContext = this.newsDs;
        }

        void recentDS_OnLoadMoreCompleted(int count)
        {
            Functions.HideProgressBar(this.pb_Top);
        }
        
        private async void SplashTimeOut(ThreadPoolTimer timer)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.grid_Splash.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    this.grid_Main.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    this.appbar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                });
        }

        void recentDS_DataRequestError(int code)
        {
            if (code == -1 && !this.popup_tips.IsOpen && this.ActualWidth > 0)
            {
                this.tbk_tips.Text = "网络好像不给力啊";
                this.grid_tips.Width = this.ActualWidth - 40;
                this.popup_tips.Margin = new Thickness(20, this.ActualHeight - 180, 20, 0);
                this.popup_tips.IsOpen = true;

                ThreadPoolTimer.CreateTimer(async t =>
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.popup_tips.IsOpen = false;
                    });

                }, TimeSpan.FromSeconds(3));
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
            this.navigationHelper = new Common.NavigationHelper(this);
        }

        #region PivotItem Today/Hot Post

        private void PostControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PostControl postControl = sender as PostControl;
            if (postControl.GotoReadingPage)
            {
                Post post = postControl.DataContext as Post;
                this.Frame.Navigate(typeof(ReadingPage), post);
            }
            else
            {
                this.lv_HomePosts.ScrollIntoView(sender, ScrollIntoViewAlignment.Leading);
            }
        }

        #endregion

        #region PivotItem Recommend Author
        private void lv_RecommendPosts_ItemClick(object sender, ItemClickEventArgs e)
        {
            Blogger blogger = e.ClickedItem as Blogger;
            this.Frame.Navigate(typeof(BloggerPage), blogger);
        }
        #endregion

        #region PivotItem News

        private void lv_News_ItemClick(object sender, ItemClickEventArgs e)
        {
            News news = e.ClickedItem as News;
            this.Frame.Navigate(typeof(ReadingPage), news);
        }

        #endregion

        #region bottom app bar

        private void btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Functions.ShowProgressBar(this.pb_Top);
            
            await this.recentDS.Refresh();
            await this.topLikeDS.Refresh();
            await this.topViewDS.Refresh();
            await this.topBloggerDS.Refresh();
            await this.newsDs.Refresh();

            Functions.HideProgressBar(this.pb_Top);
        }

        private void btn_Top_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pi = this.pivot_Main.SelectedItem as PivotItem;
            switch((string)pi.Tag)
            {
                case "home":
                    Functions.ListViewScrollToTop(this.lv_HomePosts);
                    break;
                case "hot":
                    Functions.ListViewScrollToTop(this.lv_HotPosts);
                    break;
                case "best":
                    Functions.ListViewScrollToTop(this.lv_BestPosts);
                    break;
                case "blogger":
                    Functions.ListViewScrollToTop(this.lv_Bloggers);
                    break;
                case "news":
                    Functions.ListViewScrollToTop(this.lv_News);
                    break;
            }
        }

        #endregion

        private Dictionary<string, TextBlock> dict = new Dictionary<string, TextBlock>();
        private void CreateDictionaryData()
        {
            this.dict.Add("home", this.tb_HomeTag);
            this.dict.Add("hot", this.tb_HotTag);
            this.dict.Add("best", this.tb_BestTag);
            this.dict.Add("blogger", this.tb_BloggerTag);
            this.dict.Add("news", this.tb_NewsTag);
        }

        /// <summary>
        /// when selection of pivot changed, show/hide the up-right corner number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pivot_Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem piCurrent = this.pivot_Main.SelectedItem as PivotItem;
            string currentTag = (string)piCurrent.Tag;
            foreach(KeyValuePair<string,TextBlock> kvp in this.dict)
            {
                if (kvp.Key == currentTag)
                {
                    kvp.Value.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    kvp.Value.Visibility = Windows.UI.Xaml.Visibility.Collapsed;                    
                }
            }
        }

    }
}
