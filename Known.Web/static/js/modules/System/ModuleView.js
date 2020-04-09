layui.define('index', function (exports) {
    var url = {
        QueryModules: '/System/QueryModules',
        SaveUserInfo: '/System/SaveUserInfo',
        UpdatePassword: '/System/UpdatePassword'
    };

    var tree = layui.tree,
        table = layui.table,
        layer = layui.layer,
        util = layui.util;

    //模拟数据
    var data = [{
        title: '一级1'
        , id: 1
        , field: 'name1'
        , checked: true
        , spread: true
        , children: [{
            title: '二级1-1 可允许跳转'
            , id: 3
            , field: 'name11'
            , href: 'https://www.layui.com/'
            , children: [{
                title: '三级1-1-3'
                , id: 23
                , field: ''
                , children: [{
                    title: '四级1-1-3-1'
                    , id: 24
                    , field: ''
                    , children: [{
                        title: '五级1-1-3-1-1'
                        , id: 30
                        , field: ''
                    }, {
                        title: '五级1-1-3-1-2'
                        , id: 31
                        , field: ''
                    }]
                }]
            }, {
                title: '三级1-1-1'
                , id: 7
                , field: ''
                , children: [{
                    title: '四级1-1-1-1 可允许跳转'
                    , id: 15
                    , field: ''
                    , href: 'https://www.layui.com/doc/'
                }]
            }, {
                title: '三级1-1-2'
                , id: 8
                , field: ''
                , children: [{
                    title: '四级1-1-2-1'
                    , id: 32
                    , field: ''
                }]
            }]
        }, {
            title: '二级1-2'
            , id: 4
            , spread: true
            , children: [{
                title: '三级1-2-1'
                , id: 9
                , field: ''
                , disabled: true
            }, {
                title: '三级1-2-2'
                , id: 10
                , field: ''
            }]
        }, {
            title: '二级1-3'
            , id: 20
            , field: ''
            , children: [{
                title: '三级1-3-1'
                , id: 21
                , field: ''
            }, {
                title: '三级1-3-2'
                , id: 22
                , field: ''
            }]
        }]
    }, {
        title: '一级2'
        , id: 2
        , field: ''
        , spread: true
        , children: [{
            title: '二级2-1'
            , id: 5
            , field: ''
            , spread: true
            , children: [{
                title: '三级2-1-1'
                , id: 11
                , field: ''
            }, {
                title: '三级2-1-2'
                , id: 12
                , field: ''
            }]
        }, {
            title: '二级2-2'
            , id: 6
            , field: ''
            , children: [{
                title: '三级2-2-1'
                , id: 13
                , field: ''
            }, {
                title: '三级2-2-2'
                , id: 14
                , field: ''
                , disabled: true
            }]
        }]
    }, {
        title: '一级3'
        , id: 16
        , field: ''
        , children: [{
            title: '二级3-1'
            , id: 17
            , field: ''
            , fixed: true
            , children: [{
                title: '三级3-1-1'
                , id: 18
                , field: ''
            }, {
                title: '三级3-1-2'
                , id: 19
                , field: ''
            }]
        }, {
            title: '二级3-2'
            , id: 27
            , field: ''
            , children: [{
                title: '三级3-2-1'
                , id: 28
                , field: ''
            }, {
                title: '三级3-2-2'
                , id: 29
                , field: ''
            }]
        }]
    }];

    //按钮事件
    util.event('lay-demo', {
        getChecked: function (othis) {
            var checkedData = tree.getChecked('demoId1'); //获取选中节点的数据
            layer.alert(JSON.stringify(checkedData), { shade: 0 });
            console.log(checkedData);
        }
        , setChecked: function () {
            tree.setChecked('demoId1', [12, 16]); //勾选指定节点
        }
        , reload: function () {
            //重载实例
            tree.reload('demoId1', {
            });
        }
    });

    //常规用法
    tree.render({
        elem: '#tree', //默认是点击节点可进行收缩
        data: data
    });

    table.render({
        elem: '#gridModule',
        url: url.QueryModules,
        page: true,
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
        }]],
        skin: 'line'
    });

    exports('/System/ModuleView', {});
});