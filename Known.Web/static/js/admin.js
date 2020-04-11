layui.extend({
    helper: 'helper'
}).define('helper', function (exports) {
    var $ = layui.jquery,
        layer = layui.layer,
        element = layui.element,
        helper = layui.helper;

    var url = {
        GetMenus: '/Home/GetMenus?pid=',
        UserInfo: '/Home/UserInfo',
        SignOut: '/signout'
    };

    var admin = {
        addTab: function (node) {
            if (!node.url)
                return;

            var id = node.id;
            var tab = $('.layui-tab-title li[lay-id="' + id + '"]');
            if (!tab.length) {
                var title = node.icon + ' ' + node.text;
                var content = '<iframe src="' + node.url + '" frameborder="0" class="layui-tab-iframe"></iframe>';
                element.tabAdd('tabMenu', { id: id, title: title, content: content });
            }
            element.tabChange('tabMenu', id);
        }
    };

    var topRightAction = {
        fullScreen: function () {
            helper.fullScreen();
            $(this).data('type', 'exitScreen')
                .html('<i class="layui-icon layui-icon-screen-restore"></i>');
        },
        exitScreen: function () {
            helper.exitScreen();
            $(this).data('type', 'fullScreen')
                .html('<i class="layui-icon layui-icon-screen-full"></i>');
        },
        userInfo: function () {
            admin.addTab({
                id: 'userInfo',
                text: $(this).text(),
                icon: '',
                url: url.UserInfo
            });
        },
        logout: function () {
            layer.confirm('确定要退出系统？', function (index) {
                $.post(url.SignOut, function (result) {
                    layer.msg(result.message);
                    window.location = '/login';
                });
                layer.close(index);
            });
        }
    };

    function initMenu(obj, pid) {
        $.ajax({
            url: url.GetMenus + pid, async: false,
            success: function (res) {
                renderMenu(obj, res, pid);
                if (pid === '') {
                    initMenu('leftMenu', res[0].id);
                }
                element.init();
            }
        });
    }

    function renderMenu(obj, res, pid) {
        var tree = helper.toTree(res, pid);
        var html = '';
        $(tree).each(function (i, d) {
            html += '<li class="layui-nav-item">';
            html += '  <a href="javascript:;" id="menu' + d.id + '" data-url="' + d.url + '"><i class="layui-icon ' + d.icon + '"></i> ' + d.title + '</a>';
            if (d.children) {
                html += '  <dl class="layui-nav-child">';
                $(d.children).each(function (ci, cd) {
                    html += '<dd><a href="javascript:;" id="menu' + cd.id + '" data-url="' + cd.url + '"><i class="layui-icon ' + cd.icon + '"></i> ' + cd.title + '</a></dd>';
                });
                html += '  </dl>';
            }
            html += '</li>';
        });
        $(document).find(".layui-nav[lay-filter=" + obj + "]").html(html);
    }

    function init() {
        initMenu('topMenu', '');

        $('.top-right').on('click', function () {
            var othis = $(this), type = othis.data('type');
            topRightAction[type] ? topRightAction[type].call(this, othis) : '';
        });
    }

    element.on('nav(topMenu)', function (elem) {
        var pid = elem[0].id.replace('menu', '');
        initMenu('leftMenu', pid);
    });

    element.on('nav(leftMenu)', function (elem) {
        var src = elem.data('url');
        if (src) {
            var a = elem[0];
            admin.addTab({
                id: a.id.replace('menu', ''),
                text: a.text,
                icon: elem.find('i')[0].outerHTML,
                url: src
            });
        }
    });

    element.on('tab(tabMenu)', function (elem) {
        var id = $(this).attr('lay-id');
        $('.layui-nav-child dd').removeClass('layui-this');
        $('.layui-nav-child #menu' + id).parent().addClass('layui-this');
    });

    init();

    exports('admin', admin);
});