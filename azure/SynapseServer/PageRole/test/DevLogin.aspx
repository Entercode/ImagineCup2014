<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DevLogin.aspx.cs" Inherits="PageRole.test.DevLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
		<h3>管理者専用ページ</h3>
		<asp:Label ID="Message" ForeColor="Red" runat="server" /><br />
		Password:<asp:TextBox ID="Pass" TextMode="Password" runat="server" /><br />
		<asp:Button ID="Submit" OnClick="Submit_Click" Text="Login" runat="server" />
    </form>
</body>
</html>
