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

    add: function () {
        Message.alert('AddForm');
    },

    edit: function () {
        this.grid.checkSelect(function (row) {
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