using Microsoft.WindowsAzure.ServiceRuntime;

namespace PageRole
{
	public class WebRole : RoleEntryPoint
	{
		public override bool OnStart()
		{
			// 構成の変更を処理する方法については、
			// MSDN トピック (http://go.microsoft.com/fwlink/?LinkId=166357) を参照してください。

			return base.OnStart();
		}
	}
}
