var ModuleGrid = {

    option: null,
    toolbar: null,
    grid: null,

    show: function (option) {
        this.option = option;
        this.toolbar = new Toolbar('tbModule', this);
        this.grid = new Grid('Module');
        this.grid.query.pid.setValue(option.pid);

        this.grid.load();
    },

    //toolbar
    refresh: function () {
        this.grid.reload();
    },

    add: function () {
        this._showForm({ Id: '' });
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
        ModuleView.close();
    },

    search: function () {
        this.grid.search();
    },

    //private
    _showForm: function (data) {
        var _this = this;
        Dialog.show({
            name: 'System/Module/ModuleForm',
            title: '模块管理【' + (data.Id === '' ? '新增' : '编辑') + '】',
            callback: function () {
                ModuleForm.show({
                    data: data,
                    callback: _this.refresh
                });
            }
        });
    }

};