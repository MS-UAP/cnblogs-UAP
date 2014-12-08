using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Storage;
using System.Collections.ObjectModel;
using CNBlogs.DataHelper.DataModel;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using Windows.UI.Xaml;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;

namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// provide list of recent posts
    /// you can bind this ds to ItemSource to enable incremental loading ,
    /// or call LoadMoreItemsAsync to load more
    /// </summary>
    public class LatestPostsDS : PostDataSource
    {
        protected async override Task<IList<Post>> LoadItemsAsync()
        {
            var result =  await APIWrapper.Instance.GetHomePostsAsync(_currentPage);

            if (!result.IsSuccess)
            {
                FireErrorEvent(result.Code);
            }

            return result.Result == null ? null : result.Result.Entries;
        }
    }
}
