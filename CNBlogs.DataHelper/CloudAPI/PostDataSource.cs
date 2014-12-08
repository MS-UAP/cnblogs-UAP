using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.CloudAPI
{
    public abstract class PostDataSource : DataSourceBase<Post>
    {
        protected void TrimPost(IList<Post> posts)
        {
            if (posts != null)
            {
                foreach (var post in posts)
                {
                    post.Summary = post.Summary.Trim();

                    // title and summary may contains special html code, convert them
                    post.Title = Windows.Data.Html.HtmlUtilities.ConvertToText(post.Title);
                    post.Summary = Windows.Data.Html.HtmlUtilities.ConvertToText(post.Summary);
                }
            }
        }

        protected override void AddItems(IList<Post> items)
        {
            if (items != null)
            {
                TrimPost(items);
            }

            base.AddItems(items);
        }
    }
}
