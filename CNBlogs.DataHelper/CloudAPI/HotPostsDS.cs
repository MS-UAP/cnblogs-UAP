using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// provide the top tigg posts
    /// you can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more
    /// </summary>
    public class HotPostsDS : PostDataSource
    {
        protected async override Task<IList<DataModel.Post>> LoadItemsAsync()
        {
            _count += 10;

            _lastCount = this.Count;

            //as this api only accept itemcount as parameter, we should remove exist posts
            var result = await APIWrapper.Instance.GetHotPostsAsync(_count);

            if (!result.IsSuccess)
            {
                FireErrorEvent(result.Code);
            }

            return result.Result == null ? null : result.Result.Entries;
        }

        protected override void AddItems(IList<Post> posts)
        {
            if (posts != null)
            {
                TrimPost(posts);

                var newPostsList = new List<Post>();

                for (var i = _lastCount; i < posts.Count; i++)
                {
                    newPostsList.Add(posts[i]);
                }

                base.AddItems(newPostsList);
            }
        }

        private int _lastCount = 0;
        private int _count = 0;
    }
}
