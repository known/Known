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

function SysModuleTree(option) {
    var url = {
        GetSystems: baseUrl + '/System/GetSystems',
        GetModuleTree: baseUrl + '/System/GetModuleTree'
    };
    var select, isLoad = false, selectedApp, selectedNode;
    var tree = new Tree('tree', {
        url: url.GetModuleTree, autoLoad: false,
        dataFilter: function (data) {
            if (option.hideButton) {
                return data.filter(function (d) { return d.type !== 'button'; });
            }
            return data;
        },
        onLoad: function (data) {
            option.onLoad && option.onLoad(data);
        },
        onClick: function (node) {
            selectedNode = node;
            option.onClick && option.onClick(node);
        }
    });

    this.render = function () {
        var elem = $('<div>');
        var input = $('<div>').css({ padding: '5px' }).appendTo(elem);
        select = new Input(input, {
            field: 'App', type: 'select', emptyText: '全部',
            change: function (e) {
                _setTreeData(e.selected);
            }
        });
        tree.render().appendTo(elem);
        return elem;
    }

    this.getList = function () {
        return tree.list;
    }

    this.getSelectedApp = function () {
        return selectedApp;
    }

    this.getSelectedNode = function () {
        return selectedNode;
    }

    this.reload = function () {
        if (!isLoad) {
            select.setUrl(url.GetSystems, function (d) {
                _setTreeData(d[0]);
            });
            isLoad = true;
        } else {
            tree.reload({ appId: select.getValue() });
        }
    }

    function _setTreeData(module) {
        selectedApp = module;
        selectedNode = { module: module };
        tree.reload({ appId: module.Code });
    }
}

function SysModule() {
    //fields
    var url = {
        DeleteModels: baseUrl + '/System/DeleteModules',
        MoveModel: baseUrl + '/System/MoveModule',
        SaveModel: baseUrl + '/System/SaveModule',
        CopyModules: baseUrl + '/System/CopyModules',
        MoveModules: baseUrl + '/System/MoveModules',
        AddModules: baseUrl + '/System/AddModules',
        EnableModules: baseUrl + '/System/EnableModules'
    };
    var ModTypes = [
        { Code: 'menu', Name: '菜单' },
        { Code: 'page', Name: '页面' },
        { Code: 'button', Name: '按钮' }
    ];
    var ModTargets = [
        { Code: 'tab', Name: 'Tab' },
        { Code: '_blank', Name: 'Blank' },
        { Code: 'toolbar', Name: '工具条' },
        { Code: 'grid', Name: '表格内' },
        { Code: 'batch', Name: '批量操作' }
    ];
    var colIcon = new IconColumn();
    var tree = new SysModuleTree({
        onLoad: function (data) {
            var nodes = data || [];
            var datas = nodes.map(function (d) { return d.module; });
            view.setGridData(datas);
        },
        onClick: function (node) {
            var nodes = node.children || [];
            var datas = nodes.map(function (d) { return d.module; });
            view.setGridData(datas);
        }
    });
    var view = new View('Module', {
        url: url,
        left: tree,
        columns: [
            { field: 'Id', type: 'hidden' },
            { field: 'AppId', type: 'hidden' },
            { field: 'ParentId', type: 'hidden' },
            { title: '编码', field: 'Code', width: '80px', type: 'text', required: true },
            {
                title: '名称', field: 'Name', width: '120px', type: 'text', required: true, format: function (d) {
                    return '<i class="' + d.Icon + '"></i> ' + d.Name;
                }
            },
            colIcon.field,
            { title: '类型', field: 'Type', width: '80px', type: 'radio', code: ModTypes, required: true, align: 'center' },
            { title: '目标', field: 'Target', type: 'checkbox', code: ModTargets },
            { title: 'URL', field: 'Url', type: 'text', inputBlock: true },
            { title: '状态', field: 'Enabled', width: '80px', type: 'checkbox', code: 'Enabled', required: true, align: 'center' },
            { title: '顺序', field: 'Sort', width: '50px', type: 'text', required: true, align: 'center' },
            { onlyForm: true, title: '备注', field: 'Note', type: 'textarea', lineBlock: true }
        ],
        refresh: function () {
            tree.reload();
        },
        formOption: {
            width: 980, height: 350,
            titleInfo: function (d) {
                return d.ParentName ? '- ' + d.ParentName : '';
            },
            setData: function (e) {
                colIcon.setIcon(e.data.Icon);
            }
        },
        gridOption: {
            url: '', page: false, isTradition: true,
            toolbar: {
                addMod: function (e) {
                    var node = tree.getSelectedNode();
                    if (!node) {
                        Layer.tips('请选择上级模块！');
                        return;
                    }
                    var currApp = tree.getSelectedApp();
                    var module = node.module;
                    var children = node.children || [];
                    e.addRow({
                        Id: '', AppId: currApp.Code, ParentId: module.Id, ParentName: module.Name, Enabled: 1,
                        Icon: 'fa fa-file-o', Type: 'page', Target: 'tab', Sort: children.length + 1
                    });
                },
                addBtn: function (e) {
                    var node = tree.getSelectedNode();
                    if (!node) {
                        Layer.tips('请选择模块！');
                        return;
                    }
                    addButtons(node.module, function (data) {
                        Ajax.post(url.AddModules, {
                            data: JSON.stringify(data)
                        }, function () {
                            tree.reload();
                        });
                    });
                },
                copy: function (e) {
                    e.selectRows(function (e) {
                        selectToModule('复制到', function (data) {
                            Ajax.post(url.CopyModules, {
                                data: JSON.stringify(e.ids), mid: data.Id
                            }, function () {
                                tree.reload();
                            });
                        });
                    });
                },
                move: function (e) {
                    e.selectRows(function (e) {
                        selectToModule('移动到', function (data) {
                            Ajax.post(url.MoveModules, {
                                data: JSON.stringify(e.ids), mid: data.Id
                            }, function () {
                                tree.reload();
                            });
                        });
                    });
                },
                enable: function (e) { enableModules(e, 1); },
                disable: function (e) { enableModules(e, 0); },
                sql: function (e) {
                    selectModule(function (list) {
                        var data = list.map(function (d) { return d.id; });
                        Ajax.download(url.ExportModules, {
                            data: JSON.stringify(data)
                        });
                    });
                }
            }
        }
    });

    //methods
    this.render = function (dom) {
        view.render().appendTo(dom);
    }

    this.mounted = function () {
        view.load();
    }

    //private
    function addButtons(node, callback) {
        var cbList;
        Layer.open({
            title: '添加按钮 - ' + node.Name,
            width: 400, height: 250,
            content: '<div id="listButton" style="padding:10px;">',
            success: function () {
                var btns = [
                    { Code: 'add', Name: '新增', Data: { Icon: 'fa fa-plus', Target: 'toolbar', Sort: 1 } },
                    { Code: 'copy', Name: '复制', Data: { Icon: 'fa fa-copy', Target: 'grid', Sort: 2 } },
                    { Code: 'edit', Name: '编辑', Data: { Icon: 'fa fa-edit', Target: 'grid', Sort: 3 } },
                    { Code: 'remove', Name: '删除', Data: { Icon: 'fa fa-trash-o', Target: 'grid,batch', Sort: 4 } },
                    { Code: 'import', Name: '导入', Data: { Icon: 'fa fa-sign-in', Target: 'toolbar', Sort: 5 } },
                    { Code: 'export', Name: '导出', Data: { Icon: 'fa fa-sign-out', Target: 'toolbar', Sort: 6 } },
                    { Code: '-' },
                    { Code: 'print', Name: '打印', Data: { Icon: 'fa fa-print', Target: 'grid', Sort: 7 } },
                    { Code: 'up', Name: '上移', Data: { Icon: 'fa fa-arrow-up', Target: 'grid', Sort: 8 } },
                    { Code: 'down', Name: '下移', Data: { Icon: 'fa fa-arrow-down', Target: 'grid', Sort: 9 } },
                    { Code: 'upload', Name: '上载', Data: { Icon: 'fa fa-upload', Target: 'toolbar', Sort: 10 } },
                    { Code: 'download', Name: '下载', Data: { Icon: 'fa fa-download', Target: 'toolbar', Sort: 11 } },
                    { Code: '-' },
                    { Code: 'lock', Name: '锁定', Data: { Icon: 'fa fa-lock', Target: 'grid,batch', Sort: 12 } },
                    { Code: 'unlock', Name: '解锁', Data: { Icon: 'fa fa-unlock', Target: 'grid,batch', Sort: 13 } },
                    { Code: 'enable', Name: '启用', Data: { Icon: 'fa fa-check-circle-o', Target: 'grid,batch', Sort: 14 } },
                    { Code: 'disable', Name: '禁用', Data: { Icon: 'fa fa-minus-circle', Target: 'grid,batch', Sort: 15 } },
                    { Code: '-' },
                    { Code: 'submit', Name: '提交', Data: { Icon: 'fa fa-send-o', Target: 'grid,batch', Sort: 16 } },
                    { Code: 'revoke', Name: '撤回', Data: { Icon: 'fa fa-reply', Target: 'grid,batch', Sort: 17 } },
                    { Code: 'pass', Name: '复核通过', Data: { Icon: 'fa fa-check', Target: 'grid,batch', Sort: 18 } },
                    { Code: 'fail', Name: '复核退回', Data: { Icon: 'fa fa-close', Target: 'grid,batch', Sort: 19 } },
                    { Code: 'close', Name: '关闭', Data: { Icon: 'fa fa-times-circle-o', Target: 'grid,batch', Sort: 20 } }
                ];
                cbList = new Input($('#listButton'), { type: 'checkbox', code: btns });
            },
            buttons: [{
                text: '确定', handler: function (e) {
                    var checks = cbList.getData();
                    if (!checks.length) {
                        Layer.tips('请至少选择一个按钮！');
                        return;
                    }
                    var currApp = tree.getSelectedApp();
                    var data = checks.map(function (d) {
                        var item = d.Data;
                        item.AppId = currApp.Code;
                        item.ParentId = node.Id;
                        item.Code = d.Code;
                        item.Name = d.Name;
                        item.Type = 'button';
                        item.Enabled = 1;
                        return item;
                    });
                    callback && callback(data);
                    e.close();
                }
            }]
        });
    }

    function selectToModule(title, callback) {
        var copyNode;
        Layer.open({
            title: title,
            width: 350, height: 250,
            content: '<ul id="treeModule">',
            success: function () {
                var list = tree.getList();
                new Tree('treeModule', {
                    data: list.filter(function (d) { return d.module.Type !== 'button'; }),
                    onClick: function (node) {
                        copyNode = node.module;
                    }
                });
            },
            buttons: [{
                text: '确定', handler: function (e) {
                    e.close();
                    callback && callback(copyNode);
                }
            }]
        });
    }

    function selectModule(callback) {
        var treeObj;
        Layer.open({
            title: '导出SQL', width: 450, height: 300,
            content: '<ul id="treeSql">',
            success: function () {
                treeObj = new Tree('treeSql', {
                    check: true,
                    data: tree.getList(),
                    onClick: function (node) {
                        copyNode = node.module;
                    }
                });
            },
            buttons: [{
                text: '确定', handler: function (e) {
                    e.close();
                    var data = treeObj.getCheckedNodes();
                    callback && callback(data);
                }
            }]
        });
    }

    function enableModules(e, enable) {
        var msg = enable === 1 ? '启用' : '禁用';
        e.selectRows(function (e) {
            Layer.confirm('确定要' + msg + '所选模块？', function () {
                Ajax.post(url.EnableModules, {
                    data: JSON.stringify(e.ids), enable: enable
                }, function () {
                    tree.reload();
                });
            });
        });
    }
}

$.extend(Page, {
    SysModule: { component: new SysModule() }
});