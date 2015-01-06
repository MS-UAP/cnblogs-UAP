using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogs.DataHelper.DataModel
{
    public class FavoriteItem<T> : DataModelBase
    {
        private bool _hasNew = false;
        public bool HasNew
        {
            get { return _hasNew; }
            set
            {
                _hasNew = value;
                this.OnPropertyChanged();
            }
        }
        public T Item { get; set; }
    }

    public class FavoriteItem : DataModelBase
    {
        public Author Author { get; set; }
        public Category Category { get; set; }
        public Post Post { get; set; }

        private bool _hasNew = false;
        public bool HasNew
        {
            get { return _hasNew; }
            set
            {
                _hasNew = value;
                this.OnPropertyChanged();
            }
        }
        public FollowedType Type { get; set; }

        public string Id
        {
            get
            {
                if (this.Type == FollowedType.Author)
                {
                    if (this.Author != null)
                    {
                        return this.Author.Uri;
                    }
                }
                else if (this.Type == FollowedType.Category)
                {
                    if (this.Category != null)
                    {
                        return this.Category.Id;
                    }
                }
                return null;
            }
        }
    }

    public enum FollowedType
    {
        None,
        Category,
        Author,
        Blog
    }

}
