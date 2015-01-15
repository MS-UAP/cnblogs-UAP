using CNBlogs.DataHelper.CloudAPI;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Function;
using System;
using System.ComponentModel;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls.Primitives;
using System.Threading.Tasks;
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
        private NewsDS newsDs;
        private List<Column> listColumn;
        private FavoriteGroup fg_Author, fg_Category, fg_Blog;

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
            //ThreadPoolTimer.CreateTimer(SplashTimeOut, new TimeSpan(0, 0, 2));

            this.recentDS = new LatestPostsDS();
            this.recentDS.DataRequestError += recentDS_DataRequestError;
            this.recentDS.OnLoadMoreStarted += newsDs_OnLoadMoreStarted;
            this.recentDS.OnLoadMoreCompleted += newsDs_OnLoadMoreCompleted;
            this.lv_HomePosts.ItemsSource = this.recentDS;
            this.tn_Home.DataContext = this.recentDS;
            await this.recentDS.LoadMoreItemsAsync(20);

            this.newsDs = new NewsDS();
            this.newsDs.DataRequestError += recentDS_DataRequestError;
            //this.newsDs.OnLoadMoreStarted += newsDs_OnLoadMoreStarted;
            //this.newsDs.OnLoadMoreCompleted += newsDs_OnLoadMoreCompleted;
            this.lv_News.ItemsSource = this.newsDs;
            this.tn_News.DataContext = this.newsDs;
            await this.newsDs.LoadMoreItemsAsync(20);

            this.listColumn = await DataHelper.Function.PostHelper.GetColumns();
            this.lv_Column.ItemsSource = this.listColumn;
            this.tn_Column.DataContext = this.listColumn;

            await FavoriteCategoryDS.Instance.LoadData();
            this.fgc_Category.DataContext = FavoriteCategoryDS.Instance;
            this.lv_category.ItemsSource = FavoriteCategoryDS.Instance.Items;
            this.fg_Category = new FavoriteGroup(this.lv_category);

            await FavoriteAuthorDS.Instance.LoadData();
            this.fgc_Author.DataContext = FavoriteAuthorDS.Instance;
            this.lv_author.ItemsSource = FavoriteAuthorDS.Instance.Items;
            this.fg_Author = new FavoriteGroup(this.lv_author);

            this.lv_blog.ItemsSource = FavoritePostDS.Instance;
            await FavoritePostDS.Instance.LoadMoreItemsAsync(20);
            this.fgc_Post.DataContext = FavoritePostDS.Instance;
            this.fg_Blog = new FavoriteGroup(this.lv_blog);

            // check updates for first time
            var updateTasks = new List<Task>(2);

            updateTasks.Add(FavoriteAuthorDS.Instance.CheckUpdateForAll());
            updateTasks.Add(FavoriteCategoryDS.Instance.CheckUpdateForAll());

            //this.recentDS_OnLoadMoreCompleted(20);

            // we do not need to wait for the result, let's update UI when the tasks finished
           await Task.Run(async () =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    await FavoriteAuthorDS.Instance.CheckUpdateForAll();
                    await FavoriteCategoryDS.Instance.CheckUpdateForAll();
                });
            });

            // with DispatcherTimer, we can update UI without using Dispatcher.RunAsync
            var timer = new DispatcherTimer();

            //check for update every 15 mins
            timer.Interval = TimeSpan.FromMinutes(15);
            timer.Tick += timer_Tick;

            // start the timer
            timer.Start();

            FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.cmdBar);
        }

        private async void newsDs_OnLoadMoreStarted(uint count)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, this.cmdBar);
            });
        }

        private async void newsDs_OnLoadMoreCompleted(int count)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.grid_Splash.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.grid_Main.Visibility = Windows.UI.Xaml.Visibility.Visible;
                FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.cmdBar);
            });
        }

        private async void timer_Tick(object sender, object e)
        {
            await FavoriteCategoryDS.Instance.CheckUpdateForAll();
            await FavoriteAuthorDS.Instance.CheckUpdateForAll();

            System.Diagnostics.Debug.WriteLine("timer tick.");
        }

        //private async void recentDS_OnLoadMoreStarted(uint count)
        //{
        //    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //    {
        //        FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, this.cmdBar);
        //    });
        //}

        //private async void recentDS_OnLoadMoreCompleted(int count)
        //{
        //    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //    {
        //        this.grid_Splash.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //        this.grid_Main.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //        FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.cmdBar);
        //    });
        //}

        private void recentDS_DataRequestError(int code)
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
            if (e.NavigationMode == NavigationMode.New)
            {
                Logger.LogAgent.GetInstance().WriteLog(this.GetType().ToString());
            }

            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
            this.navigationHelper = new Common.NavigationHelper(this);
            this.btn_NightMode.DataContext = CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance;
        }

        #region Today/Hot pivot page

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
                this.lv_HomePosts.ScrollIntoView(sender, ScrollIntoViewAlignment.Leading);
            }
        }

        #endregion

        #region News pivot page

        private void lv_News_ItemClick(object sender, ItemClickEventArgs e)
        {
            News news = e.ClickedItem as News;
            this.Frame.Navigate(typeof(NewsReadingPage), news);
        }

        #endregion

        #region bottom app bar

        private void btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, this.cmdBar);

            await this.recentDS.Refresh();
            await this.newsDs.Refresh();
            this.timer_Tick(sender, e);

            FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, this.cmdBar);
        }

        private void btn_Top_Click(object sender, RoutedEventArgs e)
        {
            PivotItem pi = this.pivot_Main.SelectedItem as PivotItem;
            switch ((string)pi.Tag)
            {
                case "home":
                    FunctionHelper.Functions.ListViewScrollToTop(this.lv_HomePosts);
                    break;

                case "news":
                    FunctionHelper.Functions.ListViewScrollToTop(this.lv_News);
                    break;

                case "favorite":
                    this.sv_Favorite.ChangeView(0, 0, null, true);
                    break;
            }
        }

        private void btn_NightMode_Click(object sender, RoutedEventArgs e)
        {
            FunctionHelper.Functions.btn_NightMode_Click(this);
        }


        #endregion

        #region Pivot change
        private Dictionary<string, TitleWithNumberControl> dict = new Dictionary<string, TitleWithNumberControl>();
        private void CreateDictionaryData()
        {
            this.dict.Add("home", this.tn_Home);
            this.dict.Add("news", this.tn_News);
            this.dict.Add("column", this.tn_Column);
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
            foreach (KeyValuePair<string, TitleWithNumberControl> kvp in this.dict)
            {
                if (kvp.Key == currentTag)
                {
                    kvp.Value.ShowNumber();
                }
                else
                {
                    kvp.Value.HideNumber();
                }
            }
        }
        #endregion

        #region Column pivot page
        private void lv_Column_ItemClick(object sender, ItemClickEventArgs e)
        {
            Column column = e.ClickedItem as Column;
            Type pageType = Type.GetType(column.Page);
            if (pageType != null)
            {
                this.Frame.Navigate(pageType);
            }
        }
        #endregion

        #region Favorite pivot page



        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuFlyoutItem;

            if (menu != null)
            {
                if (menu.DataContext is FavoriteItem<Category>)
                {
                    var context = menu.DataContext as FavoriteItem<Category>;

                    if (context != null)
                    {
                        await FavoriteCategoryDS.Instance.Remove(context);
                    }
                }
                else if (menu.DataContext is FavoriteItem<Author>)
                {
                    var context = menu.DataContext as FavoriteItem<Author>;

                    if (context != null)
                    {
                        await FavoriteAuthorDS.Instance.Remove(context);
                    }
                }
                else if (menu.DataContext is Post)
                {
                    var context = menu.DataContext as Post;

                    if (context != null)
                    {
                        await FavoritePostDS.Instance.RemoveFav(context);
                    }
                }
            }
        }

        private void sp_category_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.fg_Category.Tapped();
        }

        private void sp_author_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.fg_Author.Tapped();
        }

        private void sp_blog_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.fg_Blog.Tapped();
        }

        private async void lv_category_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FavoriteItem<Category>;
            if (item != null)
            {
                // set as not new post
                item.HasNew = false;

                var destCate = await PostHelper.GetCategory(item.Item.Id);

                if (destCate != null)
                {
                    Frame.Navigate(typeof(CategoryPostsPage), destCate);
                }
            }
        }

        private void lv_author_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FavoriteItem<Author>;
            if (item != null)
            {
                // set as not new post
                item.HasNew = false;

                var blogApp = Functions.ParseBlogAppFromURL(item.Item.Uri);
                if (!string.IsNullOrWhiteSpace(blogApp))
                {
                    Frame.Navigate(typeof(BloggerPage), new Blogger { Avatar = item.Item.Avatar, Name = item.Item.Name, BlogApp = blogApp });
                }
            }
        }


        private void author_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var target = sender as FavoriteAuthorControl;
            if (target != null)
            {
                FlyoutBase.ShowAttachedFlyout(target);
            }
        }

        private void category_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var target = sender as FavoriteCategoryControl;
            if (target != null)
            {
                FlyoutBase.ShowAttachedFlyout(target);
            }
        }

        private void post_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var target = sender as PostControl;
            if (target != null)
            {
                FlyoutBase.ShowAttachedFlyout(target);
            }

        }

        #endregion

    }
}
