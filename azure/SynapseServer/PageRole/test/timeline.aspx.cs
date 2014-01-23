using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml.Linq;

using SynapseServer;

namespace PageRole.test
{
	public partial class timeline : System.Web.UI.Page
	{
		string userIdHash;
		string sessionId;
		string url;
		protected void Page_Load(object sender, EventArgs e)
		{
			userIdHash = Request.QueryString.Get(Helper.UserIdHash);
			sessionId = Request.QueryString.Get(Helper.SessionId);
			url = "http://" + Request.Url.Host + "/Get/Timeline.aspx";
			if (string.IsNullOrEmpty(userIdHash) || string.IsNullOrEmpty(sessionId))
			{
				Response.Write("ログインしていないので表示できません。。。");
				Response.End();
			}
			uid_h.Text = userIdHash;
		}

		protected void TimelineSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var param = e.InputParameters;
			param.Add("UserIdHash", userIdHash);
			param.Add("SessionId", sessionId);
			param.Add("Url", url);
		}
	}

	public class ListViewObject
	{
		public static List<TweetData> GetTimeline(string UserIdHash, string SessionId, string Url)
		{
			var result = new List<TweetData>();
			XDocument xml;
			using (WebClientEx wc = new WebClientEx())
			{
				var cc = new CookieContainer();
				cc.Add(new Cookie("sid", SessionId) { Domain = (new Uri(Url).Host) });
				wc.CookieContainer = cc;
				var col = new System.Collections.Specialized.NameValueCollection();
				col.Add("uid_h", UserIdHash);
				byte[] data = wc.UploadValues(Url, col);
				if (data == null)
				{
					return result;
				}
				xml = XDocument.Parse(System.Text.Encoding.UTF8.GetString(data));
			}
			var root = xml.Root; ;
			result = root.Element("TweetData").Elements("Tweet").Select(x => new TweetData()
			{
				UserId = (string)x.Attribute("UserId"),
				Nickname = (string)x.Attribute("Nickname"),
				Time = Helper.StringConvertOfNumberToDateTime((string)x.Attribute("Time")),
				Tweet = (string)x,
				UserIdHash = Helper.BytesToString(Helper.StringHashing((string)x.Attribute("UserId")))
			}).ToList();
			return result;
		}

		class WebClientEx : WebClient
		{
			public CookieContainer CookieContainer { get; set; }

			protected override WebRequest GetWebRequest(Uri address)
			{
				var req = (HttpWebRequest)base.GetWebRequest(address);
				req.CookieContainer = this.CookieContainer;
				return req;
			}
		}
	}

	public class TweetData
	{
		public string UserId { get; set; }
		public string Nickname { get; set; }
		public string Time { get; set; }
		public string Tweet { get; set; }
		public string UserIdHash { get; set; }
	}
}