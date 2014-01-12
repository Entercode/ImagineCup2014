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
	public partial class Login : System.Web.UI.Page
	{
		readonly string[] keys = new string[] { Helper.UserIdHash, Helper.DeviceIdHash, Helper.PasswordHash };

		protected void Page_Load(object sender, EventArgs e)
		{
			Dictionary<string, string> data = keys.ToDictionary(x => x, x => Request.Form.Get(x));

			Response.Write("I recieved...<br />\n");
			foreach (var pair in data)
			{
				Response.Write(pair.Key + ":" + pair.Value + "<br />\n");
			}

			//POSTされたkeysに対応する値がすべて正しく受信できている場合
			if (!data.Any(x => string.IsNullOrEmpty(x.Value)))
			{
				try
				{
					//string checkPasswordQuery = string.Format("IF (SELECT A.PasswordHash FROM Account A WHERE CONVERT(NVARCHAR(40), HashBytes('SHA1', A.UserId), 2) = '{0}') = '{1}' SELECT 1 AS Result ELSE SELECT 0 AS Result", data[Helper.UserIdHash], data[Helper.PasswordHash]);
					string checkPasswordQuery = @"IF (SELECT A.PasswordHash FROM Account A WHERE HASHBYTES('SHA1', A.UserId) = CONVERT(varbinary, @UserIdHash, 2)) = CONVERT(varbinary, @PasswordHash, 2) SELECT 1 AS Result ELSE SELECT 0 AS Result";
					bool isPasswordOK = false;

					Helper.ExecuteSqlQuery(checkPasswordQuery,
						setAction: (param) =>
						{
							param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = data[Helper.UserIdHash];
							param.Add("@PasswordHash", SqlDbType.VarChar, 40).Value = data[Helper.PasswordHash];
						},
						getAction: (reader) =>
						{
							if (reader.Read())
							{
								isPasswordOK = int.Parse(reader["result"].ToString()) == 1;
							}
						});
					Response.Write("Check Password Finished.<br />\n");

					if (isPasswordOK)
					{
						byte[] sessionId = Helper.StringHashing(DateTime.Now.ToString() + data[Helper.DeviceIdHash]);
						//string loginQuery = string.Format("UPDATE AccountDevice SET SessionId = '{0}' WHERE CONVERT(NVARCHAR(40), HashBytes('SHA1', A.UserId) = '{1}' AND DeviceIdHash = '{2}')", sessionId, data[Helper.UserIdHash], data[Helper.DeviceIdHash]);
						string loginQuery = @"UPDATE AccountDevice SET SessionId = @SessionId WHERE HASHBYTES('SHA1', UserId) = CONVERT(varbinary, @UserIdHash, 2) AND DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)";
						bool isLogin = false;
						Helper.ExecuteSqlQuery(loginQuery,
							setAction: (param) =>
							{
								param.Add("@SessionId", SqlDbType.VarBinary).Value = sessionId;
								param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = data[Helper.UserIdHash];
								param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.DeviceIdHash];
							}, getAction: (reader) =>
							{
								isLogin = reader.RecordsAffected == 1;
							});
						Response.Write("Make Session Finished.<br />\n");
						if (isLogin)
						{
							Response.Write("Login successed.<br />\n");
							Response.Write("your SessionId:#" + Helper.BytesToString(sessionId) + "#<br />\n");
						}
						else
						{
							Response.Write("Login failed.<br />\n");
						}
					}
					else
					{
						Response.Write("Login failed.<br />\n");
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
				Response.Write("But necessary data lacks.\n");
				Response.Write(Helper.WriteNeedKeys(keys));
			}
		}
	}
}