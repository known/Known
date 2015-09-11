<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="Known.Web.Admin.UserList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <div class="right">
        <h4>添加用户</h4>
        <p>
            <label class="label" for="UserType">角色:</label>
            <select id="UserType" name="UserType" style="width:96%;">
                <option value="0"<%if(Model.UserType==UserType.Administrator){%> selected="selected"<%}%>>管理员</option>
                <option value="1"<%if(Model.UserType==UserType.CommonUser){%> selected="selected"<%}%>>普通用户</option>
            </select>
        </p>
        <p>
            <label class="label" for="UserName">用户名:<span class="small gray">(字母,数字,中文,连字符)</span></label>
            <input type="text" id="UserName" name="UserName" value="<%=Model.UserName%>" style="width:96%;" class="text" />
        </p>
        <p>
            <label class="label" for="Password">密码:<span class="small gray"><%=PasswordTips%></span></label>
            <input type="password" id="Password" name="Password" style="width:96%;" class="text" />
        </p>
        <p>
            <label class="label" for="Password2">确认密码:<span class="small gray"><%=PasswordTips%></span></label>
            <input type="password" id="Password2" name="Password2" style="width:96%;" class="text" />
        </p>
        <p>
            <label class="label" for="Email">邮箱:</label>
            <input type="text" id="Email" name="Email" value="<%=Model.Email%>" style="width:96%;" class="text" />
        </p>
        <p>
            <label class="label" for="DisplayOrder">排序:</label>
            <input type="text" id="DisplayOrder" name="DisplayOrder" value="<%=Model.DisplayOrder%>" style="width:50px;" class="text" />
            <span class="m_desc">越小越靠前</span>
        </p>
        <p>
            <input type="checkbox" id="Enabled" name="Enabled"<%if(Model.Enabled){%> checked="checked"<%}%> />
            <label for="Enabled">使用</label>
        </p>
        <p>
            <input type="submit" class="button" value="保存" />
        </p>
    </div>
    <div class="left">
        <table>
            <tr class="category">
                <td>排序</td>
                <td>用户名</td>
                <td>邮箱</td>
                <td>状态</td>
                <td>创建日期</td>
                <td>操作</td>
            </tr>
            <%foreach (User item in Models){%>
            <tr class="row">
                <td><%=item.DisplayOrder%></td>
                <td>[<%=FormatEnum(item.UserType)%>]<%=item.UserName%></td>
                <td><%=item.Email%></td>
                <td><%if(item.Enabled){%><span class="green" title="使用">✔</span><%}else{%><span class="red" title="停用">✘</span><%}%></td>
                <td><%=FormatDate(item.InsertTime)%></td>
                <td>
                    <a href="userlist.aspx?operate=update&id=<%=item.Id%>">编辑</a> | <a href="userlist.aspx?operate=delete&id=<%=item.Id%>" onclick="return confirm('确定要删除吗？');">删除</a>
                </td>
            </tr>
            <%}%>
        </table>
    </div>
</asp:Content>
