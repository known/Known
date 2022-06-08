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

function Grid(name, option) {
    //field
    var _gridId = 'grid' + name,
        _queryId = _gridId + 'Query', querys = [],
        _elem = $('#' + _gridId),
        index = option.index === undefined ? true : option.index,
        fixed = option.fixed === undefined ? true : option.fixed,
        isEdit = option.edit,
        isImport = option.import,
        autoQuery = option.autoQuery === undefined ? (option.url ? true : false) : option.autoQuery,
        showPage = option.page === undefined ? (option.url ? true : false) : option.page,
        pageSizes = [10, 20, 50, 100, 500, 1000],
        action = $.extend({
            add: function (e) { e.grid.addRow(option.row || {}); },
            remove: function (e) { e.grid.removeRow(e.index); },
            up: function (e) { e.grid.moveRow(e.index, 'up', option.moveRowUrl); },
            down: function (e) { e.grid.moveRow(e.index, 'down', option.moveRowUrl); }
        }, option.action || {}),
        rowDataName = 'row';
    var _this = this, _moving = false,
        _view, _grid = _elem.parent(),
        _where = { field: '', order: '' },
        _pageCount = 0,
        _columnLength = 0, _dataColumns = [], _forms = [], _inputs = [];
    var thead, tbody, page;
    var toolButtons = option.toolButtons || [];
    var columnButtons = toolButtons.filter(function (d) {
        return d === 'edit' || d === 'remove' || d.target && d.target.indexOf('grid') >= 0;
    });
    var _checkBox = option.showCheckBox || (!isImport && toolButtons.length > 0);
    var _multiSelect = option.multiSelect === undefined ? true : option.multiSelect;
    var _isDetail = false;

    var sort = {};
    if (option.sortField) {
        var sortFields = option.sortField.split(',');
        var sortOrders = option.sortOrder.split(',');
        for (var i = 0; i < sortFields.length; i++) {
            sort[sortFields[i]] = sortOrders[i];
        }
    }
    if (isEdit) {
        if (option.fixed === undefined) {
            fixed = false;
        }
    }
    if (isEdit || isImport) {
        showPage = false;
        _checkBox = false;
    }
    if (showPage) {
        _where.page = 1;
        _where.limit = 10;
    }

    //property
    this.elem = _elem;
    this.option = option;
    this.name = name;
    this.columns = [];
    this.data = [];
    this.toolbar = null;
    this.query = null;
    this.where = option.where || {};
    this.total = 0;

    //method
    this.render = function () {
        _gridId = 'grid' + name;
        _gridTop = 0;
        _view = $('<div>').addClass('grid-view');
        _grid = $('<div>').addClass('grid').appendTo(_view);
        _elem = $('<table>').attr('id', _gridId).appendTo(_grid);
        _init();
        return _view;
    }

    this.setDetail = function (isDetail) {
        _isDetail = isDetail;

        if (_this.toolbar) {
            if (isDetail) {
                _this.toolbar.elem.hide();
            } else {
                _this.toolbar.elem.show();
            }
        }

        if (_this.query) {
            if (isDetail) {
                _this.query.elem.removeClass('hide').css({ paddingTop: '5px' });
            } else if (option.isTradition) {
                _this.query.elem.addClass('hide');
            }
        }

        var selector = 'th.check,th.action,th.tb-head';
        if (isDetail) {
            _elem.find(selector).hide();
        } else {
            _elem.find(selector).show();
        }

        _setGridTop();
    }

    this.setReadonly = function (readonly) {
        this.setDetail(readonly);
    }

    this.setIsEdit = function (isEditable) {
        isEdit = isEditable;
    }

    this.setColumns = function (columns) {
        _dataColumns = [];
        columns = columns || [];
        var phTime = DateTimeFormat;
        if (option.showModifyBy) {
            columns.push({ title: Language.ModifyBy, field: 'ModifyBy' });
            columns.push({ title: Language.ModifyTime, field: 'ModifyTime', placeholder: phTime });
        }
        if (option.showCreateBy) {
            columns.push({ title: Language.CreateBy, field: 'CreateBy' });
            columns.push({ title: Language.CreateTime, field: 'CreateTime', placeholder: phTime });
        }
        this.columns = columns;

        _columnLength = 0;
        thead.html('');
        if (index) {
            _columnLength += 1;
            $('<th>').addClass('index').appendTo(thead);
        }

        if (_checkBox) {
            _columnLength += 1;
            _setHeadCheckBox();
        }

        if (columnButtons.length > 0 && !option.isTradition) {
            _columnLength += 1;
            $('<th>')
                .addClass('center tb-head')
                .css('width', '80px')
                .html(Language.Operate)
                .appendTo(thead);
        }

        querys = option.querys || [];
        for (var i = 0; i < columns.length; i++) {
            var column = columns[i];
            if (column.query && !isEdit && !isImport) {
                querys.push($.extend({ prefixType: 'LG' }, column));
            }

            if (column.type === 'hidden') {
                _dataColumns.push(column);
                continue;
            }
            if (column.hideColumn !== undefined && column.hideColumn)
                continue;
            if (column.visible !== undefined && !column.visible)
                continue;

            var head = _getColumnHead(column);
            var th = $('<th>').append(head).appendTo(thead);
            if (column.action)
                th.addClass('action');
            if (column.width)
                th.css({ width: column.width });
            if (column.field)
                th.attr('field', column.field);

            _setAlignClass(th, column.align);

            if (isImport) {
                if (column.required)
                    th.addClass('red');
                if (column.code) {
                    th.append(' *')
                        .css({ cursor: 'pointer' })
                        .data('column', column)
                        .on('click', function () {
                            var col = $(this).data('column');
                            _openImportCodeTip(col);
                        });
                }
            } else {
                if (column.sort || sort[column.field]) {
                    th.addClass('sorting')
                        .append('<i class="fa fa-caret-down">')
                        .append('<i class="fa fa-caret-up">');
                    th.on('click', function () {
                        var $this = $(this), field = $this.attr('field');
                        sort = {};
                        sort[field] = _where.order === 'asc' ? 'desc' : 'asc';
                        _elem.find('th').removeClass('asc desc');
                        $this.addClass(sort[field]);
                        _this.reload();
                    });
                }
                th.removeClass('asc desc');
                if (sort[column.field]) {
                    th.addClass(sort[column.field]);
                }
            }

            _dataColumns.push(column);
            _columnLength += 1;
        }
    }

    this.setData = function (data, isDetail) {
        this.data = data || [];
        tbody.html('');
        if (!this.data.length && !isEdit) {
            _setErrorMessage(Language.NoDataFound);
        } else {
            $(this.data).each(function (i, d) {
                _setDataRow(!isEdit, i, d, isDetail);
            });
        }
    }

    this.reload = function (where, isLoad) {
        var qdata = _this.query ? _this.query.getData() : {};
        $.extend(this.where, where || {}, qdata);
        if (option.querys) {
            _queryData(this.where);
        } else {
            _where.load = isLoad === 0 ? 0 : 1;
            _where.query = JSON.stringify(this.where);
            var aFields = [], aSorts = [];
            for (var f in sort) {
                aFields.push(f);
                aSorts.push(sort[f]);
            }
            _where.field = aFields.join(',');
            _where.order = aSorts.join(',');
            _queryData(_where);
        }
    }

    this.getSelected = function () {
        var rows = [];
        _elem.find('tbody .checkbox :checked').each(function () {
            var tr = $(this).parent().parent();
            var idx = tbody.find('tr').index(tr);
            var row = $(this).data(rowDataName);
            row.index = idx;
            rows.push(row);
        });
        return rows;
    }

    this.addRow = function (item) {
        _setDataRow(false, this.data.length, item, false);
        this.data.push(item);
    }

    this.removeRow = function (index) {
        tbody.find('tr:eq(' + index + ')').remove();
        this.data.splice(index, 1);
    }

    this.moveRow = function (index, direct, url) {
        if (_moving)
            return;

        if (direct === 'up') {
            if (index === 0) {
                Layer.tips(Language.CanNotMoveUp);
                return;
            }

            _moveUpRow(this.data, index, url);
        } else if (direct === 'down') {
            if (index >= this.data.length - 1) {
                Layer.tips(Language.CanNotMoveDown);
                return;
            }

            _moveDownRow(this.data, index, url);
        }
    }

    this.getData = function () {
        _acceptChange();
        return _this.data;
    }

    this.getValue = function (row, column) {
        var value = row[column.field];
        if (value === null)
            return '';

        if (column.valueFormat) {
            return column.valueFormat({ value: value, row: row, column: column });
        } else if (column.placeholder) {
            if (value instanceof Date)
                return value;
            else if (isNaN(value) && !isNaN(Date.parse(value)))
                return new Date(value);
        } else if (column.code) {
            return _getCodeCellText(column.code, value);
        } else {
            return value || value === 0 ? value : '';
        }
    }

    this.clearError = function () {
        if (!isEdit)
            return;

        tbody.find('td').removeClass('error');
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

    //private
    function _setErrorMessage(message) {
        _setMessage('center error', message);
    }

    function _setMessage(cls, message) {
        tbody.html('');
        var tr = $('<tr>').appendTo(tbody);
        $('<td>')
            .attr('colspan', _columnLength)
            .addClass(cls)
            .html(message)
            .appendTo(tr);
    }

    function _openImportCodeTip(col) {
        var codes = Utils.getCodes(col.code);
        Layer.open({
            width: 300, height: 200, showClose: true,
            content: function (el) {
                $('<h3>').css('padding', '10px')
                    .html(col.title + Language.CodeOptionTip)
                    .appendTo(el);
                var cont = $('<div>').css('padding', '0 10px 10px 10px').appendTo(el);
                for (var i = 0; i < codes.length; i++) {
                    var data = codes[i];
                    var code = data.Code === 0 ? 0 : (data.Code || data);
                    var text = code;
                    if (data.Name) {
                        text += '-' + data.Name;
                    }
                    $('<p>').html(text).appendTo(cont);
                }
            }
        });
    }

    function _queryData(data) {
        _setMessage('center', Language.Loading + '......');
        $.ajax({
            type: option.url.indexOf('Get') > -1 ? 'get' : 'post',
            url: option.url, data: data,
            cache: false, async: true,
            success: function (result) {
                if ($.isArray(result)) {
                    _this.total = result.length;
                    _setQueryData(result);
                } else {
                    _this.total = result.TotalCount;
                    _setPageInfo(result.TotalCount)
                    _setQueryData(result.PageData);
                    option.summary && option.summary(result.Summary);
                }
            },
            error: function () {
                _setErrorMessage(Language.ServerError);
            }
        });
    }

    function _setQueryData(data) {
        if (option.setData) {
            option.setData({ grid: _this, data: data });
        } else {
            _this.setData(data);
        }
    }

    function _queryPage(pageIndex) {
        if (_pageCount === 0 || pageIndex <= 0 || pageIndex > _pageCount)
            return;
        _where.page = pageIndex;
        _this.reload(null, 0);
    }

    function _setAlignClass(elm, align) {
        align = align || '';
        if (align === 'center') {
            elm.addClass('center');
        } else if (align === 'right') {
            elm.addClass('text-right');
        }
    }

    function _getColumnHead(column) {
        if (column.action) {
            var btn = $('<span>').addClass('link').data('action', column.action);
            if (column.icon) {
                btn.addClass(column.icon);
            } else {
                btn.html(column.title);
            }
            btn.on('click', function () {
                var code = $(this).data('action');
                action[code] && action[code].call(this, { grid: _this, form: option.form });
            });
            return btn;
        }

        return column.title;
    }

    function _setDataRow(alter, idx, row, isDetail) {
        var tr = $('<tr>').appendTo(tbody);
        if (alter && (idx + 1) % 2 === 0) {
            tr.addClass('alter');
        }
        var tdHidden = $('<span>');
        if (index) {
            var td = $('<td>').addClass('center').append(idx + 1).appendTo(tr);
            if (isEdit) {
                td.append(tdHidden);
            }
        }
        if (_checkBox && !_isDetail) {
            _createCheckBox(tr, row);
        }
        if (columnButtons.length > 0 && !option.isTradition && !_isDetail) {
            var td = $('<td>').addClass('center btns').data(rowDataName, row).appendTo(tr);
            _initGridToolbar(td);
        }

        var form = new Form('Row' + idx, { grid: _this });
        _forms.push(form);
        for (var i = 0; i < _dataColumns.length; i++) {
            var column = $.extend({ rowId: idx }, _dataColumns[i], true);
            if (column.action && isDetail)
                continue;
            if (!isEdit && column.type === 'hidden')
                continue;

            var td = column.type === 'hidden' ? tdHidden : $('<td>').appendTo(tr);
            if (column.action) {
                td.addClass('action');
            }
            _setAlignClass(td, column.align);

            if (column.field && column.field.indexOf('.') > -1) {
                var names = column.field.split('.');
                if (!$.isPlainObject(row[names[0]])) {
                    row[names[0]] = JSON.parse(row[names[0]] || '{}');
                }
            }

            if (column.format) {
                var format = column.format;
                if (typeof format === 'string' && format === 'detail') {
                    format = _getDetailFormat;
                }
                var tdc = format(row, {
                    index: idx, tr: tr, td: td, column: column,
                    row: row, grid: _this, form: option.form
                });
                td.append(tdc);
            } else if (column.field) {
                _setCellHtml(td, form, column, row, isDetail);
            }

            if (_checkBox)
                _setRowClickEvent(td);

            if (column.aFormat) {
                _setActionForamt({
                    index: idx, tr: tr, td: td, row: row, column: column
                });
            }
        }
    }

    function _createCheckBox(tr, row) {
        var td = $('<td>').addClass('center checkbox').appendTo(tr);
        var chkElem = _multiSelect
            ? $('<input>').attr('type', 'checkbox')
            : $('<input>').attr('type', 'radio').attr('name', 'gridCheck');
        chkElem.data(rowDataName, row).on('change', function () {
            if ($(this).is(':checked')) {
                _setSelectRow($(this), true);
            } else {
                _setSelectRow($(this), false);
            }
        }).appendTo(td);
    }

    function _getDetailFormat(row, e) {
        return $('<span>')
            .addClass('link')
            .html(row[e.column.field])
            .data('data', row)
            .on('click', function () {
                var data = $(this).data('data');
                if (e.form) {
                    e.form.show(data, true);
                    if (option.formUrl) {
                        e.form.load(option.formUrl + '?id=' + data.Id, true);
                    }
                }
            });
    }

    function _setRowClickEvent(td) {
        var chkType = _multiSelect ? ':checkbox' : ':radio';
        td.on('click', function (e) {
            _fullSelectRow(false);
            var parent = $(this).parent(),
                chk = parent.find(chkType);
            chk[0].checked = !chk.is(':checked');
            if (chk[0].checked) {
                parent.addClass('selected');
            } else {
                parent.removeClass('selected');
            }
        });

        if (option.dblclick) {
            td.on('dblclick', function (e) {
                var parent = $(this).parent(),
                    chk = parent.find(chkType);
                var data = chk.data(rowDataName);
                option.dblclick({ row: data });
            });
        }
    }

    function _setActionForamt(opt) {
        var format = {
            remove: function () {
                return $('<span>').addClass('link red fa fa-trash-o')
                    .css({ marginLeft: '5px' })
                    .attr('action', 'remove')
                    .attr('title', Language.Delete);
            },
            up: function () {
                return $('<span>').addClass('link fa fa-arrow-up')
                    .css({ marginLeft: '5px' })
                    .attr('action', 'up')
                    .attr('title', Language.MoveUp);
            },
            down: function () {
                return $('<span>').addClass('link fa fa-arrow-down')
                    .css({ marginLeft: '5px' })
                    .attr('action', 'down')
                    .attr('title', Language.MoveDown);
            },
        };
        var aFormat = opt.column.aFormat;
        if (typeof aFormat === 'string') {
            var formats = aFormat.split(',');
            for (var i = 0; i < formats.length; i++) {
                format[formats[i]]().appendTo(opt.td);
            }
        } else {
            aFormat(opt);
        }
        opt.tr.find('[action]').on('click', function (e) {
            _acceptChange();
            var code = $(this).attr('action');
            var trObj = $(this).parentsUntil('tbody').last();
            var trIdx = tbody.find('tr').index(trObj);
            var row = _this.data[trIdx];
            action[code] && action[code].call(this, {
                grid: _this, form: option.form, row: row, index: trIdx
            });
        });
    }

    function _acceptChange() {
        if (!isEdit)
            return;

        for (var i = 0; i < _forms.length; i++) {
            _this.data[i] = _forms[i].getData();
        }
    }

    function _setCellHtml(parent, form, column, data, isDetail) {
        var value;
        if (column.field.indexOf('.') > -1) {
            var names = column.field.split('.');
            value = data[names[0]][names[1]];
        } else {
            value = data[column.field];
        }

        if (isEdit && column.type && !isDetail) {
            value = value || '';
            if (column.type === 'picker') {
                var text = data[column.field + 'Name'] || '';
                if (column.textField) {
                    text = data[column.textField] || '';
                }
                column.value = { value: value, text: text };
            } else {
                column.value = value;
            }
            column.form = form;
            var input = new Input(parent, column);
            form.setInput(input);
            _inputs.push(input);
            if (column.unitField) {
                input.setUnit(data[column.unitField] || '');
            }
            return;
        }

        if (value === null || value === undefined || value === '' || column.type === 'hidden')
            return;

        if (column.type === 'file') {
            $('<span>')
                .addClass('link')
                .data('id', data[column.idField])
                .html(Language.Attachment)
                .appendTo(parent)
                .on('click', function () {
                    BizFile.download($(this).data('id'));
                });
        } else if (column.type === 'picker') {
            var text = value;
            var nameValue = data[column.field + 'Name'];
            if (nameValue) {
                text = nameValue;
            } else if (column.textField) {
                text = data[column.textField] || '';
            }
            parent.html(text);
        } else {
            var text = value;
            if (column.placeholder) {
                if (value instanceof Date) {
                    text = value.format(column.placeholder);
                } else if (isNaN(value) && !isNaN(Date.parse(value))) {
                    text = new Date(value).format(column.placeholder);
                }
            } else if (column.code) {
                if ('YesNo,HasNot,Enabled'.indexOf(column.code) > -1) {
                    if (value === '1' || value === '0') {
                        value = parseInt(value);
                    }
                    text = _getCodeCellText(column.code, value);
                    var color = value === 1 ? 'success' : 'gray';
                    text = '<span class="badge ' + color + '">' + text + '</span>';
                } else {
                    text = _getCodeCellText(column.code, value);
                }
            } else {
                text = value || value === 0 ? value : '';
            }

            if (column.unit) {
                text = text + column.unit;
            }

            if (column.unitField) {
                text = text + (data[column.unitField] || '');
            }

            parent.html(text);
        }
    }

    function _getCodeCellText(category, value) {
        if (!(value || value === 0)) {
            return '';
        }

        return Utils.getCodeName(category, value);
    }

    function _setHeadCheckBox() {
        var th = $('<th>').addClass('check').appendTo(thead);
        if (!_multiSelect)
            return;

        $('<input>')
            .attr('type', 'checkbox')
            .appendTo(th)
            .on('change', function () {
                if ($(this).is(':checked')) {
                    _fullSelectRow(true);
                } else {
                    _fullSelectRow(false);
                }
            });
    }

    function _fullSelectRow(checked) {
        var chkType = _multiSelect ? ':checkbox' : ':radio';
        _elem.find('tbody ' + chkType).each(function () {
            _setSelectRow($(this), checked);
        });
        if (!checked && _multiSelect) {
            _elem.find('.check ' + chkType)[0].checked = false;
        }
    }

    function _setSelectRow(checkbox, checked) {
        checkbox[0].checked = checked;
        if (checked) {
            checkbox.parent().parent().addClass('selected');
        } else {
            checkbox.parent().parent().removeClass('selected');
        }
    }

    function _moveUpRow(data, idx, url) {
        _moving = true;
        _elem.find('tbody tr').removeClass('selected');
        var current = _elem.find('tbody tr:eq(' + idx + ')').addClass('selected');

        if (url) {
            var row = data[idx];
            $.post(url, { id: row.Id, direct: 'up' }, function () {
                _moveUpRow(data, idx);
            });
        } else {
            _forms.splice(idx - 1, 0, _forms[idx]);
            _forms.splice(idx + 1, 1);

            data.splice(idx - 1, 0, data[idx]);
            data.splice(idx + 1, 1);

            var prev = current.prev();
            current.insertBefore(prev);

            if (index) {
                var prevTd = prev.find('td:first');
                prevTd.html(parseInt(prevTd.html()) + 1);
                var currTd = current.find('td:first');
                currTd.html(parseInt(currTd.html()) - 1);
            }
            _moving = false;
        }
    }

    function _moveDownRow(data, idx, url) {
        _moving = true;
        _elem.find('tbody tr').removeClass('selected');
        var current = _elem.find('tbody tr:eq(' + idx + ')').addClass('selected');

        if (url) {
            var row = data[idx];
            $.post(url, { id: row.Id, direct: 'up' }, function () {
                _moveDownRow(data, idx);
            });
        } else {
            _forms.splice(idx + 2, 0, _forms[idx]);
            _forms.splice(idx, 1);

            data.splice(idx + 2, 0, data[idx]);
            data.splice(idx, 1);

            var next = current.next();
            if (next) {
                current.insertAfter(next);
            }

            if (index) {
                var currTd = current.find('td:first');
                currTd.html(parseInt(currTd.html()) + 1);
                var nextTd = next.find('td:first');
                nextTd.html(parseInt(nextTd.html()) - 1);
            }
            _moving = false;
        }
    }

    function _setPageInfo(count) {
        if (!page.length) return;
        _pageCount = Math.ceil(count / _where.limit);
        page.find('.pi').val(_where.page);
        page.find('.pc').html(_pageCount);
        page.find('.pt').html(count);
    }

    function _setGridTop() {
        if (!fixed)
            return;

        setTimeout(function () {
            var top = 0;
            if (option.isTradition) {
                if (_this.toolbar)
                    top += _this.toolbar.elem[0].clientHeight;
                if (_this.query)
                    top += _this.query.elem[0].clientHeight;
            } else {
                if (_this.toolbar)
                    top += _this.toolbar.elem[0].clientHeight;
                else if (_this.query)
                    top += _this.query.elem[0].clientHeight;
            }
            if (top === 0)
                top = 5;
            _grid.css({ top: top + 'px' });
        }, 10);
    }

    function _init() {
        _isDetail = false;
        if (option.width) {
            _elem.css({ width: option.width });
        }
        thead = _elem.find('thead tr');
        if (!thead.length) {
            var head = $('<thead>').appendTo(_elem);
            thead = $('<tr>').appendTo(head);
        }
        tbody = _elem.find('tbody');
        if (!tbody.length) {
            tbody = $('<tbody>').appendTo(_elem);
        }
        page = _elem.parent().find('.page');
        if (showPage && !page.length) {
            _initPage();
        }
        if (fixed) {
            _grid.addClass('grid-fixed');
        }

        _initFooter();

        if (option.columns) {
            _this.setColumns(option.columns);
        }

        _initToolbar();
        _initQuery();
        _setGridTop();

        if (option.url && autoQuery) {
            _this.reload(option.where);
        }
    }

    function _initPage() {
        page = $('<div>').addClass('pager');
        var pn = $('<span>').addClass('pn').appendTo(page);

        function createPB(icon, title, callback) {
            $('<i>')
                .addClass(icon)
                .attr('title', title)
                .appendTo(pn)
                .on('click', callback);
        }

        createPB('fa fa-refresh', Language.Refresh, function () { _queryPage(_where.page); });
        createPB('fa fa-step-backward', Language.First, function () { _queryPage(1); });
        createPB('fa fa-caret-left', Language.Previous, function () { _queryPage(_where.page - 1); });
        createPB('fa fa-caret-right', Language.Next, function () { _queryPage(_where.page + 1); });
        createPB('fa fa-step-forward', Language.Last, function () { _queryPage(_pageCount); });

        var pm = $('<span>').addClass('pm').appendTo(page);
        pm.append(Language.PagerNavTo);
        $('<input>')
            .attr('type', 'text')
            .addClass('pi')
            .appendTo(pm)
            .on('change', function () {
                _queryPage($(this).val());
            });
        pm.append('/');
        $('<span>').addClass('pc').appendTo(pm);
        pm.append(Language.PagerPerPage);

        var ps = $('<select>')
            .addClass('ps')
            .appendTo(pm)
            .on('change', function () {
                _where.limit = $(this).val();
                _queryPage(1);
            });
        $(pageSizes).each(function (i, d) {
            $('<option>').attr('value', d).html(d).appendTo(ps);
        });
        var totalText = Utils.format(Language.PagerTotal, '<span class="pt"></span>');
        pm.append(totalText);

        _grid.after(page);
        if (fixed) {
            page.addClass('pager-fixed');
            _grid.css({ bottom: '30px' });
        }
    }

    function _initFooter() {
        if (!option.footer)
            return;

        var footer = $('<div>')
            .addClass('grid-footer')
            .append(option.footer)
            .appendTo(_grid.parent());
        if (fixed) {
            footer.addClass('grid-footer-fixed');
            setTimeout(function () {
                var footerHeight = footer.outerHeight();
                if (page.length) {
                    page.css({ bottom: footerHeight + 'px' });
                    footerHeight += 30;
                }
                _grid.css({ bottom: footerHeight + 'px' });
            }, 100);
        }
    }

    function _initToolbar() {
        if (!toolButtons.length)
            return;

        var opt = {
            buttons: toolButtons,
            toolbar: option.toolbar,
            toolbarTips: option.toolbarTips,
            isTradition: option.isTradition,
            gridParameter: function () {
                var rows = _this.getSelected();
                return new GridManager(_this, option.form, rows);
            }
        };
        if (querys.length && option.isTradition) {
            opt.query = function () {
                if (_this.query.elem.hasClass('hide')) {
                    _this.query.elem.removeClass('hide').css({ paddingTop: 0 });
                } else {
                    _this.query.elem.addClass('hide').css({ paddingTop: '5px' });
                }
                _setGridTop();
            }
        }

        _this.toolbar = new Toolbar(opt);
        _grid.before(_this.toolbar.render());
    }

    function _initGridToolbar(td) {
        var opt = {
            buttons: columnButtons,
            toolbar: option.toolbar,
            isGridCell: true,
            onlyIcon: option.isIconButton,
            gridParameter: function () {
                var rows = [td.data(rowDataName)];
                return new GridManager(_this, option.form, rows);
            }
        };
        var toolbar = new Toolbar(opt);
        toolbar.render(td);
    }

    function _initQuery() {
        if (!querys.length)
            return;

        _this.query = new Query({
            id: _queryId,
            grid: _this,
            querys: querys,
            isTradition: option.isTradition,
            onSearch: option.onSearch
        });

        if (toolButtons.length && option.isTradition) {
            _this.query.elem.addClass('hide');
        }

        var existsExport = _this.columns.filter(function (d) { return d.export; });
        if (!toolButtons.length && (option.showBtnExport || existsExport.length > 0)) {
            Utils.createButton({
                icon: 'fa fa-sign-out', text: Language.Export, handler: function () {
                    var manger = new GridManager(_this, option.form);
                    manger.export();
                }
            }).appendTo(_this.query.elem);
        }

        _grid.before(_this.query.elem);
    }

    //init
    if (_elem.length) {
        _init();
    }
}