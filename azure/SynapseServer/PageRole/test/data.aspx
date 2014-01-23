<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="data.aspx.cs" Inherits="PageRole.test.data" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
<link href="data.css" rel="stylesheet" type="text/css" />
</head>
<body style="font-family: monospace">
	<a href="data.aspx">更新</a>
	<form id="AccountTableForm" runat="server">
		<h2>Account(アカウント情報)</h2>
		<asp:ObjectDataSource ID="AccountSource" runat="server" SelectMethod="GetAccounts" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteAccount" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="UserId" Type="String" /></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:ListView runat="server" DataSourceID="AccountSource" DataKeyNames="UserId">
			<LayoutTemplate>
				<table>
					<tr>
						<th>UserId</th>
						<th>Nickname</th>
						<th>MailAddress</th>
						<th>PasswordHash</th>
						<th class="meta">UserIdHash</th>
						<th>Delete</th>
					</tr>
					<tr runat="server" id="itemPlaceholder"></tr>
				</table>
			</LayoutTemplate>
			<ItemTemplate>
				<tr>
					<td><%# Eval("UserId") %></td>
					<td><%# Eval("Nickname") %></td>
					<td><%# Eval("MailAddress") %></td>
					<td><%# Eval("PasswordHash") %></td>
					<td class="meta"><%# Eval("UserIdHash") %></td>
					<td><asp:Button runat="server" CommandName="Delete" Text="Delete" /></td>
				</tr>
			</ItemTemplate>
		</asp:ListView>

		<h2>AccountDevice(アカウントとデバイスのひも付け情報)</h2>
		<asp:ObjectDataSource ID="AccountDeviceSource" runat="server" SelectMethod="GetAccountDevices" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteAccountDevice" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="BindId" Type="Int32" /></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:ListView runat="server" DataSourceID="AccountDeviceSource" DataKeyNames="BindId">
			<LayoutTemplate>
				<table>
					<tr>
						<th>Timeline</th>
						<th>UserId</th>
						<th>BindId</th>
						<th>DeviceIdHash</th>
						<th>SessionId</th>
						<th>Delete</th>
					</tr>
					<tr runat="server" id="itemPlaceholder"></tr>
				</table>
			</LayoutTemplate>
			<ItemTemplate>
				<tr>
					<td><a target="_blank" href='<%# string.Format("timeline.aspx?uid_h={0}&sid={1}", Eval("UserIdHash"), Eval("SessionId")) %>'>Timeline</a></td>
					<td><%# Eval("UserId") %></td>
					<td><%# Eval("BindId") %></td>
					<td><%# Eval("DeviceIdHash") %></td>
					<td><%# Eval("SessionId") %></td>
					<td><asp:Button ID="Button1" runat="server" CommandName="Delete" Text="Delete" /></td>
				</tr>
			</ItemTemplate>
		</asp:ListView>

		<h2>StreetPass(すれ違い情報)</h2>
		<asp:ObjectDataSource ID="StreetPassInformationSource" runat="server" SelectMethod="GetStreetPassInformations" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteStreetPassInformation" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="UserBindId" Type="Int32" /><asp:Parameter Name="PassedTime" Type="DateTime" /></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:ListView runat="server" DataSourceID="StreetPassInformationSource" DataKeyNames="UserBindId,PassedTime">
			<LayoutTemplate>
				<table>
					<tr>
						<th>UserBindId</th>
						<th>PassedBindId</th>
						<th>PassedTime</th>
						<th>Delete</th>
					</tr>
					<tr runat="server" id="itemPlaceholder"></tr>
				</table>
			</LayoutTemplate>
			<ItemTemplate>
				<tr>
					<td><%# Eval("UserBindId") %></td>
					<td><%# Eval("PassedBindId") %></td>
					<td><%# Eval("PassedTime") %></td>
					<td><asp:Button ID="Button1" runat="server" CommandName="Delete" Text="Delete" /></td>
				</tr>
			</ItemTemplate>
		</asp:ListView>

		<h2>AccountProfile(プロフィール)</h2>
		<asp:ObjectDataSource ID="AccountProfileSource" runat="server" SelectMethod="GetAccountProfiles" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteAccountProfile" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="UserId" Type="String"/></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:ListView runat="server" DataSourceID="AccountProfileSource" DataKeyNames="UserId">
			<LayoutTemplate>
				<table>
					<tr>
						<th>Image</th>
						<th>UserId</th>
						<th>Profile</th>
						<th>Delete</th>
					</tr>
					<tr runat="server" id="itemPlaceholder"></tr>
				</table>
			</LayoutTemplate>
			<ItemTemplate>
				<tr>
					<td><asp:Image runat="server" Height="64px" Width="64px" ImageUrl='<%# Eval("UserIdHash", "~/Get/ProfileImage.aspx?uid_h={0}") %>' /></td>
					<td><%# Eval("UserId") %></td>
					<td><%# Eval("Profile") %></td>
					<td><asp:Button ID="Button1" runat="server" CommandName="Delete" Text="Delete" /></td>
				</tr>
			</ItemTemplate>
		</asp:ListView>

		<h2>Tweet(つぶやき)</h2>
		<asp:ObjectDataSource ID="TweetInformationSource" runat="server" SelectMethod="GetTweetInformations" TypeName="PageRole.test.GridViewObject" DeleteMethod="DeleteTweetInformation" OnDeleting="DataDeleting">
			<DeleteParameters><asp:Parameter Name="BindId" Type="Int32" /><asp:Parameter Name="ServerRecievedTime" Type="DateTime" /></DeleteParameters>
		</asp:ObjectDataSource>
		<asp:ListView ID="ListView1" runat="server" DataSourceID="TweetInformationSource" DataKeyNames="BindId,ServerRecievedTime">
			<LayoutTemplate>
				<table>
					<tr>
						<th class="meta">Nickname</th>
						<th class="meta">UserId</th>
						<th style="width:340px">Tweet</th>
						<th>ClientTweetTime</th>
						<th>BindId</th>
						<th>ServerRecievedTime</th>
						<th>Delete</th>
					</tr>
					<tr runat="server" id="itemPlaceholder"></tr>
				</table>
			</LayoutTemplate>
			<ItemTemplate>
				<tr>
					<td class="meta"><%# Eval("Nickname") %></td>
					<td class="meta"><%# Eval("UserId") %></td>
					<td><%# Eval("Tweet") %></td>
					<td><%# Eval("ClientTweetTime") %></td>
					<td><%# Eval("BindId") %></td>
					<td><%# Eval("ServerRecievedTime") %></td>
					<td><asp:Button ID="Button1" runat="server" CommandName="Delete" Text="Delete" /></td>
				</tr>
			</ItemTemplate>
		</asp:ListView>


		<hr />
		※備考
		<ul>
			<li>"password" => 5BAA61E4C9B93F3F0682250B6CF8331B7EE68FD8</li>
			<li>実際にデータベースに無いカラムは文字色が薄い</li>
		</ul>
		メッセージ：<%=Message %>
	</form>
</body>
</html>
