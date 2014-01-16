using System;
using System.Data;

using SynapseServer;

namespace PageRole
{
	public partial class StreetPass : ExtendedPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize(new[] { Helper.UserIdHash, Helper.DeviceIdHash, Helper.PassedDeviceIdHash, Helper.PassedTime, Helper.SessionId });

			ShowPostedDataResponse();

			RunProcess((WriteLine, data) =>
			{
				string passedTime = Helper.StringConvertOfNumberToDateTime(data[Helper.PassedTime]);
				string streetPassQuery
					= "INSERT StreetPass(UserBindId, PassedDeviceIdHash, PassedTime) "
					+ "VALUES ("
					+ "(SELECT BindId FROM AccountDevice WHERE HASHBYTES('SHA1', UserId) = CONVERT(varbinary, @UserIdHash, 2) AND DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)), "
					+ "CONVERT(varbinary, @PassedDeviceIdHash, 2), "
					+ "@PassedTime"
					+ ")";

				Helper.ExecuteSqlQuery(streetPassQuery,
					setAction: (param) =>
					{
						param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = data[Helper.UserIdHash];
						param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.DeviceIdHash];
						param.Add("@PassedDeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.PassedDeviceIdHash];
						param.Add("@PassedTime", SqlDbType.VarChar).Value = passedTime;
					});
				WriteLine("StreetPass Query Finished.");

				WriteLine("Saved new street pass data.");
			},
			checkSession: true);
		}
	}
}