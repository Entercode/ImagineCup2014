using System;
using System.Data;

using SynapseServer;

namespace PageRole
{
	public partial class StreetPass : ExtendedPage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Initialize(new[] { Helper.PassedDeviceIdHash, Helper.PassedTime });

			ShowPostedDataResponse();

			RunProcess((WriteLine, data, file, bindId) =>
			{
				string passedTime = Helper.StringConvertOfNumberToDateTime(data[Helper.PassedTime]);
				string streetPassQuery
					= "INSERT StreetPass(UserBindId, PassedBindId, PassedTime) "
					+ "VALUES (@BindId, (SELECT BindId FROM AccountDevice WHERE DeviceIdHash = CONVERT(varbinary, @PassedDeviceIdHash, 2)), @PassedTime)";

				Helper.ExecuteSqlQuery(streetPassQuery,
					setAction: (param) =>
					{
						param.Add("@BindId", SqlDbType.Int).Value = bindId;
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