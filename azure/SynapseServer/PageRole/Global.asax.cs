using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using PageRole;

namespace PageRole
{
	public class Global : HttpApplication
	{
		void Application_Start(object sender, EventArgs e)
		{
			// アプリケーションのスタートアップで実行するコードです
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterOpenAuth();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}

		void Application_End(object sender, EventArgs e)
		{
			//  アプリケーションのシャットダウンで実行するコードです

		}

		void Application_Error(object sender, EventArgs e)
		{
			// ハンドルされていないエラーが発生したときに実行するコードです

		}

		void Application_BeginRequest(object sender, EventArgs e)
		{
			if (Request.FilePath.Contains("/test/"))
			{
				return;
			}

			if (Request.FilePath.Contains("/document/"))
			{
				return;
			}

			if ("/".Equals(Request.FilePath))
			{
				return;
			}

			if (Request.FilePath.Contains("/Get/") && "GET".Equals(Request.HttpMethod))
			{
				return;
			}

			if (!"POST".Equals(Request.HttpMethod))
			{
				Response.StatusCode = 404;
				Response.End();
			}
		}
	}
}
