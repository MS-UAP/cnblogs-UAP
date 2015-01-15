using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CNBlogs.DataHelper.Function
{
    public static class FollowHelper
    {
        public static Task<Post> GetLatestFollowedPost()
        {
            throw new NotImplementedException();
        }

        public static async Task Follow(this Author author)
        {
            var destFile = await GetFollowedAuthorFileAsync(author);

            if (destFile != null)
            {
                await Windows.Storage.FileIO.WriteTextAsync(destFile, await Functions.Serialize(author));
            }
        }

        public static async Task UnFollow(this Author author)
        {
            var destFile = await GetFollowedAuthorFileAsync(author);

            if (destFile != null)
            {
                await destFile.DeleteAsync();
            }
        }

        public static async Task<List<Author>> GetFollowedAuthors()
        {
            var result = new List<Author>();

            var roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;

            var destFolder = await roamingFolder.CreateFolderAsync(FOLLOW_AUTHOR_FOLDER, Windows.Storage.CreationCollisionOption.OpenIfExists);

            var files = await destFolder.GetFilesAsync();

            if (files != null)
            {
                foreach (var file in files)
                {
                    var xml = await Windows.Storage.FileIO.ReadTextAsync(file);

                    result.Add(Functions.Deserlialize<Author>(xml));
                }
            }

            return result;
        }


        public static async Task<List<Category>> GetFollowedCategories()
        {
            var result = new List<Category>();

            var roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;

            var destFolder = await roamingFolder.CreateFolderAsync(FOLLOW_CATEGORY_FOLDER, Windows.Storage.CreationCollisionOption.OpenIfExists);

            var files = await destFolder.GetFilesAsync();

            if (files != null)
            {
                foreach (var file in files)
                {
                    var xml = await Windows.Storage.FileIO.ReadTextAsync(file);

                    result.Add(Functions.Deserlialize<Category>(xml));
                }
            }

            return result;
        }
        public static async Task Follow(this Category category)
        {
            var destFile = await GetFollowedCategoryFileAsync(category);

            if (destFile != null)
            {
                await Windows.Storage.FileIO.WriteTextAsync(destFile, await Functions.Serialize(category));
            }
        }

        public static async Task UnFollow(this Category category)
        {
            var destFile = await GetFollowedCategoryFileAsync(category);

            if (destFile != null)
            {
                await destFile.DeleteAsync();
            }
        }

        private static async Task<StorageFile> GetFollowedCategoryFileAsync(Category category)
        {
            if (category != null)
            {
                var roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;

                var destFolder = await roamingFolder.CreateFolderAsync(FOLLOW_CATEGORY_FOLDER, Windows.Storage.CreationCollisionOption.OpenIfExists);

                var destFile = await destFolder.CreateFileAsync(category.Id + ".xml", Windows.Storage.CreationCollisionOption.ReplaceExisting);

                return destFile;
            }
            else
            {
                return null;
            }
        }

        private static async Task<StorageFile> GetFollowedAuthorFileAsync(Author author)
        {
            if (author != null && !string.IsNullOrWhiteSpace(author.Uri) && Uri.IsWellFormedUriString(author.Uri, UriKind.RelativeOrAbsolute))
            {
                var roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;
                var destFolder = await roamingFolder.CreateFolderAsync(FOLLOW_AUTHOR_FOLDER, Windows.Storage.CreationCollisionOption.OpenIfExists);
                string blogApp = string.Empty;
                if (string.IsNullOrWhiteSpace(author.BlogApp))
                {
                    blogApp = Functions.ParseBlogAppFromURL(author.Uri);
                }
                else
                {
                    blogApp = author.BlogApp;
                }
                var destFile = await destFolder.CreateFileAsync(blogApp + ".xml", Windows.Storage.CreationCollisionOption.ReplaceExisting);

                return destFile;
            }
            else
            {
                return null;
            }
        }

        private static readonly string FOLLOW_AUTHOR_FOLDER = "follow_author";
        private static readonly string FOLLOW_CATEGORY_FOLDER = "follow_category";
    }
}
