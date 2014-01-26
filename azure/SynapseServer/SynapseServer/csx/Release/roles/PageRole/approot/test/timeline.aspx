<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="timeline.aspx.cs" Inherits="PageRole.test.timeline" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
<link href="timeline.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="TimelineForm" runat="server">
		<asp:ObjectDataSource ID="TimelineSource" runat="server" SelectMethod="GetTimeline" TypeName="PageRole.test.ListViewObject" OnSelecting="TimelineSource_Selecting"/>
		<asp:ListView runat="server" ID="Timeline" DataSourceID="TimelineSource">
			<ItemTemplate>
				<li>
					<asp:Image runat="server" ImageUrl='<%# Eval("UserIdHash","~/Get/ProfileImage.aspx?uid_h={0}") %>' ImageAlign="Left" Height="64px" Width="64px" />
					<asp:Label CssClass="name" runat="server" Text='<%# Eval("Nickname") %>' /><span class="userId"> @<asp:Label runat="server" Text='<%# Eval("UserId") %>' /></span><br />
					<div class="tweet"><asp:Label ID="Label1" runat="server" Text='<%# Eval("Tweet") %>' /></div><br style="clear:left" />
					<asp:Label CssClass="time" runat="server" Text='<%# Eval("Time") %>' />
				</li>
			</ItemTemplate>
			<LayoutTemplate>
				<ul runat="server"><li runat="server" id="itemPlaceholder"/></ul>
			</LayoutTemplate>
		</asp:ListView>
    </form>
</body>
</html>