///////////////////////////////////////////////////////////////////////
var Grid = function (name, options) {
    this.name = name;
    this.grid = mini.get('grid' + name);
    if ($('#query' + name).length) {
        this.query = new Form('query' + name);
    }
    this.options = $.extend(true, {}, this.options, options);

    var url = this.grid.getUrl();
    this.grid.setUrl('/api/query?url=' + url);
    this.grid.setAjaxType('post');
};

Grid.prototype = {

    options: {
    },

    _queryData: function (isLoad, callback) {
        var query = this.query ? this.query.getData(true) : '';
        var grid = this.grid;
        grid.clearSelect(false);
        grid.load(
            { query: query, isLoad: isLoad },
            function (e) {
                if (callback) {
                    callback({ sender: grid, result: e.result });
                }
            },
            function () {
                Message.tips({ content: '查询出错！' });
            }
        );
        new ColumnsMenu(grid);
    },

    search: function (callback) {
        this._queryData('0', callback);
    },

    load: function (callback) {
        this._queryData('1', callback);
    },

    reload: function () {
        this.grid.reload();
    },

    validate: function (tabsId, tabIndex) {
        this.grid.validate();
        if (this.grid.isValid())
            return true;

        if (tabsId) {
            var tabs = mini.get(tabsId);
            var tab = tabs.getTab(index);
            tabs.activeTab(tab);
        }

        var error = this.grid.getCellErrors()[0];
        this.grid.beginEditCell(error.record, error.column);
        return false;
    },

    getChanges: function (encode) {
        var data = this.grid.getChanges();
        return encode ? mini.encode(data) : data;
    },

    getSelecteds: function (encode) {
        this.grid.accept();
        var data = this.grid.getSelecteds();
        return encode ? mini.encode(data) : data;
    },

    getData: function (encode) {
        var data = this.grid.getData();
        return encode ? mini.encode(data) : data;
    },

    setData: function (data, callback) {
        this.clear();
        if (data) {
            this.grid.setData(data);
            callback && callback(data);
        }
    },

    clear: function () {
        this.grid.setData([]);
    },

    addRow: function (data, index) {
        if (!index) {
            index = this.grid.getData().length;
        }

        this.grid.addRow(data, index);
        this.grid.cancelEdit();
        this.grid.beginEditRow(data);
    },

    updateRow: function (e, data) {
        e.sender.updateRow(e.record, data);
    },

    deleteRow: function (uid) {
        var row = this.grid.getRowByUid(uid);
        if (row) {
            this.grid.removeRow(row);
        }
    },

    checkSelect: function (callback) {
        var rows = this.grid.getSelecteds();
        if (rows.length === 0)
            Message.tips({ content: '请选择一条记录！' });
        else if (rows.length > 1)
            Message.tips({ content: '只能选择一条记录！' });
        else if (callback)
            callback(rows[0]);
    },

    checkMultiSelect: function (callback) {
        var rows = this.grid.getSelecteds();
        if (rows.length === 0)
            Message.tips({ content: '请选择一条或多条记录！' });
        else if (callback)
            callback(rows);
    },

    deleteRows: function (callback) {
        this.checkMultiSelect(function (rows) {
            Message.confirm('确定要删除选中的' + rows.length + '条记录？', function () {
                callback && callback(rows);
            });
        });
    },

    hideColumn: function (indexOrName) {
        var column = this.grid.getColumn(indexOrName);
        this.grid.updateColumn(column, { visible: false });
    },

    showColumn: function (indexOrName) {
        var column = this.grid.getColumn(indexOrName);
        this.grid.updateColumn(column, { visible: true });
    },

    setColumns: function (columns) {
        this.grid.setColumns(columns);
    }

};