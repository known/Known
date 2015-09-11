<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="PostList.aspx.cs" Inherits="Known.Web.Admin.PostList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <div style="margin:5px 0;">
        标题：<input type="text" id="txtOrderNo" name="txtOrderNo" class="text" />
        创建日期：
        <input type="text" id="txtFrom" name="txtFrom" class="text Wdate" style="width:120px;" onclick="WdatePicker()" />
        到
        <input type="text" id="txtTo" name="txtTo" class="text Wdate" style="width:120px;" onclick="WdatePicker()" />
        状态：<select id="ddlStatus" name="ddlStatus"><option>所有</option><option>草稿</option><option>发布</option><option>关闭</option></select>
        &nbsp;<input type="button" class="button" value="查询" />
        &nbsp;<input type="button" class="button" value="添加" onclick="location='PostEdit.aspx';" />
    </div>
    <table>
        <tr class="category">
            <td>编号</td><td>标题</td><td>状态</td><td>创建人</td><td>创建时间</td><td>操作</td>
        </tr>
        <tr class="row">
            <td>1</td>
            <td><a href="/kweb/post/1" target="_blank">公司简介</a></td>
            <td>草稿</td>
            <td>admin</td>
            <td>2015-06-16 16:44:35</td>
            <td><a href="#">编辑</a> | <a href="#" onclick="return confirm('确认删除？');">删除</a></td>
        </tr>
        <tr class="row">
            <td>2</td>
            <td><a href="/kweb/post/1" target="_blank">联系方式</a></td>
            <td>草稿</td>
            <td>admin</td>
            <td>2015-06-16 16:44:35</td>
            <td><a href="#">编辑</a> | <a href="#" onclick="return confirm('确认删除？');">删除</a></td>
        </tr>
        <tr class="row">
            <td>3</td>
            <td><a href="/kweb/post/1" target="_blank">服务条款</a></td>
            <td>草稿</td>
            <td>admin</td>
            <td>2015-06-16 16:44:35</td>
            <td><a href="#">编辑</a> | <a href="#" onclick="return confirm('确认删除？');">删除</a></td>
        </tr>
        <tr class="row">
            <td>4</td>
            <td><a href="/kweb/post/1" target="_blank">服务协议</a></td>
            <td>草稿</td>
            <td>admin</td>
            <td>2015-06-16 16:44:35</td>
            <td><a href="#">编辑</a> | <a href="#" onclick="return confirm('确认删除？');">删除</a></td>
        </tr>
    </table>
</asp:Content>
