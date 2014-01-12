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
	public partial class SignUp : System.Web.UI.Page
	{
		readonly string[] keys = new string[] { Helper.UserId, Helper.Nickname, Helper.MailAddress, Helper.Password, Helper.DeviceIdHash };

		protected void Page_Load(object sender, EventArgs e)
		{
			Dictionary<string, string> data = keys.ToDictionary(x => x, x => Request[x]);

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
					//string signUpQuery = string.Format("INSERT Account (UserId, Nickname, MailAddress, PasswordHash) VALUES ('{0}', N'{1}', '{2}', HASHBYTES('SHA1', '{3}'))",
					//											data[Helper.UserId], data[Helper.Nickname], data[Helper.MailAddress], data[Helper.Password]);
					string signUpQuery = @"INSERT Account(UserId, Nickname, MailAddress, PasswordHash) VALUES (@UserId, @Nickname, @MailAddress, HASHBYTES('SHA1', @Password))";
					Helper.ExecuteSqlQuery(signUpQuery,
						setAction: (param) =>
						{
							param.Add("@UserId", SqlDbType.VarChar).Value = data[Helper.UserId];
							param.Add("@Nickname", SqlDbType.NVarChar).Value = data[Helper.Nickname];
							param.Add("@MailAddress", SqlDbType.VarChar).Value = data[Helper.MailAddress];
							param.Add("@Password", SqlDbType.VarChar).Value = data[Helper.Password];
						});
					Response.Write("Insert Account Finished.<br />\n");

					//string deviceBindQuery = string.Format("INSERT AccountDevice (UserId, DeviceIdHash, SessionId) VALUES ('{0}', '{1}', null);", data[Helper.UserId], data[Helper.DeviceIdHash]);
					string deviceBindQuery = @"INSERT AccountDevice(UserId, DeviceIdHash) VALUES(@UserId, CONVERT(varbinary, @DeviceIdHash, 2))";
					Helper.ExecuteSqlQuery(deviceBindQuery,
						setAction: (param) =>
						{
							param.Add("@UserId", SqlDbType.VarChar).Value = data[Helper.UserId];
							param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.DeviceIdHash];
						});
					Response.Write("Insert AccountData Finished.<br />\n");

					Response.Write("SignUp Successed.<br />");
				}
				catch (Exception ex)
				{
					Response.Write("#Error has occured#<br />\n");
					Response.Write(ex.Message + "<br />\n");
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