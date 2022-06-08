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

function App(id, option) {
    //fields
    var _option = option,
        _elem = $('#' + id),
        _topbar, _router, _tabbar,
        _this = this;

    //init
    _init();

    //properties
    this.elem = _elem;
    this.topbar = _topbar;
    this.router = _router;
    this.tabbar = _tabbar;
    this.user = {};

    //methods
    this.setTool = function (tool) {
        _topbar.setTool(tool);
    }

    this.setTabbar = function (data) {
        _tabbar.setData(data);
    }

    this.route = function (item) {
        _router.route(item);
    }

    this.login = function () {
        this.route({ component: new Login() });
    }

    this.error = function (type) {
        this.route({ component: new Error({ type: type }) });
    }

    this.logout = function () {
        Layer.confirm(Language.LogoutConfirm, function () {
            $.post(baseUrl + '/signout', function (result) {
                Utils.setUser(null);
                Utils.setCodes(null);
                app.login();
            });
        });
    }

    this.loadUser = function (url, callback) {
        $.get(url, function (res) {
            if (res.timeout) {
                app.login();
                return;
            }
            _this.user = res.user;
            Utils.setUser(res.user);
            Utils.setCodes(res.codes);
            callback && callback(res);
        });
    }

    //private
    function _init() {
        _elem.html('');
        if (_option.showTopbar) {
            _topbar = new Topbar(_elem, {});
        }
        _router = new Router(_elem, { isTop: true });
        if (_option.showTabbar) {
            _tabbar = new Tabbar(_elem, {});
        }
    }
}

var app = new App('app', {
    showTopbar: true,
    showTabbar: true
});