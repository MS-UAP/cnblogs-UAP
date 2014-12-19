using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CNBlogs.DataHelper.Helper
{
    public static class PostHelper
    {
        /// <summary>
        /// mark the post as favorite and save the status
        /// </summary>
        public static async Task AsFavorite(this Post post)
        {
            if (post != null && !post.Status.HasFlag(PostStatus.Favorite))
            {
                // remove None first
                post.Status &= ~PostStatus.None;

                //mark as favorite and read
                // changed by xiaowu
                //post.Status |= PostStatus.Read;
                //post.Status |= PostStatus.Favorite;
                post.Status = PostStatus.Favorite;

                CNBlogSettings.Instance.SaveBlogStatus(post);

                await SaveFav(post);
            }
        }

        /// <summary>
        /// delete post file from fav folder, and save the status
        /// </summary>
        public static async Task UnFavorite(this Post post)
        {
            if (post != null && post.Status.HasFlag(PostStatus.Favorite))
            {
                // remove favorite
                //post.Status &= ~PostStatus.Favorite;
                post.Status = PostStatus.Read;
                CNBlogSettings.Instance.SaveBlogStatus(post);
                await DeleteFavPost(post);
            }
        }

        /// <summary>
        /// mark blog/news as read, and save the status
        /// </summary>
        /// <param name="entry"></param>
        public static void AsRead(this EntryBase entry)
        {
            if (entry != null && !entry.Status.HasFlag(PostStatus.Read))
            {
                entry.Status &= ~PostStatus.None;
                entry.Status |= PostStatus.Read;

                if (entry is Post)
                {
                    CNBlogSettings.Instance.SaveBlogStatus(entry as Post);
                }
                else if (entry is News)
                {
                    CNBlogSettings.Instance.SaveNewsStatus(entry as News);
                }
            }
        }


        /// <summary>
        /// save post to fav folder
        /// </summary>
        public async static Task SaveFav(Post post)
        {
            var favFolder = await GetFavFolder();
            var filename = CombinePostFileName(post.ID);

            // get/overwrite post file
            var postFile = await favFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            var xml = await Functions.Serialize(post);

            await FileIO.WriteTextAsync(postFile, xml);
        }

        /// <summary>
        /// delete post from fav folder
        /// </summary>
        public async static Task DeleteFavPost(Post post)
        {
            var favFolder = await GetFavFolder();
            var filename = CombinePostFileName(post.ID);

            // get/overwrite post file
            var postFile = await favFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);

            await postFile.DeleteAsync();
        }

        /// <summary>
        /// is the post marked as favorite?
        /// </summary>
        public async static Task<bool> IsPostFavorite(string id)
        {
            var favFolder = await GetFavFolder();
            var filename = CombinePostFileName(id);

            return await Functions.IsFileExist(favFolder, filename);
        }

        /// <summary>
        /// load category info from local file
        /// </summary>
        /// <returns></returns>
        public async static Task<List<Category>> GetCategories()
        {
            var categoriesContent = await FileIO.ReadTextAsync(await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///SampleData/categories.xml")));

            var collection = Functions.Deserlialize<CategoryCollection>(categoriesContent);

            return collection.Categories;
        }

        private static string CombinePostFileName(string id)
        {
            return id + ".xml";
        }

        private static async Task<StorageFolder> GetFavFolder()
        {
            var roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;

            // get/create fav folder
            return await roamingFolder.CreateFolderAsync("fav", CreationCollisionOption.OpenIfExists);
        }
    }
}
