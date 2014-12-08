using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace CNBlogs.DataHelper.CloudAPI
{
    /// <summary>
    /// a incremental loading class base on the data binding sample on MSDN 
    /// https://code.msdn.microsoft.com/windowsapps/Data-Binding-7b1d67b5/
    /// , but using ObservableCollection to contain data and notify changes.
    /// if you want to use incremental loading in MVVM pattern, you can use this as a collection,
    /// and add a constructor with a delegate to load data,
    /// so that you can load different data in your view model, refer this blog for detail
    /// http://blogs.msdn.com/b/devosaure/archive/2012/10/15/isupportincrementalloading-loading-a-subsets-of-data.aspx
    /// 
    /// </summary>
    public abstract class IncrementalLoadingBase<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        #region ISupportIncrementalLoading

        public bool HasMoreItems
        {
            get { return HasMoreItemsOverride(); }
        }

        /// <summary>
        /// load more items, this is invoked by Controls like ListView
        /// </summary>
        /// <param name="count">how many new items want to load</param>
        /// <returns>item count actually loaded</returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_busy)
            {
                return Task.Run(() => new LoadMoreItemsResult { Count = 0 }).AsAsyncOperation();
            }

            _busy = true;

            // we need to use AsyncInfo.Run to invoke async operation, as this method cannot return a Task
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        #endregion

        /// <summary>
        /// we use this method to load data and add to self
        /// </summary>
        /// <param name="c">cancellation token</param>
        /// <param name="count">how many want to load</param>
        /// <returns>item count actually loaded</returns>
        protected async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                // we are going to load more
                if (this.OnLoadMoreStarted != null)
                {
                    this.OnLoadMoreStarted(count);
                }

                // data loading will different for sub-class
                var items = await LoadMoreItemsOverrideAsync(c, count);

                AddItems(items);

                // we finished loading operation
                if (this.OnLoadMoreCompleted != null)
                {
                    this.OnLoadMoreCompleted(items == null ? 0 : items.Count);
                }

                return new LoadMoreItemsResult { Count = items == null ? 0 : (uint)items.Count };
            }
            finally
            {
                _busy = false;
            }
        }



        public delegate void LoadMoreStarted(uint count);
        public delegate void LoadMoreCompleted(int count);

        public event LoadMoreStarted OnLoadMoreStarted;
        public event LoadMoreCompleted OnLoadMoreCompleted;

        #region Overridable methods
        /// <summary>
        /// append items to list
        /// </summary>
        protected virtual void AddItems(IList<T> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    this.Add(item);
                }
            }
        }

        protected abstract Task<IList<T>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count);

        protected abstract bool HasMoreItemsOverride();

        #endregion

        protected bool _busy = false;
    }
}
