layui.define(['index', 'helper'], function (exports) {
    var url = {
        GetModuleTree: '/System/GetModuleTree',
        SaveModule: '/System/SaveModule',
        DeleteModules: '/System/DeleteModules'
    };

    var $ = layui.jquery,
        tree = layui.tree,
        table = layui.table,
        form = layui.form,
        layer = layui.layer,
        helper = layui.helper;

    var node = null;
    function renderTree() {
        $.get(url.GetModuleTree, function (result) {
            var data = helper.toTree(result, '');
            renderTable(getGridData(data));
            tree.render({
                elem: '#tree', data: data,
                click: function (obj) {
                    node = obj.data.module;
                    var gridData = getGridData(obj.data.children);
                    renderTable(gridData);
                }
            });
        });
    }

    function renderTable(data) {
        table.render({
            elem: '#gridModule',
            data: data,
            page: true, height: 'full-25',
            toolbar: '#tbModule',
            cols: [[{
                type: 'numbers', fixed: 'left'
            }, {
                type: 'checkbox', fixed: 'left'
            }, {
                sort: true, title: '编码', field: 'Code', width: 100
            }, {
                sort: true, title: '名称', field: 'Name', width: 150, templet: function (d) {
                    return '<i class="layui-icon ' + d.Icon + '"></i>' + d.Name;
                }
            }, {
                sort: true, title: 'URL', field: 'Url', width: 250
            }, {
                sort: true, title: '状态', field: 'Enabled', width: 100, templet: function (d) {
                    var checked = d.Enabled ? ' checked' : '';
                    return '<input type="checkbox" lay-skin="switch" lay-text="启用|禁用" disabled' + checked + '>';
                }
            }, {
                sort: true, title: '顺序', field: 'Sort', width: 100, align: 'center'
            }, {
                fixed: 'right', title: '操作', toolbar: '#tbrModule', width: 120, align: 'center'
            }]],
            skin: 'line'
        });
    }

    function getGridData(treeData) {
        var data = [];
        if (treeData) {
            treeData.forEach(function (d) {
                data.push(d.module);
            });
        }
        return data;
    }

    function showForm(data) {
        var formId = 'formModule';
        layer.open({
            type: 1, title: '模块管理' + (data.Id === '' ? '【新增】' : '【编辑】'),
            area: ['550px', '290px'],
            shade: 0,
            content: $('#dialogModule').html(),
            btn: ['保存', '关闭'],
            yes: function () {
                var field = form.val(formId);
                $.post(url.SaveModule, {
                    data: JSON.stringify(field)
                }, function (result) {
                    layer.msg(result.message);
                    if (result.ok) {
                        renderTree();
                    }
                });
            },
            btn2: function () {
                layer.close(layer.index);
            },
            success: function (layero, index) {
                form.render(null, formId);
                form.val(formId, data);
            }
        });
    }

    function deleteDatas(rows, callback) {
        if (!rows || rows.length === 0) {
            layer.msg('请至少选择一条记录！');
            return;
        }

        var ids = [];
        rows.forEach(function (d) {
            ids.push(d.Id);
        });

        var msg = rows.length > 1 ? ('所选的' + rows.length + '条记录') : '该记录';
        layer.confirm('确定要删除' + msg + '吗？', function (index) {
            layer.close(index);
            $.post(url.DeleteModules, {
                data: JSON.stringify(ids)
            }, function (result) {
                layer.msg(result.message);
                if (result.ok) {
                    callback && callback();
                }
            });
        });
    }

    //tree
    renderTree();

    //toolbar
    table.on('toolbar(gridModule)', function (obj) {
        switch (obj.event) {
            case 'addSys':
                showForm({ Id: '', ParentId: '' });
                break;
            case 'add':
                if (!node) {
                    layer.msg('请选择上级模块！');
                    return;
                }
                showForm({ Id: '', ParentId: node.Id });
                break;
            case 'remove':
                var data = table.checkStatus('gridModule').data;
                deleteDatas(data, function () {
                    renderTree();
                });
                break;
        };
    });

    //grid
    table.on('tool(gridModule)', function (obj) {
        var data = obj.data;
        if (obj.event === 'del') {
            deleteDatas([data], function () {
                obj.del();
            });
        } else if (obj.event === 'edit') {
            showForm(data);
        }
    });

    exports('/System/ModuleView', {});
});