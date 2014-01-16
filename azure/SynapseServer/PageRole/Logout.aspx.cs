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
			Initialize(new[] { Helper.UserIdHash, Helper.DeviceIdHash, Helper.SessionId });

			ShowPostedDataResponse();

			RunProcess((WriteLine, data) =>
			{
				string logoutQuery
					= "UPDATE AccountDevice SET SessionId = NULL "
					+ "WHERE HASHBYTES('SHA1', UserId) = CONVERT(varbinary, @UserIdHash, 2) AND DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)";

				Helper.ExecuteSqlQuery(logoutQuery,
					setAction: (param) =>
					{
						param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = data[Helper.UserIdHash];
						param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.DeviceIdHash];
					});
				WriteLine("Logout Query Finished.");

				WriteLine("Logout successed.");
			},
			checkSession: true);
		}
	}
}