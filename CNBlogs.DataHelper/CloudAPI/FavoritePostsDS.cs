using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CNBlogs.DataHelper.CloudAPI
{
    public class FavoritePostsDS : PostDataSource
    {
        public override async Task Refresh()
        {
            this._loadedFiles.Clear();

            await base.Refresh();
        }

        protected async override Task<IList<DataModel.Post>> LoadItemsAsync()
        {
            var roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;

            var favFolder = await roamingFolder.CreateFolderAsync("fav", Windows.Storage.CreationCollisionOption.OpenIfExists);

            var files = await favFolder.GetFilesAsync();

            var result = new List<Post>();

            var num = 0;

            foreach (var file in files.OrderByDescending(f => f.DateCreated))
            {
                if (!_loadedFiles.ContainsKey(file.Name))
                {
                    _loadedFiles.Add(file.Name, true);

                    var xml = await FileIO.ReadTextAsync(file);

                    result.Add(Functions.Deserlialize<Post>(xml));

                    num++;

                    //TODO: change to a member later
                    if (num >= 20)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private Dictionary<string, bool> _loadedFiles = new Dictionary<string, bool>();
    }
}
