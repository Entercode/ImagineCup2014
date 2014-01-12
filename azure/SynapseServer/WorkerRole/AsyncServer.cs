using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;

using SynapseServer;

namespace WorkerRole
{
	/// <summary>すれ違い用のHTTPサーバー</summary>
	sealed class AsyncServer
	{
		string hostName;
		int portNumber;

		HttpListener listener;

		int maxConnections;

		/// <summary>
		/// インスタンスを生成
		/// </summary>
		/// <param name="hostName">Listenしたいホスト名</param>
		/// <param name="portNumber">Listenしたいポート番号</param>
		/// <param name="maxConnections">最大接続数</param>
		public AsyncServer(string hostName, int portNumber, int maxConnections)
		{
			this.hostName = hostName;
			this.portNumber = portNumber;
			this.maxConnections = maxConnections;
		}

		/// <summary>
		/// HTTPサーバーの処理タスク
		/// </summary>
		/// <returns>一つの処理タスク</returns>
		private async Task Work()
		{
			await Task.Run(() =>
			{
				Infomation("Start Task" + Task.CurrentId);
				while (true)
				{
					HttpListenerContext context = listener.GetContext();
					HttpListenerRequest request = context.Request;
					HttpListenerResponse response = context.Response;

					Infomation("connected by {0}:{1} @{2}", request.RemoteEndPoint.Address.ToString(), request.RemoteEndPoint.Port.ToString(), Task.CurrentId);

					string resMessage = request.RawUrl;
					Infomation(resMessage);

					string sendMessage = string.Empty;

					if ("POST".Equals(request.HttpMethod))
					{
						using (StreamReader sr = new StreamReader(request.InputStream, true))
						{
							string postedString = sr.ReadToEnd();
							Dictionary<string, string> postedData = postedString.Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0],x => Uri.UnescapeDataString(x[1]));
							StringBuilder sb = new StringBuilder("I recieved...<br />\n");
							foreach (var pair in postedData)
							{
								sb.AppendLine(pair.Key + " : " + pair.Value + "<br />");
							}
							sendMessage = sb.ToString();

							string[] keys = new string[] { Helper.UserIdHash, Helper.DeviceIdHash, Helper.PassedDeviceIdHash, Helper.PassedTime, Helper.SessionId };//すれ違いのときのkey

							Dictionary<string, string> data = keys.ToDictionary(x => x, x => (postedData.ContainsKey(x) ? postedData[x] : string.Empty));

							if (!data.Any(x => string.IsNullOrEmpty(x.Value)))
							{
								try
								{
									//string checkSessionQuery = string.Format("IF (SELECT COUNT(*) FROM AccountTable A WHERE CONVERT(NVARCHAR(40), HashBytes('SHA1', A.UserId), 2) = '{0}' AND A.DeviceIdHash = '{1}' AND A.SessionId = '{2}') = 1 SELECT 1 AS 'Result' ELSE SELECT 0 AS 'Result'", data[Helper.UserIdHash], data[Helper.DeviceIdHash], data[Helper.SessionId]);
									string checkSessionQuery = @"IF (SELECT D.SessionId FROM AccountDevice D WHERE HASHBYTES('SHA1', D.UserId) = CONVERT(varbinary, @UserIdHash, 2) AND D.DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)) = CONVERT(varbinary, @SessionId, 2) SELECT 1 AS Result ELSE SELECT 0 AS Result";
									bool isLogin = false;
									Helper.ExecuteSqlQuery(checkSessionQuery,
										setAction: (param) =>
										{
											param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = data[Helper.UserIdHash];
											param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.DeviceIdHash];
											param.Add("@SessionId", SqlDbType.VarChar, 40).Value = data[Helper.SessionId];
										},
										getAction: (reader) =>
										{
											if (reader.Read())
											{
												isLogin = int.Parse(reader["Result"].ToString()) == 1;
											}
										});
									sendMessage += "Check Session Finished.<br />\n";

									if (isLogin)
									{
										string passedTime = Helper.StringConvertOfNumberToDateTime(data[Helper.PassedTime]);
										//string streetPassQuery = string.Format("INSERT StreetPass(UserBindId, PassedDeviceIdHash, PassedTime) VALUES ((SELECT BindId FROM AccountDevice WHERE UserId = '{0}' AND DeviceIdHash = '{1}'), '{2}' ,'{3}')",
										//		data[Helper.UserId], data[Helper.DeviceId], data[Helper.PassedDeviceId], passedTime);
										string streetPassQuery = @"INSERT StreetPass(UserBindId, PassedDeviceIdHash, PassedTime) VALUES ((SELECT BindId FROM AccountDevice WHERE HASHBYTES('SHA1', UserId) = CONVERT(varbinary, @UserIdHash, 2) AND DeviceIdHash = CONVERT(varbinary, @DeviceIdHash, 2)), CONVERT(varbinary, @PassedDeviceIdHash, 2) ,@PassedTime)";
										Helper.ExecuteSqlQuery(streetPassQuery,
											setAction: (param) =>
											{
												param.Add("@UserIdHash", SqlDbType.VarChar, 40).Value = data[Helper.UserIdHash];
												param.Add("@DeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.DeviceIdHash];
												param.Add("@PassedDeviceIdHash", SqlDbType.VarChar, 40).Value = data[Helper.PassedDeviceIdHash];
												param.Add("@PassedTime", SqlDbType.VarChar).Value = passedTime;
											});
										sendMessage += "StreetPass Query Finished.<br />\n";

										sendMessage += "Saved new street pass data.<br />\n";
									}
									else
									{
										sendMessage += "SessionId is different.<br />\n";
									}
								}
								catch (Exception ex)
								{
									sendMessage += "#Error has occured<br />\n";
									sendMessage += ex.Message + "<br />\n";
								}
							}
							else
							{
								sendMessage += "But necessary data lacks.<br />\n";
								sendMessage += Helper.WriteNeedKeys(keys);
							}
						}
					}
					else
					{
						response.Abort();//レスポンスをやめる
						continue;
					}

					response.ContentType = "text/html; charset=utf-8";

					byte[] sendBytes = Encoding.UTF8.GetBytes(sendMessage);

					response.OutputStream.Write(sendBytes, 0, sendBytes.Length);

					Infomation(sendMessage);

					response.Close();

					Infomation("Connection Finished @" + Task.CurrentId);
				}
			}).ConfigureAwait(false);
		}

		/// <summary>サーバーを起動する</summary>
		public void Run()
		{
			listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://{0}:{1}/", hostName, portNumber));
			listener.Start();

			var tasks = Enumerable.Range(0, maxConnections).Select(_ => Work());

			//最大接続数の分だけ待機用のタスクを生成し、どれかが終了するまで待機(実質は無限ループ)
			Task.WaitAll(tasks.ToArray());
			listener.Stop();
			listener.Close();
		}

		[Conditional("DEBUG")]//DEBUGモード時のみ呼び出し
		private void Infomation(string format, params object[] args)
		{
			Trace.TraceInformation(string.Format(format, args), "Information");
		}
	}
}
