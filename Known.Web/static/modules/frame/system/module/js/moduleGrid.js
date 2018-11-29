var ModuleGrid = {

    option: null,
    grid: null,
    toolbar: null,

    show: function (option) {
        this.option = option;
        this.grid = new Grid('Module');
        this.toolbar = new Toolbar('tbModule', this);
        this.grid.query.pid.setValue(option.pid);

        this.grid.load();
    },

    //toolbar
    refresh: function () {
        this.grid.reload();
    },

    showForm: function (data) {
        Dialog.show({
            name: 'System/Module/ModuleForm',
            title: '模块管理【' + (data.Id === '' ? '新增' : '编辑') + '】',
            callback: function () {

            }
        });
    },

    add: function () {
        this.showForm({ Id: '' });
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
        ModuleView.close();
    },

    search: function () {
        this.grid.search();
    }

};