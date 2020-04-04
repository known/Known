layui.use(['layer', 'element'], function () {
    var layer = layui.layer
        , element = layui.element
        , $ = layui.jquery;

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
            url: '/Home/GetMenus?pid=' + pid, async: false,
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

    function init() {
        initMenu('topMenu', '');
    }

    //$('.site-demo-active').on('click', function () {
    //    var othis = $(this), type = othis.data('type');
    //    active[type] ? active[type].call(this, othis) : '';
    //});

    element.on('nav(topMenu)', function (elem) {
        var pid = elem[0].id.replace('menu', '');
        initMenu('leftMenu', pid);
    });

    element.on('nav(leftMenu)', function (elem) {
        var src = elem.data('url');
        if (src) {
            var a = elem[0];
            var id = a.id.replace('menu', '');
            var title = elem.find('i')[0].outerHTML + ' ' + a.text;
            var content = '<iframe src="' + src + '" frameborder="0" style="width:100%;height:100%;"></iframe>';
            element.tabAdd('tabMenu', { id: id, title: title, content: content });
            element.tabChange('tabMenu', id);
        }
    });

    element.on('tab(tabMenu)', function (elem) {
        //console.log(elem);
        //location.hash = 'test=' + $(this).attr('lay-id');
    });

    //Hash地址的定位
    //var layid = location.hash.replace(/^#test=/, '');
    //element.tabChange('tabMenu', layid);

    init();
});


