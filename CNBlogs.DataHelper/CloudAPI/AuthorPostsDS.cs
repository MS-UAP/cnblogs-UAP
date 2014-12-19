using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Xaml.Data;

namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// provide posts of author
    /// you can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more
    /// </summary>
    public class AuthorPostsDS : PostDataSource
    {
        public BloggerFeed Feed { get; private set; }

        public AuthorPostsDS(string authorId)
        {
            if (string.IsNullOrWhiteSpace(authorId))
            {
                throw new ArgumentException("author id");
            }

            _authorId = authorId;
        }

        protected async override Task<IList<DataModel.Post>> LoadItemsAsync()
        {
            var result = await APIWrapper.Instance.GetAuthorPostsAsync(_authorId, _currentPage);

            if (result.Result != null)
            {
                this.Feed = result.Result;
            }
            else
            {
                if (this.Feed == null)
                {
                    this.Feed = new BloggerFeed() { Entries = new List<Post>(), PostCount = 0 };
                }
            }

            if (!result.IsSuccess)
            {
                FireErrorEvent(result.Code);
            }

            return result.Result == null ? null : result.Result.Entries;
        }

        private string _authorId = string.Empty;
    }
}
