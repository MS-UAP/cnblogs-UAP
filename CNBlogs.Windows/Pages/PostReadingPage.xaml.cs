using CNBlogs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CNBlogs.DataHelper;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Function;
using CNBlogs.DataHelper.CloudAPI;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CNBlogs.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class PostReadingPage : FlatNavigationPage
    {
        public Author Author { get; set; }
        public string Title { get; set; }

        private Post post = null;
        private string content = string.Empty;
        private CommentsDS commentDS;
        private string commentsCount = "0";

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public PostReadingPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                Logger.LogAgent.GetInstance().WriteLog(this.GetType().ToString());
            }
            try
            {
                //for favorite button display
                await FavoriteAuthorDS.Instance.Refresh();

                navigationHelper.OnNavigatedTo(e);

                this.commentsCount = string.Empty;
                var pageFile = string.Empty;

                if (e.Parameter is Post)
                {
                    this.post = e.Parameter as Post;
                    this.Title = this.post.Title;
                    this.Author = post.Author;
                    if (this.Author.Avatar == null)
                    {
                        SearchAuthorInfo();
                    }
                    this.commentsCount = post.CommentsCount;
                    CNBlogs.DataHelper.CloudAPI.PostContentDS pcDS = new DataHelper.CloudAPI.PostContentDS(post.ID);
                    if (await pcDS.LoadRemoteData())
                    {
                        content = pcDS.Content;
                    }

                    pageFile = "ms-appx-web:///HTML/post_day.html";
                    currentBlogger.DataContext = this.post.Author;
                    this.commentDS = new CommentsDS(this.post.ID, "blog");

                    //set read
                    this.SetNewStatus(this.post, PostStatus.Read, true);
                }
                else
                {
                    return;
                }
                this.DataContext = this;
                this.wv_Post.Navigate(new Uri(pageFile));
                this.commentDS.OnLoadMoreCompleted += commentDS_OnLoadMoreCompleted;

                this.lv_Comments.ItemsSource = commentDS;
                if (this.commentsCount == "0")
                {
                    this.lv_Comments.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    this.tb_Message.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }

                //Set UI
                this.SetTitleFont(pageTitle.Text.Length);
                UpdateUI();
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        private async void SearchAuthorInfo()
        {
            try
            {
                var bloggerTask = await APIWrapper.Instance.SearchBloggerAsync(this.Author.Name);
                if (bloggerTask.IsSuccess)
                {
                    if (bloggerTask.Result.Entries.Count > 0)
                    {
                        if (bloggerTask.Result.Entries.Any(i => i.Id.Equals(this.Author.Uri)))
                        {
                            int index = bloggerTask.Result.Entries.FindIndex(i => i.Id.Equals(this.Author.Uri));
                            this.Author.Avatar = bloggerTask.Result.Entries[index].Avatar;
                            this.Author.BlogApp = bloggerTask.Result.Entries[index].BlogApp;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateUI()
        {
            //favorite
            if (this.post.Status == PostStatus.Favorite)
            {
                btn_Favorite.Visibility = Visibility.Collapsed;
                btn_UnFavorite.Visibility = Visibility.Visible;
            }
            else
            {
                btn_Favorite.Visibility = Visibility.Visible;
                btn_UnFavorite.Visibility = Visibility.Collapsed;
            }

            ////favorite author
            //if(FavoriteAuthorDS.Instance.Items.Any(i => i.Item.Uri == Author.Uri))
            //{
            //    btn_AuthorFavorite.Visibility = Visibility.Collapsed;
            //    btn_AuthorUnFavorite.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    btn_AuthorFavorite.Visibility = Visibility.Visible;
            //    btn_AuthorUnFavorite.Visibility = Visibility.Collapsed;
            //}
        }

        async void commentDS_OnLoadMoreCompleted(int count)
        {
            try
            {
                if (this.commentDS != null)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                      {
                          char[] charsToTrim = { '*', ' ', '\'','\\','/'};
                          try
                          {
                              this.tb_CommentCount.Text = string.Format("{0}/{1}", this.commentDS.Count, this.commentsCount.Trim(charsToTrim));
                          }
                          catch (Exception)
                          {
                              this.tb_CommentCount.Text = string.Format("{0}", this.commentDS.Count);
                          } 

                      });
                }
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void wv_Post_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            try
            {
                pr_Loading.IsActive = false;
                await this.wv_Post.InvokeScriptAsync("setContent", new[] { content });
            }
            catch (Exception ex)
            {
                this.Title = ex.Message;
            }

        }

        private void currentBlogger_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GoToBloggerPage();
        }

        private void GoToBloggerPage()
        {
            Author currentAuthor = currentBlogger.DataContext as Author;
            Blogger blogger = new Blogger()
            {
                Avatar = this.post.Author.Avatar,
                Name = this.post.Author.Name,
                BlogApp = this.post.BlogApp,
                Link = this.post.Link
            };
            try
            {
                this.Frame.Navigate(typeof(BloggerPage), blogger);
            }
            catch (Exception)
            {
            }
        }

        Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();

        private async void btn_Favorite_Click(object sender, RoutedEventArgs e)
        {
            await this.post.AsFavorite();
            notifyBlock.ShowMessage(loader.GetString("Notify_FavoriteBlog"));
            UpdateUI();
        }

        private async void btn_UnFavorite_Click(object sender, RoutedEventArgs e)
        {
            await this.post.UnFavorite();
            notifyBlock.ShowMessage(loader.GetString("Notify_UnFavoriteBlog"));
            UpdateUI();
        }

        private void SetNewStatus(CNBlogs.DataHelper.DataModel.Post post, CNBlogs.DataHelper.DataModel.PostStatus newStatus, bool updateUI = true)
        {
            if (post.Status != newStatus)
            {
                if (post.Status <= newStatus)
                {
                    post.Status = newStatus;
                    DataHelper.DataModel.CNBlogSettings.Instance.SaveBlogStatus(post);
                }
                if (updateUI)
                {
                    this.UpdateUI();
                }
            }
        }

        private void btn_OriginalLink_Click(object sender, RoutedEventArgs e)
        {
            if (this.post != null)
            {
                wv_Post.Navigate(new Uri(this.post.Link.Href));
            }
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPageVisualStatus(e);
        }

        private void SetPageVisualStatus(SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1366)
            {
                Sb_CommentHiddenVIew.Begin();
                isCommentTemperarorilyShown = false;
            }
            else
            {
                Sb_CommentShownVIew.Begin();
                isCommentTemperarorilyShown = false;
            }
            CommentButton.Content = isCommentTemperarorilyShown ? loader.GetString("CommentTextblockText_Status2") : loader.GetString("CommentTextblockText_Status1");
        }

        private void SetPageVisualStatus(double pageWidth)
        {
            if (pageWidth < 1366)
            {
                Sb_CommentHiddenVIew.Begin();
                isCommentTemperarorilyShown = false;
            }
            else
            {
                Sb_CommentShownVIew.Begin();
                isCommentTemperarorilyShown = false;
            }
            CommentButton.Content = isCommentTemperarorilyShown ? loader.GetString("CommentTextblockText_Status2") : loader.GetString("CommentTextblockText_Status1");
        }


        private bool isCommentTemperarorilyShown = false;

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (isCommentTemperarorilyShown)
            {
                Sb_CommentHidden.Begin();
            }
            else
            {
                Sb_CommentTemperaroryShown.Begin();
            }
            isCommentTemperarorilyShown = !isCommentTemperarorilyShown;
            CommentButton.Content = isCommentTemperarorilyShown ? loader.GetString("CommentTextblockText_Status2") : loader.GetString("CommentTextblockText_Status1");
        }

        private void wv_Post_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isCommentTemperarorilyShown)
            {
                Sb_CommentHidden.Begin(); 
                isCommentTemperarorilyShown = !isCommentTemperarorilyShown;
                CommentButton.Content = isCommentTemperarorilyShown ? loader.GetString("CommentTextblockText_Status2") : loader.GetString("CommentTextblockText_Status1");
            }
        }

        private void SetTitleFont(int titleLength)
        {
            if (titleLength > 20)
                pageTitle.FontSize = 24;
            else if (titleLength > 40)
                pageTitle.FontSize = 20;
        }

        private async void btn_AuthorFavorite_Click(object sender, RoutedEventArgs e)
        {
            await FavoriteAuthorDS.Instance.Follow(this.Author);
            notifyBlock.ShowMessage(loader.GetString("Notify_FavoriteBlogger"));
            UpdateUI();
        }

        private void btn_AuthorUnFavorite_Click(object sender, RoutedEventArgs e)
        {
            FavoriteAuthorDS.Instance.Remove(new FavoriteItem<Author>() { Item = this.Author });
            notifyBlock.ShowMessage(loader.GetString("Notify_FavoriteBlogger"));
            UpdateUI();
        }

        private void CommandBar_Opened(object sender, object e)
        {
            UpdateUI();
        }

        private void btn_BloggerInfo_Click(object sender, RoutedEventArgs e)
        {
            GoToBloggerPage();
        }
    }
}
