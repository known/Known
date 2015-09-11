<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="MemberList.aspx.cs" Inherits="Known.Web.Admin.MemberList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <div style="margin:5px 0;">
        手机号：<input type="text" id="txtOrderNo" name="txtOrderNo" class="text" />
        注册日期：
        <input type="text" id="txtFrom" name="txtFrom" class="text Wdate" style="width:120px;" onclick="WdatePicker()" />
        到
        <input type="text" id="txtTo" name="txtTo" class="text Wdate" style="width:120px;" onclick="WdatePicker()" />
        状态：<select id="ddlStatus" name="ddlStatus"><option>所有</option><option>待审核</option><option>审核通过</option><option>审核不通过</option></select>
        &nbsp;<input type="button" class="button" value="查询" />
    </div>
    <table>
        <tr class="category">
            <td>真实姓名</td>
            <td>手机号</td>
            <td>注册日期</td>
            <td>注册IP</td>
            <td style="width:80px;">状态</td>
            <td style="width:160px;">操作</td>
        </tr>
        <%foreach (Member item in Models){%>
        <tr class="row">
            <td><%=item.RealName%></td>
            <td><%=item.Mobile%></td>
            <td><%=FormatDate(item.InsertTime)%></td>
            <td><%=item.RegisterIP%></td>
            <td><%if(item.Status==0){%>待审核<%}else if(item.Status==1){%>审核通过<%}else{%>审核不通过<%}%></td>
            <td><a href="#" class="thickbox">审核通过</a> | <a href="#">审核不通过</a></td>
        </tr>
        <%}%>
    </table>
</asp:Content>
