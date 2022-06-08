function Input(parent, option) {
    //fields
    var _parent = parent,
        _option = option || {},
        _isComponent = _option.type === 'component',
        _readonly = _option.readonly === undefined ? false : option.readonly,
        _name = _option.field,
        _input, _elem, _this = this;

    //init
    if (option && option.type) {
        _init();
    } else {
        _initElement();
    }

    //properties
    this.option = _option;
    this.elem = _elem;
    this.readonly = _readonly;
    this.name = _name;
    this.type = _option.type;
    this.value = '';
    this.data = {};

    //methods
    this.validate = function () {
        _this.clearError();
        if (!_option.required)
            return true;

        if (_isComponent)
            return _option.component.validate();

        var value = $.trim(_this.getValue());
        if (value === '') {
            _parent.addClass('error');
            return false;
        }

        return true;
    }

    this.clearError = function () {
        if (_isComponent) {
            _option.component.clearError();
            return;
        }

        _parent.removeClass('error');
    }

    this.getValue = function () {
        if (_isComponent)
            return _option.component.getData();

        return _input.getValue();
    }

    this.setValue = function (value, isDetail) {
        _this.value = value;
        if (_isComponent) {
            _option.component.setData(value, isDetail);
        } else {
            _input.setValue(value, isDetail);
        }
    }

    this.setReadonly = function (readonly) {
        if (_isComponent) {
            _option.component.setReadonly(readonly);
        } else {
            _input.setReadonly(readonly);
        }
    }

    this.setVisible = function (visible) {
        if (visible) {
            _parent.parent().show();
        } else {
            _parent.parent().hide();
        }
    }

    this.setUrl = function (url, callback) {
        $.get(url, function (data) {
            _this.setData(data);
            callback && callback(data);
        });
    }

    this.getData = function () {
        if (_isComponent)
            return _option.component.getData();

        return _input.getData();
    }

    this.setData = function (data, value) {
        if (_isComponent) {
            _option.component.setData(data, value);
        } else {
            _input.setData(data, value);
        }
    }

    this.change = function (callback) {
        _change(callback);
    }

    this.setUnit = function (unit, style) {
        var elm = parent.find('.form-unit').html(unit);
        if (style) {
            elm.css(style);
        }
    }

    this.setTips = function (tips, style) {
        var elm = parent.find('.form-tips').html(tips);
        if (style) {
            elm.css(style);
        }
    }

    //private
    function _init() {
        if (_isComponent) {
            _option.component.render().appendTo(parent);
        } else {
            _initInput(_option.type);
            _input.render(parent);
            if (_input.elem) {
                _elem = _input.elem;
                _initElemAttr();
            }

            if (_option.unit) {
                parent.css({ position: 'relative' });
                $('<span>').addClass('form-unit').html(_option.unit).appendTo(parent);
            }

            if (_option.value) {
                _input.setValue(_option.value);
            }
        } 
    }

    function _initInput(type) {
        if (type === 'label') {
            _input = new InputLabel(_option);
        } else if (type === 'radio') {
            _input = new InputRadio(_option);
        } else if (type === 'checkbox') {
            _input = new InputCheckBox(_option);
        } else if (type === 'select') {
            _input = new InputSelect(_option);
        } else if (type === 'date') {
            _input = new InputDate(_option);
        } else if (type === 'file') {
            _input = new InputFile(_option);
        } else if (type === 'picker') {
            _input = new InputPicker(_option);
        } else if (type === 'editor') {
            _input = new InputEditor(_option);
        } else if (type === 'textarea') {
            _input = new InputTextArea(_option);
        } else {
            _input = new InputDefault(_option);
        }
    }

    function _initElemAttr() {
        //_elem.attr('id', _id);
        _elem.attr('name', _name);
        if (_option.placeholder)
            _elem.attr('placeholder', _option.placeholder);
        if (_option.required)
            _elem.attr('required', true);
        if (_option.inputStyle)
            _elem.attr('style', _option.inputStyle);
        if (_option.change)
            _change(_option.change);
    }

    function _initElement() {
        _elem = parent;
        _parent = _elem.parent();
        _name = _elem.attr('name');
        _data = [];
        var type = _elem[0].nodeName === 'SELECT' ? 'select' : _elem[0].type;
        $.extend(_option, { field: _name, type: type, required: _elem[0].required });
        _initInput(type);
    }

    function _change(callback) {
        if (!_elem)
            return;

        _elem.change(function (e) {
            var $this = $(this);
            callback && callback({
                form: _option.form,
                elem: $this,
                value: $this.val(),
                selected: $this.find(':selected').data('data')
            });
        });
    }

    function InputLabel(item) {
        var _elem, _value = item.value;

        this.option = item;

        this.render = function (dom) {
            _elem = $('<div>').addClass('label').appendTo(dom);
        }

        this.getData = function () {
        }

        this.setData = function (data, value) {
        }

        this.getValue = function () {
            return _value;
        }

        this.setValue = function (value, isDetail) {
            _value = value;
            var text = value;
            if (item.code) {
                text = Utils.getCodeName(item.code, value);
            }
            _elem.html(text);
        }

        this.setReadonly = function (readonly) {
        }
    }

    function InputRadio(item) {
        var _parent;

        this.option = item;

        this.render = function (dom) {
            _parent = dom;
            _setData(dom, item, item.code);
        }

        this.getData = function () {
            var data = [];
            _parent.find('input:checked').each(function (i, e) {
                data.push($(e).data('data'));
            });
            return data;
        }

        this.setData = function (data, value) {
            _setData(_parent, item, data, value)
        }

        this.getValue = function () {
            return _parent.find('input[name="' + item.field + '"]:checked').val() || '';
        }

        this.setValue = function (value, isDetail) {
            var items = _parent.find('input[name="' + item.field + '"]');
            for (var i = 0; i < items.length; i++) {
                items[i].checked = items[i].value === value;
            }
        }

        this.setReadonly = function (readonly) {
            _parent.find('input').attr('disabled', readonly);
        }

        function _setData(el, option, data, value) {
            if (!option.label)
                el.html('');

            var codes = Utils.getCodes(data);
            if (!codes || !codes.length)
                return;

            for (var i = 0; i < codes.length; i++) {
                var code = codes[i];
                var id = code.Code === '' ? '' : (code.Code || code);
                var text = code.Name || id;
                _createRadioItem(el, code, id, text, value || option.value, option);
            }
        }

        function _createRadioItem(el, code, id, text, value, option) {
            if (code.Code === '-') {
                $('<br>').appendTo(el);
                return;
            }

            var label = $('<label>').addClass('form-radio').appendTo(el);
            var input = $('<input>').data('data', code)
                .attr('type', option.type)
                .attr('name', option.field)
                .attr('value', id)
                .appendTo(label);

            if (value && id === value) {
                input.attr('checked', true);
            }

            if (option.onClick) {
                input.on('click', function () {
                    option.onClick($(this).val());
                });
            }

            $('<span>').html(text).appendTo(label);
        }
    }

    function InputCheckBox(item) {
        var _elem, _parent;

        this.option = item;

        this.render = function (dom) {
            _parent = dom;
            var data = item.code;
            if (data === 'Enabled') {
                data = [{ Code: 1, Name: Language.Enable }];
            } else if (item.label) {
                data = [{ Code: 1, Name: data === 'Enabled' ? Language.Enable : item.title }];
            }
            if (data) {
                _setData(dom, item, data);
            } else {
                _elem = $('<input>')
                    .attr('type', 'checkbox')
                    .attr('name', item.field)
                    .attr('value', '1')
                    .appendTo(dom);
                if (item.value === 1 || item.value === '1')
                    _elem[0].checked = true;
            }
        }

        this.getData = function () {
            var data = [];
            _parent.find('input:checked').each(function (i, e) {
                data.push($(e).data('data'));
            });
            return data;
        }

        this.setData = function (data, value) {
            _setData(_parent, item, data, value);
        }

        this.getValue = function () {
            var name = item.field;
            var inputs = _parent.find('input[name="' + name + '"]');
            if (inputs.length === 1) {
                return inputs[0].checked ? 1 : 0;
            } else {
                var values = [];
                _parent.find('input[name="' + name + '"]:checked').each(function (i, e) {
                    values.push($(e).val());
                });
                return values.join(',');
            }
        }

        this.setValue = function (value, isDetail) {
            var items = _parent.find('input[name="' + item.field + '"]');
            for (var i = 0; i < items.length; i++) {
                var itemValue = items[i].value;
                items[i].checked = value === 1 || value === true || value && value.indexOf(itemValue) > -1;
            }
        }

        this.setReadonly = function (readonly) {
            _parent.find('input').attr('disabled', readonly);
        }

        function _setData(el, option, data, value) {
            if (!option.label)
                el.html('');

            var codes = Utils.getCodes(data);
            if (!codes || !codes.length)
                return;

            for (var i = 0; i < codes.length; i++) {
                var code = codes[i];
                var id = code.Code === '' ? '' : (code.Code || code);
                var text = code.Name || id;
                _createRadioItem(el, code, id, text, value || option.value, option);
            }
        }

        function _createRadioItem(el, code, id, text, value, option) {
            if (code.Code === '-') {
                $('<br>').appendTo(el);
                return;
            }

            var label = $('<label>').addClass('form-radio').appendTo(el);
            var input = $('<input>').data('data', code)
                .attr('type', option.type)
                .attr('name', option.field)
                .attr('value', id)
                .appendTo(label);

            if (value && id === value) {
                input.attr('checked', true);
            }

            if (option.onClick) {
                input.on('click', function () {
                    option.onClick($(this).val());
                });
            }

            $('<span>').html(text).appendTo(label);
        }
    }

    function InputSelect(item) {
        var _elem, _this = this;

        this.elem;
        this.option = item;

        this.render = function (dom) {
            _elem = $('<select>').appendTo(dom);
            _this.elem = _elem;
            _setData(_elem, item, item.code);
        }

        this.getData = function () {
            return _elem.find(':selected').data('data');
        }

        this.setData = function (data, value) {
            _setData(_elem, item, data, value);
        }

        this.getValue = function () {
            return _elem.val();
        }

        this.setValue = function (value, isDetail) {
            _elem.val(value);
        }

        this.setReadonly = function (readonly) {
            _elem.attr('disabled', readonly);
        }

        function _setData(el, option, data, value) {
            if (!option.label)
                el.html('');

            var codes = Utils.getCodes(data);
            if (!codes || !codes.length)
                return;

            var items = [];
            var emptyText = '';
            if (option.emptyText !== undefined) {
                emptyText = option.emptyText;
            } else {
                emptyText = option.isQuery ? Language.All : Language.PleaseSelect;
            }
            if (emptyText !== '') {
                items.push({ Code: '', Name: emptyText });
            }

            for (var i = 0; i < codes.length; i++) {
                items.push(codes[i]);
            }

            for (var i = 0; i < items.length; i++) {
                var code = items[i];
                var id = code.Code === '' ? '' : (code.Code || code);
                var text = code.Name || id;
                _createSelectItem(el, code, id, text, value || option.value);
            }
        }

        function _createSelectItem(el, code, id, text, value) {
            var input = $('<option>')
                .data('data', code)
                .attr('value', id)
                .html(text)
                .appendTo(el);

            if (value && id === value) {
                input.attr('selected', true);
            }
        }
    }

    function InputDate(item) {
        var _elem, _parent, _this = this;

        this.elem;
        this.option = item;

        this.render = function (dom) {
            _parent = dom;
            var date = $('<div>').addClass('query-date').appendTo(dom);
            _elem = $('<input>').attr('type', 'text').attr('autocomplete', 'off').appendTo(date);
            $('<span>').addClass('icon fa fa-calendar')
                .appendTo(date)
                .on('click', function () { _elem.focus(); });
            _this.elem = _elem;
        }

        this.getData = function () {
        }

        this.setData = function (data, value) {
        }

        this.getValue = function () {
            return _elem.val();
        }

        this.setValue = function (value, isDetail) {
            var format = _elem.attr('placeholder');
            if (format) {
                if (value instanceof Date) {
                    value = value.format(format);
                } else if (isNaN(value) && !isNaN(Date.parse(value))) {
                    value = new Date(value).format(format);
                }
            }

            _elem.val(value);
        }

        this.setReadonly = function (readonly) {
            if (readonly)
                _parent.find('.icon').hide();
            else
                _parent.find('.icon').show();
            _elem.attr('disabled', readonly);
        }
    }

    function InputFile(item) {
        var _elem, _parent, _this = this;

        this.elem;
        this.option = item;

        this.render = function (dom) {
            _parent = dom;
            _elem = $('<input>')
                .attr('id', item.field)
                .attr('type', 'file')
                .attr('autocomplete', 'off')
                .attr('accept', item.fileExt || '.png,.jpg,.pdf,.doc,.docx.xls,.xlsx,.ppt,.pptx')
                .appendTo(dom);
            if (item.multiple)
                _elem.attr('multiple', 'multiple');
            _this.elem = _elem;
        }

        this.getData = function () {
        }

        this.setData = function (data, value) {
        }

        this.getValue = function () {
            return _elem.val();
        }

        this.setValue = function (value, isDetail) {
            _elem.val('');
            var fileInfo = _parent.find('.file');
            if (fileInfo.length) {
                fileInfo.html('');
            }
            var id = item.form.data[item.idField];
            if (id !== null && id !== '' && id !== undefined) {
                if (!fileInfo.length) {
                    fileInfo = $('<span>').addClass('file inline').appendTo(_parent);
                }
                //if (!isDetail) {
                //    $('<i>').addClass('fa fa-remove red').data('id', id).on('click', function () {
                //        BizFile.delete($(this).data('id'));
                //    }).appendTo(fileInfo);
                //}
                $('<span>').addClass('link').data('id', id).html(value).on('click', function () {
                    BizFile.download($(this).data('id'));
                }).appendTo(fileInfo);
            }
        }

        this.setReadonly = function (readonly) {
            _elem.attr('disabled', readonly);
        }
    }

    function InputPicker(item) {
        var _elem, _picker, _this = this;

        this.elem;
        this.option = item;

        this.render = function (dom) {
            dom.css({ position: 'relative' });
            _elem = $('<input>').attr('type', 'hidden').appendTo(dom);
            _this.elem = _elem;
            _picker = new Picker(_elem, _option);
        }

        this.getData = function () {
            return _picker.getData();
        }

        this.setData = function (data, value) {
        }

        this.getValue = function () {
            return _elem.val();
        }

        this.setValue = function (value, isDetail) {
            _picker.setValue(value);
        }

        this.setReadonly = function (readonly) {
            if (readonly)
                parent.find('.icon').hide();
            else
                parent.find('.icon').show();
            _elem.attr('disabled', readonly);
        }
    }

    function InputEditor(item) {
        var _elem, _parent, _editor, _this = this;

        this.elem;
        this.option = item;

        this.render = function (dom) {
            _parent = dom;
            _elem = $('<textarea>').appendTo(dom);
            _this.elem = _elem;
            _initEditor();
        }

        this.getData = function () {
        }

        this.setData = function (data, value) {
        }

        this.getValue = function () {
            return _elem.val();
        }

        this.setValue = function (value, isDetail) {
            _elem.val(value);
            if (_editor) {
                _editor.txt.html(value);
            }
        }

        this.setReadonly = function (readonly) {
            if (readonly) {
                _parent.find('.editor').addClass('readonly-editor');
                _parent.find('.w-e-text-container').css('border-top', '1px solid #c9d8db');
                _parent.find('.w-e-text').attr('contenteditable', false);
            } else {
                _parent.find('.editor').removeClass('readonly-editor');
                _parent.find('.w-e-text-container').css('border-top', 'none');
                _parent.find('.w-e-text').attr('contenteditable', true);
            }
        }

        function _initEditor() {
            if (!window.wangEditor) {
                Utils.addJs('/libs/wangEditor.min.js');
            }

            _elem.hide();
            var divId = 'editor' + _name;
            _elem.before('<div id="' + divId + '" class="editor">');

            _editor = new window.wangEditor('#' + divId);
            _editor.config.onchange = function (html) {
                _elem.val(html);
            };

            if (OssClient.config.accessKeyId) {
                var oss = new OssClient();
                _editor.config.customUploadImg = function (resultFiles, insertImgFn) {
                    var url = option.form.OssUrl.getValue();
                    var path = option.form.OssPath.getValue();
                    if (!url && !path) {
                        Layer.tips('OSSPath' + Language.NotEmpty + '！');
                        return;
                    }
                    var file = resultFiles[0];
                    path += '/' + file.lastModified + file.name;
                    oss.put(path, file, function (res) {
                        insertImgFn(url + res.name);
                    });
                }
            } else {
                _editor.config.uploadImgServer = baseUrl + '/System/Upload';
            }

            setTimeout(function () { _editor.create(); }, 10);
        }
    }

    function InputTextArea(item) {
        var _elem, _this = this;

        this.elem;
        this.option = item;

        this.render = function (dom) {
            _elem = $('<textarea>').appendTo(dom);
            _this.elem = _elem;
        }

        this.getData = function () {
        }

        this.setData = function (data, value) {
        }

        this.getValue = function () {
            return _elem.val();
        }

        this.setValue = function (value, isDetail) {
            _elem.val(value);
        }

        this.setReadonly = function (readonly) {
            _elem.attr('disabled', readonly);
        }
    }

    function InputDefault(item) {
        var _elem, _this = this;

        this.elem;
        this.option = item;

        this.render = function (dom) {
            _elem = $('<input>')
                .attr('type', item.type)
                .attr('autocomplete', 'off')
                .appendTo(dom);
            _this.elem = _elem;
        }

        this.getData = function () {
        }

        this.setData = function (data, value) {
        }

        this.getValue = function () {
            if (!_elem || !_elem.length)
                return '';

            return _elem.val();
        }

        this.setValue = function (value, isDetail) {
            if (!_elem || !_elem.length)
                return;

            _elem.val(value);
        }

        this.setReadonly = function (readonly) {
            if (!_elem || !_elem.length)
                return;

            _elem.attr('disabled', readonly);
        }
    }
}