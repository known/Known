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

function Card(option) {
    //fields
    var _option = option || {},
        _elem, _header, _content;

    //properties
    this.option = _option;

    //methods
    this.render = function () {
        _init();
        return _elem;
    }

    //private
    function _init() {
        _elem = $('<div>').addClass('card');
        _header = $('<div>').addClass('card-header').appendTo(_elem);
        if (_option.icon)
            $('<span>').addClass(_option.icon).appendTo(_header);
        if (_option.title)
            $('<span>').html(_option.title).appendTo(_header);
        if (_option.tool)
            $('<span>').addClass('tool').html(_option.tool).appendTo(_header);

        _content = $('<div>').addClass('card-body').appendTo(_elem);

        if (_option.style)
            _elem.css(_option.style);

        if (_option.body)
            Utils.parseDom(_content, _option.body, _content);

        _option.callback && _option.callback({ header: _header, body: _content });
    }
}

function IconCard(option) {
    //fields
    var _option = option || {},
        _elem, _value;

    //properties
    this.option = _option;

    //methods
    this.render = function () {
        _init();
        return _elem;
    }

    this.setValue = function (value) {
        _value.html(value);
    }

    //private
    function _init() {
        _elem = $('<div>').attr('id', _option.id).addClass('small-box').css('width', _option.width);
        if (_option.onClick) {
            _elem.css('cursor', 'pointer').on('click', _option.onClick);
        } else if (_option.targetPage) {
            _elem.css('cursor', 'pointer').on('click', function () {
                if ($.isPlainObject(_option.targetPage)) {
                    showTabPage(_option.targetPage);
                } else {
                    showTabPageByCode(_option.targetPage);
                }
            });
        }
        var inner = $('<div>').addClass('inner').appendTo(_elem);
        _value = $('<h3>').html(_option.value || '0').appendTo(inner);
        $('<p>').html(_option.name).appendTo(inner);
        var icon = $('<div>').addClass('icon').appendTo(inner);
        $('<i>').addClass(_option.icon).appendTo(icon);

        if (_option.style) {
            inner.css(_option.style);
        }
    }
}