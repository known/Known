var GridBase = {

    option: null,
    grid: null,

    add: function () {
        this._showForm({
            Id: '',
            ParentId: this.option.pid,
            Sort: this.grid.getData().length + 1,
            Enabled: 'Y'
        });
    },

    edit: function () {
        var _this = this;
        this.grid.checkSelect(function (row) {
            _this._showForm(row);
        });
    },

    remove: function () {
        this.grid.deleteRows(function (rows) {
        });
    },

    close: function () {
        window.CloseOwnerWindow();
    }

};

var FormBase = {

};