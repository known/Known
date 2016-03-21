<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderHandle.aspx.cs" Inherits="Known.Web.Admin.OrderHandle" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h1>处理订单[<%=Request.QueryString["no"]%>]</h1>
    <p></p>
    </div>
    </form>
</body>
</html>
