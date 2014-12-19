using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CNBlogs.DataHelper.DataModel
{
    public sealed class CNBlogSettings : DataModelBase
    {
        private ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer roaming = Windows.Storage.ApplicationData.Current.RoamingSettings;

        const string SettingKey_DefaultDisplaySummary = "cnblog_default_display_summary";
        const string SettingKey_ClickTitleForSummary = "cnblog_click_title_for_summary";
        const string SettingKey_NightModeTheme = "cnblog_night_mode_theme";
        const string SettingKeyFormat_BlogReaded = "cnblog_blog_readed_{0}";
        const string SettingKeyFormat_NewsReaded = "cnblog_news_readed_{0}";

        private PostStatus GetPostReadState(string format, string id)
        {
            var key = string.Format(format, id);

            return this.settings.Values.ContainsKey(key) ? (PostStatus)this.settings.Values[key] : PostStatus.None;
        }

        private void SetPostReadState(string format, string id, PostStatus status)
        {
            var key = string.Format(format, id);

            settings.Values[key] = (int)status;
        }

        public PostStatus GetBlogReadState(string blogId)
        {
            return GetPostReadState(SettingKeyFormat_BlogReaded, blogId);
        }

        public void SaveBlogStatus(Post post)
        {
            SetPostReadState(SettingKeyFormat_BlogReaded, post.ID, post.Status);
        }

        public PostStatus GetNewsReadState(string newsId)
        {
            return GetPostReadState(SettingKeyFormat_NewsReaded, newsId);
        }

        public void SaveNewsStatus(News news)
        {
            SetPostReadState(SettingKeyFormat_NewsReaded, news.ID, news.Status);
        }

        public bool DefaultDisplaySummary
        {
            get
            {
                var obj = this.settings.Values[SettingKey_DefaultDisplaySummary];
                return obj == null ? true : (bool)obj;
            }
            set
            {
                this.settings.Values[SettingKey_DefaultDisplaySummary] = value;
            }
        }


        public bool ClickTitleForSummary
        {
            get
            {
                var obj = this.settings.Values[SettingKey_ClickTitleForSummary];
                return obj == null ? false : (bool)obj;
            }
            set
            {
                this.settings.Values[SettingKey_ClickTitleForSummary] = value;
            }
        }

        /// <summary>
        /// default is DayMode, this value = false
        /// </summary>
        public bool NightModeTheme
        {
            get
            {
                var obj = this.settings.Values[SettingKey_NightModeTheme];
                return obj == null ? false : (bool)obj;
            }
            set
            {
                this.settings.Values[SettingKey_NightModeTheme] = value;
                base.OnPropertyChanged("NightModeTheme");
            }
        }

        private static volatile CNBlogSettings _instance;
        private static object _locker = new object();

        private CNBlogSettings() { }

        public static CNBlogSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new CNBlogSettings();
                        }
                    }
                }
                return _instance;
            }
        }


    }
}
