using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SynapseServer;

namespace PageRole.Get
{
	public partial class IsLogin : ExtendedPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize(null);

			ShowPostedDataResponse();

			bool isLogin = false;

			RunProcess((WriteLine, data, file, bindId) =>
			{
				isLogin = bindId.HasValue;
			}, checkSession: true);

			Response.Clear();
			Response.Write(isLogin);
		}
	}
}