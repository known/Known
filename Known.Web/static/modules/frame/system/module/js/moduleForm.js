var ModuleForm = {

    option: null,
    toolbar: null,
    form: null,

    show: function (option) {
        this.option = option;
        this.toolbar = new Toolbar('tbModuleForm', this);
        this.form = new Form('formModule', {
            data: option.data
        });
    },

    save: function () {
        var _this = this;
        this.form.saveData({
            url: '/api/module/savemodule',
            callback: function () {
                _this.option.callback && _this.option.callback();
            }
        });
    },

    close: function () {
        Dialog.close();
    }
};