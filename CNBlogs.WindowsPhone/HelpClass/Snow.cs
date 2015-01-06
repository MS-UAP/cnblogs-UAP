using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CNBlogs
{
    class Snow
    {
        public int x;
        public int y;
        public TextBlock tb;
             
        public Snow(int x, int y, int size)
        {
            this.tb = new TextBlock();
            tb.Text = "❆";
            tb.FontFamily = new FontFamily("Segoe UI Symbol");
            tb.FontSize = size;
            tb.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            this.x = x;
            this.y = y;
            tb.SetValue(Canvas.LeftProperty, this.x);
            tb.SetValue(Canvas.TopProperty, this.y);
        }

        public void SnowDropDown()
        {
            this.y += 5;
        }
    }
}
