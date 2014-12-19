using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CNBlogs
{
    public sealed class PostControl : Control
    {
        private Grid mainPostGrid;

        public PostControl()
        {
            this.DefaultStyleKey = typeof(PostControl);
            this.DataContextChanged += PostControl_DataContextChanged;
        }

        private void GetSummaryControl()
        {
            if (this.mainPostGrid == null)
            {
                this.mainPostGrid = this.GetTemplateChild("mainPostGrid") as Grid;
            }
        }

        void PostControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.UpdateUI();
        }
        protected override void OnApplyTemplate()
        {
            this.UpdateUI();
        }

        private void UpdateUI()
        {
            CNBlogs.DataHelper.DataModel.Post post = this.DataContext as CNBlogs.DataHelper.DataModel.Post;
            if (post == null)
            {
                return;
            }

            switch (post.Status)
            {
                case DataHelper.DataModel.PostStatus.None:
                    break;

                case DataHelper.DataModel.PostStatus.Skip:
                    break;

                case DataHelper.DataModel.PostStatus.Read:
                    this.SetReadStatus();
                    break;

                case DataHelper.DataModel.PostStatus.Favorite:
                    this.SetFavoriteStatus(); 
                    this.SetReadStatus();
                    break;
                default:
                    post.Status = DataHelper.DataModel.PostStatus.None;
                    break;
            }

            base.OnApplyTemplate();
        }

        private void SetFavoriteStatus()
        {
            
        }

        private void SetReadStatus()
        {
            GetSummaryControl();
            if (this.mainPostGrid != null)
            {
                this.mainPostGrid.Opacity = 0.5;
            }
        }
    }
}
