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

using Windows.UI.Popups;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFiDirect;
using Windows.Networking.Proximity;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace App1
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            FoundDeviceList.Items.Add(PeerFinder.DisplayName);
        }
        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //アプリが動作しているPCでWi-Fi Directが使えるかをチェック
            if ((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) == PeerDiscoveryTypes.Browse) { }
            else {
                var dialog = new MessageDialog("このデバイスはWi-FiDirectに対応してないよ");
                await dialog.ShowAsync();
            }

            //if (PeerFinder.AllowWiFiDirect) { }


            /*PeerFinder.ConnectionRequested +=
                  new TypedEventHandler<object, ConnectionRequestedEventArgs>
                      (PeerFinder_ConnectionRequested);
            PeerFinder.Start();
            */
            //同じストアアプリが動いていて、Start()メソッドをコール済みのPCを探す
            var peers = await PeerFinder.FindAllPeersAsync();

            FoundDeviceList.Items.Add(PeerFinder.DisplayName);
            

        }

        


    }
}
