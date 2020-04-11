layui.define(['index', 'helper'], function (exports) {
    var url = {
        GetModuleTree: '/System/GetModuleTree',
        SaveUserInfo: '/System/SaveUserInfo',
        UpdatePassword: '/System/UpdatePassword'
    };

    var $ = layui.jquery,
        tree = layui.tree,
        table = layui.table,
        layer = layui.layer,
        helper = layui.helper;

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
                fixed: 'right', title: '操作', toolbar: '#tbgModule', width: 120, align: 'center'
            }]],
            skin: 'line'
        });
    }

    renderTable([]);

    //tree
    $.get(url.GetModuleTree, function (result) {
        var data = helper.toTree(result, '');
        tree.render({
            elem: '#tree', data: data,
            click: function (obj) {
                //var node = obj.data.module;
                var gridData = [];
                if (obj.data.children) {
                    obj.data.children.forEach(function (d) {
                        gridData.push(d.module);
                    });
                }
                renderTable(gridData);
            }
        });
    });

    //头工具栏事件
    table.on('toolbar(tbModule)', function (obj) {
        var checkStatus = table.checkStatus(obj.config.id);
        switch (obj.event) {
            case 'addSys':
                var data = checkStatus.data;
                layer.alert(JSON.stringify(data));
                break;
            case 'add':
                var data1 = checkStatus.data;
                layer.msg('选中了：' + data1.length + ' 个');
                break;
            case 'remove':
                layer.msg(checkStatus.isAll ? '全选' : '未全选');
                break;
        };
    });

    //监听行工具事件
    table.on('tool(tbModule)', function (obj) {
        var data = obj.data;
        if (obj.event === 'del') {
            layer.confirm('真的删除行么', function (index) {
                obj.del();
                layer.close(index);
            });
        } else if (obj.event === 'edit') {
            layer.prompt({
                formType: 2, value: data.email
            }, function (value, index) {
                obj.update({
                    email: value
                });
                layer.close(index);
            });
        }
    });

    exports('/System/ModuleView', {});
});