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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CNBlogs
{
    public sealed partial class FlatNavigationControl : UserControl
    {
        public FlatNavigationControl()
        {
            this.InitializeComponent();
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Frame rootFrame = Window.Current.Content as Frame;

            if (b != null && b.Tag != null)
            {
                Type pageType = Type.GetType(b.Tag.ToString());

                // Make sure the page type exists, but don't navigate to it if it's already the current page.
                if (pageType != null && rootFrame.CurrentSourcePageType != pageType)
                {
                    rootFrame.Navigate(pageType);
                    //FlatNavigationPage.Current.TopAppBar.IsOpen = true;
                }
                else if (pageType == null)
                {
                    // TODO: Optional - Do something if page not found.
                }
            }
        }
    }
}
