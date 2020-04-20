layui.define('index', function (exports) {
    var $ = layui.jquery,
        element = layui.element,
        helper = layui.helper;

    var menuData = null;
    function renderMenu(obj, pid) {
        var html = '';
        var data = menuData;
        if (pid) {
            var parent = menuData.find(d => d.id === pid);
            data = parent.children;
        }
        $(data).each(function (i, d) {
            html += '<li class="layui-nav-item">';
            html += '  <a href="javascript:;" id="menu' + d.id + '" data-url="' + d.url + '"><i class="layui-icon ' + d.icon + '"></i> ' + d.title + '</a>';
            if (pid && d.children) {
                html += '  <dl class="layui-nav-child">';
                $(d.children).each(function (ci, cd) {
                    html += '<dd><a href="javascript:;" id="menu' + cd.id + '" data-url="' + cd.url + '"><i class="layui-icon ' + cd.icon + '"></i> ' + cd.title + '</a></dd>';
                });
                html += '  </dl>';
            }
            html += '</li>';
        });
        $(document).find(".layui-nav[lay-filter=" + obj + "]").html(html);
        element.init();
        return data;
    }

    var admin = {
        option: null,
        init: function (option) {
            this.option = option;
            menuData = helper.toTree(option.data.menus, '');
            var data = renderMenu('topMenu');
            renderMenu('leftMenu', data[0].id);
            option.callback && option.callback();
        },
        addTab: function (node) {
            if (!node.url)
                return;

            var id = node.id;
            var tab = $('.layui-tab-title li[lay-id="' + id + '"]');
            if (!tab.length) {
                var title = node.icon + ' <span>' + node.text + '</span>';
                var content = '<iframe src="' + node.url + '" frameborder="0" class="layui-tab-iframe"></iframe>';
                element.tabAdd('tabMenu', { id: id, title: title, content: content });
            }
            element.tabChange('tabMenu', id);
        },
        getCurTab: function () {
            var tab = $('.layui-tab-title .layui-this');
            var id = tab.attr('lay-id');
            var title = tab.children('span').text();
            var module = this.option.data.menus.find(m => m.id === id);
            return { id: id, title: title, module: module };
        }
    };

    element.on('nav(topMenu)', function (elem) {
        var pid = elem[0].id.replace('menu', '');
        renderMenu('leftMenu', pid);
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

    exports('admin', admin);
});