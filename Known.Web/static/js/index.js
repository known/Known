layui.use(['layer', 'element'], function () {
    var layer = layui.layer
        , element = layui.element
        , $ = layui.jquery;

    var url = {
        GetMenus: '/Home/GetMenus?pid='
    };

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

    element.on('nav(topMenu)', function (elem) {
        var pid = elem[0].id.replace('menu', '');
        initMenu('leftMenu', pid);
    });

    element.on('nav(leftMenu)', function (elem) {
        var src = elem.data('url');
        if (src) {
            var a = elem[0];
            var id = a.id.replace('menu', '');
            var tab = $('.layui-tab-title li[lay-id="' + id + '"]');
            if (!tab.length) {
                var title = elem.find('i')[0].outerHTML + ' ' + a.text;
                var content = '<iframe src="' + src + '" frameborder="0" style="width:100%;height:100%;"></iframe>';
                element.tabAdd('tabMenu', { id: id, title: title, content: content });
            }
            element.tabChange('tabMenu', id);
        }
    });

    element.on('tab(tabMenu)', function (elem) {
        var id = $(this).attr('lay-id');
        $('.layui-nav-child dd').removeClass('layui-this');
        $('.layui-nav-child #menu' + id).parent().addClass('layui-this');
    });

    initMenu('topMenu', '');
});


