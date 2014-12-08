using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.CloudAPI
{    
    /// <summary>
    /// provide list of top view posts
    /// you can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more
    /// </summary>
    public class TopViewPostsDS : DataSourceBase<Post>
    {
        protected async override Task<IList<DataModel.Post>> LoadItemsAsync()
        {
            _count += 20;

            _lastCount = this.Count;

            //as this api only accept itemcount as parameter, we should remove exist posts
            var result = await APIWrapper.Instance.GetTopTiggPostAsync(_count);

            if (!result.IsSuccess)
            {
                this.FireErrorEvent(result.Code);
            }

            return result.Result == null ? null : result.Result.Entries;
        }

        protected override void AddItems(IList<Post> posts)
        {
            if (posts != null)
            {
                for (var i = _lastCount; i < posts.Count; i++)
                {
                    this.Add(posts[i]);
                }
            }
        }

        private int _lastCount = 0;
        private int _count = 0;
    }
}
