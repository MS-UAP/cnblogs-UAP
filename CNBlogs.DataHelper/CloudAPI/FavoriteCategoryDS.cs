using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNBlogs.DataHelper.DataModel;
using System.Collections.ObjectModel;
using CNBlogs.DataHelper.Function;

namespace CNBlogs.DataHelper.CloudAPI
{

    public class FavoriteCategoryDS : FavoriteItemBaseDS<Category>
    {
        private readonly static FavoriteCategoryDS _instance = new FavoriteCategoryDS();

        private FavoriteCategoryDS()
        {
            this.Items = new ObservableCollection<FavoriteItem<Category>>();
            this.Icon = "";
            this.Title = Functions.LoadResourceString("FavoriteCategoryText");
        }

        public static FavoriteCategoryDS Instance
        {
            get
            {
                return _instance;
            }
        }

        protected override async Task AddDefaultItem()
        {
            // if it is first time using this, then add default focus category and author
            if (CNBlogSettings.Instance.AddDefaultFollowedCategoryItem)
            {
                await Follow(new Category
                {
                    Id = "winrt_metro",
                    Name = "WinRT/Metro",
                    Href = "http://feed.cnblogs.com/blog/sitecateogry/winrt_metro/rss"
                });

                CNBlogSettings.Instance.AddDefaultFollowedCategoryItem = false;
            }
        }

        protected override async Task LoadSavedItemAsync()
        {
            if (!_isLoaded)
            {
                var categories = await FollowHelper.GetFollowedCategories();

                if (categories != null && categories.Count > 0)
                {
                    foreach (var cate in categories)
                    {
                        this.Items.Add(new FavoriteItem<Category> { HasNew = false, Item = cate });
                    }
                }

                _isLoaded = true;
            }
        }

        public override async Task Follow(Category category, string latestPostId = "")
        {
            if (category != null && !this.Items.Any(i => i.Item.Id == category.Id))
            {
                await category.Follow();

                this.Items.Add(new FavoriteItem<Category> { Item = category, HasNew = false });

                // if no latest post id, we use api to get
                if (string.IsNullOrWhiteSpace(latestPostId))
                {
                    var result = await APIWrapper.Instance.GetPostsByCategoryRSSAsync(category.Href);

                    if (result.IsSuccess && result.Result != null && result.Result.Entries != null && result.Result.Entries.Count > 0)
                    {
                        latestPostId = result.Result.Entries.First().ID;
                    }
                }

                CNBlogSettings.Instance.SetCategoryLastPostId(category.Id, latestPostId);
            }
            OnPropertyChanged("Count");
        }

        public override async Task CheckUpdate(FavoriteItem<Category> item)
        {
            var result = await APIWrapper.Instance.GetPostsByCategoryRSSAsync(item.Item.Href);

            if (result.IsSuccess && result.Result != null && result.Result.Entries != null && result.Result.Entries.Count > 0)
            {
                var latestPost = result.Result.Entries.First();

                var latestPostId = Functions.ParseBlogIDFromURL(latestPost.ID);

                var lastPostId = CNBlogSettings.Instance.GetCategoryLastPostId(item.Item.Id);

                // if lastPostId is empty, means that fail to get it last time
                if (!string.IsNullOrWhiteSpace(lastPostId) && latestPostId != lastPostId)
                {
                    item.HasNew = true;
                }

                CNBlogSettings.Instance.SetCategoryLastPostId(item.Item.Id, latestPostId);
            }
        }

        public override async Task Remove(FavoriteItem<Category> item)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Item.Id == item.Item.Id)
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
