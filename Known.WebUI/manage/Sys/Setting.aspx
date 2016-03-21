<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" Inherits="Known.Web.Pages.Setting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <form method="post">
        <%foreach(SettingInfo item in Settings){%>
        <div class="form-group">
            <label for="<%=item.Code%>"><%=item.Name%></label>
            <%=GetHtmlControl(item)%>
        </div>
        <%}%>
        <div class="form-group">
            <button type="submit" class="btn btn-primary"><i class="glyphicon glyphicon-ok"></i>&nbsp;保&nbsp;存</button>
        </div>
    </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Footer" runat="server">
</asp:Content>