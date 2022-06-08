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

function Form(elem, option) {
    //fields
    var _option = option,
        _elem = typeof elem === 'string'
            ? $('<div>').attr('id', 'form' + elem)
            : elem,
        _label = {},
        _inputs = [],
        _this = this;

    if (elem.length) {
        _init();
    }

    //properties
    this.elem = _elem;

    //methods
    this.render = function () {
        _init();
        return _elem;
    }

    this.validate = function () {
        var errors = [];
        for (var i = 0; i < _inputs.length; i++) {
            var input = _inputs[i];
            if (!input.validate()) {
                errors.push(input);
            }
        }
        return errors.length === 0;
    }

    this.clear = function () {
        for (var i = 0; i < _inputs.length; i++) {
            var input = _inputs[i];
            input.clearError();
            input.setValue('');
        }
    }

    this.getData = function () {
        var data = {};
        for (var i = 0; i < _inputs.length; i++) {
            var input = _inputs[i];
            data[input.id] = input.getValue();
        }
        return data;
    }

    this.setData = function (data) {
        for (var i = 0; i < _inputs.length; i++) {
            var input = _inputs[i];
            var value = data[input.id];
            input.setValue(value);
        }

        var e = { form: _this, data: data };
        _option.setData && _option.setData(e);
    }

    this.save = function (url, callback) {
        if (!_this.validate())
            return;

        var data = _this.getData();
        _option.onSaving && _option.onSaving(data);
        var formData = { data: JSON.stringify(data) };
        if (_option.submitData) {
            var sd = _option.submitData();
            $.extend(formData, sd);
        }

        Ajax.post(url, formData, function (id) {
            data.Id = id;
            _this.setData(data);
            callback && callback(data);
        });
    }

    //pricate
    function _init() {
        _label = {};
        _inputs = [];
        _elem.html('');

        var hasToolbar = _option.toolbar && _option.toolbar.length;
        var content = hasToolbar ? $('<div>').addClass('form-content').appendTo(_elem) : _elem;

        if (_option.tips) {
            $('<div>').addClass('form-tips').html(_option.tips).appendTo(content);
        }

        if (_option.fields) {
            content.addClass(_option.type || 'form');
            _setFields(content, _option.fields);
        }

        if (hasToolbar) {
            var toolbar = $('<div>').addClass('form-button').appendTo(_elem);
            _initToolbar(toolbar);
        }
    }

    function _initToolbar(container) {
        for (var i = 0; i < _option.toolbar.length; i++) {
            var item = _option.toolbar[i];
            container.append(Utils.createButton(item, { form: _this }));
        }
    }

    function _setFields(parent, fields) {
        for (var i = 0; i < fields.length; i++) {
            var f = fields[i];
            if (f.visible !== undefined && !f.visible)
                continue;

            f.form = _this;
            if (f.type === 'hidden') {
                var input = new Input(parent, f);
                _setInput(input);
            } else if (f.type === 'group') {
                $('<div>').addClass('form-group').html(f.title).appendTo(parent);
            } else {
                var inputEl;
                if (f.label) {
                    if (_label[f.label]) {
                        inputEl = _label[f.label];
                    } else {
                        inputEl = _createItem(parent, f);
                        _label[f.label] = inputEl;
                    }
                } else {
                    inputEl = _createItem(parent, f);
                }

                var input = new Input(inputEl, f);
                _setInput(input);
            }
        }
    }

    function _createItem(parent, option) {
        var title = option.label || option.title;
        var item = $('<div>').addClass('form-item').appendTo(parent);
        var label = $('<label>').addClass('form-label').html(title + '：').appendTo(item);
        var input = $('<div>').addClass('form-input').appendTo(item);

        if (option.itemStyle)
            item.addClass(option.itemStyle);

        if (option.inputStyle)
            input.addClass(option.inputStyle);

        if (option.required)
            label.addClass('required');

        if (option.labelWidth)
            label.css({ width: option.labelWidth });

        if (option.unit)
            $('<span>').addClass('unit').html(option.unit).appendTo(input);

        return input;
    }

    function _setInput(input) {
        _inputs.push(input);
        _this[input.id] = input;
    }
}

Form.bind = function (container, data, callback) {
    for (var p in data) {
        var elem = container.find('[name="' + p + '"]');
        if (elem.length) {
            var value = data[p];
            var dateFormat = elem.attr('placeholder');
            if (dateFormat) {
                var date = value instanceof Date ? value : new Date(value);
                value = date.format(dateFormat);
            }

            var nodeName = elem[0].nodeName;
            if ('DIV,P,SPAN,TD'.indexOf(nodeName) > -1) {
                elem.html(value);
            } else {
                elem.val(value);
            }

            callback && callback({ elem, value, data });
        }
    }
};