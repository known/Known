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

    function fullScreen () {
        var el = document.documentElement;
        var rfs = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullScreen;
        if (rfs) {
            rfs.call(el);
        } else if (typeof window.ActiveXObject !== 'undefined') {
            //for IE，这里其实就是模拟了按下键盘的F11，使浏览器全屏
            var wscript = new ActiveXObject('WScript.Shell');
            if (wscript !== null) {
                wscript.SendKeys('{F11}');
            }
        }
    }

    function exitScreen () {
        var el = document;
        var cfs = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullScreen;
        if (cfs) {
            cfs.call(el);
        } else if (typeof window.ActiveXObject !== 'undefined') {
            //for IE，这里和fullScreen相同，模拟按下F11键退出全屏
            var wscript = new ActiveXObject('WScript.Shell');
            if (wscript !== null) {
                wscript.SendKeys('{F11}');
            }
        }
    }

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

    function initTopRight() {
        $('.top-right').on('click', function () {
            var othis = $(this), type = othis.data('type');
            topRight[type] ? topRight[type].call(this, othis) : '';
        });
    }

    var topRight = {
        fullScreen: function () {
            fullScreen();
            $(this).data('type', 'exitScreen')
                .html('<i class="layui-icon layui-icon-screen-restore"></i>');
        },
        exitScreen: function () {
            exitScreen();
            $(this).data('type', 'fullScreen')
                .html('<i class="layui-icon layui-icon-screen-full"></i>');
        },
        userInfo: function () {
            admin.addTab({
                id: 'userInfo',
                text: $(this).text(),
                icon: $(this).find('i')[0].outerHTML,
                url: admin.option.url.UserInfo
            });
        },
        logout: function () {
            helper.confirm('确定要退出系统？', function () {
                $.post(admin.option.url.SignOut, function (result) {
                    layer.msg(result.message);
                    window.location = '/login';
                });
            });
        }
    };

    var admin = {
        option: null,
        init: function (option) {
            this.option = option;
            menuData = helper.list2Tree(option.data.menus, '');
            var data = renderMenu('topMenu');
            renderMenu('leftMenu', data[0].id);
            initTopRight();
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

    exports('admin', admin);
});