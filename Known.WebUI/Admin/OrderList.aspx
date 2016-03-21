<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="OrderList.aspx.cs" Inherits="Known.Web.Admin.OrderList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <div style="margin:5px 0;">
        订单号：<input type="text" id="txtOrderNo" name="txtOrderNo" class="text" />
        订单日期：
        <input type="text" id="txtFrom" name="txtFrom" class="text Wdate" style="width:120px;" onclick="WdatePicker()" />
        到
        <input type="text" id="txtTo" name="txtTo" class="text Wdate" style="width:120px;" onclick="WdatePicker()" />
        状态：<select id="ddlStatus" name="ddlStatus"><option>所有</option><option>待处理</option><option>已处理</option><option>关闭</option></select>
        &nbsp;<input type="button" class="button" value="查询" />
    </div>
    <table>
        <tr class="category">
            <td>订单编号</td>
            <td>货物信息</td>
            <td>装货地址</td>
            <td>卸货地址</td>
            <td>承运信息</td>
            <td style="width:60px;">金额(元)</td>
            <td style="width:60px;">状态</td>
            <td style="width:80px;">操作</td>
        </tr>
        <tr class="row">
            <td>20150616164435675<br /><span class="small gray">2015-06-16 16:44:35</span></td>
            <td>DELL显示器 LED 19# 宽屏<br /><span class="small gray">重量：1吨；体积：800立方；长度：5米</span></td>
            <td>田三友（18922335656）<br /><span class="small gray">江苏 苏州 新区 竹园路1号 苏州AAA有限公司</span></td>
            <td>李四（18911225656）<br /><span class="small gray">江苏 昆山 经济开发区 长江路10号 苏州BBB有限公司</span></td>
            <td></td>
            <td></td>
            <td>待处理</td>
            <td><a href="#" class="thickbox">处理</a> | <a href="#">关闭</a></td>
        </tr>
        <tr class="row">
            <td>20150616164435675<br /><span class="small gray">2015-06-16 16:44:35</span></td>
            <td>DELL显示器 LED 19# 宽屏<br /><span class="small gray">重量：1吨；体积：800立方；长度：5米</span></td>
            <td>张三（18922335656）<br /><span class="small gray">江苏 苏州 新区 竹园路1号 苏州AAA有限公司</span></td>
            <td>马大三（18911225656）<br /><span class="small gray">江苏 昆山 经济开发区 长江路10号 苏州BBB有限公司</span></td>
            <td>仇老五（18955665656）<br /><span class="small gray">苏E-87T78</span></td>
            <td>1000</td>
            <td>已处理</td>
            <td><a href="#">打印</a> | <a href="#">关闭</a></td>
        </tr>
        <tr class="row">
            <td>20150616164435675<br /><span class="small gray">2015-06-16 16:44:35</span></td>
            <td>DELL显示器 LED 19# 宽屏<br /><span class="small gray">重量：1吨；体积：800立方；长度：5米</span></td>
            <td>张三（18922335656）<br /><span class="small gray">江苏 苏州 新区 竹园路1号 苏州AAA有限公司</span></td>
            <td>李四（18911225656）<br /><span class="small gray">江苏 常熟 经济开发区 长江路16号 苏州BBB有限公司</span></td>
            <td>葛二雷（18955665656）<br /><span class="small gray">苏E-87T78</span></td>
            <td>1500</td>
            <td>关闭</td>
            <td></td>
        </tr>
    </table>
</asp:Content>
