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

using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace App4
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private GattDeviceService service;
        private GattCharacteristic characteristic;

        private async void Button_Click(object sender,RoutedEventArgs e)
        {
            //デバイスを検索 (特定のサービスを公開しているBLE機器を列挙)
            var devices = await DeviceInformation.FindAllAsync(
                GattDeviceService.GetDeviceSelectorFromUuid(service.Uuid));
            if(devices.Count > 0)
            {
                //サービスを作成 (デバイス取得したIDをもとにサービスを扱うインスタンスを取得)
                this.service = await GattDeviceService.FromIdAsync(devices.First().Id);

                //キャラクタリスティックを取得
                var characteristics = service.GetCharacteristics(new Guid("84F72593-E746-44D1-8861-DEA0D61FA864"));
                if(characteristics.Count > 0)
                {
                    this.characteristic = characteristics.First();

                    //通知イベントを登録
                    //characteristic.ValueChanged += characteristic_ValueChanged;

                    var dialog = new MessageDialog("Connected!");
                    await dialog.ShowAsync();
                }
            }
            else
            {
                var dialog = new MessageDialog("Device not found");
                await dialog.ShowAsync();
            }
        }

        /*void characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var buffer = args.CharacteristicValue.ToArray();
            if (buffer.Last() == 1)
            {
                leftTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                rightTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else if (buffer.Last() == 2)
            {
                leftTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                rightTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                leftTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                rightTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
        */


        public Guid serviceUuid { get; set; }
    }
}
