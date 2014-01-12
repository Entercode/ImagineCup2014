<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="data.aspx.cs" Inherits="PageRole.test.data" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
	<a href="data.aspx">更新</a>
	<form id="AccountTableForm" runat="server">
		<h2>AccountTable(アカウント情報)</h2>
		<asp:ObjectDataSource ID="AccountTableSource" runat="server" SelectMethod="GetAccounts" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteAccount" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="UserId" Type="String" /></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:GridView runat="server" ID="AccountTable" DataSourceID="AccountTableSource" DataKeyNames="UserId">
			<Columns>
				<asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Delete" />
			</Columns>
		</asp:GridView>
		<p>"password" => 5BAA61E4C9B93F3F0682250B6CF8331B7EE68FD8</p>
		<h2>AccountDevice(アカウントとデバイスのひも付け情報)</h2>
		<asp:ObjectDataSource ID="AccountDeviceSource" runat="server" SelectMethod="GetAccountDevices" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteAccountDevice" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="BindId" Type="Int32" /></DeleteParameters> 
		</asp:ObjectDataSource>
		<asp:GridView runat="server" ID="AccountDevices" DataSourceID="AccountDeviceSource" DataKeyNames="BindId">
			<Columns>
				<asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Delete" />
			</Columns>
		</asp:GridView>
		<h2>StreetPass(すれ違い情報)</h2>
		<asp:ObjectDataSource ID="StreetPassInformationSource" runat="server" SelectMethod="GetStreetPassInformation" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteStreetPassInformation" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="UserBindId" Type="Int32" /><asp:Parameter Name="PassedTime" Type="DateTime" /></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:GridView runat="server" ID="StreetPassInformation" DataSourceID="StreetPassInformationSource" DataKeyNames="UserBindId,PassedTime">
			<Columns>
				<asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Delete" />
			</Columns>
		</asp:GridView>
		<hr />
		メッセージ：<%=Message %>
	</form>
</body>
</html>
