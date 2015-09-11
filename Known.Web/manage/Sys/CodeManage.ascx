<%@ Control Language="C#" AutoEventWireup="true" Inherits="Known.Web.Admin.Controls.CodeManage" %>
<button type="button" class="btn btn-primary" data-toggle="modal" onclick="dialog('#codeModal');"><i class="glyphicon glyphicon-plus"></i>&nbsp;新&nbsp;增</button>
<div class="table-responsive">
    <table class="table table-hover">
        <thead>
            <tr>
                <th>#</th>
                <th>类别</th>
                <th>代码</th>
                <th>名称</th>
                <th>排序</th>
                <th>操作</th>
            </tr>
        </thead>
        <tbody>
            <%int i = 0; foreach (CodeInfo item in Codes) {%>
            <tr>
                <td><%=++i%></td>
                <td><%=HtmlEncode(item.Category)%></td>
                <td><%=HtmlEncode(item.Code)%></td>
                <td><%=HtmlEncode(item.Name)%></td>
                <td><%=item.Sequence%></td>
                <td>
                    <%=GetGridButton("编辑", "glyphicon glyphicon-edit", "Menu.edit('" + item.Code + "');")%>
                    <%=GetGridButton("删除", "glyphicon glyphicon-remove", "Menu.remove('" + item.Code + "');")%>
                    <%=GetGridButton("上移", "glyphicon glyphicon-arrow-up", "Menu.up('" + item.Code + "');")%>
                    <%=GetGridButton("下移", "glyphicon glyphicon-arrow-down", "Menu.down('" + item.Code + "');")%>
                </td>
            </tr>
            <%}%>
        </tbody>
    </table>
</div>

<div class="modal fade" id="menuModal" tabindex="-1" role="dialog" aria-labelledby="menuLabel">
    <div class="modal-dialog" role="document">
        <form method="post">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="menuLabel"><i class="glyphicon glyphicon-modal-window"></i>&nbsp;菜单维护 - <span id="ParentName"></span></h4>
            </div>
            <div class="modal-body">
                <input type="hidden" id="Parent" name="Parent">
                <div class="form-group">
                    <label for="Code">代码</label>
                    <input type="text" id="Code" name="Code" class="form-control" required>
                </div>
                <div class="form-group">
                    <label for="Name">名称</label>
                    <input type="text" id="Name" name="Name" class="form-control" required>
                </div>
                <div class="form-group">
                    <label for="Icon">Icon</label>
                    <input type="text" id="Icon" name="Icon" class="form-control">
                </div>
                <div class="form-group">
                    <label for="Url">Url</label>
                    <input type="text" id="Url" name="Url" class="form-control">
                </div>
            </div>
            <div class="modal-footer">
                <button type="submit" class="btn btn-primary" onclick="return Menu.save();"><i class="glyphicon glyphicon-ok"></i>&nbsp;保&nbsp;存</button>
                <button type="button" class="btn btn-default" data-dismiss="modal"><i class="glyphicon glyphicon-remove"></i>&nbsp;关&nbsp;闭</button>
            </div>
        </div>
        </form>
    </div>
</div>