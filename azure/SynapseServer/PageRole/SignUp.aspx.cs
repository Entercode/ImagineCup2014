using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SynapseServer;

namespace PageRole
{
	public partial class SignUp : System.Web.UI.Page
	{
		readonly string[] keys = new string[] { Helper.DeviceId, Helper.PasswordHash, Helper.UserId, Helper.MailAddress, Helper.Nickname };

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
					string signUpQuery = string.Format("INSERT AccountTable (UserId, Nickname, MailAddress, PasswordHash, AuthHash) VALUES ('{0}',N'{1}','{2}',{3}, null)",
																data[Helper.UserId], data[Helper.Nickname], data[Helper.MailAddress], data[Helper.PasswordHash].GetHashCode());//GetHashCode()は仮である。
					Helper.ExecuteSqlQuery(signUpQuery);

					string deviceBindQuery = string.Format("INSERT AccountDevice (UserId, DeviceId) VALUES ('{0}',{1});", data[Helper.UserId], data[Helper.DeviceId]);
					Helper.ExecuteSqlQuery(deviceBindQuery);

					Response.Write("And saved new account data.");
				}
				catch (Exception ex)
				{
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