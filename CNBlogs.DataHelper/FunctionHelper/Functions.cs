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
using Windows.Storage;

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
            tileLarge.Image.Src = "ms-appx:///Assets/TileUpdateBG_Wide_310x150.png";
            string title = post.Title;

            tileLarge.TextBodyWrap.Text = post.Summary;
            tileLarge.TextHeading.Text = title;

            var tileSmall = TileContentFactory.CreateTileSquare150x150PeekImageAndText02();
            tileSmall.Image.Src = "ms-appx:///Assets/TileUpdateBG_Square_150x150.png";
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

        public static async Task<string> Serialize<T>(T obj)
        {
            var result = string.Empty;

            if (obj != null)
            {
                var ser = new XmlSerializer(typeof(T));

                using (var writer = new StringWriter())
                {
                    ser.Serialize(writer, obj);

                    await writer.FlushAsync();

                    result = writer.GetStringBuilder().ToString();
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


        public static int ParseInt(string number)
        {
            var result = 0;

            int.TryParse(number, out result);

            return result;
        }

        /// <summary>
        /// check if there is a file under specified folder
        /// </summary>
        /// <param name="folder">folder to check</param>
        /// <param name="filename">file name under folder</param>
        public async static Task<bool> IsFileExist(StorageFolder folder, string filename)
        {
            var fullpath = Path.Combine(folder.Path, filename);
            return await IsFileExist(fullpath);
        }

        /// <summary>
        /// is the file full path exist
        /// </summary>
        public async static Task<bool> IsFileExist(string path)
        {
            try
            {
                await StorageFile.GetFileFromPathAsync(path);
                return true;
            }
            catch (FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("File not exist: {0}", path);
            }

            return false;
        }
        public static void GridViewScrollToTop(GridView gv)
        {
            var item0 = gv.Items[0];
            gv.ScrollIntoView(item0, ScrollIntoViewAlignment.Leading);
        }
    }
}
