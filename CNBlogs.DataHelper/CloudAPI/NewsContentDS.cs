using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// provide content of news.
    /// </summary>
    public class NewsContentDS
    {
        public NewsContentDS(string newId)
        {
            _newsId = newId;
        }

        public NewsBody News { get; set; }

        public async Task<bool> LoadRemoteData()
        {
            var result = await APIWrapper.Instance.GetNewsContentAsync(_newsId);

            if (!result.IsSuccess && this.DataRequestError != null)
            {
                this.DataRequestError(result.Code);
            }

            this.News = result.Result;

            return this.News != null;
        }

        public event OnDataRequestError DataRequestError;

        private string _newsId = string.Empty;
    }
}
