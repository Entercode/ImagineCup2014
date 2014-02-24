using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using SynapseServer;

namespace PageRole.Get
{
	public partial class Profile : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string userIdHash = Request.QueryString.Get(Helper.UserIdHash);

			if (string.IsNullOrEmpty(userIdHash))
			{
				Response.StatusCode = 404;
				Response.End();
				return;
			}

			try
			{
				string profileQuery = "SELECT Profile FROM AccountProfile WHERE HASHBYTES('SHA1', UserId) = CONVERT(varbinary, @UserIdHash, 2)";

				Helper.ExecuteSqlQuery(profileQuery,
					setAction: (param) =>
					{
						param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = userIdHash;
					},
					getAction: (reader) =>
					{
						if (reader.Read())
						{
							string profile = reader["Profile"] as string;
							if (string.IsNullOrEmpty(profile))
							{
								Response.StatusCode = 404;
							}
							else
							{
								Response.Write(profile);
							}
						}
						else
						{
							Response.StatusCode = 404;
						}
					});
			}
			catch (Exception ex)
			{
				Response.StatusCode = 404;
				Response.Write(ex.Message);
			}

			Response.End();
		}
	}
}