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

		/*
		protected void AccountTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			TableCell cell = AccountTable.Rows[e.RowIndex].Cells[1];//UserId
			string query = string.Format("DELETE AccountTable WHERE UserId = '{0}'", cell.Text);
			Helper.ExecuteSqlQuery(query);
			//TODO: class GridViewObjectに書く datasourceタグのdeletemethodで指定
			AccountTableSource.Delete();
			AccountTable.DataBind();
		}

		protected void AccountDevices_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			TableCell cell = AccountDevices.Rows[e.RowIndex].Cells[3];//BindId
			string query = string.Format("DELETE AccountDevice WHERE BindId = {0}", int.Parse(cell.Text));
			Helper.ExecuteSqlQuery(query);
			AccountDeviceSource.Delete();
			AccountDevices.DataBind();
		}

		protected void StreetPassInformation_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			TableCell bindIdCell = StreetPassInformation.Rows[e.RowIndex].Cells[1];//UserBindId
			TableCell passedTimecell = StreetPassInformation.Rows[e.RowIndex].Cells[3];//PassedTime
			string query = string.Format("DELETE StreetPass WHERE UserBindId = '{0}' AND PassedTime = '{1}'", bindIdCell.Text, passedTimecell.Text);
			Helper.ExecuteSqlQuery(query);
			StreetPassInformationSource.Delete();
			StreetPassInformation.DataBind();
		}
		*/
		
		/*
		protected void AccountTableSource_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
		{
			var param = e.InputParameters;
			param.Add("Message", message);
		}

		protected void AccountDeviceSource_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
		{
			var param = e.InputParameters;
			param.Add("Message", message);
		}

		protected void StreetPassInformationSource_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
		{
			var param = e.InputParameters;
			param.Add("MessageAction", (Action<string>)((string x) => { this.message = x; }));
		}
		*/

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

			string query = "SELECT * FROM AccountTable";
			Helper.ExecuteSqlQuery(query,
				getAction: (reader) =>
				{
					while (reader.Read())
					{
						Account a = new Account();
						a.UserId = reader["UserId"] as string;
						a.Nickname = reader["Nickname"] as string;
						a.MailAddress = reader["MailAddress"] as string;
						a.PasswordHash = Convert.ToInt64(reader["PasswordHash"]);
						if (DBNull.Value.Equals(reader["AuthHash"]))
						{
							a.AuthHash = null;
						}
						else
						{
							a.AuthHash = Convert.ToInt64(reader["AuthHash"]);
						}
						result.Add(a);
					}
				});

			return result;
		}

		/*
		public static void DeleteAccount(Account a)
		{
			string query = string.Format("DELETE AccountTable WHERE UserId = '{0}'", a.UserId);
			Helper.ExecuteSqlQuery(query);
		}
		*/

		/*
		public static void DeleteAccount(string UserId, string Message)
		{
			string query = string.Format("DELETE AccountTable WHERE UserId = '{0}'", UserId);

			try
			{
				Helper.ExecuteSqlQuery(query);
				Message = "Deleted Account Successfully.";
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}
		*/

		public static void DeleteAccount(string UserId, Action<string> MessageAction)
		{
			string query = string.Format("DELETE AccountTable WHERE UserId = '{0}'", UserId);

			try
			{
				Helper.ExecuteSqlQuery(query);
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
						a.DeviceId = Convert.ToInt64(reader["DeviceId"]);
						a.BindId = Convert.ToInt32(reader["BindId"]);
						result.Add(a);
					}
				});

			return result;
		}

		/*
		public static void DeleteAccountDevice(AccountDevice ad)
		{
			string query = string.Format("DELETE AccountDevice WHERE BindId = {0}", ad.BindId);
			Helper.ExecuteSqlQuery(query);
		}
		*/

		/*
		public static void DeleteAccountDevice(int BindId,string Message)
		{
			string query = string.Format("DELETE AccountDevice WHERE BindId = {0}", BindId);

			try
			{
				Helper.ExecuteSqlQuery(query);
				Message = "Deleted Device Successfully";
				
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}
		*/

		public static void DeleteAccountDevice(int BindId, Action<string> MessageAction)
		{
			string query = string.Format("DELETE AccountDevice WHERE BindId = {0}", BindId);

			try
			{
				Helper.ExecuteSqlQuery(query);
				MessageAction("Deleted Device Successfully");

			}
			catch (Exception ex)
			{
				MessageAction(ex.Message);
			}
		}

		public static List<StreetPassInformation> GetStreetPassInformation()
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
						s.PassedDeviceId = Convert.ToInt64(reader["PassedDeviceId"]);
						s.PassedTime = DateTime.Parse(reader["PassedTime"].ToString());
						result.Add(s);
					}
				});

			return result;
		}

		/*
		public static void DeleteStreetPassInformation(StreetPassInformation info)
		{
			string query = string.Format("DELETE StreetPass WHERE UserBindId = '{0}' AND PassedTime = '{1}'", info.UserBindId, info.PassedTime);
			Helper.ExecuteSqlQuery(query);
		}
		*/

		/*
		public static void DeleteStreetPassInformation(int UserBindId, DateTime PassedTime,string Message)
		{
			string query = string.Format("DELETE StreetPass WHERE UserBindId = '{0}' AND PassedTime = '{1}'", UserBindId, PassedTime);

			try
			{
				Helper.ExecuteSqlQuery(query);
				Message = "Deleted Pass-Information Successfully";
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}
		*/

		public static void DeleteStreetPassInformation(int UserBindId, DateTime PassedTime, Action<string> MessageAction)
		{
			string query = string.Format("DELETE StreetPass WHERE UserBindId = '{0}' AND PassedTime = '{1}'", UserBindId, PassedTime);

			try
			{
				Helper.ExecuteSqlQuery(query);
				MessageAction("Deleted Pass-Information Successfully");
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
		public long PasswordHash { get; set; }
		public long? AuthHash { get; set; }//null許容
	}

	public class AccountDevice
	{
		public string UserId { get; set; }
		public long DeviceId { get; set; }
		public int BindId { get; set; }
	}

	public class StreetPassInformation
	{
		public int UserBindId { get; set; }
		public long PassedDeviceId { get; set; }
		public DateTime PassedTime { get; set; }
	}
}