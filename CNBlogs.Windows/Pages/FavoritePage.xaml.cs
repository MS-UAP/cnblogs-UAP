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
using Windows.UI.Popups;
using CNBlogs.DataHelper.CloudAPI;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Function;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CNBlogs.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FavoritePage : FlatNavigationPage
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


        public FavoritePage()
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
            this.Posts.Source = FavoritePostDS.Instance;
            this.Categories.Source = FavoriteCategoryDS.Instance.Items;
            this.Bloggers.Source = FavoriteAuthorDS.Instance.Items;
            await DataRefresh();
            TitleControl.DataContext = null;
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
            if (e.NavigationMode == NavigationMode.New)
            {
                Logger.LogAgent.GetInstance().WriteLog(this.GetType().ToString());
            }
            Frame.BackStack.Clear();
            //await DataRefresh();
        }

        private async System.Threading.Tasks.Task DataRefresh()
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                  {

                      FavoriteCategoryDS.Instance.Refresh();
                      FavoriteAuthorDS.Instance.Refresh();
                      FavoritePostDS.Instance.Refresh();
                  });
            }
            catch (Exception)
            {

            }
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
        private void gv_Bloggers_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Author author = (e.ClickedItem as FavoriteItem<Author>).Item;
                this.Frame.Navigate(typeof(BloggerPage),
                    new Blogger()
                    {
                        Avatar = author.Avatar,
                        BlogApp = author.Uri.Trim('/').Split('/').Last(),
                        Name = author.Name,
                    });
            }
            catch (Exception)
            {

            }
        }
        private void gv_Category_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Category category = (e.ClickedItem as FavoriteItem<Category>).Item;
                this.Frame.Navigate(typeof(CategoryPostsPage), category);
            }
            catch (Exception)
            {

            }
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            await DataRefresh();
        }

        private void BloggerControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Author author = ((sender as BloggerControl).DataContext as FavoriteItem<Author>).Item;
                this.Frame.Navigate(typeof(BloggerPage),
                    new Blogger()
                    {
                        Avatar = author.Avatar,
                        BlogApp = author.Uri.Trim('/').Split('/').Last(),
                        Name = author.Name,
                    });
            }
            catch (Exception)
            {

            }
        }

        private void SubCategoryControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Category category = ((sender as CategoryControl).DataContext as FavoriteItem<Category>).Item;
                this.Frame.Navigate(typeof(CategoryPostsPage), category);
            }
            catch (Exception)
            {

            }
        }


        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        private async void CategoryControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            PopupMenu favoriteContextMenu = new PopupMenu();
            favoriteContextMenu.Commands.Add(new UICommand(loader.GetString("ContextButton_Unfavorite"), (command) =>
            {
                var dataContext = (sender as Control).DataContext;
                if (dataContext is FavoriteItem<Category>)
                {
                    var context = dataContext as FavoriteItem<Category>;
                    if (context != null)
                    {
                        FavoriteCategoryDS.Instance.Remove(context);
                    }
                }
                else if (dataContext is FavoriteItem<Author>)
                {
                    var context = dataContext as FavoriteItem<Author>;
                    if (context != null)
                    {
                        FavoriteAuthorDS.Instance.Remove(context);
                    }
                }
                else if (dataContext is Post)
                {
                    var context = dataContext as Post;
                    if (context != null)
                    {
                        FavoritePostDS.Instance.RemoveFav(context);
                    }
                }
            }));
           await favoriteContextMenu.ShowAsync((sender as Control).TransformToVisual(Window.Current.Content).TransformPoint(new Point(50,50)));
        }

        private void btn_GotoCategory_Click(object sender, RoutedEventArgs e)
        {
            FavoriteHub.ScrollToSection(FavoriteHub.Sections[0]);
        }

        private void btn_GotoBlogger_Click(object sender, RoutedEventArgs e)
        {
            FavoriteHub.ScrollToSection(FavoriteHub.Sections[1]);
        }

        private void btn_GotoPost_Click(object sender, RoutedEventArgs e)
        {
            FavoriteHub.ScrollToSection(FavoriteHub.Sections[2]);
        }

        private void gv_Category_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (FavoriteCategoryDS.Instance.Count == 0)
                FavoriteCategory.Visibility = Visibility.Collapsed;
            else
                FavoriteCategory.Visibility = Visibility.Visible;
        }
    }
}
