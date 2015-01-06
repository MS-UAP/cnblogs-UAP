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
    public sealed class NewsControl : Control
    {
        private Grid mainPostGrid;
        private Button naviButton;
        private Storyboard storyBoard;

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (isButtonPressed)
                ShowStoryBoard();
        }

        public NewsControl()
        {
            this.DefaultStyleKey = typeof(NewsControl);
        }

        private void GetSummaryControl()
        {
            if (this.mainPostGrid == null)
            {
                this.mainPostGrid = this.GetTemplateChild("mainPostGrid") as Grid;
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

    }
}
