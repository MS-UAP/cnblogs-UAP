using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Data;
using System.Threading;

namespace CNBlogs.DataHelper.CloudAPI
{    
    /// <summary>
    /// provide content of Post
    /// </summary>
    public class PostContentDS
    {
        string _postId;

        public string Content { get; set; }

        public PostContentDS(string postId)
        {
            this._postId = postId;
        }

        public async Task<bool> LoadRemoteData()
        {
            var result = await APIWrapper.Instance.GetPostContentAsync(_postId);

            if (!result.IsSuccess && this.DataRequestError != null)
            {
                DataRequestError(result.Code);
            }

            this.Content = result.Result;

            return !string.IsNullOrWhiteSpace(this.Content);
        }

        public event OnDataRequestError DataRequestError;
    }
}
