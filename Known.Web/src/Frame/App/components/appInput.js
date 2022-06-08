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

function Input(parent, option) {
    //fields
    var _parent = parent,
        _option = option,
        _elem,
        _id, _name,
        _this = this;

    //init
    _init();

    //properties
    this.elem = _elem;
    this.id = _id;
    this.name = _name;
    this.value = '';

    //methods
    this.validate = function () {
        _this.clearError();

        var value = $.trim(_this.getValue());
        if (_elem) {
            if (!_elem[0].disabled && _option.required && value === '') {
                _parent.addClass('error');
                return false;
            }
        }

        if (_option.required && value === '') {
            _parent.addClass('error');
            return false;
        }

        return true;
    }

    this.clearError = function () {
        _parent.removeClass('error');
    }

    this.getValue = function () {
        if (_option.type === 'label')
            return _this.value;

        if (_option.type === 'radio')
            return _parent.find('input:checked').val() || '';

        if (_option.type === 'checkbox') {
            var values = [];
            _parent.find('input:checked').each(function (i, e) {
                values.push($(e).val());
            });
            return values.join(',');
        }

        return _elem.val();
    }

    this.setValue = function (value) {
        _this.value = value;
        if (_option.type === 'label') {
            var text = value;
            if (_option.code) {
                text = Utils.getCodeName(_option.code, value);
            }
            _elem.html(text);
        } else if (_option.type === 'radio') {
            var items = _parent.find('input');
            for (var i = 0; i < items.length; i++) {
                items[i].checked = items[i].value === value;
            }
        } else if (_option.type === 'checkbox') {
            var items = _parent.find('input');
            for (var i = 0; i < items.length; i++) {
                var itemValue = items[i].value;
                items[i].checked = value === 1 || value === true || value && value.indexOf(itemValue) > -1;
            }
        } else {
            _elem.val(value);
        }
    }

    this.setReadonly = function (readonly) {
        if (_option.type === 'label')
            return;

        _setReadonly(readonly);
    }

    this.setUrl = function (url, callback) {
        _setUrl(url, callback);
    }

    this.getData = function () {
        if (_option.type === 'select') {
            return _elem.find(':selected').data('data');
        }

        var data = [];
        _parent.find('input:checked').each(function (i, e) {
            data.push($(e).data('data'));
        });
        return data;
    }

    this.setData = function (data, value) {
        _setData(_elem, _option, data, value);
    }

    this.change = function (callback) {
        _change(callback);
    }

    //private
    function _init() {
        _id = _option.id || _option.field;
        _name = _option.field;
        var type = _option.type;

        if (type === 'label') {
            _elem = $('<div>').addClass('label').appendTo(parent);
        } else if (type === 'radio') {
            _setData(parent, _option, _option.code);
        } else if (type === 'checkbox') {
            _setData(parent, _option, _option.code);
        } else {
            if (type === 'select') {
                _elem = $('<select>').appendTo(parent);
                _setData(_elem, _option, _option.code);
            } else if (type === 'textarea') {
                _elem = $('<textarea>').appendTo(parent);
            } else {
                _elem = $('<input>').attr('type', type).appendTo(parent);
                if (_option.value) {
                    _elem.attr('value', _option.value);
                }
            }

            _elem.attr('id', _id);
            _elem.attr('name', _name);

            if (_option.placeholder)
                _elem.attr('placeholder', _option.placeholder);
            if (_option.required)
                _elem.attr('required', true);
            if (_option.width)
                _elem.css({ width: _option.width });
            if (_option.change)
                _change(_option.change);
        }

        if (_option.readonly)
            _setReadonly(true);
    }

    function _setReadonly(readonly) {
        if (_option.type === 'radio' || _option.type === 'checkbox') {
            _parent.find('input').attr('disabled', readonly);
        } else if (_option.type === 'select') {
            _elem.attr('disabled', readonly);
        } else {
            _elem.attr('readonly', readonly);
        }
    }

    function _setUrl(url, callback) {
        $.get(url, function (data) {
            _this.setData(data);
            callback && callback(data);
        });
    }

    function _setData(el, option, data, value) {
        el.html('');
        if (!data) {
            return;
        }

        data = Utils.getCodes(data);
        value = value || option.value;
        if (data && data.length) {
            var items = [];
            if (option.type === 'select') {
                var emptyText = '';
                if (option.emptyText !== undefined) {
                    emptyText = option.emptyText;
                } else {
                    emptyText = Language.PleaseSelect;
                }
                if (emptyText !== '') {
                    items.push({ Code: '', Name: emptyText });
                }
            }

            for (var i = 0; i < data.length; i++) {
                items.push(data[i]);
            }

            for (var i = 0; i < items.length; i++) {
                if (i > 0 && i % option.lineCount === 0) {
                    $('<br>').appendTo(el);
                }

                var data = items[i];
                if (data === '-') {
                    $('<br>').appendTo(el);
                } else {
                    var id = data.Code === '' ? '' : (data.Code || data);
                    var text = data.Name || id;
                    if (option.type === 'select') {
                        _createSelectItem(el, items[i], id, text, value);
                    } else {
                        _createRadioItem(el, items[i], id, text, option);
                    }
                }
            }
        }
    }

    function _change(callback) {
        _elem.change(function () {
            var $this = $(this);
            callback && callback({
                form: _option.form,
                elem: $this,
                value: $this.val(),
                selected: $this.find(':selected').data('data')
            });
        });
    }

    function _createSelectItem(el, data, id, text, value) {
        var item = $('<option>')
            .data('data', data)
            .attr('value', id)
            .html(text)
            .appendTo(el);

        if (value && id === value) {
            item.attr('selected', true);
        }
    }

    function _createRadioItem(el, data, id, text, option) {
        var label = $('<label>').addClass('form-radio').appendTo(el);
        var item = $('<input>')
            .data('data', data)
            .attr('type', option.type)
            .attr('name', option.field)
            .attr('value', id)
            .appendTo(label);

        var value = option.value;
        if (value && id === value) {
            item.attr('checked', true);
        }

        if (option.onClick) {
            item.on('click', function () {
                option.onClick($(this).val());
            });
        }

        $('<span>').html(text).appendTo(label);
    }
}