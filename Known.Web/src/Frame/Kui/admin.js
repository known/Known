/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

function Home() {
    //fields
    var option = {
        multiTab: true,
        Feedback: baseUrl + '/System/Feedback',
        GetUserData: baseUrl + '/Home/GetUserData?type=1',
        SignOut: baseUrl + '/signout'
    };
    var elem, _this = this;

    this.head = new LayoutHead(this, option);
    this.body = new LayoutBody(this, option);

    //methods
    this.render = function (dom) {
        elem = dom;
    }

    this.mounted = function () {
        $.get(option.GetUserData, { appId: appId }, function (res) {
            if (!res.user) {
                app.login();
            } else {
                if (_checkValidDate(res.user)) {
                    Utils.setUser(res.user);
                    Utils.setCodes(res.codes);
                    _this.head.render(elem, res);
                    _this.body.render(elem, res);
                    Page.complete();
                }
            }
        });
    }

    //private
    function _checkValidDate(user) {
        var validDate = user.AppValidDate;
        if (validDate === '永久')
            return true;

        if (validDate === '') {
            Layer.open({
                width: 300, height: 100,
                content: function (el) {
                    $('<div>').addClass('alert-content center').html(Language.ProductNotAuthorized).appendTo(el);
                }
            });
            return false;
        }

        var date = Date.parse(validDate);
        var now = new Date();
        var diff = parseInt((date - now) / (1000 * 60 * 60 * 24)) + 1;
        if (diff <= 30) {
            var tip = Utils.format(Language.ProductValidDateTips, diff);
            var dlg = Layer.open({
                width: 450, height: 200, content: function (el) {
                    var content = $('<div id="divValidDate">').addClass('layout-alert').appendTo(el);
                    $('<div>').addClass('alert-content').html(tip).appendTo(content);
                    var toolbar = $('<div>').addClass('alert-toolbar').appendTo(content);
                    if (user.AppYun) {
                        $('<button>').addClass('ok')
                            .attr('style', 'margin-right:20px')
                            .html(Language.OK)
                            .appendTo(toolbar)
                            .on('click', function () { dlg.close(); });
                    } else {
                        $('<button>').addClass('active')
                            .attr('style', 'margin-right:20px')
                            .html(Language.GoToActivate)
                            .appendTo(toolbar)
                            .on('click', function () {
                                dlg.close();
                                app.active();
                            });
                    }
                    $('<button>').addClass('cancel')
                        .html(Language.Close)
                        .appendTo(toolbar)
                        .on('click', function () { dlg.close(); });
                }
            });
        }
        return true;
    }

    function LayoutHead(home, option) {
        var logo, topMenu, userName, userAvatar;
        var action = {
            refresh: function () {
                home.body.refresh();
            },
            share: function () {
                alert('share');
            },
            feedback: function () {
                _feedback();
            },
            fullScreen: function () {
                _fullScreen($(this));
            },
            exitScreen: function () {
                _exitScreen($(this));
            },
            shortcut: function () {
            },
            siteMap: function () {
                home.body.showPage({
                    id: 'siteMap',
                    title: $(this).attr('title'),
                    icon: $(this).find('i').attr('class'),
                    type: 'page',
                    code: 'SysSiteMap'
                });
            },
            userInfo: function () {
                home.body.showPage({
                    id: 'userInfo',
                    title: $(this).text(),
                    icon: $(this).find('i').attr('class'),
                    type: 'page',
                    code: 'SysUserInfo'
                });
            },
            logout: function () {
                Layer.confirm(Language.LogoutConfirm, function () {
                    $.post(option.SignOut, function () {
                        app.login();
                    });
                });
            }
        };

        //methods
        this.render = function (dom, res) {
            var user = res.user;
            var header = $('<div>').addClass('layout-header').appendTo(dom);
            var toggle = $('<div>').addClass('toggleMenu').appendTo(header);
            $('<i>').addClass('fa fa-dedent')
                .attr('title', Language.Collapse)
                .appendTo(toggle)
                .on('click', function () {
                    _toggle($(this));
                });
            logo = $('<span>').addClass('appName left').html(appName).appendTo(header);
            topMenu = $('<ul>').attr('id', 'topMenu').addClass('nav left').appendTo(header);

            var right = $('<ul>').attr('id', 'topRight').addClass('nav right').appendTo(header);
            _createNavTool(right);

            var dropdown = $('<li>').addClass('nav-item dropdown').appendTo(right);
            _createUserDropdown(dropdown);

            this.setUserInfo(user);
            _bindAction(action);
        }

        this.setTitle = function (title) {
            logo.html(title);
        }

        this.setUserInfo = function (user) {
            userName.html(user.Name);
            userAvatar.attr('src', staticUrl + user.AvatarUrl);
        }

        this.showMenu = function (menus) {
            topMenu.html('');
            for (var i = 0; i < menus.length; i++) {
                $('<li>')
                    .addClass('nav-item' + (i === 0 ? ' active' : ''))
                    .data('page', menus[i])
                    .html(menus[i].name)
                    .appendTo(topMenu)
                    .on('click', function () {
                        topMenu.find('li').removeClass('active');
                        $(this).addClass('active');
                        var menu = $(this).data('page');
                        home.body.showHome(menu);
                        home.body.showMenu(menu);
                    });
            }
        }

        //private
        function _createNavTool(parent) {
            _createRightItem(parent, 'refresh', Language.RefreshPage, 'fa fa-refresh');
            //_createRightItem(parent, 'share', Language.ShareSystem, 'fa fa-share-alt');
            _createRightItem(parent, 'feedback', Language.Feedback, 'fa fa-commenting-o');
            _createRightItem(parent, 'fullScreen', Language.FullScreen, 'fa fa-arrows-alt');
            //_createRightItem(parent, 'shortcut', Language.Shortcut, 'fa fa-external-link');
            _createRightItem(parent, 'siteMap', Language.SiteMap, 'fa fa-sitemap');
        }

        function _createRightItem(parent, action, title, icon) {
            var li = $('<li>').addClass('nav-item top-right')
                .attr('title', title)
                .attr('action', action)
                .appendTo(parent);
            $('<i>').addClass(icon).appendTo(li);
        }

        function _createUserDropdown(parent) {
            var title = $('<div>').addClass('nav-title title').appendTo(parent);
            userAvatar = $('<img>')
                .attr('alt', 'Avatar')
                .attr('src', staticUrl + '/img/face1.png')
                .appendTo(title);
            userName = $('<span>').appendTo(title);
            $('<i>').addClass('fa fa-caret-down arrow').appendTo(title);

            var child = $('<dl>').addClass('nav-child child').appendTo(parent);
            $('<dd>')
                .attr('action', 'userInfo')
                .addClass('top-right')
                .append('<i class="fa fa-user">')
                .append($('<span>').html(Language.UserCenter))
                .appendTo(child);
            $('<dd>')
                .attr('action', 'logout')
                .addClass('top-right')
                .append('<i class="fa fa-power-off">')
                .append($('<span>').html(Language.SafeLogout))
                .appendTo(child);
        }

        function _feedback() {
            var formFeed = new Form('Feedback', {
                fields: [
                    { field: 'AppId', type: 'hidden' },
                    { field: 'FeedName', type: 'hidden' },
                    { field: 'FeedById', type: 'hidden' },
                    { title: Language.FeedBy, field: 'FeedBy', type: 'text', required: true, readonly: true },
                    { title: Language.ContactMode, field: 'FeedPhone', type: 'text', required: true },
                    { title: Language.FeedContent, field: 'Content', type: 'textarea', required: true, lineBlock: true, inputStyle: 'height:230px;' },
                    { title: Language.Attachment, field: 'File', type: 'file', lineBlock: true }
                ]
            });
            Layer.open({
                title: Language.Feedback, width: 650, height: 450,
                component: formFeed,
                success: function () {
                    var user = Utils.getUser();
                    formFeed.setData({
                        AppId: user.AppId, FeedName: user.CompName,
                        FeedById: user.UserName, FeedBy: user.Name, FeedPhone: user.Mobile
                    });
                },
                buttons: [{
                    text: Language.Send, icon: 'fa fa-send-o', handler: function (e) {
                        if (!formFeed.validate())
                            return;

                        var data = formFeed.getData();
                        Ajax.upload('File', option.Feedback, data, function () {
                            e.close();
                        });
                    }
                }]
            });
        }

        function _fullScreen(obj) {
            var el = document.documentElement;
            var rfs = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullScreen;
            if (rfs) {
                rfs.call(el);
            } else if (typeof window.ActiveXObject !== 'undefined') {
                //for IE
                var wscript = new ActiveXObject('WScript.Shell');
                if (wscript !== null) {
                    wscript.SendKeys('{F11}');
                }
            }
            obj.attr('action', 'exitScreen').html('<i class="fa fa-arrows">');
        }

        function _exitScreen(obj) {
            var el = document;
            var cfs = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullScreen;
            if (cfs) {
                cfs.call(el);
            } else if (typeof window.ActiveXObject !== 'undefined') {
                //for IE
                var wscript = new ActiveXObject('WScript.Shell');
                if (wscript !== null) {
                    wscript.SendKeys('{F11}');
                }
            }
            obj.attr('action', 'fullScreen').html('<i class="fa fa-arrows-alt">');
        }

        function _bindAction(action) {
            $('[action]').on('click', function () {
                var othis = $(this), type = othis.attr('action');
                action[type] ? action[type].call(this, othis) : '';
            });
        }

        function _toggle(obj) {
            var status = obj.data('status');
            if (status && status === '1') {
                obj.data('status', '0').addClass('fa-dedent').removeClass('fa-indent');
                $('body').removeClass('layout-mini');
            } else {
                obj.data('status', '1').addClass('fa-indent').removeClass('fa-dedent');
                $('body').addClass('layout-mini');
                $('.nav-tree i').each(function (i, e) {
                    var title = $(e).next().text();
                    $(e).attr('title', title);
                });
            }
        }
    }

    function LayoutBody(home, option) {
        var leftMenu;
        var _this = this;

        //methods
        this.render = function (dom, res) {
            if (!res.menus.length) {
                var div = $('<div>').addClass('layout-error').appendTo(dom);
                var error = new Error({ type: '404' });
                error.render(div);
            } else {
                _cloneMenus(res.menus);
                _createSider(dom);
                _createNavbar(dom);
                _createBody(dom);
                _initMenus(res);
            }
        }

        this.showMenu = function (menu) {
            leftMenu.html('');
            var menus = menu.children || [];
            for (var i = 0; i < menus.length; i++) {
                var li = $('<li>').addClass('nav-item').appendTo(leftMenu);
                var tl = _createNavTitle(li, menus[i]);
                if (menus[i].children && menus[i].children.length) {
                    var childs = menus[i].children.filter(function (m) { return m.type !== 'button'; });
                    if (childs && childs.length) {
                        tl.append('<span class="fa fa-chevron-down">');
                        var dl = $('<dl>').addClass('nav-child').appendTo(li);
                        $(childs).each(function (ci, cd) {
                            _createNavItem(dl, cd);
                        });
                    }
                }
            }
            Admin.CurrentMenu = menu;
            this.showHome(menu);
        }

        this.showHome = function (page) {
            page.id = 'home';
            page.isHome = true;
            this.showPage(page);
        }

        this.showPage = function (page) {
            if (!page)
                return;

            if (option.multiTab) {
                Admin.showTab(page);
            } else {
                _showPage(page);
            }
        }

        this.refresh = function () {
            if (option.multiTab) {
                Admin.refreshTab();
            } else {
                var iframe = $('.layout-body iframe');
                var url = iframe.attr('src');
                iframe.attr('src', url);
            }
        }

        //private
        function _cloneMenus(menus) {
            Admin.Buttons = [];
            Admin.Menus = [];
            for (var i = 0; i < menus.length; i++) {
                var menu = menus[i];
                if (menu.type === 'button') {
                    Admin.Buttons.push($.extend({}, menu));
                } else {
                    Admin.Menus.push($.extend({}, menu));
                }
            }
        }

        function _initMenus(res) {
            var user = res.user;
            var menus = res.menus;
            var menuTree = Utils.list2Tree(menus, appId);
            if (menuTree.length) {
                if (menuTree.length > 1) {
                    home.head.showMenu(menuTree);
                    _this.showMenu(menuTree[0]);
                } else if (user.AppTopMenu) {
                    var menu = menuTree[0].children;
                    home.head.showMenu(menu);
                    _this.showMenu(menu[0]);
                } else {
                    _this.showMenu(menuTree[0]);
                }
            }
        }

        function _createSider(layout) {
            var sider = $('<div>').addClass('layout-side').appendTo(layout);
            var logo = $('<div>').addClass('logo').appendTo(sider);
            $('<img>').attr('src', staticUrl + '/img/logo.png').appendTo(logo);
            var scroll = $('<div>').addClass('layout-scroll').appendTo(sider);
            leftMenu = $('<ul>').addClass('nav-tree').appendTo(scroll);
        }

        function _createNavbar(layout) {
            var nav = $('<div>').addClass('layout-nav').appendTo(layout);
            $('<div>').addClass('tabs-header').appendTo(nav);
            $('<div>').addClass('fa fa-question-circle-o help')
                .attr('title', Language.HelpDocument)
                .appendTo(nav)
                .on('click', function () {
                    _createHelp();
                });

            if (option.multiTab) {
                Admin.bindContextMenu();
            }
        }

        function _createNavTitle(li, menu) {
            return $('<div>').addClass('nav-title')
                .append('<i class="' + menu.icon + '">')
                .append('<span>' + menu.name + '</span>')
                .data('page', menu)
                .data('show', '0')
                .appendTo(li)
                .on('click', function () {
                    var $this = $(this);
                    _activeItem($this);
                    var page = $this.data('page');
                    if (page.url || page.type === 'page') {
                        _this.showPage(page);
                    } else {
                        leftMenu.find('.nav-title').not($this).each(function (i, elem) {
                            _activeChild($(elem), false);
                        });
                        _activeChild($this, $this.data('show') === '0');
                    }
                });
        }

        function _createNavItem(dl, item) {
            $('<dd>')
                .append('<i class="' + item.icon + '">')
                .append('<span>' + item.name + '</span>')
                .data('page', item)
                .appendTo(dl)
                .on('click', function () {
                    _activeItem($(this));
                    _this.showPage($(this).data('page'));
                });
        }

        function _activeItem(item) {
            leftMenu.find('.nav-title,.nav-child dd').removeClass('active');
            item.addClass('active');
        }

        function _activeChild(item, show) {
            if (show) {
                item.data('show', '1');
                item.next().show();
                item.find('span.fa').removeClass('fa-chevron-down').addClass('fa-chevron-up');
            } else {
                item.data('show', '0');
                item.next().hide();
                item.find('span.fa').removeClass('fa-chevron-up').addClass('fa-chevron-down');
            }
        }

        function _createBody(layout) {
            $('<div>').addClass('layout-body').appendTo(layout);
        }

        //function _createFooter(layout) {
        //    $('<div>').addClass('layout-footer')
        //        .append(Language.TechSupport + '：')
        //        .append('<a href="' + config.app.SupportUrl + '" target="_blank">' + config.app.SupportName + '</a>')
        //        .appendTo(layout);
        //}

        function _createHelp() {
            var body = $('.layout-body');
            body.css({ right: '400px' });
            var help = $('<div>').addClass('layout-help').insertAfter(body);
            $('<span>')
                .addClass('fa fa-close close')
                .appendTo(help)
                .on('click', function () {
                    body.css({ right: 0 });
                    help.remove();
                });
            $('<div>').addClass('title')
                .html('<i class="fa fa-question-circle-o"></i>' + Language.HelpDocument)
                .appendTo(help);
            $('<div>').addClass('content').appendTo(help);
            Admin.showHelp();
        }

        function _showPage(page) {
            var breadcrumb = $('.tabs-header').html('');
            $('<i>').addClass('fa fa-home').css('margin-left', '10px').appendTo(breadcrumb);
            if (!page.isHome) {
                $('<span>').addClass('link').html(Language.HomePage).on('click', function () {
                    _this.showHome(Admin.CurrentMenu);
                }).appendTo(breadcrumb);
                breadcrumb.append(' / ' + page.title);
            } else {
                breadcrumb.append(Language.HomePage);
            }

            var iframe = $('.layout-body iframe')
            if (!iframe.length) {
                iframe = $('<iframe>').attr('frameborder', '0').appendTo($('.layout-body'));
            }
            iframe.attr('title', page.title);
            Admin.CurrentTab = page;
            Admin.loadFrame(iframe, page);
            Admin.showHelp();
        }
    }
}

var Admin = window.Admin = {
    injects: [],
    CacheHelps: {},
    Buttons: [],
    Menus: [],
    CurrentMenu: null,
    CurrentTab: null,

    inject: function (js) {
        if (this.injects.indexOf(js) === -1) {
            this.injects.push(js);
            var script = document.createElement('script');
            script.src = js;
            $('script[src*="js/"]').last().after($(script));
        }
    },

    prependNav: function (item) {
        if ($('#' + item.id).length)
            return;

        var li = $('<li>')
            .attr('id', item.id)
            .addClass('nav-item')
            .css('margin-right', '10px')
            .on('click', item.handler);
        if (item.icon) {
            $('<i>').addClass(item.icon).appendTo(li);
        } else {
            li.html(item.text);
        }
        $('#topRight').prepend(li);
    },

    showTab: function (page) {
        var _this = this;
        var id = 'tab-' + page.id;
        var tab = $('#' + id);
        if (!tab.length) {
            var icon = page.isHome ? 'fa fa-home' : page.icon;
            var title = page.isHome ? Language.HomePage : page.title;
            var item = $('<div id="h' + id + '">')
                .addClass('tabs-header-item')
                .data('data', page)
                .append('<i class="icon ' + icon + '">')
                .append('<span class="title">' + title + '</span>')
                .appendTo($('.tabs-header'))
                .on('click', function () { _this.activeTab($(this).data('data')); });

            if (title !== Language.HomePage) {
                $('<i>').addClass('close fa fa-close')
                    .data('data', page)
                    .appendTo(item)
                    .on('click', function () { _this.closePage($(this).data('data')); });
            }

            tab = $('<div>').attr('id', id).addClass('tabs-body-item').appendTo($('.layout-body'));
            var iframe = $('<iframe>')
                .attr('frameborder', '0')
                .attr('title', page.title)
                .appendTo(tab);
            this.loadFrame(iframe, page);
        }
        this.activeTab(page);
    },

    activeTab: function (page) {
        this.CurrentTab = page;
        $('.tabs-header-item,.tabs-body-item').removeClass('active');
        $('#htab-' + page.id + ',#tab-' + page.id).addClass('active');
        this.showHelp();
    },

    refreshTab: function () {
        var id = this.CurrentTab.id;
        var iframe = $('#tab-' + id + ' iframe');
        var url = iframe.attr('src');
        iframe.attr('src', url);
    },

    closePage: function (page) {
        var htab = $('#htab-' + page.id);
        var isActive = htab.hasClass('active');
        if (isActive) {
            var next = htab.next();
            if (!next.length) {
                next = htab.prev();
            }
            if (next.length) {
                this.activeTab(next.data('data'));
            }
        }
        htab.remove();
        $('#tab-' + page.id).remove();
    },

    closeAll: function () {
        var _this = this;
        $('.tabs-header-item').each(function (i, tab) {
            var page = $(tab).data('data');
            if ($(tab).text() === Language.HomePage) {
                _this.activeTab(page);
            } else {
                $('#htab-' + page.id + ',#tab-' + page.id).remove();
            }
        });
    },

    closeOne: function (id) {
        var title = $('#' + id).text();
        if (title === Language.HomePage)
            return;

        this.closePage($('#' + id).data('data'));
    },

    closeOther: function (id) {
        var _this = this;
        $('.tabs-header-item').each(function (i, tab) {
            if ($(tab).text() !== Language.HomePage) {
                var page = $(tab).data('data');
                if ($(tab).attr('id') === id) {
                    _this.activeTab(page);
                } else {
                    $('#htab-' + page.id + ',#tab-' + page.id).remove();
                }
            }
        });
    },

    bindContextMenu: function () {
        var _this = this, ctxMenu;
        $(document).click(function () {
            if (ctxMenu && ctxMenu.length) {
                ctxMenu.remove();
            }
        });

        $('.tabs-header').contextmenu(function (e) {
            e.preventDefault();
            var items = [];
            if (e.target.className === 'tabs-header') {
                items.push({ Code: 'closeAll', Icon: 'fa fa-times', Name: Language.CloseAll });
            } else if (e.target.className === 'title') {
                items.push({ Code: 'closeAll', Icon: 'fa fa-times', Name: Language.CloseAll });
                items.push({ Code: 'closeOne', Icon: 'fa fa-times-circle-o', Name: Language.CloseOne });
                items.push({ Code: 'closeOther', Icon: 'fa fa-times-circle', Name: Language.CloseOther });
            }

            if (items.length) {
                ctxMenu = $('.ctxmCloseTab').html('');
                if (!ctxMenu.length) {
                    ctxMenu = $('<ul>').addClass('contextmenu ctxmCloseTab').appendTo($('body'));
                }
                ctxMenu.css({ top: e.pageY + 'px', left: e.pageX + 'px' });

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    $('<li>').data('code', item.Code)
                        .append('<i class="' + item.Icon + '">')
                        .append('<span>' + item.Name + '</span>')
                        .appendTo(ctxMenu)
                        .on('click', function () {
                            _this[$(this).data('code')].call(_this, e.target.parentNode.id);
                            ctxMenu.remove();
                        });
                }
            }
        });
    },

    showHelp: function () {
        var help = $('.layout-help');
        if (!help.length)
            return;

        var _this = this;
        var hid = _this.CurrentTab.hid;
        var body = $('.layout-help .content');
        var content = _this.CacheHelps[hid];
        if (content) {
            body.html(content);
        } else {
            body.html(Language.Loading + '......');
            $.get(baseUrl + '/Home/GetHelp?hid=' + hid, function (res) {
                _this.CacheHelps[hid] = res;
                body.html(res);
            });
        }
    },

    loadFrame: function (iframe, page) {
        if (!page)
            return;

        if (page.url) {
            iframe.attr('src', baseUrl + page.url);
            return;
        }

        var modCode = '';
        if (page.isHome) {
            modCode = 'Dashboard';
            if (page.type === 'page') {
                modCode = page.code + 'Dashboard';
            }
        } else if (page.type === 'page') {
            modCode = page.code;
        }
        
        //var lay = Layer.loading(Language.Loading + '......');
        iframe.attr('src', baseUrl + '/?a=' + appId + '&m=' + modCode);//.on('load', function () { lay.close(); });
    }
};

$(function () {
    if (!window.localStorage) {
        var dlg = Layer.open({
            width: 450, height: 250, content: function (el) {
                var content = $('<div>').addClass('layout-alert').appendTo(el);
                $('<div>').addClass('alert-content').html(Language.BrowserTips).appendTo(content);
                var toolbar = $('<div>').addClass('alert-toolbar').appendTo(content);
                $('<button>').addClass('ok')
                    .attr('style', 'margin-right:20px')
                    .html(Language.IKnow)
                    .appendTo(toolbar)
                    .on('click', function () { dlg.close(); });
                $('<button>').addClass('cancel')
                    .html(Language.GoToDownload)
                    .appendTo(toolbar)
                    .on('click', function () {
                        window.open('https://www.baidu.com/s?wd=chrome');
                    });
            }
        });
    }
});