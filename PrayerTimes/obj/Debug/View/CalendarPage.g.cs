﻿

#pragma checksum "S:\Dev\Windows 8\PrayerTimes\PrayerTimes\View\CalendarPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7F32C042532AD6BA669AB842E6CBBF0B"
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
    partial class CalendarPage : global::PrayerTimes.Common.LayoutAwarePage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 87 "..\..\View\CalendarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.AppBar)(target)).Opened += this.TopBar_Opened;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 223 "..\..\View\CalendarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.OpenParameters;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 224 "..\..\View\CalendarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.OpenAppBar;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 130 "..\..\View\CalendarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Button_Click_1;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


