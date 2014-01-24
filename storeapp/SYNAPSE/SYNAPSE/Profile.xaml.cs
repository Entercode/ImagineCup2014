﻿using SYNAPSE.Common;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// 基本ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234237 を参照してください

namespace SYNAPSE
{
    /// <summary>
    /// 多くのアプリケーションに共通の特性を指定する基本ページ。
    /// </summary>
    public sealed partial class Profile : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private IRandomAccessStream filestream;
        private BitmapImage bitmapImage;
        private string sidValue;
        private string domainValue;

        /// <summary>
        /// これは厳密に型指定されたビュー モデルに変更できます。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper は、ナビゲーションおよびプロセス継続時間管理を
        /// 支援するために、各ページで使用します。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public Profile()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        /// <param name="sender">
        /// イベントのソース (通常、<see cref="NavigationHelper"/>)>
        /// </param>
        /// <param name="e">このページが最初に要求されたときに
        /// <see cref="Frame.Navigate(Type, Object)"/> に渡されたナビゲーション パラメーターと、
        /// 前のセッションでこのページによって保存された状態の辞書を提供する
        /// セッション。ページに初めてアクセスするとき、状態は null になります。</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            ApplicationDataContainer localSid = ApplicationData.Current.LocalSettings;
            ApplicationDataContainer localDomain = ApplicationData.Current.LocalSettings;
            if (localSid.Values.ContainsKey("sid"))
            {
                sidValue = localSid.Values["sid"].ToString();
            }
            if (localDomain.Values.ContainsKey("Domain"))
            {
                domainValue = localDomain.Values["Domain"].ToString();
            }
        }

        /// <summary>
        /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
        /// このページに関連付けられた状態を保存します。値は、
        /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
        /// </summary>
        /// <param name="sender">イベントのソース (通常、<see cref="NavigationHelper"/>)</param>
        /// <param name="e">シリアル化可能な状態で作成される空のディクショナリを提供するイベント データ
        ///。</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper の登録

        /// このセクションに示したメソッドは、NavigationHelper がページの
        /// ナビゲーション メソッドに応答できるようにするためにのみ使用します。
        /// 
        /// ページ固有のロジックは、
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// および <see cref="GridCS.Common.NavigationHelper.SaveState"/> のイベント ハンドラーに配置する必要があります。
        /// LoadState メソッドでは、前のセッションで保存されたページの状態に加え、
        /// ナビゲーション パラメーターを使用できます。

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        async private void ProfileImage_doubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            //ファイルピッカーオブジェクトの生成
            FileOpenPicker openPicker = new FileOpenPicker();
            //ディレクトリの設定
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //ビューモードの設定
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            //開けるファイルの種類の設定
            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            //ファイルを一つ選べるようにする。
            StorageFile file = await openPicker.PickSingleFileAsync();
            //画像の表示開始
            if(file != null)
            {
                //streamを開く
                filestream = await file.OpenAsync(FileAccessMode.Read);
                //画像をbitmapにセットする
                bitmapImage = new BitmapImage();
                bitmapImage.SetSource(filestream);
                //ApplicationDataContainer prfimg = ApplicationData.Current.LocalSettings;
                //prfimg.Values["prfimg"] = bitmapImage;
                if(bitmapImage.PixelHeight >64 && bitmapImage.PixelWidth > 64)
                {
                    return;
                }
                profileimage.Source = bitmapImage;
                

            }

        }

        async private void SendButton_clik(object sender, RoutedEventArgs e)
        {
            Uri targetAdresse = new Uri("http://synapse-server.cloudapp.net/Set/Profile.aspx");
            try
            {
                //コンテンツの生成
                HttpMultipartFormDataContent content = new HttpMultipartFormDataContent();
                content.Add(new HttpStringContent(profileText.Text), "prf");
                content.Add(new HttpStreamContent(filestream), "prfimg");

                HttpCookie cookie = new HttpCookie("sid", domainValue, "");
                cookie.Value = sidValue;
                cookie.Secure = false;
                cookie.HttpOnly = false;
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                var replaced = filter.CookieManager.SetCookie(cookie, false);
                
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post,targetAdresse);
                requestMessage.Content = content;

                HttpClient Client = new HttpClient();
                HttpResponseMessage responseMessage;
                //responseMessage = await Client.PostAsync(targetAdresse, content);
                //result.Text = await responseMessage.Content.ReadAsStringAsync();
                responseMessage = await Client.SendRequestAsync(requestMessage);
                if(responseMessage == null)
                {
                    result.Text = "失敗\n";
                }
                result.Text = await responseMessage.Content.ReadAsStringAsync();
            }
            catch
            {
                //return;
            }

        }
    }
}
