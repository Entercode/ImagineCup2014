using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SynapseServer;

namespace PageRole
{
	public partial class Logout : System.Web.UI.Page
	{
		readonly string[] keys = new string[] { Helper.UserId, Helper.Hash };

		protected void Page_Load(object sender, EventArgs e)
		{
			Dictionary<string, string> data = keys.ToDictionary(x => x, x => Request.Form.Get(x));

			Response.Write("I recieved...<br />\n");
			foreach (var pair in data)
			{
				Response.Write(pair.Key + ":" + pair.Value + "<br />\n");
			}

			string query;
			if (!string.IsNullOrEmpty(data[Helper.Hash]))
			{
				query = string.Format("UPDATE AccountTable SET AuthHash = NULL WHERE AuthHash = {0}", data[Helper.Hash]);
			}
			else if (!string.IsNullOrEmpty(data[Helper.UserId]))
			{
				query = string.Format("UPDATE AccountTable SET AuthHash = NULL WHERE UserId = '{0}'", data[Helper.UserId]);
			}
			else
			{
				Response.Write("But necessary data lacks.\n");
				Response.Write(Helper.WriteNeedKeys(keys));
				return;
			}

			try
			{
				bool isLogout = false;
				Helper.ExecuteSqlQuery(query,
					getAction: (reader) =>
					{
						isLogout = reader.RecordsAffected == 1;
					});
				Response.Write(isLogout ? "And Logout success." : "And Logout failed.");
			}
			catch (Exception ex)
			{
				Response.Write(ex.Message);
			}
		}
	}
}