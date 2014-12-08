using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CNBlogs.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.CloudAPI;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace CNBlogs
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class PostReadingPage : Page
    {
        private Common.NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private CommentsDS commentDS;

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
            this.DataContext = this;
            this.navigationHelper = new NavigationHelper(this); 
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            LoadData();
        }

        private void LoadData()
        {
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            object navigationParameter;
            if (e.PageState != null && e.PageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = e.PageState["SelectedItem"];
            }

            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
            // TODO: Assign the selected item to this.flipView.SelectedItem
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            /*
            navigationHelper.OnNavigatedTo(e);
            this._rpp = e.Parameter as ReadingPageParam;
            string typeValue = string.Empty;
            switch (this._rpp.contentType)
            {
                case ContentTypes.Post:
                    CNBlogs.DataHelper.CloudAPI.PostContentDS pcDS = new DataHelper.CloudAPI.PostContentDS(this._rpp.contentId);
                    if (await pcDS.LoadRemoteData())
                    {
                        this.wv_Post.NavigateToString(pcDS.Content);
                    }
                    typeValue = "blog";
                    break;
                case ContentTypes.News:
                    CNBlogs.DataHelper.CloudAPI.NewsContentDS ncDS = new DataHelper.CloudAPI.NewsContentDS(this._rpp.contentId);
                    if (await ncDS.LoadRemoteData())
                    {
                        this.wv_Post.NavigateToString(ncDS.News.Content);
                    }
                    typeValue = "news";
                    break;
            }
            this.commentDS = new CommentsDS(_rpp.contentId, typeValue);
            this.lv_Comments.ItemsSource = commentDS;
            this.pageTitle.Text = _rpp.contentTitle;*/
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }
    }
}
