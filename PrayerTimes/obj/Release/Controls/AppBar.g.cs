﻿

#pragma checksum "S:\Dev\Windows 8\PrayerTimes\PrayerTimes\Controls\AppBar.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6AE2D2E61724B696B4FB3E0306A5DCF9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PrayerTimes
{
    partial class AppBar : global::Windows.UI.Xaml.Controls.UserControl, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 41 "..\..\Controls\AppBar.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ButtonGeolocation_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 43 "..\..\Controls\AppBar.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ButtonFavorites_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 45 "..\..\Controls\AppBar.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ButtonAddToFavorites_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 47 "..\..\Controls\AppBar.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ButtonRemoveFromFavorites_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

