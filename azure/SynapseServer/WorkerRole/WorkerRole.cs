using System;
using System.Diagnostics;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		//AsyncServer server;
		public override void Run()
		{
			//server.Run();
			while (true)
			{
				System.Threading.Thread.Sleep(10000);
			}
		}

		public override bool OnStart()
		{
			int limit =int.Parse(CloudConfigurationManager.GetSetting("ConnectionLimit"));

			// 同時接続の最大数を設定します
			ServicePointManager.DefaultConnectionLimit = limit;

			var endPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["StreetPass"].IPEndpoint;

			//server = new AsyncServer("+", endPoint.Port, limit);

			// 構成の変更を処理する方法については、
			// MSDN トピック (http://go.microsoft.com/fwlink/?LinkId=166357) を参照してください。

#if DEBUG
			DiagnosticMonitorConfiguration config = DiagnosticMonitor.GetDefaultInitialConfiguration();
			config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
			config.Logs.ScheduledTransferLogLevelFilter = LogLevel.Information;
			DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config);
#endif

			return base.OnStart();
		}
	}
}
