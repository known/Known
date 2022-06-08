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

function Router(parent, option) {
    //fields
    var _option = option,
        _elem,
        _current = {},
        _this = this;

    //init
    _init();

    //properties
    this.elem = _elem;

    //methods
    this.route = function (item) {
        var currComp = _current.component;
        currComp && currComp.destroy && currComp.destroy();

        if (option.isTop) {
            if (!item.previous) {
                item.previous = _current;
            }
            _current = item;
        }

        var component = item.component;
        if (component) {
            app.topbar.showTop(item.hideTop);
            if (option.isTop) {
                app.setTool('');
                app.topbar.setTitle(item.title || item.name, !item.isTabbar);
                app.tabbar.showTab(item.isTabbar);
            }
            _elem.html('');
            component.render().appendTo(_elem);
            setTimeout(function () {
                component.mounted && component.mounted();
            }, 10);
        }
    }

    this.back = function () {
        _this.route(_current.previous);
    }

    //pricate
    function _init() {
        if (_option.isTop) {
            _elem = $('<div>').addClass('router').appendTo(parent);
        } else {
            _elem = parent;
        }
    }
}