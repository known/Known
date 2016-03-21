<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="LinkList.aspx.cs" Inherits="Known.Web.Admin.LinkList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <div class="right">
        <h4>添加链接</h4>
        <p>
            <label class="label" for="Name">名称:</label>
            <input type="text" id="Name" name="Name" value="<%=Model.Name%>" style="width:96%;" class="text" />
        </p>
        <p>
            <label class="label" for="Url">链接地址:</label>
            <input type="text" id="Url" name="Url" value="<%=Model.Url%>" style="width:96%;" class="text" />
        </p>
        <p>
            <label class="label" for="Description">描述:</label>
            <input type="text" id="Description" name="Description" value="<%=Model.Description%>" style="width:96%;" class="text" />
        </p>
        <p>
            <label class="label" for="Position">位置:</label>
            <select id="Position" name="Position">
                <option value="0"<%if(Model.Position==0){%> selected="selected"<%}%>>导航</option>
                <option value="1"<%if(Model.Position==1){%> selected="selected"<%}%>>友情链接</option>
            </select>
        </p>
        <p>
            <label class="label" for="DisplayOrder">排序:</label>
            <input type="text" id="DisplayOrder" name="DisplayOrder" value="<%=Model.DisplayOrder%>" style="width:50px;" class="text" />
            <span class="m_desc">越小越靠前</span>
        </p>
        <p>
            <input type="checkbox" id="IsShow" name="IsShow"<%if(Model.IsShow){%> checked="checked"<%}%> />
            <label for="IsShow">显示</label>
            <input type="checkbox" id="IsNewWindow" name="IsNewWindow"<%if(Model.IsNewWindow){%> checked="checked"<%}%> />
            <label for="IsNewWindow">新窗口</label>
        </p>
        <p>
            <input type="submit" class="button" value="保存" />
        </p>
        <p class="notice">提示:${siteurl}表示根目录.</p>
    </div>
    <div class="left" >
        <table>
            <tr class="category">
                <td>排序</td>
                <td>名称</td>
                <td style="width:40%;">描述</td>
                <td>创建日期</td>
                <td>操作</td>
            </tr>
            <%foreach (Link item in Models){%>
            <tr class="row">
                <td><%=item.DisplayOrder%></td>
                <td>[<%if(item.Position==0){%>导航<%}else{%>友情链接<%}%>]<a href="<%=FormatUrl(item.Url)%>"><%=item.Name%></a></td>
                <td><%=item.Description%></td>
                <td><%=FormatDate(item.InsertTime)%></td>
                <td>
                    <a href="linklist.aspx?operate=update&id=<%=item.Id%>">编辑</a> | <a href="linklist.aspx?operate=delete&id=<%=item.Id%>" onclick="return confirm('确定要删除吗？');">删除</a>
                </td>
            </tr>
            <%}%>
        </table>
    </div>
</asp:Content>
