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
	public partial class Logout : ExtendedPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize(null);

			ShowPostedDataResponse();

			RunProcess((WriteLine, data, file, bindId) =>
			{
				string logoutQuery = "UPDATE AccountDevice SET SessionId = NULL WHERE BindId = @BindId";

				Helper.ExecuteSqlQuery(logoutQuery,
					setAction: (param) =>
					{
						param.Add("@BindId", SqlDbType.Int).Value = bindId;
					});
				WriteLine("Logout Query Finished.");

				Response.Cookies.Remove(Helper.SessionId);
				WriteLine("Logout successed.");
			},
			checkSession: true);
		}
	}
}