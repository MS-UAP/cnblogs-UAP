using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Data;

namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// provide list of bloggers
    /// you can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more
    /// </summary>
    public class NewsDS : DataSourceBase<News>
    {
        protected async override Task<IList<News>> LoadItemsAsync()
        {
            var result = await APIWrapper.Instance.GetRecentNewsAsync(_currentPage);

            if (!result.IsSuccess)
            {
                FireErrorEvent(result.Code);
            }

            return result.Result == null ? null : result.Result.Entries;
        }

        protected override void AddItems(IList<News> items)
        {
            if (items != null)
            {
                foreach (var news in items)
                {
                    if (!this.Any(n => n.ID == news.ID))
                    {
                        news.Title = Windows.Data.Html.HtmlUtilities.ConvertToText(news.Title);
                        news.Summary = Windows.Data.Html.HtmlUtilities.ConvertToText(news.Summary);
                        news.Status = CNBlogSettings.Instance.GetNewsReadState(news.ID);

                        this.Add(news);
                    }
                }
            }
        }
    }
}
