using System;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Data.SqlClient;

namespace SynapseServer
{
	public static class Helper//TODO ポストされたデータのインデックスをconstでここに定義
	{
		static string connectionString;

		public static string SQLConnectionString
		{
			get
			{
				return connectionString;
			}
		}

		static Helper()
		{
			connectionString = RoleEnvironment.GetConfigurationSettingValue("DatabaseConnectionString");
		}

		public static void ExecuteSqlQuery(string sqlQuery, Action<SqlParameterCollection> setAction = null, Action<SqlDataReader> getAction = null, Action<SqlParameterCollection> returnAction = null)
		{
			if (string.IsNullOrEmpty(sqlQuery))
			{
				return;
			}

			using (SqlConnection con = new SqlConnection(connectionString))
			{
				var connectionTask = con.OpenAsync();
				using (SqlCommand command = new SqlCommand(sqlQuery, con))
				{
					if (setAction != null)
					{
						setAction(command.Parameters);
					}

					connectionTask.Wait();
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (getAction != null)
						{
							getAction(reader);
						}
					}

					if (returnAction != null)
					{
						returnAction(command.Parameters);
					}
				}
			}
		}

		public static string WriteNeedKeys(string[] keys)
		{
			var sb = new System.Text.StringBuilder("<ul>\n");
			foreach (var key in keys)
			{
				sb.AppendLine("<li>" + key + "</li>\n");
			}
			sb.AppendLine("</ul>");
			return sb.ToString();
		}

		public const string UserId = "uid";
		public const string DeviceId = "did";
		public const string PassedDeviceId = "pdid";
		public const string PassedTime = "pt";
		public const string PasswordHash = "ph";
		public const string MailAddress = "mail";
		public const string Nickname = "nn";
		public const string Hash = "hash";
	}
}