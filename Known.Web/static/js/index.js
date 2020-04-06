layui.define(['layer', 'element'], function (exports) {
    var layer = layui.layer,
        element = layui.element,
        $ = layui.jquery;

    var url = {
        GetMenus: '/Home/GetMenus?pid=',
        UserInfo: '/Home/UserInfo'
    };

    var topRightAction = {
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
            addTab({
                id: 'userInfo',
                text: $(this).text(),
                icon: '',
                url: url.UserInfo
            });
        },
        logout: function () {
            layer.confirm('确定要退出系统？', function (index) {
                layer.close(index);
            }); 
        }
    };

    function fullScreen() {
        var el = document.documentElement;
        var rfs = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullScreen;

        //typeof rfs != "undefined" && rfs
        if (rfs) {
            rfs.call(el);
        } else if (typeof window.ActiveXObject !== "undefined") {
            //for IE，这里其实就是模拟了按下键盘的F11，使浏览器全屏
            var wscript = new ActiveXObject("WScript.Shell");
            if (wscript !== null) {
                wscript.SendKeys("{F11}");
            }
        }
    }

    function exitScreen() {
        var el = document;
        var cfs = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullScreen;

        //typeof cfs != "undefined" && cfs
        if (cfs) {
            cfs.call(el);
        } else if (typeof window.ActiveXObject !== "undefined") {
            //for IE，这里和fullScreen相同，模拟按下F11键退出全屏
            var wscript = new ActiveXObject("WScript.Shell");
            if (wscript !== null) {
                wscript.SendKeys("{F11}");
            }
        }
    }

    function toTree(arr, rootId) {
        arr.forEach(function (element) {
            var parentId = element.pid;
            if (parentId) {
                arr.forEach(function (ele) {
                    if (ele.id === parentId) {
                        if (!ele.children) {
                            ele.children = [];
                        }
                        ele.children.push(element);
                    }
                });
            }
        });
        arr = arr.filter(function (ele) {
            return ele.pid === rootId;
        });
        return arr;
    }

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
        var tree = toTree(res, pid);
        var html = '';
        $(tree).each(function (i, d) {
            html += '<li class="layui-nav-item">';
            html += '  <a href="javascript:;" id="menu' + d.id + '" data-url="' + d.url + '"><i class="layui-icon ' + d.icon + '"></i> ' + d.text + '</a>';
            if (d.children) {
                html += '  <dl class="layui-nav-child">';
                $(d.children).each(function (ci, cd) {
                    html += '<dd><a href="javascript:;" id="menu' + cd.id + '" data-url="' + cd.url + '"><i class="layui-icon ' + cd.icon + '"></i> ' + cd.text + '</a></dd>';
                });
                html += '  </dl>';
            }
            html += '</li>';
        });
        $(document).find(".layui-nav[lay-filter=" + obj + "]").html(html);
    }

    function addTab(node) {
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
            addTab({
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

    exports('index', {});
});