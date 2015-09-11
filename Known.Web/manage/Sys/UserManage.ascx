<%@ Control Language="C#" AutoEventWireup="true" Inherits="Known.Web.Admin.Controls.UserManage" %>
<button type="button" class="btn btn-primary" data-toggle="modal" onclick="dialog('#userModal');"><i class="glyphicon glyphicon-plus"></i>&nbsp;新&nbsp;增</button>
<div class="table-responsive">
    <table class="table table-hover">
        <thead>
            <tr>
                <th>#</th>
                <th>用户名</th>
                <th>显示名</th>
                <th>Email</th>
                <th>角色</th>
                <th>操作</th>
            </tr>
        </thead>
        <tbody>
            <%int i = 0; foreach (UserInfo item in Users) {%>
            <tr>
                <td><%=++i%></td>
                <td><%=HtmlEncode(item.UserName)%></td>
                <td><%=HtmlEncode(item.DisplayName)%></td>
                <td><%=HtmlEncode(item.Email)%></td>
                <td><%=FormatRoles(item.Roles)%></td>
                <td>
                    <%=GetGridButton("重置密码", "glyphicon glyphicon-cog", "User.resetPwd('" + item.Id + "');")%>
                    <%=GetGridButton("编辑", "glyphicon glyphicon-edit", "User.edit('" + item.Id + "');")%>
                    <%=GetGridButton("删除", "glyphicon glyphicon-remove", "User.remove('" + item.Id + "');", item.Id == "1")%>
                </td>
            </tr>
            <%}%>
        </tbody>
    </table>
</div>

<div class="modal fade" id="userModal" tabindex="-1" role="dialog" aria-labelledby="userLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="userLabel"><i class="glyphicon glyphicon-modal-window"></i>&nbsp;用户维护</h4>
            </div>
            <div class="modal-body">
                <input type="hidden" id="Id" name="Id">
                <div class="form-group">
                    <label for="UserName">用户名</label>
                    <input type="text" id="UserName" name="UserName" class="form-control" required>
                    <p class="help-block">用户密码默认888888。</p>
                </div>
                <div class="form-group">
                    <label for="DisplayName">显示名</label>
                    <input type="text" id="DisplayName" name="DisplayName" class="form-control" required>
                </div>
                <div class="form-group">
                    <label for="Email">Email</label>
                    <input type="text" id="Email" name="Email" class="form-control">
                </div>
                <div class="form-group">
                    <label>角色</label>
                    <div id="roles" class="checkbox">
                    <%foreach (RoleInfo item in Roles) {%>
                        <label class="checkbox-inline"><input type="checkbox" name="Roles" value="<%=item.Id%>"><%=item.Name%></label>
                    <%}%>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" onclick="return User.save();"><i class="glyphicon glyphicon-ok"></i>&nbsp;保&nbsp;存</button>
                <button type="button" class="btn btn-default" data-dismiss="modal"><i class="glyphicon glyphicon-remove"></i>&nbsp;关&nbsp;闭</button>
            </div>
        </div>
    </div>
</div>