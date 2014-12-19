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
using Windows.ApplicationModel.Store;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private Common.NavigationHelper navigationHelper;
        CNBlogs.DataHelper.DataModel.CNBlogSettings settings = CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance;

        public SettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = settings;
            this.navigationHelper = new Common.NavigationHelper(this);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        private async void btn_RateMe_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(string.Format("ms-windows-store:navigate?appid={0}", CurrentApp.AppId));
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pi = pivot_Main.SelectedItem as PivotItem;
            if (pi.Tag.ToString() == "about")
            {
                this.sp_aboutContent.Opacity = 0;
                this.sb_LogoMoveUp.Begin();
            }
        }

        /// <summary>
        /// because we set 'TwoWay' mode in xaml, so don't change Settings.Instance.NightModeTheme value here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ts_LightMode_Toggled(object sender, RoutedEventArgs e)
        {
            if (CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance.NightModeTheme)
            {
                this.RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                this.RequestedTheme = ElementTheme.Light;
            }
        }
    }
}
