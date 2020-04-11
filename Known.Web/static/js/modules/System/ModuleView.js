layui.define(['index', 'helper'], function (exports) {
    var url = {
        GetModuleTree: '/System/GetModuleTree',
        QueryModules: '/System/QueryModules',
        SaveUserInfo: '/System/SaveUserInfo',
        UpdatePassword: '/System/UpdatePassword'
    };

    var $ = layui.jquery,
        tree = layui.tree,
        table = layui.table,
        layer = layui.layer,
        util = layui.util,
        helper = layui.helper;

    //tree
    $.get(url.GetModuleTree, function (result) {
        var data = helper.toTree(result, '');
        console.log(data);
        tree.render({ elem: '#tree', data: data });
    });

    //grid
    table.render({
        elem: '#gridModule',
        url: url.QueryModules,
        page: true, height: 'full-100',
        toolbar: '#tbModule',
        cols: [[{
            type: 'numbers', fixed: 'left'
        }, {
            type: 'checkbox', fixed: 'left'
        }, {
            sort: true, title: '编码', field: 'Code', width: 100
        }, {
            sort: true, title: '名称', field: 'Name', width: 100
        }, {
            sort: true, title: '图标', field: 'Icon', width: 100
        }, {
            sort: true, title: 'URL', field: 'Url', width: 100
        }, {
            sort: true, title: '状态', field: 'Enabled', width: 100
        }, {
            sort: true, title: '顺序', field: 'Order', width: 100, align: 'center'
        }, {
            fixed: 'right', title: '操作', toolbar: '#tbgModule', width: 150
        }]],
        skin: 'line'
    });

    //toolbar
    util.event('lay-demo', {
        getChecked: function (othis) {
            var checkedData = tree.getChecked('demoId1'); //获取选中节点的数据
            layer.alert(JSON.stringify(checkedData), { shade: 0 });
            console.log(checkedData);
        },
        setChecked: function () {
            tree.setChecked('demoId1', [12, 16]); //勾选指定节点
        },
        reload: function () {
            //重载实例
            tree.reload('demoId1', {
            });
        }
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