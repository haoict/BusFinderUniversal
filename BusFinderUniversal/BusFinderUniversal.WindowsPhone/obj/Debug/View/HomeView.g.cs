﻿

#pragma checksum "D:\GitHub\BusFinderUniversal\BusFinderUniversal\BusFinderUniversal.WindowsPhone\View\HomeView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "274E0A8C6842F54E6732E85D729DDDAB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BusFinderUniversal.View
{
    partial class HomeView : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 160 "..\..\View\HomeView.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.SuggestApp_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 179 "..\..\View\HomeView.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Setting_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 180 "..\..\View\HomeView.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.Help_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 181 "..\..\View\HomeView.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.About_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


