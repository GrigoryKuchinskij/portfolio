﻿#pragma checksum "E:\portfclone\Задание на разработку приложения просмотра видеопотока с сервера Macroscop\MJpegStreamViewer\MJpegStreamViewerProj\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "43AC9814F307C94EA62B750257718C73"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MJpegStreamViewerProj
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainPage.xaml line 23
                {
                    this.channelsListBox = (global::Windows.UI.Xaml.Controls.ListBox)(target);
                    ((global::Windows.UI.Xaml.Controls.ListBox)this.channelsListBox).SelectionChanged += this.channelsListBox_SelectionChanged;
                }
                break;
            case 3: // MainPage.xaml line 24
                {
                    this.shadeListBoxGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 4: // MainPage.xaml line 25
                {
                    this.overListBoxCenterText = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 5: // MainPage.xaml line 26
                {
                    this.ListProgressRing = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            case 6: // MainPage.xaml line 27
                {
                    this.serverPartUriTBox = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.serverPartUriTBox).TextChanging += this.serverPartUriTBox_TextChanging;
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.serverPartUriTBox).LostFocus += this.serverPartUriTBox_LostFocus;
                }
                break;
            case 7: // MainPage.xaml line 21
                {
                    this.requestUriTBox = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.requestUriTBox).TextChanging += this.requestUriTBox_TextChanging;
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.requestUriTBox).LostFocus += this.requestUriTBox_LostFocus;
                }
                break;
            case 8: // MainPage.xaml line 18
                {
                    this.ShowExtendedOptionsBtn = (global::Windows.UI.Xaml.Controls.HyperlinkButton)(target);
                    ((global::Windows.UI.Xaml.Controls.HyperlinkButton)this.ShowExtendedOptionsBtn).Click += this.ShowExtendedOptionsBtn_Click;
                }
                break;
            case 9: // MainPage.xaml line 19
                {
                    this.getChannelsBtn = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.getChannelsBtn).Click += this.getChannelsBtn_Click;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.18362.1")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

