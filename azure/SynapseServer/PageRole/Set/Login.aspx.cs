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
	public partial class Login : ExtendedPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize(new[] { Helper.UserIdHash, Helper.DeviceIdHash, Helper.PasswordHash });

			ShowPostedDataResponse();

			RunProcess((WriteLine, data, file, bindId) =>
			{
				string checkPasswordQuery
					= "IF "
					+ "(SELECT A.PasswordHash FROM Account A WHERE HASHBYTES('SHA1', A.UserId) = CONVERT(varbinary, @UserIdHash, 2)) = CONVERT(varbinary, @PasswordHash, 2) "
					+ "SELECT 1 AS Result "
					+ "ELSE "
					+ "SELECT 0 AS Result";
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
				WriteLine("Check Password Finished.");

				if (isPasswordOK)
				{
					byte[] sessionId = Helper.StringHashing(DateTime.Now.ToString() + data[Helper.DeviceIdHash]);
					string loginQuery
						= "UPDATE AccountDevice SET SessionId = @SessionId "
						+ "WHERE HASHBYTES('SHA1', UserId) = CONVERT(varbinary, @UserIdHash, 2) AND DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)";
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
					WriteLine("Make Session Finished.");

					if (isLogin)
					{
						WriteLine("Login successed.");
						Response.Cookies[Helper.SessionId].Value = Helper.BytesToString(sessionId);
						//WriteLine("your SessionId:" + Helper.BytesToString(sessionId) + "");
					}
					else
					{
						WriteLine("Login failed.");
					}
				}
				else
				{
					WriteLine("Login failed.");
				}
			});
		}
	}
}