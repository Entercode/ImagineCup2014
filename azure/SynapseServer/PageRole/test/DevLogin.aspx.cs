using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace PageRole.test
{
	public partial class DevLogin : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Submit_Click(object sender, EventArgs e)
		{
			if ("1procon1".Equals(Pass.Text))
			{
				FormsAuthentication.RedirectFromLoginPage("admin", true);
			}
			else
			{
				Message.Text = "パスワードが違います。";
			}
		}
	}
}