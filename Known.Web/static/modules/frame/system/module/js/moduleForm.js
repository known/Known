var ModuleForm = {

    option: null,
    toolbar: null,
    form: null,

    show: function (option) {
        this.option = option;
        this.toolbar = new Toolbar('tbModuleForm', this);
        this.form = new Form('formModule');
    },

    save: function () {

    },

    close: function () {
        Dialog.close();
    }
};