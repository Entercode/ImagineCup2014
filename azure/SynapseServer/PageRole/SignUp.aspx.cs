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
	public partial class SignUp : ExtendedPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize(new[] { Helper.UserId, Helper.Nickname, Helper.MailAddress, Helper.Password, Helper.DeviceIdHash });

			ShowPostedDataResponse();

			RunProcess((WriteLine, data) =>
			{
				string signUpQuery = "INSERT Account(UserId, Nickname, MailAddress, PasswordHash) VALUES (@UserId, @Nickname, @MailAddress, HASHBYTES('SHA1', @Password))";
				
				Helper.ExecuteSqlQuery(signUpQuery,
					setAction: (param) =>
					{
						param.Add("@UserId", SqlDbType.VarChar).Value = data[Helper.UserId];
						param.Add("@Nickname", SqlDbType.NVarChar).Value = data[Helper.Nickname];
						param.Add("@MailAddress", SqlDbType.VarChar).Value = data[Helper.MailAddress];
						param.Add("@Password", SqlDbType.VarChar).Value = data[Helper.Password];
					});
				WriteLine("Insert Account Finished.");

				string deviceBindQuery = "INSERT AccountDevice(UserId, DeviceIdHash) VALUES(@UserId, CONVERT(varbinary, @DeviceIdHash, 2))";
				
				Helper.ExecuteSqlQuery(deviceBindQuery,
					setAction: (param) =>
					{
						param.Add("@UserId", SqlDbType.VarChar).Value = data[Helper.UserId];
						param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.DeviceIdHash];
					});
				WriteLine("Insert AccountData Finished.");

				WriteLine("SignUp Successed.");
			});
		}
	}
}