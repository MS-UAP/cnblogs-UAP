using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace CNBlogs
{
    public sealed partial class AboutSettingsFlyout : SettingsFlyout
    {
        public AboutSettingsFlyout()
        {
            this.InitializeComponent();
            this.sp_aboutContent.Opacity = 0;
            this.sb_LogoMoveUp.Begin();
        }

        private async void btn_RateMe_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(string.Format("ms-windows-store:navigate?appid={0}", CurrentApp.AppId));
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

    }
}
