using SYNAPSE.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Networking.BackgroundTransfer;
using Windows.Networking.Proximity;

// 基本ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234237 を参照してください

namespace SYNAPSE
{
    /// <summary>
    /// 多くのアプリケーションに共通の特性を指定する基本ページ。
    /// </summary>
    public sealed partial class TimeLine : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private string thisTime;
        private string sidValue;
        private string domainValue;
        string buf;
        StorageFile timeLineBuffer;

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


        public TimeLine()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("did_h"))
            {
                PeerFinder.DisplayName = localSettings.Values["did_h"].ToString();
            }
            //peerFinderの開始
            PeerFinder.Start();
            //一定時間間隔に発生する処理の開始
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 5);
            timer.Start();
            timer.Tick += timer_Tick;
        }

        //一定時間間隔に行う処理
        async void timer_Tick(object sender, object e)
        {
           
            //すれ違い通信の開始
            try
            {
                var peers = await PeerFinder.FindAllPeersAsync();
                if (peers.Any())
                {
                    foreach (var i in peers)
                    {
                        HttpClient client = new HttpClient();
                        //urlの設定
                        Uri streetPassAdress = new Uri("http://synapse-server.cloudapp.net/Set/StreetPass.aspx");

                        HttpResponseMessage responseMessage;
                        HttpFormUrlEncodedContent streetPassContent = new HttpFormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string,string>("pdid_h",i.DisplayName),
                                new KeyValuePair<string,string>("pt",DateTime.Now.ToString("yyyyMMddHHmmss"))
                            });
                        //クッキーのセット
                        HttpCookie cookie = new HttpCookie("sid", domainValue, "");
                        cookie.Value = sidValue;
                        cookie.Secure = false;
                        cookie.HttpOnly = false;
                        HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                        var replaced = filter.CookieManager.SetCookie(cookie, false);
                        responseMessage = await client.PostAsync(streetPassAdress, streetPassContent);


                    }
                }
            }
            catch
            {
            }

            try
            {
                //タイムラインの読み込み
                timeLineBuffer = await ApplicationData.Current.RoamingFolder.GetFileAsync("timeLineBuffer.txt");
            }
            catch
            {
            }

            try
            {
                //タイムラインを文字列として格納
                buf = await FileIO.ReadTextAsync(timeLineBuffer);
                XDocument documentBuf = XDocument.Parse(buf);
                XElement rootBuf = documentBuf.Root;

                //クッキーのセット
                HttpCookie cookie = new HttpCookie("sid", domainValue, "");
                cookie.Value = sidValue;
                cookie.Secure = false;
                cookie.HttpOnly = false;
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                var replaced = filter.CookieManager.SetCookie(cookie, false);

                //キャッシュが重複しないように
                System.DateTime ntime = System.DateTime.Now;

                //タイムライン取得用のアドレス   
                Uri targetAdresse = new Uri("http://synapse-server.cloudapp.net/Get/Timeline.aspx?" + ntime.ToString());

                HttpClient client = new HttpClient();

                //xmlをゲット
                HttpResponseMessage responseMessage = await client.GetAsync(targetAdresse);

                responseMessage.EnsureSuccessStatusCode();
                //比較用にタイムラインを読み込む
                string bufCompare = await responseMessage.Content.ReadAsStringAsync();
                XDocument documentCompare = XDocument.Parse(bufCompare);
                XElement rootCompare = documentCompare.Root;

                //タイムラインに変化があった場合のみ読み込む
                if(rootCompare.Element("TweetData").Element("Tweet").Attribute("Time").Value.ToString()
                    != rootBuf.Element("TweetData").Element("Tweet").Attribute("Time").Value.ToString())
                {
                    timeLineBuffer = await ApplicationData.Current.RoamingFolder.CreateFileAsync("timeLineBuffer.txt", CreationCollisionOption.ReplaceExisting);

                    timeline.ItemsSource = rootCompare.Element("TweetData").Elements("Tweet").Select(x => new
                    {
                        Tweet = x.Value,
                        NickName = x.Attribute("Nickname").Value,
                        Year = x.Attribute("Time").Value.Substring(0, 4) + "年",
                        Month = x.Attribute("Time").Value.Substring(4, 2) + "月",
                        Day = x.Attribute("Time").Value.Substring(6, 2) + "日",
                        Hour = x.Attribute("Time").Value.Substring(8, 2) + "時",
                        Minute = x.Attribute("Time").Value.Substring(10, 2) + "分",
                        Second = x.Attribute("Time").Value.Substring(12, 2) + "秒",
                    });
                }
                await FileIO.WriteTextAsync(timeLineBuffer, bufCompare);
            }
            catch
            {

            }
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
        async private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            ApplicationDataContainer localSid = ApplicationData.Current.LocalSettings;
            ApplicationDataContainer localDomain = ApplicationData.Current.LocalSettings;
            try
            {
               timeLineBuffer = await ApplicationData.Current.RoamingFolder.GetFileAsync("timeLineBuffer.txt");
            }
            catch
            {
            }
            if(localSid.Values.ContainsKey("sid"))
            {
                sidValue = localSid.Values["sid"].ToString();
            }
            if(localDomain.Values.ContainsKey("Domain"))
            {
                domainValue = localDomain.Values["Domain"].ToString();
            }
            try
            {
                buf = await FileIO.ReadTextAsync(timeLineBuffer);
                XDocument document = XDocument.Parse(buf);
                XElement root = document.Root;


                timeline.ItemsSource = root.Element("TweetData").Elements("Tweet").Select(x => new
                {
                    Tweet = x.Value,
                    NickName = x.Attribute("Nickname").Value,
                    Year = x.Attribute("Time").Value.Substring(0, 4) + "年",
                    Month = x.Attribute("Time").Value.Substring(4, 2) + "月",
                    Day = x.Attribute("Time").Value.Substring(6, 2) + "日",
                    Hour = x.Attribute("Time").Value.Substring(8, 2) + "時",
                    Minute = x.Attribute("Time").Value.Substring(10, 2) + "分",
                    Second = x.Attribute("Time").Value.Substring(12, 2) + "秒",
                });
            }
            catch
            {
                
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
        async private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            await FileIO.WriteTextAsync(timeLineBuffer, buf);
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


        private void TweetDone(object sender, KeyRoutedEventArgs e)
        {
            /*switch(e.Key)
            {
                case Windows.System.VirtualKey.Enter:
                    if(e.KeyStatus.RepeatCount == 1)
                    {
                        System.DateTime ntime = System.DateTime.Now;
                        string year = ntime.Year.ToString();
                        string month = ntime.Month.ToString();
                        if (ntime.Month < 10)
                            month = '0' + month;
                        string day = ntime.Day.ToString();
                        if (ntime.Day < 10)
                            day = '0' + day;
                        string hour = ntime.Hour.ToString();
                        if (ntime.Hour < 10)
                            hour = '0' + hour;
                        string minute = ntime.Minute.ToString();
                        if (ntime.Minute < 10)
                            minute = '0' + minute;
                        string second = ntime.Second.ToString();
                        if (ntime.Second < 10)
                            second = '0' + second;
                        thisTime = year + month + day + hour + minute + second;
                        //var messageDialog = new MessageDialog(thisTime);
                        //await messageDialog.ShowAsync();
                        HttpClient client = new HttpClient();
                        Uri targetAdresse = new Uri("http://synapse-server.cloudapp.net/Set/Tweet.aspx");
                        HttpResponseMessage responseMessage;
                        HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string,string>("tt",thisTime),
                            new KeyValuePair<string,string>("tweet",tweetBox.Text),
                        });
                        try
                        {
                            HttpCookie cookie = new HttpCookie("sid", domainValue, "");
                            cookie.Value = sidValue;
                            cookie.Secure = false;
                            cookie.HttpOnly = false;
                            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                            var replaced = filter.CookieManager.SetCookie(cookie, false);
                            responseMessage = await client.PostAsync(targetAdresse, content);
                            tweetBox.Text = await responseMessage.Content.ReadAsStringAsync();
                        }
                        catch
                        {
                            return;
                        }
                    }
                    break;
            }*/
        }

        async private void TweetButton_clik(object sender, RoutedEventArgs e)
        {
            System.DateTime ntime = System.DateTime.Now;
            string year = ntime.Year.ToString();
            string month = ntime.Month.ToString();
            if (ntime.Month < 10)
                month = '0' + month;
            string day = ntime.Day.ToString();
            if (ntime.Day < 10)
                day = '0' + day;
            string hour = ntime.Hour.ToString();
            if (ntime.Hour < 10)
                hour = '0' + hour;
            string minute = ntime.Minute.ToString();
            if (ntime.Minute < 10)
                minute = '0' + minute;
            string second = ntime.Second.ToString();
            if (ntime.Second < 10)
                second = '0' + second;
            thisTime = year + month + day + hour + minute + second;
            //var messageDialog = new MessageDialog(thisTime);
            //await messageDialog.ShowAsync();
            HttpClient client = new HttpClient();
            Uri targetAdresse = new Uri("http://synapse-server.cloudapp.net/Set/Tweet.aspx");
            HttpResponseMessage responseMessage;
            HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string,string>("tt",thisTime),
                            new KeyValuePair<string,string>("tweet",tweetBox.Text),
                        });
            try
            {
                HttpCookie cookie = new HttpCookie("sid", domainValue, "");
                cookie.Value = sidValue;
                cookie.Secure = false;
                cookie.HttpOnly = false;
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                var replaced = filter.CookieManager.SetCookie(cookie, false);
                responseMessage = await client.PostAsync(targetAdresse, content);
                //tweetBox.Text = await responseMessage.Content.ReadAsStringAsync();
            }
            catch
            {
                return;
            }
            this.GetButton_clik(sender,e);
        }

        async private void GetButton_clik(object sender, RoutedEventArgs e)
        {
            /*//クッキーのセット
            HttpCookie cookie = new HttpCookie("sid", domainValue, "");
            cookie.Value = sidValue;
            cookie.Secure = false;
            cookie.HttpOnly = false;
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            var replaced = filter.CookieManager.SetCookie(cookie, false);

            //キャッシュが重複しないように
            System.DateTime ntime = System.DateTime.Now;

            //タイムライン取得用のアドレス   
            Uri targetAdresse = new Uri("http://synapse-server.cloudapp.net/Get/Timeline.aspx?" + ntime.ToString());
            //プロフィール画像取得用のアドレス
            //Uri ProfileGetAdresse = new Uri("http://synapse-server.cloudapp.net/Get/ProfileImage.aspx?=" +ntime.ToString() );
            HttpClient client = new HttpClient();

            //xmlをゲット
            HttpResponseMessage responseMessage = await client.GetAsync(targetAdresse);
            //プロフィール画像のゲット
            //HttpResponseMessage profileResponseMessage = await client.GetAsync(ProfileGetAdresse);
            //BitmapImage bitmapImage = new BitmapImage();
            
            
            responseMessage.EnsureSuccessStatusCode();
            buf = await responseMessage.Content.ReadAsStringAsync();
            //ファイルにbufを保存
            timeLineBuffer = await ApplicationData.Current.RoamingFolder.CreateFileAsync("timeLineBuffer.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(timeLineBuffer,buf);

            XDocument document = XDocument.Parse(buf);
            XElement root = document.Root;
            

            timeline.ItemsSource = root.Element("TweetData").Elements("Tweet").Select(x => new
            {
                Tweet = x.Value,
                NickName = x.Attribute("Nickname").Value,
                Year = x.Attribute("Time").Value.Substring(0,4) + "年",
                Month = x.Attribute("Time").Value.Substring(4,2) + "月",
                Day = x.Attribute("Time").Value.Substring(6,2) + "日",
                Hour = x.Attribute("Time").Value.Substring(8,2) + "時",
                Minute = x.Attribute("Time").Value.Substring(10, 2) + "分",
                Second = x.Attribute("Time").Value.Substring(12, 2) + "秒",
            });*/
        }

        async private void TestButton_clik(object sender, RoutedEventArgs e)
        {
            Uri targetAdresse = new Uri("http://synapse-server.cloudapp.net/Get/ProfileImage.aspx" + "?uid_h=" + "0A63580B9A92F8B78E0241F080AB4A6438756369");
            StorageFile destinationFile = await KnownFolders.PicturesLibrary.CreateFileAsync("test.png",CreationCollisionOption.ReplaceExisting);
            BackgroundDownloader downloader = new BackgroundDownloader();
            downloader.Method = "GET";
            DownloadOperation downloaderOperation = downloader.CreateDownload(targetAdresse, destinationFile);
            await downloaderOperation.StartAsync();

        }

        private void MainPageButton_clik(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

       
    }
}
