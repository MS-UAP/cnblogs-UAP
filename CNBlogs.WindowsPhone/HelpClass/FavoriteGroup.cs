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

        public int Count
        {
            get
            {
                if (lvDetail != null)
                {
                    return this.lvDetail.Items.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public FavoriteGroup(ListView lv, Storyboard sb_show, Storyboard sb_hide)
        {
            this.lvDetail = lv;
            this.sbHide = sb_hide;
            this.sbShow = sb_show;
            this.sbHide.Completed += sbHide_Completed;
        }

        void sbHide_Completed(object sender, object e)
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
    }
}
