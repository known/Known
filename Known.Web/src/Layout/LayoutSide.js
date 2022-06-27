/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-26     KnownChen
 * ------------------------------------------------------------------------------- */

function LayoutSide(body) {
    var leftMenu;
    var _this = this;

    //methods
    this.render = function (dom) {
        var sider = $('<div>').addClass('layout-side').appendTo(dom);
        var logo = $('<div>').addClass('logo').appendTo(sider);
        $('<img>').attr('src', '/img/logo.png').appendTo(logo);
        var scroll = $('<div>').addClass('layout-scroll').appendTo(sider);
        leftMenu = $('<ul>').addClass('nav-tree').appendTo(scroll);
    }

    this.setMenus = function (menus) {
        _cloneMenus(menus);
        _initMenus(menus);
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
        //this.showHome(menu);
    }

    //private
    function _cloneMenus(menus) {
        Admin.Menus = [];
        Admin.Buttons = [];
        for (var i = 0; i < menus.length; i++) {
            var menu = menus[i];
            if (menu.type === 'button') {
                Admin.Buttons.push($.extend({}, menu));
            } else {
                Admin.Menus.push($.extend({}, menu));
            }
        }
    }

    function _initMenus(menus) {
        var menuTree = Utils.list2Tree(menus, appId);
        if (menuTree.length) {
            if (menuTree.length > 1) {
                //home.head.showMenu(menuTree);
            }
            _this.showMenu(menuTree[0]);
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
                body.showPage($(this).data('page'));
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
}
