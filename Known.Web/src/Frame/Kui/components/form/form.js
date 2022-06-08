var ExtForm = window.ExtForm = {};
function Form(elem, option) {
    //fields
    var _option = option,
        _formId = typeof elem === 'string' ? 'form' + elem : elem.attr('id'),
        _elem = typeof elem === 'string' ? $('#' + _formId) : elem,
        _saveClose = option.saveClose === undefined ? true : option.saveClose,
        _titleText = _option.title,
        _label = {},
        _inputs = [], _detailButtons = [],
        _this = this, _opened, _isDialog, _isDetail,
        _tabHeader, _tabContent,
        _tabId = _formId + '-tab-',
        _header, _title, _headToolbar, _body, _form, _footer,
        _showOption = {};


    //properties
    this.id = _formId;
    this.elem = _elem;
    this.body = null;
    this.option = _option;
    this.data = {};

    //methods
    this.render = function () {
        this.elem = _elem = $('<div>');
        _init();
        return _elem;
    }

    this.show = function (data, isDetail, option) {
        _showOption = option || {};
        _isDialog = _elem.hasClass('form-card') || _elem.hasClass('tabs-form');
        _showForm(data, isDetail, false);
    }

    this.showDialog = function (data, isDetail, option) {
        _showOption = option || {};
        _isDialog = true;
        _showForm(data, isDetail, false);
    }

    this.close = function () {
        if (_opened) {
            _close();
        }
    }

    this.setToolbar = function (toolbar) {
        this.option.toolbar = toolbar;
        _setToolbar(toolbar);
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
        var data = {}, extNames = [];
        for (var i = 0; i < _inputs.length; i++) {
            var input = _inputs[i];
            if (input.name && input.type !== 'html') {
                if (input.name.indexOf('.') > -1) {
                    var names = input.name.split('.');
                    if (!data[names[0]]) {
                        extNames.push(names[0]);
                        data[names[0]] = {};
                    }
                    data[names[0]][names[1]] = input.getValue();
                } else {
                    data[input.name] = input.getValue();
                }
            }
        }

        if (extNames.length) {
            for (var i = 0; i < extNames.length; i++) {
                data[extNames[i]] = JSON.stringify(data[extNames[i]]);
            }
        }

        return data;
    }

    this.setData = function (data, isDetail) {
        this.data = data;
        for (var i = 0; i < _inputs.length; i++) {
            var input = _inputs[i];
            input.clearError();
            input.data = data;

            if (isDetail) {
                input.setReadonly(true)
            } else {
                input.setReadonly(input.readonly);
            }

            if (input.name && input.name.indexOf('.') > -1) {
                var names = input.name.split('.');
                if (!$.isPlainObject(data[names[0]])) {
                    data[names[0]] = JSON.parse(data[names[0]] || '{}');
                }
                input.setValue(data[names[0]][names[1]], isDetail);
            } else {
                var value = data[input.name];
                if (input.type === 'picker') {
                    value = { value: value, text: data[input.name + 'Name'] };
                }
                input.setValue(value, isDetail);
            }
        }

        var e = { form: _this, data: data, isDetail: isDetail };
        _option.setData && _option.setData(e);
    }

    this.setReadonly = function (readonly) {
        for (var i = 0; i < _inputs.length; i++) {
            var input = _inputs[i];
            input.setReadonly(readonly);
        }
    }

    this.setInput = function (input) {
        _setInput(input);
    }

    this.load = function (url, isDetail) {
        _option.url = url;
        Ajax.get(url, null, function (data) {
            _this.setData(data, isDetail);
        });
    }

    this.reload = function () {
        _load(_option.url);
    }

    this.save = function (url, callback) {
        if (!_this.validate())
            return;

        var data = _this.getData();
        _option.onSaving && _option.onSaving(data);

        var files = [];
        if (_option.fields) {
            files = _option.fields.filter(function (f) {
                return f.type && f.type === 'file';
            });
        }

        if (files.length) {
            Ajax.upload(files[0].field, url, data, saveCallback);
        } else {
            var formData = { data: JSON.stringify(data) };
            if (_option.submitData) {
                var sd = _option.submitData();
                $.extend(formData, sd);
            }
            Ajax.post(url, formData, saveCallback);
        }

        function saveCallback(id) {
            data.Id = id;
            if (_option.tabs) {
                _showForm(data, false, true);
            } else if (_saveClose) {
                _this.close();
            }

            if (_showOption.onSaveCallback) {
                _showOption.onSaveCallback(data);
            } else {
                callback && callback(data);
            }
        }
    }

    //init
    if (_elem.length) {
        _init();
        if (!_option.isPrint) {
            Page.complete();
        }
    }

    //pricate
    function _init() {
        if (_option.tabs) {
            _initTabElement(_option.tabs);
        } else {
            _initFormElement();
        }

        if (_option.style) {
            _form.addClass(_option.style);
        }

        if (_option.ahtml) {
            Utils.parseDom(_body, _option.ahtml, _body);
        }

        _this.body = _body;
        _initFields(_form);

        if (_option.toolbar && _option.toolbar.length) {
            _setToolbar(_option.toolbar);
        } else {
            _body.css({ bottom: 0 });
        }

        if (_option.url) {
            _load(_option.url);
        } else if (_option.data) {
            _this.setData(_option.data);
        }
    }

    function _initTabElement(tabs) {
        _elem.addClass('tabs tabs-fit tabs-form');
        _header = $('<div>').addClass('tabs-header').appendTo(_elem);
        _title = $('<div>').addClass('tabs-title').appendTo(_header);
        _tabHeader = $('<ul>').appendTo(_header);
        _headToolbar = $('<div>').addClass('right-toolbar').appendTo(_header);
        _tabContent = $('<div>').addClass('tabs-body').appendTo(_elem);
        _body = $('<div>')
            .attr('id', _tabId + '0')
            .addClass('tabs-item active')
            .css('padding', '0')
            .appendTo(_tabContent);
        _form = $('<div>').addClass('form').appendTo(_body);

        for (var i = 0; i < tabs.length; i++) {
            var item = tabs[i];
            item.index = i;
            var li = $('<li>')
                .append(item.name)
                .appendTo(_tabHeader)
                .data('item', item)
                .on('click', function () {
                    var item = $(this).data('item');
                    _itemClick($(this), item);
                    var head = _this.getData();
                    if (item.onClick) {
                        item.onClick({ head: head, item: item.component });
                    } else if (item.component && item.component.load) {
                        item.component.load({ head: head, isDetail: _isDetail });
                    }
                });
            if (i === 0) {
                li.addClass('active');
            }
        }
    }

    function _itemClick(elem, item) {
        _tabHeader.find('li').removeClass('active');
        elem.addClass('active');
        var index = item.index || 0;
        var id = _tabId + index;
        var itemEl = $('#' + id);
        if (!itemEl.length) {
            itemEl = $('<div>').attr('id', id).addClass('tabs-item').appendTo(_tabContent);
            new Router(itemEl, {}).route(item);
        }

        $('.tabs-item', _tabContent).removeClass('active');
        itemEl.addClass('active');
    }

    function _initFormElement() {
        if (_option.card) {
            _elem.addClass('form-card');
        } else if (_option.info) {
            _elem.addClass('form-info');
        }

        if (_elem.hasClass('form-card')) {
            _header = $('<div>').addClass('form-card-header').appendTo(_elem);
            _title = $('<div>').addClass('form-title').appendTo(_header);
            _headToolbar = $('<div>').addClass('right-toolbar').appendTo(_header);
            _body = $('<div>').addClass('form-card-body').appendTo(_elem);
            _form = $('<div>').addClass('form').appendTo(_body);
        } else if (_elem.hasClass('form-info')) {
            var icon = _option.icon || 'fa fa-window-maximize';
            _header = $('<div>').addClass('form-info-header').appendTo(_elem);
            _title = $('<div>')
                .append('<i class="' + icon + '">')
                .append(_option.title)
                .appendTo(_header);
            _body = $('<div>').addClass('form-info-body').appendTo(_elem);
            _form = $('<div>').addClass('form').appendTo(_body);
        } else {
            _body = _elem;
            if (!_elem.length) {
                _form = $('<div>').addClass('form').appendTo(_body);
            } else {
                _form = _elem;
            }
        }
    }

    function _initFields(container) {
        if (_option.fields && _option.fields.length) {
            var fields = _option.fields.filter(function (f) {
                return f.type && f.type !== '';
            });
            _setFields(container, fields);
        } else {
            var inputs = _elem.find('input,select,textarea');
            for (var i = 0; i < inputs.length; i++) {
                var el = $(inputs[i]);
                var name = el.attr('name');
                if (name && name.length) {
                    var exist = _inputs.filter(function (f) { return f.name === name; });
                    if (!exist.length) {
                        _setInput(new Input(el));
                    }
                }
            }
        }
    }

    function _setToolbar(toolbar) {
        if (_elem.hasClass('form-card')) {
            if (!_footer) {
                _footer = $('<div>').addClass('form-card-footer').appendTo(_elem);
            }
            _footer.html('');
            _initToolbar(_footer, toolbar);
            Utils.createButton({
                icon: 'fa fa-close', text: Language.Close, style: 'fbClose danger', handler: function () {
                    _this.close();
                }
            }).appendTo(_footer);
        } else {
            if (!_footer) {
                _footer = $('<div>').addClass('form-button').appendTo(_form);
                if (_option.labelWidth) {
                    _footer.css({ marginLeft: (_option.labelWidth + 20) + 'px' });
                }
            }
            _footer.html('');
            _initToolbar(_footer, toolbar);
        }
    }

    function _initToolbar(container, toolbar) {
        for (var i = 0; i < toolbar.length; i++) {
            var item = toolbar[i];
            var btn = Utils.createButton(item, { form: _this }).appendTo(container);
            if (item.detail) {
                btn.addClass('fbDetail');
                _detailButtons.push({ elem: btn, item: item });
            }
        }
    }

    function _load(url) {
        if (_option.isPrint) {
            $.ajax({
                url: url, async: false,
                success: function (data) {
                    _this.setData(data);
                }
            });
        } else {
            Ajax.get(url, null, function (data) {
                _this.setData(data);
            });
        }
    }

    function _setFields(container, fields) {
        container.html('');
        for (var i = 0; i < fields.length; i++) {
            var f = fields[i];
            if (f.visible !== undefined && !f.visible)
                continue;

            f.form = _this;
            if (f.type === 'hidden') {
                var obj = new Input(container, f);
                _setInput(obj);
            } else {
                var input;
                if (f.label) {
                    if (_label[f.label]) {
                        input = _label[f.label];
                    } else {
                        input = _createItem(container, f);
                        _label[f.label] = input;
                    }
                } else {
                    input = _createItem(container, f);
                }

                if (f.type === 'html') {
                    _setInputHtml(f, input);
                } else {
                    var obj = new Input(input, f);
                    _setInput(obj);
                    _setInputHtml(f, input);
                }

                if (f.tips)
                    $('<span>').addClass('form-tips').html(f.tips).appendTo(input);
            }
        }
    }

    function _setInputHtml(field, input) {
        if (!field.inputHtml)
            return;

        Utils.parseDom(input, field.inputHtml, { form: _this, field: field, parent: input });
    }

    function _createItem(parent, option) {
        var title = option.label || option.title || '';
        if (title.length)
            title += '：';

        var item = $('<div>').addClass('form-item').appendTo(parent);
        var label = $('<label>').addClass('form-label').html(title).appendTo(item);
        var input = $('<div>').addClass('form-input').appendTo(item);
        var blockForm = _elem.hasClass('form-block');

        if (option.itemStyle)
            item.addClass(option.itemStyle);
        if (option.inputStyle)
            input.addClass(option.inputStyle);

        if (option.lineBlock || (blockForm && option.type === 'textarea'))
            item.addClass('block');
        if (option.inputBlock || (!blockForm && option.type === 'textarea'))
            input.addClass('block');

        if (option.colSpan)
            item.addClass(option.colSpan);
        if (option.required)
            label.addClass('required');
        if (_option.labelWidth) {
            label.css({ width: _option.labelWidth + 'px' });
            if (option.inputBlock || option.lineBlock || option.type === 'textarea') {
                input.css({ marginLeft: (_option.labelWidth + 20) + 'px' });
            }
        }

        return input;
    }

    function _setInput(input) {
        _inputs.push(input);
        _this[input.name] = input;
    }

    function _createHeadToolbar(container, isDialog) {
        container.html('');
        var div = {};
        function setHeadMouseEvent(header) {
            header.mousedown(function (e) {
                e.preventDefault();
                div.move = true;
                div.offset = [
                    e.clientX - parseFloat(_elem.css('left')),
                    e.clientY - parseFloat(_elem.css('top'))
                ];
            });
            $(document).mousemove(function (e) {
                e.preventDefault();
                if (div.move) {
                    var left = e.clientX - div.offset[0];
                    var top = e.clientY - div.offset[1];
                    _elem.css({ left: left, top: top });
                }
            }).mouseup(function () {
                delete div.move;
            });
        }

        if (isDialog) {
            $('<i>')
                .addClass('fa fa-window-maximize maximize')
                .data('isMax', false)
                .appendTo(container)
                .click(function () {
                    Layer.changeMax($(this), _elem, 'form-dialog-max', Layer.index + 1);
                });

            setHeadMouseEvent(_header);
        } else {
            $('<i>')
                .addClass('fa fa-chevron-up')
                .appendTo(container)
                .click(function () {
                    if (_elem.hasClass('collapse')) {
                        _elem.removeClass('collapse');
                        $(this).removeClass('fa-chevron-down').addClass('fa-chevron-up')
                    } else {
                        _elem.addClass('collapse');
                        $(this).removeClass('fa-chevron-up').addClass('fa-chevron-down');
                    }
                });
        }

        $('<i>')
            .addClass('fa fa-close close')
            .appendTo(container)
            .click(_close);
    }

    function _showForm(data, isDetail, isRefresh) {
        _opened = true;
        _isDetail = isDetail;
        data = data || { Id: '' };

        if (!_titleText) {
            var tab = getCurTab();
            _titleText = tab ? tab.title : '';
        }

        if (_header.length) {
            _title.html('');
            var icon = option.icon || 'fa fa-window-maximize';
            _title.append('<i class="' + icon + '">').append(_titleText);

            var actionName = data.Id === '' ? '【' + Language.New + '】' : (isDetail ? '【' + Language.Detail + '】' : '【' + Language.Edit + '】');
            var titleInfo = '';
            if (_option.titleInfo) {
                titleInfo = _option.titleInfo(data);
            }
            if (titleInfo.indexOf('【') > -1) {
                actionName = '';
            }
            _title.append(actionName + titleInfo);

            _createHeadToolbar(_headToolbar, _isDialog);
        }

        _this.setData(data, isDetail);

        if (_option.tabs) {
            _tabHeader.find('li:eq(0)').click();
        }

        if (_footer) {
            var hideFooter = isDetail && _detailButtons.length === 0;
            hideFooter ? _footer.hide() : _footer.show();
            if (!hideFooter && isDetail) {
                _footer.find('button').hide();
                _footer.find('.fbDetail,.fbClose').show();
            } else {
                _footer.find('button').show();
            }
            if (!_option.tabs) {
                _body.css({ bottom: (hideFooter ? 0 : 40) + 'px' });
            }
        }

        if (_isDialog && !isRefresh) {
            _elem.addClass('form-dialog');
            var index = Layer.index++;
            $('<div>')
                .attr('id', _formId + 'Mask')
                .addClass('mask')
                .css({ zIndex: index })
                .appendTo($('body'));
            var width = _option.width || 800;
            var height = _option.height || 400;
            _elem.css({
                zIndex: index + 1,
                width: width + 'px', height: height + 'px',
                marginTop: -(height / 2) + 'px', marginLeft: -(width / 2) + 'px'
            });
        }

        _elem.removeClass('collapse');
        _elem.show();

        if (_option.max) {
            var btn = _elem.find('.right-toolbar .maximize');
            Layer.changeMax(btn, _elem, 'form-dialog-max', Layer.index + 1);
        }
    }

    function _close() {
        $('#' + _formId + 'Mask').remove();
        if (_elem.hasClass('form-dialog-max')) {
            var btn = _elem.find('.right-toolbar .fa-window-restore');
            Layer.changeMax(btn, _elem, 'form-dialog-max', Layer.index + 1);
        }
        Layer.index--;
        _elem.hide();
        _option.onClose && _option.onClose();
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

            callback && callback({ elem: elem, value: value, data: data });
        }
    }
};