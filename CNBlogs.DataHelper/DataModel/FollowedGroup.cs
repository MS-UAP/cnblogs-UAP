using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.DataModel
{
    public class FollowedGroup
    {
        public FollowedGroup()
        {
            Items = new ObservableCollection<FavoriteItem>();
        }

        public string GroupName { get; set; }
        public string Icon { get; set; }

        public ObservableCollection<FavoriteItem> Items { get; set; }
    }
}
