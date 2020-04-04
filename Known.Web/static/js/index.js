layui.use(['layer', 'element'], function () {
    var layer = layui.layer
        , element = layui.element
        , $ = layui.jquery;

    var active = {
        tabAdd: function (node) {
            element.tabAdd('tabMenu', {
                title: '新选项' + (Math.random() * 1000 | 0) //用于演示
                , content: '内容' + (Math.random() * 1000 | 0)
                , id: new Date().getTime() //实际使用一般是规定好的id，这里以时间戳模拟下
            })
        }
        , tabDelete: function (othis) {
            element.tabDelete('tabMenu', '44'); //删除：“商品管理”
            othis.addClass('layui-btn-disabled');
        }
        , tabChange: function () {
            element.tabChange('tabMenu', '22');
        }
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

    function renderMenu(obj, pid) {
        pid = pid || '';
        $.ajax({
            url: '/Home/GetMenus?pid=' + pid, async: false,
            success: function (res) {
                //console.log(res);
                var tree = toTree(res, pid);
                //console.log(tree);
                var html = '';
                $(tree).each(function (i, d) {
                    html += '<li class="layui-nav-item">';
                    html += '  <a href="javascript:;">' + d.text + '</a>';
                    if (d.children) {
                        html += '  <dl class="layui-nav-child">';
                        $(d.children).each(function (ci, cd) {
                            html += '<dd><a href="">' + cd.text + '</a></dd>';
                        });
                        html += '  </dl>';
                    }
                    html += '</li>';
                });
                $(document).find(".layui-nav[lay-filter=" + obj + "]").html(html);
                element.init();
            }
        });
    }

    function init() {
        renderMenu('topMenu');
    }

    //$('.site-demo-active').on('click', function () {
    //    var othis = $(this), type = othis.data('type');
    //    active[type] ? active[type].call(this, othis) : '';
    //});

    element.on('nav(topMenu)', function (elem) {
        console.log(elem);
    });

    element.on('nav(leftMenu)', function (elem) {
        console.log(elem);
    });

    element.on('tab(tabMenu)', function (elem) {
        console.log(elem);
        //location.hash = 'test=' + $(this).attr('lay-id');
    });

    //Hash地址的定位
    //var layid = location.hash.replace(/^#test=/, '');
    //element.tabChange('tabMenu', layid);

    init();
});


