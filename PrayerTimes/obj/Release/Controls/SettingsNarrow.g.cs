﻿

#pragma checksum "S:\Dev\Windows 8\PrayerTimes\PrayerTimes\Controls\SettingsNarrow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "56A2CF62AD7BABEE4FF83C3C0CCB8DCD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PrayerTimes.Controls
{
    partial class SettingsNarrow : global::Windows.UI.Xaml.Controls.UserControl, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 162 "..\..\Controls\SettingsNarrow.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).KeyDown += this.MaghribAdjustmentTextBox_KeyDown;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 134 "..\..\Controls\SettingsNarrow.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.MySettingsBackClicked;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


