using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WorkerRole
{
	/// <summary>
	/// 最大接続数有り 多分安定
	/// </summary>
	sealed class AsyncServer
	{
		TcpListener listener;

		int maxConnections;

		public AsyncServer(int maxConnections)
		{
			this.maxConnections = maxConnections;
		}

		private async Task Work()
		{
			await Task.Run(() =>
			{
				//Console.WriteLine("Start Task" + Task.CurrentId);
				Infomation("Start Task" + Task.CurrentId);
				while (true)
				{
					using (TcpClient tc = listener.AcceptTcpClient())
					{
						//Console.WriteLine("connected by {0}:{1} @{2}", ((IPEndPoint)tc.Client.RemoteEndPoint).Address, ((IPEndPoint)tc.Client.RemoteEndPoint).Port, Task.CurrentId);
						Infomation("connected by {0}:{1} @{2}", ((IPEndPoint)tc.Client.RemoteEndPoint).Address, ((IPEndPoint)tc.Client.RemoteEndPoint).Port, Task.CurrentId);

						using (NetworkStream ns = tc.GetStream())
						{
							using (MemoryStream ms = new MemoryStream())
							{
								byte[] resBytes = new byte[256];
								bool isDisconnected = false;

								int resSize = 0;
								do
								{
									//データの一部を受信する
									resSize = ns.Read(resBytes, 0, resBytes.Length);
									if (resSize == 0)
									{
										isDisconnected = true;
										break;
									}
									//受信したデータを蓄積する
									ms.Write(resBytes, 0, resSize);
								} while (ns.DataAvailable);

								if (isDisconnected)
								{
									//Console.WriteLine("クライアントが切断しました。 @" + Task.CurrentId);
									Infomation("クライアントが切断しました。 @" + Task.CurrentId);
								}
								else
								{
									string resMessage = Encoding.UTF8.GetString(ms.ToArray());

									string sendMessage = "I recieved \"" + resMessage + "\" by Task" + Task.CurrentId;
									byte[] sendBytes = Encoding.UTF8.GetBytes(sendMessage);
									ns.Write(sendBytes, 0, sendBytes.Length);
									//Console.WriteLine(sendMessage);
									Infomation(sendMessage);
								}
							}
						}
					}
					//Console.WriteLine("Connection Finished @" + Task.CurrentId);
					Infomation("Connection Finished @" + Task.CurrentId);
				}
			}).ConfigureAwait(false);
		}

		public void Run()
		{
			listener = new TcpListener(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["StreetPass"].IPEndpoint);
			listener.Start();

			var serverTasks = Enumerable.Range(0, maxConnections).Select(_ => Work());

			//最大接続数の分だけ待機用のタスクを生成し、どれかが終了するまで待機(実質は無限ループ)
			Task.WaitAll(serverTasks.ToArray());
			listener.Stop();
		}

		private void Infomation(string format, params object[] args)
		{
#if DEBUG
			Trace.TraceInformation(string.Format(format, args), "Information");
#endif
		}
	}
}
