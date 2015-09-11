<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Known.Web.Admin.Default1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <h3>待处理订单</h3>
    <table>
        <tr class="category">
            <td>订单编号</td><td>货物信息</td><td>装货地址</td><td>卸货地址</td><td>状态</td><td>操作</td>
        </tr>
        <tr class="row">
            <td>20150616164435674<br /><span class="small gray">2015-06-16 16:44:35</span></td>
            <td>DELL显示器 LED 19# 宽屏<br /><span class="small gray">重量：1吨；体积：800立方；长度：5米</span></td>
            <td>张三（18922335656）<br /><span class="small gray">江苏 苏州 新区 竹园路1号 苏州AAA有限公司</span></td>
            <td>李四（18911225656）<br /><span class="small gray">江苏 昆山 经济开发区 长江路10号 苏州BBB有限公司</span></td>
            <td>待处理</td>
            <td><a href="orderhandle.aspx?no=20150616164435674&height=300&width=500&modal=true" class="thickbox">处理</a> | <a href="#" onclick="return confirm('确认关闭？');">关闭</a></td>
        </tr>
        <tr class="row">
            <td>20150616164435675<br /><span class="small gray">2015-06-16 16:44:35</span></td>
            <td>DELL显示器 LED 19# 宽屏<br /><span class="small gray">重量：1吨；体积：800立方；长度：5米</span></td>
            <td>张三（18922335656）<br /><span class="small gray">江苏 苏州 新区 竹园路1号 苏州AAA有限公司</span></td>
            <td>李四（18911225656）<br /><span class="small gray">江苏 昆山 经济开发区 长江路10号 苏州BBB有限公司</span></td>
            <td>待处理</td>
            <td><a href="orderhandle.aspx?no=20150616164435675&height=300&width=500&modal=true" class="thickbox">处理</a> | <a href="#" onclick="return confirm('确认关闭？');">关闭</a></td>
        </tr>
    </table>
    <h3>待审核会员</h3>
    <table>
        <tr class="category">
            <td>真实姓名</td>
            <td>手机号</td>
            <td>注册日期</td>
            <td>注册IP</td>
            <td style="width:80px;">状态</td>
            <td style="width:160px;">操作</td>
        </tr>
        <%foreach (Member item in Members){%>
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
