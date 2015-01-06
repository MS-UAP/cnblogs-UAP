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
using Windows.UI.Xaml.Media.Animation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CNBlogs
{
    public sealed class PostControl : Control
    {
        private Grid mainPostGrid;
        private Button naviButton;
        private Storyboard storyBoard;
        private TextBlock favoriteIcon;
        private AttributionControl attributionControl;

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (isButtonPressed)
                ShowStoryBoard();
        }

        public PostControl()
        {
            this.DefaultStyleKey = typeof(PostControl);
            this.DataContextChanged += PostControl_DataContextChanged;
        }

        private void GetSummaryControl()
        {
            if (this.mainPostGrid == null)
            {
                this.mainPostGrid = this.GetTemplateChild("MainGrid") as Grid;
            }
        }
        private void GetAttributionControl()
        {
            if (this.attributionControl == null)
            {
                this.attributionControl = this.GetTemplateChild("AttributionControl") as AttributionControl;
            }
        }



        private void GetTextBlockControl()
        {
            if (this.favoriteIcon == null)
            {
                this.favoriteIcon = this.GetTemplateChild("FavoriteIcon") as TextBlock;
            }
        }


        private void GetNaviButtonControl()
        {
            if (this.naviButton == null)
            {
                this.naviButton = this.GetTemplateChild("NaviButton") as Button;
            }
        }

        private void GetStoryBoardControl(string name)
        {
            if (this.storyBoard == null)
            {
                this.storyBoard = this.GetTemplateChild(name) as Storyboard;
            }
        }

        private bool isButtonPressed = false;

        public void ShowStoryBoard()
        {
            storyBoard = null;
            GetStoryBoardControl(isButtonPressed ? "sb_Button_out" : "sb_Button_in");
            this.storyBoard.Completed += storyBoard_Completed;
            if (this.storyBoard != null)
            {
                this.storyBoard.Begin();
                isButtonPressed = !isButtonPressed;
            }
        }

        void storyBoard_Completed(object sender, object e)
        {
            if (this.naviButton == null)
            {
                GetNaviButtonControl();
            }

            if (this.naviButton != null)
            {
                naviButton.Content = isButtonPressed ? Convert.ToChar(0xE09F).ToString() : Convert.ToChar(0xE09E).ToString();
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
                    break;
                default:
                    post.Status = DataHelper.DataModel.PostStatus.None;
                    break;
            }

            if (post.CommentsCount == null && post.DiggsCount == null && post.ViewsCount == null) 
            {
                GetAttributionControl();
                if(attributionControl!=null)
                {
                    attributionControl.Visibility = Visibility.Collapsed;
                }
            }

            base.OnApplyTemplate();
        }

        private void SetFavoriteStatus()
        {
            GetTextBlockControl();
            if(this.favoriteIcon != null)
            {
                this.favoriteIcon.Visibility = Visibility.Visible;
            }
        }

        private void SetReadStatus()
        {
            GetSummaryControl();
            if (this.mainPostGrid != null)
            {
                this.mainPostGrid.Opacity = 0.8;
            } 
            GetTextBlockControl();
            if (this.favoriteIcon != null)
            {
                this.favoriteIcon.Visibility = Visibility.Collapsed;
            }

        }
    }
}
