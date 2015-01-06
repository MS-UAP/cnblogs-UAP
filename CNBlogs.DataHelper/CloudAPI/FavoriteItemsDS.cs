using CNBlogs.DataHelper.DataModel;
using CNBlogs.DataHelper.Function;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace CNBlogs.DataHelper.CloudAPI
{
    public abstract class FavoriteItemBaseDS<T> : INotifyPropertyChanged
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public int Count { 
            get
            {
                if (this.Items != null)
                    return this.Items.Count;
                else
                    return 0;
            }
        }

        public ObservableCollection<FavoriteItem<T>> Items { get; protected set; }
        protected bool _isLoaded = false;

        public async Task Refresh()
        {
            _isLoaded = false;

            this.Items.Clear();

            await this.LoadData();
        }

        public async Task LoadData()
        {
            if (!_isLoaded)
            {
                await LoadSavedItemAsync();
            }

            await AddDefaultItem();
        }

        public async Task CheckUpdateForAll()
        {
            foreach (var item in Items)
            {

                await CheckUpdate(item);
            }
        }

        public abstract Task Follow(T item, string latestPostId = "");
        protected abstract Task LoadSavedItemAsync();

        protected abstract Task AddDefaultItem();
        public abstract Task Remove(FavoriteItem<T> item);
        public abstract Task CheckUpdate(FavoriteItem<T> item);

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
