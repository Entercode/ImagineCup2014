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
//using Windows.Devices.Enumeration;
//using Windows.Devices.WiFiDirect;
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
            //自機のDisplayNameを表示
            textblock.Text+=PeerFinder.DisplayName;

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


            PeerFinder.ConnectionRequested +=new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerFinder_ConnectionRequested);
            //他端末から自機端末がブラウズできるようにする
            PeerFinder.Start();
            
            //同じストアアプリが動いていて、Start()メソッドをコール済みのPCを探す
            //peersはPeerFinderInformationの員スタンす
            var peers = await PeerFinder.FindAllPeersAsync();

            if (peers.Count > 0)
            {
                for (int i = 0; i < peers.Count; i++)
                {
                    //ListBoxにDisplayNameを表示
                    FoundDeviceList.Items.Add(peers[i].DisplayName);
                }

            }
            else
            {
                FoundDeviceList.Items.Add("Device Not Found");
                //var dialog = new MessageDialog("Device Not Found");
                //await dialog.ShowAsync();
            }

        }
        private void PeerFinder_ConnectionRequested(object sender,Windows.Networking.Proximity.ConnectionRequestedEventArgs e)
            {
              Windows.Networking.Proximity.PeerInformation requestingPeer;
              requestingPeer = e.PeerInformation;
            //WriteMessageText("Connection requested by " + requestingPeer.DisplayName + ". " + "Click 'Accept Connection' to connect.");
            }

    }
}
