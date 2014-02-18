using SYNAPSE.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Networking.Proximity;
using System.Threading.Tasks;


// 基本ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234237 を参照してください

namespace SYNAPSE
{
    /// <summary>
    /// 多くのアプリケーションに共通の特性を指定する基本ページ。
    /// </summary>
    public sealed partial class UserInformationPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        Frame rootFrame;

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


        public UserInformationPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            rootFrame = Window.Current.Content as Frame;
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

        async private void SignUpButton_clik(object sender, RoutedEventArgs e)
        {
            //例外処理
            if(username.Text == "")
            {
                var messageDialog = new MessageDialog("ユーザーネームが入力されていません", "エラー");
                await messageDialog.ShowAsync();
                return;
            }
            if(nickname.Text == "")
            {
                var messageDialog = new MessageDialog("ニックネームが入力されていません", "エラー");
                await messageDialog.ShowAsync();
                return;
            }
            if(mailad.Text == "")
            {
                var messageDialog = new MessageDialog("メールアドレスが入力されていません", "エラー");
                await messageDialog.ShowAsync();
                return;
            }
            if(password.Password == "")
            {
                var messageDialog = new MessageDialog("パスワードが入力されていません", "エラー");
                await messageDialog.ShowAsync();
                return;
            }
            if(password.Password != passwordCheck.Password)
            {
                var messageDialog = new MessageDialog("パスワードが一致しません", "エラー");
                await messageDialog.ShowAsync();
                return;
            }
            //デバイスIDの取得開始
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(token.Id);
            byte[] bytes = new byte[token.Id.Length];
            dataReader.ReadBytes(bytes);
            var id = BitConverter.ToString(bytes);

            /*
            //デバイスIDをファイルに出力
            StorageFolder storageFolder = KnownFolders.MusicLibrary;
            StorageFile storageFile = await storageFolder.CreateFileAsync("deviceID.txt",CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(storageFile, id, UnicodeEncoding.Utf8);
            */


            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            Uri ressoceAddress = new Uri("http://synapse-server.cloudapp.net/Set/SignUp.aspx");
            
            //ハッシュ生成
            var algorithm = HashAlgorithmProvider.OpenAlgorithm("SHA1");
            IBuffer did = CryptographicBuffer.ConvertStringToBinary(id, BinaryStringEncoding.Utf8);
            IBuffer pass = CryptographicBuffer.ConvertStringToBinary(password.Password, BinaryStringEncoding.Utf8);
            var hash_did = algorithm.HashData(did);
            var hash_pass = algorithm.HashData(pass);
            string did_h = CryptographicBuffer.EncodeToHexString(hash_did);
            string pass_h = CryptographicBuffer.EncodeToHexString(hash_pass);

            //デバイスIDをアプリデータとして保存
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["did_h"] = did_h;

            HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("uid",username.Text),
                new KeyValuePair<string,string>("nn",nickname.Text),
                new KeyValuePair<string,string>("mail",mailad.Text),
                new KeyValuePair<string,string>("pass",password.Password),
                new KeyValuePair<string,string>("did_h",did_h),

            });

            try
            {
                //await singUpAsync(ressoceAddress, content);
                response = await client.PostAsync(ressoceAddress, content);
                //result.Text = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                //result.Text = "ユーザー登録失敗\n";
                return;
            }
            Uri loginAddress = new Uri("http://synapse-server.cloudapp.net/Set/Login.aspx");
            HttpResponseMessage loginResponse = null;
            HttpFormUrlEncodedContent loginContent = new HttpFormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("uid",username.Text),
                new KeyValuePair<string,string>("pass_h",pass_h),
                new KeyValuePair<string,string>("did_h",did_h),

            });
            try
            {
                loginResponse = await client.PostAsync(loginAddress, loginContent);
                //RunAsync(response, loginAddress, loginContent);
            }
            catch
            {

            }

            //非同期処理が完了したかどうかの処理
            while(true)
            {
                
                if(loginResponse.IsSuccessStatusCode)
                {
                    break;
                }
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
            string x = await loginResponse.Content.ReadAsStringAsync();
            while(true)
            {
                if(x.Contains("Login successed"))
                {
                    break;
                }
                else if (x.Contains("Error has occured"))
                {
                    var message = new MessageDialog("サインアップに失敗しました。","エラー");
                    await message.ShowAsync();
                    return;
                }
                await Task.Delay(TimeSpan.FromMilliseconds(100));

            }
            //cookieの取得
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(loginAddress);

            foreach (HttpCookie cookie in cookieCollection)
            {
                result.Text += "--------------------\r\n";
                result.Text += "Name: " + cookie.Name + "\r\n";
                result.Text += "Domain: " + cookie.Domain + "\r\n";
                result.Text += "Path: " + cookie.Path + "\r\n";
                result.Text += "Value: " + cookie.Value + "\r\n";
                result.Text += "Expires: " + cookie.Expires + "\r\n";
                result.Text += "Secure: " + cookie.Secure + "\r\n";
                result.Text += "HttpOnly: " + cookie.HttpOnly + "\r\n";
                //valueｎ保存
                ApplicationDataContainer localSid = ApplicationData.Current.LocalSettings;
                ApplicationDataContainer localDomain = ApplicationData.Current.LocalSettings;
                localSid.Values["sid"] = cookie.Value;
                localDomain.Values["Domain"] = cookie.Domain;
            }
            //result.Text = await loginResponse.Content.ReadAsStringAsync();
            rootFrame.Navigate(typeof(TimeLine));
        }

    }
}
