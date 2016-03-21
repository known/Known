<%@ Control Language="C#" AutoEventWireup="true" Inherits="Known.Web.Admin.Controls.RoleManage" %>
<button type="button" class="btn btn-primary" data-toggle="modal" onclick="dialog('#roleModal');"><i class="glyphicon glyphicon-plus"></i>&nbsp;新&nbsp;增</button>
<div class="table-responsive">
    <table class="table table-hover">
        <thead>
            <tr>
                <th>#</th>
                <th>角色名称</th>
                <th>角色描述</th>
                <th>操作</th>
            </tr>
        </thead>
        <tbody>
            <%int i = 0; foreach (RoleInfo item in Roles) {%>
            <tr>
                <td><%=++i%></td>
                <td><%=HtmlEncode(item.Name)%></td>
                <td><%=HtmlEncode(item.Description)%></td>
                <td>
                    <%=GetGridButton("权限设置", "glyphicon glyphicon-cog", "Role.setRight('" + item.Id + "');", item.Id == "1")%>
                    <%=GetGridButton("编辑", "glyphicon glyphicon-edit", "Role.edit('" + item.Id + "');")%>
                    <%=GetGridButton("删除", "glyphicon glyphicon-remove", "Role.remove('" + item.Id + "');", item.Id == "1")%>
                </td>
            </tr>
            <%}%>
        </tbody>
    </table>
</div>

<div class="modal fade" id="roleModal" tabindex="-1" role="dialog" aria-labelledby="roleLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="roleLabel"><i class="glyphicon glyphicon-modal-window"></i>&nbsp;角色维护</h4>
            </div>
            <div class="modal-body">
                <input type="hidden" id="Id" name="Id">
                <div class="form-group">
                    <label for="Name">角色名称</label>
                    <input type="text" id="Name" name="Name" class="form-control" required>
                </div>
                <div class="form-group">
                    <label for="Description">角色描述</label>
                    <textarea id="Description" name="Description" class="form-control" rows="4"></textarea>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" onclick="return Role.save();"><i class="glyphicon glyphicon-ok"></i>&nbsp;保&nbsp;存</button>
                <button type="button" class="btn btn-default" data-dismiss="modal"><i class="glyphicon glyphicon-remove"></i>&nbsp;关&nbsp;闭</button>
            </div>
        </div>
    </div>
</div>

<div class="modal fade" id="rightModal" tabindex="-1" role="dialog" aria-labelledby="roleLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="rightLabel"><i class="glyphicon glyphicon-modal-window"></i>&nbsp;角色权限维护 - <span id="RoleName"></span></h4>
            </div>
            <div class="modal-body">
                <input type="hidden" id="RoleId" name="RoleId">
                <div id="menus" class="table-responsive"></div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" onclick="return Role.saveRight();"><i class="glyphicon glyphicon-ok"></i>&nbsp;保&nbsp;存</button>
                <button type="button" class="btn btn-default" data-dismiss="modal"><i class="glyphicon glyphicon-remove"></i>&nbsp;关&nbsp;闭</button>
            </div>
        </div>
    </div>
</div>