var DemoGrid = {
    grid: null,
    toolbar: null,

    show: function () {
        this.grid = new Grid('Demo');
        this.toolbar = new Toolbar('tbGrid', this);

        $('.query-btn-adv').click(function () {
            $('.query').toggle(0, function () {
                mini.layout();
            });
        });


        this.grid.load();
    },

    //toolbar
    refresh: function () {
        this.grid.reload();
    },

    addForm: function () {
        Message.alert('AddForm');
    },

    addHlForm: function () {
        Message.alert('AddHlForm');
    },

    addTabForm: function () {
        Message.alert('AddTabForm');
    },

    edit: function () {
    },

    delete: function () {
    },

    import: function () {
        Message.alert('Import');
    },

    exportExcel: function () {

    },

    exportPdf: function () {
    },

    upload: function () {
        Message.alert('Upload');
    },

    download: function () {
        Message.alert('Download');
    },

    sync: function () {
        Message.alert('Sync');
    },
    
    close: function () {
        DemoView.close();
    },

    search: function () {
        this.grid.search();
    }

};