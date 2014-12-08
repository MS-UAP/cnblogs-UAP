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


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : FlatNavigationPage
    {
        private LatestPostsDS recentDS = null;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            LoadData();
        }

        private void LoadData()
        {
            this.recentDS = new LatestPostsDS();
            this.gv_HomePosts.ItemsSource = recentDS;
            this.DataContext = this.recentDS;
        }

        private void gv_HomePosts_ItemClick(object sender, ItemClickEventArgs e)
        {
            Post post = e.ClickedItem as Post;
            this.Frame.Navigate(typeof(Pages.ReadingPage), post);
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (this.recentDS != null)
            {
                await this.recentDS.Refresh();
            }
        }

        private void btn_ScrollToTop_Click(object sender, RoutedEventArgs e)
        {
            CNBlogs.DataHelper.Helper.Functions.GridViewScrollToTop(this.gv_HomePosts);
        }
    }
}
