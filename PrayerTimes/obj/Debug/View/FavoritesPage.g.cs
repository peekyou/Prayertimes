﻿

#pragma checksum "S:\Dev\Windows 8\PrayerTimes\PrayerTimes\View\FavoritesPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BF37D1F193AC1D523FCCC338AC7A15B0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PrayerTimes.View
{
    partial class FavoritesPage : global::PrayerTimes.Common.LayoutAwarePage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 81 "..\..\View\FavoritesPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.itemListView_ItemClick;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 97 "..\..\View\FavoritesPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.itemListView_ItemClick;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 66 "..\..\View\FavoritesPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.GoBack;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

