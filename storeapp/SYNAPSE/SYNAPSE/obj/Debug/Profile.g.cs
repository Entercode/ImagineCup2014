﻿

#pragma checksum "C:\Users\tomoki\documents\visual studio 2013\Projects\SYNAPSE\SYNAPSE\Profile.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "352CBF27215D6A4BF35691C69F551F67"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SYNAPSE
{
    partial class Profile : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 49 "..\..\Profile.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).DoubleTapped += this.ProfileImage_doubleTapped;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 52 "..\..\Profile.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.SendButton_clik;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


