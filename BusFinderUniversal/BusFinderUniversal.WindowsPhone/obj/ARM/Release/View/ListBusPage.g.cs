﻿

#pragma checksum "D:\GitHub\BusFinderUniversal\BusFinderUniversal\BusFinderUniversal.WindowsPhone\View\ListBusPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4DDE4D9AADCB597160449C79809CE9AB"
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
    partial class ListBusPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 328 "..\..\..\View\ListBusPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.SecondPivot_Loaded;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 318 "..\..\..\View\ListBusPage.xaml"
                ((global::Windows.UI.Xaml.Controls.TextBox)(target)).TextChanged += this.seachrbusid_TextChanged;
                 #line default
                 #line hidden
                #line 318 "..\..\..\View\ListBusPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.seachrbusid_GotFocus;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


