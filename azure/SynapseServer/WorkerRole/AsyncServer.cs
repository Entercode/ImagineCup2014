using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
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

							string[] keys = new string[] { Helper.UserId, Helper.DeviceId, Helper.PassedDeviceId, Helper.PassedTime, Helper.Hash };//すれ違いのときのkey

							Dictionary<string, string> data = keys.ToDictionary(x => x, x => (postedData.ContainsKey(x) ? postedData[x] : string.Empty));

							bool isAuthed = false;
							if (data.Any(x => Helper.Hash.Equals(x.Key)))
							{
								try
								{
									string checkAuthQuery = string.Format("IF (SELECT COUNT(A.AuthHash) FROM AccountTable A WHERE A.AuthHash = {0}) = 1 SELECT 1 AS 'Result' ELSE SELECT 0 AS 'Result'", data[Helper.Hash]);
									Helper.ExecuteSqlQuery(checkAuthQuery,
										getAction: (reader) =>
										{
											if (reader.Read())
											{
												isAuthed = int.Parse(reader["Result"].ToString()) == 1;
											}
										});
								}
								catch (Exception ex)
								{
									sendMessage += ex.Message;
								}
							}

							if (isAuthed)
							{
								//POSTされたkeysに対応する値がすべて正しく受信できている場合
								if (!data.Any(x => string.IsNullOrEmpty(x.Value)))
								{
									try
									{
										string passedTime = Helper.StringConvertOfNumberToDateTime(data[Helper.PassedTime]);
										string query = string.Format("INSERT StreetPass(UserBindId, PassedDeviceId, PassedTime) VALUES ((SELECT BindId FROM AccountDevice WHERE UserId = '{0}' AND DeviceId = {1}), {2},'{3}')",
												data[Helper.UserId], data[Helper.DeviceId], data[Helper.PassedDeviceId], passedTime);
										Helper.ExecuteSqlQuery(query);
										sendMessage += "And saved new account data.";
									}
									catch (Exception ex)
									{
										sendMessage += ex.Message;
									}
								}
								else
								{
									sendMessage += "But necessary data lacks.<br />\n";
									sendMessage += Helper.WriteNeedKeys(keys);
								}
							}
							else
							{
								sendMessage += "But AuthHash is different.";
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
