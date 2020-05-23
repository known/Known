layui.define('common', function (exports) {
    var $ = layui.$,
        layer = layui.layer,
        element = layui.element,
        common = layui.common;

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
            this.treeData = common.list2Tree(option.data, '');
            var menus = this.render('topMenu');
            this.render('leftMenu', menus[0].id);
            this.initEvent();
            var tm = $('.layui-nav[lay-filter=topMenu]').hide();
            if (menus.length > 1) {
                tm.show();
            }
            option.topChanged && option.topChanged(menus[0]);
        },

        //private
        render: function (obj, pid) {
            var html = '';
            var data = this.treeData;
            if (pid) {
                var parent = this.treeData.filter(function (d) { return d.id === pid; });
                data = parent[0].children;
            }

            function getLink(d) {
                if (d.target === '_blank') {
                    return '<a href="' + d.url + '" id="menu' + d.id + '" target="_blank"><i class="layui-icon ' + d.icon + '"></i>';
                }

                var url = d.url ? (' data-url="' + d.url + '"') : '';
                return '<a href="javascript:;" id="menu' + d.id + '"' + url + '><i class="layui-icon ' + d.icon + '"></i>';
            }

            $(data).each(function (i, d) {
                html += '<li class="layui-nav-item menuItem">';
                html += getLink(d) + '<span class="title">' + d.title + '</span></a>';
                if (pid && d.children) {
                    html += '  <dl class="layui-nav-child">';
                    $(d.children).each(function (ci, cd) {
                        html += '<dd>' + getLink(cd) +' ' + cd.title + '</a></dd>';
                    });
                    html += '  </dl>';
                }
                html += '</li>';
            });
            $('.layui-nav[lay-filter=' + obj + ']').html(html);
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
                _this.initMenuTips();
                element.init();
            });

            _this.initMenuEvent();
            element.on('nav(topMenu)', function (elem) {
                var pid = elem[0].id.replace('menu', '');
                var url = elem.data('url');
                _this.render('leftMenu', pid);
                _this.initMenuTips();
                _this.initMenuEvent();
                _this.option.topChanged && _this.option.topChanged({ url: url });
            });
        },

        initMenuEvent: function () {
            var _this = this;
            $('.layui-nav-tree [data-url]').click(function () {
                _this.closeMenuTips();
                var elem = $(this),
                    src = elem.data('url');
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
        initMenuTips: function () {
            var item = $('.layui-side .menuItem').unbind('mouseenter');
            var pops = $('.popup-tips').unbind('mouseleave');
            this.closeMenuTips();
            if (!this.miniSide)
                return;

            var _this = this;
            item.bind('mouseenter', function () {
                var tip = $(this).html();
                tip = '<ul class="layui-nav layui-nav-tree layui-this"><li class="layui-nav-item popMenuItem layui-nav-itemed">' + tip + '</li></ul>';
                _this.menuTipId = layer.tips(tip, $(this), {
                    tips: [2, '#2f4056'],
                    time: 300000,
                    skin: 'popup-tips',
                    success: function (el) {
                        var left = $(el).position().left - 159;
                        $(el).css({ left: left });
                        $('.popMenuItem').click(function () {
                            var elem = $(this);
                            if (elem.hasClass('layui-nav-itemed')) {
                                elem.removeClass('layui-nav-itemed');
                            } else {
                                elem.addClass('layui-nav-itemed');
                            }
                        });
                        _this.initMenuEvent();
                    }
                });
            });
            pops.bind('mouseleave', function () {
                _this.closeMenuTips();
            });
        },

        closeMenuTips: function () {
            if (this.menuTipId !== '') {
                layer.close(this.menuTipId);
                this.menuTipId = '';
            }
        }

    }

    var Tab = {

        option: null,
        tabId: 'tabMenu',
        homeTabId: '1',
        clsTabTitle: '.layui-layout-admin .layui-tab-title',
        clsTabContext: '.layui-tab-context',

        //public
        show: function (option) {
            this.option = option;
            this.initEvent();
        },

        loadHome: function (url) {
            url = url || this.option.url.Welcome;
            var ifm = $('#ifmWelcome'), src = ifm.attr('src');
            if (src !== url) {
                ifm.attr('src', url);
            }
        },

        addTab: function (node) {
            if (!node.url)
                return;

            var id = node.id;
            var tab = $(this.clsTabTitle + ' li[lay-id="' + id + '"]');
            if (!tab.length) {
                var title = node.icon + ' <span>' + node.text + '</span>';
                var content = '<iframe src="' + node.url + '" frameborder="0" class="layui-tab-iframe"></iframe>';
                element.tabAdd(this.tabId, { id: id, title: title, content: content });
            }
            element.tabChange(this.tabId, id);
        },

        getCurTab: function () {
            var tab = $(this.clsTabTitle + ' .layui-this');
            var id = tab.attr('lay-id');
            var title = tab.children('span').text();
            var module = this.option.menus
                ? this.option.menus.filter(function (m) { return m.id === id; })
                : null;
            return { id: id, title: title, module: module };
        },

        //private
        initEvent: function () {
            var _this = this;
            _this.initContextMenu();

            element.on('tab(tabMenu)', function (elem) {
                var id = $(this).attr('lay-id');
                $('.layui-nav-child dd').removeClass('layui-this');
                $('.layui-nav-child #menu' + id).parent().addClass('layui-this');
            });

            $('.layui-tab-left').click(function () { _this.roll('left'); });
            $('.layui-tab-right').click(function () { _this.roll('right'); });
        },

        roll: function (direction) {
            var title = $(this.clsTabTitle);
            var left = title.scrollLeft();
            if (direction === 'left') {
                title.animate({ scrollLeft: left - 450 }, 200);
            } else {
                title.animate({ scrollLeft: left + 450 }, 200);
            }
        },

        initContextMenu: function () {
            var _this = this, cls = _this.clsTabContext, target;
            $(this.clsTabTitle).contextmenu(function (e) {
                target = $(e.target);
                $(cls).show().css({
                    left: (e.offsetX + 10) + 'px',
                    top: (e.offsetY + 10) + 'px'
                });
                return false;
            }).click(function () { $(cls).hide(); });

            $(cls + ' [tab-close="current"]').click(function () { _this.closeTab(target, 'current'); });
            $(cls + ' [tab-close="other"]').click(function () { _this.closeTab(target, 'other'); });
            $(cls + ' [tab-close="all"]').click(function () { _this.closeTab(target, 'all'); });
        },

        closeTab: function (target, type) {
            var tabId = this.tabId, homeTabId = this.homeTabId;
            function deleteTab(id) {
                if (id && id !== homeTabId) {
                    element.tabDelete(tabId, id);
                }
            }

            var currendId = $(target).parent().attr('lay-id');
            switch (type) {
                case 'current':
                    deleteTab(currendId);
                    break;
                case 'other':
                    $(this.clsTabTitle + ' li').each(function (i, el) {
                        var id = $(el).attr('lay-id');
                        if (id !== currendId) {
                            deleteTab(id);
                        }
                    });
                    break;
                case 'all':
                    $(this.clsTabTitle + ' li').each(function (i, el) {
                        deleteTab($(el).attr('lay-id'));
                    });
                    break;
            }
            $(this.clsTabContext).hide();
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
                Tab.show({ menus: result.menus, url: option.url });
                Menu.show({
                    data: result.menus, topChanged: function (menu) {
                        Tab.loadHome(menu.url);
                    }
                });
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