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

function List(option) {
    //fields
    var _option = option,
        _elem,
        _template = option.template || function (e) {
            $('<span>').addClass('icon')
                .append('<i class="' + e.data.icon + '">')
                .appendTo(e.el);
            $('<span>').addClass('title').html(e.data.title).appendTo(e.el);
            if (e.data.onClick) {
                $('<span>').addClass('right fa fa-chevron-right').appendTo(e.el);
            }
        };

    //properties

    //methods
    this.render = function () {
        _init();
        return _elem;
    }

    this.loading = function () {
        _loading();
    }

    this.load = function (url, callback) {
        _load(url, callback);
    }

    this.setData = function (data) {
        _setData(data);
    }

    //pricate
    function _init() {
        _elem = $('<ul>').addClass('list');

        if (_option.style)
            _elem.addClass(_option.style);

        if (_option.items) {
            var items = _option.items;
            if ($.isFunction(_option.items))
                items = _option.items();
            _setData(items);
        }
    }

    function _loading() {
        _setMessage('load', Language.Loading + '......');
    }

    function _load(url, callback) {
        _loading();
        $.post(url, function (data) {
            if ($.isArray(data))
                _setData(data);
            callback && callback(data);
        });
    }

    function _setData(data) {
        _elem.html('');
        if (data && data.length) {
            for (var i = 0; i < data.length; i++) {
                var d = data[i];
                var li = $('<li>').data('item', d).appendTo(_elem);
                _template({ el: li, data: d });
                if (d.onClick) {
                    li.on('click', function () {
                        var item = $(this).data('item');
                        item.onClick();
                    });
                } else if (_option.onClick) {
                    li.on('click', function () {
                        var item = $(this).data('item');
                        _option.onClick(item);
                    });
                }
            }
        } else {
            _setMessage('error', Language.NoData);
        }
    }

    function _setMessage(cls, message) {
        _elem.html('');
        $('<li>').addClass(cls).html(message).appendTo(_elem);
    }
}