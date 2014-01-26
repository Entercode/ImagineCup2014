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

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace App2
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

        private async void Button_Clicked(object sender, RoutedEventArgs e)
        {
            //DeviceSelector取得
            String deviceSelector = WiFiDirectDevice.GetDeviceSelector();
            //WiFiDirectDeviceを列挙
            DeviceInformationCollection deviceCollection = await DeviceInformation.FindAllAsync(deviceSelector);
            if (deviceCollection.Count > 0)
            {
        
                for (int i = 0; i < deviceCollection.Count; i++)
                {
                    //インスタンス生成
                    WiFiDirectDevice wfdDevice = await WiFiDirectDevice.FromIdAsync(deviceCollection[i].Id);
                    //ListBoxにDeviceIDを表示
                    wfdDeviceList.Items.Add(wfdDevice.DeviceId);
                }
            }
            else
            {
                wfdDeviceList.Items.Add("Device Not Found");
            }
        }
    }
}
