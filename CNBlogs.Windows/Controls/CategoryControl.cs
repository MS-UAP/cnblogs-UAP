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
    public sealed class CategoryControl : Control
    {
        public CategoryControl()
        {
            this.DefaultStyleKey = typeof(CategoryControl);
        }

        private TextBlock textBlock;

        private void GetTextBlockControl(string tb_name)
        {
            this.textBlock = this.GetTemplateChild(tb_name) as TextBlock;
        }

        private void SetTextBlockVisibility(string tb_name, bool isVisible)
        {
            GetTextBlockControl(tb_name);
            if (textBlock != null)
            {
                textBlock.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void SetLeftVisible()
        {
            SetTextBlockVisibility("LeftSymbol", true);
            SetTextBlockVisibility("RightSymbol", false);
        }

        public void SetRightVisible()
        {
            SetTextBlockVisibility("LeftSymbol", false);
            SetTextBlockVisibility("RightSymbol", true);
        }
    }
}
