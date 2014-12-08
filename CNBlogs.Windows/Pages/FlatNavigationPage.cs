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
        }
    }
}
