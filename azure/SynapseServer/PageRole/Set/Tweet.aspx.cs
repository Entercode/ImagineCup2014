using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using SynapseServer;

namespace PageRole
{
	public partial class Tweet : ExtendedPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize(new[] { Helper.ClientTweetTime, Helper.Tweet });

			ShowPostedDataResponse();

			RunProcess((WriteLine, data, file, bindId) =>
			{
				string tweetQuery = "INSERT Tweet(BindId, ClientTweetTime, ServerRecievedTime, Tweet) VALUES(@BindId, @ClientTweetTime, @ServerRecievedTime, @Tweet)";

				Helper.ExecuteSqlQuery(tweetQuery,
					setAction: (param) =>
					{
						param.Add("@BindId", SqlDbType.Int).Value = bindId;
						param.Add("@ClientTweetTime", SqlDbType.DateTime2).Value = Helper.StringConvertOfNumberToDateTime(data[Helper.ClientTweetTime]);
						param.Add("@ServerRecievedTime", SqlDbType.DateTime2).Value = DateTime.Now.ToString();
						param.Add("@Tweet", SqlDbType.NVarChar).Value = data[Helper.Tweet];
					});
				WriteLine("Tweet Query Finished.");

				WriteLine("Saved new tweet.");
			},
			checkSession: true);
		}
	}
}