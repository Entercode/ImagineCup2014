using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PageRole
{
	public partial class Register : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Helper.ShowAllPost(Response, Request, new string[] { "duid", "did", "ph", "uid", "mail", "nn" });

			Response.Write("test_test");
		}
	}
}