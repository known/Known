var Grid = {
    get: function (name) {
        if (typeof name === 'string')
            return mini.get('grid' + name);

        return name;
    },
    _query: function (name, isLoad, callback) {
        var ctl = this.get(name);
        var query = Form.getData('query' + name, true);
        ctl.clearSelect(false);
        ctl.load(
            { query: query, isLoad: isLoad },
            function () {
                if (callback) {
                    var data = ctl.getResultObject();
                    callback(data);
                }
            },
            function () {
                Message.tips({ content: '查询出错！' });
            }
        );
        var menu = new ColumnsMenu(ctl);
        return ctl;
    },
    search: function (name, callback) {
        return this._query(name, '0', callback);
    },
    load: function (name, callback) {
        return this._query(name, '1', callback);
    },
    reload: function (grid) {
        this.get(grid).reload();
    },
    validate: function (grid, tabsId, tabIndex) {
        var ctl = this.get(grid);
        ctl.validate();
        if (ctl.isValid())
            return true;

        if (tabsId) {
            var tabs = mini.get(tabsId);
            var tab = tabs.getTab(index);
            tabs.activeTab(tab);
        }

        var error = ctl.getCellErrors()[0];
        ctl.beginEditCell(error.record, error.column);
        return false;
    },
    getData: function (grid, encode) {
        var ctl = this.get(grid);
        var data = ctl.getData();
        return encode ? mini.encode(data) : data;
    },
    setData: function (grid, data, callback) {
        var ctl = this.get(grid);
        if (data) {
            ctl.setData(data);
            callback && callback(data);
        }
    }
};