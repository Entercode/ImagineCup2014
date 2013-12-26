using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PageRole
{
	public partial class Login : System.Web.UI.Page
	{
		readonly string[] keys = new string[] { "did", "ph" };

		protected void Page_Load(object sender, EventArgs e)
		{
			Dictionary<string, string> data = keys.ToDictionary(x => x, x => Request.Form.Get(x));

			Response.Write("I recieved...<br />\n");
			foreach (var pair in data)
			{
				Response.Write(pair.Key + ":" + pair.Value + "<br />\n");
			}
		}
	}
}