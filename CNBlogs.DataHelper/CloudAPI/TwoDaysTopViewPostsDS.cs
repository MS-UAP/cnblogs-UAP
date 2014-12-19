using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// provide list of top view posts
    /// you can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more
    /// </summary>
    public class TenDaysTopLikePostsDS : PostDataSource
    {
        public override async Task Refresh()
        {
            this._count = 0;
            this.Clear();
            await this.LoadMoreItemsAsync(20);
        }

        protected override async Task<IList<Post>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count)
        {
            if (IsInTime())
            {
                return null;
            }

            var newItems = await this.LoadItemsAsync();

            //update page state
            if (newItems != null)
            {
                _count += _increaseCount;
            }

            //as cnblog does not provide total of items, so we check if there is any new post
            //this._hasMoreItems = newItems != null && newItems.Count > 0;

            return newItems;
        }

        protected async override Task<IList<DataModel.Post>> LoadItemsAsync()
        {
            _lastCount = this.Count;
            var nextCount = _count + _increaseCount;

            //as this api only accept itemcount as parameter, we should remove exist posts
            var result = await APIWrapper.Instance.GetTopTiggPostAsync(_count + _increaseCount);

            if (!result.IsSuccess)
            {
                this.FireErrorEvent(result.Code);
            }
            else
            {
                if (result.Result != null && result.Result.Entries != null)
                {
                    this._hasMoreItems = nextCount == result.Result.Entries.Count;
                }
            }

            return result.Result == null ? null : result.Result.Entries;
        }

        protected override void AddItems(IList<Post> posts)
        {
            if (posts != null)
            {
                TrimPost(posts);

                var newPostList = new List<Post>();

                for (var i = _lastCount; i < posts.Count; i++)
                {
                    this.Add(posts[i]);
                }

                base.AddItems(newPostList);
            }
        }
        private int _increaseCount = 20;

        private int _lastCount = 0;
        private int _count = 0;
    }
}
