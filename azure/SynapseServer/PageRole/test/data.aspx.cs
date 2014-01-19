using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using SynapseServer;

namespace PageRole.test
{
	public partial class data : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void DataDeleting(object sender, ObjectDataSourceMethodEventArgs e)
		{
			var param = e.InputParameters;
			param.Add("MessageAction",
				(Action<string>)((string x) =>
				{
					this.message = x;
				}));
		}

		protected string Message
		{
			get
			{
				return message;
			}
		}

		private string message = "無し";
	}

	public class GridViewObject
	{
		public static List<Account> GetAccounts()
		{
			var result = new List<Account>();

			string query = "SELECT * FROM Account";
			Helper.ExecuteSqlQuery(query,
				getAction: (reader) =>
				{
					while (reader.Read())
					{
						Account a = new Account();
						a.UserId = reader["UserId"] as string;
						a.Nickname = reader["Nickname"] as string;
						a.MailAddress = reader["MailAddress"] as string;
						a.PasswordHash = Helper.BytesToString(reader["PasswordHash"] as byte[]);
						a.UserIdHash = Helper.BytesToString(Helper.StringHashing(a.UserId));
						result.Add(a);
					}
				});

			return result;
		}

		public static void DeleteAccount(string UserId, Action<string> MessageAction)
		{
			string query = "DELETE Account WHERE UserId = @UserId";

			try
			{
				Helper.ExecuteSqlQuery(query,
					setAction: (param) =>
					{
						param.Add("@UserId", SqlDbType.VarChar).Value = UserId;
					});
				MessageAction("Deleted Account Successfully.");
			}
			catch (Exception ex)
			{
				MessageAction(ex.Message);
			}
		}


		public static List<AccountDevice> GetAccountDevices()
		{
			var result = new List<AccountDevice>();

			string query = "SELECT * FROM AccountDevice";

			Helper.ExecuteSqlQuery(query,
				getAction: (reader) =>
				{
					while (reader.Read())
					{
						AccountDevice a = new AccountDevice();
						a.UserId = reader["UserId"] as string;
						a.DeviceIdHash = Helper.BytesToString(reader["DeviceIdHash"] as byte[]);
						a.BindId = Convert.ToInt32(reader["BindId"]);
						if (DBNull.Value.Equals(reader["SessionId"]))
						{
							a.SessionId = "#None#";
						}
						else
						{
							a.SessionId = Helper.BytesToString(reader["SessionId"] as byte[]);
						}
						result.Add(a);
					}
				});

			return result;
		}

		public static void DeleteAccountDevice(int BindId, Action<string> MessageAction)
		{
			string query = "DELETE AccountDevice WHERE BindId = @BindId";

			try
			{
				Helper.ExecuteSqlQuery(query,
					setAction: (param) =>
					{
						param.Add("@BindId", SqlDbType.Int).Value = BindId;
					});
				MessageAction("Deleted Device Successfully");

			}
			catch (Exception ex)
			{
				MessageAction(ex.Message);
			}
		}


		public static List<StreetPassInformation> GetStreetPassInformations()
		{
			var result = new List<StreetPassInformation>();

			string query = "SELECT * FROM StreetPass";

			Helper.ExecuteSqlQuery(query,
				getAction: (reader) =>
				{
					while (reader.Read())
					{
						StreetPassInformation s = new StreetPassInformation();
						s.UserBindId = Convert.ToInt32(reader["UserBindId"]);
						s.PassedBindId = Convert.ToInt32(reader["PassedBindId"]);
						s.PassedTime = DateTime.Parse(reader["PassedTime"].ToString());
						result.Add(s);
					}
				});

			return result;
		}

		public static void DeleteStreetPassInformation(int UserBindId, DateTime PassedTime, Action<string> MessageAction)
		{
			string query = "DELETE StreetPass WHERE UserBindId = @UserBindId AND PassedTime = @PassedTime";

			try
			{
				Helper.ExecuteSqlQuery(query,
					setAction: (param) =>
					{
						param.Add("@UserBindId", SqlDbType.Int).Value = UserBindId;
						param.Add("@PassedTime", SqlDbType.DateTime2).Value = PassedTime;
					});
				MessageAction("Deleted Pass-Information Successfully");
			}
			catch (Exception ex)
			{
				MessageAction(ex.Message);
			}
		}


		public static List<AccountProfile> GetAccountProfiles()
		{
			var result = new List<AccountProfile>();

			string query = "SELECT * FROM AccountProfile";

			Helper.ExecuteSqlQuery(query,
				getAction: (reader) =>
				{
					while (reader.Read())
					{
						AccountProfile a = new AccountProfile();
						a.UserId = reader["UserId"] as string;
						a.Profile = reader["Profile"] as string;
						a.UserIdHash = Helper.BytesToString(Helper.StringHashing(a.UserId));
						result.Add(a);
					}
				});

			return result;
		}

		public static void DeleteAccountProfile(string UserId,Action<string> MessageAction)
		{
			string query = "DELETE AccountProfile WHERE UserId = @UserId";

			try
			{
				Helper.ExecuteSqlQuery(query,
					setAction: (param) =>
					{
						param.Add("@UserId", SqlDbType.VarChar).Value = UserId;
					});
				MessageAction("Deleted Profile Successfully");
			}
			catch (Exception ex)
			{
				MessageAction(ex.Message);
			}
		}


		public static List<TweetInformation> GetTweetInformations()
		{
			var result = new List<TweetInformation>();

			string query = "SELECT * FROM Tweet ORDER BY ServerRecievedTime";

			Helper.ExecuteSqlQuery(query,
				getAction: (reader) =>
				{
					while (reader.Read())
					{
						TweetInformation t = new TweetInformation();
						t.BindId = Convert.ToInt32(reader["BindId"]);
						t.ClientTweetTime = DateTime.Parse(reader["ClientTweetTime"].ToString());
						t.ServerRecievedTime = DateTime.Parse(reader["ServerRecievedTime"].ToString());
						t.Tweet = reader["Tweet"] as string;
						result.Add(t);
					}
				});

			return result;
		}

		public static void DeleteTweetInformation(int BindId, DateTime ServerRecievedTime,Action<string> MessageAction)
		{
			string query = "DELETE Tweet WHERE BindId = @BindId AND ServerRecievedTime = @ServerRecievedTime";

			try
			{
				Helper.ExecuteSqlQuery(query,
					setAction: (param) =>
					{
						param.Add("@BindId", SqlDbType.Int).Value = BindId;
						param.Add("@ServerRecievedTime", SqlDbType.DateTime2).Value = ServerRecievedTime;
					});
				MessageAction("Deleted Tweet Successfully");
			}
			catch (Exception ex)
			{
				MessageAction(ex.Message);
			}
		}
	}

	public class Account
	{
		public string UserId { get; set; }
		public string Nickname { get; set; }
		public string MailAddress { get; set; }
		public string PasswordHash { get; set; }
		public string UserIdHash { get; set; }
	}

	public class AccountDevice
	{
		public string UserId { get; set; }
		public string DeviceIdHash { get; set; }
		public int BindId { get; set; }
		public string SessionId { get; set; }
	}

	public class StreetPassInformation
	{
		public int UserBindId { get; set; }
		public int PassedBindId { get; set; }
		public DateTime PassedTime { get; set; }
	}

	public class AccountProfile
	{
		public string UserId { get; set; }
		public string Profile { get; set; }
		public string UserIdHash { get; set; }
	}

	public class TweetInformation
	{
		public int BindId { get; set; }
		public DateTime ClientTweetTime { get; set; }
		public DateTime ServerRecievedTime { get; set; }
		public string Tweet { get; set; }
	}
}