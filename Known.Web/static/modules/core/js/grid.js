///////////////////////////////////////////////////////////////////////
var Grid = function (name, options) {
    this.name = name;
    this.grid = mini.get('grid' + name);
    this.query = new Form('query' + name);
    this.options = $.extend(true, {}, this.options, options);
}

Grid.prototype = {

    options: {
    },

    _queryData: function (isLoad, callback) {
        var query = this.query.getData(true);
        this.grid.clearSelect(false);
        this.grid.load(
            { query: query, isLoad: isLoad },
            function (e) {
                if (callback) {
                    var data = e.sender.getResultObject();
                    callback(data);
                }
            },
            function () {
                Message.tips({ content: '查询出错！' });
            }
        );
        new ColumnsMenu(this.grid);
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

    getData: function (encode) {
        var data = this.grid.getData();
        return encode ? mini.encode(data) : data;
    },

    setData: function (data, callback) {
        if (data) {
            this.grid.setData(data);
            callback && callback(data);
        }
    }

};