using CNBlogs.DataHelper.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace CNBlogs
{
    public sealed partial class PreferenceSettingsFlyout : SettingsFlyout
    {
        public PreferenceSettingsFlyout()
        {
            this.InitializeComponent();
            SetUILanguageComboBox();
            LanguageNoteTextbox.Visibility = Visibility.Collapsed;
        }

        private void SetUILanguageComboBox()
        {
            switch (CNBlogSettings.Instance.UILanguage)
            {
                case "zh-CN":
                    UILanguageComboBox.SelectedItem = UILanguageComboItem_zh_CN;
                    break;
                case "en-US":
                    UILanguageComboBox.SelectedItem = UILanguageComboItem_en_US;
                    break;
                default:
                    UILanguageComboBox.SelectedItem = UILanguageComboItem_zh_CN;
                    break;
            }
        }

        private void ReadingModeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (CNBlogs.DataHelper.DataModel.CNBlogSettings.Instance.NightModeTheme)
            {
                this.RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                this.RequestedTheme = ElementTheme.Light;
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = ((sender as ComboBox).SelectedItem as ComboBoxItem).Tag.ToString();
                CNBlogSettings.Instance.UILanguage = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
                LanguageNoteTextbox.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
