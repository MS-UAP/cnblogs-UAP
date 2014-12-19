using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Data.Xml.Dom;
using CNBlogs.DataHelper.DataModel;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using System.Threading;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;

namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// provide comment of post
    /// you can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more
    /// </summary>
    public class CommentsDS : DataSourceBase<Comment>
    {
        /// <summary>
        /// create a new instance of CommentsDS
        /// </summary>
        /// <param name="parentId">post id or news id</param>
        /// <param name="category">
        /// this used to specified comment category, "blog" for post, "news" for news
        /// 
        /// TODO: use enum instead later
        /// </param>
        public CommentsDS(string parentId, string category = "blog")
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                throw new ArgumentException("parentId");
            }

            this._parentId = parentId;
            this._category = category;
        }

        protected async override Task<IList<Comment>> LoadItemsAsync()
        {
            var result = await APIWrapper.Instance.GetCommentsAsync(_category, _parentId, _currentPage);

            if (!result.IsSuccess)
            {
                FireErrorEvent(result.Code);
            }
            else if (result.Result != null && result.Result.Entries != null)
            {
                foreach (var comment in result.Result.Entries)
                {
                    if (comment != null)
                    {
                        if (!string.IsNullOrEmpty(comment.Content))
                        {
                            // convert html content to string
                            comment.Content = Windows.Data.Html.HtmlUtilities.ConvertToText(comment.Content);
                        }

                        if (comment.Author != null && !string.IsNullOrEmpty(comment.Author.Name))
                        {
                            // name of some authors contains special characters, convert it
                            comment.Author.Name = Windows.Data.Html.HtmlUtilities.ConvertToText(comment.Author.Name);
                        }
                    }
                }
            }

            return result.Result == null ? null : result.Result.Entries;
        }

        protected override void AddItems(IList<Comment> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (!this.Any(it => it.ID == item.ID))
                    {
                        this.Add(item);
                    }
                }
            }
        }

        private string _category = string.Empty;
        private string _parentId = string.Empty;

    }
}
