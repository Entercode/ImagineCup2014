using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Data;

namespace SynapseServer
{
	/// <summary>
	/// SynapseServer用の拡張Pageクラス
	/// </summary>
	public abstract class ExtendedPage : Page
	{
		/// <summary>
		/// 必要なname
		/// </summary>
		private string[] keys;

		/// <summary>
		/// 正常に取得できたデータ
		/// </summary>
		private Dictionary<string, string> postedData;

		/// <summary>
		/// POSTされたデータを整形して保持する
		/// </summary>
		/// <param name="keys">必要なname</param>
		public void Initialize(string[] keys)
		{
			this.keys = keys;
			this.postedData = GetPostedDataKeyValuePair();
		}

		/// <summary>
		/// POSTされたデータから必要なものを抜き出して保持する
		/// </summary>
		/// <returns>保持すべきデータ</returns>
		private Dictionary<string, string> GetPostedDataKeyValuePair()
		{
			return keys.ToDictionary(x => x, x => Request.Form.Get(x));
		}

		/// <summary>
		/// 整形したデータをクライアントに表示する
		/// </summary>
		public void ShowPostedDataResponse()
		{
			Response.Write("I recieved..." + Helper.NewLine);
			foreach (var pair in postedData)
			{
				Response.Write(pair.Key + ":" + pair.Value + Helper.NewLine);
			}
		}

		/// <summary>
		/// 必要なデータがすべてそろっているかを調べる
		/// </summary>
		/// <returns>必要なデータがすべてそろっているか</returns>
		private bool IsPostedDataValid()
		{
			return !postedData.Any(x => string.IsNullOrEmpty(x.Value));
		}

		/// <summary>
		/// メインとなる処理を実行する
		/// </summary>
		/// <param name="process">実行するアクション</param>
		public void RunProcess(Action<Action<string>, Dictionary<string, string>> process, bool checkSession = false)
		{
			if (IsPostedDataValid())
			{
				if (!checkSession || CheckSessionQuery())
				{
					try
					{
						process((str) =>
						{
							this.Response.Write(str + Helper.NewLine);
						}, this.postedData);
					}
					catch (KeyNotFoundException ex)
					{
						throw ex;
					}
					catch (AggregateException ex)
					{
						Response.Write("#Error has occured#" + Helper.NewLine);
						foreach(var error in ex.InnerExceptions){
							Response.Write(error.Message + Helper.NewLine);
						}
					}
					catch (Exception ex)
					{
						Response.Write("#Error has occured#" + Helper.NewLine);
						Response.Write(ex.Message + Helper.NewLine);
					}
				}
				else
				{
					Response.Write("SessionId is different." + Helper.NewLine);
				}
			}
			else
			{
				Response.Write("Necessary data lacks" + Helper.NewLine);
				Response.Write(WriteNeedKeys() + Helper.NewLine);
			}
		}

		/// <summary>
		/// クライアントへ渡す、必要なデータの一覧の文字列
		/// </summary>
		/// <returns>必要なデータ一覧の文字列</returns>
		private string WriteNeedKeys()
		{
			var sb = new System.Text.StringBuilder("<ul>\n");
			foreach (var key in keys)
			{
				sb.AppendLine("<li>" + key + "</li>\n");
			}
			sb.AppendLine("</ul>");
			return sb.ToString();
		}

		/// <summary>
		/// セッションを確認する
		/// </summary>
		/// <returns>セッションが妥当かどうか</returns>
		private bool CheckSessionQuery()
		{
			try
			{
				string checkSessionQuery
					= "IF "
					+ "(SELECT D.SessionId FROM AccountDevice D WHERE HASHBYTES('SHA1', D.UserId) = CONVERT(varbinary, @UserIdHash, 2) AND D.DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)) = CONVERT(varbinary, @SessionId, 2)"
					+ "SELECT 1 AS Result "
					+ "ELSE "
					+ "SELECT 0 AS Result";
				bool isLogin = false;

				Helper.ExecuteSqlQuery(checkSessionQuery,
					setAction: (param) =>
					{
						param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = postedData[Helper.UserIdHash];
						param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = postedData[Helper.DeviceIdHash];
						param.Add("@SessionId", SqlDbType.VarChar, 40).Value = postedData[Helper.SessionId];
					},
					getAction: (reader) =>
					{
						if (reader.Read())
						{
							isLogin = int.Parse(reader["Result"].ToString()) == 1;
						}
					});
				Response.Write("Check Session Finished." + Helper.NewLine);

				return isLogin;
			}
			catch (KeyNotFoundException ex)
			{
				throw ex;
			}
			catch (Exception ex)
			{
				Response.Write("#Error has occured#" + Helper.NewLine);
				Response.Write(ex.Message + Helper.NewLine);
				return false;
			}
		}
	}
}
