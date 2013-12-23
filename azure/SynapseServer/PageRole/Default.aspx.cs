using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PageRole
{
	public partial class _Default : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}
	}

	public static class Helper
	{
		public static void ShowAllPost(HttpResponse responce, HttpRequest request, IEnumerable<string> keys)
		{
			foreach (var key in keys)
			{
				responce.Write(key + ":" + request.Form.Get(key) + "<br />\n");
			}
		}
	}
}