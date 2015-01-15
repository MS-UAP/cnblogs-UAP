using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CNBlogs.FunctionHelper
{
    public static class Functions
    {
        static Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        public static string LoadResourceString(string resourceName)
        {
            string str = loader.GetString(resourceName);
            return str;
        }


        public static string GetVersionString()
        {
            string appVersion = string.Format("{0}.{1}.{2}.{3}",
                Package.Current.Id.Version.Major,
                Package.Current.Id.Version.Minor,
                Package.Current.Id.Version.Build,
                Package.Current.Id.Version.Revision);
            return appVersion;
        }

        public static void btn_NightMode_Click(Page page)
        {
            CNBlogs.DataHelper.DataModel.CNBlogSettings setting = CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance;
            if (setting.NightModeTheme)    // true = night mode
            {
                page.RequestedTheme = ElementTheme.Light;
                setting.NightModeTheme = false;
            }
            else // false = day mode
            {
                page.RequestedTheme = ElementTheme.Dark;
                setting.NightModeTheme = true;
            }
        }

        public static void SetTheme(Page page)
        {
            if (CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance.NightModeTheme)
            {
                page.RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                page.RequestedTheme = ElementTheme.Light;
            }
        }

        public static void RefreshUIOnDataLoading(ProgressBar pb, CommandBar cb)
        {
            if (cb != null)
            {
                cb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            if (pb != null)
            {
                pb.IsIndeterminate = true;
                pb.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        public static void RefreshUIOnDataLoaded(ProgressBar pb, CommandBar cb)
        {
            if (cb != null)
            {
                cb.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            if (pb != null)
            {
                pb.IsIndeterminate = false;
                pb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        public static void ListViewScrollToTop(ListView lv)
        {
            if (lv.Items.Count > 0)
            {
                var item0 = lv.Items[0];
                lv.ScrollIntoView(item0, ScrollIntoViewAlignment.Leading);
            }
        }

        public static void GridViewScrollToTop(GridView gv)
        {
            if (gv.Items.Count > 0)
            {
                var item0 = gv.Items[0];
                gv.ScrollIntoView(item0, ScrollIntoViewAlignment.Leading);
            }
        }

        public static ScrollViewer GetScrollViewer(Windows.UI.Xaml.DependencyObject depObj)
        {
            if (depObj is ScrollViewer)
            {
                return depObj as ScrollViewer;
            }

            for (int i = 0; i < Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
