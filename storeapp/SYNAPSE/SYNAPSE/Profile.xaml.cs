using SYNAPSE.Common;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net;
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
using Windows.UI.Popups;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage.FileProperties;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Windows.Networking.BackgroundTransfer;

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
        private string fileName;
        

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

        //async private void ProfileImage_doubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        //{
            /*
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
            fileName = file.Name;
            //var messageDialog = new MessageDialog(fileName);
            //await messageDialog.ShowAsync();
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

                Uri targetAdresse = new Uri("http://synapse-server.cloudapp.net/Set/Profile.aspx");
                //cookieの設定
                
                HttpCookie cookie = new HttpCookie("sid", domainValue, "");
                cookie.Value = sidValue;
                cookie.Secure = false;
                cookie.HttpOnly = false;
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                var replaced = filter.CookieManager.SetCookie(cookie, false);

                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundaryByte = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                BasicProperties bp = await file.GetBasicPropertiesAsync();
                string formDataTemplate = "name = \"{0}\"\r\n\r\n{1}";
                string headerTemplate = "name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "prfimg", fileName, file.ContentType);
                string formItem = string.Format(formDataTemplate, "prfimg", file);
                byte[] fdata = new byte[bp.Size];
                byte[] headerByte = System.Text.Encoding.UTF8.GetBytes(header);
                byte[] formItemByte = System.Text.Encoding.UTF8.GetBytes(formItem);
                byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                IInputStream istream = filestream.GetInputStreamAt(0);
                DataReader reader = new DataReader(istream);
                await reader.LoadAsync((uint)fdata.Length);
                reader.ReadBytes(fdata);
                //profileText.Text = System.Text.Encoding.UTF8.GetString(fdata,0,fdata.Length);

                byte[] data = new byte[headerByte.Length + fdata.Length];
                Array.Copy(headerByte, 0, data,0, headerByte.Length);
                Array.Copy(fdata, 0, data,headerByte.Length, fdata.Length);

                MemoryStream mem = new MemoryStream(data);
                IInputStream rstream = mem.AsInputStream();

                /*System.Net.Http.HttpContent hcontent = new System.Net.Http.ByteArrayContent(data);
                System.Net.Http.HttpRequestMessage requestMessage = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post,targetAdresse);
                System.Net.Http.MultipartFormDataContent content = new System.Net.Http.MultipartFormDataContent(boundary);
                content.Add(hcontent);
                requestMessage.Content = content;
                System.Net.Http.HttpResponseMessage responseMessage;
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                responseMessage = await client.SendAsync(requestMessage);
                profileText.Text = await responseMessage.Content.ReadAsStringAsync();*/
                //profileText.Text = await content.ReadAsStringAsync();

            /*
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, targetAdresse);
                HttpMultipartFormDataContent content = new HttpMultipartFormDataContent(boundary);
                HttpStreamContent stream = new HttpStreamContent(rstream);
                HttpStringContent stringContent = new HttpStringContent("aaaaaaa");
                content.Add(stream);
                content.Add(stringContent, "prf");
                requestMessage.Content = content;

                HttpResponseMessage response;
                HttpClient client = new HttpClient();
                //response = await client.SendRequestAsync(requestMessage);
                //profileText.Text = await response.Content.ReadAsStringAsync();
                profileText.Text = await requestMessage.Content.ReadAsStringAsync();
            }
             */ 
        //}

        async private void SendButton_clik(object sender, RoutedEventArgs e)
        {
            Uri targetAdresse = new Uri("http://synapse-server.cloudapp.net/Set/Profile.aspx");
            //コンテンツの生成

            //cookieの設定
            HttpCookie cookie = new HttpCookie("sid", domainValue, "");
            cookie.Value = sidValue;
            cookie.Secure = false;
            cookie.HttpOnly = false;
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            var replaced = filter.CookieManager.SetCookie(cookie, false);

            HttpResponseMessage profileResponse;
            HttpClient clinet = new HttpClient();

            HttpFormUrlEncodedContent profileContent = new HttpFormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("prf",profileText.Text),
            });

            profileResponse = await clinet.PostAsync(targetAdresse, profileContent);

            //wifiがつながれているか確認
            if (profileContent == null)
            {
                var messageDialog = new MessageDialog("端末がインターネットにつながっていません", "ネットワークエラー");
                await messageDialog.ShowAsync();
                return;
            }

            if(profileResponse.IsSuccessStatusCode)
            {
                string str = await profileResponse.Content.ReadAsStringAsync();
                if(str.Contains("Edited profile"))
                {

                }else if(str.Contains("Error has occured"))
                {
                    var messageDialog = new MessageDialog("プロフィールの更新に失敗しました", "ネットワークエラー");
                    await messageDialog.ShowAsync();
                    return;
                }
            }
            
        }
    }

}

