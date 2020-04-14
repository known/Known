layui.define(['index', 'helper'], function (exports) {
    var url = {
        GetModuleTree: '/System/GetModuleTree',
        DeleteModules: '/System/DeleteModules',
        SaveModule: '/System/SaveModule'
    };

    var $ = layui.jquery,
        tree = layui.tree,
        layer = layui.layer,
        helper = layui.helper;

    var f = helper.form({
        name: 'formModule',
        title: '模块管理',
        config: {
            area: ['550px', '290px'],
            content: $('#dialogModule').html(),
            setData: function (e) {
                $('#dvIcon').attr('icon', e.data.Icon)
                            .html('<i class="layui-icon ' + e.data.Icon + '"></i>');
            },
            init: function (e) {
                $('#dvIcon').click(function () {
                    var icon1 = $(this).attr('icon');
                    selectIcon(icon1, function (icon) {
                        var data = e.form.getData();
                        data.Icon = icon;
                        e.form.setData(data);
                    });
                });
            }
        },
        toolbar: [{
            text: '保存', handler: function (e) {
                var data = e.form.getData();
                $.post(url.SaveModule, {
                    data: JSON.stringify(data)
                }, function (result) {
                    layer.msg(result.message);
                    if (result.ok) {
                        data.Id = result.data;
                        e.form.setData(data);
                        renderTree();
                    }
                });
            }
        }]
    });

    var node = null;
    var g = helper.grid({
        name: 'gridModule',
        config: {
            page: true, height: 'full-25', toolbar: '#tbModule',
            initSort: { field: 'Sort', type: 'asc' },
            cols: [[
                { type: 'numbers', fixed: 'left' },
                { type: 'checkbox', fixed: 'left' },
                { sort: true, title: '编码', field: 'Code', width: 100 },
                {
                    sort: true, title: '名称', field: 'Name', width: 150, templet: function (d) {
                        return '<i class="layui-icon ' + d.Icon + '"></i>' + d.Name;
                    }
                },
                { sort: true, title: 'URL', field: 'Url', width: 250 },
                {
                    sort: true, title: '状态', field: 'Enabled', width: 100, templet: function (d) {
                        var checked = d.Enabled ? ' checked' : '';
                        return '<input type="checkbox" lay-skin="switch" lay-text="启用|禁用" disabled' + checked + '>';
                    }
                },
                { sort: true, title: '顺序', field: 'Sort', width: 100, align: 'center' },
                { fixed: 'right', title: '操作', toolbar: '#tbrModule', width: 120, align: 'center' }
            ]]
        },
        toolbar: {
            addSys: function (e) {
                f.open({ Id: '', ParentId: '', Icon: 'layui-icon-file', Enabled: 1 });
            },
            add: function (e) {
                if (!node) {
                    layer.msg('请选择上级模块！');
                    return;
                }
                f.open({ Id: '', ParentId: node.Id, Icon: 'layui-icon-file', Enabled: 1 });
            },
            remove: function (e) {
                deleteDatas(e.grid, e.rows, function () {
                    renderTree();
                });
            },
            edit: function (e) {
                f.open(e.row);
            },
            del: function (e) {
                deleteDatas(e.grid, [e.row], function () {
                    renderTree();
                });
            }
        }
    });

    function renderTree() {
        $.get(url.GetModuleTree, function (result) {
            var data = helper.toTree(result, '');
            g.setData(getGridData(data));
            tree.render({
                elem: '#tree', data: data, onlyIconControl: true,
                click: function (obj) {
                    node = obj.data.module;
                    var gridData = getGridData(obj.data.children);
                    g.setData(gridData);
                }
            });
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

    function selectIcon(icon, callback) {
        var data = [
            'heart-fill', 'heart', 'light', 'time', 'bluetooth', 'at', 'mute',
            'mike', 'key', 'gift', 'email', 'rss', 'wifi', 'logout',
            'android', 'ios', 'windows', 'transfer', 'service', 'subtraction', 'addition',
            'slider', 'print', 'export', 'cols', 'screen-restore', 'screen-full', 'rate-half',
            'rate', 'rate-solid', 'cellphone', 'vercode', 'login-wechat', 'login-qq', 'login-weibo',
            'password', 'username', 'refresh-3', 'auz', 'spread-left', 'shrink-right', 'snowflake',
            'tips', 'note', 'home', 'senior', 'refresh', 'refresh-1', 'flag',
            'theme', 'notice', 'website', 'console', 'face-surprised', 'set', 'template-1',
            'app', 'template', 'praise', 'tread', 'male', 'female', 'camera',
            'camera-fill', 'more', 'more-vertical', 'rmb', 'dollar', 'diamond', 'fire',
            'return', 'location', 'read', 'survey', 'face-smile', 'face-cry', 'cart-simple',
            'cart', 'next', 'prev', 'upload-drag', 'upload', 'download-circle', 'component',
            'file-b', 'user', 'find-fill', 'loading', 'loading-1', 'add-1', 'play',
            'pause', 'headset', 'video', 'voice', 'speaker', 'fonts-del', 'fonts-code',
            'fonts-html', 'fonts-strong', 'unlink', 'picture', 'link', 'face-smile-b', 'align-left',
            'align-right', 'align-center', 'fonts-u', 'fonts-i', 'tabs', 'radio', 'circle',
            'edit', 'share', 'delete', 'form', 'cellphone-fine', 'dialogue', 'fonts-clear',
            'layer', 'date', 'water', 'code-circle', 'carousel', 'prev-circle', 'layouts',
            'util', 'templeate-1', 'upload-circle', 'tree', 'table', 'chart', 'chart-screen',
            'engine', 'triangle-d', 'triangle-r', 'file', 'set-sm', 'reduce-circle', 'add-circle',
            '404', 'about', 'up', 'down', 'left', 'right', 'circle-dot',
            'search', 'set-fill', 'group', 'friends', 'reply-fill', 'menu-fill', 'log',
            'picture-fine', 'face-smile-fine', 'list', 'release', 'ok', 'help', 'chat',
            'top', 'star', 'star-fill', 'close-fill', 'close', 'ok-circle', 'add-circle-fine'
        ];
        var content = '<ul class="icon-list">';
        data.forEach(function (d) {
            var icon1 = 'layui-icon-' + d;
            content += '<li id="' + icon1 + '"';
            content += icon1 === icon ? ' class="active">' : '>';
            content += '<i class="layui-icon ' + icon1 + '"></i>';
            content += '</li>';
        });
        content += '</ul>';
        layer.open({
            type: 1, title: '选择图标',
            area: ['400px', '250px'],
            shade: 0,
            content: content,
            btn: ['确定', '关闭'],
            yes: function () {
                var icon = $('.icon-list li.active').attr('id');
                callback && callback(icon);
            },
            btn2: function () {
                layer.close(layer.index);
            },
            success: function (layero, index) {
                $('.icon-list li').click(function () {
                    $('.icon-list li').removeClass('active');
                    $(this).addClass('active');
                });
            }
        });
    }

    function deleteDatas(grid, rows, callback) {
        grid.deleteRows(rows, function (data) {
            $.post(url.DeleteModules, {
                data: data
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

    exports('/System/ModuleView', {});
});