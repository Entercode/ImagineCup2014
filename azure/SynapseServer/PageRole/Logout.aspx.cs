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
	public partial class Logout : System.Web.UI.Page
	{
		readonly string[] keys = new string[] { Helper.UserIdHash, Helper.DeviceIdHash, Helper.SessionId };

		protected void Page_Load(object sender, EventArgs e)
		{
			Dictionary<string, string> data = keys.ToDictionary(x => x, x => Request.Form.Get(x));

			Response.Write("I recieved...<br />\n");
			foreach (var pair in data)
			{
				Response.Write(pair.Key + ":" + pair.Value + "<br />\n");
			}

			if (!data.Any(x => string.IsNullOrEmpty(x.Value)))
			{
				try
				{
					//string checkSessionQuery = string.Format("IF (SELECT A.SessionId FROM Account A WHERE CONVERT(NVARCHAR(40), HashBytes('SHA1', A.UserId), 2) = '{0}' AND A.DeviceIdHash = '{1}') = '{2}' SELECT 1 AS Result ELSE SELECT 0 AS Result", data[Helper.UserIdHash], data[Helper.DeviceIdHash], data[Helper.SessionId]);
					string checkSessionQuery = @"IF (SELECT D.SessionId FROM AccountDevice D WHERE HASHBYTES('SHA1', D.UserId) = CONVERT(varbinary, @UserIdHash, 2) AND D.DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)) = CONVERT(varbinary, @SessionId, 2) SELECT 1 AS Result ELSE SELECT 0 AS Result";
					bool isLogout = false;

					Helper.ExecuteSqlQuery(checkSessionQuery,
						setAction: (param) =>
						{
							param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = data[Helper.UserIdHash];
							param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.DeviceIdHash];
							param.Add("@SessionId", SqlDbType.VarChar, 40).Value = data[Helper.SessionId];
						},
						getAction: (reader) =>
						{
							if (reader.Read())
							{
								isLogout = int.Parse(reader["result"].ToString()) == 1;
							}
						});
					Response.Write("Check Session Finished.<br />\n");

					if (isLogout)
					{
						//string logoutQuery = string.Format("UPDATE AccountDevice SET SessionId = NULL WHERE CONVERT(NVARCHAR(40), HashBytes('SHA1', A.UserId), 2) = '{0}' AND DeviceIdHash = '{1}')", data[Helper.UserIdHash], data[Helper.DeviceIdHash]);
						string logoutQuery = @"UPDATE AccountDevice SET SessionId = NULL WHERE HASHBYTES('SHA1', UserId) = CONVERT(varbinary, @UserIdHash, 2) AND DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)";
						Helper.ExecuteSqlQuery(logoutQuery,
							setAction: (param) =>
							{
								param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = data[Helper.UserIdHash];
								param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.DeviceIdHash];
							});
						Response.Write("Logout Query Finished.<br />\n");

						Response.Write("Logout successed.<br />\n");
					}
					else
					{
						Response.Write("Logout failed.<br />\n");
					}
				}
				catch (Exception ex)
				{
					Response.Write("#Error has occured<br />#n");
					Response.Write(ex.Message);
				}
			}
			else
			{
				Response.Write("But necessary data lacks.<br />\n");
				Response.Write(Helper.WriteNeedKeys(keys));
			}
		}
	}
}