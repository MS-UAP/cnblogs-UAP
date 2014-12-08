using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using Windows.UI.Xaml.Controls;
using Windows.UI.Notifications;
using NotificationsExtensions.TileContent;

namespace CNBlogs.DataHelper.Helper
{
    public static class Functions
    {
        public static void CreateTile(Post post)
        {
            TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            var tileLarge = TileContentFactory.CreateTileWide310x150PeekImage01();
            tileLarge.Image.Src = "ms-appx:///Assets/WideLogo.scale-240.png";
            string title = post.Title;
            //if (title.Length > 13)
            //{
            //    title = title.Substring(0, 12) + "…";
            //}
            tileLarge.TextBodyWrap.Text = post.Summary;
            tileLarge.TextHeading.Text = title;

            var tileSmall = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
            tileSmall.Image.Src = "ms-appx:///Assets/Logo.scale-240.png";
            tileSmall.TextHeading.Text = post.BlogApp;
            tileSmall.TextBodyWrap.Text = post.Title;

            tileLarge.Square150x150Content = tileSmall;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileLarge.CreateNotification());
        }

        public static double GetWindowsWidth()
        {
            Windows.UI.Xaml.Controls.Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            double Width = rootFrame.ActualWidth;
            return Width;
        }

        /// <summary>
        /// Deserlialize xml to specified type
        /// </summary>
        public static T Deserlialize<T>(string xml)
        {
            T result = default(T);

            if (!string.IsNullOrWhiteSpace(xml))
            {
                var ser = new XmlSerializer(typeof(T));

                using (var reader = new StringReader(xml))
                {
                    result = (T)ser.Deserialize(reader);
                }
            }

            return result;
        }

        /// <summary>
        /// parse string to datetime,  if fail will return DateTime.MinValue
        /// </summary>
        public static DateTime ParseDateTime(string datetime)
        {
            var date = DateTime.MinValue;

            DateTime.TryParse(datetime, out date);

            return date;
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

        public static int ParseInt(string number)
        {
            var result = 0;

            int.TryParse(number, out result);

            return result;
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

        public static void ShowProgressBar(ProgressBar pb)
        {
            pb.IsIndeterminate = true;
            pb.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        public static void HideProgressBar(ProgressBar pb)
        {
            pb.IsIndeterminate = false;
            pb.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

    }
}
