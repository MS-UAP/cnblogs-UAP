using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace CNBlogs
{
    class FavoriteGroup
    {
        bool ShowListView = true;
        ListView lvDetail;
        Storyboard sbShow, sbHide;

        public FavoriteGroup(ListView lv)
        {
            this.lvDetail = lv;
            CreateStoryboard();
            this.sbHide.Completed += sbHide_Completed;
        }

        private void sbHide_Completed(object sender, object e)
        {
            this.lvDetail.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        public void Tapped()
        {
            this.ShowListView = !this.ShowListView;
            if (this.ShowListView)
            {
                this.lvDetail.Opacity = 0;
                this.lvDetail.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.sbShow.Begin();
            }
            else
            {
                this.sbHide.Begin();
            }
        }

        private void CreateStoryboard()
        {
            // show listview in 1 second
            DoubleAnimation daShow = new DoubleAnimation();
            daShow.From = 0;
            daShow.To = 1;
            daShow.Duration = new Windows.UI.Xaml.Duration(TimeSpan.FromSeconds(1));

            this.sbShow = new Storyboard();
            sbShow.Children.Add(daShow);
            Storyboard.SetTarget(daShow, this.lvDetail);
            Storyboard.SetTargetProperty(daShow, "Opacity");

            // hide listview in 1 second
            DoubleAnimation daHide = new DoubleAnimation();
            daHide.From = 1;
            daHide.To = 0;
            daHide.Duration = new Windows.UI.Xaml.Duration(TimeSpan.FromSeconds(1));

            this.sbHide = new Storyboard();
            sbHide.Children.Add(daHide);
            Storyboard.SetTarget(daHide, this.lvDetail);
            Storyboard.SetTargetProperty(daHide, "Opacity");
        }

    }
}
