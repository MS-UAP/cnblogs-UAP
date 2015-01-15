using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CNBlogs.DataHelper.CloudAPI
{
    public class FavoritePostDS : PostDataSource
    {
        private readonly static FavoritePostDS _instance = new FavoritePostDS();

        public static FavoritePostDS Instance { get { return _instance; } }

        public string Icon { get; set; }
        public string Title { get; set; }

        public FavoritePostDS()
        {
            this.Icon = "";
            this.Title = Functions.LoadResourceString("FavoriteBlogText");
        }

        public async Task AddFavPost(Post post)
        {
            if (!this.Any(p => p.ID == post.ID))
            {
                await post.AsFavorite();

                this.Insert(0, post);
            }
        }

        public async Task RemoveFav(Post post)
        {
            for (var i = 0; i < this.Count; i++)
            {
                if (this[i].ID == post.ID)
                {
                    this.RemoveAt(i);
                    break;
                }
            }

            await post.UnFavorite();
        }

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

            foreach (var file in files.Where(f=>f.FileType==".xml").OrderByDescending(f => f.DateCreated))
            {
                if (!_loadedFiles.ContainsKey(file.Name))
                {
                    //var properties = await file.GetBasicPropertiesAsync();
                    //System.Diagnostics.Debug.WriteLine("file {0}:{1}", file.DisplayName, properties.Size);

                    _loadedFiles.Add(file.Name, true);

                    var xml = await FileIO.ReadTextAsync(file);
                    var newItem = Functions.Deserlialize<Post>(xml);
                    newItem.Status = PostStatus.Favorite;
                    result.Add(newItem);

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
