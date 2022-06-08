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

function Query(option) {
    //fields
    var _option = option,
        _elem = $('<div>').addClass('query'),
        _inputs = [],
        _this = this;

    //init
    if (option.autoRender === undefined || option.autoRender) {
        _init();
    }

    //properties
    this.elem = _elem;

    //methods
    this.render = function () {
        _init();
        return _elem;
    }

    this.getData = function () {
        var data = {};
        for (var i = 0; i < _inputs.length; i++) {
            var input = _inputs[i];
            data[input.name] = input.getValue();
        }
        return data;
    }

    this.search = function () {
        _search();
    }

    //private
    function _init() {
        if (_option.id) {
            _elem.attr('id', _option.id);
        }

        if (!_option.isTradition) {
            _elem.addClass('left');
        }

        if (_option.querys && _option.querys.length) {
            _setQuery(_option.querys);
        }

        _this.elem = _elem;
    }

    function _setQuery(querys) {
        _elem.html('');
        for (var i = 0; i < querys.length; i++) {
            var qi = querys[i];
            if (qi.visible !== undefined && !qi.visible)
                continue;

            var item = $('<div>').addClass('query-item').appendTo(_elem);
            var label = $('<label>').html(qi.title).appendTo(item);

            if (qi.labelWidth) {
                label.css({ width: qi.labelWidth });
            }

            qi.form = _this;
            qi.width = '';
            qi.inputStyle = '';
            qi.type = qi.type || 'text';
            qi.isQuery = true;
            if (qi.type === 'textarea') {
                qi.type = 'text';
            }
            if (qi.code) {
                qi.type = 'select';
            }

            var now = new Date();
            if (qi.type === 'dateRange') {
                var sDate = new Date(now.getFullYear(), now.getMonth(), now.getDate() - (qi.diffDay || 7));
                var sPre = qi.prefixType === 'LG' ? 'L' : 'start';
                var start = new Input(item, $.extend({}, qi, {
                    type: 'date', field: sPre + qi.field, placeholder: DateFormat
                }));
                _setInput(start);
                start.elem.fdatepicker({
                    format: 'yyyy-mm-dd', initialDate: sDate.format(DateFormat), endDate: now
                });
                $('<span>~</span>').appendTo(item);
                var ePre = qi.prefixType === 'LG' ? 'G' : 'end';
                var end = new Input(item, $.extend({}, qi, {
                    type: 'date', field: ePre + qi.field, placeholder: DateFormat
                }));
                _setInput(end);
                end.elem.fdatepicker({
                    format: 'yyyy-mm-dd', initialDate: now.format(DateFormat), endDate: now
                });
            } else if (qi.type === 'dateMonth') {
                var input = new Input(item, $.extend({}, qi, {
                    type: 'date', placeholder: 'yyyy-MM'
                }));
                _setInput(input);
                input.elem.fdatepicker({
                    format: 'yyyy-mm', startView: 'year',
                    minView: 'year', maxView: 'year',
                    initialDate: now.format('yyyy-MM'),
                    endDate: now.format('yyyy-MM')
                });
            } else if (qi.type === 'date') {
                var input = new Input(item, $.extend({}, qi, {
                    type: 'date', placeholder: DateFormat
                }));
                _setInput(input);
                input.elem.fdatepicker({
                    format: 'yyyy-mm-dd', initialDate: now.format(DateFormat), endDate: now
                });
            } else if (qi.type === 'picker') {
                var input = new Input(item, { type: 'text', field: qi.field });
                _setInput(input);
            } else {
                var input = new Input(item, qi);
                _setInput(input);
            }
        }

        Utils.createButton({
            style: 'primary', icon: 'fa fa-search', text: Language.Search,
            handler: function () { _search(); }
        }).appendTo(_elem);

        if (_option.buttons && _option.buttons.length) {
            for (var i = 0; i < _option.buttons.length; i++) {
                var btn = _option.buttons[i];
                Utils.createButton(btn, function () {
                    var data = _this.getData();
                    return { grid: _option.grid, data: data };
                }).appendTo(_elem);
            }
        }
    }

    function _setInput(input) {
        input.setReadonly(false);
        _inputs.push(input);
        _this[input.name] = input;
    }

    function _search() {
        var data = _this.getData();
        if (_option.onSearch) {
            _option.onSearch({ grid: _option.grid, data: data });
        } else {
            _option.grid.reload(data, 0);
        }
    }
}