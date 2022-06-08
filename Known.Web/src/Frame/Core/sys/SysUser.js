/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

Picker.action.user = {
    title: Language.SelectUser,
    url: baseUrl + '/System/QueryUsers',
    where: { OrgNo: curUser.CompNo },
    columns: [
        { title: Language.UserName, field: 'UserName', width: '100px', query: true },
        { title: Language.UserRealName, field: 'Name', width: '100px', query: true },
        { title: Language.Mobile, field: 'Mobile', width: '120px' },
        { title: Language.Email, field: 'Email', width: '150px' }
    ],
    valueField: 'UserName', textField: 'Name'
}

function SysUser(hasOrgTree) {
    var url = {
        QueryModels: baseUrl + '/System/QueryUsers',
        DeleteModels: baseUrl + '/System/DeleteUsers',
        ImportModels: baseUrl + '/System/ImportUsers',
        SaveModel: baseUrl + '/System/SaveUser',
        GetOrganizations: baseUrl + '/System/GetOrganizations',
        SetUserPwds: baseUrl + '/System/SetUserPwds',
        EnableUsers: baseUrl + '/System/EnableUsers',
        GetUserRoles: baseUrl + '/System/GetUserRoles',
        SaveUserRoles: baseUrl + '/System/SaveUserRoles'
    };
    var chkRoleList;

    var tree = !hasOrgTree
        ? null
        : new Tree('treeUser', {
            url: url.GetOrganizations, autoLoad: false,
            onClick: function (node) {
                view.loadGrid({ AppId: appId, OrgNo: node.code });
            }
        });

    var view = new View('SysUser', {
        url: url,
        left: tree,
        columns: [
            { field: 'Id', type: 'hidden' },
            { field: 'AppId', type: 'hidden' },
            { field: 'OrgNo', type: 'hidden' },
            { title: Language.UserName, field: 'UserName', width: '100px', query: true, sort: true, type: 'text', required: true },
            { title: Language.UserRealName, field: 'Name', width: '100px', query: true, sort: true, type: 'text', required: true },
            { title: Language.EnglishName, field: 'EnglishName', width: '100px', type: 'text' },
            { title: Language.Role, field: 'Role', width: '100px' },
            {
                title: Language.Gender, field: 'Gender', width: '60px', align: 'center', sort: true, format: function (d) {
                    return d.Gender === Language.Male ? Language.Male : '<span style="color: #F581B1;">' + Language.Female + '</span>';
                }, required: true, type: 'radio', code: [Language.Male, Language.Female]
            },
            { title: Language.Phone, field: 'Phone', width: '120px', type: 'text' },
            { title: Language.Mobile, field: 'Mobile', width: '120px', type: 'text' },
            { title: Language.Email, field: 'Email', width: '150px', type: 'text' },
            { title: Language.Status, field: 'Enabled', width: '60px', type: 'checkbox', code: 'Enabled', required: true },
            { title: Language.FirstLoginTime, field: 'FirstLoginTime', width: '140px', placeholder: DateTimeFormat, align: 'center' },
            { title: Language.FirstLoginIP, field: 'FirstLoginIP', width: '100px', align: 'center' },
            { title: Language.LastLoginTime, field: 'LastLoginTime', width: '140px', placeholder: DateTimeFormat, align: 'center' },
            { title: Language.LastLoginIP, field: 'LastLoginIP', width: '100px', align: 'center' },
            { title: Language.Role, field: 'Role', type: 'html', lineBlock: true, inputHtml: '<div id="chkUserRole"></div>', onlyForm: true },
            { title: Language.Note, field: 'Note', type: 'textarea', lineBlock: true }
        ],
        formOption: {
            data: { Id: '', Enabled: 1 },
            titleInfo: function (d) {
                var item = hasOrgTree ? tree.getNodeData(function (l) { return l.id === d.OrgNo; }) : null;
                return item ? '- <span class="bold">' + item.Code + '|' + item.Name + '</span>' : '';
            },
            setData: function (e) {
                e.form.UserName.setReadonly(e.data.Id !== '');
                $.get(url.GetUserRoles, {
                    userId: e.data.Id
                }, function (res) {
                    chkRoleList = new Input($('#chkUserRole'), {
                        field: 'UserRole',
                        type: 'checkbox',
                        code: res.roles,
                        value: res.value
                    });
                });
            },
            submitData: function (d) {
                var data = chkRoleList.getData();
                return { role: JSON.stringify(data) };
            }
        },
        gridOption: {
            autoQuery: false, width: '1800px',
            toolbar: {
                add: function (e) {
                    var orgNo = curUser.CompNo;
                    if (hasOrgTree) {
                        var node = tree.selectedNode;
                        if (!node) {
                            Layer.tips(Language.PleaseSelectOrg);
                            return;
                        }

                        orgNo = node.data.Code;
                    }
                    e.addRow({ Id: '', AppId: appId, Enabled: 1, OrgNo: orgNo });
                },
                //role: function (e) {
                //    e.selectRow(function (e) {
                //        _selectRole(e.row, function (data) {
                //            Ajax.post(url.SaveUserRoles, {
                //                userId: e.row.Id,
                //                data: JSON.stringify(data)
                //            }, function () {
                //                e.grid.reload();
                //            });
                //        });
                //    });
                //},
                setPwd: function (e) {
                    e.selectRows(function (e) {
                        Layer.confirm(Language.ConfirmResetPassword, function () {
                            Ajax.post(url.SetUserPwds, {
                                data: JSON.stringify(e.ids)
                            });
                        });
                    });
                },
                enable: function (e) { _enableUsers(e, 1); },
                disable: function (e) { _enableUsers(e, 0); }
            }
        }
    });

    //methods
    this.render = function (dom) {
        view.render().appendTo(dom);
    }

    this.mounted = function () {
        view.load({ AppId: appId, OrgNo: curUser.CompNo });
    }

    //private
    //function _selectRole(data, callback) {
    //    var userInfo = data.Name + '(' + data.UserName + ')', chkList;
    //    Layer.open({
    //        title: Language.RoleSetting + ' - <span class="bold">' + userInfo + '</span>',
    //        width: 350, height: 200,
    //        content: '<div id="chkUserRole"></div>',
    //        success: function () {
    //            $.get(url.GetUserRoles, {
    //                userId: data.Id
    //            }, function (res) {
    //                chkList = new Input($('#chkUserRole'), {
    //                    field: 'UserRole',
    //                    type: 'checkbox',
    //                    code: res.roles,
    //                    value: res.value
    //                });
    //            });
    //        },
    //        buttons: [{
    //            text: Language.OK, handler: function (e) {
    //                var data = chkList.getData();
    //                callback && callback(data);
    //                e.close();
    //            }
    //        }]
    //    });
    //}

    function _enableUsers(e, enable) {
        var msg = enable === 1 ? Language.Enable : Language.Disable;
        e.selectRows(function (e) {
            var message = Utils.format(Language.ConfirmOperateUser, msg);
            Layer.confirm(message, function () {
                Ajax.post(url.EnableUsers, {
                    data: JSON.stringify(e.ids), enable: enable
                }, function () {
                    e.grid.reload();
                });
            });
        });
    }
}

$.extend(Page, {
    SysUser: { component: new SysUser(false) },
    SysOrgUser: { component: new SysUser(true) }
});