using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CNBlogs
{
    public sealed class AppCommandsCalloutButton : Control
    {
        public AppCommandsCalloutButton()
        {
            this.DefaultStyleKey = typeof(AppCommandsCalloutButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            GetCalloutButton();
            SetCalloutButtonFunction();
        }

        private void SetCalloutButtonFunction()
        {
            if(calloutButton!=null)
            {
                calloutButton.Tapped += calloutButton_Tapped;
            }
        }

        void calloutButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (calloutButton != null)
                {
                    object parent = VisualTreeHelper.GetParent(calloutButton);
                    while (!(parent is Page || parent is FlatNavigationPage))
                        if (parent != null)
                            parent = VisualTreeHelper.GetParent(parent as DependencyObject);
                        else break;
                    if (parent != null)
                    {
                        Page page = parent as Page;
                        if (page != null)
                        {
                            if (page.TopAppBar != null)
                            {
                                page.TopAppBar.IsOpen = true;
                            }
                            if (page.BottomAppBar != null)
                            {
                                page.BottomAppBar.IsOpen = true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void GetCalloutButton()
        {
            if (calloutButton == null)
            {
                this.calloutButton = this.GetTemplateChild("calloutButton") as HyperlinkButton;
            }
        }

        HyperlinkButton calloutButton;


    }
}
