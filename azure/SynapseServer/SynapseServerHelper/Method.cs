using System;
using System.Web;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace SynapseServer
{
	public static partial class Helper
	{
		static string connectionString;

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

		public static string StringConvertOfNumberToDateTime(string number)
		{
			if (number.Length != 14)
			{
				throw new ArgumentException("数列の桁数が異常です。" + "#" + number);
			}

			if (Regex.IsMatch(number, @"\D"))
			{
				throw new ArgumentException("数字以外の文字が混入しています。");
			}

			StringBuilder sb = new StringBuilder();
			sb.Append(number.Substring(0, 4));
			sb.Append("/");
			sb.Append(number.Substring(4, 2));
			sb.Append("/");
			sb.Append(number.Substring(6, 2));
			sb.Append(" ");
			sb.Append(number.Substring(8, 2));
			sb.Append(":");
			sb.Append(number.Substring(10, 2));
			sb.Append(":");
			sb.Append(number.Substring(12, 2));
			return sb.ToString();
		}

		public static string StringConvertOfDateTimeToNumber(string dateTime)
		{
			var time = DateTime.Parse(dateTime);
			return time.ToString("yyyyMMddHHmmss");
		}

		public static byte[] StringToBytes(string str)
		{
			List<byte> bytes = new List<byte>(str.Length / 2);
			for (int i = 0; i < str.Length; i += 2)
			{
				bytes.Add(Convert.ToByte(str.Substring(i, 2), 16));
			}
			return bytes.ToArray();
		}

		public static string BytesToString(byte[] bytes)
		{
			return BitConverter.ToString(bytes).Replace("-", string.Empty);
		}

		public static byte[] StringHashing(string origin)
		{
			if (string.IsNullOrEmpty(origin))
			{
				return new byte[0];
			}
			byte[] bytes = Encoding.UTF8.GetBytes(origin);
			SHA1 crypto = new SHA1CryptoServiceProvider();
			return crypto.ComputeHash(bytes);
		}
	}
}