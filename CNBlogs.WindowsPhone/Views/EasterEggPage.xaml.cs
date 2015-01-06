using CNBlogs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.System.Threading;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CNBlogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EasterEggPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private ThreadPoolTimer timerToShowProgress;
        private ThreadPoolTimer timerToCreateSnow;
        private ThreadPoolTimer timerToMoveSnow;
        private int TotalSnowCount = 100;
        private int _snowCount = 0;
        private List<Snow> listSnow = new List<Snow>();
        private int _pageWidth = 100;
        private int _pageHeight = 400;
        private Random rand = new Random(DateTime.Now.Second);
        private int elapseSecond = 0;
        private int _snowHeight = 0;
        private int[] sizeOfSnow = new int[3] { 15, 20, 30 };

        public EasterEggPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            this.ReStart();
        }

        private void ReStart()
        {
            this.btn_Refresh.IsEnabled = false;
            this._snowCount = 0;
            this.listSnow.Clear();
            this.elapseSecond = 0;
            this._snowHeight = 0;
            this.pb_Progress.Value = 0;

            this.timerToShowProgress = ThreadPoolTimer.CreatePeriodicTimer(TimeToShowProgress, TimeSpan.FromSeconds(1));
            this.timerToCreateSnow = ThreadPoolTimer.CreatePeriodicTimer(TimeToAddSnow, TimeSpan.FromMilliseconds(100));
            this.timerToMoveSnow = ThreadPoolTimer.CreatePeriodicTimer(TimeToMoveSnow, TimeSpan.FromMilliseconds(50));
        }

        private async void TimeToShowProgress(ThreadPoolTimer timer)
        {
            await this.canvas.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.elapseSecond++;
                    this.pb_Progress.Value = this.elapseSecond;

                    if (this.elapseSecond >= this.pb_Progress.Maximum)
                    {
                        this.timerToShowProgress.Cancel();
                        this.timerToShowProgress = null;
                    }
                });
        }

        private async void TimeToAddSnow(ThreadPoolTimer timer)
        {
            await this.canvas.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Snow snow = new Snow(rand.Next(0, this._pageWidth), 0, this.sizeOfSnow[rand.Next(0,3)]);
                listSnow.Add(snow);
                this.canvas.Children.Add(snow.tb);
                _snowCount++;
                if (this._snowCount >= TotalSnowCount)
                {
                    this.timerToCreateSnow.Cancel();
                    this.timerToCreateSnow = null;
                }

            });
        }

        private async void TimeToMoveSnow(ThreadPoolTimer timer)
        {
            await this.canvas.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                List<Snow> listToRemove = new List<Snow>();
                foreach(Snow snow in this.listSnow)
                {
                    snow.SnowDropDown();
                    snow.tb.SetValue(Canvas.LeftProperty, snow.x);
                    snow.tb.SetValue(Canvas.TopProperty, snow.y);

                    if (snow.y >= this._snowHeight)
                    {
                        listToRemove.Add(snow);
                        
                        this.rect.Height += 2;
                        this._snowHeight -= 2;
                        this.rect.SetValue(Canvas.TopProperty, this._snowHeight);
                    }
                }

                foreach(Snow snow in listToRemove)
                {
                    this.canvas.Children.Remove(snow.tb);
                    this.listSnow.Remove(snow);
                }

                if (this.listSnow.Count == 0 && this.timerToCreateSnow == null)
                {
                    this.timerToMoveSnow.Cancel();
                    this.timerToMoveSnow = null;
                    this.sp_Plate.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    this.sb_PlateRaiseUp.Begin();
                }
            });
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this._pageWidth = (int)this.page.ActualWidth;
            this._pageHeight = (int)this.page.ActualHeight;
            this.rect.Width = this.page.ActualWidth;
            this._snowHeight = (int)this.page.ActualHeight - 150;
            this.rect.Height = 10;
            this.rect.SetValue(Canvas.LeftProperty, 0);
            this.rect.SetValue(Canvas.TopProperty, this._snowHeight);
            this.tb_SnowMan.SetValue(Canvas.TopProperty, this._pageHeight - 400);
            this.sp_Plate.SetValue(Canvas.TopProperty, this._pageHeight - 380);

            this.sb_Stat1.Begin(); 
            this.sb_Stat2.Begin(); 
            this.sb_Stat3.Begin();
            this.sb_Happy2.Begin();
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            this.sp_Plate.RenderTransform.SetValue(CompositeTransform.RotationProperty, 0);
            this.sp_Plate.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.tb_Happy.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.tb_Happy2.Opacity = 0;
            this.ReStart();
            this.Page_Loaded(sender, e);
        }

        private void sb_Happy_Completed(object sender, object e)
        {
            this.btn_Refresh.IsEnabled = true;
        }

        private void sb_PlateRaiseUp_Completed(object sender, object e)
        {
            this.tb_Happy.Visibility = Windows.UI.Xaml.Visibility.Visible; 
            this.sb_Happy.Begin();
        }
    }
}
