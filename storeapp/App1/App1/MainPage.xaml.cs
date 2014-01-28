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

            //アプリが動作しているPCでWi-Fi Directが使えるかをチェック
            if ((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) == PeerDiscoveryTypes.Browse) { }
            else
            {
                WriteMessageText("このデバイスはWi-Fi Directに対応してないよ");
            }

            //他端末から自機端末がブラウズできるようにする
            PeerFinder.Start();

            //InitializeProximity();

            //GetDefaulがNULLになっちゃう
            proximityDevice = Windows.Networking.Proximity.ProximityDevice.GetDefault();

            if (proximityDevice != null)
            {
                proximityDevice.DeviceArrived += ProximityDeviceArrived;
                proximityDevice.DeviceDeparted += ProximityDeviceDeparted;
            }
            else
            {
                WriteMessageText("Failed to initialize proximity device.\n");
            }
            
        }
        
        //Searchボタンのイベント関数
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //Connectの設定、検索するだけで接続はしないので不要?
            //PeerFinder.ConnectionRequested +=new TypedEventHandler<object, ConnectionRequestedEventArgs>(PeerFinder_ConnectionRequested);
            

            //他端末から自機端末がブラウズできるようにする
            PeerFinder.Start();
            
            //同じストアアプリが動いていて、Start()メソッドをコール済みのPCを探す
            //peersはPeerFinderInformationのインスタンス
            var peers = await PeerFinder.FindAllPeersAsync();

            if (peers.Any())
            {
                foreach (var i in peers)
                {
                    //FoundDeviceList.Items.Add(i.Id);
                    WriteMessageText("Found Peer ID is " + i.Id + "Time is " + DateTime.Now + "\n");
                }

            }
            else
            {
                //FoundDeviceList.Items.Add("Device Not Found");
                WriteMessageText("Device Not Found.\n"+"Time is "+DateTime.Now+"\n");
            }

        }

        //ConnectionRequestedの設定をする関数、Connectしないので不要?
        private void PeerFinder_ConnectionRequested(object sender,Windows.Networking.Proximity.ConnectionRequestedEventArgs e)
        {
              Windows.Networking.Proximity.PeerInformation requestingPeer;
              requestingPeer = e.PeerInformation;
              WriteMessageText("Connection requested by " + requestingPeer.DisplayName + ". " + "Click 'Accept Connection' to connect.");
        }

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

        //
        private async void InitializeProximity()
        {
            string selectorString = Windows.Networking.Proximity.ProximityDevice.GetDeviceSelector();

            var deviceInfoCollection =
                await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(selectorString, null);

            if (deviceInfoCollection.Count == 0)
            {
                WriteMessageText("No proximity devices found.\n");
            }
            else
            {
                WriteMessageText("Proximity Device id = " + deviceInfoCollection[0].Id+"\n");
                proximityDevice =
                    Windows.Networking.Proximity.ProximityDevice.FromId(deviceInfoCollection[0].Id);
            }
        }


        // Write a message to MessageBlock on the UI thread.
        private Windows.UI.Core.CoreDispatcher messageDispatcher = Window.Current.CoreWindow.Dispatcher;

        //MessageBlockに非同期でメッセージを書き込む関数
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
