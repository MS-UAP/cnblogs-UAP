using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace CNBlogs.FunctionHelper
{
    public static class Functions
    {
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
            var item0 = lv.Items[0];
            lv.ScrollIntoView(item0, ScrollIntoViewAlignment.Leading);
        }

        public static void GridViewScrollToTop(GridView gv)
        {
            var item0 = gv.Items[0];
            gv.ScrollIntoView(item0, ScrollIntoViewAlignment.Leading);
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
