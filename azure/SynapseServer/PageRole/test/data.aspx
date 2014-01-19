<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="data.aspx.cs" Inherits="PageRole.test.data" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body style="font-family: monospace">
	<a href="data.aspx">更新</a>
	<form id="AccountTableForm" runat="server">
		<h2>Account(アカウント情報)</h2>
		<asp:ObjectDataSource ID="AccountSource" runat="server" SelectMethod="GetAccounts" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteAccount" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="UserId" Type="String" /></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:GridView runat="server" ID="Accounts" DataSourceID="AccountSource" DataKeyNames="UserId">
			<Columns>
				<asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Delete" />
			</Columns>
		</asp:GridView>

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
		<asp:ObjectDataSource ID="StreetPassInformationSource" runat="server" SelectMethod="GetStreetPassInformations" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteStreetPassInformation" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="UserBindId" Type="Int32" /><asp:Parameter Name="PassedTime" Type="DateTime" /></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:GridView runat="server" ID="StreetPassInformations" DataSourceID="StreetPassInformationSource" DataKeyNames="UserBindId,PassedTime">
			<Columns>
				<asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Delete" />
			</Columns>
		</asp:GridView>

		<h2>AccountProfile(プロフィール)</h2>
		<asp:ObjectDataSource ID="AccountProfileSource" runat="server" SelectMethod="GetAccountProfiles" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteAccountProfile" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="UserId" Type="String"/></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:GridView runat="server" ID="AccountProfiles" DataSourceID="AccountProfileSource" DataKeyNames="UserId">
			<Columns>
				<asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Delete" />
				<asp:ImageField DataImageUrlFormatString="~/Get/ProfileImage.aspx?uid_h={0}" DataImageUrlField="UserIdHash"/>
			</Columns>
		</asp:GridView>

		<h2>Tweet(つぶやき)</h2>
		<asp:ObjectDataSource ID="TweetInformationSource" runat="server" SelectMethod="GetTweetInformations" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteTweetInformation" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="BindId" Type="Int32" /><asp:Parameter Name="ServerRecievedTime" Type="DateTime" /></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:GridView runat="server" ID="TweetInformations" DataSourceID="TweetInformationSource" DataKeyNames="BindId,ServerRecievedTime">
			<Columns>
				<asp:ButtonField ButtonType="Button" CommandName="Delete" Text="Delete" />
			</Columns>
		</asp:GridView>

		<hr />
		※備考
		<ul>
			<li>"password" => 5BAA61E4C9B93F3F0682250B6CF8331B7EE68FD8</li>
			<li>※UserIdHashは実際のテーブル上にはありません</li>
		</ul>
		メッセージ：<%=Message %>
	</form>
</body>
</html>
