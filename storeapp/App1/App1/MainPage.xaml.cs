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
using Windows.Networking.Proximity;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace App1
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Windows.Networking.Proximity.ProximityDevice proximityDevice;
        public MainPage()
        {
            this.InitializeComponent();
            //自機のDisplayNameを表示
            textblock.Text += PeerFinder.DisplayName;
            
        }
        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            proximityDevice = Windows.Networking.Proximity.ProximityDevice.GetDefault();
            if (proximityDevice != null)
            {
                proximityDevice.DeviceArrived += ProximityDeviceArrived;
                proximityDevice.DeviceDeparted += ProximityDeviceDeparted;

                WriteMessageText("Proximity device initialized.\n");
            }
            else
            {
                WriteMessageText("Failed to initialized proximity device.\n");
            }
           


            //アプリが動作しているPCでWi-Fi Directが使えるかをチェック
            if ((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) == PeerDiscoveryTypes.Browse) { }
            else {
                var dialog = new MessageDialog("このデバイスはWi-FiDirectに対応してないよ");
                await dialog.ShowAsync();
            }

            //PeerFinder.ConnectionRequested +=new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerFinder_ConnectionRequested);
            //他端末から自機端末がブラウズできるようにする
            PeerFinder.Start();
            
            //同じストアアプリが動いていて、Start()メソッドをコール済みのPCを探す
            //peersはPeerFinderInformationの員スタンす
            var peers = await PeerFinder.FindAllPeersAsync();

            if (peers.Any())
            {
                foreach (var i in peers)
                {
                    //FoundDeviceList.Items.Add(i.Id);
                    WriteMessageText("Found Device Id is "+i.Id);
                }

            }
            else
            {
                //FoundDeviceList.Items.Add("Device Not Found");
                WriteMessageText("Device Not Found.\n");
            }

        }
        /*private void PeerFinder_ConnectionRequested(object sender,Windows.Networking.Proximity.ConnectionRequestedEventArgs e)
        {
              Windows.Networking.Proximity.PeerInformation requestingPeer;
              requestingPeer = e.PeerInformation;
              WriteMessageText("Connection requested by " + requestingPeer.DisplayName + ". " + "Click 'Accept Connection' to connect.");
        }*/

        //近接デバイスの出現を検出し、メッセージ
        private void ProximityDeviceArrived(Windows.Networking.Proximity.ProximityDevice device)
        {
            WriteMessageText("Proximate device arrived. id = " + device.DeviceId + "\n");
        }
        //近接デバイスの消滅をけんすつし、メッセージ
        private void ProximityDeviceDeparted(Windows.Networking.Proximity.ProximityDevice device)
        {
            WriteMessageText("Proximate device departed. id = " + device.DeviceId + "\n");
        }
        
        // Write a message to MessageBlock on the UI thread.
        private Windows.UI.Core.CoreDispatcher messageDispatcher = Window.Current.CoreWindow.Dispatcher;

        //MessageBlockにメッセージを書き込む関数
        async private void WriteMessageText(string message, bool overwrite = false)
        {
            await messageDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    if (overwrite)
                        MessageBlock.Text = message;
                    else
                        MessageBlock.Text += message;
                });
        }
    }
}
