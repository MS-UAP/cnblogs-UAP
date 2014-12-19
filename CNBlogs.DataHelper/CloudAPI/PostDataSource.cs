using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Helper;
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
                    if (post != null)
                    {
                        if (!string.IsNullOrEmpty(post.Summary))
                        {
                            post.Summary = post.Summary.Trim();
                            post.Summary = Windows.Data.Html.HtmlUtilities.ConvertToText(post.Summary);
                        }

                        if (!string.IsNullOrEmpty(post.Title))
                        {
                            // title and summary may contains special html code, convert them
                            post.Title = Windows.Data.Html.HtmlUtilities.ConvertToText(post.Title);
                        }

                        if (post.Author != null && !string.IsNullOrEmpty(post.Author.Name))
                        {
                            post.Author.Name = Windows.Data.Html.HtmlUtilities.ConvertToText(post.Author.Name);
                        }

                        // check blog status
                        post.Status = CNBlogSettings.Instance.GetBlogReadState(post.ID);

                        // check if author blogapp is empty
                        if (string.IsNullOrWhiteSpace(post.BlogApp) && post.Author != null && !string.IsNullOrWhiteSpace(post.Author.Uri))
                        {
                            var uri = new Uri(post.Author.Uri);

                            post.BlogApp = uri.LocalPath.Trim(new[] { '/' });
                        }
                    }
                }
            }
        }

        protected override void AddItems(IList<Post> items)
        {
            if (items != null)
            {
                TrimPost(items);

                foreach (var item in items)
                {
                    if (!this.Any(it => it.ID == item.ID))
                    {
                        this.Add(item);
                    }
                }
            }
        }
    }
}
