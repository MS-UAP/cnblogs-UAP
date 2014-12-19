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
    public sealed class PageTitleControl : Control
    {
        public ProgressBar pb_Top;
        public TextBlock tb_Count;

        public string TitleContent
        {
            get { return (string)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

        private void GetProgressBarControl(string pb_name)
        {
            if (this.pb_Top == null)
            {
                this.pb_Top = this.GetTemplateChild(pb_name) as ProgressBar;
            }
        }

        private void GetTextBlockControl(string tb_name)
        {
            if (this.tb_Count == null)
            {
                this.tb_Count = this.GetTemplateChild(tb_name) as TextBlock;
            }
        }



        // Using a DependencyProperty as the backing store for TitleContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleContentProperty =
            DependencyProperty.Register("TitleContent", typeof(string), typeof(PageTitleControl), new PropertyMetadata("title"));


        public PageTitleControl()
        {
            this.DefaultStyleKey = typeof(PageTitleControl);
            this.DataContextChanged += PageTitleControl_DataContextChanged;
        }

        void PageTitleControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.UpdateUI();
        }

        private void UpdateUI()
        {
            GetTextBlockControl("CountControl");
            if (tb_Count != null)
            {
                if (this.DataContext == null)
                    this.tb_Count.Visibility = Visibility.Collapsed;
                else
                    this.tb_Count.Visibility = Visibility.Visible;

                base.OnApplyTemplate();
            }
        }

        public async void DS_OnLoadMoreStarted(uint count)
        {
            GetProgressBarControl("pb_Top");
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FunctionHelper.Functions.RefreshUIOnDataLoading(this.pb_Top, null);
            });
        }

        public async void DS_OnLoadMoreCompleted(int count)
        {
            GetProgressBarControl("pb_Top");
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FunctionHelper.Functions.RefreshUIOnDataLoaded(this.pb_Top, null);
            });
        }

        public void DS_DataRequestError(int code)
        {
        }

        public bool SetTopProgressBarIndeterminate(bool isIndeterminate)
        {
            GetProgressBarControl("pb_Top");
            if (this.pb_Top != null)
            {
                this.pb_Top.IsIndeterminate = isIndeterminate;
                return true;
            }
            else return false;
        }
        public bool SetTopProgressBarVisibility(bool isVisible)
        {
            GetProgressBarControl("pb_Top");
            if (this.pb_Top != null)
            {
                if (isVisible)
                    this.pb_Top.Visibility = Visibility.Visible;
                else
                    this.pb_Top.Visibility = Visibility.Collapsed;
                return true;
            }
            else return false;
        }
    }
}
