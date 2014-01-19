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
		private string[] valueKeys;

		/// <summary>
		/// 必要なname(ファイル)
		/// </summary>
		private string[] fileKeys;

		/// <summary>
		/// 取得したBindId
		/// </summary>
		private int? bindId;

		/// <summary>
		/// 正常に取得できた値
		/// </summary>
		protected Dictionary<string, string> postedValue;

		/// <summary>
		/// 正常に取得できたファイル
		/// </summary>
		protected Dictionary<string,Content> postedFile;

		/// <summary>
		/// POSTされたデータを整形して保持する
		/// </summary>
		/// <param name="keys">必要なname</param>
		public void Initialize(string[] valueKeys, string[] fileKeys = null)
		{
			this.valueKeys = valueKeys ?? new string[0];
			this.fileKeys = fileKeys ?? new string[0];

			this.postedValue = GetPostedValueKeyValuePair();
			this.postedFile = GetPostedFileKeyValuePair();
		}

		/// <summary>
		/// POSTされた値から必要なものを抜き出して保持する
		/// </summary>
		/// <returns>保持すべき値</returns>
		private Dictionary<string, string> GetPostedValueKeyValuePair()
		{
			var a = new Dictionary<string, string>();
			if (valueKeys.Any())
			{
				a = valueKeys.ToDictionary(x => x, x => Request.Form.Get(x));
			}
			if (Request.Cookies[Helper.SessionId] != null)
			{
				a.Add(Helper.SessionId, Request.Cookies[Helper.SessionId].Value);
			}
			return a;
		}

		/// <summary>
		/// POSTされたファイルから必要なものを抜き出して保管する
		/// </summary>
		/// <returns>保持すべきファイル</returns>
		private Dictionary<string, Content> GetPostedFileKeyValuePair()
		{
			if (fileKeys.Any())
			{
				var files = fileKeys.ToDictionary(x => x, x => Request.Files[x]);
				Dictionary<string, Content> filesBytes = new Dictionary<string, Content>(files.Count);
				foreach (var file in files.AsParallel())
				{
					Content c = new Content();
					if (file.Value == null)
					{
						filesBytes.Add(file.Key, null);
						continue;
					}
					c.ContentType = file.Value.ContentType;
					byte[] b = new byte[file.Value.ContentLength];
					file.Value.InputStream.Read(b, 0, file.Value.ContentLength);
					c.Binary = b;
					filesBytes.Add(file.Key, c);
				}
				return filesBytes;
			}
			else
			{
				return new Dictionary<string, Content>();
			}
		}

		/// <summary>
		/// 整形した値をクライアントに表示する
		/// </summary>
		public void ShowPostedDataResponse()
		{
			Response.Write("I recieved..." + Helper.NewLine);
			foreach (var pair in postedValue)
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
			return !postedValue.Any(x => string.IsNullOrEmpty(x.Value));
		}

		/// <summary>
		/// メインとなる処理を実行する
		/// </summary>
		/// <param name="process">実行するアクション</param>
		public void RunProcess(Action<Action<string>, Dictionary<string, string>, Dictionary<string, Content>, int?> process, bool checkSession = false)
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
						}, this.postedValue, this.postedFile, bindId);
					}
					catch (KeyNotFoundException ex)
					{
						throw ex;
					}
					catch (AggregateException ex)
					{
						Response.Write("#Error has occured#" + Helper.NewLine);
						foreach (var error in ex.InnerExceptions)
						{
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
			foreach (var key in valueKeys)
			{
				sb.AppendLine("<li>" + key + "</li>\n");
			}
			foreach (var key in fileKeys)
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
			if (!postedValue.ContainsKey(Helper.SessionId))
			{
				return false;
			}
			else
			{
				try
				{
					string checkSessionQuery = "SELECT D.BindId FROM AccountDevice D WHERE D.SessionId = CONVERT(varbinary, @SessionId, 2)";

					Helper.ExecuteSqlQuery(checkSessionQuery,
						setAction: (param) =>
						{
							param.Add("@SessionId", SqlDbType.VarChar, 40).Value = postedValue[Helper.SessionId];
						},
						getAction: (reader) =>
						{
							if (reader.Read())
							{
								bindId = Convert.ToInt32(reader["BindId"]);
							}
						});
					Response.Write("Check Session Finished." + Helper.NewLine);
				}
				catch (KeyNotFoundException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
					Response.Write("#Error has occured#" + Helper.NewLine);
					Response.Write(ex.Message + Helper.NewLine);
				}

				return bindId != null;
			}
		}

		public sealed class Content
		{
			public string ContentType { get; set; }
			public byte[] Binary { get; set; }
		}
	}
}
