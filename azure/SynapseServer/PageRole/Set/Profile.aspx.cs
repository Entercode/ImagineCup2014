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
	public partial class EditProfile : ExtendedPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize(new[] { Helper.Profile }, new[] { Helper.ProfileImage });

			ShowPostedDataResponse();

			RunProcess((WriteLine, data, file, bindId) =>
			{
				string editProfileQuery
					= "UPDATE AccountProfile SET Profile = @Profile, ImageType = @ImageType ,ImageBinary = @ImageBinary "
					+ "WHERE UserId = (SELECT D.UserId FROM AccountDevice D WHERE D.BindId = @BindId)";

				Helper.ExecuteSqlQuery(editProfileQuery,
					setAction: (param) =>
					{
						param.Add("@Profile", SqlDbType.NVarChar).Value = data[Helper.Profile];
						if (file[Helper.ProfileImage] != null)
						{
							param.Add("@ImageType", SqlDbType.VarChar).Value = file[Helper.ProfileImage].ContentType;
							param.Add("@ImageBinary", SqlDbType.Image).Value = file[Helper.ProfileImage].Binary;
						}
						else
						{
							param.Add("@ImageType", SqlDbType.VarChar).Value = DBNull.Value;
							param.Add("@ImageBinary", SqlDbType.Image).Value = DBNull.Value;
						}
						param.Add("@BindId", SqlDbType.Int).Value = bindId;
					});
				WriteLine("Edit profile Query Finished.");

				WriteLine("Edited profile.");
			},
			checkSession: true);
		}
	}
}