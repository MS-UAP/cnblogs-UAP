using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// TODO: now we are using category rss to retrieve post, but it only provide 20 items per day, later should use category page instead
    /// </summary>
    public class CategoryPostDS : PostDataSource
    {
        public Category Category { get; private set; }

        public CategoryPostDS(Category cateogry)
        {
            // check category 
            if (cateogry == null)
            {
                throw new ArgumentNullException("category");
            }

            if (!Uri.IsWellFormedUriString(cateogry.Href, UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("href of category");
            }

            this.Category = cateogry;
        }

        protected override async Task<IList<Post>> LoadItemsAsync()
        {
            // TODO: now we are using rss source to get post for category, and it only return 20 post,
            // so we will not load more except first time
            if (!_firstTime)
            {
                return null;
            }

            var result = await APIWrapper.Instance.GetPostsByCategoryRSSAsync(Category.Href);

            if (!result.IsSuccess)
            {
                FireErrorEvent(result.Code);
            }
            else
            {
                // parse id from url
                foreach (var post in result.Result.Entries)
                {
                    if (Uri.IsWellFormedUriString(post.ID, UriKind.RelativeOrAbsolute))
                    {
                        var id = System.IO.Path.GetFileNameWithoutExtension(post.ID);
                        post.ID = id;
                    }
                }

                _firstTime = false;
            }

            return result.Result == null ? null : result.Result.Entries;
        }

        public override async Task Refresh()
        {
            this._firstTime = true;

            await base.Refresh();
        }

        private bool _firstTime = true;
    }
}
