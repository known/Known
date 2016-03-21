<%@ Control Language="C#" AutoEventWireup="true" Inherits="Known.Web.Admin.Controls.MenuManage" %>
<button type="button" class="btn btn-primary" data-toggle="modal" onclick="dialog('#menuModal');"><i class="glyphicon glyphicon-plus"></i>&nbsp;新增一级菜单</button>
<div class="table-responsive">
    <table class="table table-hover">
        <thead>
            <tr>
                <th style="width:1px;"></th>
                <th>代码</th>
                <th colspan="2">名称</th>
                <th>Url</th>
                <th>排序</th>
                <th>操作</th>
            </tr>
        </thead>
        <tbody>
            <%int i = 0; foreach (MenuInfo item in Menus) {%>
            <%string attachAttrs = string.IsNullOrEmpty(item.Parent) ? " class=\"info\"" : " parent=\"" + item.Parent + "\" class=\"hide\""; %>
            <%string toggle = string.IsNullOrEmpty(item.Parent) ? "<i id=\"" + item.Id + "\" class=\"glyphicon glyphicon-chevron-right\" onclick=\"Menu.toggle('" + item.Id + "');\" aria-expanded=\"0\"></i>" : ""; %>
            <tr<%=attachAttrs%>>
                <td><%=toggle%></td>
                <td><%=HtmlEncode(item.Code)%></td>
                <td style="width:1px;"><i class="<%=HtmlEncode(item.Icon)%>"></i></td>
                <td><%=HtmlEncode(item.Name)%></td>
                <td><%=item.Url%></td>
                <td><%=item.Sequence%></td>
                <td>
                    <%=GetGridButton("添加子菜单", "glyphicon glyphicon-plus", "Menu.addSub('" + item.Id + "','" + item.Name + "');")%>
                    <%=GetGridButton("编辑", "glyphicon glyphicon-edit", "Menu.edit('" + item.Id + "');")%>
                    <%=GetGridButton("删除", "glyphicon glyphicon-remove", "Menu.remove('" + item.Id + "');")%>
                    <%=GetGridButton("上移", "glyphicon glyphicon-arrow-up", "Menu.up('" + item.Id + "');")%>
                    <%=GetGridButton("下移", "glyphicon glyphicon-arrow-down", "Menu.down('" + item.Id + "');")%>
                </td>
            </tr>
            <%}%>
        </tbody>
    </table>
</div>

<div class="modal fade" id="menuModal" tabindex="-1" role="dialog" aria-labelledby="menuLabel">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title" id="menuLabel"><i class="glyphicon glyphicon-modal-window"></i>&nbsp;<span id="menuTitle">新增一级菜单</span></h4>
            </div>
            <div class="modal-body">
                <input type="hidden" id="Id" name="Id">
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
                <button type="button" class="btn btn-primary" onclick="return Menu.save();"><i class="glyphicon glyphicon-ok"></i>&nbsp;保&nbsp;存</button>
                <button type="button" class="btn btn-default" data-dismiss="modal"><i class="glyphicon glyphicon-remove"></i>&nbsp;关&nbsp;闭</button>
            </div>
        </div>
    </div>
</div>