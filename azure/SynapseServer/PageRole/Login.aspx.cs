using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SynapseServer;

namespace PageRole
{
	public partial class Login : System.Web.UI.Page
	{
		readonly string[] keys = new string[] { Helper.DeviceId, Helper.PasswordHash };

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
					string checkPasswordQuery = string.Format("IF (SELECT A.PasswordHash FROM AccountTable A WHERE A.UserId = (SELECT D.UserId FROM AccountDevice D WHERE D.DeviceId = {0})) = {1} SELECT 1 AS Result ELSE SELECT 0 AS Result", data[Helper.DeviceId], data[Helper.PasswordHash].GetHashCode());
					bool isAuthed = false;
					
					Helper.ExecuteSqlQuery(checkPasswordQuery,
						getAction: (reader) =>
						{
							if (reader.Read())
							{
								isAuthed = int.Parse(reader["result"].ToString()) == 1;
							}
						});

					if (isAuthed)
					{
						//仮の認証ハッシュ
						int authHash=DateTime.Now.GetHashCode();
						string authHashQuery = string.Format("UPDATE AccountTable SET AuthHash = {0} WHERE UserId = (SELECT D.UserId FROM AccountDevice D WHERE D.DeviceId = {1})", authHash, data[Helper.DeviceId]);
						Helper.ExecuteSqlQuery(authHashQuery);
						Response.Write("And Login success.<br />\n");
						Response.Write("your AuthHash:#" + authHash + "#");
					}
					else
					{
						Response.Write("And Login failed.");
					}
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