using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Function;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.CloudAPI
{
    public class FavoriteAuthorDS : FavoriteItemBaseDS<Author>
    {
        private readonly static FavoriteAuthorDS _instance = new FavoriteAuthorDS();

        private FavoriteAuthorDS()
        {
            this.Items = new ObservableCollection<FavoriteItem<Author>>();
            this.Icon = "";
            this.Title = "博主";
        }

        public static FavoriteAuthorDS Instance
        {
            get
            {
                return _instance;
            }
        }

        protected override async Task AddDefaultItem()
        {
            if (CNBlogSettings.Instance.AddDefaultFollowedAuthorItem)
            {
                await this.Follow(new Author
                {
                    Avatar = "http://pic.cnitblog.com/avatar/700062/20141211165421.png",
                    Name = "MS-UAP",
                    Uri = "http://www.cnblogs.com/ms-uap/"
                });

                CNBlogSettings.Instance.AddDefaultFollowedAuthorItem = false;
            }
        }

        protected override async Task LoadSavedItemAsync()
        {
            if (!_isLoaded)
            {
                var authors = await FollowHelper.GetFollowedAuthors();

                if (authors != null && authors.Count > 0)
                {
                    foreach (var author in authors)
                    {
                        this.Items.Add(new FavoriteItem<Author> { Item = author, HasNew = false });
                    }
                }

                _isLoaded = true;
            }
        }

        public override async Task Follow(Author author, string latestPostId = "")
        {
            if (author != null && !this.Items.Any(i => i.Item.Uri == author.Uri))
            {
                await author.Follow();
                this.Items.Add(new FavoriteItem<Author> { HasNew = false, Item = author });

                if (Uri.IsWellFormedUriString(author.Uri, UriKind.RelativeOrAbsolute))
                {
                    var blogApp = Functions.ParseBlogAppFromURL(author.Uri);

                    if (!string.IsNullOrWhiteSpace(blogApp))
                    {
                        if (string.IsNullOrWhiteSpace(latestPostId))
                        {
                            var result = await APIWrapper.Instance.GetAuthorPostsAsync(blogApp, 1, 1);

                            if (result.IsSuccess && result.Result != null && result.Result.Entries != null && result.Result.Entries.Count > 0)
                            {
                                latestPostId = result.Result.Entries.First().ID;
                            }
                        }

                        CNBlogSettings.Instance.SetCategoryLastPostId(blogApp, latestPostId);
                    }
                }
            }
            OnPropertyChanged("Count");
        }

        public override async Task CheckUpdate(FavoriteItem<Author> item)
        {
            var blogapp = Functions.ParseBlogAppFromURL(item.Item.Uri);

            if (!string.IsNullOrWhiteSpace(blogapp))
            {
                var result = await APIWrapper.Instance.GetAuthorPostsAsync(blogapp, 1, 1);

                if (result != null && result.Result != null && result.Result.Entries != null && result.Result.Entries.Count > 0)
                {
                    var latestPost = result.Result.Entries.First();
                    var lastPostId = CNBlogSettings.Instance.GetAuthorLastPostId(blogapp);

                    if (!string.IsNullOrWhiteSpace(lastPostId) && latestPost.ID != lastPostId)
                    {
                        item.HasNew = true;
                    }

                    CNBlogSettings.Instance.SetAuthorLastPostId(blogapp, latestPost.ID);
                }
            }
        }

        public override async Task Remove(FavoriteItem<Author> item)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Item.Uri == item.Item.Uri)
                {
                    await Items[i].Item.UnFollow();

                    Items.RemoveAt(i);

                    break;
                }
            }
            OnPropertyChanged("Count");
        }
    }
}
