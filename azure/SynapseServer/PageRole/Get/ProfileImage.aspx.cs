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
	public partial class ProfileImage : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string userIdHash = Request.QueryString.Get(Helper.UserIdHash);

			if (string.IsNullOrEmpty(userIdHash))
			{
				Response.StatusCode = 404;
				Response.End();
			}

			string profileImageQuery = "SELECT ImageType,ImageBinary FROM AccountProfile WHERE HASHBYTES('SHA1', UserId) = CONVERT(varbinary, @UserId, 2)";

			try
			{
				Helper.ExecuteSqlQuery(profileImageQuery,
					setAction: (param) =>
					{
						param.Add("@UserId", SqlDbType.VarChar, 40).Value = userIdHash;
					},
					getAction: (reader) =>
					{
						if (reader.Read())
						{
							string type = reader["ImageType"] as string;
							byte[] binary = reader["ImageBinary"] as byte[];
							if (type == null || binary == null)
							{
								Response.StatusCode = 404;
							}
							else
							{
								Response.ContentType = type;
								Response.BinaryWrite(binary);
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