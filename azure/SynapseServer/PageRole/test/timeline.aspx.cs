using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;

using SynapseServer;

namespace PageRole.test
{
	public partial class timeline : System.Web.UI.Page
	{
		string userId;
		string deviceIdHash;
		string url;

		public string UserId
		{
			get
			{
				return userId;
			}
		}
		public string DeviceIdHash
		{
			get
			{
				return deviceIdHash;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			userId = Request.QueryString.Get(Helper.UserId);
			deviceIdHash = Request.QueryString.Get(Helper.DeviceIdHash);
			url = "http://" + Request.Url.Host + "/Get/Timeline.aspx";
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(deviceIdHash))
			{
				Response.Write("異常です。");
				Response.End();
			}
		}

		protected void TimelineSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			var param = e.InputParameters;
			param.Add("UserId", userId);
			param.Add("DeviceIdHash", deviceIdHash);
			param.Add("Url", url);
		}
	}

	public class ListViewObject
	{
		public static List<TweetData> GetTimeline(string UserId, string DeviceIdHash, string Url)
		{
			var result = new List<TweetData>();
			using (WebClientEx wc = new WebClientEx())
			{
				string query = "SELECT * FROM Timeline((SELECT BindId FROM AccountDevice WHERE UserId=@UserId AND DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)), @Param) ORDER BY TweetTime DESC";
				Helper.ExecuteSqlQuery(query,
					setAction: (param) =>
					{
						param.Add("@UserId", SqlDbType.VarChar).Value = UserId;
						param.Add("@DeviceIdHash", SqlDbType.VarChar).Value = DeviceIdHash;
						param.Add("@Param", SqlDbType.Float).Value = double.Parse(RoleEnvironment.GetConfigurationSettingValue("PassedTimeParameter"));
					},
					getAction: (reader) =>
					{
						while (reader.Read())
						{
							string userId = reader["UserId"] as string;
							string nickname = reader["Nickname"] as string;
							string tweetTime = reader["TweetTime"].ToString();
							string tweet = reader["Tweet"] as string;
							result.Add(new TweetData() { UserId = userId, Nickname = nickname, Time = tweetTime, Tweet = tweet, UserIdHash = Helper.BytesToString(Helper.StringHashing(userId)) });
						}
					});
				return result;
			}
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