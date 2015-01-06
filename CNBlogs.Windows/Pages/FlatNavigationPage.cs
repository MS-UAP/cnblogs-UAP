using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CNBlogs
{
    public class FlatNavigationPage : Page
    {
        public static FlatNavigationPage Current;

        public FlatNavigationPage()
        {
            this.Loaded += FlatNavigationPage_Loaded;
        }

        void FlatNavigationPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Current = this;

            AppBar appBar = new AppBar();
            appBar.Content = new FlatNavigationControl();
            this.TopAppBar = appBar;

            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0, 0);
            myLinearGradientBrush.EndPoint = new Point(1, 1);
            GradientStop gradientStop = new GradientStop();

            myLinearGradientBrush.GradientStops.Add(new GradientStop() { Color = Colors.AliceBlue, Offset = 0 });
            myLinearGradientBrush.GradientStops.Add(new GradientStop() { Color = Colors.Magenta, Offset = 1 });

            this.Background = myLinearGradientBrush;
        }
    }
}
