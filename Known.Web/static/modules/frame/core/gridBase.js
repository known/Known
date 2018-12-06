var GridBase = {

    option: null,
    grid: null,

    init: function (option) {
        this.option = option;
        new Toolbar(option.toolbarId, this);
        this.grid = new Grid(option.gridName);
    },

    showForm: function (data) {
        var _this = this, option = this.option;
        Dialog.show({
            name: option.formView,
            title: option.moduleName + '【' + (data.Id === '' ? '新增' : '编辑') + '】',
            width: 500, height: 250,
            callback: function () {
                option.form.show({
                    data: data,
                    callback: function () {
                        _this.grid.reload();
                        option.callback && option.callback();
                    }
                });
            }
        });
    },

    add: function () {
        this.showForm(this.option.formData);
    },

    edit: function () {
        var _this = this;
        this.grid.checkSelect(function (row) {
            _this.showForm(row);
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