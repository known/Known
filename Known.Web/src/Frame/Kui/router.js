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

function Router(elem, option) {
    //fields
    var _option = option || {},
        _elem = elem,
        _current = {},
        _this = this;

    //properties
    this.elem = _elem;

    //methods
    this.route = function (item) {
        var currComp = _current.component;
        currComp && currComp.destroy && currComp.destroy();

        if (_option.isTop) {
            if (!item.previous) {
                item.previous = _current;
            }
            _current = item;
        }

        var component = item.component;
        if (component) {
            if (typeof component === 'string') {
                _elem.html(component);
            } else {
                _elem.html('');
                if (_option.multiNode) {
                    component.render(_elem);
                } else {
                    component.render().appendTo(_elem);
                }
            }
            setTimeout(function () {
                Page.complete();
                component.mounted && component.mounted();
            }, 10);
        }
    }

    this.back = function () {
        _this.route(_current.previous);
    }

}