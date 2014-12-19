using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using CNBlogs.DataHelper.Helper;

namespace CNBlogs.DataHelper
{
    /// <summary>
    /// api wrapper for cnblog.
    /// </summary>
    /// <remarks>
    ///    TODO: check result of request
    /// 
    ///    the api of cnblog is a feed, so if we so do not need extra attributes,
    ///    we can use SyndicationClient to retrieve feed
    /// </remarks>

    public class APIWrapper
    {
        public static APIWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new APIWrapper();
                }

                return _instance;
            }
        }

        public async Task<bool> Signin(string username, string password)
        {
            var isSignin = false;

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri("http://passport.cnblogs.com/login.aspx"));
            request.Content = new HttpStringContent("tbUserName=" + Uri.EscapeDataString(username) + "&tbPassword=" + Uri.EscapeDataString(password));

            try
            {
                var response = await _client.SendRequestAsync(request);
                var cookie = string.Empty;

                if (response.StatusCode == HttpStatusCode.Ok 
                    && response.Headers.TryGetValue("Set-Cookie", out cookie))
                {
                    isSignin = true;

                    cookie = cookie.Replace("HttpOnly,", string.Empty);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("fail to get cookie, can not use it");
                }

                //request.Headers.Cookie.TryParseAdd("ASP.NET_SessionId=04oawm1g05w23so40u0e31nb; path=/; HttpOnly, SERVERID=9b2e527de1fc6430919cfb3051ec3e6c|1418716636|1418716636;Path=/");
            }
            catch
            {
            }

            return isSignin;
        }

        /// <summary>
        /// get posts of category from rss source, this will return 20 items as max 
        /// </summary>
        public async Task<RequestResult<Feed<Post>>> GetPostsByCategoryRSSAsync(string rssUrl)
        {
            return await RequestData<Feed<Post>>(rssUrl, xml => Functions.Deserlialize<Feed<Post>>(xml));
        }

        /// <summary>
        /// search blogger by keyword
        /// </summary>
        public async Task<RequestResult<Feed<Blogger>>> SearchBloggerAsync(string keyword)
        {
            var apiWithParameter = string.Format("bloggers/search?t={0}", Uri.EscapeUriString(keyword));
            var url = CombineUrl(apiWithParameter);

            return await RequestData<Feed<Blogger>>(url, xml => Functions.Deserlialize<Feed<Blogger>>(xml));
        }

        /// <summary>
        /// Get content of news
        /// </summary>
        public async Task<RequestResult<NewsBody>> GetNewsContentAsync(string newId)
        {
            var apiWithParameter = string.Format("item/{0}", newId);
            var url = CombineUrl(apiWithParameter, "news");

            return await RequestData<NewsBody>(url, xml => Functions.Deserlialize<NewsBody>(xml));
        }

        /// <summary>
        /// get content of post
        /// </summary>
        public async Task<RequestResult<string>> GetPostContentAsync(string postid)
        {
            var apiWithParameter = string.Format("post/body/{0}", postid);
            var url = CombineUrl(apiWithParameter);

            return await RequestData<string>(url, xml =>
            {
                var xmlDoc = XDocument.Parse(xml);

                // the content of root the is post body
                return xmlDoc.Root.Value;
            });
        }

        /// <summary>
        /// get paged comments of post, default page size is 20
        /// </summary>
        public async Task<RequestResult<Feed<Comment>>> GetPostCommentsAsync(string postid, int page, int pageSize = 20)
        {
            return await GetCommentsAsync("blog", postid, page, pageSize);
        }

        /// <summary>
        /// get paged comment of news, default page size is 20
        /// </summary>
        public async Task<RequestResult<Feed<Comment>>> GetNewsCommentsAsync(string newsId, int page, int pageSize = 20)
        {
            return await GetCommentsAsync("news", newsId, page, pageSize);
        }

        /// <summary>
        /// Get paged posts of specified author
        /// </summary>
        public async Task<RequestResult<BloggerFeed>> GetAuthorPostsAsync(string userid, int page, int pageSize = 20)
        {
            var apiWithParameter = string.Format("u/{0}/posts/{1}/{2}", userid, page, pageSize);

            var fullUrl = CombineUrl(apiWithParameter);

            return await RequestData<BloggerFeed>(fullUrl, xml => Functions.Deserlialize<BloggerFeed>(xml));
        }

        /// <summary>
        /// get paged news
        /// </summary>
        public async Task<RequestResult<Feed<News>>> GetRecentNewsAsync(int page, int pageSize = 20)
        {
            var apiWithParameter = string.Format("recent/paged/{0}/{1}", page, pageSize);

            var fullUrl = CombineUrl(apiWithParameter, "news");

            return await RequestData<Feed<News>>(fullUrl, xml => Functions.Deserlialize<Feed<News>>(xml));
        }

        /// <summary>
        /// get top view posts within 48 hours
        /// </summary>
        public async Task<RequestResult<Feed<Post>>> GetHotPostsAsync(int count = 20)
        {
            var apiWithParameter = string.Format("48HoursTopViewPosts/{0}", count);

            var fullUrl = CombineUrl(apiWithParameter);

            return await RequestData<Feed<Post>>(fullUrl, xml => Functions.Deserlialize<Feed<Post>>(xml));
        }

        /// <summary>
        /// get top tigg posts within 10 days
        /// </summary>
        public async Task<RequestResult<Feed<Post>>> GetTopTiggPostAsync(int count = 20)
        {
            var apiWithParameter = string.Format("TenDaysTopDiggPosts/{0}", count);

            var fullUrl = CombineUrl(apiWithParameter);

            return await RequestData<Feed<Post>>(fullUrl, xml => Functions.Deserlialize<Feed<Post>>(xml));
        }

        /// <summary>
        /// get paged recommanded bloggers
        /// </summary>
        public async Task<RequestResult<Feed<Blogger>>> GetRecommandBloggersAsync(int page, int pageSize = 20)
        {
            var apiWithParameter = string.Format("bloggers/recommend/{0}/{1}", page, pageSize);

            var fullUrl = CombineUrl(apiWithParameter);

            return await RequestData<Feed<Blogger>>(fullUrl, xml => Functions.Deserlialize<Feed<Blogger>>(xml));
        }

        /// <summary>
        /// get paged home site post 
        /// </summary>
        public async Task<RequestResult<Feed<Post>>> GetHomePostsAsync(int page, int pageSize = 20)
        {
            var apiWithParameter = string.Format("sitehome/paged/{0}/{1}", page, pageSize);

            var fullUrl = CombineUrl(apiWithParameter);

            return await RequestData<Feed<Post>>(fullUrl, xml => Functions.Deserlialize<Feed<Post>>(xml));
        }


        public async Task<RequestResult<Feed<Comment>>> GetCommentsAsync(string category, string parentId, int page, int pageSize = 30)
        {
            // comments from new and blog are same, but with different api path
            var apiWithParameter = string.Format("{0}/{1}/comments/{2}/{3}",
                category == "blog" ? "post" : "item",
                parentId,
                page,
                pageSize);

            var fullUrl = CombineUrl(apiWithParameter, category);

            return await RequestData<Feed<Comment>>(fullUrl, xml => Functions.Deserlialize<Feed<Comment>>(xml));
        }

        /// <summary>
        /// request data by url (now only Get support)
        /// </summary>
        private async Task<RequestResult<T>> RequestData<T>(string url, Func<string, T> func)
        {
            var result = new RequestResult<T>();
            result.IsNetworkError = false;

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

            try
            {
                var response = await _client.SendRequestAsync(request);

                // if no error then parse the response
                if (response.StatusCode == HttpStatusCode.Ok)
                {

                    // tranform the responst content to data model
                    if (func != null)
                    {
                        var xmlString = await response.Content.ReadAsStringAsync();
                        result.IsSuccess = true;
                        result.Result = func(xmlString);
                    }
                }
                else
                {
                    result.Code = (int)response.StatusCode;
                }
            }
            catch
            {
                result.IsNetworkError = true;
                result.Code = -1;
            }

            return result;
        }

        /// <summary>
        /// combine api with root
        /// </summary>
        private string CombineUrl(string apiWithParameter, string category = "blog")
        {
            return string.Format("{0}/{1}/{2}", "http://wcf.open.cnblogs.com", category, apiWithParameter);
        }

        private APIWrapper()
        {
            _client = new HttpClient();
        }

        private HttpClient _client = null;

        private static APIWrapper _instance = null;
    }
}
