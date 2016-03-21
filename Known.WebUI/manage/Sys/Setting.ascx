<%@ Control Language="C#" AutoEventWireup="true" Inherits="Known.Web.Admin.Controls.Setting" %>
<%foreach(SettingInfo item in Settings){%>
<div class="form-group">
    <label for="<%=item.Code%>"><%=item.Name%></label>
    <%=GetHtmlControl(item)%>
</div>
<%}%>
<div class="form-group">
    <button type="submit" class="btn btn-primary" onclick="return Setting.save('<%=Keys%>');"><i class="glyphicon glyphicon-ok"></i>&nbsp;保&nbsp;存</button>
</div>
