layui.define('common', function (exports) {
    var $ = layui.$,
        layer = layui.layer,
        element = layui.element,
        common = layui.common;

    if (!/http(s*):\/\//.test(location.href)) {
        var tips = '请在Web服务器（Apache/Tomcat/Nginx/IIS/等）运行，否则部分数据将无法显示！';
        return layer.alert(tips);
    }

    function fullScreen() {
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

    function exitScreen() {
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

    function renderAnim(anim) {
        if (!anim) return;
        $('head').append('<style>\n' +
            '.layui-tab-item.layui-show{animation:moveTop 1s;-webkit-animation:moveTop 1s;animation-fill-mode:both;-webkit-animation-fill-mode:both;position:relative;height:100%;-webkit-overflow-scrolling:touch;}\n' +
            '@keyframes moveTop {\n' +
            '  0% {opacity:0;-webkit-transform:translateY(30px);-ms-transform:translateY(30px);transform:translateY(30px);}\n' +
            '  100% {opacity:1;-webkit-transform:translateY(0);-ms-transform:translateY(0);transform:translateY(0);}\n' +
            '}\n' +
            '@-o-keyframes moveTop {\n' +
            '  0% {opacity:0;-webkit-transform:translateY(30px);-ms-transform:translateY(30px);transform:translateY(30px);}\n' +
            '  100% {opacity:1;-webkit-transform:translateY(0);-ms-transform:translateY(0);transform:translateY(0);}\n' +
            '}\n' +
            '@-moz-keyframes moveTop {\n' +
            '  0% {opacity:0;-webkit-transform:translateY(30px);-ms-transform:translateY(30px);transform:translateY(30px);}\n' +
            '  100% {opacity:1;-webkit-transform:translateY(0);-ms-transform:translateY(0);transform:translateY(0);}\n' +
            '}\n' +
            '@-webkit-keyframes moveTop {\n' +
            '  0% {opacity:0;-webkit-transform:translateY(30px);-ms-transform:translateY(30px);transform:translateY(30px);}\n' +
            '  100% {opacity:1;-webkit-transform:translateY(0);-ms-transform:translateY(0);transform:translateY(0);}\n' +
            '}\n' +
            '</style>');
    }

    var Menu = {

        option: null,
        treeData: null,

        //public
        show: function (option) {
            this.option = option;
            this.treeData = common.list2Tree(option.data, '')
            var menu = this.render('topMenu');
            this.render('leftMenu', menu[0].id);
            this.initEvent();
        },

        //private
        render: function (obj, pid) {
            var html = '';
            var data = this.treeData;
            if (pid) {
                var parent = this.treeData.find(d => d.id === pid);
                data = parent.children;
            }
            $(data).each(function (i, d) {
                html += '<li class="layui-nav-item menuItem">';
                html += '  <a href="javascript:;" id="menu' + d.id + '" data-url="' + d.url + '"><i class="layui-icon ' + d.icon + '"></i><span class="title">' + d.title + '</span></a>';
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
        },

        miniSide: false,
        initEvent: function () {
            var _this = this;
            $('.toggleMenu').click(function () {
                var clsLeft = 'layui-icon-spread-left',
                    clsRight = 'layui-icon-shrink-right';
                if (!_this.miniSide) {
                    _this.miniSide = true;
                    $(this).removeClass(clsRight).addClass(clsLeft);
                    $('.layui-layout-admin').addClass('layui-mini');
                } else {
                    _this.miniSide = false;
                    $(this).removeClass(clsLeft).addClass(clsRight);
                    $('.layui-layout-admin').removeClass('layui-mini');
                }
                _this.initMenuTips(_this.miniSide);
            });

            element.on('nav(topMenu)', function (elem) {
                var pid = elem[0].id.replace('menu', '');
                _this.render('leftMenu', pid);
                _this.initMenuTips(_this.miniSide);
            });

            element.on('nav(leftMenu)', function (elem) {
                var src = elem.data('url');
                if (src) {
                    var a = elem[0];
                    Tab.addTab({
                        id: a.id.replace('menu', ''),
                        text: a.text,
                        icon: elem.find('i')[0].outerHTML,
                        url: src
                    });
                }
            });
        },

        menuTipId: '',
        initMenuTips: function (isMiniSide) {
            var item = $('.layui-side .menuItem').unbind('mouseenter');
            var pops = $('.popup-tips').unbind('mouseleave');
            if (this.menuTipId !== '') {
                layer.close(this.menuTipId);
            }
            if (!isMiniSide) return;

            var _this = this;
            item.bind('mouseenter', function () {
                var tip = tips = $(this).html();
                tip = '<ul class="layui-nav layui-nav-tree layui-this"><li class="layui-nav-item layui-nav-itemed">' + tip + '</li></ul>';
                _this.menuTipId = layer.tips(tip, $(this), {
                    tips: [2, '#2f4056'],
                    time: 300000,
                    skin: 'popup-tips',
                    success: function (el) {
                        var left = $(el).position().left - 159;
                        $(el).css({ left: left });
                        element.render('nav');
                    }
                });
            });
            pops.bind('mouseleave', function () {
                layer.close(_this.menuTipId);
            });
        }

    }

    var Tab = {

        option: null,

        //public
        show: function (option) {
            this.option = option;
            this.initEvent();
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
            var module = this.option.menus
                ? this.option.menus.find(m => m.id === id)
                : null;
            return { id: id, title: title, module: module };
        },

        //private
        initEvent: function () {
            element.on('tab(tabMenu)', function (elem) {
                var id = $(this).attr('lay-id');
                $('.layui-nav-child dd').removeClass('layui-this');
                $('.layui-nav-child #menu' + id).parent().addClass('layui-this');
            });
        }

    }

    var Toolbar = {

        option: null,

        //public
        show: function (option) {
            this.option = option;
            this.initEvent();
        },

        //action
        refresh: function () {
            var iframe = $('.layui-layout-admin .layui-tab-content .layui-show iframe');
            var url = iframe.attr('src');
            iframe.attr('src', url);
        },

        share: function () {

        },

        cache: function () {
            common.post(Toolbar.option.url.RefreshCache);
        },

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
            Tab.addTab({
                id: 'userInfo',
                text: $(this).text(),
                icon: $(this).find('i')[0].outerHTML,
                url: Toolbar.option.url.UserInfo
            });
        },

        logout: function () {
            common.confirm('确定要退出系统？', function () {
                $.post(Toolbar.option.url.SignOut, function (result) {
                    layer.msg(result.message);
                    window.location = '/login';
                });
            });
        },

        //private
        initEvent: function () {
            var _this = this;
            $('.top-right').on('click', function () {
                var othis = $(this), type = othis.data('type');
                _this[type] ? _this[type].call(this, othis) : '';
            });
        }

    }

    exports('admin', {

        show: function (option) {
            renderAnim(option.pageAnim || true);
            Toolbar.show({ url: option.url });
            $.get(option.url.GetUserMenus, function (result) {
                Menu.show({ data: result.menus });
                Tab.show({ menus: result.menus });
                option.callback && option.callback(result);
                $('.loader').fadeOut();
            });
        },

        addTab: function (node) {
            Tab.addTab(node);
        },

        getCurTab: function () {
            return Tab.getCurTab();
        }

    });
});