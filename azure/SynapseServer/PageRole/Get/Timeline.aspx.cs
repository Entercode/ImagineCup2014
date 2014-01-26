using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Data;
using Microsoft.WindowsAzure.ServiceRuntime;

using SynapseServer;

namespace PageRole.Get
{
	public partial class Timeline : ExtendedPage
	{
		XDocument result = new XDocument();
		bool isValid = false;

		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize(null);

			ShowPostedDataResponse();

			RunProcess((WriteLine, data, file, bindId) =>
			{
				XNamespace space = "http://synapse-server.cloudapp.net/Timeline";
				XNamespace empty = "";
				var timeline = new XElement(space + "Timeline");
				var tweetData = new XElement(empty + "TweetData");

				string timelineQuery = "SELECT * FROM Timeline(@BindId, @Param) ORDER BY TweetTime DESC";
				Helper.ExecuteSqlQuery(timelineQuery,
					setAction: (param) =>
					{
						param.Add("@BindId", SqlDbType.Int).Value = bindId;
						param.Add("@Param", SqlDbType.Float).Value = double.Parse(RoleEnvironment.GetConfigurationSettingValue("PassedTimeParameter"));
					},
					getAction: (reader) =>
					{
						while (reader.Read())
						{
							string userId = reader["UserId"] as string;
							string nickname = reader["Nickname"] as string;
							string tweetTime = Helper.StringConvertOfDateTimeToNumber(reader["TweetTime"].ToString());
							string tweet = reader["Tweet"] as string;
							tweetData.Add(new XElement("Tweet", new XAttribute("UserId", userId), new XAttribute("Nickname", nickname), new XAttribute("Time", tweetTime), tweet));
						}
					});
				timeline.Add(tweetData);
				result.Add(timeline);

				var metaData = new XElement(empty + "MetaData");
				var updateTime = new XElement("UpdateTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
				metaData.Add(updateTime);
				timeline.Add(metaData);

				XmlSchemaSet schemas=new XmlSchemaSet();
				isValid = true;
				schemas.Add(XmlSchema.Read(new XmlTextReader(Server.MapPath("~/App_Data/TimelineSchema.xsd")), (_sender, _e) =>
				{
					isValid = false;
				}));
				result.Validate(schemas, (_sender, _e) =>
				{
					isValid = false;
				});
			},
			checkSession: true);

			if (isValid)
			{
				Response.Clear();
				Response.Write(result.ToString());
			}
		}

		private class TweetColom
		{
			public string UserId { get; set; }
			public string Nickname { get; set; }
			public string ClientTweetTime { get; set; }
			public string Tweet { get; set; }
		}
	}
}