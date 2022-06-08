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

function getCurTab() {
    return top.Admin.CurrentTab;
}

function showTabPage(page) {
    page.children = top.Admin.Buttons.filter(function (d) { return d.pid === page.id; });
    top.Admin.showTab(page);
}

function showTabPageByCode(code) {
    var pages = top.Admin.Menus.filter(function (d) { return d.code === code; });
    if (pages.length) {
        showTabPage(pages[0]);
    }
}

function App(id, option) {
    //fields
    var _elem = $('#' + id),
        _router;

    //init
    _init();

    //properties
    this.elem = _elem;
    this.router = _router;
    this.user = {};

    //methods
    this.route = function (item) {
        _router.route(item);
    }

    this.render = function () {
        var m = Utils.getUrlParam('m');
        var page = Page[m];
        if (!page) {
            _elem.html('');
            $('<div>').addClass('content').html(m + Language.NotExist + '！').appendTo(_elem);
        } else {
            _router.route(page);
            setTimeout(function () {
                Page.complete();
                BizHistory.init();
            }, 10);
        }
    }

    this.home = function () {
        this.route({ component: new Home() });
    }

    this.login = function () {
        Utils.setUser(null);
        Utils.setCodes(null);
        this.route({ component: new Login() });
    }

    this.install = function () {
        this.route({ component: new Install() });
    }

    this.active = function () {
        this.route({ component: new Active() });
    }

    //private
    function _init() {
        _router = new Router(_elem, { isTop: true, multiNode: true });
    }
}

var app = new App('app', {});