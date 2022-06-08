function SysRole() {
    var url = {
        QueryModels: baseUrl + '/System/QueryRoles',
        DeleteModels: baseUrl + '/System/DeleteRoles',
        SaveModel: baseUrl + '/System/SaveRole',
        GetRoleModules: baseUrl + '/System/GetRoleModules',
        SaveRoleModules: baseUrl + '/System/SaveRoleModules'
    };
    var treeObj;

    var view = new View('SysRole', {
        url: url,
        columns: [
            { field: 'Id', type: 'hidden' },
            { field: 'AppId', type: 'hidden' },
            { title: Language.Name, field: 'Name', width: '150px', query: true, type: 'text', required: true, inputBlock: true },
            { title: Language.Status, field: 'Enabled', width: '100px', type: 'checkbox', code: 'Enabled', required: true },
            { title: Language.Note, field: 'Note', type: 'textarea' }
        ],
        formOption: {
            style: 'form-block form-split', width: 1024, height: 450, labelWidth: 60,
            ahtml: '<div class="form-split-view"><ul id="treeRoleRight"></ul></div>',
            data: { Id: '', AppId: appId, Enabled: 1 },
            setData: function (e) {
                treeObj = new Tree('treeRoleRight', {
                    url: url.GetRoleModules + '?roleId=' + e.data.Id,
                    check: true, expandAll: true,
                    onLoad: function (data) {
                        var ul = $('.tree-button').parent().parent().addClass('tree-button-ul');
                        ul.parent().addClass('tree-page-li');
                    }
                });
            },
            submitData: function (d) {
                var data = treeObj.getCheckedNodes();
                var ids = data.map(function (d) { return d.id; });
                return { ids: JSON.stringify(ids) };
            }
        },
        gridOption: {
            toolbar: {
                right: function (e) {
                    //e.selectRow(function (e) {
                    //    _selectRight(e.row, function (data) {
                    //        var ids = data.map(function (d) { return d.id; });
                    //        Ajax.post(url.SaveRoleModules, {
                    //            roleId: e.row.Id,
                    //            data: JSON.stringify(ids)
                    //        });
                    //    });
                    //});
                }
            }
        }
    });

    //methods
    this.render = function (dom) {
        view.render().appendTo(dom);
    }

    this.mounted = function () {
        view.load({ AppId: appId });
    }

    //private
    //function _selectRight(data, callback) {
    //    Layer.open({
    //        title: Language.PermissionSetting + ' - ' + data.Name,
    //        width: 850, height: 450,
    //        content: '<ul id="treeRoleRight"></ul>',
    //        success: function () {
    //            treeObj = new Tree('treeRoleRight', {
    //                url: url.GetRoleModules + '?roleId=' + data.Id,
    //                check: true, expandAll: true,
    //                onLoad: function (data) {
    //                    var ul = $('.tree-button').parent().parent().addClass('tree-button-ul');
    //                    ul.parent().addClass('tree-page-li');
    //                }
    //            });
    //        },
    //        buttons: [{
    //            text: Language.OK, handler: function (e) {
    //                e.close();
    //                var data = treeObj.getCheckedNodes();
    //                callback && callback(data);
    //            }
    //        }]
    //    });
    //}
}

$.extend(Page, {
    SysRole: { component: new SysRole() }
});