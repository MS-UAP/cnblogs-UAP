using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Data.Xml.Dom;
using CNBlogs.DataHelper.DataModel;
using System.Collections.ObjectModel;


namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// in this ds, we will load 100 authors, and pick 10 random
    /// </summary>
    public class RecommendAuthorsDS
    {
        public ObservableCollection<Blogger> Bloggers = new ObservableCollection<Blogger>();

        public async Task<bool> Refresh()
        {
            this.Bloggers.Clear();
            return await this.LoadData();
        }

        public async Task<bool> LoadRemoteData()
        {
            return await this.LoadData();
        }

        private async Task<bool> LoadData()
        {
            var feed = await APIWrapper.Instance.GetRecommandBloggersAsync(1, 100);

            if (feed != null && feed.Entries != null)
            {
                var ran = new Random();

                var existList = new List<int>();
                
                for (var i = 0; i < 10; i++)
                {
                    var index = ran.Next(0, feed.Entries.Count - 1);

                    if (existList.Contains(index))
                    {
                        i--;
                        continue;
                    }

                    existList.Add(index);

                    this.Bloggers.Add(feed.Entries[index]);
                }
            }

            // null means there is an error when request
            return this.Bloggers.Count > 0;
        }
    }
}
