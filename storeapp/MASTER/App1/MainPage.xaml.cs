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

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;


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

            //海野からもらったデバイスIDをDisplayNameに格納
            PeerFinder.DisplayName = "Fuck";
            
            //DisplayNameの表示
            textblock.Text += PeerFinder.DisplayName;

            //アプリが動作しているPCでWi-Fi Directが使えるかをチェック
            if ((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) == PeerDiscoveryTypes.Browse) { }
            else
            {
                WriteMessageText("このデバイスはWi-Fi Directに対応してないよ");
            }

            //多端末からブラウズできるようにする
            PeerFinder.Start();
        }
        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //同じアプリが動いていて、Start()メソッドをコール済みのPCのPeerInformationのリストを取得
            var peers = await PeerFinder.FindAllPeersAsync();

            if (peers.Any())
            {
                foreach (var i in peers)
                {
                    WriteMessageText("Found Device Id is " + i.DisplayName
　                                    +"\nFound Time is " +　DateTime.Now.ToString("yyyyMMddHHmmss") + "\n");
                }
            }
            else
            {
                WriteMessageText("Device Not Found.\n");
            }
        }
        
        //UIスレッドへのディスパッチャ
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
