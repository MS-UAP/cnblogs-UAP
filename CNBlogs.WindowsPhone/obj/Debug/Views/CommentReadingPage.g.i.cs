﻿

#pragma checksum "E:\GitHub\cnblogs-UAP\CNBlogs.WindowsPhone\Views\CommentReadingPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "771DA52646C3B41BBAD1CBC3413300F3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CNBlogs
{
    partial class CommentReadingPage : global::Windows.UI.Xaml.Controls.Page
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard sb_Hide; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard sb_Show; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.ProgressBar pb_Top; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock tb_CommentCount; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.ListView lv_Comments; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton btn_ScrollToTop; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///Views/CommentReadingPage.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            sb_Hide = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("sb_Hide");
            sb_Show = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("sb_Show");
            pb_Top = (global::Windows.UI.Xaml.Controls.ProgressBar)this.FindName("pb_Top");
            tb_CommentCount = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("tb_CommentCount");
            lv_Comments = (global::Windows.UI.Xaml.Controls.ListView)this.FindName("lv_Comments");
            btn_ScrollToTop = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("btn_ScrollToTop");
        }
    }
}



