function Grid(option) {
    //fields
    var _option = option,
        _elem = $('<div>').addClass('grid'),
        _columns = [],
        _columnLength = 0,
        _query, _thead, _tbody,
        _this = this;
    var action = $.extend({
        remove: function (e) { e.grid.removeRow(e.index); }
    }, option.action || {});

    //properties
    this.elem = _elem;
    this.columns = _columns;
    this.data = [];
    this.where = _option.where || {};

    //methods
    this.render = function () {
        _elem.html('');
        _init();
        return _elem;
    }

    this.setColumns = function (columns) {
        _setColumns(columns);
    }

    this.getData = function () {
        return _this.data;
    }

    this.setData = function (data) {
        _setData(data);
    }

    this.reload = function (where) {
        var query = _query ? _query.getData() : {};
        $.extend(_this.where, query, where || {});
        _queryData();
    }

    this.removeRow = function (index) {
        _tbody.find('tr:eq(' + index + ')').remove();
        this.data.splice(index, 1);
    }

    //pricate
    function _init() {
        if (_option.querys && _option.querys.length) {
            _query = _createQuery(_option);
        }

        var table = $('<table>').appendTo(_elem);
        _thead = $('<thead>').appendTo(table);

        if (_option.columns && _option.columns.length) {
            _setColumns(_option.columns);
        }

        _tbody = $('<tbody>').appendTo(table);

        if (_option.url && _option.autoQuery === undefined) {
            _queryData();
        }
    }

    function _queryData() {
        _setBodyMessage(Language.Loading + '......', 'load');
        $.post(_option.url, _this.where, function (res) {
            var data = [];
            if ($.isArray(res)) {
                data = res;
            } else {
                if (!res.IsValid) {
                    _setBodyMessage(res.Message, 'error');
                } else {
                    data = res.Data;
                }
            }

            if (_option.setData) {
                _this.data = data;
                _option.setData({ grid: _this, data });
            } else {
                _this.setData(data);
            }
        });
    }

    function _createQuery(option) {
        var queryEl = $('<div>').addClass('query').appendTo(_elem);
        var query = new Query(option);
        query.render().appendTo(queryEl);
        return query;
    }

    function _setColumns(columns) {
        _thead.html('');
        var tr = $('<tr>').appendTo(_thead);
        _columns = columns;
        _columnLength = columns.length;
        for (var i = 0; i < columns.length; i++) {
            var d = columns[i];
            if (d.visible !== undefined && !d.visible)
                continue;

            $('<th>').html(d.title).appendTo(tr);
        }
    }

    function _setData(data) {
        _this.data = data || [];
        _tbody.html('');
        if (data && data.length) {
            for (var i = 0; i < data.length; i++) {
                _setDataRow(data[i]);
            }
        } else {
            _setBodyMessage(Language.NoDataFound, 'error');
        }
    }

    function _setDataRow(row) {
        var tr = $('<tr>').appendTo(_tbody);
        for (var i = 0; i < _columns.length; i++) {
            var column = _columns[i];
            if (column.visible !== undefined && !column.visible)
                continue;

            var td = $('<td>').appendTo(tr);
            if (column.format) {
                var tdc = column.format(row, { tr, td, column });
                td.append(tdc);
            } else if (column.actionFormat) {
                var tdc = column.actionFormat(row, {
                    index: i, tr: tr, td: td, column: column
                });
                td.append(tdc);
                tr.find('[action]').on('click', function (e) {
                    var code = $(this).attr('action');
                    var trObj = $(this).parentsUntil('tbody').last();
                    var trIdx = _tbody.find('tr').index(trObj);
                    var row = _this.data[trIdx];
                    action[code] && action[code].call(this, {
                        grid: _this, row: row, index: trIdx
                    });
                });
            } else if (column.field) {
                var text = row[column.field];
                td.append(text);
            }
        }
    }

    function _setBodyMessage(message, cls) {
        _tbody.html('');
        var tr = $('<tr>').appendTo(_tbody);
        $('<td>').attr('colspan', _columnLength).addClass(cls).html(message).appendTo(tr);
    }
}